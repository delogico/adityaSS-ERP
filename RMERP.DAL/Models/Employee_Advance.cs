using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models
{
    public partial class Employee_Advance
    {
        public int ADV_Id { get; set; }
        public int EMP_Id { get; set; }
        public decimal ADV_Amount { get; set; }
        public DateTime ADV_RegisteredOn { get; set; }
        public bool ADV_Status { get; set; }
        public int ADM_Id_RegisteredBy { get; set; }
        public int? WAG_Id_Closed_On { get; set; }

        public Employees EMP_ { get; set; }
        public Wage_Process WAG_Id_Closed_OnNavigation { get; set; }
    }
}
