using Microsoft.AspNetCore.Http;
using MMS.DataService.IRepository;
using MMS.DataService.IService;
using MMS.DataService.Paypal;
using MMS.Entities.DbSet;
using MMS.Entities.Dtos.Incomming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.DataService.Service
{
    public class PaypalService:IPaypalService
    {
        private readonly PaypalClient _paypalClient;
        private readonly IUnitOfWork _unitOfWork;

        public PaypalService(IUnitOfWork unitOfWork)
        {
            _paypalClient = new PaypalClient();
        }

        public async Task<CreateOrderResponse> CreateOrderService(DepositDTO deposit, string PersonId)
        {
            var price = Convert.ToString(deposit.Amount);
            var currency = "USD";

            //Set client id and Client secret for each person

            bool isSet = await SetClientSecret(deposit.MessId,PersonId);
            if (!isSet)
            {
                throw new Exception("Failed to assign Client Secret");
            }

            //Make a order request
            var response = await _paypalClient.CreateOrder(price, currency);
          
            return response;
        }

        public async Task<Deposit> SaveOrderService(CreateOrderResponse response, DepositDTO deposit, string PersonId)
        {
            var deposits = new Deposit()
            {
                Amount = deposit.Amount,
                Success = false,
                PersonId = Guid.Parse(PersonId),
                MessId = deposit.MessId,
                OrderId = response.id,
                UpdatedAt = DateTime.Now
            };


            await _unitOfWork.Deposits.Add(deposits);
            await _unitOfWork.CompleteAsync();

            return deposits;
        }

        public async Task<bool> SetClientSecret(Guid MessId,string id)
        {

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
