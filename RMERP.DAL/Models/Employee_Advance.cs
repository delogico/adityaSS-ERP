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
        public int ADM_Id_RegisteredBy { get; set; }

        public Employees EMP_ { get; set; }
    }
}
