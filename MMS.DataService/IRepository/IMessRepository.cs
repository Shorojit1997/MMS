using MMS.Entities.DbSet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.DataService.IRepository
{
    public interface IMessRepository:IGenericRepository<Mess>
    {
        Task<IEnumerable<Mess>> GetByPersonId(Guid Id);

        Task<bool> DeleteMessByMessId(Guid Id);
    }
}
