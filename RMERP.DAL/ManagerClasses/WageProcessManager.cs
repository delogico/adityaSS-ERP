using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using RMERP.DAL.Models;
using RMERP.DAL.App_Code;
using System.Globalization;
using Microsoft.Extensions.Configuration;

namespace RMERP.DAL.ManagerClasses
{
    public class WageProcessManager
    {
        RMERPContext _context;
        public IConfiguration _configuration;
        public WageProcessManager(RMERPContext context, IConfiguration configuration = null)
        {
            _context = context;
            _configuration = configuration;
        }
        public IEnumerable<Wage_Process> getAllWageProcesses()
        {
            IEnumerable<Wage_Process> list = _context.Wage_Processes.OrderBy(m => m.WAG_Month).ToList();
            return list;
        }
        public IEnumerable<Wage_Process> getPendingWageProcessList(int FRM_Id)
        {
            DateTime dtTesting = Convert.ToDateTime(_configuration.GetSection("TESTING_MONTH_UPTO").Value);
            DateOnly dtOnly = new(dtTesting.Year, dtTesting.Month, dtTesting.Day);

            return _context.Wage_Processes.Where(m => m.FRM_Id == FRM_Id && !m.WAG_Status && m.WAG_Month > dtOnly)
                //.Include(m => m.Attendances)
                //.Include(m => m.Wage_Process_Clients)
                .OrderByDescending(m => m.WAG_Month);
        }

        public Wage_Process getWageProcessById(int WAG_Id)
        {
            Wage_Process wageProcess = _context.Wage_Processes.Where(m => m.WAG_Id == WAG_Id).Include(m => m.Wage_Process_Clients).Include(m => m.Wage_Register_Advances).Include(m => m.Attendances).Include(m => m.FRM).FirstOrDefault();
            return wageProcess;
        }

