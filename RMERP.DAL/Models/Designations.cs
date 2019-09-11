using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models
{
    public partial class Designations
    {
        public Designations()
        {
            Attendance = new HashSet<Attendance>();
            Client_Requirements = new HashSet<Client_Requirements>();
            Clients_Employees = new HashSet<Clients_Employees>();
        }

        public int DES_Id { get; set; }
        public string DES_Title { get; set; }
        public bool DES_Exclude_LWF { get; set; }

        public ICollection<Attendance> Attendance { get; set; }
        public ICollection<Client_Requirements> Client_Requirements { get; set; }
        public ICollection<Clients_Employees> Clients_Employees { get; set; }
    }
}
