using System;
using System.Collections.Generic;
using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using RMERP.DAL.ManagerClasses;
using Microsoft.Extensions.Configuration;
using RMERP.DAL.App_Code;
using RMERP.DAL.Helpers;

namespace RMERP.DAL.Mappers
{
    public class WageProcessMapper
    {
        public IConfiguration _configuration;

        public static WageProcessVM MapMe(Wage_Process wageProcess)
        {
            WageProcessVM wageProcessVM = new WageProcessVM();
            wageProcessVM.WAG_Id = wageProcess.WAG_Id;
            wageProcessVM.WAG_Month = Helpers.ProjectUtils.DateToDateTime(wageProcess.WAG_Month);
            wageProcessVM.WageStatus = wageProcess.WAG_Status;
            wageProcessVM.FRM_Id = wageProcess.FRM_Id;
            wageProcessVM.FRM_Title = wageProcess.FRM.FRM_ShortName;
            if (wageProcess.Attendances != null && wageProcess.Attendances.Count() > 0)
                wageProcessVM.Attendance = wageProcess.Attendances.ToList();
            if (wageProcess.Wage_Process_Clients != null && wageProcess.Wage_Process_Clients.Count() > 0)
                wageProcessVM.wage_Process_Clients = wageProcess.Wage_Process_Clients.ToList();
            if (wageProcess.Wage_Register_Advances != null && wageProcess.Wage_Register_Advances.Count() > 0)
                wageProcessVM.wage_Register_Advances = wageProcess.Wage_Register_Advances.ToList();
            return wageProcessVM;
        }

        public static WageProcessVM MapMe2(Wage_Process wageProcess)
        {
            WageProcessVM wageProcessVM = new()
            {
                WAG_Id = wageProcess.WAG_Id,
                WAG_Month = Helpers.ProjectUtils.DateToDateTime(wageProcess.WAG_Month),
                WageStatus = wageProcess.WAG_Status,
                FRM_Id = wageProcess.FRM_Id,
                FRM_Title = wageProcess.FRM.FRM_ShortName,
                Attendance_Summaries = wageProcess.Attendance_Summaries?.ToList(),
                wage_Process_Clients = wageProcess.Wage_Process_Clients?.ToList(),
                wage_Register_Advances = wageProcess.Wage_Register_Advances?.ToList()
            };
            return wageProcessVM;
        }

        public static WageProcessVM mapMeWageProcessVM(Wage_Process wageProcess, Firm firm, RMERPContext _context, IConfiguration _configuration)
        {
            WageProcessVM wageProcessVM = new WageProcessVM();
            ClientsManager clientsManager = new ClientsManager(_context, _configuration);
            WageProcessManager wageProcessManager = new WageProcessManager(_context);
            AdvanceWageRegisterManager advance = new AdvanceWageRegisterManager(_context);
            wageProcessVM.WAG_Id = wageProcess.WAG_Id;
            wageProcessVM.WageStatus = wageProcess.WAG_Status;
            wageProcessVM.WAG_Month = new DateTime(wageProcess.WAG_Month.Year, wageProcess.WAG_Month.Month, wageProcess.WAG_Month.Day);
            wageProcessVM.FRM_Id = wageProcess.FRM_Id;
            wageProcessVM.FRM_Title = firm.FRM_ShortName;
            wageProcessVM.totEmpTakeAdvance = advance.AdvanceRptForBank(new DateTime(wageProcess.WAG_Month.Year, wageProcess.WAG_Month.Month, wageProcess.WAG_Month.Day), firm.FRM_Id).Select(m => m.EMP_Id).Distinct().Count();
            List<Client> clients = clientsManager.GetActiveClientsOfMonthByFirmId(new DateTime(wageProcess.WAG_Month.Year, wageProcess.WAG_Month.Month, wageProcess.WAG_Month.Day), firm.FRM_Id);
            if (clients != null)
                wageProcessVM.ActiveClients = clients.Count();
            if (wageProcess.Attendances != null)
                wageProcessVM.Attendance = wageProcess.Attendances.ToList();
            if (wageProcess.Wage_Process_Clients != null)
                wageProcessVM.wage_Process_Clients = wageProcess.Wage_Process_Clients.ToList();
            if (wageProcess.Wage_Register_Advances != null)
                wageProcessVM.wage_Register_Advances = wageProcess.Wage_Register_Advances.ToList();
            return wageProcessVM;
        }

