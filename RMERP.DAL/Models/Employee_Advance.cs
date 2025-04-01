using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models;

public partial class Employee_Advance
{
    public int ADV_Id { get; set; }

    public int EMP_Id { get; set; }

    public decimal ADV_Amount { get; set; }

    public DateTime ADV_RegisteredOn { get; set; }

    /// <summary>
    /// false: Pending True: Done
    /// </summary>
    public bool ADV_Status { get; set; }

    public int ADM_Id_RegisteredBy { get; set; }

    public int? WAG_Id_Closed_On { get; set; }

    public virtual Employee EMP { get; set; }

    public virtual Wage_Process WAG_Id_Closed_OnNavigation { get; set; }
}
