using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.Entities.DbSet
{
    public class Account:BaseEntity
    {
        public string  ClientId { get; set; }
        public string ClientSecret { get; set; }
        public Guid PersonId { get; set; }
        public Person Person { get; set; }
    }
}
