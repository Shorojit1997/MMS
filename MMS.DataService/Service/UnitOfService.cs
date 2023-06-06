using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MMS.Authentication.IService;
using MMS.DataService.Data;
using MMS.DataService.IRepository;
using MMS.DataService.IService;
using MMS.DataService.Repository;
using MMS.Entities.DbSet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.DataService.Service
{
    public class UnitOfService :IUnitOfService
    {
        private readonly IUnitOfWork _unitOfWork;
        public IProfileService ProfileService { get; private set; }
        public IMealService MealService { get; private set; }

        public IDashboardServices DashboardService { get; private set; }

        public IAuthService AuthService { get; private set; }

        public UnitOfService(
            IUnitOfWork unitOfWork,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signManager,
            IEmailService emailService
            )
        {
            _unitOfWork = unitOfWork;
            ProfileService = new ProfileService(unitOfWork,userManager,signManager);
            MealService = new MealService(unitOfWork);
            DashboardService=new DashboardService(unitOfWork);
            AuthService = new AuthService(unitOfWork,userManager, emailService,signManager);
        }




    }

}
