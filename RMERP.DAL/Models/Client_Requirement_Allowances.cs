using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models
{
    public partial class Client_Requirement_Allowances
    {
        public Client_Requirement_Allowances()
        {
            Wage_Register_Allowances = new HashSet<Wage_Register_Allowances>();
        }

        public int CRA_Id { get; set; }
        public int CRI_Id { get; set; }
        public int ALL_Id { get; set; }
        public decimal CRA_Amount { get; set; }
        public bool CRA_DayswiseOrFull { get; set; }

        public Allowances ALL_ { get; set; }
        public Client_Requirements CRI_ { get; set; }
        public ICollection<Wage_Register_Allowances> Wage_Register_Allowances { get; set; }
    }
}
