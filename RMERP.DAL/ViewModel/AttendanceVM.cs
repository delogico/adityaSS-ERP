using System;
using System.Collections.Generic;
using System.Text;
using RMERP.DAL.Models;

namespace RMERP.DAL.ViewModel
{
    public class AttendanceVM
    {
        public EmployeeVM employee { get; set; }
        public WageProcessVM wage_Process { get; set; }
        public Designations designation { get; set; }
        public int ATT_Id { get; set; }
        public int WAG_Id { get; set; }
        public int EMP_Id { get; set; }
        public int DES_Id { get; set; }
        public int CLI_Id { get; set; }
        public DateTime ATT_Date { get; set; }
        public bool ATT_IsPresent { get; set; }
        public bool ATT_IsPaidHoliday { get; set; }
        public string ATT_Shift { get; set; }
        public bool ATT_IsWeeklyOff { get; set; }
        public bool ATT_IsEarnLeave { get; set; }
        public double ATT_ExtraHoursWorked { get; set; }
    }
}
