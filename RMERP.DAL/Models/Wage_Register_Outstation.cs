using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models;

public partial class Wage_Register_Outstation
{
    public int WRO_Id { get; set; }

    public int WAG_Id { get; set; }

    public int CLE_Id { get; set; }

    public double WRO_Hours { get; set; }

    public virtual Clients_Employee CLE { get; set; }

    public virtual Wage_Process WAG { get; set; }
}
