using Microsoft.EntityFrameworkCore;
using RMERP.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMERP.DAL.ManagerClasses
{
    public class AttendanceSummaryManager(RMERPContext contexts)
    {
        RMERPContext _context = contexts;
        public bool IsAlreadyUploaded(int WAG_Id, int CLI_Id)
        {
            return _context.Attendance_Summaries.Where(m => m.WAG_Id.Equals(WAG_Id) && m.CLI_Id.Equals(CLI_Id)).Count() > 0;
        }

        public void Save(Attendance_Summary _Summary)
        {
            try
            {
                _context.Attendance_Summaries.Add(_Summary);
                _context.SaveChanges();
            }
            catch (Exception ex) { throw ex; }
        }
        
        public void Save(List<Attendance_Summary> Summaries)
        {
            try
            {
                _context.Attendance_Summaries.AddRange(Summaries);
                _context.SaveChanges();
            }
            catch (Exception ex) { throw ex; }
        }

        public void Delete(Attendance_Summary _Summary)
        {
            _context.Attendance_Summaries.Remove(_Summary);
            _context.SaveChanges();
        }

        public IQueryable<Attendance_Summary> GetAttendance_Wage(int WAG_Id)
        {
            return _context.Attendance_Summaries.Where(a => a.WAG_Id == WAG_Id);
        }

        public IQueryable<Attendance_Summary> GetAttendance_Wage_Client(int WAG_Id, int CLI_Id)
        {
            return _context.Attendance_Summaries.Where(a => a.WAG_Id == WAG_Id && a.CLI_Id == CLI_Id).Include(a => a.EMP);
        }

        public void DeleteAllAttendanceOfWageClient(int WAG_Id, int CLI_Id)
        {
            _context.Attendance_Summaries.RemoveRange(_context.Attendance_Summaries.Where(a => a.WAG_Id == WAG_Id && a.CLI_Id == CLI_Id).ToList());
            _context.SaveChanges();
        }

        public Attendance_Summary GetAttendance_Wage_Client_Employee(int WAG_Id, int CLI_Id, int EMP_Id)
        {
            return _context.Attendance_Summaries.Where(a => a.WAG_Id == WAG_Id && a.CLI_Id == CLI_Id && a.EMP_Id == EMP_Id).FirstOrDefault();
        }

        public IEnumerable<Attendance_Summary> GetAttendanceSummary_WageEmployee(int WAG_Id, int EMP_Id)
        {
            return _context.Attendance_Summaries.Where(a => a.WAG_Id == WAG_Id && a.EMP_Id == EMP_Id);
        }

        public Attendance_Summary GetAttendanceSummary_WageClientEmployee_Designation(int WAG_Id, int CLI_Id, int EMP_Id, int DES_Id)
        {
            return _context.Attendance_Summaries.Where(a => a.WAG_Id == WAG_Id && a.CLI_Id == CLI_Id && a.EMP_Id == EMP_Id).FirstOrDefault();
        }

        public Attendance_Summary GetAttendanceSummary_WageClient_EmployeeDesignation(int WAG_Id, int CLI_Id, int EMP_Id, int DES_Id)
        {
            return _context.Attendance_Summaries.Where(a => a.WAG_Id == WAG_Id && a.CLI_Id == CLI_Id && a.EMP_Id == EMP_Id && a.DES_Id == DES_Id).FirstOrDefault();
        }

    }
}