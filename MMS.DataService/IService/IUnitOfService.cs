using MMS.DataService.IRepository;
using MMS.DataService.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.DataService.Service
{
    public interface IUnitOfService
    {
        IProfileService ProfileService { get; }
        IMealService MealService { get; }

        IDashboardServices DashboardService { get; }

        IAuthService AuthService { get; }
    }
}
