using System;
using System.Collections.Generic;
using System.Linq;
using RMERP.DAL.Models;

namespace RMERP.Helpers
{
    public static class DateHelper
    {
        public static string getDateWithFormat(DateTime dt)
        {
            return dt.ToShortDateString();
        }
        public static string getLongDateFormat(DateTime dt)
        {
            return dt.ToString("dd-MMM-yyyy");
        }

        public static DateTime[] getStartEndDatePeriodForAttendance(Clients client, DateTime wageDate)
        {
            DateTime startDate = wageDate, endDate = wageDate;
            if (client.CLI_Att_MonthReal == true)
            {
                startDate = new DateTime(wageDate.Year, wageDate.Month, 1);
                endDate = startDate.AddMonths(1).AddDays(-1);
            }
            else if (client.CLI_Att_MonthReal == false)
            {
                startDate = new DateTime(wageDate.AddMonths(-1).Year, wageDate.AddMonths(-1).Month, client.CLI_Att_Month_Start.Value);
                endDate = new DateTime(wageDate.Year, wageDate.Month, client.CLI_Att_Month_End.Value); ;
            }
            DateTime[] arr = { startDate, endDate };
            return arr;
        }
    }
}
