using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MMS.Authentication.Configuration;
using MMS.Authentication.Models.DTO.Incomming;
using MMS.Entities.DbSet;
using MMS.Entities.Dtos.Incomming;
using MMS.Entities.Dtos.Outgoing;
using System.Security.Claims;
using MMS.DataService.IRepository;
using MMS.DataService.Service;

namespace MMS.Web.Controllers
{
    [Authorize]
    public class ProfileController : BaseController
    {

        private readonly IUnitOfService _unitOfService;
       
        public ProfileController(
            IUnitOfService unitOfService,
            IUnitOfWork unitOfWork
            ) : base(unitOfWork, unitOfService)
        {
            _unitOfService = unitOfService;

        }

        public async Task< IActionResult> Details()
        {
            try
            {
                var Id = HttpContext.User.Identity.Name;
                var person = await _unitOfService.ProfileService.DetailsService(Id);
                ViewBag.PersonId = person.Id;
                return View(person);
            }
            catch(Exception ex )
            {
                TempData["Error"] =ex.Message;
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(ProfileUpdateRequestDTO person, IFormFile ImageFile)
        {

            try
            {
                var Id = HttpContext.User.Identity.Name;
                await _unitOfService.ProfileService.UpdateProfileService(person, ImageFile, Id);
                return RedirectToAction("Details", "Profile");
            }
            catch (FileNotFoundException)
            {
                TempData["Error"] = "Image file not found.";
                return RedirectToAction("Index", "Error");
            }
            catch(Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Error");
            }
         
        }


        public async Task<IActionResult> ChangePassword()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO Details)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }
                var id = HttpContext.User.Identity.Name;
                bool isUpdate = await _unitOfService.ProfileService.ChangePasswordService(Details, id);
                if (isUpdate)
                {
                    TempData["Success"] = "Change password successfully";
                    return RedirectToAction("Details", "Profile");

                }
                
                return View();
            }
            catch(Exception e)
            {
                TempData["Error"] = e.Message;
                return RedirectToAction("Index", "Error");
            }
        }



    }
}
