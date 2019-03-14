using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models
{
    public partial class AdminUsers
    {
        public int ADM_Id { get; set; }
        public string ADM_FirstName { get; set; }
        public string ADM_MiddleName { get; set; }
        public string ADM_LastName { get; set; }
        public string ADM_EmailId { get; set; }
        public string ADM_Password { get; set; }
        public string ADM_Mobile { get; set; }
        public int? FRM_Id { get; set; }

        public Firms FRM_ { get; set; }
    }
}
