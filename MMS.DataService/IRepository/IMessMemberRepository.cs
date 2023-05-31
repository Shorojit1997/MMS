using MMS.Entities.DbSet;
using MMS.Entities.Dtos.Incomming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.DataService.IRepository
{
    public interface IMessMemberRepository:IGenericRepository<MessHaveMember>
    {
        Task<IEnumerable<Mess>> GetByPersonId(Guid personId);
        Task<IEnumerable<PersonDTO>> GetAllMembersByMessId(Guid Id);

        Task<MessHaveMember> GetByMessIdAndPersonId(Guid messId, Guid personId);

        Task<bool> RemoveByMessIdAndPersonId(Guid messId,Guid personId, Guid currentPersonId);
        Task<IEnumerable<Guid>> GetAllManagersDetailsByMessId(Guid messId);

    }
}
