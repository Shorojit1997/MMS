using MMS.Entities.Dtos.Incomming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.DataService.IService
{
    public interface IDashboardServices
    {
        Task<bool> Create(MessRequestDTO mess, string PersonId);

        Task<bool> EditMessHistory(MessRequestDTO mess);

        Task<bool> AddIntoMess(string MessId, string UserId,string type, string curUserId);
    }
}
