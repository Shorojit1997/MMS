using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.Entities.DbSet
{
    public class MessHaveMember:BaseEntity
    {
        public bool IsManager { get; set; } = false;
        public Guid PersonId { get; set; }
        public Guid MessId { get; set; }
        public Person? Persons { get; set; }
        public Mess? Messes { get; set; }

    }
}
