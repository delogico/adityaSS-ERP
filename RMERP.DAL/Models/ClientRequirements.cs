using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models
{
    public partial class ClientRequirements
    {
        public ClientRequirements()
        {
            Attendance = new HashSet<Attendance>();
        }

        public int CriId { get; set; }
        public int CliId { get; set; }
        public int DesId { get; set; }
        public decimal? CriBasic { get; set; }
        public double? CriDa { get; set; }
        public decimal? CriBasicDa { get; set; }
        public decimal? CriHraFixed { get; set; }
        public double? CriHraPercentage { get; set; }
        public decimal? CriAllowanceUpKeep { get; set; }
        public decimal? CriAllowanceGrade { get; set; }
        public decimal? CriAllowanceConveyance { get; set; }
        public decimal? CriAllowanceAttention { get; set; }
        public double? CriPfPercentage { get; set; }
        public double? CriEsicPercentage { get; set; }
        public string CriEsicArea { get; set; }
        public decimal? CriOtRate { get; set; }
        public double? CriOtMultipleTimes { get; set; }
        public bool CriWageCalculationOnWeeklyOffPlus { get; set; }
        public DateTime CriRegisteredOn { get; set; }
        public bool? CriActive { get; set; }
        public DateTime? CriInactivatedOn { get; set; }
        public int? AdmIdInactivatedBy { get; set; }

        public Clients Cli { get; set; }
        public Designations Des { get; set; }
        public ICollection<Attendance> Attendance { get; set; }
    }
}
