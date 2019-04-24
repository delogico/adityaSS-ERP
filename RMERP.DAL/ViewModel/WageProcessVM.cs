using System;
using System.Collections.Generic;
using System.Text;
using RMERP.DAL.Models;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace RMERP.DAL.ViewModel
{
    public class WageProcessVM
    {        
        public int WAG_Id { get; set; }
        public DateTime WAG_Month { get; set; }
        public List<Attendance> Attendance { get; set; }
        public List<Wage_Process_Clients> wage_Process_Clients { get; set; }
        public List<Wage_Register_Advances> wage_Register_Advances { get; set; }      
        public bool WageStatus { get; set; }
        public int ActiveClients { get; set; }

        int imported = 0;
        public int ImportedClients()
        {            
            if (this.Attendance!=null)
                imported = this.Attendance.Where(m=>m.WAG_Id.Equals(this.WAG_Id)).Select(m => m.CLI_Id).Distinct().Count();
            return imported;
        }
        public int NotImportedClients()
        {
            int Notimported = this.ActiveClients - imported;
            return Notimported;
        }
        int wageRegSaved = 0;
        public int WageRegisterSaved()
        {            
            if (this.wage_Process_Clients != null)
                wageRegSaved = this.wage_Process_Clients.Where(m => m.WAG_Id.Equals(this.WAG_Id)).Select(m => m.CLI_Id).Distinct().Count();
            return wageRegSaved;
        }
        public int WageRegisterNotSaved()
        {
            int wageRegNotSaved = this.ActiveClients - wageRegSaved;
            return wageRegNotSaved;
        }
        public string WAG_Full_Month()
        {
            return WAG_Month.ToString("MMMM") + "-" + WAG_Month.ToString("yyyy");
        }
        public int totEmpTakeAdvance { get; set; }
    }
    
    public class WageProcessClientAttendancePageVM
    {
        public WageProcessVM wageProcess { get; set; }
        public List<WageProcessClientAttendanceVM> lstClient { get; set; }
    }

    public class WageProcessClientAttendanceVM
    {
        public int CLI_Id { get; set; }
        public string CLI_Name { get; set; }
        public int totalEmployees { get; set; }
    }

    public class WageProcessClientVM
    {
        public int WPC_Id { get; set; }
        public bool WPC_WageRegisterSaved { get; set; }
        public DateTime WPC_SavedOn { get; set; }
    }

    
}
