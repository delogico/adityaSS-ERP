using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models;

public partial class Wage_Register_Allowance
{
    public int WAA_Id { get; set; }

    public int WAR_Id { get; set; }

    public int CRA_Id { get; set; }

    public decimal WAA_Amount { get; set; }

    public decimal WAA_Amount_Calculated { get; set; }

    public virtual Client_Requirement_Allowance CRA { get; set; }

    public virtual Wage_Register WAR { get; set; }
}
