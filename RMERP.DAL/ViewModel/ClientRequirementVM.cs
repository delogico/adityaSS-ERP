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
        [Display(Name = "PF Apply Max.")]
        public decimal? CRI_PF_ApplyMAX { get; set; }   //new
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
        [Required(ErrorMessage = "Date is required")]
        [Display(Name = "Effective From")]
        [DataType(DataType.Date)]
        public DateTime CRI_RegisteredOn { get; set; }
        [Required]
        public bool? CRI_Active { get; set; }

        [Display(Name = "Inactivated On")]
        [DataType(DataType.Date)]
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
        public bool IsHistory { get; set; }
        public bool IsMajorModified { get; set; }


        [Display(Name = "Employer PF Contribution Rate(%)")]
        public double? CRI_PF_Employer_Cont_Rate { get; set; }
        [Display(Name = "Employer ESIC Contribution Rate(%)")]
        public double? CRI_ESIC_Employer_Cont_Rate { get; set; }
        [Display(Name = "Employee Pension Scheme Rate(%)")]
        public double CRI_EPS_Rate { get; set; }
        [Display(Name = "Employer MLWF Contribution(INR)")]

        public decimal? CRI_MLWF_Employer_GThen { get; set; }
        public decimal? CRI_MLWF_Employer_LThen { get; set; }
        public decimal? CRI_MLWF_Employee_GThen { get; set; }
        public decimal? CRI_MLWF_Employee_LThen { get; set; }
        public decimal? CRI_MLWF_Employer_Base { get; set; }
        public decimal? CRI_MLWF_Employee_Base { get; set; }
        public decimal? CRI_MLWF_Employee_Max_Base { get; set; }
        public decimal? CRI_MLWF_Employer_Max_Base { get; set; }

        public decimal CRI_ProffTax_M_From_1 { get; set; }
        public decimal CRI_ProffTax_M_To_1 { get; set; }
        public decimal CRI_ProffTax_M_Amount_1 { get; set; }
        public decimal CRI_ProffTax_M_From_2 { get; set; }
        public decimal CRI_ProffTax_M_To_2 { get; set; }
        public decimal CRI_ProffTax_M_Amount_2 { get; set; }
        public decimal CRI_ProffTax_M_From_3 { get; set; }
        public decimal CRI_ProffTax_M_To_3 { get; set; }
        public decimal CRI_ProffTax_M_Amount_3 { get; set; }
        public decimal CRI_ProffTax_F_From_1 { get; set; }
        public decimal CRI_ProffTax_F_To_1 { get; set; }
        public decimal CRI_ProffTax_F_Amount_1 { get; set; }
        public decimal CRI_ProffTax_F_From_2 { get; set; }
        public decimal CRI_ProffTax_F_To_2 { get; set; }
        public decimal CRI_ProffTax_F_Amount_2 { get; set; }
        public decimal CRI_ProffTax_F_From_3 { get; set; }
        public decimal CRI_ProffTax_F_To_3 { get; set; }
        public decimal CRI_ProffTax_F_Amount_3 { get; set; }

        [Display(Name = "Allowance 1")]
        public bool CRI_Allowance_1 { get; set; }
        public string CRI_Allowance_Name_1 { get; set; }
        [Display(Name = "Allowance 2")]
        public bool CRI_Allowance_2 { get; set; }
        public string CRI_Allowance_Name_2 { get; set; }
        [Display(Name = "Allowance 3")]
        public bool CRI_Allowance_3 { get; set; }
        public string CRI_Allowance_Name_3 { get; set; }
        [Display(Name = "Allowance 4")]
        public bool CRI_Allowance_4 { get; set; }
        public string CRI_Allowance_Name_4 { get; set; }
        [Display(Name = "Allowance 5")]
        public bool CRI_Allowance_5 { get; set; }
        public string CRI_Allowance_Name_5 { get; set; }

        //ADDED ON : 27/02/2025 

        [Display(Name = "Allowance 6")]
        public bool CRI_Allowance_6 { get; set; }
        public string CRI_Allowance_Name_6 { get; set; }

        [Display(Name = "Allowance 7")]
        public bool CRI_Allowance_7 { get; set; }
        public string CRI_Allowance_Name_7 { get; set; }

        [Display(Name = "Allowance 8")]
        public bool CRI_Allowance_8 { get; set; }
        public string CRI_Allowance_Name_8 { get; set; }

        [Display(Name = "Allowance 9")]
        public bool CRI_Allowance_9 { get; set; }
        public string CRI_Allowance_Name_9 { get; set; }

        [Display(Name = "Allowance 10")]
        public bool CRI_Allowance_10 { get; set; }
        public string CRI_Allowance_Name_10 { get; set; }

    }
}