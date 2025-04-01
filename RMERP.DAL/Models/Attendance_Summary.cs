using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models;

public partial class Attendance_Summary
{
    public int ATS_Id { get; set; }

    public int WAG_Id { get; set; }

    public int CLI_Id { get; set; }

    public int EMP_Id { get; set; }

    public int DES_Id { get; set; }

    public double ATS_PresentDays { get; set; }

    public double ATS_HalfDays { get; set; }

    public double ATS_WeekOff { get; set; }

    public double ATS_PublicHolidays { get; set; }

    public double ATS_EarnLeaves { get; set; }

    public double ATS_NightShifts { get; set; }

    public double ATS_ExtraHours { get; set; }

    public string ATS_TemplateReference { get; set; }

    public DateTime ATS_ImportedOn { get; set; }

    public int ADM_Id_ImportedBy { get; set; }

    public virtual Client CLI { get; set; }

    public virtual Designation DES { get; set; }

    public virtual Employee EMP { get; set; }

    public virtual Wage_Process WAG { get; set; }
}
