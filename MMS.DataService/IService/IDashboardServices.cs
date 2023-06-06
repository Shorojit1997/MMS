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

        Task<bool> DeleteMessHistory(string MessId,string PersonId);

        Task<bool> AddIntoMess(string MessId, string UserId,string type, string curUserId);

        Task<bool> CreateNewMonth(MonthDTO monthDetails,string id);

        Task<Guid> DeleteMonthHistory(string MonthId, string PersonId);
        Task<MonthDTO> EditMonthHistory(string MessId, string MonthId);

        Task<MonthDTO> EditMonthHistoryForPostController(MonthDTO month, string PersonId);

    }
}
