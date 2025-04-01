using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models;

public partial class Wage_Process_Client
{
    public int WPC_Id { get; set; }

    public int WAG_Id { get; set; }

    public int CLI_Id { get; set; }

    public bool WPC_WageRegisterSaved { get; set; }

    public DateTime WPC_SavedOn { get; set; }

    public int ADM_Id_SavedBy { get; set; }

    public virtual Client CLI { get; set; }

    public virtual Wage_Process WAG { get; set; }
}
