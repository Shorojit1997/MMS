using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MMS.DataService.Data;
using MMS.DataService.IRepository;
using MMS.Entities.DbSet;
using MMS.Entities.Dtos.Incomming;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MMS.DataService.Repository
{
    public class PersonRepository : GenericRepository<Person>, IPersonRepository
    {
        public PersonRepository(AppDbContext context, ILogger logger) : base(context, logger)
        {
 
        }

        public async Task<Person> GetByEmail(string email)
        {
            return await dbset.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<IEnumerable<PersonDTO>> GetAllMembersByQueryParameters(string query, string types)
        {

            if (query == null)
            {
                return Enumerable.Empty<PersonDTO>();
            }

            string newQuery = "%"+query+"%";

            if (types == "Email")
                return await dbset.Include(a=>a.Members).Where(u => EF.Functions.Like(u.Email, newQuery)).Select(e=> new PersonDTO (){ Name=e.Name,Email=e.Email,Phone=e.Phone,PictureUrl=e.PictureUrl,Id=e.Id}).ToListAsync();
            else if(types =="Phone")
                return await dbset.Include(a => a.Members).Where(u => EF.Functions.Like(u.Phone, newQuery)).Select(e => new PersonDTO() { Name = e.Name, Email = e.Email, Phone = e.Phone, PictureUrl = e.PictureUrl, Id = e.Id }).ToListAsync();

            else
                return await dbset.Include(a => a.Members).Where(u => EF.Functions.Like(u.Name, newQuery)).Select(e => new PersonDTO() { Name = e.Name, Email = e.Email, Phone = e.Phone , PictureUrl = e.PictureUrl, Id = e.Id }).ToListAsync();
        }

    }
}
