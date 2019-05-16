using RMERP.DAL.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using RMERP.DAL.ViewModel;
using Microsoft.AspNetCore.Hosting;
using RMERP.DAL.Helpers;

namespace RMERP.DAL.ManagerClasses
{    
    public class ReportsManager
    {
        RMERPContext _context;
        public ReportsManager(RMERPContext context)
        {
            _context = context;
        }
        public List<Employees> GetActiveEmployeesOfMonth(int WAG_Id)
        {
            List<Employees> employees = new List<Employees>();
            employees = _context.Wage_Register.Where(m => m.WAG_Id.Equals(WAG_Id)).Select(m => m.EMP_).Distinct().ToList();
            return employees;
        }
        public List<PFReportVM> PFReports(int WAG_Id)
        {
            List<PFReportVM> reports = new List<PFReportVM>();
            List<Employees> employees = GetActiveEmployeesOfMonth(WAG_Id);
            foreach(var emp in employees)
            {                
                WageRegisterManager registerManager = new WageRegisterManager(_context);
                List<Wage_Register> wage_Registers = registerManager.GetWageRegistersByEmpId(WAG_Id,emp.EMP_Id);
                PFReportVM PFreport = new PFReportVM();
                decimal GrossWages = 0M, EPFWages=0M, EPSWages=0M, EDLIWages=0M, EPF_CONTRI_REMITTED = 0M, EPS_CONTRI_REMITTED = 0M, EPF_EPS_DIFF_REMITTED = 0M, REFUND_OF_ADVANCES=0M;
                double NCP_DAYS = 0;
                foreach (var register in wage_Registers)
                {
                    GrossWages = ProjectUtils.GetGrossAmountBasedOnFormula(register.WAR_PF_Formula, register);
                    //GrossWages += register.WAR_GrossTotal;
                    EPFWages += GrossWages;
                    EPSWages += GrossWages;
                    EDLIWages += GrossWages;
                    NCP_DAYS += register.WAR_TotalPaybleDays;
                }
                EPF_CONTRI_REMITTED = (GrossWages * 12) / 100; //=ROUND(GrossWages*12%,0)
                EPS_CONTRI_REMITTED = ( GrossWages * Convert.ToDecimal(8.33)) / 100; // =ROUND(GrossWages*8.33%,0)
                EPF_EPS_DIFF_REMITTED = (EPF_CONTRI_REMITTED - EPS_CONTRI_REMITTED); // =EPF_CONTRI_REMITTED -EPS_CONTRI_REMITTED

                PFreport.GrossWages = GrossWages;
                PFreport.EPFWages = EPFWages;
                PFreport.EPSWages = EPSWages;
                PFreport.EDLIWages = EDLIWages;
                PFreport.EPF_CONTRI_REMITTED = EPF_CONTRI_REMITTED;
                PFreport.EPS_CONTRI_REMITTED = EPS_CONTRI_REMITTED;
                PFreport.EPF_EPS_DIFF_REMITTED = EPF_EPS_DIFF_REMITTED;
                PFreport.REFUND_OF_ADVANCES = REFUND_OF_ADVANCES;
                PFreport.NCP_DAYS = NCP_DAYS;
                PFreport.EMP_UAN_Number = emp.EMP_UAN_Number; ;
                PFreport.Emp_Id = emp.EMP_Id;
                PFreport.EMP_FirstName = emp.EMP_FirstName;
                PFreport.EMP_MiddleName = emp.EMP_MiddleName;
                PFreport.EMP_SurName = emp.EMP_SurName;
                PFreport.EMP_UAN_Number = emp.EMP_UAN_Number;
                reports.Add(PFreport);
            }
            return reports;
        }

        public List<ESICReportVM> ESICReports(int WAG_Id)
        {
            List<ESICReportVM> reports = new List<ESICReportVM>();
            List<Employees> employees = GetActiveEmployeesOfMonth(WAG_Id);
            foreach (var emp in employees)
            {
                WageRegisterManager registerManager = new WageRegisterManager(_context);
                List<Wage_Register> wage_Registers = registerManager.GetWageRegistersByEmpId(WAG_Id, emp.EMP_Id);
                ESICReportVM ESICreport = new ESICReportVM();
                decimal TotalMonthlyWages = 0M;
                double PayableDays = 0;
                foreach (var register in wage_Registers)
                {
                    TotalMonthlyWages = ProjectUtils.GetGrossAmountBasedOnFormula(register.WAR_ESIC_Formula, register);                    
                    PayableDays += register.WAR_TotalPaybleDays;
                }
                //ESICreport.ReasonCode = "";
                ESICreport.TotalMonthlyWages = TotalMonthlyWages;
                ESICreport.PayableDays = PayableDays;
                ESICreport.LastWorkingDay = DateTime.Now;
                ESICreport.EMP_FirstName = emp.EMP_FirstName;
                ESICreport.EMP_MiddleName = emp.EMP_MiddleName;
                ESICreport.EMP_SurName = emp.EMP_SurName;
                ESICreport.IP_Number = emp.EMP_ESIC_Number;
                reports.Add(ESICreport);
            }
            return reports;
        }
    }
}
