using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models
{
    public partial class Departments
    {
        public Departments()
        {
            Employees = new HashSet<Employees>();
        }

        public int DeptId { get; set; }
        public string DeptTitle { get; set; }

        public ICollection<Employees> Employees { get; set; }
    }
}
