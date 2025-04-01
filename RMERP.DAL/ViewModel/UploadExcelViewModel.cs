using Microsoft.AspNetCore.Http;
using RMERP.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RMERP.DAL.ViewModel
{
    public class UploadExcelViewModel
    {
        public WageProcessVM WageProcessVM { get; set; }
        [Required(ErrorMessage = "Excel file is required")]
        public IFormFile ExcelFile { get; set; }
        public Client Client { get; set; }
        [Required(ErrorMessage = "Choose Template")]
        public string Template { get; set; }
    }
    
    public class UploadExcelVM
    {
        public WageProcessVM WageProcessVM { get; set; }
        [Required(ErrorMessage = "Excel file is required")]
        public IFormFile ExcelFile { get; set; }
        public Client Client { get; set; }
    }

    public class ATT_ExcelVM
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalEmployees { get; set; } = 0;
        public List<ExcelRowVM> ExcelRows { get; set; }
        public string FileName { get; set; }
        public int WAG_Id { get; set; }
        public int CLI_Id { get; set; }
        public int FRM_Id { get; set; }

        public List<Employee> NotExistsInClient { get; set; }
        public List<Employee> RemainingEmployees { get; set; }
        public bool IsEnableExport { get; set; } = true;
        public bool datePeriod { get; set; } = true;
        public List<Attendance_Summary> AttendanceSummary { get; set; }
        public string Template { get; set; }

    }
    public class ExcelRowVM
    {
        public string EMP_Id { get; set; }
        public string EMP_Name { get; set; }
        public string Designation { get; set; }
        public double TotalPresenceDays { get; set; }
        public int TotalWeeklyOff { get; set; }
        public int TotalHolidays { get; set; }
        public double TotalEarnLeave { get; set; }
        public Double TotalNightshift { get; set; }
        public Double TotalExtraHours { get; set; }
        public double TotalPaybleDays { get; set; }
    }

    #region OLD
    public class ExcelViewModel
    {
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public int totalEmployees { get; set; }
        public int totalPublicHolidays { get; set; }
        public List<ExcelRowViewModel> excelRows { get; set; }
        public string fileName { get; set; }
        public int WAG_Id { get; set; }
        public int CLI_Id { get; set; }
        public int FRM_Id { get; set; }

        public List<Employee> empListExtraInExcel { get; set; }
        public List<Employee> EmpListExtraInDb { get; set; }
        public bool btnExportToDatabase { get; set; } = true;
        public bool datePeriod { get; set; } = true;
        public List<Attendance> listAttendance { get; set; }
        public List<Attendance_Summary> AttendanceSummary { get; set; }

        public string Template { get; set; }
    }
    public class ExcelRowViewModel
    {
        public string EMP_Id { get; set; }
        public string EMP_Name { get; set; }
        public string Designation { get; set; }
        public double totalPresenceDays { get; set; }
        public int totalWeeklyOff { get; set; }
        public int totalHolidays { get; set; }
        public double TotalEarnLeave { get; set; }
        public Double TotalNightshift { get; set; }
        public Double TotalExtraHours { get; set; }
        public double totalPaybleDays { get; set; }
    }
    #endregion

}
