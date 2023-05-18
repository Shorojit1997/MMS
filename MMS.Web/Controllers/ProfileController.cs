using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MMS.Authentication.Configuration;
using MMS.DataService.IConfiguration;

namespace MMS.Web.Controllers
{
    [Authorize]
    public class ProfileController : BaseController
    {
        private readonly UserManager<IdentityUser> _userManager;
        public ProfileController(
            IUnitOfWork unitOfWork,
            UserManager<IdentityUser> userManager,
            IOptionsMonitor<JwtConfig> optionsMonitor
            ) : base(unitOfWork)
        {
            _userManager = userManager;
        }

        public IActionResult Details()
        {
            return View();
        }
    }
}
