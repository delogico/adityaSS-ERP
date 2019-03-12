using System;
using System.Collections.Generic;
using System.Text;

namespace RMERP.DAL.ViewModel
{
    public class AttendanceListViewModel
    {
        public int EmpID { get; set; }
        public string EmpName { get; set; }
        public string EmpDesignation { get; set; }
        public string WorkingHours { get; set; }
        public double ExtraWorkingHours { get; set; }
        public List<UserData> UserDataList { get; set; }
    }
    public class UserData
    {
        public DateTime AttendanceDate { get; set; }
        public bool IsPresent { get; set; }
    }

}
