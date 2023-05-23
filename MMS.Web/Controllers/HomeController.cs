using Microsoft.AspNetCore.Mvc;
using MMS.DataService.IConfiguration;

namespace MMS.Web.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(IUnitOfWork unitOfWork):base(unitOfWork) { }
        public IActionResult Index()
        {
            var user = HttpContext.User;
            string username = user.Identity.Name;
            return View();
        }
    }
}
