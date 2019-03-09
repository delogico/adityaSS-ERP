using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models
{
    public partial class Clients
    {
        public Clients()
        {
            Attendance = new HashSet<Attendance>();
            ClientContacts = new HashSet<ClientContacts>();
            ClientRequirements = new HashSet<ClientRequirements>();
            ClientsEmployees = new HashSet<ClientsEmployees>();
        }

        public int CliId { get; set; }
        public int FrmId { get; set; }
        public string CliName { get; set; }
        public byte CliInternationalDomestic { get; set; }
        public string CliAddress { get; set; }
        public int CityId { get; set; }
        public string CliPincode { get; set; }
        public string CliPhone { get; set; }
        public string CliFax { get; set; }
        public string CliEmail { get; set; }
        public string CliEmail2 { get; set; }
        public string CliGstNumber { get; set; }
        public int CliGstRate { get; set; }
        public string CliHsnCode { get; set; }
        public int CliTdsRate { get; set; }
        public int AdmIdRegisterBy { get; set; }
        public DateTime CliRegisteredOn { get; set; }
        public string CliLogo { get; set; }
        public bool? CliIsActive { get; set; }
        public DateTime? CliInActivatedOn { get; set; }
        public int? AdmIdInactivatedBy { get; set; }
        public bool? CliAttMonthReal { get; set; }
        public int? CliAttMonthStart { get; set; }
        public int? CliAttMonthEnd { get; set; }

        public Cities City { get; set; }
        public Firms Frm { get; set; }
        public ICollection<Attendance> Attendance { get; set; }
        public ICollection<ClientContacts> ClientContacts { get; set; }
        public ICollection<ClientRequirements> ClientRequirements { get; set; }
        public ICollection<ClientsEmployees> ClientsEmployees { get; set; }
    }
}
