using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RMERP.DAL.Models;

namespace RMERP.DAL.ViewModel
{
    public class ClientRequirementVM
    {
        [Key]
        public int CRI_Id { get; set; }
        public string CLI_Name { get; set; }
        [Display(Name = "Client")]
        [Required(ErrorMessage = "Please select client")]
        public int CLI_Id { get; set; }
        [Display(Name = "Requirement for")]
        [Required(ErrorMessage = "Please select designation")]
        public int DES_Id { get; set; }
        
        [Display(Name = "Total Post")]
        [Required(ErrorMessage = "Please select Total Posts")]
        public int CRI_Total { get; set; }
        [Display(Name = "Basic(INR)")]
        [Required(ErrorMessage = "Please add BASIC")]
        public decimal? CRI_Basic { get; set; }
        [Display(Name = "DA(INR)")]
        [Required(ErrorMessage = "Please add DA")]
        public decimal? CRI_DA { get; set; }

        [Display(Name = "HRA Fixed")]
        public decimal? CRI_HRA_Fixed { get; set; } = 0;
        [Display(Name = "HRA Percentage (%)")]
        public double? CRI_HRA_Percentage { get; set; } = 0;
        [Display(Name = "PF Formula")]
        public string CRI_PF_Formula { get; set; }
        [Display(Name = "PF Percentage (%)")]
        public double? CRI_PF_Percentage { get; set; }
        [Display(Name = "ESIC Formula")]
        public string CRI_ESIC_Formula { get; set; }
        [Display(Name = "ESIC Percentage (%)")]
        public double? CRI_ESIC_Percentage { get; set; }
        [Display(Name = "ESIC Area")]
        public string CRI_ESIC_Area { get; set; }

        [Display(Name = "Professional Tax")]
        public bool CRI_ProfessionalTax { get; set; }
        [Display(Name = "Revenue Deduction")]
        public bool CRI_RevenueDeduction { get; set; }
        [Display(Name = "Canteen Facility")]
        public bool CRI_CanteenFacility { get; set; }

        [Display(Name = "OT Formula")]
        public string CRI_OT_Formula { get; set; }
        [Display(Name = "OT Rate")]
        public decimal? CRI_OT_Rate { get; set; }
        [Display(Name = "OT Payment")]
        public double? CRI_OT_MultipleTimes { get; set; }
        [Display(Name = "Weekly off is payable?")]
        public bool CRI_IsPayable_WeeklyOff { get; set; }
        [Display(Name = "Public holiday is payable?")]
        public bool CRI_IsPayable_PublicHoliday { get; set; }
        [Required(ErrorMessage ="Date is required")]       
        [Display(Name = "Effective From")]
        [DataType(DataType.DateTime)]
        public DateTime CRI_RegisteredOn { get; set; }
        [Required]
        public bool? CRI_Active { get; set; }
        public DateTime? CRI_InactivatedOn { get; set; }
        public int? ADM_Id_InactivatedBy { get; set; }

        public string tabName { get; set; }
        public string DES_Title { get; set; }

        public bool HRAselection { get; set; }

        public bool Basic_PFselection { get; set; }
        public bool DA_PFselection { get; set; }
        public bool HRA_PFselection { get; set; }

        public bool Basic_ESICselection { get; set; }
        public bool DA_ESICselection { get; set; }
        public bool HRA_ESICselection { get; set; }

        public List<ClientReqAllowanceVM> allAllowances { get; set; }

        [Display(Name = "Out station allowance")]
        public bool CRI_OutStation_Allowance { get; set; }
        public decimal? CRI_OutStation_Allowance_Rate { get; set; }
        [Display(Name = "Attendance allowance")]
        public bool CRI_Attendance_Allowance { get; set; }
        public int? CRI_Attendance_Allowance_MaximumDays { get; set; }
        public decimal? CRI_Attendance_Allowance_Rate { get; set; }

        [Display(Name = "Consider OT by adding it in payable days")]
        public bool CRI_OT_Calculate_Payableday { get; set; } = true;
        [Display(Name = "Consider OT differently")]
        public bool CRI_OT_Calculate_Differently { get; set; } = true;
        [Display(Name = "Consider fixed amount per hour")]       
        public decimal? CRI_OT_Fixed_PerHour { get; set; } = 0;
        [Display(Name = "Calculate by formula")]
        public bool CRI_OT_Calculate_By_Formula { get; set; }

        [Display(Name = "Performance allowance")]
        public bool CRI_Performance_Allowance { get; set; }
        [Display(Name = "Nightshift allowance")]
        public bool CRI_Nightshift_Allowance { get; set; }
        public decimal? CRI_Nightshift_Allowance_Rate { get; set; }

        [Display(Name = "Billing Type")]
        public int CRI_Billing_Type { get; set; }
        [Display(Name = "Billing Amount")]
        public decimal? CRI_Billing_Amount { get; set; }
        [Display(Name = "Billing Service Charge")]
        public double? CRI_Billing_ServiceCharge { get; set; }
        [Display(Name = "Billing Service Charge Formula")]
        public string CRI_Billing_ServiceCharge_Formula { get; set; }

        public string Edit_History { get; set; }
        public DateTime LastRecordRegOn { get; set; }

    }
}
