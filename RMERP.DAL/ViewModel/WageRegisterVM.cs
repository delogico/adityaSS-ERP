using System;
using System.Collections.Generic;
using System.Text;
using RMERP.DAL.Models;

namespace RMERP.DAL.ViewModel
{
    public class WageRegisterVM
    {
        public List<Clients> listClients { get; set; }
        public List<Attendance> listAttendance { get; set; }
        public IEnumerable<Designations> listDesignations { get; set; }
        public IEnumerable<Employees> listEmployee { get; set; }
        public List<Client_Requirements> client_Requirements { get; set; }
        public DateTime WAG_Month { get; set; }
        public List<Allowances> allowances { get; set; }
        public int WAG_Id { get; set; }
        public string WAG_Full_Month()
        {
            return WAG_Month.ToString("MMMM") + "-" + WAG_Month.ToString("yyyy");
        }
    }
   
}
