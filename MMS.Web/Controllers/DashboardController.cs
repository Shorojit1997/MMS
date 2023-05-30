using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MMS.DataService.Data;
using MMS.DataService.IConfiguration;
using MMS.DataService.Others;
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
                UpdatedAt = DateTime.Now,
            };

            var personAddMess = new MessHaveMember()
            {
                IsManager = true,
                PersonId=Guid.Parse(Id),
                MessId=newMess.Id,
                UpdatedAt = DateTime.Now,
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


        [HttpGet]
        public async Task<IActionResult> ShowHistory()
        {
            var Id = HttpContext.User.Identity.Name;
            var person = await _unitOfWork.Persons.GetById(Guid.Parse(Id));
            var messes = await _unitOfWork.MessHaveMembers.GetByPersonId(Guid.Parse( Id));

            ViewBag.Messes= messes;
            ViewBag.Name=person.Name;
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> DeleteMessHistory(string MessId)
        {
            if(string.IsNullOrEmpty(MessId) || !ValidityChecker.IsValidGuid(MessId))
            {
                TempData["Error"] = "Invalid request.";
                return RedirectToAction("ShowHistory", "Dashboard");
            }

            var Id = HttpContext.User.Identity.Name;
            var member = await _unitOfWork.MessHaveMembers
                .GetByMessIdAndPersonId(Guid.Parse(MessId), Guid.Parse(Id));

            if (member == null)
            {
                TempData["Error"] = "Somethings Happend wrong!";
                return RedirectToAction("ShowHistory", "Dashboard");
            }

            if (!member.IsManager)
            {
                TempData["Error"] = "Sorry you have't any permission!";
                return RedirectToAction("ShowHistory", "Dashboard");
            }

            await _unitOfWork.Messes.DeleteMessByMessId(Guid.Parse(MessId));
            await _unitOfWork.CompleteAsync();

            TempData["Success"] = "Successfully deleted";

            return RedirectToAction("ShowHistory","Dashboard");
        }


        [HttpGet]
        public async Task<IActionResult> EditMessHistory(string MessId)
        {
            if (string.IsNullOrEmpty(MessId)|| !ValidityChecker.IsValidGuid(MessId))
            {
                TempData["Error"] = "Something happend error";
                return RedirectToAction("ShowHistory", "Dashboard");
            }

            var mess = await _unitOfWork.Messes.GetById(Guid.Parse(MessId));
            if (mess == null)
            {
                TempData["Error"] = "Invalid Request";
                return RedirectToAction("ShowHistory", "Dashboard");
            }

            var requestDto = new MessRequestDTO()
            {
                Id= mess.Id,
                Name=mess.Name,
                StartDate=mess.StartDate,
            };

            return View(requestDto);
        }

        [HttpPost]
        public async Task<IActionResult> EditMessHistory(MessRequestDTO mess)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please provide valid informations";
                return RedirectToAction("ShowHistory", "Dashboard");
            }

            var id = HttpContext.User.Identity.Name;
            var details =await _unitOfWork.MessHaveMembers.GetByMessIdAndPersonId(mess.Id,Guid.Parse(id));

            if (details == null || !details.IsManager)
            {
                TempData["Error"] = "Sorry! You have not edit permission";
                return RedirectToAction("ShowHistory", "Dashboard");

            }

            var messDetails = await _unitOfWork.Messes.GetById(mess.Id);
            messDetails.Name = mess.Name??messDetails.Name;
            messDetails.StartDate = mess.StartDate??messDetails.StartDate;

            _unitOfWork.Messes.Update(messDetails);

            await _unitOfWork.CompleteAsync();

            TempData["Success"] = "Successfully Updated";
            return RedirectToAction("ShowHistory", "Dashboard");
        }


        [HttpGet]
        public async Task<IActionResult> MessMembers(string Id)
        {
            if(Id == null || !ValidityChecker.IsValidGuid(Id))
            {
                return RedirectToAction("ShowHistory", "Dashboard");
            }

            ViewBag.MessId = Id;
         
            var members = await _unitOfWork.MessHaveMembers.GetAllMembersByMessId(Guid.Parse(Id));
            if (!members.Any()) return RedirectToAction("ShowHistory", "Dashboard");
            ViewBag.Members= members;

            var userId = HttpContext.User.Identity.Name;
            var isMember =members.FirstOrDefault(e => e.Id ==Guid.Parse( userId));

            if (isMember==null) return RedirectToAction("ShowHistory", "Dashboard");

            return View();
        }


        [HttpGet]
        public async Task<IActionResult> FindMembers(string query,string types)
        {
            if (query == null) return RedirectToAction("ShowHistory", "Dashboard");

            var members = await _unitOfWork.Persons.GetAllMembersByQueryParameters(query, types);
            return Ok(members);
        }

        [HttpGet]

        public async Task<IActionResult> AddIntoMess(string MessId, string UserId,string type)
        {
            if(MessId == null|| UserId==null) {
                TempData["Error"] = "Something happend wrong. Please try again";
                return RedirectToAction("ShowHistory", "Dashboard");
            }
            var curUserId=HttpContext.User.Identity?.Name;
            var person = await _unitOfWork.MessHaveMembers
                .GetByMessIdAndPersonId(Guid.Parse(MessId), Guid.Parse(curUserId));
            if (person != null && person.IsManager == false)
            {
                TempData["Unauthorized"] = "You have not permitted to add user";
                return RedirectToAction("MessMembers", "Dashboard", new { Id = MessId });
            }

            var addIntoMess = new MessHaveMember()
            {
                PersonId=Guid.Parse(UserId),
                MessId=Guid.Parse(MessId),
                IsManager=(type=="Manager")?true:false,
                UpdatedAt = DateTime.Now,

            };

            await _unitOfWork.MessHaveMembers.Add(addIntoMess);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction("MessMembers","Dashboard",new {Id=MessId});
        }
        [HttpGet]
        public async Task<IActionResult> RemoveFromMess(string MessId, string UserId)
        {
            if (MessId == null || UserId == null || !ValidityChecker.IsValidGuid(MessId) || !ValidityChecker.IsValidGuid(UserId))
            {
                TempData["Error"] = "Something happend wrong. Please try again";
                return RedirectToAction("ShowHistory", "Dashboard");
            }

            var currentUserId = HttpContext.User.Identity.Name;

            await _unitOfWork.MessHaveMembers.RemoveByMessIdAndPersonId(Guid.Parse(MessId),Guid.Parse(UserId),Guid.Parse(currentUserId));
            await _unitOfWork.CompleteAsync();

            return RedirectToAction("MessMembers", "Dashboard", new { Id = MessId });
        }


        [HttpPost]
        public async Task<IActionResult> CreateNewMonth(MonthDTO monthDetails)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }
            var id = HttpContext.User.Identity.Name;
            var messId= monthDetails.Id;

            var member= await _unitOfWork.MessHaveMembers.GetByMessIdAndPersonId(messId,Guid.Parse(id));

            if (member == null)
            {
                TempData["Error"] = "Something happend error! please try again";
                return RedirectToAction("ShowMonthHistory", "Dashboard", new { MessId = messId });
            }

            if (!member.IsManager)
            {
                TempData["Error"] = "Sorry! Only mess manager can create the month history";
                return RedirectToAction("ShowMonthHistory", "Dashboard", new { MessId = messId });
            }

            var person = await _unitOfWork.Persons.GetById(Guid.Parse(id));

            var newMonth = new Month()
            {
                Name = monthDetails.Name,
                StartDate = monthDetails.StartDate,
                AddedBy= person.Name,
                MessId= messId,
                UpdatedAt = DateTime.Now,
            };

            await _unitOfWork.Months.Add(newMonth);
            await _unitOfWork.CompleteAsync();
            TempData["Succes"] = "Successfully added";

            return RedirectToAction("ShowMonthHistory", "Dashboard", new { MessId = messId });
        

        }
        [HttpGet]
        public async Task<IActionResult> ShowMonthHistory(string MessId)
        {
            if (string.IsNullOrEmpty(MessId) || !ValidityChecker.IsValidGuid(MessId))
            {
                TempData["Error"] = string.Empty;
                return RedirectToAction("ShowHistory","Dashboard");
            }
           
            var months = await _unitOfWork.Months.GetMonthsByMessId(Guid.Parse(MessId));

            var newMonth = new MonthDTO()
            {
                Id = Guid.Parse(MessId),
                StartDate = DateTime.Now,
            };
            ViewBag.MessId = MessId;
            ViewBag.Months = months;
            return View(newMonth);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteMonthHistory(string MonthId)
        {
            if (string.IsNullOrEmpty(MonthId) || !ValidityChecker.IsValidGuid(MonthId))
            {
                TempData["Error"] = "Invalid request.";
                return RedirectToAction("ShowMonthHistory", "Dashboard");
            }

            var id = HttpContext.User.Identity.Name;
            var monthDetails = await _unitOfWork.Months.GetById(Guid.Parse(MonthId));
            var member = await _unitOfWork.MessHaveMembers.GetByMessIdAndPersonId(monthDetails.MessId, Guid.Parse(id));

            if (member == null)
            {
                TempData["Error"] = "Somethings Happend wrong!";
                return RedirectToAction("ShowMonthHistory", "Dashboard");
            }

            if (!member.IsManager)
            {
                TempData["Error"] = "Sorry you have't any permission!";
                return RedirectToAction("ShowMonthHistory", "Dashboard");
            }

            await _unitOfWork.Months.Delete(monthDetails);
            await _unitOfWork.CompleteAsync();

            TempData["Success"] = "Successfully deleted";

            return RedirectToAction("ShowMonthHistory", "Dashboard",new { MessId=monthDetails.MessId});
        }


        [HttpGet]
        public async Task<IActionResult> EditMonthHistory(string MessId,string MonthId)
        {
            if (string.IsNullOrEmpty(MonthId) || string.IsNullOrEmpty(MessId) || !ValidityChecker.IsValidGuid(MonthId) || !ValidityChecker.IsValidGuid(MessId))
            {
                TempData["Error"] = "Something happend error";
                return RedirectToAction("ShowMonthHistory", "Dashboard",new {MessId=MessId});
            }

            var month = await _unitOfWork.Months.GetById(Guid.Parse(MonthId));
            if (month == null)
            {
                TempData["Error"] = "Invalid Request";
                return RedirectToAction("ShowMonthHistory", "Dashboard", new { MessId = MessId });
            }

            var requestDto = new MonthDTO()
            {
                Id = month.Id,
                MessId=Guid.Parse(MessId),
                Name = month.Name,
                StartDate = month.StartDate,
            };
            return View(requestDto);
        }



        [HttpPost]
        public async Task<IActionResult> EditMonthHistory(MonthDTO month)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please provide valid informations";
                return RedirectToAction("ShowMonthHistory", "Dashboard", new {MessId=month.MessId});
            }

            if( !ValidityChecker.IsValidGuid(month.Id) || !ValidityChecker.IsValidGuid(Convert.ToString(month.MessId)))
            {
                return RedirectToAction("ShowMonthHistory", "Dashboard", new { MessId = month.MessId });
            }

            var id = HttpContext.User.Identity.Name;
            var messId=month.MessId?? Guid.NewGuid();
            var details = await _unitOfWork.MessHaveMembers.GetByMessIdAndPersonId(messId, Guid.Parse(id));

            if (details == null || !details.IsManager)
            {
                TempData["Error"] = "Sorry! You have not edit permission";
                return RedirectToAction("ShowMonthHistory", "Dashboard", new { MessId = month.MessId });

            }

            var existingMonth = await _unitOfWork.Months.GetById(month.Id);

            if (existingMonth == null)
            {
                TempData["Error"] = "Sorry! You have provided wrong informations";
                return RedirectToAction("ShowMonthHistory", "Dashboard", new { MessId = month.MessId });
            }

            existingMonth.Name = month.Name ?? existingMonth.Name;
            existingMonth.StartDate = month.StartDate!=null ? month.StartDate: existingMonth.StartDate;

            _unitOfWork.Months.Update(existingMonth);

            await _unitOfWork.CompleteAsync();

            TempData["Success"] = "Successfully Updated";
            return RedirectToAction("ShowMonthHistory", "Dashboard",new {MessId=month.MessId});
        }


    }
}
