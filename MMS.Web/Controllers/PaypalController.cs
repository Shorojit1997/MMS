using Azure;
using Microsoft.AspNetCore.Mvc;
using MMS.DataService.IConfiguration;
using MMS.DataService.Paypal;
using MMS.Entities.Dtos.Incomming;

namespace MMS.Web.Controllers
{
    public class PaypalController : BaseController
    {
        private readonly PaypalClient _paypalClient;

        public PaypalController(IUnitOfWork unitOfWork) : base(unitOfWork) 
        {
            _paypalClient = new PaypalClient();
        }
        public async Task<IActionResult> Deposit()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Deposit(DepositDTO deposit)
        {
            try
            {
                // set the transaction price and currency
                var price = Convert.ToString(deposit.Amount);
                var currency = "USD";
                SetClientSecret();

                var id = HttpContext.User.Identity.Name.ToString();
                var response = await _paypalClient.CreateOrder(price, currency);
                HttpContext.Session.SetString(id,response.id);

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

            var orderid= HttpContext.Session.GetString(HttpContext.User.Identity.Name.ToString());
            HttpContext.Session.Clear();
            try
            {
                SetClientSecret();
                var response = await _paypalClient.CaptureOrder(orderid);

               // var reference = response.purchase_units[0].reference_id;

                // Put your logic to save the transaction here
                // You can use the "reference" variable as a transaction key

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


        private async void SetClientSecret()
        {
                var id = HttpContext.User.Identity.Name;
                var clientSecret = await _unitOfWork.Accounts.GetAccountDetailsByPersonId(Guid.Parse(id));

                _paypalClient.ClientId = clientSecret.ClientId;
                _paypalClient.ClientSecret = clientSecret.ClientSecret;
            
        }
    }
}
