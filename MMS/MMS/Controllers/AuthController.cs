using Microsoft.AspNetCore.Mvc;

namespace MMS.Controllers
{
    public class AuthController : BaseController
    {
        public IActionResult Login()
        {
            return View();
        }
    }
}
