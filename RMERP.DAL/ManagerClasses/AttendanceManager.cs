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

        public List<Attendance> getAttendance_Wage_Client(int WAG_Id, int CLI_Id)
        {
            return _context.Attendance.Where(a => a.WAG_Id == WAG_Id && a.CLI_Id == CLI_Id).Include(a=>a.EMP_).Include(a=>a.WAG_).Include(a=>a.DES_).ToList();
        }

        public void save(Attendance attendace)
        {
            try
            {
                _context.Attendance.Add(attendace);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
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
        #region Methods by rinku
        public Clients_Employees assignEmployee(int Cli_Id,int EMP_Id)
        {
            Clients_Employees clients_Employees = new Clients_Employees();
            clients_Employees = _context.Clients_Employees.Where(m => m.CLI_Id.Equals(Cli_Id) && m.EMP_Id.Equals(EMP_Id)).FirstOrDefault();
            return clients_Employees;
        }
        public List<Clients_Employees> assignEmployeeList(int Cli_Id)
        {
            List<Clients_Employees> clients_Employees = new List<Clients_Employees>();
            clients_Employees = _context.Clients_Employees.Where(m => m.CLI_Id.Equals(Cli_Id)).ToList();
            return clients_Employees;
        }        
        #endregion
    }
}
