using Microsoft.EntityFrameworkCore;
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
    public class GenericRepository<T>:IGenericRepository<T> where T: class
    {
        protected readonly AppDbContext _context;
        protected readonly ILogger _logger;
        internal DbSet<T> dbset;
        public GenericRepository(AppDbContext context, ILogger logger) { 
            _context = context;
            _logger = logger;
            dbset = _context.Set<T>();
        
        }

        public virtual async Task<bool> Add(T entity)
        {
            await dbset.AddAsync(entity);
            return true;
        }

        public virtual async Task<IEnumerable<T>> All()
        {
            return await dbset.ToListAsync();
        }

        public virtual async Task<bool> Delete(Guid id, string userId)
        {
            throw new NotImplementedException();
        }


        public virtual async Task<T> GetById(Guid id)
        {
            return await dbset.FindAsync(id);
        }

        public async void Update(T entity)
        {
            dbset.Update(entity);
             _context.SaveChanges();
            return;
        }
    }
}
