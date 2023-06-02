using MMS.Entities.DbSet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.Entities.Dtos.Incomming
{
    public class DayResponseDTO
    {
        public bool Success { get; set; }
        public Days? Days { get; set; }
    }
}
