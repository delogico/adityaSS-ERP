using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models;

public partial class Wage_PaySlip
{
    public int WPS_Id { get; set; }

    public int WAG_Id { get; set; }

    public int EMP_Id { get; set; }

    public string WPS_FileName { get; set; }

    public int WPS_Status { get; set; }

    public DateTime WPS_GeneratedOn { get; set; }

    public virtual Employee EMP { get; set; }

    public virtual Wage_Process WAG { get; set; }
}
