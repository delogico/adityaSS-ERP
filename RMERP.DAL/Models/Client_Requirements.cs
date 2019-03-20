using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models
{
    public partial class Client_Requirements
    {
        public Client_Requirements()
        {
            Attendance = new HashSet<Attendance>();
            Clients_Employees = new HashSet<Clients_Employees>();
        }

        public int CRI_Id { get; set; }
        public int CLI_Id { get; set; }
        public int DES_Id { get; set; }
        public decimal? CRI_Basic { get; set; }
        public double? CRI_DA { get; set; }
        public decimal? CRI_BasicDA { get; set; }
        public decimal? CRI_HRA_Fixed { get; set; }
        public double? CRI_HRA_Percentage { get; set; }
        public decimal? CRI_Allowance_UpKeep { get; set; }
        public decimal? CRI_Allowance_Grade { get; set; }
        public decimal? CRI_Allowance_Conveyance { get; set; }
        public decimal? CRI_Allowance_Attention { get; set; }
        public double? CRI_PF_Percentage { get; set; }
        public double? CRI_ESIC_Percentage { get; set; }
        public string CRI_ESIC_Area { get; set; }
        public decimal? CRI_OT_Rate { get; set; }
        public double? CRI_OT_MultipleTimes { get; set; }
        public bool CRI_WageCalculationOnWeeklyOffPlus { get; set; }
        public DateTime CRI_RegisteredOn { get; set; }
        public bool? CRI_Active { get; set; }
        public DateTime? CRI_InactivatedOn { get; set; }
        public int? ADM_Id_InactivatedBy { get; set; }

        public Clients CLI_ { get; set; }
        public Designations DES_ { get; set; }
        public ICollection<Attendance> Attendance { get; set; }
        public ICollection<Clients_Employees> Clients_Employees { get; set; }

    }
}
