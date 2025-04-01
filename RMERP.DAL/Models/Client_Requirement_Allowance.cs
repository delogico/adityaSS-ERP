using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models;

public partial class Client_Requirement_Allowance
{
    public int CRA_Id { get; set; }

    public int CRI_Id { get; set; }

    public int ALL_Id { get; set; }

    public decimal CRA_Amount { get; set; }

    /// <summary>
    /// 1: Daywise Calculation; 0: give all amount without calculation
    /// </summary>
    public bool CRA_DayswiseOrFull { get; set; }

    public virtual Allowance ALL { get; set; }

    public virtual Client_Requirement CRI { get; set; }

    public virtual ICollection<Wage_Register_Allowance> Wage_Register_Allowances { get; set; } = new List<Wage_Register_Allowance>();
}
