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
    public class PersonRepository : GenericRepository<Person>, IPersonRepository
    {
        public PersonRepository(AppDbContext context, ILogger logger) : base(context, logger)
        {
        }
    }
}
