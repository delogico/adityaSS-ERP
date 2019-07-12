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
        public AttendanceVM attendanceVM { get; set; }
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
        public int ATT_Id { get; set; }
        [Required]
        public int WAG_Id { get; set; }
        [Required]
        public int EMP_Id { get; set; }
        public int DES_Id { get; set; }
        [Required]
        public int CLI_Id { get; set; }
        [Required]
        [Display(Name ="Date")]
        public DateTime ATT_Date { get; set; }
        [Display(Name = "Present")]
        public bool ATT_IsPresent { get; set; }
        [Display(Name = "Halfday")]
        public bool ATT_IsHalfday { get; set; }
        [Display(Name = "Paid Holiday")]
        public bool ATT_IsPaidHoliday { get; set; }
        [Display(Name = "Shift")]
        public string ATT_Shift { get; set; }
        [Display(Name = "Weekly Off")]
        public bool ATT_IsWeeklyOff { get; set; }
        [Display(Name = "Earn Leave")]
        public bool ATT_IsEarnLeave { get; set; }
        [Display(Name = "Night Shift")]
        public bool ATT_IsNightShift { get; set; }
        [Display(Name = "Extra Working Hours")]
        public double ATT_ExtraHoursWorked { get; set; }
        [Display(Name = "Imported On")]
        public DateTime ATT_ImportedOn { get; set; }
        public int ADM_Id_ImportedBy { get; set; }

    }

}

