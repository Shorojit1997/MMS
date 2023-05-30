using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.Entities.DbSet
{
    public class Mess:BaseEntity
    {
        public string Name { get; set; }

        public string StartDate { get; set; }

        public List<MessHaveMember> Members { get; set; } = new List<MessHaveMember>();

        public List<Month> Months { get; set; } = new List<Month>();
    }
}
