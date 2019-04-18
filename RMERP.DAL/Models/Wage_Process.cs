using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models
{
    public partial class Wage_Process
    {
        public Wage_Process()
        {
            Attendance = new HashSet<Attendance>();
            Wage_Process_Clients = new HashSet<Wage_Process_Clients>();
            Wage_Register = new HashSet<Wage_Register>();
        }

        public int WAG_Id { get; set; }
        public DateTime WAG_Month { get; set; }
        public DateTime WAG_RegisteredOn { get; set; }
        public bool WAG_Status { get; set; }
        public int ADM_Id_RegisteredBy { get; set; }

        public ICollection<Attendance> Attendance { get; set; }
        public ICollection<Wage_Process_Clients> Wage_Process_Clients { get; set; }
        public ICollection<Wage_Register> Wage_Register { get; set; }
    }
}
