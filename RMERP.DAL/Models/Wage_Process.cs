using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models;

public partial class Wage_Process
{
    public int WAG_Id { get; set; }

    public int FRM_Id { get; set; }

    public DateOnly WAG_Month { get; set; }

    public DateTime WAG_RegisteredOn { get; set; }

    public bool WAG_Status { get; set; }

    public int ADM_Id_RegisteredBy { get; set; }

    public virtual ICollection<Attendance_Summary> Attendance_Summaries { get; set; } = new List<Attendance_Summary>();

    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    public virtual ICollection<Employee_Advance> Employee_Advances { get; set; } = new List<Employee_Advance>();

    public virtual Firm FRM { get; set; }

    public virtual ICollection<Wage_PaySlip> Wage_PaySlips { get; set; } = new List<Wage_PaySlip>();

    public virtual ICollection<Wage_Process_Client> Wage_Process_Clients { get; set; } = new List<Wage_Process_Client>();

    public virtual ICollection<Wage_Register_Advance> Wage_Register_Advances { get; set; } = new List<Wage_Register_Advance>();

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

    public virtual ICollection<Wage_Register> Wage_Registers { get; set; } = new List<Wage_Register>();
}
