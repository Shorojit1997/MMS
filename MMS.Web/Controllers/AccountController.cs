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
    public class AccountController : BaseController
    {
        public AccountController(IUnitOfWork unitOfWork,IUnitOfService unitOfService) : base(unitOfWork, unitOfService)
        {
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var id = HttpContext.User.Identity.Name;
                var accountDto = await _unitOfService.AccountService.GetDetailsServices(id);
                return View(accountDto);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index","Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit_Accounts(AccountDTO account)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Invalid informations";
                    return RedirectToAction("Index", "Account");
                }
                var id= HttpContext.User.Identity?.Name;
                await _unitOfService.AccountService.Edit_Accounts(account, id);
                return RedirectToAction("Index", "Account");

            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message; 
                return RedirectToAction("Index","Error");
            }
            
        }

        [HttpGet]
        public async Task<IActionResult> MessTransactions(string MessId)
        {
            if (MessId == null || !ValidityChecker.IsValidGuid(MessId))
            {
                return RedirectToAction("ShowHistory", "Dashboard");
            }

            ViewBag.MessId = MessId;
            var id= HttpContext.User.Identity.Name;

            var transactions = new List<DepositDTO>();
            transactions= await _unitOfWork.Deposits.GetTransactionsByMessId(Guid.Parse(MessId))??transactions;
        

            ViewBag.Transactions = transactions;

            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Transactions()
        {
           
            var id = HttpContext.User.Identity.Name;

            var transactions = new List<DepositDTO>();
            transactions = await _unitOfWork.Deposits.GetTransactionsByPersonId(Guid.Parse(id)) ?? transactions;
            ViewBag.Transactions = transactions;

            return View();
        }















    }
}