        public string CreateNextMonthWage(int AdminId, int FRM_Id, DateTime date)
        {
            string res = string.Empty;
            if (_context.Wage_Processes.Where(m => m.FRM_Id == FRM_Id && m.WAG_Month.Month == date.Month && m.WAG_Month.Year == date.Year).FirstOrDefault() == null)
            {

                Wage_Process wageProcess = new Wage_Process();
                wageProcess.WAG_Month = DateOnly.FromDateTime(date);
                wageProcess.WAG_RegisteredOn = ProjectUtils.DateNow();
                wageProcess.ADM_Id_RegisteredBy = AdminId;
                wageProcess.FRM_Id = FRM_Id;
                _context.Wage_Processes.Add(wageProcess);
                _context.SaveChanges();
            }
            else
            {
                FirmsManager firmsManager = new FirmsManager(_context);
                Firm firm = firmsManager.GetFirm(FRM_Id);
                res = "Wage Register for " + date.ToString("MMMM") + " is already available in " + firm.FRM_ShortName;
            }
            return res;
        }
        public string DeleteWageProcess(int WagId)
        {
            string res = string.Empty;
            try
            {
                var wp = _context.Wage_Processes.Find(WagId);
                _context.Wage_Processes.Remove(wp);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return "";
        }

        public string GetMonthFromID(int WagId)
        {
            string res = string.Empty;
            try
            {
                var monthYear = _context.Wage_Processes.Find(WagId);
                res = monthYear.WAG_Month.ToString("MMMM", CultureInfo.CreateSpecificCulture("IN")) + "-" + monthYear.WAG_Month.Year.ToString();
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
            list = _context.Attendances.Include(m => m.EMP).Include(m => m.CLI).Where(m => m.WAG_Id.Equals(wagId) && m.CLI_Id.Equals(CliId)).OrderBy(m => m.EMP.EMP_FirstName).ToList();
            return list;
        }
        public List<Attendance> GetAttendanceList(int wagId, int CliId, int empId)
        {
            List<Attendance> list = null;
            list = _context.Attendances.Include(m => m.EMP).Include(m => m.CLI).Where(m => m.WAG_Id.Equals(wagId) && m.CLI_Id.Equals(CliId) && m.EMP_Id.Equals(empId)).ToList();
            return list;
        }
        public string UpdateAttendance(Attendance attendance)
        {
            string res = string.Empty;
            try
            {
                _context.Attendances.Update(attendance);
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
            attendance = _context.Attendances.Find(attId);
            return attendance;
        }

        public List<Wage_Process_Client> GetWage_Process_Clients(int WAG_Id)
        {
            List<Wage_Process_Client> lst = new List<Wage_Process_Client>();
            lst = _context.Wage_Process_Clients.Where(m => m.WAG_Id.Equals(WAG_Id)).ToList();
            return lst;
        }

        public Wage_Process_Client GetWage_Process_Clients(int WAG_Id, int CLI_Id)
        {
            return _context.Wage_Process_Clients.Where(c => c.WAG_Id == WAG_Id && c.CLI_Id == CLI_Id).FirstOrDefault();
        }
        public string WageRegisterStatus(Wage_Process wage)
        {
            string res = string.Empty;
            try
            {
                wage.WAG_Status = true;
                _context.Wage_Processes.Update(wage);
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
        public IEnumerable<Wage_Process> getDistinctWageMonths(int FRM_Id)
        {
            IEnumerable<Wage_Process> list = null;
            if (FRM_Id > 0)
            {
                list = _context.Wage_Processes.Where(m => m.FRM_Id == FRM_Id).Include(m => m.Attendances).Include(m => m.Wage_Process_Clients).OrderBy(m => m.WAG_Month).ToList();

            }
            else
            {
                list = _context.Wage_Processes.Include(m => m.Attendances).Include(m => m.Wage_Process_Clients).OrderBy(m => m.WAG_Month).ToList();

            }
            return list;
        }

        public IEnumerable<Wage_Process> GetWagFromDate(DateTime date, int FRM_Id)
        {
            if (FRM_Id > 0)
            {
                return _context.Wage_Processes.Where(m => m.WAG_Month.Month.Equals(date.Month) && m.WAG_Month.Year.Equals(date.Year) && m.FRM_Id.Equals(FRM_Id)).Include(m => m.FRM);
            }
            else
            {
                return _context.Wage_Processes.Where(m => m.WAG_Month.Month.Equals(date.Month) && m.WAG_Month.Year.Equals(date.Year)).Include(m => m.FRM);
            }
        }


        public IEnumerable<Wage_Process> getPendingWageProcessListByYearMonth(int FRM_Id, int Year, int Month)
        {
            IEnumerable<Wage_Process> list = getPendingWageProcessList(FRM_Id);
            if (Year > 0)
            {
                list = list.Where(m => m.WAG_Month.Year.Equals(Year));
            }
            if (Month > 0)
            {
                list = list.Where(m => m.WAG_Month.Month.Equals(Month));
            }
            return list;
        }
        public IEnumerable<Wage_Process> getTestingWageProcessList(int FRM_Id, DateTime dt)
        {
            return _context.Wage_Processes.Where(m => m.FRM_Id == FRM_Id && m.WAG_Month <= DateOnly.FromDateTime(dt))
                 .Include(m => m.Attendances)
                 .Include(m => m.Wage_Process_Clients).OrderByDescending(m => m.WAG_Month);
        }

        #region ADDED ON 20-12-2024

        public IEnumerable<Wage_Process> GetPendingWageProcessList(int FRM_Id)
        {
            DateTime dtTesting = Convert.ToDateTime(_configuration.GetSection("NEW_VERSION_STARTED").Value);
            // DateTime dtTesting = Convert.ToDateTime("01/03/2025");
            DateOnly dtOnly = new(dtTesting.Year, dtTesting.Month, dtTesting.Day);
            //DateOnly dtOnly = new(2025, 3, 1);
            return _context.Wage_Processes.Where(m => m.FRM_Id == FRM_Id && !m.WAG_Status)
                .Include(m => m.Attendance_Summaries)
                .Include(m => m.Wage_Process_Clients).OrderByDescending(m => m.WAG_Month);
        }

        public IEnumerable<Wage_Process> getWageProcessListByYearMonth(int FRM_Id, int Year, int? Month)
        {
            IEnumerable<Wage_Process> list = null;
            list = _context.Wage_Processes.Where(m => m.FRM_Id == FRM_Id && m.WAG_Month.Year.Equals(Year))
                .Include(m => m.Attendance_Summaries)
                .Include(m => m.Wage_Process_Clients).OrderByDescending(m => m.WAG_Month);
            if (Month.HasValue && Month > 0)
            {
                list = list.Where(m => m.WAG_Month.Month.Equals(Month));
            }
            return list;
        }

        public Wage_Process GetWageProcessByWAG_Id(int WAG_Id, bool AttSummaries = false, bool ProcessClients = false, bool RegisterAdvances = false, bool Firm = false)
        {
            var query = _context.Wage_Processes.Where(m => m.WAG_Id == WAG_Id);

            if (AttSummaries) query = query.Include(f => f.Attendance_Summaries);

            if (ProcessClients) query = query.Include(f => f.Wage_Process_Clients);

            if (RegisterAdvances) query = query.Include(f => f.Wage_Register_Advances);

            if (Firm) query = query.Include(f => f.FRM);

            return query.AsSplitQuery().FirstOrDefault();
        }

        public Wage_Process GetWageProcess_WAG_Id(int WAG_Id)
        {
            var query = _context.Wage_Processes.Where(m => m.WAG_Id == WAG_Id)
                .Include(f => f.FRM)
                .Include(f => f.Wage_Registers).ThenInclude(r => r.CRI).ThenInclude(cc => cc.DES)
                .Include(f => f.Wage_Registers).ThenInclude(r => r.EMP)
                .Include(f => f.Wage_Process_Clients).ThenInclude(c => c.CLI);
            return query.AsSplitQuery().AsNoTrackingWithIdentityResolution().FirstOrDefault();
        }

        #endregion
    }
}
