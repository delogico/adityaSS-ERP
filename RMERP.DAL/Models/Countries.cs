using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models
{
    public partial class Countries
    {
        public Countries()
        {
            States = new HashSet<States>();
        }

        public int COU_Id { get; set; }
        public string COU_Code { get; set; }
        public string COU_Name { get; set; }

        public ICollection<States> States { get; set; }
    }
}
