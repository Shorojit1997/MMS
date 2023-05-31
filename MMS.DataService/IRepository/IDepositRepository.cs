using MMS.Entities.DbSet;
using MMS.Entities.Dtos.Incomming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.DataService.IRepository
{
    public interface IDepositRepository:IGenericRepository<Deposit>
    {
        Task<Deposit> GetByOrderId(string id);
        public Task<List<Deposit>> GetTransactionsByMessIdAndPersonId(Guid MessId, Guid PersonId);
        public Task<List<DepositDTO>> GetTransactionsByMessId(Guid MessId);

        public Task< List<DepositDTO> > GetTransactionsByPersonId(Guid PersonId);
    }
}
