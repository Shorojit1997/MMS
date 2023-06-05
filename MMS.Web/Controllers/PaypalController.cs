using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MMS.DataService.IRepository;
using MMS.DataService.Paypal;
using MMS.DataService.Service;
using MMS.Entities.DbSet;
using MMS.Entities.Dtos.Incomming;
using System.Runtime.CompilerServices;

namespace MMS.Web.Controllers
{
    [Authorize]
    public class PaypalController : BaseController
    {
        private readonly PaypalClient _paypalClient;

        public PaypalController(IUnitOfWork unitOfWork, IUnitOfService unitOfService) : base(unitOfWork, unitOfService) 
        {
            _paypalClient = new PaypalClient();
        }

        [HttpGet]
        public async Task<IActionResult> Deposit(string MessId)
        {
            if (MessId == null)
            {
                return RedirectToAction();
            }
            var Id = HttpContext.User.Identity.Name;
            var deposit = new DepositDTO()
            {
                MessId=Guid.Parse(MessId),
                Amount=10,
            };

            return View(deposit);
        }

        [HttpPost]
        public async Task<IActionResult> Deposit(DepositDTO deposit)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return View();
                }
                
                var price = Convert.ToString(deposit.Amount);
                var currency = "USD";

                //Set client id and Client secret for each person

                bool isSet=await SetClientSecret(deposit.MessId);
                if (!isSet)
                {
                    throw new Exception("Failed to assign Client Secret");
                }

                //Get Person Id from the Identity user
                var id = HttpContext.User.Identity.Name;

                //Make a order request
                var response = await _paypalClient.CreateOrder(price, currency);

                //Set orderid from the resposces
                HttpContext.Session.SetString(id,response.id);
                HttpContext.Session.SetString("MessId", deposit.MessId.ToString());

                var deposits = new Deposit()
                {
                    Amount = deposit.Amount,
                    Success = false,
                    PersonId=Guid.Parse(id),
                    MessId=deposit.MessId,
                    OrderId=response.id,
                    UpdatedAt=DateTime.Now
                };
               

                await _unitOfWork.Deposits.Add(deposits);
                await _unitOfWork.CompleteAsync();
                //Finding the approve link from the responses

                if (response.links.Count > 0)
                {
                    foreach (var item in response.links)
                    {
                        if (item.rel == "approve")
                        {
                            return Redirect(item.href);
                        }
                    }
                }

                return Ok(response);
            }
            catch (Exception e)
            {
                var error = new
                {
                    e.GetBaseException().Message
                };

                return BadRequest(error);
            }
        }

 
        public async Task<IActionResult> Capture(CancellationToken cancellationToken)
        {
            //Get orderId from the session
            var orderid= HttpContext.Session.GetString(HttpContext.User.Identity.Name.ToString());
            var MessId = HttpContext.Session.GetString("MessId");
            //After getting the orderId clear the session
            HttpContext.Session.Clear();
            try
            {
                //set client secret for each user
                bool isSet=  await SetClientSecret(Guid.Parse(MessId));
                if (!isSet)
                {
                    throw new Exception("Failed to assign Client Secret");
                }

                //Requesting for the captureing the deposit process
                var response = await _paypalClient.CaptureOrder(orderid);
                if(response==null)
                {
                    throw new Exception("Gateway error");
                }

                var existingDeposit=await _unitOfWork.Deposits.GetByOrderId(orderid);
                existingDeposit.Success = true;
                existingDeposit.UpdatedAt = DateTime.Now;
                _unitOfWork.Deposits.Update(existingDeposit);
                await _unitOfWork.CompleteAsync();

                return RedirectToAction("Success","Paypal");
            }
            catch (Exception e)
            {
                var error = new
                {
                    e.GetBaseException().Message
                };

                return BadRequest(error);
            }
        }

        public IActionResult Success()
        {
            return View();
        }


        private async Task<bool> SetClientSecret(Guid MessId)
        {
            //Get userId from the Identity user
            var id = HttpContext.User.Identity.Name;

            //Get client secret from the AccountDetails
            var persons = await _unitOfWork.MessHaveMembers.GetAllManagersDetailsByMessId(MessId);
            if (persons != null)
            {
                var clientSecret = await _unitOfWork.Accounts.GetAccountDetailsByPersonId(persons.First());

                if (clientSecret != null)
                {
                    _paypalClient.ClientId = clientSecret.ClientId;
                    _paypalClient.ClientSecret = clientSecret.ClientSecret;
                    return true;
                }
            }
           
            return false;
            
        }
    }
}
