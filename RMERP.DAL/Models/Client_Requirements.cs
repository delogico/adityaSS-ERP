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
        public string CRI_OT_Formula { get; set; }
        public decimal? CRI_OT_Rate { get; set; }
        public double? CRI_OT_MultipleTimes { get; set; }
        public bool CRI_WageCalculationOnWeeklyOffPlus { get; set; }
        public DateTime CRI_RegisteredOn { get; set; }
        public bool? CRI_Active { get; set; }
        public DateTime? CRI_InactivatedOn { get; set; }
        public int? ADM_Id_InactivatedBy { get; set; }

        public Clients CLI_ { get; set; }
        public Designations DES_ { get; set; }
        public ICollection<Client_Requirement_Allowances> Client_Requirement_Allowances { get; set; }
        public ICollection<Wage_Register> Wage_Register { get; set; }
    }
}
