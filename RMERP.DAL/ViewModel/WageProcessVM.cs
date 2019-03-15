using System;
using System.Collections.Generic;
using System.Text;
using RMERP.DAL.Models;

namespace RMERP.DAL.ViewModel
{
    public class WageProcessVM
    {
        public int WAG_Id { get; set; }
        public DateTime WAG_Month { get; set; }
        public List<Attendance> Attendance { get; set; }

        public string WAG_Full_Month()
        {
            return WAG_Month.ToString("MMMM") + "-" + WAG_Month.ToString("yyyy");
        }
    }

    public class WageProcessClientAttendancePageVM
    {
        public WageProcessVM wageProcess { get; set; }
        public List<WageProcessClientAttendanceVM> lstClient { get; set; }
    }

    public class WageProcessClientAttendanceVM
    {
        public int CLI_Id { get; set; }
        public string CLI_Name { get; set; }
        public int totalEmployees { get; set; }
    }
}
