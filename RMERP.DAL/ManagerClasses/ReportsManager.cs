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
            foreach (var item in wage_Registers.Select(m => new { m.CLI_Id, m.CLI_.CLI_Name }).Distinct())
            {
                MLWF_ContributionVM report = new MLWF_ContributionVM();
                report.CLI_Id = item.CLI_Id;
                report.CLI_Name = item.CLI_Name;
                List<Wage_Register> register = wage_Registers.Where(m => m.CLI_Id.Equals(item.CLI_Id)).ToList();
                int EMP_BELOW_3K = 0, EMP_ABOVE_3K = 0;
                foreach (var emp in register)
                {
                    if (emp.WAR_GrossTotal > 0 && emp.WAR_GrossTotal < 3000)
                    {
                        EMP_BELOW_3K++;
                    }
                    else if (emp.WAR_GrossTotal >= 3000)
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
                int UpTo7500 = 0, UpTo7500Ladies = 0, UpTo10000 = 0, UpTo10000Ladies = 0, Above10000 = 0, Above10000Ladies = 0;
                foreach (var emp in register)
                {
                    emp.WAR_FinalTotal = emp.WAR_GrossTotal;
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

        public List<BankReportVM> IDBI_TO_IDBI_AdvanceReports(int WAG_Id, int FRM_Id)
        {
            WageProcessManager wageProcessManager = new WageProcessManager(_context);
            Wage_Process wage_Process = wageProcessManager.getWageProcessById(WAG_Id);
            DateTime WAG_Month = wage_Process.WAG_Month;
            List<BankReportVM> advanceBankReportVMs = new List<BankReportVM>();
            List<Employee_Advance> employee_Advances = this.GetIDBI_TO_IDBIEmployeeAdvances(WAG_Month, FRM_Id);

            foreach (var item in employee_Advances)

            {
                BankReportVM bankReportVM = new BankReportVM();
                bankReportVM.EMP_FirstName = item.EMP_.EMP_FirstName;
                bankReportVM.EMP_MiddleName = item.EMP_.EMP_MiddleName;
                bankReportVM.EMP_SurName = item.EMP_.EMP_SurName;
                bankReportVM.EMP_ACCOUNT_NUMBER = item.EMP_.EMP_Account_Number;
                bankReportVM.EMP_CURRENCY_CODE = "INR";

                string IFSC_Code = item.EMP_.EMP_Bank_IFSC;
                string IFSC = IFSC_Code.Substring(IFSC_Code.Length - 3);
                int code;
                if (Int32.TryParse(IFSC, out code))
                {
                    code = Convert.ToInt32(IFSC_Code.Substring(IFSC_Code.Length - 3));
                    bankReportVM.EMP_SERVICE_OUTLET = code.ToString("D3");
                }
                else
                {
                    bankReportVM.EMP_SERVICE_OUTLET = "";
                }                
                bankReportVM.EMP_PART_TRAN_TYPE = "C";
                bankReportVM.EMP_TRANSACTION_AMOUNT = item.ADV_Amount;

                bankReportVM.EMP_TRANSACTION_PARTICULARS = "Advance " + WAG_Month.ToString("MMMM") + "-" + WAG_Month.ToString("yyyy");
                advanceBankReportVMs.Add(bankReportVM);
            }
            return advanceBankReportVMs;
        }

        public List<BankReportVM> IDBI_TO_Other_AdvanceReports(int WAG_Id, int FRM_Id)
        {
            WageProcessManager wageProcessManager = new WageProcessManager(_context);
            Wage_Process wage_Process = wageProcessManager.getWageProcessById(WAG_Id);
            DateTime WAG_Month = wage_Process.WAG_Month;
            List<BankReportVM> advanceBankReportVMs = new List<BankReportVM>();
            List<Employee_Advance> employee_Advances = this.GetIDBI_TO_OtherEmployeeAdvances(WAG_Month, FRM_Id);

            foreach (var item in employee_Advances)
            {
                BankReportVM bankReportVM = new BankReportVM();
                bankReportVM.EMP_TRANSACTION_AMOUNT = item.ADV_Amount;
                bankReportVM.ACCOUNT_SENDER_NUMBER = item.EMP_.FRM_?.FRM_AccountNumber;
                bankReportVM.ACCOUNT_IFSC_CODE = item.EMP_.EMP_Bank_IFSC;
                bankReportVM.ACCOUNT_RECEIVERS_NUMBER = item.EMP_.EMP_Account_Number;
                bankReportVM.ACCOUNT_TYPE = "S/A";

                bankReportVM.EMP_FirstName = item.EMP_.EMP_FirstName;
                bankReportVM.EMP_MiddleName = item.EMP_.EMP_MiddleName;
                bankReportVM.EMP_SurName = item.EMP_.EMP_SurName;

                bankReportVM.EMP_ADDRESS = item.EMP_.EMP_CityNavigation?.CITY_Name;
                bankReportVM.MESSAGE = "SALARY";
                bankReportVM.ORIGINETOR = "RELIABLE";
                advanceBankReportVMs.Add(bankReportVM);
            }
            return advanceBankReportVMs;
        }

        public List<BankReportVM> ChequeCash_AdvancesReports(int WAG_Id, int FRM_Id)
        {
            WageProcessManager wageProcessManager = new WageProcessManager(_context);
            Wage_Process wage_Process = wageProcessManager.getWageProcessById(WAG_Id);
            DateTime WAG_Month = wage_Process.WAG_Month;
            List<BankReportVM> bankReportVMs = new List<BankReportVM>();
            List<Employee_Advance> employee_Advances = this.GetCheque_CashEmployeeAdvances(WAG_Month, FRM_Id);

            foreach (var item in employee_Advances)
            {
                BankReportVM bankReport = new BankReportVM();

                bankReport.EMP_FirstName = item.EMP_.EMP_FirstName;
                bankReport.EMP_MiddleName = item.EMP_.EMP_MiddleName;
                bankReport.EMP_SurName = item.EMP_.EMP_SurName;
                bankReport.EMP_ADDRESS = item.EMP_.EMP_CityNavigation?.CITY_Name;
                bankReport.EMP_TRANSACTION_AMOUNT = item.ADV_Amount;
                bankReportVMs.Add(bankReport);
            }
            return bankReportVMs;
        }

        private List<Employee_Advance> GetIDBI_TO_IDBIEmployeeAdvances(DateTime WAG_Month, int FRM_Id)
        {
            AdvanceWageRegisterManager advanceWageRegisterManager = new AdvanceWageRegisterManager(_context);
            List<Employee_Advance> employee_Advances = advanceWageRegisterManager.IDBI_TO_IDBI_AdvanceRpt(WAG_Month, FRM_Id);
            return employee_Advances;
        }

        private List<Employee_Advance> GetIDBI_TO_OtherEmployeeAdvances(DateTime WAG_Month, int FRM_Id)
        {
            AdvanceWageRegisterManager advanceWageRegisterManager = new AdvanceWageRegisterManager(_context);
            List<Employee_Advance> employee_Advances = advanceWageRegisterManager.IDBI_TO_Other_AdvanceRpt(WAG_Month, FRM_Id);
            return employee_Advances;
        }

        private List<Employee_Advance> GetCheque_CashEmployeeAdvances(DateTime WAG_Month, int FRM_Id)
        {
            AdvanceWageRegisterManager advanceWageRegisterManager = new AdvanceWageRegisterManager(_context);
            List<Employee_Advance> employee_Advances = advanceWageRegisterManager.CHEQUE_CASH_AdvanceRpt(WAG_Month, FRM_Id);
            return employee_Advances;
        }

        public List<PFClientReportVM> Client_Wise_PF_Details_Excel(int WAG_Id, List<SelectionVM> selectionVMs, bool IsSelected)
        {
            WageRegisterManager wageManager = new WageRegisterManager(_context);
            List<Wage_Register> wage_Registers = wageManager.GetWageRegistersByWAG_Id(WAG_Id);
            dynamic PFClientList = null;
            if (IsSelected == true)
            {
                int[] CLI_Ids = selectionVMs.Select(m => m.CLI_Id).ToArray();
                var wage = from wageReg in wage_Registers
                           where CLI_Ids.Contains(wageReg.CLI_Id)
                           select wageReg;
                PFClientList = wage.Select(m => new
                {
                    m.CLI_Id,
                    m.CLI_.CLI_Name,
                }).Distinct();
            }
            else
            {
                PFClientList = wage_Registers.Select(m => new
                {
                    m.CLI_Id,
                    m.CLI_.CLI_Name,
                }).Distinct();
            }
            List<PFClientReportVM> reportVMs = new List<PFClientReportVM>();
            List<Wage_Register> wage_Register = new List<Wage_Register>();
            foreach (var client in PFClientList)
            {
                PFClientReportVM pFClient = new PFClientReportVM();
                wage_Register = wage_Registers.Where(m => m.CLI_Id.Equals(client.CLI_Id)).ToList();
                pFClient.COMPANY_NAME = client.CLI_Name;
                pFClient.STRENGTH = wage_Register.Select(m => m.EMP_Id).Count();
                decimal ApplicableSalary = 0M, EMPLOYEE_CONTRIBUTION = 0M, EMPLOYER_CONTRIBUTION = 0M;
                foreach (var item in wage_Register)
                {
                    List<Client_Requirement_Allowances> All = item.CRI_.Client_Requirement_Allowances.ToList();
                    decimal AppSalary = GetAmountBasedOnFormula(
                        item.WAR_PF_Formula,
                        item.WAR_Basic_Calculated,
                        item.WAR_DA_Calculated,
                        item.WAR_HRA_Calculated, All,
                        item.WAR_TotalWorkingDays,
                        item.WAR_TotalPaybleDays,
                        item.WAR_OverTime_Calculated,
                        (item.WAR_OutStation_Allowance_Calculated != null ? item.WAR_OutStation_Allowance_Calculated.Value : 0),
                        (item.WAR_Attendance_Allowance_Calculated != null ? item.WAR_Attendance_Allowance_Calculated.Value : 0),
                        (item.WAR_Nightshift_Allowance_Calculated != null ? item.WAR_Nightshift_Allowance_Calculated.Value : 0),
                        (item.WAR_Performance_Allowance_Calculated != null ? item.WAR_Performance_Allowance_Calculated.Value : 0));

                    EMPLOYEE_CONTRIBUTION = Math.Round(EMPLOYEE_CONTRIBUTION + (AppSalary * item.WAR_PF) / 100, MidpointRounding.AwayFromZero);
                    if(item.CLI_.CLI_PF_Employer_Cont_Rate!=null)
                        EMPLOYER_CONTRIBUTION = Math.Round(EMPLOYER_CONTRIBUTION + (AppSalary * Convert.ToDecimal(item.CLI_.CLI_PF_Employer_Cont_Rate)) / 100, MidpointRounding.AwayFromZero);
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

        public List<PFClientReportVM> Employees_Pending_For_Registration(int WAG_Id, List<SelectionVM> selectionVMs, bool IsSelected)
        {
            WageRegisterManager wageManager = new WageRegisterManager(_context);
            List<PFClientReportVM> reportVMs = new List<PFClientReportVM>();
            List<Wage_Register> wage_Registers = wageManager.GetWageRegisters(WAG_Id).Where(m => m.EMP_.EMP_UAN_Number == null || m.EMP_.EMP_UAN_Number.Equals("Pending") || m.EMP_.EMP_UAN_Number.Equals("")).ToList();
            dynamic PFClientList = null;
            if (IsSelected == true)
            {
                int[] CLI_Ids = selectionVMs.Select(m => m.CLI_Id).ToArray();
                var wage = from wageReg in wage_Registers
                           where CLI_Ids.Contains(wageReg.CLI_Id)
                           select wageReg;
                PFClientList = wage.Select(m => new
                {
                    m.CLI_Id,
                    m.CLI_.CLI_Name,
                    m.EMP_.EMP_FirstName,
                    m.EMP_.EMP_MiddleName,
                    m.EMP_.EMP_SurName,
                    m.EMP_.EMP_RegisteredOn,
                    m.EMP_.EMP_UAN_Remark
                }).Distinct();
            }
            else
            {
                PFClientList = wage_Registers.Select(m => new
                {
                    m.CLI_Id,
                    m.CLI_.CLI_Name,
                    m.EMP_.EMP_FirstName,
                    m.EMP_.EMP_MiddleName,
                    m.EMP_.EMP_SurName,
                    m.EMP_.EMP_RegisteredOn,
                    m.EMP_.EMP_UAN_Remark
                }).Distinct();
            }
            foreach (var item in PFClientList)
            {
                PFClientReportVM pFClient = new PFClientReportVM();
                pFClient.COMPANY_NAME = item.CLI_Name;
                pFClient.EMP_FirstName = item.EMP_FirstName;
                pFClient.EMP_MiddleName = item.EMP_MiddleName;
                pFClient.EMP_SurName = item.EMP_SurName;
                pFClient.PENDING_REGISTRAION_SINCE = item.EMP_RegisteredOn.ToShortDateString();
                pFClient.REMARKS = item.EMP_UAN_Remark;
                reportVMs.Add(pFClient);
            }
            return reportVMs;
        }

        public List<PFClientReportVM> Employees_PF_Excel(int WAG_Id, List<SelectionVM> selectionVMs, bool IsSelected)
        {
            WageRegisterManager wageManager = new WageRegisterManager(_context);
            List<PFClientReportVM> reportVMs = new List<PFClientReportVM>();
            IEnumerable<Wage_Register> wage_Registers = wageManager.GetWageRegistersByWAG_Id(WAG_Id).Where(m => m.EMP_.EMP_UAN_Number != null && m.EMP_.EMP_UAN_Number != "Pending" && m.EMP_.EMP_UAN_Number != "").ToList();

            if (IsSelected == true)
            {
                int[] CLI_Ids = selectionVMs.Select(m => m.CLI_Id).ToArray();
                wage_Registers = from wageReg in wage_Registers
                                 where CLI_Ids.Contains(wageReg.CLI_Id)
                                 select wageReg;
            }
            foreach (var item in wage_Registers)
            {
                List<Client_Requirement_Allowances> All = item.CRI_.Client_Requirement_Allowances.ToList();
                decimal ApplicableSalary = Math.Round(GetAmountBasedOnFormula(
                            item.WAR_PF_Formula,
                            item.WAR_Basic_Calculated,
                            item.WAR_DA_Calculated,
                            item.WAR_HRA_Calculated, All,
                            item.WAR_TotalWorkingDays,
                            item.WAR_TotalPaybleDays,
                            item.WAR_OverTime_Calculated,
                            (item.WAR_OutStation_Allowance_Calculated != null ? item.WAR_OutStation_Allowance_Calculated.Value : 0),
                            (item.WAR_Attendance_Allowance_Calculated != null ? item.WAR_Attendance_Allowance_Calculated.Value : 0),
                            (item.WAR_Nightshift_Allowance_Calculated != null ? item.WAR_Nightshift_Allowance_Calculated.Value : 0),
                            (item.WAR_Performance_Allowance_Calculated != null ? item.WAR_Performance_Allowance_Calculated.Value : 0)), MidpointRounding.AwayFromZero);
                decimal EPF_CONTRIBUTION = Math.Round((ApplicableSalary * Convert.ToDecimal(item.CLI_.CLI_EPF_Rate)) / 100, MidpointRounding.AwayFromZero);
                decimal EPS_CONTRIBUTION = Math.Round((ApplicableSalary * Convert.ToDecimal(item.CLI_.CLI_EPS_Rate)) / 100, MidpointRounding.AwayFromZero);
                PFClientReportVM pFClient = new PFClientReportVM();
                pFClient.EMP_Id = item.EMP_Id;
                pFClient.UAN_Number = item.EMP_.EMP_UAN_Number;
                pFClient.EMP_FirstName = item.EMP_.EMP_FirstName;
                pFClient.EMP_MiddleName = item.EMP_.EMP_MiddleName;
                pFClient.EMP_SurName = item.EMP_.EMP_SurName;
                pFClient.PF_APPLICABLE_SALARY = Math.Round(ApplicableSalary, MidpointRounding.AwayFromZero);
                pFClient.EPF_CONTRIBUTION = Math.Round(EPF_CONTRIBUTION, MidpointRounding.AwayFromZero);
                pFClient.EPS_CONTRIBUTION = Math.Round(EPS_CONTRIBUTION, MidpointRounding.AwayFromZero);
                pFClient.NCP1 = 0;
                pFClient.NCP2 = 0;
                reportVMs.Add(pFClient);
            }
            return reportVMs;
        }

        public List<PFClientReportVM> Client_Wise_PF_Above58_Excel(int WAG_Id, List<SelectionVM> selectionVMs, bool IsSelected)
        {
            WageRegisterManager wageManager = new WageRegisterManager(_context);
            DateTime Above57YearDate = DateTime.Today.AddYears(-58);
            IEnumerable<Wage_Register> wage_Registers = wageManager.GetWageRegistersByWAG_Id(WAG_Id).Where(m => m.EMP_.EMP_UAN_Number != null && m.EMP_.EMP_UAN_Number != "Pending" && m.EMP_.EMP_UAN_Number != "" && m.EMP_.EMP_DOB <= Above57YearDate).ToList();
            List<PFClientReportVM> reportVMs = new List<PFClientReportVM>();
            if (IsSelected == true)
            {
                int[] CLI_Ids = selectionVMs.Select(m => m.CLI_Id).ToArray();
                wage_Registers = from wageReg in wage_Registers
                                 where CLI_Ids.Contains(wageReg.CLI_Id)
                                 select wageReg;
            }
            foreach (var item in wage_Registers)
            {
                List<Client_Requirement_Allowances> All = item.CRI_.Client_Requirement_Allowances.ToList();
                decimal ApplicableSalary = Math.Round(GetAmountBasedOnFormula(
                            item.WAR_PF_Formula,
                            item.WAR_Basic_Calculated,
                            item.WAR_DA_Calculated,
                            item.WAR_HRA_Calculated, All,
                            item.WAR_TotalWorkingDays,
                            item.WAR_TotalPaybleDays,
                            item.WAR_OverTime_Calculated,
                            (item.WAR_OutStation_Allowance_Calculated != null ? item.WAR_OutStation_Allowance_Calculated.Value : 0),
                            (item.WAR_Attendance_Allowance_Calculated != null ? item.WAR_Attendance_Allowance_Calculated.Value : 0),
                            (item.WAR_Nightshift_Allowance_Calculated != null ? item.WAR_Nightshift_Allowance_Calculated.Value : 0),
                            (item.WAR_Performance_Allowance_Calculated != null ? item.WAR_Performance_Allowance_Calculated.Value : 0)), MidpointRounding.AwayFromZero);
                decimal EMPLOYEE_CONTRIBUTION = Math.Round((ApplicableSalary * item.WAR_PF) / 100, MidpointRounding.AwayFromZero);
                decimal EMPLOYER_CONTRIBUTION = 0M;
                if (item.CLI_.CLI_PF_Employer_Cont_Rate != null)
                    EMPLOYER_CONTRIBUTION = Math.Round((ApplicableSalary * Convert.ToDecimal(item.CLI_.CLI_PF_Employer_Cont_Rate)) / 100, MidpointRounding.AwayFromZero);
                PFClientReportVM pFClient = new PFClientReportVM();
                pFClient.COMPANY_NAME = item.CLI_.CLI_Name;
                pFClient.EMP_FirstName = item.EMP_.EMP_FirstName;
                pFClient.EMP_MiddleName = item.EMP_.EMP_MiddleName;
                pFClient.EMP_SurName = item.EMP_.EMP_SurName;
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

        public List<NEFT_BankReportVM> NEFT_BankReports(int WAG_Id, List<SelectionVM> selectionVMs, bool IsSelected)
        {
            List<NEFT_BankReportVM> reports = new List<NEFT_BankReportVM>();
            WageRegisterManager registerManager = new WageRegisterManager(_context);
            List<Wage_Register> wage_Registers = registerManager.GetWageRegistersByWAG_Id(WAG_Id);
            dynamic BankClientList = null;
            if (IsSelected == true)
            {
                int[] CLI_Ids = selectionVMs.Select(m => m.CLI_Id).ToArray();
                var wage = from wageReg in wage_Registers
                           where CLI_Ids.Contains(wageReg.CLI_Id)
                           select wageReg;
                BankClientList = wage.Select(m => new
                {
                    m.CLI_Id,
                    m.CLI_.CLI_Name,
                }).Distinct();
            }
            else
            {
                BankClientList = wage_Registers.Select(m => new
                {
                    m.CLI_Id,
                    m.CLI_.CLI_Name,
                }).Distinct();
            }
            foreach (var cli in BankClientList)
            {
                NEFT_BankReportVM bankReportVM = new NEFT_BankReportVM();
                bankReportVM.CLI_Id = cli.CLI_Id;
                bankReportVM.CLI_Name = cli.CLI_Name;
                List<Wage_Register> register = registerManager.GetWageRegisters(WAG_Id, cli.CLI_Id);
                List<NEFTBank_EMP_ReportVM> rptEmployees = new List<NEFTBank_EMP_ReportVM>();
                foreach (var wage in register)
                {
                    NEFTBank_EMP_ReportVM rptEmployee = new NEFTBank_EMP_ReportVM();
                    rptEmployee.EMP_FirstName = wage.EMP_.EMP_FirstName;
                    rptEmployee.EMP_MiddleName = wage.EMP_.EMP_MiddleName;
                    rptEmployee.EMP_SurName = wage.EMP_.EMP_SurName;
                    rptEmployee.EMP_Account_Number = "";
                    rptEmployee.CURRENCY_CODE = "-";
                    rptEmployee.PART_TRAN_TYPE = "-";
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

        public List<BankReportVM> IDBI_TO_IDBI_BankReports(int WAG_Id, List<SelectionVM> selectionVMs, bool IsSelected)
        {
            List<BankReportVM> bankReportVMs = new List<BankReportVM>();
            WageRegisterManager registerManager = new WageRegisterManager(_context);
            IEnumerable<Wage_Register> wage_Registers = registerManager.GetWageRegistersForIDBI_To_IDBI(WAG_Id);

            if (IsSelected == true)
            {
                int[] CLI_Ids = selectionVMs.Select(m => m.CLI_Id).ToArray();
                wage_Registers = from wageReg in wage_Registers
                                 where CLI_Ids.Contains(wageReg.CLI_Id)
                                 select wageReg;
            }
            foreach (var item in wage_Registers)
            {
                BankReportVM bankReport = new BankReportVM();
                bankReport.EMP_FirstName = item.EMP_.EMP_FirstName;
                bankReport.EMP_MiddleName = item.EMP_.EMP_MiddleName;
                bankReport.EMP_SurName = item.EMP_.EMP_SurName;
                bankReport.EMP_ACCOUNT_NUMBER = item.EMP_.EMP_Account_Number;
                bankReport.EMP_CURRENCY_CODE = "INR";
                string IFSC_Code = item.EMP_.EMP_Bank_IFSC;
                string IFSC = IFSC_Code.Substring(IFSC_Code.Length - 3);
                int code;
                if (Int32.TryParse(IFSC, out code))
                {
                    code = Convert.ToInt32(IFSC_Code.Substring(IFSC_Code.Length - 3));
                    bankReport.EMP_SERVICE_OUTLET = code.ToString("D3");
                }
                else
                {
                    bankReport.EMP_SERVICE_OUTLET = "";
                }                
                
                bankReport.EMP_PART_TRAN_TYPE = "C";
                bankReport.EMP_TRANSACTION_AMOUNT = item.WAR_FinalTotal;
                DateTime WAG_Month = wage_Registers.First().WAG_.WAG_Month;
                bankReport.EMP_TRANSACTION_PARTICULARS = "Salary " + WAG_Month.ToString("MMMM") + "-" + WAG_Month.ToString("yyyy");
                bankReportVMs.Add(bankReport);
            }
            return bankReportVMs;
        }

        public List<BankReportVM> IDBI_TO_Other_BankReports(int WAG_Id, List<SelectionVM> selectionVMs, bool IsSelected)
        {
            List<BankReportVM> bankReportVMs = new List<BankReportVM>();
            WageRegisterManager registerManager = new WageRegisterManager(_context);
            IEnumerable<Wage_Register> wage_Registers = registerManager.GetWageRegistersForIDBI_To_Other(WAG_Id);
            if (IsSelected == true)
            {
                int[] CLI_Ids = selectionVMs.Select(m => m.CLI_Id).ToArray();
                wage_Registers = from wageReg in wage_Registers
                                 where CLI_Ids.Contains(wageReg.CLI_Id)
                                 select wageReg;
            }
            foreach (var item in wage_Registers)
            {
                BankReportVM bankReport = new BankReportVM();
                bankReport.EMP_TRANSACTION_AMOUNT = item.WAR_FinalTotal;
                bankReport.ACCOUNT_SENDER_NUMBER = item.WAG_.FRM_.FRM_AccountNumber;
                bankReport.ACCOUNT_IFSC_CODE = item.EMP_.EMP_Bank_IFSC;
                bankReport.ACCOUNT_RECEIVERS_NUMBER = item.EMP_.EMP_Account_Number;
                bankReport.ACCOUNT_TYPE = "S/A";

                bankReport.EMP_FirstName = item.EMP_.EMP_FirstName;
                bankReport.EMP_MiddleName = item.EMP_.EMP_MiddleName;
                bankReport.EMP_SurName = item.EMP_.EMP_SurName;

                if(item.EMP_.EMP_CityNavigation!=null)
                    bankReport.EMP_ADDRESS = item.EMP_.EMP_CityNavigation?.CITY_Name;
                bankReport.MESSAGE = "SALARY";
                bankReport.ORIGINETOR = "RELIABLE";

                bankReportVMs.Add(bankReport);
            }
            return bankReportVMs;
        }

        public List<BankReportVM> ChequeCash_BankReports(int WAG_Id, List<SelectionVM> selectionVMs, bool IsSelected)
        {
            List<BankReportVM> bankReportVMs = new List<BankReportVM>();
            WageRegisterManager registerManager = new WageRegisterManager(_context);
            IEnumerable<Wage_Register> wage_Registers = registerManager.GetWageRegistersForChequeCash(WAG_Id);
            if (IsSelected == true)
            {
                int[] CLI_Ids = selectionVMs.Select(m => m.CLI_Id).ToArray();
                wage_Registers = from wageReg in wage_Registers
                                 where CLI_Ids.Contains(wageReg.CLI_Id)
                                 select wageReg;
            }
            foreach (var item in wage_Registers)
            {
                BankReportVM bankReport = new BankReportVM();

                bankReport.EMP_FirstName = item.EMP_.EMP_FirstName;
                bankReport.EMP_MiddleName = item.EMP_.EMP_MiddleName;
                bankReport.EMP_SurName = item.EMP_.EMP_SurName;
                bankReport.EMP_ADDRESS = item.EMP_.EMP_CityNavigation?.CITY_Name;
                bankReport.EMP_TRANSACTION_AMOUNT = item.WAR_FinalTotal;
                bankReportVMs.Add(bankReport);
            }
            return bankReportVMs;
        }

        #region ESIC
        public List<ESICReportVM> Client_Wise_ESIC(int WAG_Id, List<SelectionVM> selectionVMs, bool IsSelected)
        {
            WageRegisterManager wageManager = new WageRegisterManager(_context);
            List<Wage_Register> wage_Registers = wageManager.GetWageRegistersByWAG_Id(WAG_Id);
            dynamic ESICClientList = null;
            if (IsSelected == true)
            {
                int[] CLI_Ids = selectionVMs.Select(m => m.CLI_Id).ToArray();
                var wage = from wageReg in wage_Registers
                           where CLI_Ids.Contains(wageReg.CLI_Id)
                           select wageReg;
                ESICClientList = wage.Select(m => new
                {
                    m.CLI_Id,
                    m.CLI_.CLI_Name,
                    m.CLI_.CLI_ESIC_Employer_Cont_Rate
                }).Distinct();
            }
            else
            {
                ESICClientList = wage_Registers.Select(m => new
                {
                    m.CLI_Id,
                    m.CLI_.CLI_Name,
                    m.CLI_.CLI_ESIC_Employer_Cont_Rate
                }).Distinct();
            }

            List<ESICReportVM> reportVMs = new List<ESICReportVM>();
            List<Wage_Register> wage_Register = new List<Wage_Register>();
            foreach (var client in ESICClientList)
            {                
                ESICReportVM reportVM = new ESICReportVM();
                wage_Register = wage_Registers.Where(m => m.CLI_Id.Equals(client.CLI_Id)).ToList();
                reportVM.NAME_OF_COMPANY = client.CLI_Name;
                reportVM.NO_OF_EMPLOYEE = wage_Register.Select(m => m.EMP_Id).Count();
                decimal TOTAL_WAGES = 0M, EMPLOYEES_CONTRIBUTION = 0M;
                foreach (var item in wage_Register)
                {
                    List<Client_Requirement_Allowances> All = item.CRI_.Client_Requirement_Allowances.ToList();
                    decimal AppSalary = GetAmountBasedOnFormula(
                        item.WAR_ESIC_Formula,
                        item.WAR_Basic_Calculated,
                        item.WAR_DA_Calculated,
                        item.WAR_HRA_Calculated, All,
                        item.WAR_TotalWorkingDays,
                        item.WAR_TotalPaybleDays,
                        item.WAR_OverTime_Calculated,
                        (item.WAR_OutStation_Allowance_Calculated != null ? item.WAR_OutStation_Allowance_Calculated.Value : 0),
                        (item.WAR_Attendance_Allowance_Calculated != null ? item.WAR_Attendance_Allowance_Calculated.Value : 0),
                        (item.WAR_Nightshift_Allowance_Calculated != null ? item.WAR_Nightshift_Allowance_Calculated.Value : 0),
                        (item.WAR_Performance_Allowance_Calculated != null ? item.WAR_Performance_Allowance_Calculated.Value : 0));

                    EMPLOYEES_CONTRIBUTION = EMPLOYEES_CONTRIBUTION + item.WAR_ESIC_Calculated;
                    //  EMPLOYEES_CONTRIBUTION = EMPLOYEES_CONTRIBUTION + (AppSalary * item.WAR_ESIC) / 100;
                   // GROSS = GROSS+ item.WAR_GrossTotal;
                    TOTAL_WAGES = Math.Round(AppSalary, MidpointRounding.AwayFromZero) + TOTAL_WAGES;
                }
                reportVM.TOTAL_WAGES = Math.Round(TOTAL_WAGES, MidpointRounding.AwayFromZero);
                reportVM.EMPLOYEES_CONTRIBUTION = Math.Round(EMPLOYEES_CONTRIBUTION,MidpointRounding.AwayFromZero);                
                decimal EMPLOYERS_CONTRI = 0m;
                if (client.CLI_ESIC_Employer_Cont_Rate != null)
                    EMPLOYERS_CONTRI = TOTAL_WAGES * Convert.ToDecimal(client.CLI_ESIC_Employer_Cont_Rate) / 100;
                reportVM.EMPLOYERS_CONTRIBUTION = Math.Round(EMPLOYERS_CONTRI, MidpointRounding.AwayFromZero);
                reportVM.TOTAL_CONTRIBUTION = Math.Round(EMPLOYEES_CONTRIBUTION + EMPLOYERS_CONTRI, MidpointRounding.AwayFromZero);
                reportVMs.Add(reportVM);
            }
            return reportVMs;
        }

        public List<ESICReportEmpWiseVM> ESICReportEmpWise1(int WAG_Id, List<SelectionVM> selectionVMs, bool IsSelected)
        {
            WageRegisterManager wageManager = new WageRegisterManager(_context);
            List<Wage_Register> wage_Registers = wageManager.GetWageRegistersByWAG_Id(WAG_Id);
            dynamic ESICClientList = null;

            if (IsSelected == true)
            {
                int[] CLI_Ids = selectionVMs.Select(m => m.CLI_Id).ToArray();
                var wage = from wageReg in wage_Registers
                           where CLI_Ids.Contains(wageReg.CLI_Id)
                           select wageReg;
                ESICClientList = wage.Select(m => new
                {
                    m.CLI_Id,
                    m.CLI_.CLI_Name,
                }).Distinct();
            }
            else
            {
                ESICClientList = wage_Registers.Select(m => new
                {
                    m.CLI_Id,
                    m.CLI_.CLI_Name,
                }).Distinct();
            }

            List<ESICReportEmpWiseVM> reports = new List<ESICReportEmpWiseVM>();
            List<Wage_Register> wage_Register = new List<Wage_Register>();
            foreach (var client in ESICClientList)
            {
                wage_Register = wage_Registers.Where(m => m.CLI_Id.Equals(client.CLI_Id)).ToList();
                ESICReportEmpWiseVM ESICReport = new ESICReportEmpWiseVM();
                ESICReport.NAME_OF_COMPANY = client.CLI_Name;                          
                List<ESICReportVM> reportVMs = new List<ESICReportVM>();
                foreach (var register in wage_Register)
                {
                    ESICReportVM reportVM = new ESICReportVM();
                    List<Client_Requirement_Allowances> All = register.CRI_.Client_Requirement_Allowances.ToList();
                    decimal TotalMonthlyWages = GetAmountBasedOnFormula(
                        register.WAR_ESIC_Formula,
                        register.WAR_Basic_Calculated,
                        register.WAR_DA_Calculated,
                        register.WAR_HRA_Calculated, All,
                        register.WAR_TotalWorkingDays,
                        register.WAR_TotalPaybleDays,
                        register.WAR_OverTime_Calculated,
                        (register.WAR_OutStation_Allowance_Calculated != null ? register.WAR_OutStation_Allowance_Calculated.Value : 0),
                        (register.WAR_Attendance_Allowance_Calculated != null ? register.WAR_Attendance_Allowance_Calculated.Value : 0),
                        (register.WAR_Nightshift_Allowance_Calculated != null ? register.WAR_Nightshift_Allowance_Calculated.Value : 0),
                        (register.WAR_Performance_Allowance_Calculated != null ? register.WAR_Performance_Allowance_Calculated.Value : 0));

                    reportVM.TotalMonthlyWages = Math.Round(TotalMonthlyWages, MidpointRounding.AwayFromZero);
                    double PayableDays = register.WAR_TotalPaybleDays;
                    if (register.WAR_TotalPaybleDays > 27)
                    {
                        PayableDays = 27;
                    }
                    reportVM.PayableDays = intRoundFigure(PayableDays);
                    reportVM.LastWorkingDay = "";
                    reportVM.EMP_FirstName = register.EMP_.EMP_FirstName;
                    reportVM.EMP_MiddleName = register.EMP_.EMP_MiddleName;
                    reportVM.EMP_SurName = register.EMP_.EMP_SurName;
                    reportVM.IP_Number = register.EMP_.EMP_ESIC_Number;
                    reportVMs.Add(reportVM);
                }
                ESICReport.ESICReportVMs = reportVMs;
                reports.Add(ESICReport);
            }
            #region Left Employees
            //IEnumerable<Employees> employees = GetLeftEmployeesOfPrevMonth(wage_Registers[0].WAG_.WAG_Month);
            //List<Wage_Register> Prev_wageRegister = wageManager.GetWageRegistersByWAG_Id(WAG_Id);
            //int[] EMP_Ids = employees.Select(m => m.EMP_Id).ToArray();
            //dynamic LeftclientList = null;
            //var Prev_wageRegisters = from wageReg in Prev_wageRegister
            //                         where EMP_Ids.Contains(wageReg.EMP_Id)
            //                         select wageReg;
            //LeftclientList = Prev_wageRegisters.Select(m => new
            //{
            //    m.CLI_Id,
            //    m.CLI_.CLI_Name,
            //}).Distinct();
            //List<Wage_Register> Prev_wage_Register = new List<Wage_Register>();
            //foreach (var client in LeftclientList)
            //{
            //    Prev_wage_Register = Prev_wageRegister.Where(m => m.CLI_Id.Equals(client.CLI_Id)).ToList();
            //    ESICReportEmpWiseVM ESICReport = new ESICReportEmpWiseVM();
            //    ESICReport.NAME_OF_COMPANY = client.CLI_Name;
            //    List<ESICReportVM> reportVMs = new List<ESICReportVM>();
            //    foreach (var emp in Prev_wage_Register)
            //    {
            //        ESICReportVM reportVM = new ESICReportVM();
            //        reportVM.TotalMonthlyWages = 0;
            //        reportVM.PayableDays = 0;

            //        reportVM.LastWorkingDay = emp.EMP_.EMP_InactivatedOn.Value.ToShortDateString();
            //        reportVM.EMP_FirstName = emp.EMP_.EMP_FirstName;
            //        reportVM.EMP_MiddleName = emp.EMP_.EMP_MiddleName;
            //        reportVM.EMP_SurName = emp.EMP_.EMP_SurName;
            //        reportVM.IP_Number = emp.EMP_.EMP_ESIC_Number;
            //        reportVM.ReasonCode = emp.EMP_.EMP_ReasonCode != null ? emp.EMP_.EMP_ReasonCode.ToString() : "-";
            //        reportVMs.Add(reportVM);
            //    }
            //    ESICReport.ESICReportVMs = reportVMs;
            //    reports.Add(ESICReport);
            //}
            #endregion

            #region employees
            IEnumerable<Employees> employees = GetLeftEmployeesOfPrevMonth(wage_Registers[0].WAG_.WAG_Month);
            ESICReportEmpWiseVM ESICReportLeft = new ESICReportEmpWiseVM();
            List<ESICReportVM> reportVMLefts = new List<ESICReportVM>();
            foreach (var emp in employees)
            {
                ESICReportVM reportVM = new ESICReportVM();
                reportVM.TotalMonthlyWages = 0;
                reportVM.PayableDays = 0;
                reportVM.LastWorkingDay = emp.EMP_InactivatedOn.Value.ToShortDateString();
                reportVM.EMP_FirstName = emp.EMP_FirstName;
                reportVM.EMP_MiddleName = emp.EMP_MiddleName;
                reportVM.EMP_SurName = emp.EMP_SurName;
                reportVM.IP_Number = emp.EMP_ESIC_Number;
                reportVM.ReasonCode = emp.EMP_ReasonCode != null ? emp.EMP_ReasonCode.ToString() : "-";
                reportVMLefts.Add(reportVM);
            }
            ESICReportLeft.ESICReportVMs = reportVMLefts;
            reports.Add(ESICReportLeft);
            #endregion
            return reports;
        }

        public List<ESICReportVM> ESICReportEmpWise(int WAG_Id, List<SelectionVM> selectionVMs, bool IsSelected)
        {
            WageRegisterManager wageManager = new WageRegisterManager(_context);
            IEnumerable<Wage_Register> wage_Registers = wageManager.GetWageRegistersByWAG_Id(WAG_Id);          
            if (IsSelected == true)
            {
                int[] CLI_Ids = selectionVMs.Select(m => m.CLI_Id).ToArray();
                wage_Registers = from wageReg in wage_Registers
                                 where CLI_Ids.Contains(wageReg.CLI_Id)
                                 select wageReg;
            }
            List<ESICReportVM> ESICReportVMs = new List<ESICReportVM>();            
            foreach (Wage_Register register in wage_Registers)
            {                
                ESICReportVM reportVM = new ESICReportVM();
                List<Client_Requirement_Allowances> All = register.CRI_.Client_Requirement_Allowances.ToList();
                decimal TotalMonthlyWages = GetAmountBasedOnFormula(
                    register.WAR_ESIC_Formula,
                    register.WAR_Basic_Calculated,
                    register.WAR_DA_Calculated,
                    register.WAR_HRA_Calculated, All,
                    register.WAR_TotalWorkingDays,
                    register.WAR_TotalPaybleDays,
                    register.WAR_OverTime_Calculated,
                    (register.WAR_OutStation_Allowance_Calculated != null ? register.WAR_OutStation_Allowance_Calculated.Value : 0),
                    (register.WAR_Attendance_Allowance_Calculated != null ? register.WAR_Attendance_Allowance_Calculated.Value : 0),
                    (register.WAR_Nightshift_Allowance_Calculated != null ? register.WAR_Nightshift_Allowance_Calculated.Value : 0),
                    (register.WAR_Performance_Allowance_Calculated != null ? register.WAR_Performance_Allowance_Calculated.Value : 0));

                reportVM.TotalMonthlyWages = Math.Round(TotalMonthlyWages, MidpointRounding.AwayFromZero);
                double PayableDays = register.WAR_TotalPaybleDays;
                if (register.WAR_TotalPaybleDays > 27)
                {
                    PayableDays = 27;
                }
                reportVM.PayableDays = intRoundFigure(PayableDays);
                reportVM.LastWorkingDay = "";
                reportVM.EMP_FirstName = register.EMP_.EMP_FirstName;
                reportVM.EMP_MiddleName = register.EMP_.EMP_MiddleName;
                reportVM.EMP_SurName = register.EMP_.EMP_SurName;
                reportVM.IP_Number = register.EMP_.EMP_ESIC_Number;
                ESICReportVMs.Add(reportVM);
            }

            #region Left employees
            IEnumerable<Employees> employees = GetLeftEmployeesOfPrevMonth(wage_Registers.First().WAG_.WAG_Month);
           // ESICReportEmpWiseVM ESICReportLeft = new ESICReportEmpWiseVM();
           // List<ESICReportVM> reportVMLefts = new List<ESICReportVM>();
            foreach (var emp in employees)
            {
                ESICReportVM reportVM = new ESICReportVM();
                reportVM.TotalMonthlyWages = 0;
                reportVM.PayableDays = 0;
                reportVM.LastWorkingDay = emp.EMP_InactivatedOn.Value.ToShortDateString();
                reportVM.EMP_FirstName = emp.EMP_FirstName;
                reportVM.EMP_MiddleName = emp.EMP_MiddleName;
                reportVM.EMP_SurName = emp.EMP_SurName;
                reportVM.IP_Number = emp.EMP_ESIC_Number;
                reportVM.ReasonCode = emp.EMP_ReasonCode != null ? emp.EMP_ReasonCode.ToString() : "-";
                ESICReportVMs.Add(reportVM);
            }                     
            #endregion
            return ESICReportVMs;
        }

        public List<ESICReportVM> ESIC_Employees_Pending_For_Registration(int WAG_Id, List<SelectionVM> selectionVMs, bool IsSelected)
        {
            WageRegisterManager wageManager = new WageRegisterManager(_context);
            List<ESICReportVM> reportVMs = new List<ESICReportVM>();
            List<Wage_Register> wage_Registers = wageManager.GetWageRegisters(WAG_Id).Where(m => m.EMP_.EMP_ESIC_Number == null || m.EMP_.EMP_ESIC_Number.Equals("Pending") || m.EMP_.EMP_ESIC_Number.Equals("")).ToList();
            dynamic ESICClientList = null;
            if (IsSelected == true)
            {
                int[] CLI_Ids = selectionVMs.Select(m => m.CLI_Id).ToArray();
                var wage = from wageReg in wage_Registers
                           where CLI_Ids.Contains(wageReg.CLI_Id)
                           select wageReg;
                ESICClientList = wage.Select(m => new
                {
                    m.CLI_Id,
                    m.CLI_.CLI_Name,
                    m.EMP_.EMP_FirstName,
                    m.EMP_.EMP_MiddleName,
                    m.EMP_.EMP_SurName,
                    m.EMP_.EMP_RegisteredOn,
                    m.EMP_.EMP_ESIC_Remark
                }).Distinct();
            }
            else
            {
                ESICClientList = wage_Registers.Select(m => new
                {
                    m.CLI_Id,
                    m.CLI_.CLI_Name,
                    m.EMP_.EMP_FirstName,
                    m.EMP_.EMP_MiddleName,
                    m.EMP_.EMP_SurName,
                    m.EMP_.EMP_RegisteredOn,
                    m.EMP_.EMP_ESIC_Remark
                }).Distinct();
            }
            foreach (var item in ESICClientList)
            {
                ESICReportVM ESICClient = new ESICReportVM();
                ESICClient.NAME_OF_COMPANY = item.CLI_Name;
                ESICClient.EMP_FirstName = item.EMP_FirstName;
                ESICClient.EMP_MiddleName = item.EMP_MiddleName;
                ESICClient.EMP_SurName = item.EMP_SurName;
                ESICClient.PENDING_REGISTRAION_SINCE = item.EMP_RegisteredOn.ToShortDateString();
                ESICClient.REMARKS = item.EMP_ESIC_Remark;
                reportVMs.Add(ESICClient);
            }
            return reportVMs;
        }

        public List<Employees> GetLeftEmployeesOfPrevMonth(DateTime Wag_Month)
        {
            DateTime date = Wag_Month.AddMonths(-1);
            DateTime StartDate = new DateTime(date.Year, date.Month, 1); //1-march-18
            DateTime EndDate = new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1); //31-march-18
            List<Employees> emp = _context.Employees.Where(m => m.EMP_InactivatedOn.Value.Date <= EndDate.Date && m.EMP_InactivatedOn >= StartDate).ToList();
            return emp;
        }
        #endregion
    }
}
