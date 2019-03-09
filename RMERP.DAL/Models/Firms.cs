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

        public int FrmId { get; set; }
        public string FrmName { get; set; }

        public ICollection<AdminUsers> AdminUsers { get; set; }
        public ICollection<Clients> Clients { get; set; }
    }
}
