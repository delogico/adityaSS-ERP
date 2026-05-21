using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models;

public partial class Company_Bank_Account
{
    public int CBA_Id { get; set; }

    public int? FRM_Id { get; set; }

    public string CBA_Bank { get; set; }

    public string CBA_Account_Number { get; set; }

    public string CBA_Bank_IFSC { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual Firm FRM { get; set; }
}
