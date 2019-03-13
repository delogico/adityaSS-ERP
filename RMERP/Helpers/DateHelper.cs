using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RMERP.Helpers
{
    public static class DateHelper
    {
        public static string getDateWithFormat(DateTime dt)
        {
            return dt.ToShortDateString();
        }
    }
}
