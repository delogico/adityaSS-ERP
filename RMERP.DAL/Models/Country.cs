using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models;

public partial class Country
{
    public int COU_Id { get; set; }

    public string COU_Code { get; set; }

    public string COU_Name { get; set; }

    public virtual ICollection<State> States { get; set; } = new List<State>();
}
