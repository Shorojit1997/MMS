using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.Entities.Dtos.Incomming
{
    public class MonthDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }

        public Guid? MessId { get; set; }
    }
}
