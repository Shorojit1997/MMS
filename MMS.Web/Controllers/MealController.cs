using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MMS.DataService.IConfiguration;
using MMS.Entities.Dtos.Incomming;

namespace MMS.Web.Controllers
{

    [Authorize]
    public class MealController : BaseController
    {
        public MealController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<IActionResult> MyAttendance(string MonthId,string MessId)
        {
            try
            {
                if(MonthId == null || MessId==null)
                {
                    throw new Exception("Invalid Route");
                }
                var id = HttpContext.User.Identity.Name;

                var days= await _unitOfWork.Days.GetDaysByMonthIdAndPersonId(Guid.Parse(MonthId),Guid.Parse(id));
                ViewBag.Days= days;
                ViewBag.MonthId = MonthId;
                ViewBag.MessId = MessId;
                return View();
            }
            catch(Exception ex)
            {
                TempData["error"]=ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }



        public async Task<IActionResult> AllMembersAttendance(string MonthId, string MessId)
        {
            try
            {
                if (MonthId == null || MessId == null)
                {
                    throw new Exception("Invalid Route");
                }
                var id = HttpContext.User.Identity.Name;

                var AllDetails = await _unitOfWork.Days.GetDaysByMonthId(Guid.Parse(MonthId));
                ViewBag.AllMembers = AllDetails;
                ViewBag.MonthId = MonthId;
                ViewBag.MessId = MessId;

                return View();
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatus(string DaysId)
        {
            try
            {
                if (DaysId == null)
                {
                    throw new Exception("Invalid Route");
                }
                var id = HttpContext.User.Identity.Name;

                var day = await _unitOfWork.Days.GetById(Guid.Parse(DaysId));
                if (day == null) throw new Exception("Invalid Route");

                
                day.Breakfast = day.Breakfast > 0 ? 0 : 1;
                day.Lunch = day.Lunch > 0 ? 0 : 1;
                day.Dinner = day.Dinner > 0 ? 0 : 1;
                day.UpdatedAt= DateTime.Now;

                _unitOfWork.Days.Update(day);
                await _unitOfWork.CompleteAsync();

                return Ok(new DayResponseDTO()
                {
                    Success = true,
                    Days= day
                });
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return BadRequest(new DayResponseDTO()
                {
                    Success = false
                });
            }
        }
    }
}
