using Microsoft.AspNetCore.Mvc;

namespace MMS.Web.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
