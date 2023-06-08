using MMS.DataService.Paypal;
using MMS.Entities.DbSet;
using MMS.Entities.Dtos.Incomming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.DataService.IService
{
    public interface IPaypalService
    {
        Task<bool> SetClientSecret(Guid MessId, string id);
        Task<CreateOrderResponse> CreateOrderService(DepositDTO deposit, string PersonId);
        Task<Deposit> SaveOrderService(CreateOrderResponse response,DepositDTO deposit, string PersonId);
    }
}
