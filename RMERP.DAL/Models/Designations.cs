using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models
{
    public partial class Designations
    {
        public Designations()
        {
            ClientRequirements = new HashSet<ClientRequirements>();
            ClientsEmployees = new HashSet<ClientsEmployees>();
        }

        public int DesId { get; set; }
        public string DesTitle { get; set; }

        public ICollection<ClientRequirements> ClientRequirements { get; set; }
        public ICollection<ClientsEmployees> ClientsEmployees { get; set; }
    }
}
