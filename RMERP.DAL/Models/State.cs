using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models;

public partial class State
{
    public int STA_Id { get; set; }

    public string STA_Name { get; set; }

    public int COU_Id { get; set; }

    public string STA_GST_Code { get; set; }

    public virtual Country COU { get; set; }

    public virtual ICollection<Client> Clients { get; set; } = new List<Client>();

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<Firm> Firms { get; set; } = new List<Firm>();
}
