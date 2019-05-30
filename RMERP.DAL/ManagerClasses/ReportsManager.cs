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

        public List<NEFT_BankReportVM> NEFT_BankReports(int WAG_Id)
        {
            List<NEFT_BankReportVM> reports = new List<NEFT_BankReportVM>();
            //List<Employees> employees = GetActiveEmployeesOfMonth(WAG_Id);
            WageRegisterManager registerManager = new WageRegisterManager(_context);
            List<Wage_Register> wage_Registers = registerManager.GetWageRegistersByWAG_Id(WAG_Id);
            
            foreach (var cli in wage_Registers.Select( m => new { m.CLI_Id ,m.CLI_.CLI_Name}).Distinct())
            {
                NEFT_BankReportVM bankReportVM = new NEFT_BankReportVM();
                bankReportVM.CLI_Id = cli.CLI_Id;
                bankReportVM.CLI_Name = cli.CLI_Name;
                List<Wage_Register> register = registerManager.GetWageRegisters(WAG_Id,cli.CLI_Id);
                List<NEFTBank_EMP_ReportVM> rptEmployees = new List<NEFTBank_EMP_ReportVM>();
                foreach(var wage in register)
                {
                    NEFTBank_EMP_ReportVM rptEmployee = new NEFTBank_EMP_ReportVM();
                    rptEmployee.EMP_FirstName = wage.EMP_.EMP_FirstName;
                    rptEmployee.EMP_MiddleName = wage.EMP_.EMP_MiddleName;
                    rptEmployee.EMP_SurName = wage.EMP_.EMP_SurName;
                    rptEmployee.EMP_Account_Number = "";
                    rptEmployee.CURRENCY_CODE = "INR";
                    rptEmployee.PART_TRAN_TYPE = "C";
                    rptEmployee.TRANSACTION_AMOUNT = wage.WAR_FinalTotal;
                    DateTime WAG_Month = wage_Registers.First().WAG_.WAG_Month;
                    rptEmployee.TRANSACTION_PARTICULARS = "Salary " + WAG_Month.ToString("MMMM") + "-" + WAG_Month.ToString("yyyy");
                    rptEmployees.Add(rptEmployee);
                }
                bankReportVM.NEFTBank_EMP_ReportVMs = rptEmployees;
                reports.Add(bankReportVM);
            }            
            return reports;
        }

        public List<MLWF_ContributionVM> MLWF_ContributionReports(int WAG_Id)
        {
            List<MLWF_ContributionVM> reports = new List<MLWF_ContributionVM>();
            WageRegisterManager registerManager = new WageRegisterManager(_context);
            List<Wage_Register> wage_Registers = registerManager.GetWageRegistersByWAG_Id(WAG_Id);
            foreach(var item in wage_Registers.Select(m => new { m.CLI_Id ,m.CLI_.CLI_Name}).Distinct())
            {
                MLWF_ContributionVM report = new MLWF_ContributionVM();
                report.CLI_Id = item.CLI_Id;
                report.CLI_Name = item.CLI_Name;
                List<Wage_Register> register = registerManager.GetWageRegisters(WAG_Id, item.CLI_Id);
                int EMP_BELOW_3K = 0, EMP_ABOVE_3K = 0;
                foreach (var emp in register)
                {
                    if(emp.WAR_FinalTotal > 0 && emp.WAR_FinalTotal < 3000)
                    {
                        EMP_BELOW_3K++;
                    }
                    else if(emp.WAR_FinalTotal >= 3000)
                    {
                        EMP_ABOVE_3K++;
                    }
                }
                report.EMP_ABOVE_3K = EMP_ABOVE_3K;
                report.EMP_BELOW_3K = EMP_BELOW_3K;
                reports.Add(report);
            }
            return reports;
        }

        public List<PayTaxReportVM> PayTaxReports(int WAG_Id)
        {
            List<PayTaxReportVM> reports = new List<PayTaxReportVM>();
            WageRegisterManager registerManager = new WageRegisterManager(_context);
            List<Wage_Register> wage_Registers = registerManager.GetWageRegistersByWAG_Id(WAG_Id);
            foreach (var item in wage_Registers.Select(m => new { m.CLI_Id, m.CLI_.CLI_Name }).Distinct())
            {
                PayTaxReportVM report = new PayTaxReportVM();
                report.CLI_Id = item.CLI_Id;
                report.CLI_Name = item.CLI_Name;               
                List<Wage_Register> register = registerManager.GetWageRegisters(WAG_Id, item.CLI_Id);
                int UpTo7500 = 0, UpTo7500Ladies = 0, UpTo10000=0, UpTo10000Ladies=0, Above10000=0, Above10000Ladies=0;
                foreach (var emp in register)
                {
                    if (emp.WAR_FinalTotal > 0 && emp.WAR_FinalTotal <= 7500)
                    {
                        if (emp.EMP_.EMP_Gender.Equals(true)) //Male
                        {
                            UpTo7500++;
                        }
                        else
                        {
                            UpTo7500Ladies++;
                        }
                       
                    }
                    else if (emp.WAR_FinalTotal > 7500 && emp.WAR_FinalTotal <= 10000)
                    {
                        if (emp.EMP_.EMP_Gender.Equals(true)) //Male
                        {
                            UpTo10000++;
                        }
                        else
                        {
                            UpTo10000Ladies++;
                        }
                    }
                    else if (emp.WAR_FinalTotal > 10000)
                    {
                        if (emp.EMP_.EMP_Gender.Equals(true)) //Male
                        {
                            Above10000++;
                        }
                        else
                        {
                            Above10000Ladies++;
                        }
                    }
                }
                report.UpTo7500 = UpTo7500;
                report.UpTo7500Ladies = UpTo7500Ladies;
                report.UpTo10000 = UpTo10000;
                report.UpTo10000Ladies = UpTo10000Ladies;
                report.Above10000 = Above10000;
                report.Above10000Ladies = Above10000Ladies;
                reports.Add(report);
            }
            return reports;
        }
    }
}
