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
        [Display(Name = "Wage calculation on weekly off")]
        [Required(ErrorMessage = "Please select")]
        public bool CRI_WageCalculationOnWeeklyOffPlus { get; set; }
        [Required]
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
       
    }
}
