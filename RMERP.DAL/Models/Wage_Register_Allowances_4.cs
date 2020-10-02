using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models
{
    public partial class Wage_Register_Allowances_4
    {
        public int WRA_Id_4 { get; set; }
        public int WAG_Id { get; set; }
        public int CLE_Id { get; set; }
        public decimal WRA_Amount_4 { get; set; }

        public Clients_Employees CLE_ { get; set; }
        public Wage_Process WAG_ { get; set; }
    }
}
