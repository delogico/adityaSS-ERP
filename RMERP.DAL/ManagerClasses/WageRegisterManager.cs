using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
//using RMERP.DAL.App_Code;
using RMERP.DAL.Helpers;
using RMERP.DAL.Mappers;
using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using static RMERP.DAL.Helpers.ProjectUtils;

namespace RMERP.DAL.ManagerClasses
{
    public class WageRegisterManager
    {
        RMERPContext _context;
        public IConfiguration _configuration;
        public WageRegisterManager(RMERPContext context)
        {
            _context = context;
        }
        public WageRegisterManager(RMERPContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public List<Wage_Register> GetWageRegisters(int WAG_Id, int CLI_Id)
        {
            return _context.Wage_Register.Where(r => r.WAG_Id == WAG_Id && r.CLI_Id == CLI_Id).Include(m => m.CLI_).Include(m => m.EMP_).ThenInclude(m => m.Employee_Advance).Include(n => n.EMP_).ThenInclude(n => n.Wage_Register_Advances).Include(m => m.CRI_).ThenInclude(m => m.DES_).Include(n => n.Wage_Register_Allowances).ThenInclude(n => n.CRA_).ThenInclude(n => n.ALL_).ToList();
        }

        public List<Wage_Register> GetWageRegistersByWAG_Id(int WAG_Id)
        {
            return _context.Wage_Register.Where(r => r.WAG_Id == WAG_Id).Include(m => m.CLI_).Include(m => m.EMP_).ThenInclude(m => m.Employee_Advance).Include(n => n.EMP_).ThenInclude(n => n.Wage_Register_Advances).Include(m => m.CRI_).ThenInclude(m => m.DES_).ThenInclude(m => m.Client_Requirements).Include(n => n.Wage_Register_Allowances).ThenInclude(n => n.CRA_).ThenInclude(n => n.ALL_).ToList();
        }
        public List<Wage_Register> GetWageRegistersByLWF(int WAG_Id)
        {
            return _context.Wage_Register.Where(r => r.WAG_Id == WAG_Id && r.CRI_.DES_.DES_Exclude_LWF.Equals(false)).Include(m => m.CLI_).Include(m => m.EMP_).ThenInclude(m => m.Employee_Advance).Include(n => n.EMP_).ThenInclude(n => n.Wage_Register_Advances).Include(m => m.CRI_).ThenInclude(m => m.DES_).ThenInclude(m => m.Client_Requirements).Include(n => n.Wage_Register_Allowances).ThenInclude(n => n.CRA_).ThenInclude(n => n.ALL_).ToList();
        }
        public List<Wage_Register> GetWageRegisters(int WAG_Id)
        {
            List<Wage_Register> lst = _context.Wage_Register.Where(r => r.WAG_Id == WAG_Id).Include(m => m.WAG_).Include(m => m.CLI_).Include(m => m.EMP_).ToList();
            return lst;
        }
        public Wage_Register GetWageRegister(int WAR_Id)
        {
            return _context.Wage_Register.Include(m => m.CRI_).Include(m => m.WAG_).Include(m => m.CLI_).Include(m => m.EMP_).Where(r => r.WAR_Id == WAR_Id).FirstOrDefault();
        }
        public List<ClientWageRegisterVM> GenerateWageRegisterTable(int WAG_Id, int AdminID, int FRM_Id)
        {
            List<ClientWageRegisterVM> lst = new List<ClientWageRegisterVM>();
            ClientsManager clientsManager = new ClientsManager(_context, _configuration);
            WageProcessManager wageManager = new WageProcessManager(_context);
            Wage_Process wageProcess = wageManager.getWageProcessById(WAG_Id);
            List<Clients> lstCli = clientsManager.GetActiveClientOfMonthByFirmId(wageProcess.WAG_Month, FRM_Id);
            foreach (Clients client in lstCli)
            {
                ClientWageRegisterVM clientWageRegisterVM = new ClientWageRegisterVM();
                Wage_Process_Clients wage_Process_Clients = wageManager.GetWage_Process_Clients(WAG_Id, client.CLI_Id);
                if (wage_Process_Clients != null)
                    clientWageRegisterVM.wageProcessClientVM = WageProcessClientsMapper.mapMe(wage_Process_Clients);
                clientWageRegisterVM.client = ClientMapper.mapMe(client);
                clientWageRegisterVM.wageProcessVM = WageProcessMapper.mapMe(wageProcess);
                List<WageRegisterVM> lstRegister = new List<WageRegisterVM>();
                if (wage_Process_Clients != null)
                    lstRegister = WageRegisterMapper.mapWageRegisters(GetWageRegisters(WAG_Id, client.CLI_Id));
                else
                    lstRegister = GetWageRegisterCalculated(wageProcess, client.CLI_Id, AdminID);
                clientWageRegisterVM.wageRegisterVMs = lstRegister;
                lst.Add(clientWageRegisterVM);
            }
            return lst;
        }

        public List<WageRegisterVM> GetWageRegisterCalculated(Wage_Process wageProcess, int CLI_Id, int AdminID)
        {

            List<WageRegisterVM> lstRegister = new List<WageRegisterVM>();
            ClientsManager clientsManager = new ClientsManager(_context, _configuration);
            AttendanceManager attManager = new AttendanceManager(_context);
            CanteenManager canteenManager = new CanteenManager(_context);
            OutstationManager outstationManager = new OutstationManager(_context);
            PerformanceManager performanceManager = new PerformanceManager(_context);
            IEnumerable<Clients_Employees> clientsEmployees = clientsManager.listActiveClientsEmployees(CLI_Id, wageProcess.WAG_Month);
            foreach (Clients_Employees employee in clientsEmployees)
            {
                IEnumerable<Attendance> attendances = attManager.getAttendance_Wage_Client_Employee_Designation(wageProcess.WAG_Id, CLI_Id, employee.EMP_Id, employee.DES_.DES_Id);
                if (attendances.Count() > 0)
                {
                    Client_Requirements cr = clientsManager.getActiveClientRequirement(CLI_Id, employee.DES_.DES_Id);
                    int totalWorkingDays = 0;
                    double totalPaybleDays = 0;
                    decimal CRI_Basic = 0M, WAR_Basic_Calculated = 0M, BasicDa = 0M, WAR_OverTime_Calculated = 0M, WAR_PF, WAR_PF_Calculated = 0M, WAR_ESIC = 0M, WAR_ESIC_Calculated = 0M, WAR_LWF_Deduction_Calculated = 0M;
                    decimal CRI_DA = 0M, CRI_DA_Calculated = 0M, CRI_HRA = 0M, CRI_HRA_Calculated = 0M;
                    decimal WAR_Attendance_Allowance_Calculated = 0M, WAR_Outstation_Allowance_Calculated = 0M, WAR_Performance_Allowance_Calculated = 0M, WAR_Nightshift_Allowance_Calculated = 0M;

                    #region Total working day calculation
                    Clients cli = clientsManager.GetClientById(CLI_Id);
                    int days = attendances.Count();
                    switch (cli.CLI_Total_WorkingDays)
                    {
                        case 0: //  0:Consider_RealDays
                            totalWorkingDays = days;
                            break;
                        case 1://   1:Excluding_WeeklyOff
                            totalWorkingDays = attendances.Where(a => a.ATT_IsWeeklyOff == false).Count();
                            break;
                        case 2://   2:Consider StaticDays 
                            totalWorkingDays = cli.CLI_No_Reduce_Days.Value;
                            break;
                        default: break;
                    }
                    #endregion

                    #region Paybale day counting

                    int totWeekOffs = 0, totalPublicHoliday = 0, totEarnLeave = 0, totNightShift = 0, totAbsent = 0;
                    double totPresentDays = 0, totHalfDays = 0, totExtraWorkingDays = 0, WAR_ExtraWorkingHours = 0, totNighthours = 0;

                    totPresentDays = attendances.Where(a => a.ATT_IsPresent == true).Count();
                    totEarnLeave = attendances.Where(a => a.ATT_IsEarnLeave == true).Count();
                    totNightShift = attendances.Where(a => a.ATT_NightShift == true).Count();
                    totHalfDays = attendances.Where(a => a.ATT_IsHalfday == true).Count();

                    WAR_ExtraWorkingHours = attendances.Sum(m => m.ATT_ExtraHoursWorked);
                    totExtraWorkingDays = WAR_ExtraWorkingHours / cli.CLI_WorkingHours_In_Day;

                    totWeekOffs = attendances.Where(a => a.ATT_IsWeeklyOff == true).Count();
                    totalPublicHoliday = attendances.Where(a => a.ATT_IsPublicHoliday == true).Count();
                    totNighthours = totNightShift * cli.CLI_WorkingHours_In_Day;

                    totalPaybleDays = (totPresentDays - totHalfDays) + (totHalfDays / 2) + totEarnLeave;

                    if (cr.CRI_IsPayable_WeeklyOff)
                    {
                        totalPaybleDays = totalPaybleDays + totWeekOffs;
                    }
                    if (cr.CRI_IsPayable_PublicHoliday)
                    {
                        totalPaybleDays = totalPaybleDays + totalPublicHoliday;
                    }
                    if (cr.CRI_OT_Calculate_Payableday)
                    {
                        totalPaybleDays = totalPaybleDays + totExtraWorkingDays;
                    }
                    totAbsent = attendances.Where(m => m.ATT_IsPresent.Equals(false) && m.ATT_IsEarnLeave.Equals(false) && m.ATT_IsPublicHoliday.Equals(false) && m.ATT_IsWeeklyOff.Equals(false)).Count();
                    #endregion

                    WageRegisterVM wageRegisterVM = new WageRegisterVM();
                    wageRegisterVM.CLE_Id = employee.CLE_Id;
                    wageRegisterVM.WAG_Id = wageProcess.WAG_Id;
                    wageRegisterVM.CLI_Id = CLI_Id;
                    wageRegisterVM.EMP_Id = employee.EMP_Id;
                    wageRegisterVM.wageProcessVM = WageProcessMapper.mapMe(wageProcess);
                    wageRegisterVM.employeeVM = EmployeesMapper.MapMe(employee.EMP_);
                    wageRegisterVM.designation = employee.DES_;

                    #region EMI Calculation

                    decimal totalEMI = 0;
                    if (employee.EMP_Id == 134)
                    {

                    }
                    if (employee.EMP_.Wage_Register_Advances.Count() > 0)
                    {
                        bool flag = false;
                        int clients = attManager.getAttendance_Wage_Employee(wageProcess.WAG_Id, employee.EMP_Id).Select(m => m.CLI_Id).Distinct().Count();
                        if (clients > 1)
                        {
                            IEnumerable<Clients_Employees> clientList = clientsManager.listActiveClientsEmployees_clients(employee.EMP_Id, wageProcess.WAG_Month);
                            Client_Requirements requirementMax = null;
                            foreach (Clients_Employees clients_Employee in clientList)
                            {
                                Client_Requirements requirement = clientsManager.getActiveClientRequirement(clients_Employee.CLI_Id, clients_Employee.DES_Id);
                                if (requirementMax == null)
                                    requirementMax = requirement;
                                else
                                {
                                    if (requirementMax.CRI_Basic < requirement.CRI_Basic)
                                        requirementMax = requirement;
                                }
                            }
                            if (requirementMax.CLI_Id.Equals(CLI_Id))
                                flag = true;
                        }
                        else
                        {
                            flag = true;
                        }
                        if (flag)
                        {
                            var EMI = employee.EMP_.Wage_Register_Advances.Where(m => m.WAD_Status == false).GroupBy(m => m.EMP_Id).Select(g => new
                            {
                                WAD_Amt = g.Sum(n => n.WAD_Amount)
                            });
                            totalEMI = Math.Round(EMI.Select(g => g.WAD_Amt).FirstOrDefault(), MidpointRounding.AwayFromZero);
                        }
                        


                    }
                    wageRegisterVM.WAR_Advance_Amount = totalEMI;
                    #endregion                  

                    #region New Allowances                    
                    if (cr.CRI_Attendance_Allowance)
                    {
                        if (totAbsent <= cr.CRI_Attendance_Allowance_MaximumDays)
                        {
                            WAR_Attendance_Allowance_Calculated = cr.CRI_Attendance_Allowance_Rate.Value;
                        }
                    }
                    if (cr.CRI_OutStation_Allowance)
                    {
                        Wage_Register_Outstation outstation = new Wage_Register_Outstation();
                        outstation = outstationManager.GetOutstationByCLE(wageProcess.WAG_Id, employee.CLE_Id, employee.CLI_Id);
                        if (outstation != null)
                        {
                            WAR_Outstation_Allowance_Calculated = Convert.ToDecimal(outstation.WRO_Hours * Convert.ToDouble(cr.CRI_OutStation_Allowance_Rate.Value));
                            wageRegisterVM.WRO_Id = outstation.WRO_Id;
                            wageRegisterVM.WAR_OutStation_Allowance_Calculated = WAR_Outstation_Allowance_Calculated;
                        }
                        else
                        {
                            wageRegisterVM.WAR_CanteenFacility_Calculation = "-";
                        }
                    }
                    if (cr.CRI_Nightshift_Allowance)
                    {
                        WAR_Nightshift_Allowance_Calculated = Convert.ToDecimal(Convert.ToDouble(cr.CRI_Nightshift_Allowance_Rate.Value) * totNighthours);
                        wageRegisterVM.WAR_Nightshift_Allowance_Calculated = WAR_Nightshift_Allowance_Calculated;
                    }
                    if (cr.CRI_Performance_Allowance)
                    {
                        Wage_Register_Performance performance = new Wage_Register_Performance();
                        performance = performanceManager.GetPerformanceByCLE(wageProcess.WAG_Id, employee.CLE_Id, employee.CLI_Id);
                        if (performance != null)
                        {
                            WAR_Performance_Allowance_Calculated = performance.WRP_Amount;
                            wageRegisterVM.WRP_Id = performance.WRP_Id;
                            wageRegisterVM.WAR_Performance_Allowance_Calculated = WAR_Performance_Allowance_Calculated;
                        }
                        else
                        {
                            wageRegisterVM.WAR_Performance_Allowance_Calculated = 0;
                        }
                    }
                    #endregion

                    wageRegisterVM.clientRequirementVM = ClientRequirementMapper.mapMe(clientsManager.getActiveClientRequirement(CLI_Id, employee.DES_Id));
                    wageRegisterVM.WAR_TotalWorkingDays = totalWorkingDays;
                    wageRegisterVM.WAR_TotalPaybleDays = totalPaybleDays;
                    wageRegisterVM.WAR_ExtraWorkingHours = WAR_ExtraWorkingHours;

                    CRI_Basic = Math.Round(cr.CRI_Basic.Value, MidpointRounding.AwayFromZero);
                    CRI_DA = Math.Round(Convert.ToDecimal(cr.CRI_DA), MidpointRounding.AwayFromZero);
                    BasicDa = Math.Round(Decimal.Add(CRI_Basic, Convert.ToDecimal(cr.CRI_DA)), MidpointRounding.AwayFromZero);

                    CRI_HRA = Math.Round(cr.CRI_HRA_Fixed == null ? Convert.ToDecimal(cr.CRI_HRA_Percentage.Value) : cr.CRI_HRA_Fixed.Value, MidpointRounding.AwayFromZero);

                    wageRegisterVM.WAR_Basic = CRI_Basic;
                    wageRegisterVM.WAR_HRA = CRI_HRA;
                    wageRegisterVM.WAR_LastModifiedOn = ProjectUtils.DateNow();

                    if (totalWorkingDays != 0)
                    {
                        double OvertimeInDay = WAR_ExtraWorkingHours / Convert.ToDouble(cli.CLI_WorkingHours_In_Day);

                        CRI_DA_Calculated = Math.Round(Math.Round(CRI_DA * Convert.ToDecimal(totalPaybleDays), MidpointRounding.AwayFromZero) / totalWorkingDays, MidpointRounding.AwayFromZero);
                        WAR_Basic_Calculated = Math.Round(Math.Round((Decimal.Multiply(CRI_Basic, Convert.ToDecimal(totalPaybleDays))) / totalWorkingDays, MidpointRounding.AwayFromZero), MidpointRounding.AwayFromZero);
                        decimal CalculatedBasicDa = Math.Round(Decimal.Add(WAR_Basic_Calculated, Convert.ToDecimal(CRI_DA_Calculated)), MidpointRounding.AwayFromZero);
                        CRI_HRA_Calculated = (cr.CRI_HRA_Fixed == null ? Math.Round((Math.Round(CalculatedBasicDa * Convert.ToDecimal(cr.CRI_HRA_Percentage))) / 100, MidpointRounding.AwayFromZero) : Math.Round((Decimal.Multiply(cr.CRI_HRA_Fixed.Value, Convert.ToDecimal(totalPaybleDays))) / totalWorkingDays, MidpointRounding.AwayFromZero));

                        #region START PF-ESIC -OT CALCULATION  
                        if (OvertimeInDay > 0)
                        {
                            if (!cr.CRI_OT_Calculate_Payableday)
                            {
                                if (cr.CRI_OT_Fixed_PerHour > 0)
                                {
                                    WAR_OverTime_Calculated = Convert.ToDecimal(WAR_ExtraWorkingHours) * cr.CRI_OT_Fixed_PerHour.Value;
                                }
                                else if (cr.CRI_OT_Formula != null)
                                {
                                    decimal OTsum = Math.Round(GetAmountBasedOnFormulaOT(cr.CRI_OT_Formula, WAR_Basic_Calculated, CRI_DA_Calculated,
                                        CRI_HRA_Calculated, cr.Client_Requirement_Allowances.ToList(), totalWorkingDays, totalPaybleDays, WAR_Outstation_Allowance_Calculated,
                                        WAR_Attendance_Allowance_Calculated, WAR_Nightshift_Allowance_Calculated, WAR_Performance_Allowance_Calculated), MidpointRounding.AwayFromZero);

                                    WAR_OverTime_Calculated = Math.Round(Convert.ToDecimal(((Convert.ToDouble(OTsum) / totalPaybleDays) * OvertimeInDay) * cr.CRI_OT_MultipleTimes), MidpointRounding.AwayFromZero);
                                }
                            }
                        }

                        decimal PFsum = Math.Round(GetAmountBasedOnFormula(
                            cr.CRI_PF_Formula, WAR_Basic_Calculated, CRI_DA_Calculated, CRI_HRA_Calculated,
                            cr.Client_Requirement_Allowances.ToList(), totalWorkingDays, totalPaybleDays,
                            WAR_OverTime_Calculated, WAR_Outstation_Allowance_Calculated, WAR_Attendance_Allowance_Calculated,
                            WAR_Nightshift_Allowance_Calculated, WAR_Performance_Allowance_Calculated), MidpointRounding.AwayFromZero);

                        decimal ESICsum = Math.Round(GetAmountBasedOnFormula(
                            cr.CRI_ESIC_Formula, WAR_Basic_Calculated, CRI_DA_Calculated, CRI_HRA_Calculated,
                            cr.Client_Requirement_Allowances.ToList(), totalWorkingDays, totalPaybleDays, WAR_OverTime_Calculated,
                            WAR_Outstation_Allowance_Calculated, WAR_Attendance_Allowance_Calculated, WAR_Nightshift_Allowance_Calculated,
                            WAR_Performance_Allowance_Calculated), MidpointRounding.AwayFromZero);

                        WAR_PF = Convert.ToDecimal(cr.CRI_PF_Percentage);
                        WAR_ESIC = Convert.ToDecimal(cr.CRI_ESIC_Percentage);
                        WAR_PF_Calculated = Math.Round(Decimal.Multiply(PFsum, WAR_PF) / 100, MidpointRounding.AwayFromZero);
                        WAR_ESIC_Calculated = Math.Ceiling(Decimal.Multiply(ESICsum, WAR_ESIC) / 100);
                        #endregion

                        wageRegisterVM.WAR_PF_Formula = cr.CRI_PF_Formula;
                        wageRegisterVM.WAR_ESIC_Formula = cr.CRI_ESIC_Formula;
                        wageRegisterVM.WAR_PF = WAR_PF;
                        wageRegisterVM.WAR_PF_Calculated = WAR_PF_Calculated;
                        wageRegisterVM.WAR_ESIC = WAR_ESIC;
                        wageRegisterVM.WAR_ESIC_Calculated = WAR_ESIC_Calculated;
                    }

                    wageRegisterVM.WAR_OverTime_Calculated = WAR_OverTime_Calculated;
                    wageRegisterVM.WAR_OverTime_Formula = cr.CRI_OT_Formula;
                    wageRegisterVM.WAR_WorkingHrs_In_Day = cli.CLI_WorkingHours_In_Day;
                    wageRegisterVM.WAR_OverTime_Payment = Convert.ToInt16(cr.CRI_OT_MultipleTimes);
                    wageRegisterVM.WAR_Basic_Calculated = WAR_Basic_Calculated;
                    wageRegisterVM.WAR_HRA_Calculated = CRI_HRA_Calculated;


                    //************************** ALLOWANCES CALCULATION *****************************//
                    List<WageRegisterAllowanceVM> allowances = new List<WageRegisterAllowanceVM>();
                    decimal AllowancesTotal = 0;
                    foreach (var allowance in cr.Client_Requirement_Allowances)
                    {
                        WageRegisterAllowanceVM all = new WageRegisterAllowanceVM();
                        all.CRA_Id = allowance.CRA_Id;
                        all.CRI_Id = allowance.CRI_Id;
                        all.WAA_Amount = allowance.CRA_Amount;
                        all.WAA_DayswiseOrFull = allowance.CRA_DayswiseOrFull;
                        all.allowanceVM = AllowanceMapper.mapMe(allowance.ALL_);
                        decimal amount = allowance.CRA_Amount;
                        decimal fullAmt = 0M;
                        if (totalWorkingDays != 0)
                            fullAmt = Math.Round((Decimal.Multiply(allowance.CRA_Amount, Convert.ToDecimal(totalPaybleDays))) / totalWorkingDays, MidpointRounding.AwayFromZero);

                        if (allowance.CRA_DayswiseOrFull)
                        {
                            all.WAA_Amount_Calculated = fullAmt;
                            AllowancesTotal += fullAmt;
                        }
                        else
                        {
                            all.WAA_Amount_Calculated = amount;
                            AllowancesTotal += amount;
                        }
                        allowances.Add(all);
                    }

                    wageRegisterVM.WAR_Attendance_Allowance_Calculated = WAR_Attendance_Allowance_Calculated;
                    wageRegisterVM.allowanceVMs = allowances;
                    //************************** ALLOWANCES CALCULATION *****************************/
                    decimal WAR_GrossTotal = Math.Round(WAR_Basic_Calculated + CRI_DA_Calculated + CRI_HRA_Calculated + WAR_OverTime_Calculated +
                                                        AllowancesTotal + WAR_Outstation_Allowance_Calculated + WAR_Nightshift_Allowance_Calculated +
                                                        WAR_Performance_Allowance_Calculated + WAR_Attendance_Allowance_Calculated, MidpointRounding.AwayFromZero);

                    #region ProfessionalTax Calculation
                    decimal WAR_ProffesionalTax_Calculated = 0M, WAR_RevenueDeduction_Calculated = 0M, WAR_CanteenFacility_Calculation = 0M;
                    if (cr.CRI_ProfessionalTax == true)
                    {
                        ProfessionalTaxCalculationManager ptcManager = new ProfessionalTaxCalculationManager(_context);

                        WAR_ProffesionalTax_Calculated = Math.Round(ptcManager.GetPT((employee.EMP_.EMP_Gender ? "M" : "F"), WAR_GrossTotal), MidpointRounding.AwayFromZero);
                    }
                    #endregion

                    #region LWF Calculation   
                    if (wageProcess.WAG_Month.Month == (int)Month.June || wageProcess.WAG_Month.Month == (int)Month.December)
                    {
                        if (!employee.DES_.DES_Exclude_LWF)
                        {
                            if (WAR_GrossTotal > 0 && WAR_GrossTotal < 3000)
                            {
                                WAR_LWF_Deduction_Calculated = 6; //Rs.6
                            }
                            else if (WAR_GrossTotal >= 3000)
                            {
                                WAR_LWF_Deduction_Calculated = 12;  //Rs.12
                            }
                        }
                    }

                    wageRegisterVM.WAR_LWF_Deduction_Calculated = WAR_LWF_Deduction_Calculated;
                    #endregion

                    if (cr.CRI_RevenueDeduction == true)
                    {
                        WAR_RevenueDeduction_Calculated = 1;
                    }
                    if (cr.CRI_CanteenFacility == true)
                    {
                        Wage_Register_Canteen canteen = new Wage_Register_Canteen();
                        canteen = canteenManager.GetCanteenByCLE(wageProcess.WAG_Id, employee.CLE_Id, employee.CLI_Id);
                        if (canteen != null)
                        {
                            WAR_CanteenFacility_Calculation = Math.Round(canteen.WRC_Amount, MidpointRounding.AwayFromZero);
                            wageRegisterVM.WRC_Id = canteen.WRC_Id;
                            wageRegisterVM.WAR_CanteenFacility_Calculation = WAR_CanteenFacility_Calculation.ToString();
                        }
                        else
                        {
                            wageRegisterVM.WAR_CanteenFacility_Calculation = "-";
                        }

                    }
                    wageRegisterVM.WAR_ProffesionalTax_Calculated = WAR_ProffesionalTax_Calculated.ToString();
                    wageRegisterVM.WAR_RevenueDeduction_Calculated = WAR_RevenueDeduction_Calculated.ToString();

                    decimal WAR_FinalTotal = Math.Round(WAR_GrossTotal - (WAR_PF_Calculated + WAR_ESIC_Calculated + totalEMI + WAR_ProffesionalTax_Calculated + WAR_RevenueDeduction_Calculated + WAR_CanteenFacility_Calculation + WAR_LWF_Deduction_Calculated), MidpointRounding.AwayFromZero);
                    wageRegisterVM.WAR_GrossTotal = WAR_GrossTotal;
                    wageRegisterVM.WAR_FinalTotal = WAR_FinalTotal;
                    wageRegisterVM.WAR_DA = CRI_DA;
                    wageRegisterVM.WAR_DA_Calculated = CRI_DA_Calculated;
                    wageRegisterVM.ADM_LastModifiedBy = AdminID;
                    lstRegister.Add(wageRegisterVM);
                }
            }
            return lstRegister;
        }

        public string SaveWageRegister(List<Wage_Register> wage_Registers, int WAG_Id, string CLI_Id, int AdminID)
        {
            string res = string.Empty;
            try
            {

                foreach (Wage_Register wage_Register in wage_Registers)
                {
                    if (wage_Register.Wage_Register_Allowances != null)
                    {
                        foreach (var wageRegisterAllowances in wage_Register.Wage_Register_Allowances)
                        {
                            _context.Wage_Register_Allowances.Add(wageRegisterAllowances);
                        }
                    }
                }
                _context.Wage_Register.AddRange(wage_Registers);
                Wage_Process_Clients process_Client = new Wage_Process_Clients();
                process_Client.WAG_Id = WAG_Id;
                process_Client.CLI_Id = Convert.ToInt32(CLI_Id);
                process_Client.WPC_WageRegisterSaved = true;
                process_Client.ADM_Id_SavedBy = AdminID;
                process_Client.WPC_SavedOn = ProjectUtils.DateNow();
                _context.Wage_Process_Clients.Add(process_Client);

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        }

        public string ResetWageRegister(int WAG_Id, string CLI_Id)
        {
            string res = string.Empty;
            try
            {
                List<Wage_Register> lst = _context.Wage_Register.Where(m => m.WAG_Id.Equals(WAG_Id) && m.CLI_Id.Equals(Convert.ToInt32(CLI_Id))).Include(m => m.Wage_Register_Allowances).ToList();
                List<Wage_Register_Allowances> wra = lst.SelectMany(m => m.Wage_Register_Allowances).ToList();
                if (wra.Count > 0)
                {
                    _context.Wage_Register_Allowances.RemoveRange(wra);
                }
                _context.Wage_Register.RemoveRange(lst);
                Wage_Process_Clients wg = _context.Wage_Process_Clients.Where(m => m.WAG_Id.Equals(WAG_Id) && m.CLI_Id.Equals(Convert.ToInt32(CLI_Id))).FirstOrDefault();
                _context.Wage_Process_Clients.RemoveRange(wg);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }

            return res;
        }

        public Wage_Register GetWage_RegisterByID(int WAR_Id)
        {
            Wage_Register wage = new Wage_Register();
            wage = _context.Wage_Register.Include(m => m.CRI_).ThenInclude(m => m.DES_).SingleOrDefault(m => m.WAR_Id.Equals(WAR_Id));
            return wage;
        }

        public List<Wage_Register_Allowances> GetWage_Register_Allowances(int WAR_Id)
        {
            List<Wage_Register_Allowances> wage_Register_Allowances = new List<Wage_Register_Allowances>();
            wage_Register_Allowances = _context.Wage_Register_Allowances.Include(m => m.CRA_).ThenInclude(m => m.ALL_).Where(m => m.WAR_Id.Equals(WAR_Id)).ToList();
            return wage_Register_Allowances;
        }

        public string UpdateWageRegister(Wage_Register wage_Reg)
        {
            string res = string.Empty;
            try
            {
                wage_Reg.CRI_ = null;
                Wage_Register wage_Register = new Wage_Register();
                wage_Register = wage_Reg;
                wage_Register.WAR_LastModifiedOn = ProjectUtils.DateNow();
                //_context.Wage_Register.Update(wage_Register);
                _context.Entry(wage_Register).State = EntityState.Modified;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                res = ex.InnerException.Message;
            }
            return res;
        }

        public List<Wage_Register_Advances> GetWageRegisterAdvances(int WAG_Id)
        {
            List<Wage_Register_Advances> wage_Register_Advances = new List<Wage_Register_Advances>();
            wage_Register_Advances = _context.Wage_Register_Advances.Where(m => m.WAG_Id.Equals(WAG_Id)).Include(m => m.EMP_)
                .ToList();

            return wage_Register_Advances;
        }
        public List<Wage_Register_Advances> GetWageRegisterAdvances()
        {
            List<Wage_Register_Advances> wage_Register_Advances = new List<Wage_Register_Advances>();
            wage_Register_Advances = _context.Wage_Register_Advances.Include(m => m.WAG_).Include(m => m.EMP_)
                .ToList();

            return wage_Register_Advances;
        }


        public List<Wage_Register> GetWageRegistersByEmpId(int WAG_Id, int EMP_Id)
        {
            return _context.Wage_Register.Where(r => r.WAG_Id == WAG_Id && r.EMP_Id == EMP_Id).Include(m => m.EMP_).ThenInclude(m => m.Employee_Advance).Include(n => n.EMP_).ThenInclude(n => n.Wage_Register_Advances).Include(m => m.CRI_).ThenInclude(m => m.DES_).Include(n => n.Wage_Register_Allowances).ThenInclude(n => n.CRA_).ThenInclude(n => n.ALL_).ToList();
        }

        public List<Wage_Register> GetWageRegistersForIDBI_To_IDBI(int WAG_Id)
        {
            return _context.Wage_Register.Where(m => m.WAG_Id == WAG_Id && m.EMP_.EMP_Payment_Type.Equals((int)PAYMENT_TYPE.Bank_Account) && m.EMP_.EMP_Is_IDBI_Other.Equals((int)PAYMENT_BANK_TYPE.IDBI_To_IDBI)).Include(m => m.CLI_).Include(m => m.EMP_).ThenInclude(m => m.Employee_Advance).Include(n => n.EMP_).ThenInclude(n => n.Wage_Register_Advances).Include(m => m.CRI_).ThenInclude(m => m.DES_).Include(n => n.Wage_Register_Allowances).ThenInclude(n => n.CRA_).ThenInclude(n => n.ALL_).ToList();
        }
        public List<Wage_Register> GetWageRegistersForIDBI_To_Other(int WAG_Id)
        {
            return _context.Wage_Register.Include(M => M.WAG_).ThenInclude(m => m.FRM_).Where(m => m.WAG_Id == WAG_Id && m.EMP_.EMP_Payment_Type.Equals((int)PAYMENT_TYPE.Bank_Account) && m.EMP_.EMP_Is_IDBI_Other.Equals((int)PAYMENT_BANK_TYPE.IDBI_To_Others)).Include(m => m.CLI_).Include(m => m.EMP_).ThenInclude(m => m.EMP_CityNavigation).Include(m => m.EMP_).ThenInclude(m => m.Employee_Advance).Include(n => n.EMP_).ThenInclude(n => n.Wage_Register_Advances).Include(m => m.CRI_).ThenInclude(m => m.DES_).Include(n => n.Wage_Register_Allowances).ThenInclude(n => n.CRA_).ThenInclude(n => n.ALL_).ToList();
        }
        public List<Wage_Register> GetWageRegistersForChequeCash(int WAG_Id)
        {
            return _context.Wage_Register.Where(m => m.WAG_Id == WAG_Id && m.EMP_.EMP_Payment_Type.Equals((int)PAYMENT_TYPE.Cheque_Cash)).Include(m => m.CLI_).Include(m => m.EMP_).ThenInclude(m => m.Employee_Advance).Include(n => n.EMP_).ThenInclude(n => n.Wage_Register_Advances).Include(m => m.CRI_).ThenInclude(m => m.DES_).Include(n => n.Wage_Register_Allowances).ThenInclude(n => n.CRA_).ThenInclude(n => n.ALL_).ToList();
        }

        public List<Wage_Register> GetWageRegistersForInvoice(int CLI_Id)
        {
            return _context.Wage_Register.Include(m => m.WAG_).Include(m => m.CRI_).ThenInclude(m => m.DES_).Include(n => n.Wage_Register_Allowances).ThenInclude(n => n.CRA_).ThenInclude(n => n.ALL_).Where(r => r.CLI_Id == CLI_Id).ToList();
        }

        public int GetClient_EmployeeHavingMaxBasic(int EMP_Id, int WAG_Id)
        {
            int CLI_Id = 0;
            return CLI_Id;
        }

    }
}
