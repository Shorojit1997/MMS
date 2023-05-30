using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MMS.DataService.IConfiguration;
using MMS.Entities.DbSet;
using MMS.Entities.Dtos.Incomming;

namespace MMS.Web.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        public AccountController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<IActionResult> Index()
        {
            //Checking the user account 
            var id = HttpContext.User.Identity.Name;
            var account = await _unitOfWork.Accounts.GetAccountDetailsByPersonId(Guid.Parse(id));
            if(account == null)
            {
                //If user not exist in Account table then create new account
                var newAccount = new Account()
                {
                    ClientId="xxxx-xxxx-xxx-xxxx",
                    ClientSecret="yyyy-yyyy-yyyy-yyyy",
                    PersonId=Guid.Parse(id),
                    UpdatedAt=DateTime.Now,
                };
                await _unitOfWork.Accounts.Add(newAccount);
                await _unitOfWork.CompleteAsync();
                account = newAccount;
            }

            //Preparing the account transfer object
            var accountDto = new AccountDTO()
            {
                ClientId = account.ClientId.Substring(0, 4) + "*****************************" + account.ClientId.Substring(account.ClientId.Length - 4, 4),
                ClientSecret = account.ClientSecret.Substring(0, 4) + "*****************************" + account.ClientSecret.Substring(account.ClientSecret.Length - 4, 4),
            };
            return View(accountDto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit_Accounts(AccountDTO account)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid informations";
                return RedirectToAction("Index", "Account");
            }
            //get account details by person id
            var id = HttpContext.User.Identity.Name;
            var accountDetails= await _unitOfWork.Accounts.GetAccountDetailsByPersonId(Guid.Parse(id));
            
            //Checking the changes
            if(!account.ClientId.Contains('*'))
                accountDetails.ClientId=account.ClientId??accountDetails.ClientId;
            if(!account.ClientSecret.Contains("*"))
                accountDetails.ClientSecret = account.ClientSecret ?? accountDetails.ClientSecret;
            //Assign the updated date
            accountDetails.UpdatedAt = DateTime.UtcNow;

            //Update the account details
            _unitOfWork.Accounts.Update(accountDetails);
            await _unitOfWork.CompleteAsync();
            //If completed the update procedure then Redirect to the Index page
            return RedirectToAction("Index","Account");
        }
    }
}
