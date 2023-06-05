using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using MMS.DataService.Data;
using MMS.DataService.IRepository;
using MMS.Entities.DbSet;
using MMS.Entities.Dtos.Incomming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.DataService.Repository
{
    public class MessMemberRepository : GenericRepository<MessHaveMember>, IMessMemberRepository
    {
        public MessMemberRepository(AppDbContext context, ILogger logger) : base(context, logger)
        {
         
        }
        public async Task<IEnumerable<Mess> > GetByPersonId(Guid personId)
        {
             return await dbset.Include(m=>m.Messes).Where(a=>a.PersonId==personId).Select(item=>item.Messes).OrderByDescending(d=>d.UpdatedAt).ToListAsync();
        }

        public async Task<IEnumerable<PersonDTO>> GetAllMembersByMessId(Guid Id)
        {
            var member= await dbset.Include(p=>p.Persons)
                .Where(a=>a.MessId==Id)
                .Select(item=>new PersonDTO() {
                    Id = item.Persons.Id,
                    Name=item.Persons.Name,
                    Email=item.Persons.Email,
                    IsManager=item.IsManager,
                    Phone=item.Persons.Phone,
                })
                .ToListAsync();
            return member;
        }

        public override async Task<bool> Add(MessHaveMember entity)
        {
            var isExist=await dbset.Where(e=> e.MessId==entity.MessId && e.PersonId==entity.PersonId).ToListAsync();
            if(isExist.Any())
            {
                return true;
            }
            return await base.Add(entity);
        }

        public async Task<bool> RemoveByMessIdAndPersonId(Guid messId, Guid personId,Guid currentPersonId)
        {
            var entities = await dbset.FirstOrDefaultAsync(e => e.MessId == messId && e.PersonId == personId);
            if (entities!=null)
            {
                var person = dbset.FirstOrDefault(e => e.MessId == messId && e.PersonId == currentPersonId);
                if (person!=null && person.IsManager) {
                    dbset.Remove(entities);
                }
               
                return true;
            }
            return false;
        }

        public async Task<MessHaveMember> GetByMessIdAndPersonId(Guid messId, Guid personId)
        {
            return await dbset.FirstOrDefaultAsync(e => e.MessId == messId && e.PersonId == personId);
            
        }

        public async Task<IEnumerable<Guid>> GetAllManagersDetailsByMessId(Guid messId)
        {
            return await dbset
                .Include(e => e.Persons)
                .Where(x => x.MessId == messId && x.IsManager == true)
                .Select(item => (Guid)item.PersonId)
                .ToListAsync();
        }
    }
}
