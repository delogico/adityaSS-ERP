using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models
{
    public partial class Employee_Documents
    {
        public int EMD_Id { get; set; }
        public int EMP_Id { get; set; }
        public int DOT_Id { get; set; }
        public string EMD_Name { get; set; }
        public DateTime EMD_UploadedOn { get; set; }

        public Document_Types DOT_ { get; set; }
        public Employees EMP_ { get; set; }
    }
}
