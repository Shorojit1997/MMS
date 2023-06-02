using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.Entities.Dtos.Incomming
{
    public class ExpenseDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public double Amount { get; set; }
        public Guid? MessId { get; set; }
        public Guid MonthId { get; set; }
    }
}
