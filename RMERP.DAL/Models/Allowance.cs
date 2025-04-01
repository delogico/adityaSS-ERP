using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models;

public partial class Allowance
{
    public int ALL_Id { get; set; }

    public string ALL_Title { get; set; }

    public string ALL_Alias { get; set; }

    public string ALL_Shortform { get; set; }

    public virtual ICollection<Client_Requirement_Allowance> Client_Requirement_Allowances { get; set; } = new List<Client_Requirement_Allowance>();
}
