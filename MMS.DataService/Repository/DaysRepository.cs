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

        public async Task<List<PersonDTO>> GetDaysByMonthId(Guid MonthId)
        {
            var days= await dbset
               .Where(e => e.Month_Id == MonthId)
               .OrderBy(a => a.Person_Id)
               .OrderBy(b=>b.Number)
               .ToListAsync();

            var uniquePersonIds = days.Select(item => item.Person_Id).Distinct().ToList();
            var allPersons = await _context.Persons
                           .Where(item => uniquePersonIds.Contains(item.Id))
                           .Select(item=>new PersonDTO()
                           {
                               Name = item.Name,
                               Id = item.Id,
                           })
                           .ToListAsync();
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
