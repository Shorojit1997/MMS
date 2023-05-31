using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MMS.DataService.Data;
using MMS.DataService.IRepository;
using MMS.Entities.DbSet;
using MMS.Entities.Dtos.Incomming;

namespace MMS.DataService.Repository
{
    public class DepositRepository : GenericRepository<Deposit>, IDepositRepository
    {
        public DepositRepository(AppDbContext context, ILogger logger) : base(context, logger)
        {
        }

        public async Task<Deposit> GetByOrderId(string id)
        {
            return await dbset.FirstOrDefaultAsync(e => e.OrderId == id);
        }

        public async Task<List<DepositDTO>> GetTransactionsByMessId(Guid MessId)
        {
            return await dbset
                .Include(person=> person.Person)
                .Where(s=>s.MessId == MessId)
                .Select(item=>new DepositDTO()
                {
                    Amount = item.Amount,
                    Success=item.Success,
                    PersonName=item.Person.Name,
                    CreatedAt=item.CreatedAt,
                }).OrderByDescending(e=>e.CreatedAt).ToListAsync();
        }

        public async Task<List< Deposit > > GetTransactionsByMessIdAndPersonId(Guid MessId, Guid PersonId)
        {
            return await dbset.Where(e=>e.PersonId==PersonId &&  e.MessId==MessId).ToListAsync();
        }

        public async Task<List<DepositDTO>> GetTransactionsByPersonId(Guid PersonId)
        {
            return await dbset
                .Include(m=>m.Mess)
                .Where(s => s.PersonId==PersonId)
                .Select(item => new DepositDTO()
                {
                    Amount = item.Amount,
                    Success = item.Success,
                    Messname=item.Mess.Name,
                    CreatedAt = item.CreatedAt,
                }).OrderByDescending(e => e.CreatedAt).ToListAsync();
        }
    }
}
