using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace MMS.Entities.Dtos.Incomming
{
    public class DepositDTO
    {
        public int Amount { get; set; }
        public Guid MessId { get; set; }
        public string? PersonName { get; set; }
        public bool? Success { get; set; }=false;
        public string? Messname { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
