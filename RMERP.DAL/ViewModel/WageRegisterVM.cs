using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using RMERP.DAL.Models;

namespace RMERP.DAL.ViewModel
{
    public class WageRegisterAllowanceVM
    {
        public int WAA_Id { get; set; }
        public WageRegisterVM wageRegisterVM { get; set; }
        public ClientRequirementVM clientRequirementVM { get; set; }
        public AllowanceVM allowanceVM { get; set; }
        public int CRI_Id { get; set; }
        public int CRA_Id { get; set; }
        public bool WAA_DayswiseOrFull { get; set; }
        public decimal WAA_Amount { get; set; }
        public decimal WAA_Amount_Calculated { get; set; }
    }

    public class WageRegisterVM
    {
        public int WAR_Id { get; set; }
        public int WAG_Id { get; set; }
        public WageProcessVM wageProcessVM { get; set; }
        public int CLI_Id { get; set; }
        public ClientsModel clientVM { get; set; }
        public int EMP_Id { get; set; }
        public EmployeeVM employeeVM { get; set; }
        public int CRI_Id { get; set; }
        public ClientRequirementVM clientRequirementVM { get; set; }
        [Display(Name ="Total Payble Days")]
        public double WAR_TotalPaybleDays { get; set; }
        [Display(Name = "Total Working Days")]
        public double WAR_TotalWorkingDays { get; set; }
        [Display(Name = "Total Extra Working Hours")]
        public double WAR_ExtraWorkingHours { get; set; }
        [Display(Name = "Basic")]
        public decimal WAR_Basic { get; set; }
        [Display(Name = "Calculated Basic")]
        public decimal WAR_Basic_Calculated { get; set; }
        [Display(Name = "DA")]
        public decimal WAR_DA { get; set; }
        [Display(Name = "Calculated DA")]
        public decimal WAR_DA_Calculated { get; set; }
        [Display(Name = "HRA")]
        public decimal WAR_HRA { get; set; }
        [Display(Name = "HRA")]
        public string WAR_HRA_PER {
            get
            {                
                return (clientRequirementVM.CRI_HRA_Fixed.ToString() == "" ? (clientRequirementVM.CRI_HRA_Percentage.Value.ToString() + "%") :  clientRequirementVM.CRI_HRA_Fixed.Value + ""); 
            }
        }
        [Display(Name = "Calculated HRA")]
        public decimal WAR_HRA_Calculated { get; set; }

        [Display(Name = "OT Formula")]
        public string WAR_OverTime_Formula { get; set; }
        [Display(Name = "OverTime Payment Times")]
        public int? WAR_OverTime_Payment { get; set; }   

        [Display(Name = "Calculated Extra Working Amount")]
        public decimal WAR_OverTime_Calculated { get; set; }
        [Display(Name = "WorkingHrs in day")]
        public int WAR_WorkingHrs_In_Day { get; set; }
        [Display(Name = "Gross Total")]
        public decimal WAR_GrossTotal { get; set; }
        [Display(Name = "PF")]
        public decimal WAR_PF { get; set; }
        [Display(Name = "PF Formula")]
        public string WAR_PF_Formula { get; set; }
        [Display(Name = "Calculated PF")]
        public decimal WAR_PF_Calculated { get; set; }
        [Display(Name = "ESIC")]
        public decimal WAR_ESIC { get; set; }
        [Display(Name = "ESIC Formula")]
        public string WAR_ESIC_Formula { get; set; }
        [Display(Name = "Calculated ESIC")]
        public decimal WAR_ESIC_Calculated { get; set; }
        [Display(Name = "Final Total")]
        public decimal WAR_FinalTotal { get; set; }
        public DateTime WAR_LastModifiedOn { get; set; }
        public int ADM_LastModifiedBy { get; set; }
        public Designations designation { get; set; }
        public decimal WAR_Advance_Amount { get; set; }
        public List<WageRegisterAllowanceVM> allowanceVMs { get; set; }
    }


    public class ClientWageRegisterVM
    {
        public List<WageRegisterVM> wageRegisterVMs { get; set; }
        public ClientsModel client { get; set; }
        public WageProcessVM wageProcessVM { get; set; }
        public WageProcessClientVM wageProcessClientVM { get; set; }
    }

    public class EditWageRegisterVM
    {
        public WageRegisterVM wageRegisterVM { get; set; }
        public List<Wage_Register_Allowances> wage_Register_Allowances { get; set; }
    }
}
