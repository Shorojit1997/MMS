using Microsoft.Extensions.Logging;
using MMS.DataService.Data;
using MMS.DataService.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.DataService.Repository
{
    public class UnitOfWork : IDisposable, IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly ILogger _logger;

        public IPersonRepository Persons { get; private set; }

        public IMessRepository Messes { get; private set; }

        public IMessMemberRepository MessHaveMembers { get; private set; }
        public IMonthRepository Months { get; private set; }

        public IAccountRepository Accounts { get; private set; }

        public IDepositRepository Deposits { get; private set; }

        public IExpensesRepository Expenses { get; private set; }

        public IDaysRepository Days { get; private set; }

        public UnitOfWork(AppDbContext context, ILoggerFactory loggerFactory)
        {
            _context = context;
            _logger = loggerFactory.CreateLogger<UnitOfWork>();
            Persons = new PersonRepository(context, _logger);
            Messes = new MessRepository(context, _logger);
            MessHaveMembers = new MessMemberRepository(context, _logger);
            Months = new MonthRepository(context, _logger);
            Accounts = new AccountRepository(context, _logger);
            Deposits = new DepositRepository(context, _logger);
            Expenses = new ExpensesRepository(context, _logger);
            Days = new DaysRepository(context, _logger);
        }

        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

    }
}
