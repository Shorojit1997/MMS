using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MMS.DataService.IConfiguration;

namespace MMS.Web.Controllers
{
 
    public class MealController : BaseController
    {
        public MealController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IActionResult MyAttendance()
        {

            return View();
        }
    }
}
