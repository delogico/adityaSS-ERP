using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models;

public partial class Wage_Register
{
    public int WAR_Id { get; set; }

    public int WAG_Id { get; set; }

    public int CLI_Id { get; set; }

    public int EMP_Id { get; set; }

    public int CRI_Id { get; set; }

    public double WAR_TotalPaybleDays { get; set; }

    public double WAR_TotalWorkingDays { get; set; }

    public double WAR_ExtraWorkingHours { get; set; }

    public decimal WAR_Basic { get; set; }

    public decimal WAR_Basic_Calculated { get; set; }

    public decimal WAR_DA { get; set; }

    public decimal WAR_DA_Calculated { get; set; }

    public decimal WAR_HRA { get; set; }

    public decimal WAR_HRA_Calculated { get; set; }

    public string WAR_OverTime_Formula { get; set; }

    public int? WAR_OverTime_Payment { get; set; }

    public int WAR_WorkingHrs_In_Day { get; set; }

    public decimal WAR_OverTime_Calculated { get; set; }

    public decimal WAR_GrossTotal { get; set; }

    public decimal WAR_PF { get; set; }

    public string WAR_PF_Formula { get; set; }

    public decimal WAR_PF_Calculated { get; set; }

    public decimal WAR_ESIC { get; set; }

    public string WAR_ESIC_Formula { get; set; }

    public decimal WAR_ESIC_Calculated { get; set; }

    public string WAR_ProffesionalTax_Calculated { get; set; }

    public string WAR_RevenueDeduction_Calculated { get; set; }

    public string WAR_CanteenFacility_Calculation { get; set; }

    public decimal? WAR_FinalTotal { get; set; }

    public DateTime? WAR_LastModifiedOn { get; set; }

    public int? ADM_LastModifiedBy { get; set; }

    public decimal? WAR_Advance_Amount { get; set; }

    public decimal? WAR_OutStation_Allowance_Calculated { get; set; }

    public decimal? WAR_Attendance_Allowance_Calculated { get; set; }

    public decimal? WAR_Performance_Allowance_Calculated { get; set; }

    public decimal? WAR_Nightshift_Allowance_Calculated { get; set; }

    public decimal? WAR_LWF_Deduction_Employer { get; set; }

    public decimal? WAR_LWF_Deduction_Employee { get; set; }

    public int? CLE_Id { get; set; }

    public decimal? WAR_Allowance_Calculated_1 { get; set; }

    public decimal? WAR_Allowance_Calculated_2 { get; set; }

    public decimal? WAR_Allowance_Calculated_3 { get; set; }

    public decimal? WAR_Allowance_Calculated_4 { get; set; }

    public decimal? WAR_Allowance_Calculated_5 { get; set; }

    public decimal? WAR_Allowance_Calculated_6 { get; set; }

    public decimal? WAR_Allowance_Calculated_7 { get; set; }

    public decimal? WAR_Allowance_Calculated_8 { get; set; }

    public decimal? WAR_Allowance_Calculated_9 { get; set; }

    public decimal? WAR_Allowance_Calculated_10 { get; set; }

    public virtual Client CLI { get; set; }

    public virtual Client_Requirement CRI { get; set; }

    public virtual Employee EMP { get; set; }

    public virtual Wage_Process WAG { get; set; }

    public virtual ICollection<Wage_Register_Allowance> Wage_Register_Allowances { get; set; } = new List<Wage_Register_Allowance>();
}
