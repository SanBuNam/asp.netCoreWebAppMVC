using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace asp.netCoreWebAppMVC.General
{
    // https://docs.microsoft.com/en-us/dotnet/api/system.exception?view=net-5.0
    public class TryParseBetter
    {
        // If you're not sure string is a valid DateTime, use DateTime.TryParse()
        DateTime dt1 = DateTime.Parse(""); // throws FormatException, not a valid format

        DateTime dt2 = DateTime.Parse("02/30/2010 12:35"); // throws FormatException, February doesn''t have a 30th day, not a valid date

        // It is recommended to use DateTime.TryParse() method to avoid unexpected exceptions especially when accepting inputs from users.

        public DateTime Parse(string inputString)
        {
            DateTime result;
            if(!DateTime.TryParse(inputString, out result))
            {
                throw new FormatException("Some format handling message.");
            }
            return result;
        }
    }
}
