using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MMS.DataService.Data;
using MMS.DataService.IRepository;
using MMS.Entities.DbSet;
using MMS.Entities.Dtos.Incomming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MMS.DataService.Repository
{
    public class DaysRepository : GenericRepository<Days>, IDaysRepository
    {
        public DaysRepository(AppDbContext context, ILogger logger) : base(context, logger)
        {

        }

        public async Task<bool> AddRange(IEnumerable<Days> days)
        {
            try
            {
                await dbset.AddRangeAsync(days);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> ChangeDayStatusByMonthIdAndDayNo(Guid MonthId, int DayNo)
        {
            try
            {
                var days = await dbset.Where(e => e.Month_Id == MonthId && e.Number == DayNo).ToListAsync();
                foreach (var day in days)
                {
                    day.IsEnd = !day.IsEnd;
                }
                dbset.UpdateRange(days);
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public async Task<List<PersonDTO>> GetDaysByMonthId(Guid MonthId, Guid MessId, Guid PersonId)
        {
            //finding all of the days based on the monthId 
            var days= await dbset
               .Where(e => e.Month_Id == MonthId)
               .OrderBy(a => a.Person_Id)
               .OrderBy(b=>b.Number)
               .ToListAsync();


            //finding the all of person_id according to this mess
            var uniquePersonIds = days.Select(item => item.Person_Id).Distinct().ToList();
            var allPersons = await _context.Persons
                           .Where(item => uniquePersonIds.Contains(item.Id))
                           .Select(item=>new PersonDTO()
                           {
                               Name = item.Name,
                               Id = item.Id,
                           })
                           .ToListAsync();


            //adding day of in each person
            foreach (var day in days)
            {
                foreach(var person in allPersons)
                {
                    if(day.Person_Id== person.Id)
                    {
                        person.Days.Add(day);
                        break;
                    }
                }
            }


            //finding totalDeposit based on the MessId and groupby the messId
            var deposits = await _context.Deposits
                        .Where(e => e.MessId == MessId && e.Success == true)
                        .GroupBy(b => b.PersonId)
                        .Select(g => new
                                      {
                                         PersonId = g.Key,
                                          TotalAmount = g.Sum(a => a.Amount)
                                       })
                          .ToListAsync();

            foreach(var person in allPersons)
            {
                 foreach( var deposit in deposits)
                 {
                    if (deposit.PersonId == person.Id)
                    {
                        person.Balance = deposit.TotalAmount>0 ?deposit.TotalAmount:person.Balance;
                        break;
                    }
                }
            }
            //finding individual total meal
           var individualTotalMeal = await _context.Days
                       .Where(e => e.Month_Id == MonthId && e.IsEnd == true)
                       .GroupBy(b => b.Person_Id)
                       .Select(g => new
                       {
                           PersonId = g.Key,
                           TotalMeal = g.Sum(a => a.Breakfast+a.Lunch+a.Dinner)
                       })
                         .ToListAsync();


            //counting total meal of this month
            var totalMeal = individualTotalMeal.Sum(a => a.TotalMeal);
            var totalCost = await _context.Expenses
                        .Where(e => e.MessId == MessId && e.MonthId == MonthId)
                        .SumAsync(b => b.Amount);

            var mealRate = 0.00;
            if(totalMeal>0)
                mealRate= totalCost / totalMeal;
            

            foreach (var person in allPersons)
            {
                foreach (var meal in individualTotalMeal)
                {
                    if (meal.PersonId == person.Id)
                    {
                        person.TotalMeal = meal.TotalMeal;
                        person.TotalCost=Convert.ToDouble( meal.TotalMeal*mealRate);
                        break;
                    }
                }
            }
  

            return allPersons;
        }

        public async Task<List<Days>> GetDaysByMonthIdAndPersonId(Guid MonthId, Guid PersonId)
        {
            return await dbset
                .Where(e => e.Person_Id == PersonId && e.Month_Id == MonthId)
                .OrderBy(a=>a.Number)
                .ToListAsync();
        }
    }
}
