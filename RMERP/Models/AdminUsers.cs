using System;
using System.Collections.Generic;

namespace RMERP.Models
{
    public partial class AdminUsers
    {
        public int AdmId { get; set; }
        public string AdmFirstName { get; set; }
        public string AdmMiddleName { get; set; }
        public string AdmLastName { get; set; }
        public string AdmEmailId { get; set; }
        public string AdmPassword { get; set; }
        public string AdmMobile { get; set; }
        public int FrmId { get; set; }

        public Firms Frm { get; set; }
    }
}
