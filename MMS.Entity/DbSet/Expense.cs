
using System.ComponentModel.DataAnnotations.Schema;

namespace MMS.Entities.DbSet
{
    public class Expense:BaseEntity
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public double Amount { get; set; }
        public Guid? MessId { get; set; }
        public Guid? MonthId { get; set; }
      
        public Month? Month { get; set; } 

        public Mess? Mess { get; set; } 

    }
}
