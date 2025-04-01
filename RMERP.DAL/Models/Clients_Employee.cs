using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models;

public partial class Clients_Employee
{
    public int CLE_Id { get; set; }

    public int CLI_Id { get; set; }

    public int EMP_Id { get; set; }

    public int DES_Id { get; set; }

    public DateTime CLE_RegisteredOn { get; set; }

    public DateTime? CLE_UnassignedOn { get; set; }

    public int? ADM_Id_UnassignedBy { get; set; }

    public int ADM_Id_RegisteredBy { get; set; }

    public DateTime? CLE_ReassignedOn { get; set; }

    public virtual Client CLI { get; set; }

    public virtual Designation DES { get; set; }

    public virtual Employee EMP { get; set; }

    public virtual ICollection<Wage_Register_Allowances_10> Wage_Register_Allowances_10s { get; set; } = new List<Wage_Register_Allowances_10>();

    public virtual ICollection<Wage_Register_Allowances_1> Wage_Register_Allowances_1s { get; set; } = new List<Wage_Register_Allowances_1>();

    public virtual ICollection<Wage_Register_Allowances_2> Wage_Register_Allowances_2s { get; set; } = new List<Wage_Register_Allowances_2>();

    public virtual ICollection<Wage_Register_Allowances_3> Wage_Register_Allowances_3s { get; set; } = new List<Wage_Register_Allowances_3>();

    public virtual ICollection<Wage_Register_Allowances_4> Wage_Register_Allowances_4s { get; set; } = new List<Wage_Register_Allowances_4>();

    public virtual ICollection<Wage_Register_Allowances_5> Wage_Register_Allowances_5s { get; set; } = new List<Wage_Register_Allowances_5>();

    public virtual ICollection<Wage_Register_Allowances_6> Wage_Register_Allowances_6s { get; set; } = new List<Wage_Register_Allowances_6>();

    public virtual ICollection<Wage_Register_Allowances_7> Wage_Register_Allowances_7s { get; set; } = new List<Wage_Register_Allowances_7>();

    public virtual ICollection<Wage_Register_Allowances_8> Wage_Register_Allowances_8s { get; set; } = new List<Wage_Register_Allowances_8>();

    public virtual ICollection<Wage_Register_Allowances_9> Wage_Register_Allowances_9s { get; set; } = new List<Wage_Register_Allowances_9>();

    public virtual ICollection<Wage_Register_Canteen> Wage_Register_Canteens { get; set; } = new List<Wage_Register_Canteen>();

    public virtual ICollection<Wage_Register_Outstation> Wage_Register_Outstations { get; set; } = new List<Wage_Register_Outstation>();

    public virtual ICollection<Wage_Register_Performance> Wage_Register_Performances { get; set; } = new List<Wage_Register_Performance>();
}
