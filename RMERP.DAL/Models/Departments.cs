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

        public int DEPT_Id { get; set; }
        public string DEPT_Title { get; set; }

        public ICollection<Employees> Employees { get; set; }
    }
}
