using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.DataService.IRepository
{
    public interface IUnitOfWork
    {
        IPersonRepository Persons { get; }
        IMessRepository Messes { get; }

        IMessMemberRepository MessHaveMembers { get; }

        IMonthRepository Months { get; }

        IAccountRepository Accounts { get; }

        IDepositRepository Deposits { get; }

        IExpensesRepository Expenses { get; }

        IDaysRepository Days { get; }
        Task CompleteAsync();

    }
}
