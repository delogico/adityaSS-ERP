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

        public int CityId { get; set; }
        public string CityName { get; set; }

        public ICollection<Clients> Clients { get; set; }
    }
}
