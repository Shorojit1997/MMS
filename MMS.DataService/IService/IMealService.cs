using MMS.Entities.DbSet;
using MMS.Entities.Dtos.Incomming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.DataService.IService
{
    public interface IMealService
    {
        Task<Days> ChangeStatus(string dayId);
        Task<bool> CloseTheDay(string MonthId, string MessId, string DayNo, string id);

        Task<Days> UpdateMealStatus(string DayId, int BreakFast, int Lunch, int Dinner);
    }
}
