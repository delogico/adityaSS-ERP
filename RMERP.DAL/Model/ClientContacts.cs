using System;
using System.Collections.Generic;

namespace RMERP.DAL.Model
{
    public partial class ClientContacts
    {
        public int ConId { get; set; }
        public int CliId { get; set; }
        public string ConFirstName { get; set; }
        public string ConSurName { get; set; }
        public string ConDesignation { get; set; }
        public string ConMobile { get; set; }
        public string ConEmail { get; set; }
        public int AdmIdRegisteredBy { get; set; }
        public DateTime ConRegisteredOn { get; set; }
        public bool ConIsPrimary { get; set; }

        public Clients Cli { get; set; }
    }
}
