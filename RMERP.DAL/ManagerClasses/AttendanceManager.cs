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

        public void save(Attendance attendace)
        {
            _context.Attendance.Add(attendace);
            _context.SaveChanges();
        }

        public void delete(Attendance attendace)
        {
            _context.Attendance.Remove(attendace);
            _context.SaveChanges();
        }

        public void deleteAllAttendanceofWageClient(int WAG_Id, int CLI_Id)
        {
            _context.Attendance.RemoveRange(_context.Attendance.Where(a => a.WAG_Id == WAG_Id && a.CLI_Id == CLI_Id).ToList());
            _context.SaveChanges();
        }
    }
}
