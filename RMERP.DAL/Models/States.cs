using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models
{
    public partial class States
    {
        public States()
        {
            Employees = new HashSet<Employees>();
        }

        public int STA_Id { get; set; }
        public string STA_Name { get; set; }
        public int COU_Id { get; set; }

        public Countries COU_ { get; set; }
        public ICollection<Employees> Employees { get; set; }
    }
}
