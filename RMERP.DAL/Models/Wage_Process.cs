using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models
{
    public partial class Wage_Process
    {
        public Wage_Process()
        {
            Attendance = new HashSet<Attendance>();
            Employee_Advance = new HashSet<Employee_Advance>();
            Wage_PaySlips = new HashSet<Wage_PaySlips>();
            Wage_Process_Clients = new HashSet<Wage_Process_Clients>();
            Wage_Register = new HashSet<Wage_Register>();
            Wage_Register_Advances = new HashSet<Wage_Register_Advances>();
            Wage_Register_Allowances_1 = new HashSet<Wage_Register_Allowances_1>();
            Wage_Register_Allowances_2 = new HashSet<Wage_Register_Allowances_2>();
            Wage_Register_Allowances_3 = new HashSet<Wage_Register_Allowances_3>();
            Wage_Register_Allowances_4 = new HashSet<Wage_Register_Allowances_4>();
            Wage_Register_Allowances_5 = new HashSet<Wage_Register_Allowances_5>();
            Wage_Register_Canteen = new HashSet<Wage_Register_Canteen>();
            Wage_Register_Outstation = new HashSet<Wage_Register_Outstation>();
            Wage_Register_Performance = new HashSet<Wage_Register_Performance>();
        }

        public int WAG_Id { get; set; }
        public int FRM_Id { get; set; }
        public DateTime WAG_Month { get; set; }
        public DateTime WAG_RegisteredOn { get; set; }
        public bool WAG_Status { get; set; }
        public int ADM_Id_RegisteredBy { get; set; }

        public Firms FRM_ { get; set; }
        public ICollection<Attendance> Attendance { get; set; }
        public ICollection<Employee_Advance> Employee_Advance { get; set; }
        public ICollection<Wage_PaySlips> Wage_PaySlips { get; set; }
        public ICollection<Wage_Process_Clients> Wage_Process_Clients { get; set; }
        public ICollection<Wage_Register> Wage_Register { get; set; }
        public ICollection<Wage_Register_Advances> Wage_Register_Advances { get; set; }
        public ICollection<Wage_Register_Allowances_1> Wage_Register_Allowances_1 { get; set; }
        public ICollection<Wage_Register_Allowances_2> Wage_Register_Allowances_2 { get; set; }
        public ICollection<Wage_Register_Allowances_3> Wage_Register_Allowances_3 { get; set; }
        public ICollection<Wage_Register_Allowances_4> Wage_Register_Allowances_4 { get; set; }
        public ICollection<Wage_Register_Allowances_5> Wage_Register_Allowances_5 { get; set; }
        public ICollection<Wage_Register_Canteen> Wage_Register_Canteen { get; set; }
        public ICollection<Wage_Register_Outstation> Wage_Register_Outstation { get; set; }
        public ICollection<Wage_Register_Performance> Wage_Register_Performance { get; set; }
    }
}
