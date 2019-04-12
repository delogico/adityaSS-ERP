using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models
{
    public partial class Wage_Register
    {
        public Wage_Register()
        {
            Wage_Register_Allowances = new HashSet<Wage_Register_Allowances>();
        }

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
        public decimal WAR_OverTime_Calculated { get; set; }
        public decimal WAR_GrossTotal { get; set; }
        public decimal WAR_PF { get; set; }
        public string WAR_PF_Formula { get; set; }
        public decimal WAR_PF_Calculated { get; set; }
        public decimal WAR_ESIC { get; set; }
        public string WAR_ESIC_Formula { get; set; }
        public decimal WAR_ESIC_Calculated { get; set; }
        public decimal WAR_FinalTotal { get; set; }
        public DateTime? WAR_LastModifiedOn { get; set; }
        public int? ADM_LastModifiedBy { get; set; }

        public Clients CLI_ { get; set; }
        public Client_Requirements CRI_ { get; set; }
        public Employees EMP_ { get; set; }
        public Wage_Process WAG_ { get; set; }
        public ICollection<Wage_Register_Allowances> Wage_Register_Allowances { get; set; }
    }
}
