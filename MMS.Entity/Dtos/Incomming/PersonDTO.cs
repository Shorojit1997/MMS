using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.Entities.Dtos.Incomming
{
    public class PersonDTO
    {
     
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }

        public bool?  IsManager { get; set; }
        public string? PictureUrl { get; set; } = "/images/Default.jpg";


    }
}
