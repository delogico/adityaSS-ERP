using System;
using System.Collections.Generic;
using System.Text;

namespace RMERP.DAL.ViewModel
{   
    public class CalculationEditVM
    {
        public int WAR_Id { get; set; }
        public decimal CRI_PF_Percentage { get; set; }
        public decimal CRI_ESIC_Percentage { get; set; }

        public string CRI_PF_Formula { get; set; }
        public string CRI_ESIC_Formula { get; set; }
        public string CRI_OT_Formula { get; set; }

        public decimal WAR_Basic_Calculated { get; set; }
        public decimal CRI_DA_Calculated { get; set; }
        public decimal CRI_HRA_Calculated { get; set; }

        public int totalWorkingDays { get; set; }
        public float totalPaybleDays { get; set; }
        public float ExtraWorkingHours { get; set; }
        
        public decimal WAR_OverTime_Calculated { get; set; }
        public decimal WAR_Outstation_Allowance_Calculated { get; set; }
        public decimal WAR_Attendance_Allowance_Calculated { get; set; }
        public decimal WAR_Nightshift_Allowance_Calculated { get; set; }
        public decimal WAR_Performance_Allowance_Calculated { get; set; }

        public decimal WAR_Allowance_Calculated_1 { get; set; }
        public decimal WAR_Allowance_Calculated_2 { get; set; }
        public decimal WAR_Allowance_Calculated_3 { get; set; }
        public decimal WAR_Allowance_Calculated_4 { get; set; }
        public decimal WAR_Allowance_Calculated_5 { get; set; }
        
        public decimal WAR_Allowance_Calculated_6 { get; set; }
        public decimal WAR_Allowance_Calculated_7 { get; set; }
        public decimal WAR_Allowance_Calculated_8 { get; set; }
        public decimal WAR_Allowance_Calculated_9 { get; set; }
        public decimal WAR_Allowance_Calculated_10 { get; set; }

        public List<CalculatedAllowanceVM> CalculatedAllowanceVM { get; set; }
    }

    public class CalculatedEditVM
    {
        public decimal WAR_PF_Calculated { get; set; }
        public decimal WAR_ESIC_Calculated { get; set; }
    }
    public class CalculatedAllowanceVM
    {
        public int WAA_Id { get; set; }
        public decimal WAA_Amount { get; set; }
        public string ALL_Shortform { get; set; }
        // public bool CRA_Dayswise { get; set; }
        // public bool CRA_Full { get; set; }
        public decimal WAA_Amount_Calculated { get; set; }

    }
}
