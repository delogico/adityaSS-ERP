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

        public int CITY_Id { get; set; }
        public string CITY_Name { get; set; }

        public ICollection<Clients> Clients { get; set; }
    }
}
