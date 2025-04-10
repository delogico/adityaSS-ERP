using RMERP.DAL.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using RMERP.DAL.ViewModel;
using Microsoft.AspNetCore.Hosting;
using RMERP.DAL.Helpers;
using static RMERP.DAL.Helpers.ProjectUtils;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace RMERP.DAL.ManagerClasses
{
    public class ReportsManager
    {
        RMERPContext _context;
        public ReportsManager(RMERPContext context)
        {
            _context = context;
        }
        public List<Employee> GetActiveEmployeesOfMonth(int WAG_Id)
        {
            List<Employee> employees = new List<Employee>();
            employees = _context.Wage_Registers.Where(m => m.WAG_Id.Equals(WAG_Id)).Select(m => m.EMP).Distinct().ToList();
            return employees;
        }

        public List<PFReportVM> PFReports(int WAG_Id)
        {
            List<PFReportVM> reports = new List<PFReportVM>();
            List<Employee> employees = GetActiveEmployeesOfMonth(WAG_Id);
            foreach (var emp in employees)
            {
                WageRegisterManager registerManager = new WageRegisterManager(_context);
                List<Wage_Register> wage_Registers = registerManager.GetWageRegistersByEmpId(WAG_Id, emp.EMP_Id);
                PFReportVM PFreport = new PFReportVM();
                decimal GrossWages = 0M, EPFWages = 0M, EPSWages = 0M, EDLIWages = 0M, EPF_CONTRI_REMITTED = 0M, EPS_CONTRI_REMITTED = 0M, EPF_EPS_DIFF_REMITTED = 0M, REFUND_OF_ADVANCES = 0M;
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
                EPS_CONTRI_REMITTED = (GrossWages * Convert.ToDecimal(8.33)) / 100; // =ROUND(GrossWages*8.33%,0)
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

        public List<MLWF_ContributionVM> MLWF_ContributionReports(int WAG_Id)
        {
            List<MLWF_ContributionVM> reports = new List<MLWF_ContributionVM>();
            WageRegisterManager registerManager = new WageRegisterManager(_context);
            List<Wage_Register> wage_Registers = registerManager.GetWageRegistersByLWF(WAG_Id);
            var vv = wage_Registers.Select(m => new { m.CLI_Id, m.CLI.CLI_Name }).Distinct();

            foreach (var item in wage_Registers.Select(m => new { m.CLI_Id, m.CLI.CLI_Name }).Distinct())
            {
                MLWF_ContributionVM report = new MLWF_ContributionVM();
                report.CLI_Id = item.CLI_Id;
                report.CLI_Name = item.CLI_Name;
                List<Wage_Register> register = wage_Registers.Where(m => m.CLI_Id.Equals(item.CLI_Id)).ToList();
                int EMP_BELOW_3K = 0, EMP_ABOVE_3K = 0, EMPLOYER_BELOW_3K = 0, EMPLOYER_ABOVE_3K = 0;
                decimal EMP_DEDUCTION_BELOW = 0, EMP_DEDUCTION_ABOVE = 0, EMPLOYER_CONTR_BELOW = 0, EMPLOYER_CONTR_ABOVE = 0;


                if (register.Count() > 0)
                {
                    foreach (var emp in register)
                    {
                        if (emp.WAR_GrossTotal < emp.CRI.CRI_MLWF_Employee_Base)
                        {
                            EMP_BELOW_3K++;
                            EMP_DEDUCTION_BELOW = EMP_DEDUCTION_BELOW + (emp.WAR_LWF_Deduction_Employee != null ? emp.WAR_LWF_Deduction_Employee.Value : 0);
                        }
                        else if (emp.WAR_GrossTotal >= emp.CRI.CRI_MLWF_Employee_Base)
                        {
                            EMP_ABOVE_3K++;
                            EMP_DEDUCTION_ABOVE = EMP_DEDUCTION_ABOVE + (emp.WAR_LWF_Deduction_Employee != null ? emp.WAR_LWF_Deduction_Employee.Value : 0);
                        }
                        if (emp.WAR_GrossTotal < emp.CRI.CRI_MLWF_Employer_Base)
                        {
                            EMPLOYER_CONTR_BELOW = EMPLOYER_CONTR_BELOW + (emp.WAR_LWF_Deduction_Employer != null ? emp.WAR_LWF_Deduction_Employer.Value : 0);
                        }
                        else if (emp.WAR_GrossTotal >= emp.CRI.CRI_MLWF_Employer_Base)
                        {
                            EMPLOYER_CONTR_ABOVE = EMPLOYER_CONTR_ABOVE + (emp.WAR_LWF_Deduction_Employer != null ? emp.WAR_LWF_Deduction_Employer.Value : 0);
                        }

                    }

                    report.CRI_MLWF_Employee_GThen = (register[0].CRI.CRI_MLWF_Employee_GThen != null ? register[0].CRI.CRI_MLWF_Employee_GThen.Value : 0);
                    report.CRI_MLWF_Employee_LThen = (register[0].CRI.CRI_MLWF_Employee_LThen != null ? register[0].CRI.CRI_MLWF_Employee_LThen.Value : 0);
                    report.CRI_MLWF_Employer_GThen = (register[0].CRI.CRI_MLWF_Employer_GThen != null ? register[0].CRI.CRI_MLWF_Employer_GThen.Value : 0);
                    report.CRI_MLWF_Employer_LThen = (register[0].CRI.CRI_MLWF_Employer_LThen != null ? register[0].CRI.CRI_MLWF_Employer_LThen.Value : 0);

                    report.MLWF_Employee_Base = (register[0].CRI.CRI_MLWF_Employee_Base != null ? register[0].CRI.CRI_MLWF_Employee_Base.Value : 0);
                    report.MLWF_Employer_Base = (register[0].CRI.CRI_MLWF_Employer_Base != null ? register[0].CRI.CRI_MLWF_Employer_Base.Value : 0);

                    report.EMP_DEDUCTION_BELOW = EMP_DEDUCTION_BELOW;
                    report.EMPLOYER_CONTR_BELOW = EMPLOYER_CONTR_BELOW;
                    report.EMP_DEDUCTION_ABOVE = EMP_DEDUCTION_ABOVE;
                    report.EMPLOYER_CONTR_ABOVE = EMPLOYER_CONTR_ABOVE;

                }

                report.EMP_ABOVE_3K = EMP_ABOVE_3K;
                report.EMP_BELOW_3K = EMP_BELOW_3K;
                reports.Add(report);
            }
            return reports;
        }

        //------------------------------------------------- REPORTS

        #region PF

        public List<PFClientReportVM> Client_Wise_PF_Details_Excel(int WAG_Id, int[] CLI_Ids)
        {
            ClientsManager cliManager = new(_context);
            WageRegisterManager registerManager = new(_context);
            List<Client> PFClientList = cliManager.GetClientsByIds(CLI_Ids);

            List<PFClientReportVM> reportVMs = [];
            List<Wage_Register> wage_Register;
            foreach (var client in PFClientList)
            {
                PFClientReportVM pFClient = new();
                wage_Register = registerManager.GetWageRegisters_ClientsWiseReports(WAG_Id, client.CLI_Id);
                pFClient.COMPANY_NAME = client.CLI_Name;
                pFClient.STRENGTH = wage_Register.Select(m => m.EMP_Id).Count();
                decimal ApplicableSalary = 0M, EMPLOYEE_CONTRIBUTION = 0M, EMPLOYER_CONTRIBUTION = 0M;
                foreach (var item in wage_Register)
                {
                    decimal AppSalary = GetAmountBasedOnFormula_Report(
                        item.WAR_PF_Formula,
                        item.WAR_Basic_Calculated,
                        item.WAR_DA_Calculated,
                        item.WAR_HRA_Calculated, item.Wage_Register_Allowances.ToList(),
                        item.WAR_TotalWorkingDays,
                        item.WAR_TotalPaybleDays,
                        item.WAR_OverTime_Calculated,
                        (item.WAR_OutStation_Allowance_Calculated != null ? item.WAR_OutStation_Allowance_Calculated.Value : 0),
                        (item.WAR_Attendance_Allowance_Calculated != null ? item.WAR_Attendance_Allowance_Calculated.Value : 0),
                        (item.WAR_Nightshift_Allowance_Calculated != null ? item.WAR_Nightshift_Allowance_Calculated.Value : 0),
                        (item.WAR_Performance_Allowance_Calculated != null ? item.WAR_Performance_Allowance_Calculated.Value : 0),
                        (item.WAR_Allowance_Calculated_1 != null ? item.WAR_Allowance_Calculated_1.Value : 0),
                        (item.WAR_Allowance_Calculated_2 != null ? item.WAR_Allowance_Calculated_2.Value : 0),
                        (item.WAR_Allowance_Calculated_3 != null ? item.WAR_Allowance_Calculated_3.Value : 0),
                        (item.WAR_Allowance_Calculated_4 != null ? item.WAR_Allowance_Calculated_4.Value : 0),
                        (item.WAR_Allowance_Calculated_5 != null ? item.WAR_Allowance_Calculated_5.Value : 0),
                        (item.WAR_Allowance_Calculated_6 != null ? item.WAR_Allowance_Calculated_6.Value : 0),
                        (item.WAR_Allowance_Calculated_7 != null ? item.WAR_Allowance_Calculated_7.Value : 0),
                        (item.WAR_Allowance_Calculated_8 != null ? item.WAR_Allowance_Calculated_8.Value : 0),
                        (item.WAR_Allowance_Calculated_9 != null ? item.WAR_Allowance_Calculated_9.Value : 0),
                        (item.WAR_Allowance_Calculated_10 != null ? item.WAR_Allowance_Calculated_10.Value : 0));

                    EMPLOYEE_CONTRIBUTION = Math.Round(EMPLOYEE_CONTRIBUTION + (AppSalary * item.WAR_PF) / 100, MidpointRounding.AwayFromZero);
                    if (item.CRI.CRI_PF_Employer_Cont_Rate != null)
                        EMPLOYER_CONTRIBUTION = Math.Round(EMPLOYER_CONTRIBUTION + (AppSalary * Convert.ToDecimal(item.CRI.CRI_PF_Employer_Cont_Rate)) / 100, MidpointRounding.AwayFromZero);
                    ApplicableSalary = Math.Round(AppSalary + ApplicableSalary, MidpointRounding.AwayFromZero);
                }
                pFClient.PF_APPLICABLE_SALARY = Math.Round(ApplicableSalary, MidpointRounding.AwayFromZero);
                pFClient.EMPLOYEE_CONTRIBUTION = Math.Round(EMPLOYEE_CONTRIBUTION, MidpointRounding.AwayFromZero);
                pFClient.EMPLOYER_CONTRIBUTION = Math.Round(EMPLOYER_CONTRIBUTION, MidpointRounding.AwayFromZero);
                pFClient.TOTAL_CONTRIBUTION = Math.Round(EMPLOYEE_CONTRIBUTION + EMPLOYER_CONTRIBUTION, MidpointRounding.AwayFromZero);
                pFClient.REMARKS = "-";
                reportVMs.Add(pFClient);
            }
            return reportVMs;
        }

        public List<PFClientReportVM> Employees_Pending_For_Registration(int WAG_Id, int[] CLI_Ids)
        {
            WageRegisterManager wageManager = new(_context);
            List<PFClientReportVM> reportVMs = [];
            IQueryable<Wage_Register> wage_Registers = wageManager.GetWageRegister_EmployeesPendingForRegistration(WAG_Id, CLI_Ids);
            foreach (var item in wage_Registers)
            {
                PFClientReportVM pFClient = new();
                pFClient.COMPANY_NAME = item.CLI.CLI_Name;
                pFClient.EMP_FirstName = item.EMP.EMP_FirstName;
                pFClient.EMP_MiddleName = item.EMP.EMP_MiddleName;
                pFClient.EMP_SurName = item.EMP.EMP_SurName;
                pFClient.PENDING_REGISTRAION_SINCE = item.EMP.EMP_RegisteredOn.ToShortDateString();
                pFClient.REMARKS = item.EMP.EMP_UAN_Remark;
                reportVMs.Add(pFClient);
            }
            return reportVMs;
        }

        public List<PFClientReportVM> Client_Wise_PF_Above58_Excel(int WAG_Id, int[] CLI_Ids)
        {
            List<PFClientReportVM> reportVMs = [];
            WageRegisterManager wageManager = new(_context);
            DateOnly Above57YearDate = Helpers.ProjectUtils.DateTimeToDate(DateTime.Today.AddYears(-58));
            IQueryable<Wage_Register> wage_Registers = wageManager.GetWageRegister_EmployeesOnlyRegistered(WAG_Id, CLI_Ids);
            wage_Registers = wage_Registers.Where(m => m.EMP.EMP_DOB <= Above57YearDate);
            foreach (var item in wage_Registers)
            {
                decimal ApplicableSalary = Math.Round(GetAmountBasedOnFormula_Report(
                            item.WAR_PF_Formula,
                            item.WAR_Basic_Calculated,
                            item.WAR_DA_Calculated,
                            item.WAR_HRA_Calculated, item.Wage_Register_Allowances.ToList(),
                            item.WAR_TotalWorkingDays,
                            item.WAR_TotalPaybleDays,
                            item.WAR_OverTime_Calculated,
                            (item.WAR_OutStation_Allowance_Calculated != null ? item.WAR_OutStation_Allowance_Calculated.Value : 0),
                            (item.WAR_Attendance_Allowance_Calculated != null ? item.WAR_Attendance_Allowance_Calculated.Value : 0),
                            (item.WAR_Nightshift_Allowance_Calculated != null ? item.WAR_Nightshift_Allowance_Calculated.Value : 0),
                            (item.WAR_Performance_Allowance_Calculated != null ? item.WAR_Performance_Allowance_Calculated.Value : 0),
                            (item.WAR_Allowance_Calculated_1 != null ? item.WAR_Allowance_Calculated_1.Value : 0),
                             (item.WAR_Allowance_Calculated_2 != null ? item.WAR_Allowance_Calculated_2.Value : 0),
                              (item.WAR_Allowance_Calculated_3 != null ? item.WAR_Allowance_Calculated_3.Value : 0),
                               (item.WAR_Allowance_Calculated_4 != null ? item.WAR_Allowance_Calculated_4.Value : 0),
                                (item.WAR_Allowance_Calculated_5 != null ? item.WAR_Allowance_Calculated_5.Value : 0),
                                (item.WAR_Allowance_Calculated_6 != null ? item.WAR_Allowance_Calculated_6.Value : 0),
                        (item.WAR_Allowance_Calculated_7 != null ? item.WAR_Allowance_Calculated_7.Value : 0),
                        (item.WAR_Allowance_Calculated_8 != null ? item.WAR_Allowance_Calculated_8.Value : 0),
                        (item.WAR_Allowance_Calculated_9 != null ? item.WAR_Allowance_Calculated_9.Value : 0),
                        (item.WAR_Allowance_Calculated_10 != null ? item.WAR_Allowance_Calculated_10.Value : 0)), MidpointRounding.AwayFromZero);
                decimal EMPLOYEE_CONTRIBUTION = Math.Round((ApplicableSalary * item.WAR_PF) / 100, MidpointRounding.AwayFromZero);
                decimal EMPLOYER_CONTRIBUTION = 0M;
                if (item.CRI.CRI_PF_Employer_Cont_Rate != null)
                    EMPLOYER_CONTRIBUTION = Math.Round((ApplicableSalary * Convert.ToDecimal(item.CRI.CRI_PF_Employer_Cont_Rate)) / 100, MidpointRounding.AwayFromZero);
                PFClientReportVM pFClient = new PFClientReportVM();
                pFClient.COMPANY_NAME = item.CLI.CLI_Name;
                pFClient.EMP_FirstName = item.EMP.EMP_FirstName;
                pFClient.EMP_MiddleName = item.EMP.EMP_MiddleName;
                pFClient.EMP_SurName = item.EMP.EMP_SurName;
                pFClient.STRENGTH = 1;
                pFClient.PF_APPLICABLE_SALARY = Math.Round(ApplicableSalary, MidpointRounding.AwayFromZero);
                pFClient.EMPLOYEE_CONTRIBUTION = Math.Round(EMPLOYEE_CONTRIBUTION, MidpointRounding.AwayFromZero);
                pFClient.EMPLOYER_CONTRIBUTION = Math.Round(EMPLOYER_CONTRIBUTION, MidpointRounding.AwayFromZero);
                pFClient.TOTAL_CONTRIBUTION = Math.Round(EMPLOYEE_CONTRIBUTION + EMPLOYER_CONTRIBUTION);
                pFClient.REMARKS = "-";
                reportVMs.Add(pFClient);
            }
            return reportVMs;
        }

        public List<PFClientReportVM> Employees_PF_Excel(int WAG_Id, int[] CLI_Ids)
        {
            WageRegisterManager wageManager = new WageRegisterManager(_context);
            List<PFClientReportVM> reportVMs = new List<PFClientReportVM>();
            IEnumerable<Wage_Register> wage_Registers = wageManager.GetWageRegister_EmployeesOnlyRegistered(WAG_Id, CLI_Ids);
            foreach (var item in wage_Registers)
            {
                //List<Client_Requirement_Allowance> All = item.CRI.Client_Requirement_Allowances.ToList();
                decimal ApplicableSalary = GetAmountBasedOnFormula_Report(
                            item.WAR_PF_Formula,
                            item.WAR_Basic_Calculated,
                            item.WAR_DA_Calculated,
                            item.WAR_HRA_Calculated, item.Wage_Register_Allowances.ToList(),
                            item.WAR_TotalWorkingDays,
                            item.WAR_TotalPaybleDays,
                            item.WAR_OverTime_Calculated,
                            (item.WAR_OutStation_Allowance_Calculated != null ? item.WAR_OutStation_Allowance_Calculated.Value : 0),
                            (item.WAR_Attendance_Allowance_Calculated != null ? item.WAR_Attendance_Allowance_Calculated.Value : 0),
                            (item.WAR_Nightshift_Allowance_Calculated != null ? item.WAR_Nightshift_Allowance_Calculated.Value : 0),
                            (item.WAR_Performance_Allowance_Calculated != null ? item.WAR_Performance_Allowance_Calculated.Value : 0),
                            (item.WAR_Allowance_Calculated_1 != null ? item.WAR_Allowance_Calculated_1.Value : 0),
                            (item.WAR_Allowance_Calculated_2 != null ? item.WAR_Allowance_Calculated_2.Value : 0),
                            (item.WAR_Allowance_Calculated_3 != null ? item.WAR_Allowance_Calculated_3.Value : 0),
                            (item.WAR_Allowance_Calculated_4 != null ? item.WAR_Allowance_Calculated_4.Value : 0),
                            (item.WAR_Allowance_Calculated_5 != null ? item.WAR_Allowance_Calculated_5.Value : 0),
                            (item.WAR_Allowance_Calculated_6 != null ? item.WAR_Allowance_Calculated_6.Value : 0),
                            (item.WAR_Allowance_Calculated_7 != null ? item.WAR_Allowance_Calculated_7.Value : 0),
                            (item.WAR_Allowance_Calculated_8 != null ? item.WAR_Allowance_Calculated_8.Value : 0),
                            (item.WAR_Allowance_Calculated_9 != null ? item.WAR_Allowance_Calculated_9.Value : 0),
                            (item.WAR_Allowance_Calculated_10 != null ? item.WAR_Allowance_Calculated_10.Value : 0));

                decimal EPF_CONTRIBUTION = (ApplicableSalary * Convert.ToDecimal(item.CRI.CRI_PF_Percentage)) / 100;
                decimal EPS_CONTRIBUTION = (ApplicableSalary * Convert.ToDecimal(item.CRI.CRI_EPS_Rate)) / 100;

                PFClientReportVM pFClient = new PFClientReportVM();
                pFClient.EMP_Id = item.EMP_Id;
                pFClient.UAN_Number = item.EMP.EMP_UAN_Number;
                pFClient.EMP_FirstName = item.EMP.EMP_FirstName;
                pFClient.EMP_MiddleName = item.EMP.EMP_MiddleName;
                pFClient.EMP_SurName = item.EMP.EMP_SurName;
                pFClient.PF_APPLICABLE_SALARY = ApplicableSalary;
                pFClient.EPF_CONTRIBUTION = EPF_CONTRIBUTION;
                pFClient.EPS_CONTRIBUTION = EPS_CONTRIBUTION;
                if (ApplicableSalary > 0)
                    pFClient.NCP1 = 0;
                else
                    pFClient.NCP1 = DateTime.DaysInMonth(item.WAG.WAG_Month.Year, item.WAG.WAG_Month.Month);
                pFClient.NCP2 = 0;
                reportVMs.Add(pFClient);
            }
            return reportVMs;
        }

        #endregion

        #region ESIC
        public List<ESICReportVM> Client_Wise_ESIC(int WAG_Id, int[] CLI_Ids)
        {
            ClientsManager cliManager = new(_context);
            List<Client> PFClientList = cliManager.GetClientsByIds(CLI_Ids);

            WageRegisterManager wageManager = new(_context);

            List<ESICReportVM> reportVMs = [];
            List<Wage_Register> wage_Register;
            foreach (var client in PFClientList)
            {
                ESICReportVM reportVM = new();
                wage_Register = wageManager.GetWageRegisters_ClientsWiseReports(WAG_Id, client.CLI_Id);
                reportVM.NAME_OF_COMPANY = client.CLI_Name;
                reportVM.NO_OF_EMPLOYEE = wage_Register.Select(m => m.EMP_Id).Count();
                decimal TOTAL_WAGES = 0M, EMPLOYEES_CONTRIBUTION = 0M, EMPLOYERS_CONTRI = 0m;
                foreach (var item in wage_Register)
                {
                    decimal AppSalary = 0;

                    AppSalary = GetAmountBasedOnFormula_Report(
                   item.WAR_ESIC_Formula,
                   Math.Round(item.WAR_Basic_Calculated, MidpointRounding.AwayFromZero),
                   Math.Round(item.WAR_DA_Calculated, MidpointRounding.AwayFromZero),
                   Math.Round(item.WAR_HRA_Calculated, MidpointRounding.AwayFromZero),
                    item.Wage_Register_Allowances.ToList(),
                   item.WAR_TotalWorkingDays,
                   item.WAR_TotalPaybleDays,
                   Math.Round(item.WAR_OverTime_Calculated, MidpointRounding.AwayFromZero),
                   (item.WAR_OutStation_Allowance_Calculated != null ? Math.Round(item.WAR_OutStation_Allowance_Calculated.Value, MidpointRounding.AwayFromZero) : 0),
                   (item.WAR_Attendance_Allowance_Calculated != null ? Math.Round(item.WAR_Attendance_Allowance_Calculated.Value, MidpointRounding.AwayFromZero) : 0),
                   (item.WAR_Nightshift_Allowance_Calculated != null ? Math.Round(item.WAR_Nightshift_Allowance_Calculated.Value, MidpointRounding.AwayFromZero) : 0),
                   (item.WAR_Performance_Allowance_Calculated != null ? Math.Round(item.WAR_Performance_Allowance_Calculated.Value, MidpointRounding.AwayFromZero) : 0),
                   (item.WAR_Allowance_Calculated_1 != null ? item.WAR_Allowance_Calculated_1.Value : 0),
                        (item.WAR_Allowance_Calculated_2 != null ? item.WAR_Allowance_Calculated_2.Value : 0),
                         (item.WAR_Allowance_Calculated_3 != null ? item.WAR_Allowance_Calculated_3.Value : 0),
                          (item.WAR_Allowance_Calculated_4 != null ? item.WAR_Allowance_Calculated_4.Value : 0),
                           (item.WAR_Allowance_Calculated_5 != null ? item.WAR_Allowance_Calculated_5.Value : 0),
                           (item.WAR_Allowance_Calculated_6 != null ? item.WAR_Allowance_Calculated_6.Value : 0),
                   (item.WAR_Allowance_Calculated_7 != null ? item.WAR_Allowance_Calculated_7.Value : 0),
                   (item.WAR_Allowance_Calculated_8 != null ? item.WAR_Allowance_Calculated_8.Value : 0),
                   (item.WAR_Allowance_Calculated_9 != null ? item.WAR_Allowance_Calculated_9.Value : 0),
                   (item.WAR_Allowance_Calculated_10 != null ? item.WAR_Allowance_Calculated_10.Value : 0));


                    EMPLOYEES_CONTRIBUTION = EMPLOYEES_CONTRIBUTION + Math.Round(item.WAR_ESIC_Calculated, MidpointRounding.AwayFromZero);
                    if (item.CRI.CRI_ESIC_Employer_Cont_Rate != null)
                        EMPLOYERS_CONTRI = EMPLOYERS_CONTRI + (AppSalary * Convert.ToDecimal(item.CRI.CRI_ESIC_Employer_Cont_Rate) / 100);

                    TOTAL_WAGES = AppSalary + TOTAL_WAGES;
                }

                reportVM.TOTAL_WAGES = Math.Round(TOTAL_WAGES, MidpointRounding.AwayFromZero);
                reportVM.EMPLOYEES_CONTRIBUTION = Math.Round(EMPLOYEES_CONTRIBUTION, MidpointRounding.AwayFromZero);

                reportVM.EMPLOYERS_CONTRIBUTION = Math.Round(EMPLOYERS_CONTRI, MidpointRounding.AwayFromZero);
                reportVM.TOTAL_CONTRIBUTION = Math.Round(EMPLOYEES_CONTRIBUTION + EMPLOYERS_CONTRI, MidpointRounding.AwayFromZero);
                reportVMs.Add(reportVM);
            }
            return reportVMs;
        }

        public List<ESICReportEmpWiseVM> ESICReportEmpWise(int WAG_Id, int FRM_Id, int[] CLI_Ids, DateOnly WAG_Month)
        {
            ClientsManager cliManager = new(_context);
            List<Client> PFClientList = cliManager.GetClientsByIds(CLI_Ids);

            WageRegisterManager wageManager = new(_context);

            List<ESICReportEmpWiseVM> reports = [];
            List<Wage_Register> wage_Register;
            foreach (var client in PFClientList)
            {
                ESICReportEmpWiseVM ESICReport = new() { NAME_OF_COMPANY = client.CLI_Name };
                List<ESICReportVM> reportVMs = [];

                wage_Register = wageManager.GetWageRegisters_ClientsWiseReports(WAG_Id, client.CLI_Id);
                foreach (var register in wage_Register)
                {
                    ESICReportVM reportVM = new();

                    decimal TotalMonthlyWages = GetAmountBasedOnFormula_Report(
                        register.WAR_ESIC_Formula,
                        register.WAR_Basic_Calculated,
                        register.WAR_DA_Calculated,
                        register.WAR_HRA_Calculated, register.Wage_Register_Allowances.ToList(),
                        register.WAR_TotalWorkingDays,
                        register.WAR_TotalPaybleDays,
                        register.WAR_OverTime_Calculated,
                        (register.WAR_OutStation_Allowance_Calculated != null ? register.WAR_OutStation_Allowance_Calculated.Value : 0),
                        (register.WAR_Attendance_Allowance_Calculated != null ? register.WAR_Attendance_Allowance_Calculated.Value : 0),
                        (register.WAR_Nightshift_Allowance_Calculated != null ? register.WAR_Nightshift_Allowance_Calculated.Value : 0),
                        (register.WAR_Performance_Allowance_Calculated != null ? register.WAR_Performance_Allowance_Calculated.Value : 0),
                        (register.WAR_Allowance_Calculated_1 != null ? register.WAR_Allowance_Calculated_1.Value : 0),
                             (register.WAR_Allowance_Calculated_2 != null ? register.WAR_Allowance_Calculated_2.Value : 0),
                              (register.WAR_Allowance_Calculated_3 != null ? register.WAR_Allowance_Calculated_3.Value : 0),
                               (register.WAR_Allowance_Calculated_4 != null ? register.WAR_Allowance_Calculated_4.Value : 0),
                                (register.WAR_Allowance_Calculated_5 != null ? register.WAR_Allowance_Calculated_5.Value : 0),
                                (register.WAR_Allowance_Calculated_6 != null ? register.WAR_Allowance_Calculated_6.Value : 0),
                        (register.WAR_Allowance_Calculated_7 != null ? register.WAR_Allowance_Calculated_7.Value : 0),
                        (register.WAR_Allowance_Calculated_8 != null ? register.WAR_Allowance_Calculated_8.Value : 0),
                        (register.WAR_Allowance_Calculated_9 != null ? register.WAR_Allowance_Calculated_9.Value : 0),
                        (register.WAR_Allowance_Calculated_10 != null ? register.WAR_Allowance_Calculated_10.Value : 0));

                    reportVM.TotalMonthlyWages = Math.Round(TotalMonthlyWages, MidpointRounding.AwayFromZero);
                    double PayableDays = register.WAR_TotalPaybleDays;

                    if (register.WAR_TotalPaybleDays > 26)
                    {
                        int days = DateTime.DaysInMonth(WAG_Month.Year, WAG_Month.Month);
                        if (days == 30)
                        {
                            PayableDays = 26;
                        }
                        else if (days == 31)
                        {
                            PayableDays = 27;
                        }
                        else
                        {
                            PayableDays = 26;
                        }
                    }
                    reportVM.PayableDays = intRoundFigure(PayableDays);
                    reportVM.LastWorkingDay = "";
                    reportVM.EMP_FirstName = register.EMP.EMP_FirstName;
                    reportVM.EMP_MiddleName = register.EMP.EMP_MiddleName;
                    reportVM.EMP_SurName = register.EMP.EMP_SurName;
                    reportVM.IP_Number = register.EMP.EMP_ESIC_Number;
                    reportVMs.Add(reportVM);
                }
                ESICReport.ESICReportVMs = reportVMs;
                reports.Add(ESICReport);
            }

            #region employees
            IEnumerable<Employee> employees = GetLeftEmployeesOfPrevMonth(new DateTime(WAG_Month.Year, WAG_Month.Month, WAG_Month.Day), FRM_Id);
            ESICReportEmpWiseVM ESICReportLeft = new();
            List<ESICReportVM> reportVMLefts = [];
            foreach (var emp in employees)
            {
                ESICReportVM reportVM = new()
                {
                    TotalMonthlyWages = 0,
                    PayableDays = 0,
                    LastWorkingDay = emp.EMP_InactivatedOn.Value.ToShortDateString(),
                    EMP_FirstName = emp.EMP_FirstName,
                    EMP_MiddleName = emp.EMP_MiddleName,
                    EMP_SurName = emp.EMP_SurName,
                    IP_Number = emp.EMP_ESIC_Number,
                    ReasonCode = emp.EMP_ReasonCode != null ? emp.EMP_ReasonCode.ToString() : "-"
                };
                reportVMLefts.Add(reportVM);
            }
            ESICReportLeft.ESICReportVMs = reportVMLefts;
            reports.Add(ESICReportLeft);
            #endregion

            return reports;
        }

        public List<ESICReportVM> ESIC_Employees_Pending_For_Registration(int WAG_Id, int[] CLI_Ids)
        {
            WageRegisterManager wageManager = new(_context);
            List<ESICReportVM> reportVMs = [];
            IQueryable<Wage_Register> wage_Registers = wageManager.GetWageRegister_EmployeesPendingForRegistration(WAG_Id, CLI_Ids);
            foreach (var item in wage_Registers)
            {
                ESICReportVM ESICClient = new ESICReportVM();
                ESICClient.NAME_OF_COMPANY = item.CLI.CLI_Name;
                ESICClient.EMP_FirstName = item.EMP.EMP_FirstName;
                ESICClient.EMP_MiddleName = item.EMP.EMP_MiddleName;
                ESICClient.EMP_SurName = item.EMP.EMP_SurName;
                ESICClient.PENDING_REGISTRAION_SINCE = item.EMP.EMP_RegisteredOn.ToShortDateString();
                ESICClient.REMARKS = item.EMP.EMP_ESIC_Remark;
                reportVMs.Add(ESICClient);
            }
            return reportVMs;
        }

        public List<Employee> GetLeftEmployeesOfPrevMonth(DateTime Wag_Month, int FRM_Id)
        {
            DateTime date = Wag_Month.AddMonths(-1);
            DateTime StartDate = new DateTime(date.Year, date.Month, 1); //1-nov-18
            DateTime EndDate = new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1); //30-nov-18
            List<Employee> emp = _context.Employees.Where(m => m.EMP_InactivatedOn.Value.Date <= EndDate.Date && m.EMP_InactivatedOn >= StartDate && m.FRM_Id.Equals(FRM_Id)).ToList();
            return emp;
        }
        #endregion

        #region PAY TAX

        public List<PayTaxReportVM> PayTaxReports(int WAG_Id)
        {
            List<PayTaxReportVM> reports = [];
            WageRegisterManager registerManager = new(_context);
            var Allregisters = registerManager.GetRegisterFor_PayTaxReports(WAG_Id);
            foreach (var item in Allregisters.Select(m => new { m.CLI_Id, m.CLI.CLI_Name }).Distinct())
            {
                int UpTo7500 = 0, UpTo7500Ladies = 0, UpTo10000 = 0, UpTo10000Ladies = 0, Above10000 = 0, Above10000Ladies = 0;
                decimal AMOUNT = 0;

                PayTaxReportVM report = new() { CLI_Id = item.CLI_Id, CLI_Name = item.CLI_Name };
                IQueryable<Wage_Register> ClientRegister = Allregisters.Where(c => c.WAG_Id == WAG_Id && c.CLI_Id == item.CLI_Id);

                foreach (var emp in ClientRegister)
                {
                    emp.WAR_FinalTotal = emp.WAR_GrossTotal;

                    if (emp.EMP.EMP_Gender.Equals(true)) //Male
                    {
                        if (emp.WAR_FinalTotal >= emp.CRI.CRI_ProffTax_M_From_1 && emp.WAR_FinalTotal <= emp.CRI.CRI_ProffTax_M_To_1)
                        {
                            UpTo7500++;
                            AMOUNT = AMOUNT + emp.CRI.CRI_ProffTax_M_Amount_1;
                        }
                        else if (emp.WAR_FinalTotal >= emp.CRI.CRI_ProffTax_M_From_2 && emp.WAR_FinalTotal <= emp.CRI.CRI_ProffTax_M_To_2)
                        {
                            UpTo10000++;
                            AMOUNT = AMOUNT + emp.CRI.CRI_ProffTax_M_Amount_2;
                        }
                        else if (emp.WAR_FinalTotal >= emp.CRI.CRI_ProffTax_M_From_3 && emp.WAR_FinalTotal <= emp.CRI.CRI_ProffTax_M_To_3)
                        {
                            Above10000++;
                            AMOUNT = AMOUNT + emp.CRI.CRI_ProffTax_M_Amount_3;
                        }
                    }
                    else //Female
                    {
                        if (emp.WAR_FinalTotal >= emp.CRI.CRI_ProffTax_F_From_1 && emp.WAR_FinalTotal <= emp.CRI.CRI_ProffTax_F_To_1)
                        {
                            UpTo7500Ladies++;
                            AMOUNT = AMOUNT + emp.CRI.CRI_ProffTax_F_Amount_1;
                        }
                        else if (emp.WAR_FinalTotal >= emp.CRI.CRI_ProffTax_F_From_2 && emp.WAR_FinalTotal <= emp.CRI.CRI_ProffTax_F_To_2)
                        {
                            UpTo10000Ladies++;
                            AMOUNT = AMOUNT + emp.CRI.CRI_ProffTax_F_Amount_2;
                        }
                        else if (emp.WAR_FinalTotal >= emp.CRI.CRI_ProffTax_F_From_3 && emp.WAR_FinalTotal <= emp.CRI.CRI_ProffTax_F_To_3)
                        {
                            Above10000Ladies++;
                            AMOUNT = AMOUNT + emp.CRI.CRI_ProffTax_F_Amount_3;
                        }
                    }
                }

                report.UpTo7500 = UpTo7500;
                report.UpTo7500Ladies = UpTo7500Ladies;
                report.UpTo10000 = UpTo10000;
                report.UpTo10000Ladies = UpTo10000Ladies;
                report.Above10000 = Above10000;
                report.Above10000Ladies = Above10000Ladies;
                report.AMOUNT = AMOUNT;
                reports.Add(report);
            }
            return reports;
        }

        #endregion

        //-----------------------------

        #region BANK REPORTS

        public List<NEFT_BankReportVM> NEFT_BankReports(int WAG_Id, string WAG_Month, int[] CLI_Ids)
        {
            List<NEFT_BankReportVM> reports = [];
            ClientsManager cliManager = new(_context);
            WageRegisterManager registerManager = new(_context);
            List<Client> clients = cliManager.GetClientsByIds(CLI_Ids);
            foreach (var cli in clients)
            {
                NEFT_BankReportVM bankReportVM = new() { CLI_Id = cli.CLI_Id, CLI_Name = cli.CLI_Name };
                List<Wage_Register> register = registerManager.GetWageRegistersForBank(WAG_Id, cli.CLI_Id);
                List<NEFTBank_EMP_ReportVM> rptEmployees = [];
                foreach (var wage in register)
                {
                    NEFTBank_EMP_ReportVM rptEmployee = new()
                    {
                        EMP_FirstName = wage.EMP.EMP_FirstName,
                        EMP_MiddleName = wage.EMP.EMP_MiddleName,
                        EMP_SurName = wage.EMP.EMP_SurName,
                        EMP_Account_Number = wage.EMP.EMP_Account_Number ?? "-",
                        CURRENCY_CODE = "-",
                        PART_TRAN_TYPE = "-",
                        TRANSACTION_AMOUNT = wage.WAR_FinalTotal ?? 0,
                        SERVICE_OUTLET = wage.EMP.EMP_Bank_IFSC ?? "-",
                        TRANSACTION_PARTICULARS = "Salary " + WAG_Month
                    };
                    rptEmployees.Add(rptEmployee);
                }
                bankReportVM.NEFTBank_EMP_ReportVMs = rptEmployees;
                reports.Add(bankReportVM);
            }
            return reports;
        }

        public List<BankReportVM> IDBI_TO_IDBI_BankReports(int WAG_Id, string WAG_Month, int[] CLI_Ids)
        {
            List<BankReportVM> bankReportVMs = [];
            WageRegisterManager registerManager = new(_context);
            IEnumerable<Wage_Register> wage_Registers = registerManager.GetWageRegistersForIDBI_To_IDBI(WAG_Id, CLI_Ids);

            foreach (var item in wage_Registers)
            {
                BankReportVM bankReport = new()
                {
                    EMP_FirstName = item.EMP.EMP_FirstName,
                    EMP_MiddleName = item.EMP.EMP_MiddleName,
                    EMP_SurName = item.EMP.EMP_SurName,
                    EMP_ACCOUNT_NUMBER = item.EMP.EMP_Account_Number,
                    EMP_CURRENCY_CODE = "INR"
                };
                string IFSC_Code = item.EMP.EMP_Bank_IFSC;
                bankReport.EMP_SERVICE_OUTLET = !string.IsNullOrEmpty(IFSC_Code) ? IFSC_Code[^4..] : "";
                bankReport.EMP_PART_TRAN_TYPE = "C";
                bankReport.EMP_TRANSACTION_AMOUNT = (item.WAR_FinalTotal != null ? item.WAR_FinalTotal.Value : 0);
                bankReport.EMP_TRANSACTION_PARTICULARS = "Salary " + WAG_Month;
                bankReportVMs.Add(bankReport);
            }
            return bankReportVMs;
        }

        public List<BankReportVM> IDBI_TO_Other_BankReports(int WAG_Id, int[] CLI_Ids)
        {
            List<BankReportVM> bankReportVMs = [];
            WageRegisterManager registerManager = new(_context);
            IEnumerable<Wage_Register> wage_Registers = registerManager.GetWageRegistersForIDBI_To_Other(WAG_Id, CLI_Ids);
            foreach (var item in wage_Registers)
            {
                BankReportVM bankReport = new()
                {
                    EMP_TRANSACTION_AMOUNT = (item.WAR_FinalTotal != null ? item.WAR_FinalTotal.Value : 0),
                    ACCOUNT_SENDER_NUMBER = item.WAG.FRM.FRM_AccountNumber,
                    ACCOUNT_IFSC_CODE = item.EMP.EMP_Bank_IFSC,
                    ACCOUNT_RECEIVERS_NUMBER = item.EMP.EMP_Account_Number,
                    ACCOUNT_TYPE = "S/A",
                    EMP_FirstName = item.EMP.EMP_FirstName,
                    EMP_MiddleName = item.EMP.EMP_MiddleName,
                    EMP_SurName = item.EMP.EMP_SurName,
                    CLI_ADDRESS = item.CLI != null ? item.CLI.CLI_Place_Of_Supply : "",
                    MESSAGE = "SALARY",
                    ORIGINETOR = "RELIABLE"
                };
                bankReportVMs.Add(bankReport);
            }
            return bankReportVMs;
        }

        public List<BankReportVM> ChequeCash_BankReports(int WAG_Id, int[] CLI_Ids)
        {
            List<BankReportVM> bankReportVMs = new List<BankReportVM>();
            WageRegisterManager registerManager = new WageRegisterManager(_context);
            IEnumerable<Wage_Register> wage_Registers = registerManager.GetWageRegistersForChequeCash(WAG_Id, CLI_Ids);
            foreach (var item in wage_Registers)
            {
                BankReportVM bankReport = new()
                {
                    EMP_FirstName = item.EMP.EMP_FirstName,
                    EMP_MiddleName = item.EMP.EMP_MiddleName,
                    EMP_SurName = item.EMP.EMP_SurName,
                    EMP_ADDRESS = item.CLI.CLI_Invoicing_Location,
                    EMP_TRANSACTION_AMOUNT = (item.WAR_FinalTotal != null ? item.WAR_FinalTotal.Value : 0)
                };
                bankReportVMs.Add(bankReport);
            }
            return bankReportVMs;
        }

        #endregion

        #region Advance

        public List<BankReportVM> IDBI_TO_IDBI_AdvanceReports(int FRM_Id, DateOnly WAG_Month)
        {
            WageProcessManager wageProcessManager = new(_context);
            List<BankReportVM> advanceBankReportVMs = [];

            AdvanceWageRegisterManager advanceWageRegisterManager = new(_context);
            List<Employee_Advance> employee_Advances = advanceWageRegisterManager.IDBI_TO_IDBI_AdvanceRpt(new DateTime(WAG_Month.Year, WAG_Month.Month, WAG_Month.Day), FRM_Id);

            foreach (var item in employee_Advances)
            {
                BankReportVM bankReportVM = new();
                bankReportVM.EMP_FirstName = item.EMP.EMP_FirstName;
                bankReportVM.EMP_MiddleName = item.EMP.EMP_MiddleName;
                bankReportVM.EMP_SurName = item.EMP.EMP_SurName;
                bankReportVM.EMP_ACCOUNT_NUMBER = item.EMP.EMP_Account_Number;
                bankReportVM.EMP_CURRENCY_CODE = "INR";

                string IFSC_Code = item.EMP.EMP_Bank_IFSC;
                string IFSC = "";
                if (!string.IsNullOrEmpty(IFSC_Code))
                {
                    IFSC = IFSC_Code.Substring(IFSC_Code.Length - 4);
                }
                bankReportVM.EMP_SERVICE_OUTLET = IFSC;
                bankReportVM.EMP_PART_TRAN_TYPE = "C";
                bankReportVM.EMP_TRANSACTION_AMOUNT = item.ADV_Amount;

                bankReportVM.EMP_TRANSACTION_PARTICULARS = "Advance " + WAG_Month.ToString("MMMM") + "-" + WAG_Month.ToString("yyyy");
                advanceBankReportVMs.Add(bankReportVM);
            }
            return advanceBankReportVMs;
        }

        public List<BankReportVM> IDBI_TO_Other_AdvanceReports(int FRM_Id, DateOnly WAG_Month)
        {
            List<BankReportVM> advanceBankReportVMs = [];
            AdvanceWageRegisterManager advanceWageRegisterManager = new AdvanceWageRegisterManager(_context);
            List<Employee_Advance> employee_Advances = advanceWageRegisterManager.IDBI_TO_Other_AdvanceRpt(new DateTime(WAG_Month.Year, WAG_Month.Month, WAG_Month.Day), FRM_Id);
            foreach (var item in employee_Advances)
            {
                BankReportVM bankReportVM = new()
                {
                    EMP_TRANSACTION_AMOUNT = item.ADV_Amount,
                    ACCOUNT_SENDER_NUMBER = item.EMP.FRM?.FRM_AccountNumber,
                    ACCOUNT_IFSC_CODE = item.EMP.EMP_Bank_IFSC,
                    ACCOUNT_RECEIVERS_NUMBER = item.EMP.EMP_Account_Number,
                    ACCOUNT_TYPE = "S/A",

                    EMP_FirstName = item.EMP.EMP_FirstName,
                    EMP_MiddleName = item.EMP.EMP_MiddleName,
                    EMP_SurName = item.EMP.EMP_SurName,

                    EMP_ADDRESS = item.EMP.EMP_CityNavigation?.CITY_Name,
                    MESSAGE = "SALARY",
                    ORIGINETOR = "RELIABLE"
                };
                advanceBankReportVMs.Add(bankReportVM);
            }
            return advanceBankReportVMs;
        }

        public List<BankReportVM> ChequeCash_AdvancesReports(int FRM_Id, DateOnly WAG_Month)
        {
            List<BankReportVM> bankReportVMs = [];
            AdvanceWageRegisterManager advanceWageRegisterManager = new(_context);
            List<Employee_Advance> employee_Advances = advanceWageRegisterManager.CHEQUE_CASH_AdvanceRpt(new DateTime(WAG_Month.Year, WAG_Month.Month, WAG_Month.Day), FRM_Id);

            foreach (var item in employee_Advances)
            {
                BankReportVM bankReport = new()
                {
                    EMP_FirstName = item.EMP.EMP_FirstName,
                    EMP_MiddleName = item.EMP.EMP_MiddleName,
                    EMP_SurName = item.EMP.EMP_SurName,
                    EMP_ADDRESS = item.EMP.EMP_CityNavigation?.CITY_Name,
                    EMP_TRANSACTION_AMOUNT = item.ADV_Amount
                };
                bankReportVMs.Add(bankReport);
            }
            return bankReportVMs;
        }

        #endregion

        #region Payslip        
        public EmployeePaySlipVM GeneratePaySlip(int WAG_Id, int EMP_Id)
        {
            EmployeePaySlipVM paySlipVM = new EmployeePaySlipVM();
            WageRegisterManager registerManager = new WageRegisterManager(_context);
            List<Wage_Register> wage_Registers = registerManager.GetWageRegistersForSalarySlip(WAG_Id, EMP_Id);
            decimal WAR_Basic_Calculated = 0M, WAR_DA_Calculated = 0M, WAR_ESIC_Calculated = 0M, WAR_FinalTotal = 0M, WAR_GrossTotal = 0M, WAR_HRA_Calculated = 0M, WAR_PF_Calculated = 0M, WAR_ProffesionalTax_Calculated = 0M;
            decimal WAR_LWF_Deduction_Calculated = 0M, WAR_Advance_Amount = 0M, WAR_RevenueDeduction_Calculated = 0M, WAR_CanteenFacility_Calculation = 0M;
            decimal WAR_OverTime_Calculated = 0M, WAR_Outstation_Allowance_Calculated = 0M, WAR_Nightshift_Allowance_Calculated = 0M, WAR_Performance_Allowance_Calculated = 0M, WAR_Attendance_Allowance_Calculated = 0M;
            decimal WAR_Allowance_Calculated_1 = 0M, WAR_Allowance_Calculated_2 = 0M, WAR_Allowance_Calculated_3 = 0M, WAR_Allowance_Calculated_4 = 0M, WAR_Allowance_Calculated_5 = 0M, WAR_Allowance_Calculated_6 = 0M, WAR_Allowance_Calculated_7 = 0M, WAR_Allowance_Calculated_8 = 0M, WAR_Allowance_Calculated_9 = 0M, WAR_Allowance_Calculated_10 = 0M;
            double WAR_TotalPaybleDays = 0, WAR_TotalWorkingDays = 0, Max_TotalPaybleDays = 0;
            List<Wage_Register_Allowance> allowances = new List<Wage_Register_Allowance>();
            foreach (Wage_Register wage in wage_Registers)
            {
                WAR_Basic_Calculated = WAR_Basic_Calculated + wage.WAR_Basic_Calculated;
                WAR_DA_Calculated = WAR_DA_Calculated + wage.WAR_DA_Calculated;

                WAR_HRA_Calculated = WAR_HRA_Calculated + wage.WAR_HRA_Calculated;
                WAR_ESIC_Calculated = WAR_ESIC_Calculated + wage.WAR_ESIC_Calculated;
                WAR_PF_Calculated = WAR_PF_Calculated + wage.WAR_PF_Calculated;
                WAR_ProffesionalTax_Calculated = WAR_ProffesionalTax_Calculated + Convert.ToDecimal(wage.WAR_ProffesionalTax_Calculated);
                WAR_TotalPaybleDays = WAR_TotalPaybleDays + wage.WAR_TotalPaybleDays;
                WAR_TotalWorkingDays = WAR_TotalWorkingDays + wage.WAR_TotalWorkingDays;

                WAR_GrossTotal = WAR_GrossTotal + wage.WAR_GrossTotal;
                WAR_FinalTotal = WAR_FinalTotal + (wage.WAR_FinalTotal != null ? wage.WAR_FinalTotal.Value : 0); ;

                if (wage.WAR_TotalPaybleDays > Max_TotalPaybleDays)
                {
                    Max_TotalPaybleDays = wage.WAR_TotalPaybleDays;
                    paySlipVM.EMP_Location = wage.CLI.CLI_Invoicing_Location;
                    paySlipVM.EMP_Designation = wage.CRI.DES.DES_Title;
                    paySlipVM.EMP_Region = wage.CLI.CLI_Invoicing_City;
                }

                if (wage.WAR_LWF_Deduction_Employee != null)
                    WAR_LWF_Deduction_Calculated = WAR_LWF_Deduction_Calculated + wage.WAR_LWF_Deduction_Employee.Value;
                if (wage.WAR_Advance_Amount != null)
                    WAR_Advance_Amount = WAR_Advance_Amount + wage.WAR_Advance_Amount.Value;
                if (wage.WAR_RevenueDeduction_Calculated != null)
                    WAR_RevenueDeduction_Calculated = WAR_RevenueDeduction_Calculated + Convert.ToDecimal(wage.WAR_RevenueDeduction_Calculated);
                if (wage.WAR_CanteenFacility_Calculation != null && wage.WAR_CanteenFacility_Calculation != "-")
                    WAR_CanteenFacility_Calculation = WAR_CanteenFacility_Calculation + Convert.ToDecimal(wage.WAR_CanteenFacility_Calculation);

                WAR_OverTime_Calculated = WAR_OverTime_Calculated + wage.WAR_OverTime_Calculated;
                if (wage.WAR_OutStation_Allowance_Calculated != null)
                    WAR_Outstation_Allowance_Calculated = WAR_Outstation_Allowance_Calculated + wage.WAR_OutStation_Allowance_Calculated.Value;
                if (wage.WAR_Nightshift_Allowance_Calculated != null)
                    WAR_Nightshift_Allowance_Calculated = WAR_Nightshift_Allowance_Calculated + wage.WAR_Nightshift_Allowance_Calculated.Value;
                if (wage.WAR_Performance_Allowance_Calculated != null)
                    WAR_Performance_Allowance_Calculated = WAR_Performance_Allowance_Calculated + wage.WAR_Performance_Allowance_Calculated.Value;
                if (wage.WAR_Attendance_Allowance_Calculated != null)
                    WAR_Attendance_Allowance_Calculated = WAR_Attendance_Allowance_Calculated + wage.WAR_Attendance_Allowance_Calculated.Value;

                if (wage.WAR_Allowance_Calculated_1 != null)
                {
                    WAR_Allowance_Calculated_1 = WAR_Allowance_Calculated_1 + wage.WAR_Allowance_Calculated_1.Value;
                }
                if (wage.WAR_Allowance_Calculated_2 != null)
                {
                    WAR_Allowance_Calculated_2 = WAR_Allowance_Calculated_2 + wage.WAR_Allowance_Calculated_2.Value;
                }
                if (wage.WAR_Allowance_Calculated_3 != null)
                {
                    WAR_Allowance_Calculated_3 = WAR_Allowance_Calculated_3 + wage.WAR_Allowance_Calculated_3.Value;
                }
                if (wage.WAR_Allowance_Calculated_4 != null)
                {
                    WAR_Allowance_Calculated_4 = WAR_Allowance_Calculated_4 + wage.WAR_Allowance_Calculated_4.Value;
                }
                if (wage.WAR_Allowance_Calculated_5 != null)
                {
                    WAR_Allowance_Calculated_5 = WAR_Allowance_Calculated_5 + wage.WAR_Allowance_Calculated_5.Value;
                }
                if (wage.WAR_Allowance_Calculated_6 != null)
                {
                    WAR_Allowance_Calculated_6 = WAR_Allowance_Calculated_6 + wage.WAR_Allowance_Calculated_6.Value;
                }
                if (wage.WAR_Allowance_Calculated_7 != null)
                {
                    WAR_Allowance_Calculated_7 = WAR_Allowance_Calculated_7 + wage.WAR_Allowance_Calculated_7.Value;
                }
                if (wage.WAR_Allowance_Calculated_8 != null)
                {
                    WAR_Allowance_Calculated_8 = WAR_Allowance_Calculated_8 + wage.WAR_Allowance_Calculated_8.Value;
                }
                if (wage.WAR_Allowance_Calculated_9 != null)
                {
                    WAR_Allowance_Calculated_9 = WAR_Allowance_Calculated_9 + wage.WAR_Allowance_Calculated_9.Value;
                }
                if (wage.WAR_Allowance_Calculated_10 != null)
                {
                    WAR_Allowance_Calculated_10 = WAR_Allowance_Calculated_10 + wage.WAR_Allowance_Calculated_10.Value;
                }
                allowances.AddRange(wage.Wage_Register_Allowances);
            }

            if (WAR_Allowance_Calculated_1 > 0)
            {
                paySlipVM.CRI_Allowance_Name_1 = wage_Registers[0].CRI.CRI_Allowance_Name_1;
            }
            if (WAR_Allowance_Calculated_2 > 0)
            {
                paySlipVM.CRI_Allowance_Name_2 = wage_Registers[0].CRI.CRI_Allowance_Name_2;
            }
            if (WAR_Allowance_Calculated_3 > 0)
            {
                paySlipVM.CRI_Allowance_Name_3 = wage_Registers[0].CRI.CRI_Allowance_Name_3;
            }
            if (WAR_Allowance_Calculated_4 > 0)
            {
                paySlipVM.CRI_Allowance_Name_4 = wage_Registers[0].CRI.CRI_Allowance_Name_4;
            }
            if (WAR_Allowance_Calculated_5 > 0)
            {
                paySlipVM.CRI_Allowance_Name_5 = wage_Registers[0].CRI.CRI_Allowance_Name_5;
            }
            if (WAR_Allowance_Calculated_6 > 0)
            {
                paySlipVM.CRI_Allowance_Name_6 = wage_Registers[0].CRI.CRI_Allowance_Name_6;
            }
            if (WAR_Allowance_Calculated_7 > 0)
            {
                paySlipVM.CRI_Allowance_Name_7 = wage_Registers[0].CRI.CRI_Allowance_Name_7;
            }
            if (WAR_Allowance_Calculated_8 > 0)
            {
                paySlipVM.CRI_Allowance_Name_8 = wage_Registers[0].CRI.CRI_Allowance_Name_8;
            }
            if (WAR_Allowance_Calculated_9 > 0)
            {
                paySlipVM.CRI_Allowance_Name_9 = wage_Registers[0].CRI.CRI_Allowance_Name_9;
            }
            if (WAR_Allowance_Calculated_10 > 0)
            {
                paySlipVM.CRI_Allowance_Name_10 = wage_Registers[0].CRI.CRI_Allowance_Name_10;
            }

            paySlipVM.ArrearsDays = 0;
            paySlipVM.firm = wage_Registers[0].WAG.FRM;
            paySlipVM.EMP_Account_Number = wage_Registers[0].EMP.EMP_Account_Number;
            paySlipVM.EMP_Bank = wage_Registers[0].EMP.EMP_Bank;
            paySlipVM.EMP_Branch = wage_Registers[0].EMP.EMP_Branch;
            paySlipVM.EMP_DateOfJoining = new DateTime(wage_Registers[0].EMP.EMP_DateOfJoining.Year, wage_Registers[0].EMP.EMP_DateOfJoining.Month, wage_Registers[0].EMP.EMP_DateOfJoining.Day);
            paySlipVM.EMP_ESIC_Number = wage_Registers[0].EMP.EMP_ESIC_Number;
            paySlipVM.EMP_FirstName = wage_Registers[0].EMP.EMP_FirstName;
            paySlipVM.EMP_MiddleName = wage_Registers[0].EMP.EMP_MiddleName;
            paySlipVM.EMP_SurName = wage_Registers[0].EMP.EMP_SurName;
            paySlipVM.EMP_UAN_Number = wage_Registers[0].EMP.EMP_UAN_Number;
            paySlipVM.EMP_Gender = wage_Registers[0].EMP.EMP_Gender;
            paySlipVM.EMP_Id = wage_Registers[0].EMP.EMP_Id;
            paySlipVM.EMP_Pan_Number = wage_Registers[0].EMP.EMP_Pan_Number;
            paySlipVM.EMP_PF_Number = "";
            paySlipVM.WAG_Month = new DateTime(wage_Registers[0].WAG.WAG_Month.Year, wage_Registers[0].WAG.WAG_Month.Month, wage_Registers[0].WAG.WAG_Month.Day);
            paySlipVM.Wage_Register_Allowances = allowances;
            paySlipVM.WAR_Basic_Calculated = WAR_Basic_Calculated;
            paySlipVM.WAR_DA_Calculated = WAR_DA_Calculated;
            paySlipVM.WAR_ESIC_Calculated = WAR_ESIC_Calculated;
            paySlipVM.WAR_FinalTotal = WAR_FinalTotal;
            paySlipVM.WAR_GrossTotal = WAR_GrossTotal;
            paySlipVM.WAR_HRA_Calculated = WAR_HRA_Calculated;
            paySlipVM.WAR_PF_Calculated = WAR_PF_Calculated;
            paySlipVM.WAR_ProffesionalTax_Calculated = WAR_ProffesionalTax_Calculated;
            paySlipVM.WAR_TotalPaybleDays = WAR_TotalPaybleDays;
            paySlipVM.WAR_TotalWorkingDays = WAR_TotalWorkingDays;
            paySlipVM.WAR_LWF_Deduction_Calculated = WAR_LWF_Deduction_Calculated;
            paySlipVM.WAR_Advance_Amount = WAR_Advance_Amount;
            paySlipVM.WAR_RevenueDeduction_Calculated = WAR_RevenueDeduction_Calculated;
            paySlipVM.WAR_CanteenFacility_Calculation = WAR_CanteenFacility_Calculation;
            paySlipVM.WAR_OverTime_Calculated = WAR_OverTime_Calculated;

            paySlipVM.WAR_Outstation_Allowance_Calculated = WAR_Outstation_Allowance_Calculated;
            paySlipVM.WAR_Nightshift_Allowance_Calculated = WAR_Nightshift_Allowance_Calculated;
            paySlipVM.WAR_Performance_Allowance_Calculated = WAR_Performance_Allowance_Calculated;
            paySlipVM.WAR_Attendance_Allowance_Calculated = WAR_Attendance_Allowance_Calculated;

            paySlipVM.WAR_Allowance_Calculated_1 = WAR_Allowance_Calculated_1;
            paySlipVM.WAR_Allowance_Calculated_2 = WAR_Allowance_Calculated_2;
            paySlipVM.WAR_Allowance_Calculated_3 = WAR_Allowance_Calculated_3;
            paySlipVM.WAR_Allowance_Calculated_4 = WAR_Allowance_Calculated_4;
            paySlipVM.WAR_Allowance_Calculated_5 = WAR_Allowance_Calculated_5;
            paySlipVM.WAR_Allowance_Calculated_6 = WAR_Allowance_Calculated_5;
            paySlipVM.WAR_Allowance_Calculated_7 = WAR_Allowance_Calculated_6;
            paySlipVM.WAR_Allowance_Calculated_8 = WAR_Allowance_Calculated_7;
            paySlipVM.WAR_Allowance_Calculated_9 = WAR_Allowance_Calculated_8;
            paySlipVM.WAR_Allowance_Calculated_10 = WAR_Allowance_Calculated_10;

            decimal DeductTotal = WAR_PF_Calculated + WAR_ESIC_Calculated + WAR_ProffesionalTax_Calculated + WAR_LWF_Deduction_Calculated + WAR_Advance_Amount + WAR_RevenueDeduction_Calculated + WAR_CanteenFacility_Calculation;
            paySlipVM.DeductTotal = DeductTotal;

            return paySlipVM;
        }

        public Wage_PaySlip AddWagePaySlip(Wage_PaySlip wagePaySlip)
        {
            if (wagePaySlip.WPS_Id > 0)
            {
                _context.Wage_PaySlips.Update(wagePaySlip);
            }
            else
            {
                _context.Wage_PaySlips.Add(wagePaySlip);
            }
            _context.SaveChanges();
            return wagePaySlip;
        }

        public Wage_PaySlip GetPaySlip(int WPS_Id)
        {
            return _context.Wage_PaySlips.Find(WPS_Id);
        }

        public List<EmployeePaySlipVM> GeneratePaySlipForAll(int WAG_Id)
        {
            WageRegisterManager registerManager = new WageRegisterManager(_context);
            List<Employee> employees = registerManager.GetEmployeesForSalarySlip(WAG_Id);
            List<EmployeePaySlipVM> employeePaySlips = new List<EmployeePaySlipVM>();
            foreach (Employee employee in employees)
            {
                employeePaySlips.Add(GeneratePaySlip(WAG_Id, employee.EMP_Id));
            }
            return employeePaySlips;
        }

        public List<Wage_PaySlip> GetWagePaySlips(int WAG_Id, int CLI_Id)
        {
            int[] EMP_Ids = [.. _context.Wage_Registers.Where(m => m.CLI_Id.Equals(CLI_Id)).Include(m => m.EMP).Where(m => m.WAG_Id.Equals(WAG_Id)).Select(m => m.EMP_Id)];
            return [.. _context.Wage_PaySlips.Where(m => EMP_Ids.Contains(m.EMP_Id) && m.WAG_Id == WAG_Id)];
        }

        #endregion

    }
}
