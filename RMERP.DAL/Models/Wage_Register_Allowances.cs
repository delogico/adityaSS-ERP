using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models
{
    public partial class Wage_Register_Allowances
    {
        public int WAA_Id { get; set; }
        public int WAR_Id { get; set; }
        public int CRA_Id { get; set; }
        public decimal WAA_Amount { get; set; }
        public decimal WAA_Amount_Calculated { get; set; }

        public Client_Requirement_Allowances CRA_ { get; set; }
        public Wage_Register WAR_ { get; set; }
    }
}
