using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models
{
    public partial class Clients_Employees
    {
        public Clients_Employees()
        {
            Wage_Register_Allowances_1 = new HashSet<Wage_Register_Allowances_1>();
            Wage_Register_Allowances_2 = new HashSet<Wage_Register_Allowances_2>();
            Wage_Register_Allowances_3 = new HashSet<Wage_Register_Allowances_3>();
            Wage_Register_Allowances_4 = new HashSet<Wage_Register_Allowances_4>();
            Wage_Register_Allowances_5 = new HashSet<Wage_Register_Allowances_5>();
            Wage_Register_Canteen = new HashSet<Wage_Register_Canteen>();
            Wage_Register_Outstation = new HashSet<Wage_Register_Outstation>();
            Wage_Register_Performance = new HashSet<Wage_Register_Performance>();
        }

        public int CLE_Id { get; set; }
        public int CLI_Id { get; set; }
        public int EMP_Id { get; set; }
        public int DES_Id { get; set; }
        public DateTime CLE_RegisteredOn { get; set; }
        public DateTime? CLE_UnassignedOn { get; set; }
        public int? ADM_Id_UnassignedBy { get; set; }
        public int ADM_Id_RegisteredBy { get; set; }
        public DateTime? CLE_ReassignedOn { get; set; }

        public Clients CLI_ { get; set; }
        public Designations DES_ { get; set; }
        public Employees EMP_ { get; set; }
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
