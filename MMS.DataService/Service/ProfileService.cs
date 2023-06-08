using Microsoft.AspNetCore.Identity;
using MMS.DataService.IRepository;
using MMS.DataService.IService;

using Microsoft.Extensions.Options;
using MMS.Entities.DbSet;
using MMS.Entities.Dtos.Incomming;
using MMS.Entities.Dtos.Outgoing;
using System.Security.Claims;

using Microsoft.AspNetCore.Http;
using PayPal.Core;
using MMS.Authentication.Models.DTO.Incomming;
using Microsoft.AspNetCore.Hosting.Server;
using System.IO;
using Grpc.Core;

namespace MMS.DataService.Service
{
    public class ProfileService:IProfileService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;


        public ProfileService(
            IUnitOfWork unitOfWork,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signManager
            )
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signManager;
        }

        public async Task<bool> ChangePasswordService(ChangePasswordDTO Details,string id)
        {
            var existingUser = await _unitOfWork.Persons.GetById(Guid.Parse(id));
            var user = await _userManager.FindByEmailAsync(existingUser.Email);
            if (user != null)
            {

                var isSignin = await _signInManager.PasswordSignInAsync(user, Details.Password, false, false);
                if (isSignin.Succeeded)
                {
                    await _userManager.ChangePasswordAsync(user, Details.CurrentPassword, Details.Password);
                    return true;
                }

            }
            return false;
        }

        public async Task<ProfileUpdateResponseDTO> DetailsService(string Id)
        {

            var person = await _unitOfWork.Persons.GetById(Guid.Parse(Id));
            if (person == null) throw new Exception("Invalid Route");

            var newPerson = new ProfileUpdateResponseDTO()
            {
                Id = person.Id,
                Name = person.Name,
                Email = person.Email,
                Phone = person.Phone,
                PictureUrl = person.PictureUrl,
            };
           return newPerson;
        }

        public async Task<bool> UpdateProfileService(ProfileUpdateRequestDTO person, IFormFile ImageFile, string Id)
        {

            var existingPerson = await _unitOfWork.Persons.GetById(Guid.Parse(Id));
            var originalPath = existingPerson.PictureUrl;

            // Delete previous Image
            if (ImageFile != null)
            {

                // Delete previous profile picture
                string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/" + existingPerson.PictureUrl);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }

                // Write new image 
                originalPath = "myImage"+ DateTime.Now.Ticks + ImageFile.FileName;
                var filePath = Path.Combine("wwwroot", "images", originalPath);

                if (existingPerson.PictureUrl != "/images/Default.jpg")
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }
                }

                originalPath = "/images/" + originalPath;
            }

            existingPerson.PictureUrl = originalPath;
            existingPerson.Name = person.Name != "" ? person.Name : existingPerson.Name;
            existingPerson.Phone = person.Phone != "" ? person.Phone : existingPerson.Phone;

            _unitOfWork.Persons.Update(existingPerson);
            await _unitOfWork.CompleteAsync();

            return true;
        }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    }
}
