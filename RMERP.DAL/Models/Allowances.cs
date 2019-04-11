using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models
{
    public partial class Allowances
    {
        public Allowances()
        {
            Client_Requirement_Allowances = new HashSet<Client_Requirement_Allowances>();
        }

        public int ALL_Id { get; set; }
        public string ALL_Title { get; set; }
        public string ALL_Alias { get; set; }
        public string ALL_Shortform { get; set; }

        public ICollection<Client_Requirement_Allowances> Client_Requirement_Allowances { get; set; }
    }
}
