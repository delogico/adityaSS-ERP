using System;
using System.Collections.Generic;
using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using RMERP.DAL.ManagerClasses;
using Microsoft.Extensions.Configuration;

namespace RMERP.DAL.Mappers
{
    public class WageProcessMapper
    {
        private readonly RMERPContext _context = new RMERPContext();
        public IConfiguration _configuration;

        public static WageProcessVM mapMe(Wage_Process wageProcess)
        {            
            WageProcessVM wageProcessVM = new WageProcessVM();
            wageProcessVM.WAG_Id = wageProcess.WAG_Id;
            wageProcessVM.WAG_Month = wageProcess.WAG_Month;
            wageProcessVM.WageStatus = wageProcess.WAG_Status;
            wageProcessVM.FRM_Id = wageProcess.FRM_Id;
            if (wageProcess.Attendance != null)           
                wageProcessVM.Attendance = wageProcess.Attendance.ToList();
            if (wageProcess.Wage_Process_Clients != null)
                wageProcessVM.wage_Process_Clients = wageProcess.Wage_Process_Clients.ToList();
            if (wageProcess.Wage_Register_Advances != null)
                wageProcessVM.wage_Register_Advances = wageProcess.Wage_Register_Advances.ToList();
            return wageProcessVM;
        }

        public static WageProcessVM mapMeWageProcessVM(Wage_Process wageProcess,int FirmID, RMERPContext _context, IConfiguration _configuration)
        {
            WageProcessVM wageProcessVM = new WageProcessVM();
            ClientsManager clientsManager = new ClientsManager(_context, _configuration);
            WageProcessManager wageProcessManager = new WageProcessManager(_context);
            AdvanceWageRegisterManager advance = new AdvanceWageRegisterManager(_context);            
            wageProcessVM.WAG_Id = wageProcess.WAG_Id;
            wageProcessVM.WageStatus = wageProcess.WAG_Status;
            wageProcessVM.WAG_Month = wageProcess.WAG_Month;
            wageProcessVM.FRM_Id = wageProcess.FRM_Id;
            wageProcessVM.totEmpTakeAdvance = advance.AdvanceRptForBank((wageProcess.WAG_Month)).Select(m=>m.EMP_Id).Distinct().Count();            
            List<Clients> clients = clientsManager.GetActiveClientOfMonthByFirmId(wageProcess.WAG_Month, FirmID);
            if (clients!=null)
                wageProcessVM.ActiveClients = clients.Count();
            if (wageProcess.Attendance != null)
                wageProcessVM.Attendance = wageProcess.Attendance.ToList();            
            if (wageProcess.Wage_Process_Clients != null)
                wageProcessVM.wage_Process_Clients = wageProcess.Wage_Process_Clients.ToList();
            if (wageProcess.Wage_Register_Advances != null)
                wageProcessVM.wage_Register_Advances = wageProcess.Wage_Register_Advances.ToList();
            return wageProcessVM;
        }

        public static IEnumerable<WageProcessVM> mapMeVMs(IEnumerable<Wage_Process> wage_Processes,int FirmId, RMERPContext _context, IConfiguration _configuration)
        {
            List<WageProcessVM> wageProcessVMs = new List<WageProcessVM>();
            foreach(Wage_Process item in wage_Processes)
            {
                wageProcessVMs.Add(mapMeWageProcessVM(item,FirmId, _context, _configuration));
            }
            return wageProcessVMs;
        }

        public static List<WageProcessClientAttendanceVM> mapClientToAttendanceWages(List<Clients> clients, Wage_Process wage, List<Attendance> lstAttendance)
        {
            List<WageProcessClientAttendanceVM> lst = new List<WageProcessClientAttendanceVM>();
            foreach (Clients client in clients){
                lst.Add(mapClientToAttendanceWage(client, wage, lstAttendance));
            }
            return lst;
        }

        public static WageProcessClientAttendanceVM mapClientToAttendanceWage(Clients client, Wage_Process wage, List<Attendance> lstAttendance)
        {
            WageProcessClientAttendanceVM obj = new WageProcessClientAttendanceVM();
            obj.CLI_Id = client.CLI_Id;
            obj.CLI_Name = client.CLI_Name;
            obj.totalEmployees = lstAttendance.Where(a => a.CLI_Id == client.CLI_Id).Select(a=>a.EMP_Id).Distinct().Count();
            return obj;
        }
    }
}
