using RMERP.DAL.Models;
using System;
using System.Collections.Generic;

namespace RMERP.DAL.ViewModel
{
    #region ADDED ON 19-12-2024
    public class AttendanceSummaryVM
    {
        public EmployeeVM Employee { get; set; }
        public WageProcessVM Wage_Process { get; set; }
        public Designation Designation { get; set; }
        public int ATS_Id { get; set; }
        public int WAG_Id { get; set; }
        public int EMP_Id { get; set; }
        public int CLI_Id { get; set; }
        public DateTime ATS_ImportedOn { get; set; }
        public double ATS_PresentDays { get; set; }
        public double ATS_WeekOff { get; set; }
        public double ATS_PublicHolidays { get; set; }
        public double ATS_EarnLeaves { get; set; }
        public double ATS_NightShifts { get; set; }
        public double ATS_ExtraHours { get; set; }
    }
    #endregion


    public class ViewAttendanceVM
    {
        public List<AttendanceVM> Attendances { get; set; } = [];
        public List<AttendanceSummaryVM> AttendanceSummaries { get; set; } = [];
    }
}
