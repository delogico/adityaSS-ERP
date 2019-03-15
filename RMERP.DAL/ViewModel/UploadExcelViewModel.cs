using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using RMERP.DAL.ViewModel;
using RMERP.DAL.Models;

namespace RMERP.DAL.ViewModel
{
    public class UploadExcelViewModel
    {
        public WageProcessVM wageProcessVM { get; set; }
        public IFormFile ExcelFile { get; set; }
        public Clients client { get; set; }
        public string Template { get; set; }       
    }

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

        public List<Employees> empListExtraInExcel { get; set; }
        public List<Employees> EmpListExtraInDb { get; set; }
        public bool btnExportToDatabase { get; set; } = true;
        public bool datePeriod { get; set; } = true;
    }
    public class ExcelRowViewModel
    {
        public string EMP_Id { get; set; }
        public string EMP_Name { get; set; }
        public string Designation { get; set; }
        public int TotalPresenceDays { get; set; }
        public Double TotalExtraHours { get; set; }
    }
        
}
