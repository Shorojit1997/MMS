using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.DataService.Others
{
    public static class ValidityChecker
    {
        public static bool IsNull(string value)
        {
            if (string.IsNullOrEmpty(value)) { return true; }
            return false;
        }

        public static bool IsValidGuid(string value)
        {
            Guid guid;
           return Guid.TryParse(value, out guid);

        }

        public static bool IsValidGuid(Guid value)
        {
            Guid guid;
            return Guid.TryParse(Convert.ToString(value),out guid);
        }
    }
}
