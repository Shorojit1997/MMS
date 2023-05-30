using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MMS.DataService.Data;
using MMS.DataService.IRepository;
using MMS.Entities.DbSet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.DataService.Repository
{
    public class AccountRepository : GenericRepository<Account>, IAccountRepository
    {
        public AccountRepository(AppDbContext context, ILogger logger) : base(context, logger)
        {
        }

        public async Task<Account> GetAccountDetailsByPersonId(Guid personId)
        {
            return await dbset.FirstOrDefaultAsync(e => e.PersonId == personId);
        }
    }
}
