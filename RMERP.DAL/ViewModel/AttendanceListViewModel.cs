using RMERP.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RMERP.DAL.ViewModel
{
    public class AttendanceListViewModel
    {
        public int EmpID { get; set; }
        public string EmpName { get; set; }
        public string EmpDesignation { get; set; }
        public string WorkingHours { get; set; }
        public double ExtraWorkingHours { get; set; }
        public List<UserData> UserDataList { get; set; }
        public AttendanceViewModel attendanceViewModel { get; set; }
        public List<Attendance> attendancesList { get; set; }
        public Attendance attendanceModel { get; set; }
        public UserData userData { get; set; }
        
    }
    public class UserData
    {
        public DateTime AttendanceDate { get; set; }
        public bool IsPresent { get; set; }
        public bool AttIsWeeklyOff { get; set; }
        public double AttExtraHoursWorked { get; set; }
        
    }
    public class AttendanceViewModel
    {
        [Key]
        public int AttId { get; set; }
        [Required]
        public int WagId { get; set; }
        [Required]
        public int EmpId { get; set; }
        public int CriId { get; set; }
        [Required]
        public int CliId { get; set; }
        [Required]
        [Display(Name ="Date")]
        public DateTime AttDate { get; set; }
        [Display(Name = "Present")]
        public bool AttIsPresent { get; set; }
        [Display(Name = "Paid Holiday")]
        public bool AttIsPaidHoliday { get; set; }
        [Display(Name = "Shift")]
        public string AttShift { get; set; }
        [Display(Name = "Weekly Off")]
        public bool AttIsWeeklyOff { get; set; }
        [Display(Name = "Earn Leave")]
        public bool AttIsEarnLeave { get; set; }
        [Display(Name = "Extra Working Hours")]
        public double AttExtraHoursWorked { get; set; }
        [Display(Name = "Imported On")]
        public DateTime AttImportedOn { get; set; }
        public int AdmIdImportedBy { get; set; }

    }

}
