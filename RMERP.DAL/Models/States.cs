using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models
{
    public partial class States
    {
        public States()
        {
            Clients = new HashSet<Clients>();
            Employees = new HashSet<Employees>();
            Firms = new HashSet<Firms>();
        }

        public int STA_Id { get; set; }
        public string STA_Name { get; set; }
        public int COU_Id { get; set; }
        public string STA_GST_Code { get; set; }

        public Countries COU_ { get; set; }
        public ICollection<Clients> Clients { get; set; }
        public ICollection<Employees> Employees { get; set; }
        public ICollection<Firms> Firms { get; set; }
    }
}
