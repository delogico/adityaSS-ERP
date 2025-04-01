using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models;

public partial class Employee_Document
{
    public int EMD_Id { get; set; }

    public int EMP_Id { get; set; }

    public int DOT_Id { get; set; }

    public string EMD_Name { get; set; }

    public DateTime EMD_UploadedOn { get; set; }

    public virtual Document_Type DOT { get; set; }

    public virtual Employee EMP { get; set; }
}
