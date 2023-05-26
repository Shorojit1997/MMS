using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using MMS.DataService.Data;
using MMS.DataService.IRepository;
using MMS.Entities.DbSet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MMS.DataService.Repository
{
    public class MessRepository : GenericRepository<Mess>,IMessRepository
    {
        public MessRepository(AppDbContext context, ILogger logger) : base(context, logger)
        {
           
        }
        public async Task<IEnumerable<Mess> > GetByPersonId(Guid Id)
        {
            return null;
        }
    }
}
