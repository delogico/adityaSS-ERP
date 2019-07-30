using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models
{
    public partial class Firms
    {
        public Firms()
        {
            AdminUsers = new HashSet<AdminUsers>();
            Clients = new HashSet<Clients>();
            Employees = new HashSet<Employees>();
            Wage_Process = new HashSet<Wage_Process>();
        }

        public int FRM_Id { get; set; }
        public string FRM_Name { get; set; }
        public string FRM_ShortName { get; set; }
        public string FRM_InvoicingName { get; set; }
        public string FRM_Email { get; set; }
        public string FRM_Address1 { get; set; }
        public string FRM_Address2 { get; set; }
        public string FRM_GST_No { get; set; }
        public string FRM_BankName { get; set; }
        public string FRM_AccountNumber { get; set; }
        public string FRM_IFSC_Code { get; set; }

        public ICollection<AdminUsers> AdminUsers { get; set; }
        public ICollection<Clients> Clients { get; set; }
        public ICollection<Employees> Employees { get; set; }
        public ICollection<Wage_Process> Wage_Process { get; set; }
    }
}
