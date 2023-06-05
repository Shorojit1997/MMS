using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MMS.DataService.IRepository;
using MMS.DataService.Others;
using MMS.DataService.Service;
using MMS.Entities.DbSet;
using MMS.Entities.Dtos.Incomming;


namespace MMS.Web.Controllers
{
    [Authorize]
    public class DashboardController : BaseController
    {

        public DashboardController(
            IUnitOfWork unitOfWork,
             IUnitOfService unitOfService
            ) : base(unitOfWork, unitOfService)
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

            try
            {
                await _unitOfService.DashboardService.Create(mess, Id);
                return RedirectToAction("ShowHistory", "Dashboard");

            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Error");
            }

        }


        [HttpGet]
        public async Task<IActionResult> ShowHistory()
        {
            try
            {
                var Id = HttpContext.User.Identity.Name;
                var person = await _unitOfWork.Persons.GetById(Guid.Parse(Id));
                var messes = await _unitOfWork.MessHaveMembers.GetByPersonId(Guid.Parse(Id));
                ViewBag.Messes = messes;
                ViewBag.Name = person.Name;
                var mess = new MessRequestDTO()
                {
                    StartDate = DateTime.Now,
                };
                return View(mess);
            }
            catch(Exception ex)
            {
                TempData["Error"]=ex.Message;
                return RedirectToAction("Index", "Error");
            }
           
        }
        
        
        
        
        [HttpGet]
        public async Task<IActionResult> DeleteMessHistory(string MessId)
        {
            try
            {
                if (string.IsNullOrEmpty(MessId) || !ValidityChecker.IsValidGuid(MessId))
                    throw new Exception("Invalid request");
                

                var Id = HttpContext.User.Identity.Name;
                var member = await _unitOfWork.MessHaveMembers
                    .GetByMessIdAndPersonId(Guid.Parse(MessId), Guid.Parse(Id));

                if (member == null)
                    throw new Exception( "Somethings Happened wrong!");
                if (!member.IsManager)
                    throw new Exception("Sorry you have't any permission!");
              

                await _unitOfWork.Messes.DeleteMessByMessId(Guid.Parse(MessId));
                await _unitOfWork.CompleteAsync();

                TempData["Success"] = "Successfully deleted";

                return RedirectToAction("ShowHistory", "Dashboard");
            }
            catch(Exception e)
            {
                TempData["Error"]=e.Message;
                return RedirectToAction("Index", "Error");
            }
            
        }





        [HttpGet]
        public async Task<IActionResult> EditMessHistory(string MessId)
        {
            try
            {
                if (string.IsNullOrEmpty(MessId) || !ValidityChecker.IsValidGuid(MessId))
                     throw new Exception("Invalid route");

                var mess = await _unitOfWork.Messes.GetById(Guid.Parse(MessId));
                if (mess == null)
                    throw new Exception("Invalid route");
                
                var requestDto = new MessRequestDTO()
                {
                    Id = mess.Id,
                    Name = mess.Name,
                    StartDate = mess.StartDate,
                };

                return View(requestDto);
            }
            catch(Exception ex)
            {
                TempData["Error"]=ex.Message;
                return RedirectToAction("Index", "Error");
            }
           
        }




        [HttpPost]
        public async Task<IActionResult> EditMessHistory(MessRequestDTO mess)
        {
            try
            {
                if (!ModelState.IsValid)
                    throw new Exception("Please provide valid informations");

                var id = HttpContext.User.Identity.Name;
                var details = await _unitOfWork.MessHaveMembers.GetByMessIdAndPersonId(mess.Id, Guid.Parse(id));

                if (details == null || !details.IsManager)
                    throw new Exception("Sorry! You have no edit permission");

                bool isUpdated=await _unitOfService.DashboardService.EditMessHistory(mess);
                if (isUpdated)
                {
                    TempData["Success"] = "Successfully Updated";
                    return RedirectToAction("ShowHistory", "Dashboard");
                }
                throw new Exception("Internal server error");
            }
            catch(Exception ex )
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Error");
            }
        }




        [HttpGet]
        public async Task<IActionResult> MessMembers(string Id)
        {
            try {

                if (Id == null || !ValidityChecker.IsValidGuid(Id))
                    throw new Exception("Invalid Route");

                ViewBag.MessId = Id;

                var members = await _unitOfWork.MessHaveMembers.GetAllMembersByMessId(Guid.Parse(Id));
                if (members==null) throw new Exception("Invalid Route");

                ViewBag.Members = members;

                var userId = HttpContext.User.Identity.Name;
                var isMember = members.FirstOrDefault(e => e.Id == Guid.Parse(userId));

                if (isMember == null) throw new Exception("Invalid Route");

                return View();
            }
            catch(Exception e)
            {
                TempData["Error"]=e.Message;
                return RedirectToAction("Index", "Error");
            }

        }






        [HttpPost]
        public async Task<IActionResult> FindMembers(string query,string types)
        {
            if (query == null) return RedirectToAction("ShowHistory", "Dashboard");

            var members = await _unitOfWork.Persons.GetAllMembersByQueryParameters(query, types);
            return Ok(members);
        }










        [HttpGet]

        public async Task<IActionResult> AddIntoMess(string MessId, string UserId,string type)
        {
            try
            {
                if (MessId == null || UserId == null)
                    throw new Exception( "Something happend wrong. Please try again");
                var curUserId= HttpContext.User.Identity?.Name;
                var isAdded= await _unitOfService.DashboardService.AddIntoMess(MessId, UserId, type,curUserId);
                if (isAdded)
                {
                    TempData["Message"] = "The newly added user i";
                    return RedirectToAction("MessMembers", "Dashboard", new { Id = MessId });
                }
                throw new Exception("Internal server error");
            }
            catch(Exception e)
            {
                TempData["Error"] = e.Message;
                return RedirectToAction("Index", "Error");
            }
        }










        [HttpGet]
        public async Task<IActionResult> RemoveFromMess(string MessId, string UserId)
        {
            try
            {

            }
            catch(Exception e)
            {
                TempData["Error"]=e.Message;
            }
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

            //Creating this month days history for this user.
            var allMembers = await _unitOfWork.MessHaveMembers.GetAllMembersByMessId(messId);

            //adding all days accoding to the newly created month.
            List<Days> days = new List<Days>();

            foreach (var singleMember in allMembers)
            {
                for (int i = 1; i < 32; i++)
                {
                    Days days2 = new Days()
                    {
                        Number = i,
                        Breakfast = 0,
                        Lunch = 0,
                        Dinner = 0,
                        Person_Id =singleMember.Id,
                        Month_Id = newMonth.Id
                    };

                    days.Add(days2);
                }
            }

          
            await _unitOfWork.Months.Add(newMonth);
            await _unitOfWork.Days.AddRange(days);
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
            if (monthDetails == null)
            {
                TempData["Error"] = "Invalid request.";
                return RedirectToAction("ShowMonthHistory", "Dashboard");
            }
            Guid messId =monthDetails.MessId!=null? monthDetails.MessId:Guid.NewGuid();
            var member = await _unitOfWork.MessHaveMembers.GetByMessIdAndPersonId(messId, Guid.Parse(id));

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
