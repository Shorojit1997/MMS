using Microsoft.AspNetCore.Mvc;
using MMS.DataService.IConfiguration;

namespace MMS.Controllers
{
    public class AuthController : BaseController
    {
        public IUnitOfWork _uniOfWork;
        public IActionResult Login()
        {
            return View();
        }
    }
}
