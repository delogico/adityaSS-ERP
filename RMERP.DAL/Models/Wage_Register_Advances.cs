using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models
{
    public partial class Wage_Register_Advances
    {
        public int WAD_Id { get; set; }
        public int WAG_Id { get; set; }
        public int EMP_Id { get; set; }
        public decimal WAD_Amount { get; set; }
        public int? CLI_Id { get; set; }
        public bool WAD_Status { get; set; }
        public DateTime? WAD_ClosedOn { get; set; }

        public Employees EMP_ { get; set; }
        public Wage_Process WAG_ { get; set; }
    }
}
