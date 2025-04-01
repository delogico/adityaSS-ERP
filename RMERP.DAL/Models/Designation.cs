using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models;

public partial class Designation
{
    public int DES_Id { get; set; }

    public string DES_Title { get; set; }

    public bool DES_Exclude_LWF { get; set; }

    public virtual ICollection<Attendance_Summary> Attendance_Summaries { get; set; } = new List<Attendance_Summary>();

    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    public virtual ICollection<Client_Requirement> Client_Requirements { get; set; } = new List<Client_Requirement>();

    public virtual ICollection<Clients_Employee> Clients_Employees { get; set; } = new List<Clients_Employee>();
}
