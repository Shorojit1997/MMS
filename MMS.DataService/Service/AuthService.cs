using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using MMS.Authentication.IService;
using MMS.Authentication.Models.DTO.Incomming;
using MMS.DataService.IRepository;
using MMS.DataService.IService;
using MMS.Entities.DbSet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace MMS.DataService.Service
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailService _emailService;

        public AuthService(
            IUnitOfWork unitOfWork,
            UserManager<IdentityUser> userManager,
            IEmailService emailService,
            SignInManager<IdentityUser> signManager
            )
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signManager;
            _emailService = emailService;
        }

        public async Task<ClaimsIdentity> ConfirmMail(string Token, string Email)
        {
            var user = await _userManager.FindByEmailAsync(Email);
            if (user is not null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, Token);
                if (result.Succeeded)
                {

                    //Update email confirmations
                    user.EmailConfirmed = true;
                    await _userManager.UpdateAsync(user);


                    var claims = new[]
                    {
                     new Claim(ClaimTypes.Name, user.Email),
                     new Claim(ClaimTypes.Email, user.Email),
                    };

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = false
                    };
                    //set cookie for the client side
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    return claimsIdentity;

                }
                throw new Exception(result.Errors.ToString());

            }
            throw new Exception("Invalid mail");
        }

        public async Task<ClaimsIdentity> LoginPost(LoginRequestDTO person)
        {
            var userExist = await _userManager.FindByEmailAsync(person.Email);
            if (userExist == null)
                throw new Exception("Please provide a valid email");

            var isSignin = await _signInManager.PasswordSignInAsync(userExist, person.Password, false, false);
            if (isSignin.Succeeded)
            {
                var claims = new List<Claim>
                {
                       new Claim(ClaimTypes.Name, userExist.Id),
                       new Claim(ClaimTypes.Email, userExist.Email)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                return claimsIdentity;
            }
            else if (isSignin.IsLockedOut)
            {
                throw new Exception("Your account is locked. Please contact support.");
            }
            else
            {
                throw new Exception("Please provide the right credentials.");
            }

        }




        public async Task<ClaimsIdentity> RegistrationPost(RegistrationRequestDTO person)
        {
            var userExist = await _userManager.FindByEmailAsync(person.Email);

            //checking the email is already use or not
            if (userExist != null)
                throw new Exception("Email already in use another person");


            //Add new user
            var newUser = new IdentityUser()
            {
                Email = person.Email,
                UserName = person.Email,
                EmailConfirmed = true
            };

            //Adding user to the table
            var isCreated = await _userManager.CreateAsync(newUser, person.Password);
            // If failed to adding to the table

            if (!isCreated.Succeeded)
            {
                var error = isCreated.Errors.Select(x => x.Description).ToList();
                throw new Exception(error[0]);
            }

            //Save the user Details in User models
            var _person = new Person();
            _person.Id = new Guid(newUser.Id);
            _person.Name = person.Name;
            _person.Email = person.Email;
            _person.Phone = person.Phone;

            _person.UpdatedAt = DateTime.Now;

            await _unitOfWork.Persons.Add(_person);
            await _unitOfWork.CompleteAsync();

            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,  newUser.Id),
                    new Claim(ClaimTypes.Email, newUser.Email)
                 };

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = false
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            return claimsIdentity;

            /*  var user = new IdentityUser()
             {
                 Email = person.Email,
                 SecurityStamp = Guid.NewGuid().ToString(),
                 UserName = person.Name,
             };

             var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

             var confirmationLink = Url.Action(nameof(ConfirmMail), "Auth", new { Token = token, Email = _person.Email }, Request.Scheme);
             var message = new Message(new string[] { user.Email }, "Confirmation email Link", confirmationLink);
            */

            // var result = await _userManager.ConfirmEmailAsync(user, token);
            //commenting because of bugs
            //await _emailService.SendMail(message);


        }

        public async Task<bool> ResetPassword(ResetPasswordDTO Details)
        {
            var user = await _userManager.FindByEmailAsync(Details.Email);
            if (user != null)
            {
                var result = await _userManager.ResetPasswordAsync(user, Details.Token, Details.Password);
                if (result.Succeeded)
                {
                    return true;
                }
                throw new Exception(result.Errors.ToString());

            }
            throw new Exception("Invalid Email");
        }
    }
}
