using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models
{
    public partial class Wage_Register_Outstation
    {
        public int WRO_Id { get; set; }
        public int WAG_Id { get; set; }
        public int CLE_Id { get; set; }
        public double WRO_Hours { get; set; }

        public Clients_Employees CLE_ { get; set; }
        public Wage_Process WAG_ { get; set; }
    }
}
