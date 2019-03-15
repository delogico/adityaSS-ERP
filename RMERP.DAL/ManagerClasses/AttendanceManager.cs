using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using RMERP.DAL.Models;
using System.Collections.Generic;

namespace RMERP.DAL.ManagerClasses
{
    public class AttendanceManager
    {
        RMERPContext _context;
        public AttendanceManager(RMERPContext context)
        {
            _context = context;
        }

        public List<Attendance> getAttendance_Wage(int WAG_Id)
        {
            return _context.Attendance.Where(a => a.WAG_Id == WAG_Id).ToList();
        }
    }
}
