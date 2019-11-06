using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models
{
    public partial class Client_Requirements
    {
        public Client_Requirements()
        {
            Client_Requirement_Allowances = new HashSet<Client_Requirement_Allowances>();
            Wage_Register = new HashSet<Wage_Register>();
        }

        public int CRI_Id { get; set; }
        public int CLI_Id { get; set; }
        public int DES_Id { get; set; }
        public int CRI_Total { get; set; }
        public decimal? CRI_Basic { get; set; }
        public decimal? CRI_DA { get; set; }
        public decimal? CRI_HRA_Fixed { get; set; }
        public double? CRI_HRA_Percentage { get; set; }
        public string CRI_PF_Formula { get; set; }
        public double? CRI_PF_Percentage { get; set; }
        public string CRI_ESIC_Formula { get; set; }
        public double? CRI_ESIC_Percentage { get; set; }
        public string CRI_ESIC_Area { get; set; }
        public bool CRI_ProfessionalTax { get; set; }
        public bool CRI_RevenueDeduction { get; set; }
        public bool CRI_CanteenFacility { get; set; }
        public bool CRI_OT_Calculate_Payableday { get; set; }
        public decimal? CRI_OT_Fixed_PerHour { get; set; }
        public string CRI_OT_Formula { get; set; }
        public decimal? CRI_OT_Rate { get; set; }
        public double? CRI_OT_MultipleTimes { get; set; }
        public bool CRI_IsPayable_WeeklyOff { get; set; }
        public bool CRI_IsPayable_PublicHoliday { get; set; }
        public DateTime CRI_RegisteredOn { get; set; }
        public bool? CRI_Active { get; set; }
        public DateTime? CRI_InactivatedOn { get; set; }
        public int? ADM_Id_InactivatedBy { get; set; }
        public bool CRI_OutStation_Allowance { get; set; }
        public decimal? CRI_OutStation_Allowance_Rate { get; set; }
        public bool CRI_Attendance_Allowance { get; set; }
        public int? CRI_Attendance_Allowance_MaximumDays { get; set; }
        public decimal? CRI_Attendance_Allowance_Rate { get; set; }
        public bool CRI_Performance_Allowance { get; set; }
        public bool CRI_Nightshift_Allowance { get; set; }
        public decimal? CRI_Nightshift_Allowance_Rate { get; set; }
        public int CRI_Billing_Type { get; set; }
        public decimal? CRI_Billing_Amount { get; set; }
        public double? CRI_Billing_ServiceCharge { get; set; }
        public string CRI_Billing_ServiceCharge_Formula { get; set; }

        public Clients CLI_ { get; set; }
        public Designations DES_ { get; set; }
        public ICollection<Client_Requirement_Allowances> Client_Requirement_Allowances { get; set; }
        public ICollection<Wage_Register> Wage_Register { get; set; }
    }
}
