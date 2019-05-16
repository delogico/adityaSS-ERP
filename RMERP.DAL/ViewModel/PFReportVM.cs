using System;
using System.Collections.Generic;
using System.Text;
using RMERP.DAL.ViewModel;
using RMERP.DAL.Models;
using System.ComponentModel.DataAnnotations;

namespace RMERP.DAL.ViewModel
{
    public class PFReportVM
    {
        public int Emp_Id { get; set; }

        [Display(Name = "UAN number")]
        public string EMP_UAN_Number { get; set; }
        [Display(Name = "Name")]      
        public string EMP_FirstName { get; set; }
        [Display(Name = "Middle name")]
        public string EMP_MiddleName { get; set; }
        [Display(Name = "Last name")]       
        public string EMP_SurName { get; set; }


        [Display(Name = "GROSS WAGES")]
        public decimal GrossWages { get; set; }
        [Display(Name = "EPF WAGES")]
        public decimal EPFWages { get; set; }
        [Display(Name = "EPS WAGES")]
        public decimal EPSWages { get; set; }
        [Display(Name = "EDLI WAGES")]
        public decimal EDLIWages { get; set; }
        [Display(Name = "EPF CONTRI REMITTED")]
        public decimal EPF_CONTRI_REMITTED { get; set; }
        [Display(Name = "EPS CONTRI REMITTED")]
        public decimal EPS_CONTRI_REMITTED { get; set; }
        [Display(Name = "EPF EPS DIFF REMITTED")]
        public decimal EPF_EPS_DIFF_REMITTED { get; set; }
        [Display(Name = "NCP DAYS")]
        public double NCP_DAYS { get; set; }
        [Display(Name = "REFUND OF ADVANCES")]
        public decimal REFUND_OF_ADVANCES { get; set; }

        [Display(Name = "MEMBER NAME")]
        public string EMP_FullName
        {
            get
            {
                return string.Concat(EMP_FirstName + " " + EMP_MiddleName + " " + EMP_SurName);
            }
        }
    }
}
