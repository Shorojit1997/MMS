using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MMS.DataService.IRepository;
using MMS.DataService.Service;
using MMS.Entities.Dtos.Incomming;

namespace MMS.Web.Controllers
{

    [Authorize]
    public class MealController : BaseController
    {
        public MealController(IUnitOfWork unitOfWork, IUnitOfService unitOfService) : base(unitOfWork, unitOfService)
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
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Error");
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

                var AllDetails = await _unitOfWork.Days.GetDaysByMonthId(Guid.Parse(MonthId),Guid.Parse(MessId),Guid.Parse(id));
                ViewBag.AllMembers = AllDetails;
                ViewBag.MonthId = MonthId;
                ViewBag.MessId = MessId;
                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatus(string DaysId)
        {
            try
            {
                var day = await _unitOfService.MealService.ChangeStatus(DaysId);
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
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        public async Task<IActionResult> CloseTheDay(string MonthId,string MessId, string DayNo)
        {
            try
            {
                var id = HttpContext.User.Identity.Name;
                await _unitOfService.MealService.CloseTheDay(MonthId, MessId, DayNo,id);
                return RedirectToAction("AllMembersAttendance", "Meal", new { MonthId = MonthId, MessId = MessId });
            }
            catch(Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Error");
            }
        }




        [HttpPost]
        public async Task<IActionResult> UpdateMealStatus(string DayId,int BreakFast = 0,int Lunch = 0, int Dinner = 0)
        {
            try
            {
                if (DayId == null)
                {
                    throw new Exception("Invalid Route");
                }
                var id = HttpContext.User.Identity.Name;
                var day=await _unitOfService.MealService.UpdateMealStatus( DayId, BreakFast,  Lunch, Dinner);
                return Ok(new DayResponseDTO()
                {
                    Success = true,
                    Days = day
                });


            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return BadRequest(new DayResponseDTO()
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }


    }
}
