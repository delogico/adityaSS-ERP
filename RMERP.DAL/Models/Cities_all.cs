using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models;

public partial class Cities_all
{
    public int CITY_Id { get; set; }

    public string CITY_Name { get; set; }

    public int STA_Id { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
