using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MMS.Entities.DbSet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MMS.DataService.Data
{
    public class AppDbContext: IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
  
        public  DbSet<Person> Persons { get; set; }
        public  DbSet<Mess> Messes { get; set; }
        public DbSet<Month> Months { get; set; }
        public DbSet<MessHaveMember> MessHaveMembers { get; set; }

        public DbSet<Account> Accounts { get; set; }
    }
}
