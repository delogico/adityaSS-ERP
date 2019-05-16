using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RMERP.DAL.ViewModel
{
    public class ESICReportVM
    {
        [Display(Name = "IP Number")]
        public string IP_Number { get; set; }
        public string EMP_FirstName { get; set; }
        public string EMP_MiddleName { get; set; }
        public string EMP_SurName { get; set; }
        [Display(Name = "IP Name")]
        public string IP_Name
        {
            get
            {
                return string.Concat(EMP_FirstName + " " + EMP_MiddleName + " " + EMP_SurName);
            }
        }
        [Display(Name = "No of Days for which wages paid/payable during the month")]
        public double PayableDays { get; set; }
        [Display(Name = "Total Monthly Wages")]
        public decimal TotalMonthlyWages { get; set; }
        [Display(Name = "Reason Code For Zero Working Days")]
        public string ReasonCode { get; set; }
        [Display(Name = "Last Working Day")]
        public DateTime LastWorkingDay { get; set; }
    }
}
