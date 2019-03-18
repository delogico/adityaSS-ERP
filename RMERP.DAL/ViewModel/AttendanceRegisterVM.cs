using System;
using System.Collections.Generic;
using System.Text;
using RMERP.DAL.Models;

namespace RMERP.DAL.ViewModel
{
    public class AttendanceRegisterVM
    {
        public List<Clients> listClients { get; set; }
        public List<Attendance> listAttendance { get; set; }
    }
   
}
