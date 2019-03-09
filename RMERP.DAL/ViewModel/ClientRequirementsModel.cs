using RMERP.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RMERP.DAL.ViewModel
{
    public class ClientRequirementsViewModel
    {
        public IEnumerable<ClientRequirements> ClientRequirementsList { get; set; }
        public ClientRequirementsModel ClientRequirementsModel { get; set; }
    }
    public class ClientRequirementsModel
    { 
        [Key]
        public int CriId { get; set; }

        public string CliName { get; set; }


        [Display(Name = "Client")]
        [Required(ErrorMessage = "Please select client")]
        public int CliId { get; set; }
        [Display(Name = "Designation")]
        [Required(ErrorMessage = "Please insert designation")]
        public int DesId { get; set; }
        [Display(Name = "Basic")]
        public decimal? CriBasic { get; set; }
        [Display(Name = "DA")]
        public double? CriDa { get; set; }
        [Display(Name = "Basic DA")]
        public decimal? CriBasicDa { get; set; }
        [Display(Name = "HRA Fixed")]
        public decimal? CriHraFixed { get; set; }
        [Display(Name = "HRA Percentage (%)")]
        public double? CriHraPercentage { get; set; }
        [Display(Name = "Allowance Up-Keep")]
        public decimal? CriAllowanceUpKeep { get; set; }
        [Display(Name = "Allowance Grade")]
        public decimal? CriAllowanceGrade { get; set; }
        [Display(Name = "Allowance Conveyance")]
        public decimal? CriAllowanceConveyance { get; set; }
        [Display(Name = "Allowance Attention")]
        public decimal? CriAllowanceAttention { get; set; }
        [Display(Name = "PF Percentage (%)")]
        public double? CriPfPercentage { get; set; }
        [Display(Name = "ESIC Percentage (%)")]
        public double? CriEsicPercentage { get; set; }
        [Display(Name = "ESIC Area")]
        public string CriEsicArea { get; set; }
        [Display(Name = "OT Rate")]
        public decimal? CriOtRate { get; set; }
        [Display(Name = "OT Times")]
        public double? CriOtMultipleTimes { get; set; }
        [Display(Name = "Wage calculation on weekly off")]
        [Required(ErrorMessage = "Please select")]
        public bool CriWageCalculationOnWeeklyOffPlus { get; set; }
        [Required]
        public DateTime CriRegisteredOn { get; set; }
        [Required]
        public bool? CriActive { get; set; }
        public DateTime? CriInactivatedOn { get; set; }
        public int? AdmIdInactivatedBy { get; set; }

        public string tabName { get; set; }
        public string DesTitle { get; set; }


    }
}
