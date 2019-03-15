using System;
using System.Collections.Generic;
using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace RMERP.DAL.Mappers
{
    public class WageProcessMapper
    {
        public static WageProcessVM mapMe(Wage_Process wageProcess)
        {
            WageProcessVM wageProcessVM = new WageProcessVM();
            wageProcessVM.WAG_Id = wageProcess.WAG_Id;
            wageProcessVM.WAG_Month = wageProcess.WAG_Month;
            return wageProcessVM;
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
