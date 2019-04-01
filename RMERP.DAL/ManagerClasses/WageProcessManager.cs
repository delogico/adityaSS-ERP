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
        public IEnumerable<Wage_Process> getWageProcessList(int AdminId)
        {
            IEnumerable<Wage_Process> list = _context.Wage_Process.Where(m => m.ADM_Id_RegisteredBy.Equals(AdminId)).Include(m => m.Attendance).OrderByDescending(m=>m.WAG_RegisteredOn).ToList();
            return list;
        }
        public Wage_Process getWageProcessById(int WAG_Id)
        {
            Wage_Process wageProcess = _context.Wage_Process.Where(m => m.WAG_Id == WAG_Id).FirstOrDefault();
            return wageProcess;
        }

        public string CreateNextMonthWage(int AdminId)
        {
            string res = string.Empty;
            Wage_Process wageProcess = new Wage_Process();           
            wageProcess.WAG_Month = nextWageMonth(AdminId);            
            wageProcess.WAG_RegisteredOn = ProjectUtils.DateNow();
            wageProcess.ADM_Id_RegisteredBy = AdminId;
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
        public DateTime nextWageMonth(int AdminId)
        {
            int count = _context.Wage_Process.Where(m => m.ADM_Id_RegisteredBy.Equals(AdminId)).Count();
            if (count > 0)
            {
                var wp = _context.Wage_Process.Where(m => m.ADM_Id_RegisteredBy.Equals(AdminId)).OrderByDescending(m => m.WAG_Month).First();
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
        public string SaveWageRegister(List<Wage_Register> wage_Registers, int WAG_Id, string CurrentActiveCLI_Id,int AdminID)
        {
            string res = string.Empty;
            try
            {
                Wage_Process_Clients process_Client = new Wage_Process_Clients();
                process_Client.WAG_Id = WAG_Id;
                process_Client.CLI_Id =Convert.ToInt32(CurrentActiveCLI_Id);
                process_Client.WPC_WageRegisterSaved = true;
                process_Client.ADM_Id_SavedBy = AdminID;
                process_Client.WPC_SavedOn = ProjectUtils.DateNow();

                _context.Wage_Process_Clients.Add(process_Client);
                _context.AddRange(wage_Registers);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }            
            return res;
        }
        public string ResetWageRegister(int WAG_Id,string CurrentActiveCLI_Id)
        {
            string res = string.Empty;
            List<Wage_Register> lst = _context.Wage_Register.Where(m => m.WAG_Id.Equals(WAG_Id) && m.CLI_Id.Equals(Convert.ToInt32(CurrentActiveCLI_Id))).ToList();
            _context.Wage_Register.RemoveRange(lst);
            Wage_Process_Clients wg  = _context.Wage_Process_Clients.Where(m => m.WAG_Id.Equals(WAG_Id) && m.CLI_Id.Equals(Convert.ToInt32(CurrentActiveCLI_Id))).FirstOrDefault();
            _context.Wage_Process_Clients.RemoveRange(wg);
            _context.SaveChanges();
            return res;
        }
        
        public List<Wage_Process_Clients> GetWage_Process_Clients(int WAG_Id)
        {
            List<Wage_Process_Clients> lst = new List<Wage_Process_Clients>();
            lst = _context.Wage_Process_Clients.Where(m=>m.WAG_Id.Equals(WAG_Id)).ToList();
            return lst;
        }


    }
}
