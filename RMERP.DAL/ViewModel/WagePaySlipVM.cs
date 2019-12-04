using System;
using System.Collections.Generic;
using System.Text;
using RMERP.DAL.Models;

namespace RMERP.DAL.ViewModel
{
    public class WagePaySlipMasterVM
    {
        public List<EmployeePaySlipVM> EmployeePaySlipVMs { get; set; }
        public string WAG_Month { get; set; }
        public int FRM_Id { get; set; }
        public string FRM_Name { get; set; }

    }
    public class EmployeePaySlipVM
    {
        public int EMP_Id { get; set; }
        public string EMP_FirstName { get; set; }
        public string EMP_MiddleName { get; set; }
        public string EMP_SurName { get; set; }
        public string EMP_FullName
        {
            get
            {
                return string.Concat(EMP_FirstName + " " + EMP_MiddleName + " " + EMP_SurName);
            }
        }
        public bool IsPaySlipGenerated { get; set; }
        public DateTime? WPS_GeneratedOn { get; set; }

    }
}
