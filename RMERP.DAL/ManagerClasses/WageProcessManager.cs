using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using RMERP.DAL.Models;
using RMERP.DAL.App_Code;
using System.Globalization;

namespace RMERP.DAL.ManagerClasses
{
    public class WageProcessManager
    {
        RMERPContext _context;
        public WageProcessManager(RMERPContext context)
        {
            _context = context;
        }
        public IEnumerable<WageProcess> getWageProcessList(int AdminId)
        {
            IEnumerable<WageProcess> list = _context.WageProcess.Where(m => m.AdmIdRegisteredBy.Equals(AdminId)).Include(m => m.Attendance).OrderByDescending(m=>m.WagRegisteredOn).ToList();
            return list;
        }
        
        public string CreateNextMonthWage(int AdminId)
        {
            string res = string.Empty;
            WageProcess wageProcess = new WageProcess();           
            wageProcess.WagMonth = nextWageMonth(AdminId);            
            wageProcess.WagRegisteredOn = ProjectUtils.DateNow();
            wageProcess.AdmIdRegisteredBy = AdminId;
            _context.WageProcess.Add(wageProcess);
            _context.SaveChanges();
            return res;
        }
        public string DeleteWageProcess(int WagId)
        {
            string res = string.Empty;
            try
            {
                var wp = _context.WageProcess.Find(WagId);
                _context.WageProcess.Remove(wp);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return "";
        }
        public DateTime nextWageMonth(int AdminId)
        {
            int count = _context.WageProcess.Where(m => m.AdmIdRegisteredBy.Equals(AdminId)).Count();
            if (count > 0)
            {
                var wp = _context.WageProcess.Where(m => m.AdmIdRegisteredBy.Equals(AdminId)).OrderByDescending(m => m.WagMonth).First();
                return wp.WagMonth.AddMonths(1);
            }
            else
            {
                return ProjectUtils.DateNow();
            }
        }

        public string GetMonthFromID(int WagId)
        {
            string res = string.Empty;
            try
            {
                var monthYear = _context.WageProcess.Find(WagId);
                res = monthYear.WagMonth.ToString("MMMM", CultureInfo.CreateSpecificCulture("IN"))+"-"+ monthYear.WagMonth.Year.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
            return res;
        }
        public IEnumerable<Attendance> GetAttendanceList(int wagId, int CliId)
        {
            IEnumerable<Attendance> list = null;
            list = _context.Attendance.Include(m=>m.Emp).Include(m=>m.Cli).Where(m=>m.WagId.Equals(wagId) && m.CliId.Equals(CliId)).OrderBy(m=>m.Emp.EmpFirstName).ToList();           
            return list;
        }
        public List<Attendance> GetAttendanceList(int wagId, int CliId,int empId)
        {
            List<Attendance> list = null;
            list = _context.Attendance.Include(m => m.Emp).Include(m => m.Cli).Where(m => m.WagId.Equals(wagId) && m.CliId.Equals(CliId) &&m.EmpId.Equals(empId)).ToList();
            return list;
        }
        public string UpdateAttendance(Attendance attendance)
        {
            string res = string.Empty;
            try
            {      
               _context.Attendance.Update(attendance);                   
               _context.SaveChanges();                              
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        }
        public Attendance GetAttendanceById(int attId)
        {
            Attendance attendance = new Attendance();
            attendance = _context.Attendance.Find(attId);
            return attendance;
        }
    }
}
