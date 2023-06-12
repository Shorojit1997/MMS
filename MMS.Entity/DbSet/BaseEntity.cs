using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.Entities.DbSet
{
    public class BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public int? Status { get; set; } = 1;
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }=DateTime.UtcNow;
    }
}
