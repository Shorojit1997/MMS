using MMS.Entities.Dtos.Incomming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.DataService.IService
{
    public interface IAccountService
    {
        Task<AccountDTO> GetDetailsServices(string PersonId);
        Task<bool> Edit_Accounts(AccountDTO account, string PersonId);
    }
}
