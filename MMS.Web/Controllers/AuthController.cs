using Microsoft.AspNetCore.Mvc;
using MMS.DataService.IConfiguration;
using MMS.Authentication.Models.DTO.Incomming;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MMS.Authentication.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using MMS.Entities.DbSet;
using MMS.Authentication.Models.DTO.Outgoing;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using MMS.Authentication.IService;
using MMS.Authentication.Models.Mail;
using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.V5.Pages.Account.Internal;

namespace MMS.Web.Controllers
{

    public class AuthController : BaseController
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly JwtConfig _jwtConfig;
        private readonly IEmailService _emailService;

        public AuthController(
            IUnitOfWork unitOfWork,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signManager,
            IOptionsMonitor<JwtConfig> optionsMonitor,
            IEmailService emailService


            ) : base(unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signManager;
            _jwtConfig = optionsMonitor.CurrentValue;
            _emailService = emailService;

        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDTO person)
        {
            if (ModelState.IsValid)
            {
                var userExist = await _userManager.FindByEmailAsync(person.Email);
                if (userExist == null)
                {
                    ModelState.AddModelError("Email", "Please provide a valid email");
                    return View();
                }
                var isSignin = await _signInManager.PasswordSignInAsync(userExist, person.Password, false, false);
                if (isSignin.Succeeded)
                {
                    var claims = new List<Claim>
                    {
                       new Claim(ClaimTypes.Name, userExist.Email),
                       new Claim(ClaimTypes.Email, userExist.Email)
                     };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = false
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity), authProperties);

                    return RedirectToAction("Details", "Profile");


                }
                else if (isSignin.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Your account is locked. Please contact support.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt");
                }

                return View();

            }

            return View();
        }


        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registration(RegistrationRequestDTO person)
        {

            if (ModelState.IsValid)
            {

                var userExist = await _userManager.FindByEmailAsync(person.Email);
                //checking the email is already use or not
                if (userExist != null)
                {
                    ModelState.AddModelError("Email", "Email already in use another person");
                    return View();
                }

    
                //Add new user
                var newUser = new IdentityUser()
                {
                    Email = person.Email,
                    UserName = person.Email,
                    EmailConfirmed = false
                };

                //Adding user to the table
                var isCreated = await _userManager.CreateAsync(newUser, person.Password);
                // If failed to adding to the table
                if (!isCreated.Succeeded)
                {
                    var error = isCreated.Errors.Select(x => x.Description).ToList();
                    ModelState.AddModelError("Error", error[0]);
                    return View();

                }
                //Save the user Details in User models
                var _person = new Person();
                _person.Id = new Guid(newUser.Id);
                _person.Name = person.Name;
                _person.Email = person.Email;
                _person.Phone = person.Phone;

                _person.UpdateDate = DateTime.Now;

                await _unitOfWork.Persons.Add(_person);
                await _unitOfWork.CompleteAsync();
         

               /* var claims = new[]
                 {
                     new Claim(ClaimTypes.Name, person.Email),
                     new Claim(ClaimTypes.Email, person.Email),
                };

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = false
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity), authProperties);*/

                 var user = new IdentityUser()
                 {
                    Email=person.Email,
                    SecurityStamp=Guid.NewGuid().ToString(),
                    UserName= person.Name,
                };

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.Action(nameof(ConfirmMail), "Auth", new {Token=token,Email=_person.Email },Request.Scheme);
                var message = new Message(new string[] { user.Email },"Confirmation email Link",confirmationLink);

                
                var result = await _userManager.ConfirmEmailAsync(user, token);
                //commenting because of bugs
                await _emailService.SendMail(message);


                //return RedirectToAction("Details", "Profile");
                return RedirectToAction("SendMail", "Auth");


            }
            return View();
        }

        public async Task<IActionResult> ConfirmMail(string Token,string Email)
        {
            var user = await _userManager.FindByEmailAsync(Email);
            if(user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, Token);
                if(result.Succeeded)
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
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity), authProperties);


                    return RedirectToAction("Details", "Profile");
                }

            }
            return RedirectToAction("Login", "Auth");
        }


        public async Task<IActionResult> ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var confirmationLink = Url.Action(nameof(ResetPassword), "Auth", new {Token=token,Email=user.Email }, Request.Scheme);
                var message = new Message(new string[] {user.Email }, "Confirmation email Link", confirmationLink);
                _emailService.SendMail(message);
                return RedirectToAction("SendMail","Auth");
            }
            ModelState.AddModelError("Error ", "Please provide right email");

            return View();
        }

        public IActionResult SendMail()
        {
            return View();
        }


        public async Task<IActionResult> ResetPassword(string Token,string Email)
        {
            var users = new ResetPasswordDTO()
            {
                Token = Token,
                Email = Email
            };
      
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO Details)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }
            var user = await _userManager.FindByEmailAsync(Details.Email);
            if (user != null)
            {
                
               var result = await _userManager.ResetPasswordAsync(user, Details.Token,Details.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Details", "Profile");
                }

            }
            return RedirectToAction("Login", "Auth");
        }

    }
}
