using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models;

public partial class Firm
{
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

    public int STA_Id { get; set; }

    public virtual ICollection<AdminUser> AdminUsers { get; set; } = new List<AdminUser>();

    public virtual ICollection<Client> Clients { get; set; } = new List<Client>();

    public virtual ICollection<Company_Bank_Account> Company_Bank_Accounts { get; set; } = new List<Company_Bank_Account>();

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual State STA { get; set; }

    public virtual ICollection<Wage_Process> Wage_Processes { get; set; } = new List<Wage_Process>();
}
