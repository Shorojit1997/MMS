using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MMS.DataService.Data;
using MMS.DataService.IConfiguration;
using MMS.Entities.DbSet;
using MMS.Entities.Dtos.Incomming;

namespace MMS.Web.Controllers
{
    [Authorize]
    public class DashboardController : BaseController
    {

        public DashboardController(
            IUnitOfWork unitOfWork
            ) : base(unitOfWork)
        {
         
        }


        [HttpPost]
        public async Task<IActionResult> Create(MessRequestDTO mess)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }
            var Id = HttpContext.User.Identity.Name;
            var newMess = new Mess()
            {
                Name = mess.Name,
                StartDate = mess.StartDate,
                UpdateDate = DateTime.Now,
            };

            var personAddMess = new MessHaveMember()
            {
                IsManager = true,
                PersonId=Guid.Parse(Id),
                MessId=newMess.Id,
                UpdateDate = DateTime.Now,
            };

            try
            {
                await _unitOfWork.Messes.Add(newMess);
                await _unitOfWork.MessHaveMembers.Add(personAddMess);
                await _unitOfWork.CompleteAsync();
                return RedirectToAction("ShowHistory", "Dashboard");

            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Error");
            }

        }



        public async Task<IActionResult> ShowHistory()
        {
            var Id = HttpContext.User.Identity.Name;
            var person = await _unitOfWork.Persons.GetById(Guid.Parse(Id));
            var messes = await _unitOfWork.MessHaveMembers.GetByPersonId(Guid.Parse( Id));

            ViewBag.Messes= messes;
            ViewBag.Name=person.Name;
            return View();
        }



        public async Task<IActionResult> MessMembers(string Id)
        {
            if(Id == null)
            {
                return RedirectToAction("ShowHistory", "Dashboard");
            }

            ViewBag.Id = Id;
            var members = await _unitOfWork.MessHaveMembers.GetAllMembersByMessId(Guid.Parse(Id));
            if (!members.Any()) return RedirectToAction("ShowHistory", "Dashboard");
            ViewBag.Members= members;

            var userId = HttpContext.User.Identity.Name;
            var isMember =members.FirstOrDefault(e => e.Id ==Guid.Parse( userId));
            if (isMember==null) return RedirectToAction("ShowHistory", "Dashboard");

            return View();
        }



        public async Task<IActionResult> FindMembers(string query,string types)
        {
            if (query == null) return RedirectToAction("ShowHistory", "Dashboard");

            var members = await _unitOfWork.Persons.GetAllMembersByQueryParameters(query, types);
            return Ok(members);
        }



        public async Task<IActionResult> AddIntoMess(string MessId, string UserId)
        {
            if(MessId == null|| UserId==null) {
                TempData["Error"] = "Something happend wrong. Please try again";
                return RedirectToAction("ShowHistory", "Dashboard");
            }

            var addIntoMess = new MessHaveMember()
            {
                PersonId=Guid.Parse(UserId),
                MessId=Guid.Parse(MessId),
                IsManager=false,
                UpdateDate=DateTime.Now,
            };

            await _unitOfWork.MessHaveMembers.Add(addIntoMess);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction("MessMembers","Dashboard",new {Id=MessId});
        }

        public async Task<IActionResult> RemoveFromMess(string MessId, string UserId)
        {
            if (MessId == null || UserId == null)
            {
                TempData["Error"] = "Something happend wrong. Please try again";
                return RedirectToAction("ShowHistory", "Dashboard");
            }

            var currentUserId = HttpContext.User.Identity.Name;

            await _unitOfWork.MessHaveMembers.RemoveByMessIdAndPersonId(Guid.Parse(MessId),Guid.Parse(UserId),Guid.Parse(currentUserId));
            await _unitOfWork.CompleteAsync();

            return RedirectToAction("MessMembers", "Dashboard", new { Id = MessId });
        }

    }
}
