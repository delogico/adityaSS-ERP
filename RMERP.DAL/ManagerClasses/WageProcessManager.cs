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
        public IEnumerable<Wage_Process> getAllWageProcesses()
        {
            IEnumerable<Wage_Process> list = _context.Wage_Process.OrderBy(m => m.WAG_Month).ToList();
            return list;
        }
        public IEnumerable<Wage_Process> getWageProcessList(int FRM_Id)
        {
            IEnumerable<Wage_Process> list = _context.Wage_Process.Where(m=>m.FRM_Id == FRM_Id).Include(m => m.Attendance).Include(m=>m.Wage_Process_Clients).OrderBy(m=>m.WAG_Month).ToList();
            return list;
        }
        public Wage_Process getWageProcessById(int WAG_Id)
        {
            Wage_Process wageProcess = _context.Wage_Process.Include(m=>m.Wage_Process_Clients).Include(m=>m.Wage_Register_Advances).Include(m=>m.Attendance).Include(m=>m.FRM_).Where(m => m.WAG_Id == WAG_Id).FirstOrDefault();
            return wageProcess;
        }

        public string CreateNextMonthWage(int AdminId,int FRM_Id)
        {
            string res = string.Empty;
            Wage_Process wageProcess = new Wage_Process();           
            wageProcess.WAG_Month = nextWageMonth(AdminId, FRM_Id);            
            wageProcess.WAG_RegisteredOn = ProjectUtils.DateNow();
            wageProcess.ADM_Id_RegisteredBy = AdminId;
            wageProcess.FRM_Id = FRM_Id;
            _context.Wage_Process.Add(wageProcess);
            _context.SaveChanges();
            return res;
        }
        public string DeleteWageProcess(int WagId)
        {
            string res = string.Empty;
            try
            {
                var wp = _context.Wage_Process.Find(WagId);
                _context.Wage_Process.Remove(wp);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return "";
        }
        public DateTime nextWageMonth(int AdminId,int FirmId)
        {
            int count = _context.Wage_Process.Where(m => m.ADM_Id_RegisteredBy.Equals(AdminId) && m.FRM_Id.Equals(FirmId)).Count();
            if (count > 0)
            {
                var wp = _context.Wage_Process.Where(m => m.ADM_Id_RegisteredBy.Equals(AdminId) && m.FRM_Id.Equals(FirmId)).OrderByDescending(m => m.WAG_Month).First();
                return wp.WAG_Month.AddMonths(1);
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
                var monthYear = _context.Wage_Process.Find(WagId);
                res = monthYear.WAG_Month.ToString("MMMM", CultureInfo.CreateSpecificCulture("IN"))+"-"+ monthYear.WAG_Month.Year.ToString();
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
            list = _context.Attendance.Include(m=>m.EMP_).Include(m=>m.CLI_).Where(m=>m.WAG_Id.Equals(wagId) && m.CLI_Id.Equals(CliId)).OrderBy(m=>m.EMP_.EMP_FirstName).ToList();           
            return list;
        }
        public List<Attendance> GetAttendanceList(int wagId, int CliId,int empId)
        {
            List<Attendance> list = null;
            list = _context.Attendance.Include(m => m.EMP_).Include(m => m.CLI_).Where(m => m.WAG_Id.Equals(wagId) && m.CLI_Id.Equals(CliId) &&m.EMP_Id.Equals(empId)).ToList();
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
        
        public List<Wage_Process_Clients> GetWage_Process_Clients(int WAG_Id)
        {
            List<Wage_Process_Clients> lst = new List<Wage_Process_Clients>();
            lst = _context.Wage_Process_Clients.Where(m=>m.WAG_Id.Equals(WAG_Id)).ToList();
            return lst;
        }        

        public Wage_Process_Clients GetWage_Process_Clients(int WAG_Id, int CLI_Id)
        {
            return _context.Wage_Process_Clients.Where(c => c.WAG_Id == WAG_Id && c.CLI_Id == CLI_Id).FirstOrDefault();
        }
        public string WageRegisterStatus(Wage_Process wage)
        {
            string res = string.Empty;
            try
            {
                wage.WAG_Status = true;
                _context.Wage_Process.Update(wage);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }                       
            return res;
        }
        public bool confirmWageRegister(int WAG_Id)
        {
            return true;
        }
    }
}
