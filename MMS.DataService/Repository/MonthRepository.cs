using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MMS.DataService.Data;
using MMS.DataService.IRepository;
using MMS.Entities.DbSet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.DataService.Repository
{
    public class MonthRepository: GenericRepository<Month>, IMonthRepository
    {
        public MonthRepository(AppDbContext context, ILogger logger) : base(context, logger)
        {

        }

        public async Task<bool> AddRange(List<Days> days)
        {
            return true;
        }

        public async Task<Month> GetLastMonthByMessId(Guid MessId)
        {
            var months=await dbset.Where(e=>e.MessId==MessId).OrderByDescending(e=>e.CreatedAt).ToListAsync();
            if(months.Any())
            {
                return months.First();
            }
            return null;
        }

        public async Task<IEnumerable<Month>> GetMonthsByMessId(Guid MessId)
        {
           return await dbset.Where(e=> e.MessId==MessId).ToListAsync();
        }
    }
}
