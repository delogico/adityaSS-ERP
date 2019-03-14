using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models
{
    public partial class Client_Contacts
    {
        public int CON_Id { get; set; }
        public int CLI_Id { get; set; }
        public string CON_FirstName { get; set; }
        public string CON_SurName { get; set; }
        public string CON_Designation { get; set; }
        public string CON_Mobile { get; set; }
        public string CON_Email { get; set; }
        public int ADM_Id_RegisteredBy { get; set; }
        public DateTime CON_RegisteredOn { get; set; }
        public bool CON_isPrimary { get; set; }

        public Clients CLI_ { get; set; }
    }
}
