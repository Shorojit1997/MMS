using MMS.Entities.DbSet;
using MMS.Entities.Dtos.Incomming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.DataService.IRepository
{
    public interface IPersonRepository:IGenericRepository<Person>
    {
        Task<Person> GetByEmail(string email);
        Task<IEnumerable<PersonDTO>> GetAllMembersByQueryParameters(string query, string types);

    }
}