        public static IEnumerable<WageProcessVM> mapMeVMs(IEnumerable<Wage_Process> wage_Processes, Firm firm, RMERPContext _context, IConfiguration _configuration)
        {
            List<WageProcessVM> wageProcessVMs = new List<WageProcessVM>();
            foreach (Wage_Process item in wage_Processes)
            {
                wageProcessVMs.Add(mapMeWageProcessVM(item, firm, _context, _configuration));
            }
            return wageProcessVMs;
        }

        public static List<WageProcessClientAttendanceVM> MapClientToAttendanceWages(List<Client> clients, Wage_Process wage, IEnumerable<Attendance> lstAttendance)
        {
            List<WageProcessClientAttendanceVM> lst = new List<WageProcessClientAttendanceVM>();
            foreach (Client client in clients)
            {
                lst.Add(mapClientToAttendanceWage(client, wage, lstAttendance));
            }
            return lst;
        }

        public static WageProcessClientAttendanceVM mapClientToAttendanceWage(Client client, Wage_Process wage, IEnumerable<Attendance> lstAttendance)
        {
            WageProcessClientAttendanceVM obj = new WageProcessClientAttendanceVM();
            obj.CLI_Id = client.CLI_Id;
            obj.CLI_Name = client.CLI_Name;
            obj.totalEmployees = lstAttendance.Where(a => a.CLI_Id == client.CLI_Id).Select(a => a.EMP_Id).Distinct().Count();
            return obj;
        }

        public static WageProcessListVM mapMeList(Wage_Process wageProcess)
        {
            WageProcessListVM wageProcessVM = new WageProcessListVM();
            wageProcessVM.WAG_Id = wageProcess.WAG_Id;
            wageProcessVM.WAG_Month = Helpers.ProjectUtils.DateToDateTime(wageProcess.WAG_Month);
            wageProcessVM.WageStatus = wageProcess.WAG_Status;
            wageProcessVM.FRM_Id = wageProcess.FRM_Id;
            wageProcessVM.FRM_Title = wageProcess.FRM.FRM_ShortName;
            // if (wageProcess.Wage_Register_Advances != null)
            //    wageProcessVM.wage_Register_Advances = wageProcess.Wage_Register_Advances.ToList();
            return wageProcessVM;
        }

        public static WageProcessListVM mapMeWageProcessListVM(Wage_Process wageProcess, Firm firm, RMERPContext _context, IConfiguration _configuration)
        {
            WageProcessListVM wageProcessVM = new WageProcessListVM();
            ClientsManager clientsManager = new ClientsManager(_context, _configuration);
            WageProcessManager wageProcessManager = new WageProcessManager(_context);
            AdvanceWageRegisterManager advance = new AdvanceWageRegisterManager(_context);
            wageProcessVM.WAG_Id = wageProcess.WAG_Id;
            wageProcessVM.WageStatus = wageProcess.WAG_Status;
            wageProcessVM.WAG_Month = new DateTime(wageProcess.WAG_Month.Year, wageProcess.WAG_Month.Month, wageProcess.WAG_Month.Day);
            wageProcessVM.FRM_Id = wageProcess.FRM_Id;
            wageProcessVM.FRM_Title = firm.FRM_ShortName;
            wageProcessVM.totEmpTakeAdvance = advance.AdvanceRptForBank(new DateTime(wageProcess.WAG_Month.Year, wageProcess.WAG_Month.Month, wageProcess.WAG_Month.Day), firm.FRM_Id).Select(m => m.EMP_Id).Distinct().Count();
            List<Client> clients = clientsManager.GetActiveClientsOfMonthByFirmId(new DateTime(wageProcess.WAG_Month.Year, wageProcess.WAG_Month.Month, wageProcess.WAG_Month.Day), firm.FRM_Id);

            if (clients != null)
                wageProcessVM.ActiveClients = clients.Count();
            int imported = 0, wageRegSaved = 0;
            if (wageProcess.Attendances != null)
                imported = wageProcess.Attendances.Where(m => m.WAG_Id.Equals(wageProcess.WAG_Id)).Select(m => m.CLI_Id).Distinct().Count();
            if (wageProcess.Wage_Process_Clients != null)
                wageRegSaved = wageProcess.Wage_Process_Clients.Where(m => m.WAG_Id.Equals(wageProcess.WAG_Id)).Select(m => m.CLI_Id).Distinct().Count();
            int Notimported = clients.Count() - imported;

            wageProcessVM.ImportedClients = imported;
            wageProcessVM.NotImportedClients = Notimported;

            if (wageProcess.Wage_Process_Clients != null)
                wageRegSaved = wageProcess.Wage_Process_Clients.Where(m => m.WAG_Id.Equals(wageProcess.WAG_Id)).Select(m => m.CLI_Id).Distinct().Count();

            wageProcessVM.WageRegisterSaved = wageRegSaved;
            wageProcessVM.WageRegisterNotSaved = clients.Count() - wageRegSaved;

            //if (wageProcess.Wage_Register_Advances != null)
            //    wageProcessVM.wage_Register_Advances = wageProcess.Wage_Register_Advances.ToList();

            return wageProcessVM;
        }

