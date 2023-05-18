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

namespace MMS.Web.Controllers
{

    public class AuthController : BaseController
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly JwtConfig _jwtConfig;

        public AuthController(
            IUnitOfWork unitOfWork,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signManager,
            IOptionsMonitor<JwtConfig> optionsMonitor


            ) : base(unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signManager;
            _jwtConfig = optionsMonitor.CurrentValue;

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

                }

                if (!ModelState.IsValid)
                {
                    return View();

                }

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
                    ModelState.AddModelError("Error", error[0]);
                   
                }
                if (!ModelState.IsValid)
                {
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

                var claims = new[]
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
                    new ClaimsPrincipal(claimsIdentity), authProperties);

                return RedirectToAction("Details","Profile");


            }
            return View();
        }

    
    }
}
