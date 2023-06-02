using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MMS.DataService.Data;
using MMS.DataService.IRepository;
using MMS.Entities.DbSet;
using MMS.Entities.Dtos.Incomming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.DataService.Repository
{
    public class ExpensesRepository : GenericRepository<Expense>, IExpensesRepository
    {
        public ExpensesRepository(AppDbContext context, ILogger logger) : base(context, logger)
        {
        }

        public async Task<IEnumerable<ExpenseDTO>> GetExpensesByMonthId(Guid MonthId)
        {
            return await dbset
                .Where(e=>e.MonthId==MonthId)
                .OrderByDescending(e => e.CreatedAt)
                .Select(item=> new ExpenseDTO()
                {
                    Id = item.Id,
                    Title = item.Title,
                    Description = item.Description,
                    Amount = item.Amount,
                    MessId = item.MessId,
                })
                .ToListAsync();
        }
    }
}
