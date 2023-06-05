using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MMS.DataService.Data;
using MMS.DataService.IRepository;
using MMS.Entities.DbSet;


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
