using Microsoft.Extensions.Logging;
using MMS.DataService.Data;
using MMS.DataService.IRepository;
using MMS.Entities.DbSet;
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
    }
}
