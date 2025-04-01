using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using RMERP.DAL.Models;
using System.Collections.Generic;
using RMERP.DAL.Helpers;

namespace RMERP.DAL.ManagerClasses
{
    public class AttendanceManager
    {
        RMERPContext _context;
        public AttendanceManager(RMERPContext context)
        {
            _context = context;
        }

        public IEnumerable<Attendance> getAttendance_Wage(int WAG_Id)
        {
            return _context.Attendances.Include(m => m.DES).Where(a => a.WAG_Id == WAG_Id);
        }

        public List<Attendance> getAttendance_Wage_Client(int WAG_Id, int CLI_Id)
        {
            return _context.Attendances.Where(a => a.WAG_Id == WAG_Id && a.CLI_Id == CLI_Id).Include(a => a.EMP).Include(a => a.WAG).Include(a => a.DES).ToList();
        }

        public List<Attendance> GetAttendance_WageClient_Employee(int WAG_Id, int CLI_Id, int EMP_Id)
        {
            return _context.Attendances.Where(a => a.WAG_Id == WAG_Id && a.CLI_Id == CLI_Id && a.EMP_Id == EMP_Id).Include(a => a.EMP).Include(a => a.WAG).Include(a => a.DES).ToList();
        }

        public IEnumerable<Attendance> getAttendance_Wage_Employee(int WAG_Id, int EMP_Id)
        {
            return _context.Attendances.Where(a => a.WAG_Id == WAG_Id && a.EMP_Id == EMP_Id);
        }

        public IEnumerable<Attendance> getAttendance_Wage_Client_Employee_Designation(int WAG_Id, int CLI_Id, int EMP_Id, int DES_Id)
        {
            return _context.Attendances.Where(a => a.WAG_Id == WAG_Id && a.CLI_Id == CLI_Id && a.EMP_Id == EMP_Id && a.DES_Id == DES_Id);
        }

        public void save(Attendance attendace)
        {
            try
            {
                _context.Attendances.Add(attendace);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void Save(List<Attendance> attendaces)
        {
            try
            {
                _context.Attendances.AddRange(attendaces);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void delete(Attendance attendace)
        {
            _context.Attendances.Remove(attendace);
            _context.SaveChanges();
        }

        public void deleteAllAttendanceofWageClient(int WAG_Id, int CLI_Id)
        {
            _context.Attendances.RemoveRange(_context.Attendances.Where(a => a.WAG_Id == WAG_Id && a.CLI_Id == CLI_Id).ToList());
            _context.SaveChanges();
        }

        #region Methods by rinku
        //public Clients_Employees assignEmployee(int Cli_Id,int EMP_Id)
        //{
        //    Clients_Employees clients_Employees = new Clients_Employees();
        //    clients_Employees = _context.Clients_Employees.Where(m => m.CLI_Id.Equals(Cli_Id) && m.EMP_Id.Equals(EMP_Id)).FirstOrDefault();
        //    return clients_Employees;
        //}
        //public List<Clients_Employees> assignEmployeeList(int Cli_Id)
        //{
        //    List<Clients_Employees> clients_Employees = new List<Clients_Employees>();
        //    clients_Employees = _context.Clients_Employees.Where(m => m.CLI_Id.Equals(Cli_Id)).ToList();
        //    return clients_Employees;
        //}        
        #endregion

        public bool IsAttendanceAlreadyUploaded(int WAG_Id, int CLI_Id)
        {
            return _context.Attendances.Where(m => m.WAG_Id.Equals(WAG_Id) && m.CLI_Id.Equals(CLI_Id)).Count() > 0;
        }

        public Tuple<DateTime, DateTime> GetFirstLastDateFromAttendance(int WAG_Id, int CLI_Id, int EMP_Id)
        {
            IEnumerable<Attendance> attendances = _context.Attendances.Where(m => m.WAG_Id.Equals(WAG_Id) && m.CLI_Id.Equals(CLI_Id) && m.EMP_Id.Equals(EMP_Id)).OrderBy(m => m.ATT_Date);
            DateOnly startDate = attendances.First().ATT_Date;
            DateOnly lastDate = attendances.Last().ATT_Date;
            return new Tuple<DateTime, DateTime>(new DateTime(startDate.Year, startDate.Month, startDate.Day), new DateTime(lastDate.Year, lastDate.Month, lastDate.Day));
        }
    }
}