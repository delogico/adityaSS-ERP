using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models
{
    public partial class Wage_PaySlips
    {
        public int WPS_Id { get; set; }
        public int WAG_Id { get; set; }
        public int EMP_Id { get; set; }
        public string WPS_FileName { get; set; }
        public int WPS_Status { get; set; }
        public DateTime WPS_GeneratedOn { get; set; }

        public Employees EMP_ { get; set; }
        public Wage_Process WAG_ { get; set; }
    }
}
