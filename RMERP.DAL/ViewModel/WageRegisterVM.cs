using System;
using System.Collections.Generic;
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

        public double WAR_TotalPaybleDays { get; set; }
        public double WAR_TotalWorkingDays { get; set; }
        public double WAR_ExtraWorkingHours { get; set; }
        public decimal WAR_Basic { get; set; }
        public decimal WAR_Basic_Calculated { get; set; }
        public decimal WAR_DA { get; set; }
        public decimal WAR_DA_Calculated { get; set; }
        public decimal WAR_HRA { get; set; }
        public decimal WAR_HRA_Calculated { get; set; }
        public decimal WAR_OverTime_Calculated { get; set; }
        public decimal WAR_GrossTotal { get; set; }

        public decimal WAR_PF { get; set; }
        public decimal WAR_PF_Calculated { get; set; }
        public decimal WAR_ESIC { get; set; }
        public decimal WAR_ESIC_Calculated { get; set; }
        public decimal WAR_FinalTotal { get; set; }
        public DateTime WAR_LastModifiedOn { get; set; }
        public int ADM_LastModifiedBy { get; set; }
        public Designations designation { get; set; }
        

        public List<WageRegisterAllowanceVM> allowanceVMs { get; set; }
    }


    public class ClientWageRegisterVM
    {
        public List<WageRegisterVM> wageRegisterVMs { get; set; }
        public ClientsModel client { get; set; }
        public WageProcessVM wageProcessVM { get; set; }
        public WageProcessClientVM wageProcessClientVM { get; set; }

    }
}
