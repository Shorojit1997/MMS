using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MMS.Entities.DbSet;
using System.Reflection.Emit;


namespace MMS.DataService.Data
{
    public class AppDbContext: IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            
            builder.Entity<Month>()
              .HasMany(m => m.Expenses)
              .WithOne(e => e.Month)
              .HasForeignKey(e => e.MonthId)
              .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Mess>()
              .HasMany(m => m.Months)
              .WithOne(month => month.Mess)
              .HasForeignKey(month => month.MessId)
              .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Mess>()
                .HasMany(m => m.Deposits)
                .WithOne(deposit => deposit.Mess)
                .HasForeignKey(deposit => deposit.MessId)
                .OnDelete(DeleteBehavior.Cascade);
            base.OnModelCreating(builder);
        }

        public  DbSet<Person> Persons { get; set; }
        public  DbSet<Mess> Messes { get; set; }
        public DbSet<Month> Months { get; set; }
        public DbSet<MessHaveMember> MessHaveMembers { get; set; }

        public DbSet<Account> Accounts { get; set; }

        public DbSet<Deposit> Deposits { get; set; }
        public DbSet<Expense> Expenses { get; set; }

        public DbSet<Days> Days { get; set; }

    }
}
