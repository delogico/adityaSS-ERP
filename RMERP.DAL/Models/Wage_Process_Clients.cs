using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models
{
    public partial class Wage_Process_Clients
    {
        public int WPC_Id { get; set; }
        public int WAG_Id { get; set; }
        public int CLI_Id { get; set; }
        public bool WPC_WageRegisterSaved { get; set; }
        public DateTime WPC_SavedOn { get; set; }
        public int ADM_Id_SavedBy { get; set; }

        public Clients CLI_ { get; set; }
        public Wage_Process WAG_ { get; set; }
    }
}
