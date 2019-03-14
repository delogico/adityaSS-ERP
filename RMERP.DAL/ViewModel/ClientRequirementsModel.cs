using RMERP.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RMERP.DAL.ViewModel
{
    public class ClientRequirementsViewModel
    {
        public IEnumerable<Client_Requirements> ClientRequirementsList { get; set; }
        public ClientRequirementsModel ClientRequirementsModel { get; set; }
    }
    public class ClientRequirementsModel
    { 
        [Key]
        public int CRI_Id { get; set; }

        public string CliName { get; set; }


        [Display(Name = "Client")]
        [Required(ErrorMessage = "Please select client")]
        public int CLI_Id { get; set; }
        [Display(Name = "Designation")]
        [Required(ErrorMessage = "Please insert designation")]
        public int DES_Id { get; set; }
        [Display(Name = "Basic")]
        public decimal? CRI_Basic { get; set; }
        [Display(Name = "DA")]
        public double? CRI_DA { get; set; }
        [Display(Name = "Basic DA")]
        public decimal? CRI_BasicDA { get; set; }
        [Display(Name = "HRA Fixed")]
        public decimal? CRI_HRA_Fixed { get; set; }
        [Display(Name = "HRA Percentage (%)")]
        public double? CRI_HRA_Percentage { get; set; }
        [Display(Name = "Allowance Up-Keep")]
        public decimal? CRI_Allowance_UpKeep { get; set; }
        [Display(Name = "Allowance Grade")]
        public decimal? CRI_Allowance_Grade { get; set; }
        [Display(Name = "Allowance Conveyance")]
        public decimal? CRI_Allowance_Conveyance { get; set; }
        [Display(Name = "Allowance Attention")]
        public decimal? CRI_Allowance_Attention { get; set; }
        [Display(Name = "PF Percentage (%)")]
        public double? CRI_PF_Percentage { get; set; }
        [Display(Name = "ESIC Percentage (%)")]
        public double? CRI_ESIC_Percentage { get; set; }
        [Display(Name = "ESIC Area")]
        public string CRI_ESIC_Area { get; set; }
        [Display(Name = "OT Rate")]
        public decimal? CRI_OT_Rate { get; set; }
        [Display(Name = "OT Times")]
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
        public string DesTitle { get; set; }


    }
}
