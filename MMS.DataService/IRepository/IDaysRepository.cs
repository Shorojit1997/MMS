using MMS.Entities.DbSet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.DataService.IRepository
{
    public interface IDaysRepository:IGenericRepository<Days>
    {
        Task<bool> AddRange(IEnumerable<Days> days); 
    }
}
