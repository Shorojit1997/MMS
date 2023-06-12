using Microsoft.AspNetCore.Http;
using MMS.DataService.IRepository;
using MMS.DataService.IService;
using MMS.Entities.DbSet;
using MMS.Entities.Dtos.Incomming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MMS.DataService.Service
{
    public class AccountService:IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        public AccountService(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Edit_Accounts(AccountDTO account, string PersonId)
        {
            
            //get account details by person id

            var accountDetails = await _unitOfWork.Accounts.GetAccountDetailsByPersonId(Guid.Parse(PersonId));

            //Checking the changes
            if (!account.ClientId.Contains('*'))
                accountDetails.ClientId = account.ClientId ?? accountDetails.ClientId;
            if (!account.ClientSecret.Contains("*"))
                accountDetails.ClientSecret = account.ClientSecret ?? accountDetails.ClientSecret;


            //Assign the updated date
            accountDetails.UpdatedAt = DateTime.UtcNow;

            //Update the account details
            _unitOfWork.Accounts.Update(accountDetails);
            await _unitOfWork.CompleteAsync();

            //If completed the update procedure then Redirect to the Index page
            return true;
        }

        public async Task<AccountDTO> GetDetailsServices(string PersonId)
        {
          
            var account = await _unitOfWork.Accounts.GetAccountDetailsByPersonId(Guid.Parse(PersonId));
            if (account == null)
            {
                //If user not exist in Account table then create new account
                var newAccount = new Account()
                {
                    ClientId = "xxxx-xxxx-xxx-xxxx",
                    ClientSecret = "yyyy-yyyy-yyyy-yyyy",
                    PersonId = Guid.Parse(PersonId),
                    UpdatedAt = DateTime.Now,
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
            return accountDto;
        }
    
    
    
    
    
    
    
    
    
    }
}
