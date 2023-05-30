using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.Entities.DbSet
{
    public class Deposit:BaseEntity
    {
        public int Amount { get; set; }
        public bool Success { get; set; }
    }
}
