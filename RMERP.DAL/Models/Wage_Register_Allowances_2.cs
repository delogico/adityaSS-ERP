using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models;

public partial class Wage_Register_Allowances_2
{
    public int WRA_Id_2 { get; set; }

    public int WAG_Id { get; set; }

    public int CLE_Id { get; set; }

    public decimal WRA_Amount_2 { get; set; }

    public virtual Clients_Employee CLE { get; set; }

    public virtual Wage_Process WAG { get; set; }
}
