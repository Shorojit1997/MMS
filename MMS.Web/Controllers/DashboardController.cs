using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MMS.DataService.IRepository;
using MMS.DataService.Others;
using MMS.DataService.Service;
using MMS.Entities.DbSet;
using MMS.Entities.Dtos.Incomming;
using Org.BouncyCastle.Ocsp;


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
                var id = HttpContext.User.Identity.Name;
                await _unitOfService.DashboardService.DeleteMessHistory(MessId, id);
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

                var members = await _unitOfWork.MessHaveMembers.GetAllMembersByMessId(Guid.Parse(Id));
                if (members==null) throw new Exception("Invalid Route");

                

                var userId = HttpContext.User.Identity.Name;
                var isMember = members.FirstOrDefault(e => e.Id == Guid.Parse(userId));

                if (isMember == null) throw new Exception("Invalid Route");

                ViewBag.MessId = Id;
                ViewBag.Members = members;

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
                //adding the current user into mess
                var isAdded= await _unitOfService.DashboardService.AddIntoMess(MessId, UserId, type,curUserId);

                //checking is it possible to add or not
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
                //Null value checking or wrong guid id checking 
                if (MessId == null || UserId == null || !ValidityChecker.IsValidGuid(MessId) || !ValidityChecker.IsValidGuid(UserId))
                    throw new Exception( "Something happend wrong. Please try again");

                //finding the current userId from the HttpContext
                var currentUserId = HttpContext.User.Identity.Name;

                //adding into database
                await _unitOfWork.MessHaveMembers.RemoveByMessIdAndPersonId(Guid.Parse(MessId), Guid.Parse(UserId), Guid.Parse(currentUserId));
                await _unitOfWork.CompleteAsync();

                return RedirectToAction("MessMembers", "Dashboard", new { Id = MessId });
            }
            catch(Exception e)
            {
                TempData["Error"]=e.Message;
                return RedirectToAction("Index", "Error");
            }
           
        }







        [HttpPost]
        public async Task<IActionResult> CreateNewMonth(MonthDTO monthDetails)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }
                var id = HttpContext.User.Identity.Name;
                //will create new month
                await _unitOfService.DashboardService.CreateNewMonth(monthDetails, id);
                TempData["Succes"] = "Successfully added";
                return RedirectToAction("ShowMonthHistory", "Dashboard", new { MessId = monthDetails.Id });
            }
            catch(Exception e)
            {
                TempData["Error"] = e.Message;
                return RedirectToAction("Index", "Error");
            }
        

        }






        [HttpGet]
        public async Task<IActionResult> ShowMonthHistory(string MessId)
        {
            try
            {
                if (string.IsNullOrEmpty(MessId) || !ValidityChecker.IsValidGuid(MessId))
                    throw new Exception("Invalid route");
              
                //getting all of  month by messId
                var months = await _unitOfWork.Months.GetMonthsByMessId(Guid.Parse(MessId));

                //Created new view-Model for the filling the input from
                var newMonth = new MonthDTO()
                {
                    Id = Guid.Parse(MessId),
                    StartDate = DateTime.Now,
                };
                ViewBag.MessId = MessId;
                ViewBag.Months = months;

                return View(newMonth);
            }
            catch (Exception e)
            {
                TempData["Error"]=e.Message;
                return RedirectToAction("Index", "Error");  
            }
        }




        [HttpGet]
        public async Task<IActionResult> DeleteMonthHistory(string MonthId)
        {
            try
            {
                var id = HttpContext.User.Identity.Name;
                var messId = await _unitOfService.DashboardService.DeleteMonthHistory(MonthId, id);

                TempData["Success"] = "Successfully deleted";
                return RedirectToAction("ShowMonthHistory", "Dashboard", new { MessId = messId });

            }
            catch(Exception ex) {
                TempData["Error"] =ex.Message;
                return RedirectToAction("Index", "Error");
            }
        }


        [HttpGet]
        public async Task<IActionResult> EditMonthHistory(string MessId,string MonthId)
        {
            try
            {
                var requestDto = await _unitOfService.DashboardService.EditMonthHistory(MessId,MonthId);
                return View(requestDto);
            }
            catch(Exception ex )
            {
                TempData["Error"] =ex.Message;
                return RedirectToAction("Index", "Error");
            }
        }



        [HttpPost]
        public async Task<IActionResult> EditMonthHistory(MonthDTO month)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Please provide valid informations";
                    return View(month);
                }

                var id = HttpContext.User.Identity.Name;
                await _unitOfService.DashboardService.EditMonthHistoryForPostController(month, id);
                TempData["Success"] = "Successfully Updated";


                return RedirectToAction("ShowMonthHistory", "Dashboard", new { MessId = month.MessId });
            }
            catch(Exception ex)
            {
                TempData["Error"]=ex.Message;
                return RedirectToAction("Index", "Error");
            }
        }


    }
}
