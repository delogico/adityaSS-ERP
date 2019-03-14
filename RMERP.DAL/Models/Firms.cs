using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models
{
    public partial class Firms
    {
        public Firms()
        {
            AdminUsers = new HashSet<AdminUsers>();
            Clients = new HashSet<Clients>();
        }

        public int FRM_Id { get; set; }
        public string FRM_Name { get; set; }

        public ICollection<AdminUsers> AdminUsers { get; set; }
        public ICollection<Clients> Clients { get; set; }
    }
}
