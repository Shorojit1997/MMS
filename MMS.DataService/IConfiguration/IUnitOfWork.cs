using MMS.DataService.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.DataService.IConfiguration
{
    public interface IUnitOfWork
    {
        IPersonRepository Persons { get; }
        IMessRepository Messes { get; }

        IMessMemberRepository MessHaveMembers { get; }

        IMonthRepository Months { get; }

        IAccountRepository Accounts { get; }
        Task CompleteAsync();

    }
}
