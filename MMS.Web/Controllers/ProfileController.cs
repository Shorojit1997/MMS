using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MMS.Authentication.Configuration;
using MMS.Authentication.Models.DTO.Incomming;
using MMS.DataService.IConfiguration;
using MMS.Entities.DbSet;
using MMS.Entities.Dtos.Incomming;
using MMS.Entities.Dtos.Outgoing;
using System.Security.Claims;

namespace MMS.Web.Controllers
{
    [Authorize]
    public class ProfileController : BaseController
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        public ProfileController(
            IUnitOfWork unitOfWork,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signManager,
            IOptionsMonitor<JwtConfig> optionsMonitor
            ) : base(unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signManager;
        }

        public async Task< IActionResult> Details()
        {
            var Id = HttpContext.User.Identity.Name;
            var person = await _unitOfWork.Persons.GetById(Guid.Parse(Id));

            
            if (person == null)
            {
                return RedirectToAction("Index", "Error");
            }

            var newPerson = new ProfileUpdateResponseDTO()
            {
                Id=person.Id,
                Name = person.Name,
                Email = person.Email,
                Phone = person.Phone,
                PictureUrl = person.PictureUrl,
            };
            ViewBag.PersonId = person.Id;
            return View(newPerson);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(ProfileUpdateRequestDTO person, IFormFile ImageFile)
        {

            try
            {
                var Id = HttpContext.User.Identity.Name;
                var existingPerson = await _unitOfWork.Persons.GetById(Guid.Parse(Id)); ;

                var originalpath = existingPerson.PictureUrl;
                //Delete previous Image
                if (ImageFile != null)
                {
                    string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingPerson.PictureUrl);
                    if (System.IO.File.Exists(fullPath) && existingPerson.PictureUrl != "/images/default.jpg")
                    {
                        System.IO.File.Delete(fullPath);
                    }

                    //Write new image 
                    originalpath =  DateTime.Now.Ticks + ImageFile.FileName;


                    var filePath = Path.Combine("wwwroot", "images", originalpath);
                    if (existingPerson.PictureUrl != "/images/Default.jpg")
                    {
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(stream);

                        }
                    }
                    originalpath = "/images/" + originalpath;

                }
               
                existingPerson.PictureUrl = originalpath;
                existingPerson.Name = person.Name!=""? person.Name:existingPerson.Name;
                existingPerson.Phone = person.Phone != "" ? person.Phone : existingPerson.Phone;
                
                _unitOfWork.Persons.Update(existingPerson);


                return RedirectToAction("Details", "Profile");
            }
            catch (FileNotFoundException)
            {
                TempData["message"] = "Image file not found.";
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
            if (!ModelState.IsValid)
            {
                return View();
            }
            var email = HttpContext.User.Identity.Name;
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {

                var isSignin = await _signInManager.PasswordSignInAsync(user, Details.Password, false, false);
                if (isSignin.Succeeded)
                {
                    await _userManager.ChangePasswordAsync(user, Details.CurrentPassword, Details.Password);
                    TempData["Success"] = "Change password Successfully";
                    return RedirectToAction("Details", "Profile");
                }

            }
            return View();
        }



    }
}
