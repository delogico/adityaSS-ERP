using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models;

public partial class Wage_Register_Advance
{
    public int WAD_Id { get; set; }

    public int WAG_Id { get; set; }

    public int EMP_Id { get; set; }

    public decimal WAD_Amount { get; set; }

    public int? CLI_Id { get; set; }

    public bool WAD_Status { get; set; }

    public bool WAD_Is_LoanCompleted { get; set; }

    public DateTime? WAD_ClosedOn { get; set; }

    public virtual Employee EMP { get; set; }

    public virtual Wage_Process WAG { get; set; }
}
