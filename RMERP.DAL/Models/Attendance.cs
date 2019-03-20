using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models
{
    public partial class Attendance
    {
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
        public DateTime ATT_ImportedOn { get; set; }
        public int ADM_Id_ImportedBy { get; set; }

        public Clients CLI_ { get; set; }
        public Designations DES_ { get; set; }
        public Employees EMP_ { get; set; }
        public Wage_Process WAG_ { get; set; }
    }
}
