using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models
{
    public partial class Wage_Process
    {
        public Wage_Process()
        {
            Attendance = new HashSet<Attendance>();
        }

        public int WAG_Id { get; set; }
        public DateTime WAG_Month { get; set; }
        public DateTime WAG_RegisteredOn { get; set; }
        public int ADM_Id_RegisteredBy { get; set; }

        public ICollection<Attendance> Attendance { get; set; }
    }
}
