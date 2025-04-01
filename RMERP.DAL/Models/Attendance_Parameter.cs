using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models;

public partial class Attendance_Parameter
{
    public int ATP_Id { get; set; }

    public bool ATP_Att_MonthReal { get; set; }

    public int? ATP_Att_Month_Start { get; set; }

    public int? ATP_Att_Month_End { get; set; }

    public int? CLI_Id { get; set; }

    public DateTime ATP_RegisteredOn { get; set; }
}
