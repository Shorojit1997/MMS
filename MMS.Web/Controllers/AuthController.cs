using Microsoft.AspNetCore.Mvc;
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
using Microsoft.AspNetCore.Authorization;
using MMS.DataService.IRepository;
using MMS.DataService.Service;

namespace MMS.Web.Controllers
{

    [AllowAnonymous]
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
            IEmailService emailService,
            IUnitOfService unitOfService


            ) : base(unitOfWork, unitOfService)
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
            try
            {
                var claimsIdentity = await _unitOfService.AuthService.LoginPost(person);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = false
                };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity), authProperties);

                return RedirectToAction("ShowHistory", "Dashboard");

            }
            catch(Exception e)
            {
                ModelState.AddModelError("Error",e.Message);
                return View();
            }

        }


        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registration(RegistrationRequestDTO person)
        {

            try
            {
                if (!ModelState.IsValid)
                    return View(person);
                

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = false
                };
                var claimsIdentity = await _unitOfService.AuthService.RegistrationPost(person);


                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity), authProperties);

                return RedirectToAction("ShowHistory", "Dashboard");
            }
            catch(Exception e)
            {
                ModelState.AddModelError("Error",e.Message);
                return View();
            }

        }

        public async Task<IActionResult> ConfirmMail(string Token,string Email)
        {
            try
            {
                var claimsIdentity = await _unitOfService.AuthService.ConfirmMail(Token, Email);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = false
                };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity), authProperties);

                return RedirectToAction("ShowHistory", "Dashboard");
            }
            catch(Exception e)
            {
                ModelState.AddModelError("Error",e.Message);
                return RedirectToAction("Registration", "Auth");
            }
           
           
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
            try
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }
                await _unitOfService.AuthService.ResetPassword(Details);
                return RedirectToAction("Details", "Profile");

            }
            catch (Exception ex)
            {
                TempData["Error"]=ex.Message;
                return RedirectToAction("Login", "Auth");
            }
        }
        public IActionResult SendMail()
        {
            return View();
        }

        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Auth");
        }
    }
}
