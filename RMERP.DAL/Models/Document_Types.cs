using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models
{
    public partial class Document_Types
    {
        public Document_Types()
        {
            Employee_Documents = new HashSet<Employee_Documents>();
        }

        public int DOT_Id { get; set; }
        public string DOT_Title { get; set; }

        public ICollection<Employee_Documents> Employee_Documents { get; set; }
    }
}
