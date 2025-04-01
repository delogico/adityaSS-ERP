using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models;

public partial class City
{
    public int CIT_Id { get; set; }

    public string CIT_Name { get; set; }

    public virtual ICollection<Client> Clients { get; set; } = new List<Client>();
}
