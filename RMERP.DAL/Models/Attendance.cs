using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models;

public partial class Attendance
{
    public int ATT_Id { get; set; }

    public int WAG_Id { get; set; }

    public int EMP_Id { get; set; }

    public int DES_Id { get; set; }

    public int CLI_Id { get; set; }

    public DateOnly ATT_Date { get; set; }

    public bool ATT_IsPresent { get; set; }

    public bool ATT_IsHalfday { get; set; }

    public bool ATT_IsPublicHoliday { get; set; }

    public string ATT_Shift { get; set; }

    public bool ATT_IsWeeklyOff { get; set; }

    public bool ATT_IsEarnLeave { get; set; }

    public bool ATT_NightShift { get; set; }

    public string ATT_Orignal_Row1 { get; set; }

    public string ATT_Orignal_Row2 { get; set; }

    public double ATT_ExtraHoursWorked { get; set; }

    public DateTime ATT_ImportedOn { get; set; }

    public int ADM_Id_ImportedBy { get; set; }

    public virtual Client CLI { get; set; }

    public virtual Designation DES { get; set; }

    public virtual Employee EMP { get; set; }

    public virtual Wage_Process WAG { get; set; }
}
