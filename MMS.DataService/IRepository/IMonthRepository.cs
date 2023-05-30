using MMS.Entities.DbSet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.DataService.IRepository
{
    public interface IMonthRepository:IGenericRepository<Month>
    {
        Task<IEnumerable<Month>> GetMonthsByMessId(Guid MessId);
    }
}
