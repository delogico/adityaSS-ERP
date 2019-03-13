using System;
using System.Collections.Generic;

namespace RMERP.DAL.Model
{
    public partial class Attendance
    {
        public int AttId { get; set; }
        public int WagId { get; set; }
        public int EmpId { get; set; }
        public int CriId { get; set; }
        public int CliId { get; set; }
        public DateTime AttDate { get; set; }
        public bool AttIsPresent { get; set; }
        public bool AttIsPaidHoliday { get; set; }
        public string AttShift { get; set; }
        public bool AttIsWeeklyOff { get; set; }
        public bool AttIsEarnLeave { get; set; }
        public double AttExtraHoursWorked { get; set; }
        public DateTime AttImportedOn { get; set; }
        public int AdmIdImportedBy { get; set; }

        public Clients Cli { get; set; }
        public ClientRequirements Cri { get; set; }
        public Employees Emp { get; set; }
        public WageProcess Wag { get; set; }
    }
}
