using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models
{
    public partial class Cities_all
    {
        public Cities_all()
        {
            Employees = new HashSet<Employees>();
        }

        public int CITY_Id { get; set; }
        public string CITY_Name { get; set; }
        public int STA_Id { get; set; }

        public ICollection<Employees> Employees { get; set; }
    }
}
