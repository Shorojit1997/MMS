using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.Entities.Dtos.Incomming
{
    public class CurrentDayCalculationDTO
    {
        public int Number { get; set; }
        public int BreakFast { get; set; } = 0;
        public int Lunch { get; set; } = 0;
        public int Dinner { get; set; } = 0;
    }
}
