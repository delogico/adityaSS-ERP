using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models
{
    public partial class Cities
    {
        public Cities()
        {
            Clients = new HashSet<Clients>();
        }

        public int CIT_Id { get; set; }
        public string CIT_Name { get; set; }

        public ICollection<Clients> Clients { get; set; }
    }
}
