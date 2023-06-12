using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.Entities.DbSet
{
    public class Days:BaseEntity
    {
        public Guid Month_Id { get; set; }
        public Guid Person_Id { get; set; } 
        public int Number { get; set; }
        public int Breakfast { get; set; } = 0;
        public int Lunch { get; set; }= 0;
        public int Dinner { get; set; } = 0;
        public bool IsEnd { get; set; } = false;

        public bool IsStart { get; set; }= false;
    }
}
