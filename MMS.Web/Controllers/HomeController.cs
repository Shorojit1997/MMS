using Microsoft.AspNetCore.Mvc;
using MMS.DataService.IRepository;
using MMS.DataService.Service;

namespace MMS.Web.Controllers
{
    public class HomeController : BaseController
    {

        public HomeController(IUnitOfWork unitOfWork, IUnitOfService unitOfService) :base(unitOfWork, unitOfService) { }
        public IActionResult Index()
        {
            var user = HttpContext.User;
            string username = user.Identity.Name;
            return View();
        }
    }
}
