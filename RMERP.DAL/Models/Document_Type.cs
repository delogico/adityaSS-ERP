using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models;

public partial class Document_Type
{
    public int DOT_Id { get; set; }

    public string DOT_Title { get; set; }

    public virtual ICollection<Employee_Document> Employee_Documents { get; set; } = new List<Employee_Document>();
}
