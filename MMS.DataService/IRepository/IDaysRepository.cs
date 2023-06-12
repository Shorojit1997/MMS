using MMS.Entities.DbSet;
using MMS.Entities.Dtos.Incomming;
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
        Task<List<Days>> GetDaysByMonthIdAndPersonId(Guid MonthId,Guid PersonId);
        Task<List<PersonDTO>> GetDaysByMonthId(Guid MonthId,Guid MessId,Guid PersonId);

        Task<bool> ChangeDayStatusByMonthIdAndDayNo(Guid MonthId,int DayNo);

        Task<CurrentDayCalculationDTO> GetTodaysMealCountByMonthId(Guid MonthId);
    }
}
