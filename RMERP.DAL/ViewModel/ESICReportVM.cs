using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RMERP.DAL.ViewModel
{
    public class ESICReportEmpWiseVM
    {
        public string NAME_OF_COMPANY { get; set; }
        public List<ESICReportVM> ESICReportVMs { get; set; }        
    }   

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

        public string NAME_OF_COMPANY { get; set; }
        public int NO_OF_EMPLOYEE { get; set; }
        public decimal TOTAL_WAGES { get; set; }
        public decimal EMPLOYEES_CONTRIBUTION { get; set; }
        public decimal EMPLOYERS_CONTRIBUTION { get; set; }
        public decimal TOTAL_CONTRIBUTION { get; set; }

       
    }

}