        public static IEnumerable<WageProcessListVM> mapMeListVMs(IEnumerable<Wage_Process> wage_Processes, Firm firm, RMERPContext _context, IConfiguration _configuration)
        {
            List<WageProcessListVM> wageProcessVMs = [];
            foreach (Wage_Process item in wage_Processes)
            {
                wageProcessVMs.Add(mapMeWageProcessListVM(item, firm, _context, _configuration));
            }
            return wageProcessVMs;
        }

        #region NEW-ATTENDANCE PROCESS | 19-12-2024

        public static List<WageProcessClientAttendanceVM> MapClientToAttendanceWages(List<Client> clients, IEnumerable<Attendance_Summary> lstAttendance)
        {
            List<WageProcessClientAttendanceVM> lst = [];
            lst.AddRange(clients.Select(client => new WageProcessClientAttendanceVM
            {
                CLI_Id = client.CLI_Id,
                CLI_Name = client.CLI_Name,
                totalEmployees = lstAttendance.Where(a => a.CLI_Id == client.CLI_Id).Select(a => a.EMP_Id).Distinct().Count()
            }));
            return lst;
        }

        public static IEnumerable<WageProcessListVM> MapMeListVMs(IEnumerable<Wage_Process> wage_Processes, Firm firm, RMERPContext _context)
        {
            List<WageProcessListVM> wageProcessVMs = [];
            foreach (Wage_Process item in wage_Processes)
                wageProcessVMs.Add(MapMeWageProcessListVM(item, firm, _context));
            return wageProcessVMs;
        }

        public static WageProcessListVM MapMeWageProcessListVM(Wage_Process wageProcess, Firm firm, RMERPContext _context)
        {
            ClientsManager clientsManager = new(_context);
            AdvanceWageRegisterManager advance = new(_context);

            List<Client> clients = clientsManager.GetActiveClientsOfMonthByFirmId(new DateTime(wageProcess.WAG_Month.Year, wageProcess.WAG_Month.Month, wageProcess.WAG_Month.Day), firm.FRM_Id);

            WageProcessListVM wageProcessVM = new()
            {
                WAG_Id = wageProcess.WAG_Id,
                WageStatus = wageProcess.WAG_Status,
                FRM_Id = wageProcess.FRM_Id,
                FRM_Title = firm.FRM_ShortName,
                WAG_Month = new DateTime(wageProcess.WAG_Month.Year, wageProcess.WAG_Month.Month, wageProcess.WAG_Month.Day),
                totEmpTakeAdvance = advance.AdvanceRptForBank(new DateTime(wageProcess.WAG_Month.Year, wageProcess.WAG_Month.Month, wageProcess.WAG_Month.Day), firm.FRM_Id).Select(m => m.EMP_Id).Distinct().Count(),
                ActiveClients = clients != null && clients.Count > 0 ? clients.Count : 0
            };

            int imported = wageProcess.Attendance_Summaries != null ? wageProcess.Attendance_Summaries.Select(m => m.CLI_Id).Distinct().Count() : 0;
            wageProcessVM.ImportedClients = imported;
            wageProcessVM.NotImportedClients = wageProcessVM.ActiveClients > 0 ? wageProcessVM.ActiveClients - imported : 0;

            int wageRegSaved = wageProcess.Wage_Process_Clients != null ? wageProcess.Wage_Process_Clients.Where(m => m.WAG_Id.Equals(wageProcess.WAG_Id)).Select(m => m.CLI_Id).Distinct().Count() : 0;
            wageRegSaved = wageProcess.Wage_Process_Clients != null ? wageProcess.Wage_Process_Clients.Where(m => m.WAG_Id.Equals(wageProcess.WAG_Id)).Select(m => m.CLI_Id).Distinct().Count() : 0;

            wageProcessVM.WageRegisterSaved = wageRegSaved;
            wageProcessVM.WageRegisterNotSaved = wageProcessVM.ActiveClients > 0 ? wageProcessVM.ActiveClients - wageRegSaved : 0;

            //if (wageProcess.Wage_Register_Advances != null)
            //    wageProcessVM.wage_Register_Advances = wageProcess.Wage_Register_Advances.ToList();

            return wageProcessVM;
        }

        #endregion

        #region WAGE PROCESS CLIENTS | 23-12-2024



        #endregion
    }
}
