using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RMERP.DAL.App_Code;
using RMERP.DAL.Mappers;
using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public List<Wage_Register> GetWageRegisters(int WAG_Id,int CLI_Id)
        {
            return _context.Wage_Register.Where(r => r.WAG_Id == WAG_Id && r.CLI_Id == CLI_Id).Include(m=>m.EMP_).Include(m=>m.CRI_).ThenInclude(m=>m.DES_).Include(n => n.Wage_Register_Allowances).ThenInclude(n=>n.CRA_).ThenInclude(n=>n.ALL_).ToList();
        }

        public List<ClientWageRegisterVM> GenerateWageRegisterTable(int WAG_Id,int AdminID)
        {
            List<ClientWageRegisterVM> lst = new List<ClientWageRegisterVM>();
            ClientsManager clientsManager = new ClientsManager(_context, _configuration);
            WageProcessManager wageManager = new WageProcessManager(_context);
            Wage_Process wageProcess = wageManager.getWageProcessById(WAG_Id);
            List<Clients> lstCli = clientsManager.GetActiveClientForAttandanceReg(wageProcess.WAG_Month);
            foreach (Clients client in lstCli)
            {               
                ClientWageRegisterVM clientWageRegisterVM = new ClientWageRegisterVM();
                Wage_Process_Clients wage_Process_Clients = wageManager.GetWage_Process_Clients(WAG_Id, client.CLI_Id);
                if(wage_Process_Clients!=null)
                    clientWageRegisterVM.wageProcessClientVM = WageProcessClientsMapper.mapMe(wage_Process_Clients);
                clientWageRegisterVM.client = ClientMapper.mapMe(client);
                clientWageRegisterVM.wageProcessVM = WageProcessMapper.mapMe(wageProcess);
                Wage_Process_Clients wageOfClient = wageManager.GetWage_Process_Clients(WAG_Id, client.CLI_Id);
                List<WageRegisterVM> lstRegister = new List<WageRegisterVM>();
                if (wageOfClient != null)
                    lstRegister = WageRegisterMapper.mapWageRegisters(GetWageRegisters(WAG_Id, client.CLI_Id));
                else
                    lstRegister = GetWageRegisterCalculated(wageProcess, client.CLI_Id, AdminID); 
                clientWageRegisterVM.wageRegisterVMs = lstRegister;
                
                lst.Add(clientWageRegisterVM);
            }
            return lst;
        }
        public List<WageRegisterVM> GetWageRegisterCalculated(Wage_Process wageProcess, int CLI_Id,int AdminID)
        {
            List<WageRegisterVM> lstRegister = new List<WageRegisterVM>();
            ClientsManager clientsManager = new ClientsManager(_context, _configuration);
            AttendanceManager attManager = new AttendanceManager(_context);
            IEnumerable<Clients_Employees> clientsEmployees = clientsManager.listClientsEmployees(CLI_Id);
            foreach(Clients_Employees employee in clientsEmployees)
            {
                IEnumerable<Attendance> attendances = attManager.getAttendance_Wage_Client_Employee_Designation(wageProcess.WAG_Id, CLI_Id, employee.EMP_Id, employee.DES_.DES_Id);
                Client_Requirements cr = clientsManager.getActiveClientRequirement(CLI_Id,employee.DES_.DES_Id);
                int totalWorkingDays = 0, totalPaybleDays = 0;
                decimal CRI_Basic = 0M, WAR_Basic_Calculated=0M, BasicDa = 0M, WAR_OverTime_Calculated = 0M, WAR_PF, WAR_PF_Calculated = 0M, WAR_ESIC = 0M, WAR_ESIC_Calculated = 0M;
                decimal CRI_DA = 0M, CRI_DA_Calculated = 0M, CRI_HRA = 0M, CRI_HRA_Calculated = 0M;
                double WAR_ExtraWorkingHours = attendances.Sum(m => m.ATT_ExtraHoursWorked);

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
                    case 2://   2:Reduce_StaticDays
                        totalWorkingDays = days-Convert.ToInt16(cli.CLI_No_Reduce_Days);
                        break;
                    default: break;
                }

               
                #endregion

                totalPaybleDays = attendances.Where(a => a.ATT_IsPresent == true).Count();
                WageRegisterVM wageRegisterVM = new WageRegisterVM();
                wageRegisterVM.WAG_Id = wageProcess.WAG_Id;
                wageRegisterVM.CLI_Id = CLI_Id;
                wageRegisterVM.EMP_Id = employee.EMP_Id;
                wageRegisterVM.wageProcessVM = WageProcessMapper.mapMe(wageProcess);
                wageRegisterVM.employeeVM = EmployeesMapper.MapMe(employee.EMP_);
                wageRegisterVM.designation = employee.DES_;
                
                wageRegisterVM.clientRequirementVM = ClientRequirementMapper.mapMe(clientsManager.getActiveClientRequirement(CLI_Id, employee.DES_Id));
                wageRegisterVM.WAR_TotalWorkingDays = totalWorkingDays;
                wageRegisterVM.WAR_TotalPaybleDays = totalPaybleDays;
                wageRegisterVM.WAR_ExtraWorkingHours = WAR_ExtraWorkingHours;

                CRI_Basic = cr.CRI_Basic.Value;
                CRI_DA = Convert.ToDecimal(cr.CRI_DA);
               // CRI_DA_Calculated = (Decimal.Multiply(CRI_Basic, Convert.ToDecimal(CRI_DA))) / 100;
                BasicDa = (Decimal.Add(CRI_Basic, Convert.ToDecimal(cr.CRI_DA)));
                CRI_HRA = (cr.CRI_HRA_Fixed == null ? Convert.ToDecimal(cr.CRI_HRA_Percentage.Value) : cr.CRI_HRA_Fixed.Value);

                wageRegisterVM.WAR_Basic = CRI_Basic;
                wageRegisterVM.WAR_HRA = CRI_HRA;
                wageRegisterVM.WAR_LastModifiedOn = ProjectUtils.DateNow();
                
                if (totalWorkingDays != 0)
                {
                    CRI_DA_Calculated = (CRI_DA * totalPaybleDays) / totalWorkingDays;
                    CRI_HRA_Calculated = (cr.CRI_HRA_Fixed == null ? ((BasicDa * Convert.ToDecimal(cr.CRI_HRA_Percentage)) / 100) : ((Decimal.Multiply(cr.CRI_HRA_Fixed.Value, totalPaybleDays)) / totalWorkingDays));
                    WAR_Basic_Calculated = (Decimal.Multiply(CRI_Basic, Convert.ToDecimal(totalPaybleDays))) / totalWorkingDays;
                    WAR_OverTime_Calculated = (((CRI_Basic / Convert.ToDecimal(totalWorkingDays)) / 8) * Convert.ToDecimal(WAR_ExtraWorkingHours));                   
                    
                }                
               
                wageRegisterVM.WAR_OverTime_Calculated = WAR_OverTime_Calculated;
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
                    all.allowanceVM = AllowanceMapper.mapMe(allowance.ALL_);
                    decimal amount = allowance.CRA_Amount;
                    decimal fullAmt = 0M;
                    if (totalWorkingDays != 0) 
                        fullAmt = (Decimal.Multiply(allowance.CRA_Amount, Convert.ToDecimal(totalPaybleDays))) / totalWorkingDays;                        
                    
                    if (allowance.CRA_DayswiseOrFull) { 
                        all.WAA_Amount_Calculated = fullAmt;
                        AllowancesTotal += fullAmt;
                    }
                    else { 
                        all.WAA_Amount_Calculated = amount;
                        AllowancesTotal += amount;
                    }                    
                    allowances.Add(all);
                }
                #region START PF-ESIC CALCULATION                              
                decimal PFsum = GetAmountBasedOnFormula(cr.CRI_PF_Formula, WAR_Basic_Calculated, CRI_DA_Calculated, CRI_HRA_Calculated, cr.Client_Requirement_Allowances.ToList(), totalWorkingDays, totalPaybleDays);
                decimal ESICsum = GetAmountBasedOnFormula(cr.CRI_ESIC_Formula, WAR_Basic_Calculated, CRI_DA_Calculated, CRI_HRA_Calculated, cr.Client_Requirement_Allowances.ToList(), totalWorkingDays, totalPaybleDays);               
                WAR_PF = Convert.ToDecimal(cr.CRI_PF_Percentage);
                WAR_ESIC = Convert.ToDecimal(cr.CRI_ESIC_Percentage);
                WAR_PF_Calculated = Decimal.Multiply(PFsum, WAR_PF) / 100;
                WAR_ESIC_Calculated = Decimal.Multiply(ESICsum, WAR_ESIC) / 100;
                #endregion
                wageRegisterVM.WAR_PF_Formula = cr.CRI_PF_Formula;
                wageRegisterVM.WAR_ESIC_Formula = cr.CRI_ESIC_Formula;
                wageRegisterVM.WAR_PF = WAR_PF;
                wageRegisterVM.WAR_PF_Calculated = WAR_PF_Calculated;
                wageRegisterVM.WAR_ESIC = WAR_ESIC;
                wageRegisterVM.WAR_ESIC_Calculated = WAR_ESIC_Calculated;
                wageRegisterVM.allowanceVMs = allowances;
                //************************** ALLOWANCES CALCULATION *****************************/
                decimal WAR_GrossTotal = WAR_Basic_Calculated + CRI_DA_Calculated + CRI_HRA_Calculated  + WAR_OverTime_Calculated + AllowancesTotal;
                decimal WAR_FinalTotal = WAR_GrossTotal - (WAR_PF_Calculated + WAR_ESIC_Calculated);

                wageRegisterVM.WAR_GrossTotal = WAR_GrossTotal;
                wageRegisterVM.WAR_FinalTotal = WAR_FinalTotal;
                wageRegisterVM.WAR_DA =CRI_DA;
                wageRegisterVM.WAR_DA_Calculated = CRI_DA_Calculated;
                wageRegisterVM.ADM_LastModifiedBy = AdminID;
                lstRegister.Add(wageRegisterVM);
            }
            return lstRegister;
        }
        public decimal GetAmountBasedOnFormula(string CRI_Formula, decimal WAR_Basic_Calculated, decimal CRI_DA_Calculated, decimal CRI_HRA_Calculated,List<Client_Requirement_Allowances> All,int totalWorkingDays,int totalPaybleDays)
        {
            decimal sum = 0M;
            string[] arr_CRI_Formula;

            if (CRI_Formula != null)
            {
                arr_CRI_Formula = CRI_Formula.Split("+");
                foreach (string item in arr_CRI_Formula)
                {
                    switch (item)
                    {
                        case "BASIC":
                            sum += Convert.ToDecimal(WAR_Basic_Calculated);
                            break;
                        case "DA":
                            sum += Convert.ToDecimal(CRI_DA_Calculated);
                            break;
                        case "HRA":
                            sum += Convert.ToDecimal(CRI_HRA_Calculated);
                            break;
                        default:
                            {
                                foreach (var allowance in All)
                                {
                                    if (allowance.ALL_.ALL_Shortform == item)
                                    {
                                        decimal amount = allowance.CRA_Amount;
                                        decimal fullAmt = 0M;
                                        if (totalWorkingDays != 0)
                                            fullAmt = (Decimal.Multiply(amount, Convert.ToDecimal(totalPaybleDays))) / totalWorkingDays;

                                        if (allowance.CRA_DayswiseOrFull)
                                        {
                                            sum += fullAmt;
                                        }
                                        else
                                        {
                                            sum += amount;
                                        }
                                    }
                                }
                            }
                            break;
                    }
                }
            }
            return sum;
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
                List<Wage_Register> lst = _context.Wage_Register.Where(m => m.WAG_Id.Equals(WAG_Id) && m.CLI_Id.Equals(Convert.ToInt32(CLI_Id))).Include(m=>m.Wage_Register_Allowances).ToList();
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
            wage = _context.Wage_Register.Include(m => m.CRI_).ThenInclude(m=>m.DES_).SingleOrDefault(m => m.WAR_Id.Equals(WAR_Id));            
            return wage;
        }
        public List<Wage_Register_Allowances> GetWage_Register_Allowances(int WAR_Id)
        {
            List<Wage_Register_Allowances> wage_Register_Allowances = new List<Wage_Register_Allowances>();
            wage_Register_Allowances = _context.Wage_Register_Allowances.Include(m=>m.CRA_).ThenInclude(m=>m.ALL_).Where(m => m.WAR_Id.Equals(WAR_Id)).ToList();
            return wage_Register_Allowances;
        }
        public string UpdateWageRegister(Wage_Register wage_Register)
        {
            string res = string.Empty;
            try
            {
                wage_Register.WAR_LastModifiedOn = ProjectUtils.DateNow();
                //;_context.Wage_Register.Update(wage_Register);
                _context.Entry(wage_Register).State = EntityState.Modified;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                res = ex.InnerException.Message;
            }
            return res;
        }
    }
}
