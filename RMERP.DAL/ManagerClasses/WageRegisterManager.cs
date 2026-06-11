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
			return _context.Wage_Registers.Where(r => r.WAG_Id == WAG_Id && r.CLI_Id == CLI_Id)
				.Include(m => m.CLI)
				.Include(m => m.EMP).ThenInclude(m => m.Employee_Advances)
				.Include(n => n.EMP).ThenInclude(n => n.Wage_Register_Advances)
				.Include(m => m.CRI).ThenInclude(m => m.DES)
				.Include(n => n.Wage_Register_Allowances).ThenInclude(n => n.CRA).ThenInclude(n => n.ALL).ToList();
		}

		public List<Wage_Register> GetWageRegistersByWAG_Id(int WAG_Id)
		{
			return _context.Wage_Registers.Where(r => r.WAG_Id == WAG_Id).Include(m => m.CLI).Include(m => m.EMP).ThenInclude(m => m.Employee_Advances).Include(n => n.EMP).ThenInclude(n => n.Wage_Register_Advances).Include(m => m.CRI).ThenInclude(m => m.DES).ThenInclude(m => m.Client_Requirements).Include(n => n.Wage_Register_Allowances).ThenInclude(n => n.CRA).ThenInclude(n => n.ALL).ToList();
		}



		//public List<Wage_Register> GetWageRegistersByLWF(int WAG_Id)
		//{
		//    return _context.Wage_Register.Where(r => r.WAG_Id == WAG_Id && r.CRI_.DES_.DES_Exclude_LWF.Equals(false)).Include(m => m.CLI).Include(m => m.EMP_).ThenInclude(m => m.Employee_Advance).Include(n => n.EMP_).ThenInclude(n => n.Wage_Register_Advances).Include(m => m.CRI_).ThenInclude(m => m.DES_).ThenInclude(m => m.Client_Requirements).Include(n => n.Wage_Register_Allowances).ThenInclude(n => n.CRA_).ThenInclude(n => n.ALL).ToList();
		//}

		public List<Wage_Register> GetWageRegistersByLWF(int WAG_Id)
		{
			//return _context.Wage_Registers.Where(r => r.WAG_Id == WAG_Id).Include(m => m.CLI).Include(m => m.EMP).ThenInclude(m => m.Employee_Advances).Include(n => n.EMP).ThenInclude(n => n.Wage_Register_Advances).Include(m => m.CRI).ThenInclude(m => m.DES).ThenInclude(m => m.Client_Requirements).Include(n => n.Wage_Register_Allowances).ThenInclude(n => n.CRA).ThenInclude(n => n.ALL).ToList();
			return [.. _context.Wage_Registers.Where(r => r.WAG_Id == WAG_Id).Include(m => m.CLI).Include(m => m.CRI)];
		}
		public List<Wage_Register> GetWageRegisters(int WAG_Id)
		{
			List<Wage_Register> lst = _context.Wage_Registers.Where(r => r.WAG_Id == WAG_Id).Include(m => m.WAG).Include(m => m.CLI).Include(m => m.EMP).ToList();
			return lst;
		}

		public Wage_Register GetWageRegister(int WAR_Id)
		{
			return _context.Wage_Registers.Include(m => m.CRI).Include(m => m.WAG).Include(m => m.CLI).Include(m => m.EMP).Where(r => r.WAR_Id == WAR_Id).FirstOrDefault();
		}
		public List<ClientWageRegisterVM> GenerateWageRegisterTable(int WAG_Id, int AdminID, int FRM_Id)
		{
			List<ClientWageRegisterVM> lst = [];
			ClientsManager clientsManager = new(_context, _configuration);
			WageProcessManager wageManager = new(_context);
			Wage_Process wageProcess = wageManager.getWageProcessById(WAG_Id);
			List<Client> clients = clientsManager.GetActiveClientsOfMonthByFirmId(new DateTime(wageProcess.WAG_Month.Year, wageProcess.WAG_Month.Month, wageProcess.WAG_Month.Day), FRM_Id);

			foreach (Client client in clients)
			{
				Wage_Process_Client wage_Process_Clients = wageManager.GetWage_Process_Clients(WAG_Id, client.CLI_Id);

				List<WageRegisterVM> lstRegister;
				if (wage_Process_Clients != null)
					lstRegister = WageRegisterMapper.mapWageRegisters(GetWageRegisters(WAG_Id, client.CLI_Id));
				else
					lstRegister = GetWageRegisterCalculated(wageProcess, client.CLI_Id, AdminID);

				ClientWageRegisterVM clientWageRegisterVM = new()
				{
					wageProcessClientVM = wage_Process_Clients != null ? WageProcessClientsMapper.MapMe(wage_Process_Clients) : null,
					client = ClientMapper.MapMe(client),
					wageProcessVM = WageProcessMapper.MapMe(wageProcess),
					wageRegisterVMs = lstRegister
				};

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
			AllowanceManager allowanceManager = new AllowanceManager(_context);
			IEnumerable<Clients_Employee> clientsEmployees = clientsManager.listActiveClientsEmployees(CLI_Id, new DateTime(wageProcess.WAG_Month.Year, wageProcess.WAG_Month.Month, wageProcess.WAG_Month.Day));
			foreach (Clients_Employee employee in clientsEmployees)
			{
				IEnumerable<Attendance> attendances = attManager.getAttendance_Wage_Client_Employee_Designation(wageProcess.WAG_Id, CLI_Id, employee.EMP_Id, employee.DES.DES_Id);
				if (attendances.Count() > 0)
				{
					Client_Requirement cr = clientsManager.getActiveClientRequirement(CLI_Id, employee.DES.DES_Id, new DateTime(wageProcess.WAG_Month.Year, wageProcess.WAG_Month.Month, wageProcess.WAG_Month.Day));

					int totalWorkingDays = 0;
					double totalPaybleDays = 0;
					decimal CRI_Basic = 0M, WAR_Basic_Calculated = 0M, BasicDa = 0M, WAR_OverTime_Calculated = 0M, WAR_PF, WAR_PF_Calculated = 0M, WAR_ESIC = 0M, WAR_ESIC_Calculated = 0M, WAR_LWF_Deduction_Employer = 0M, WAR_LWF_Deduction_Employee = 0M;
					decimal CRI_DA = 0M, CRI_DA_Calculated = 0M, CRI_HRA = 0M, CRI_LeaveAndPH = 0M, CRI_HRA_Calculated = 0M, CRI_LeaveAndPH_Calculated = 0M;
					decimal WAR_Attendance_Allowance_Calculated = 0M,
						WAR_Outstation_Allowance_Calculated = 0M,
						WAR_Performance_Allowance_Calculated = 0M,
						WAR_Nightshift_Allowance_Calculated = 0M,
						WAR_Allowance_Calculated_1 = 0M, WAR_Allowance_Calculated_2 = 0M, WAR_Allowance_Calculated_3 = 0M, WAR_Allowance_Calculated_4 = 0M, WAR_Allowance_Calculated_5 = 0M, WAR_Allowance_Calculated_6 = 0M, WAR_Allowance_Calculated_7 = 0M, WAR_Allowance_Calculated_8 = 0M, WAR_Allowance_Calculated_9 = 0M, WAR_Allowance_Calculated_10 = 0M;

					#region Total working day calculation
					Client cli = clientsManager.GetClientById(CLI_Id);
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
					double totPresentDays = 0, totHalfDays = 0, totExtraWorkingDays = 0, WAR_ExtraWorkingHours = 0;// totNighthours = 0;

					totPresentDays = attendances.Where(a => a.ATT_IsPresent == true).Count();
					totEarnLeave = attendances.Where(a => a.ATT_IsEarnLeave == true).Count();
					totNightShift = attendances.Where(a => a.ATT_NightShift == true).Count();
					totHalfDays = attendances.Where(a => a.ATT_IsHalfday == true).Count();

					WAR_ExtraWorkingHours = attendances.Sum(m => m.ATT_ExtraHoursWorked);
					totExtraWorkingDays = WAR_ExtraWorkingHours / cli.CLI_WorkingHours_In_Day;

					totWeekOffs = attendances.Where(a => a.ATT_IsWeeklyOff == true).Count();
					totalPublicHoliday = attendances.Where(a => a.ATT_IsPublicHoliday == true).Count();
					// totNighthours = totNightShift * cli.CLI_WorkingHours_In_Day;

					totalPaybleDays = (totPresentDays - totHalfDays) + (totHalfDays / 2) + totEarnLeave;

					if (cr != null)
					{
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
					}
					totAbsent = attendances.Where(m => m.ATT_IsPresent.Equals(false) && m.ATT_IsEarnLeave.Equals(false) && m.ATT_IsPublicHoliday.Equals(false) && m.ATT_IsWeeklyOff.Equals(false)).Count();
					#endregion

					WageRegisterVM wageRegisterVM = new WageRegisterVM();
					wageRegisterVM.CLE_Id = employee.CLE_Id;
					wageRegisterVM.WAG_Id = wageProcess.WAG_Id;
					wageRegisterVM.CLI_Id = CLI_Id;
					wageRegisterVM.EMP_Id = employee.EMP_Id;
					//  wageRegisterVM.wageProcessVM = WageProcessMapper.mapMe(wageProcess);
					wageRegisterVM.employeeVM = EmployeesMapper.MapMe(employee.EMP);
					wageRegisterVM.designation = employee.DES;


					#region New Allowances 
					if (cr != null)
					{
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
							// WAR_Nightshift_Allowance_Calculated = Convert.ToDecimal(Convert.ToDouble(cr.CRI_Nightshift_Allowance_Rate.Value) * totNighthours);
							WAR_Nightshift_Allowance_Calculated = Convert.ToDecimal(Convert.ToDouble(cr.CRI_Nightshift_Allowance_Rate.Value) * totNightShift);
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
						if (cr.CRI_Allowance_1)
						{
							Wage_Register_Allowances_1 all_1 = new Wage_Register_Allowances_1();
							all_1 = allowanceManager.GetAllowances_ByCLE_1(wageProcess.WAG_Id, employee.CLE_Id, employee.CLI_Id);
							if (all_1 != null)
							{
								WAR_Allowance_Calculated_1 = all_1.WRA_Amount_1;
								wageRegisterVM.WRA_Id_1 = all_1.WRA_Id_1;
								wageRegisterVM.WAR_Allowance_Calculated_1 = WAR_Allowance_Calculated_1;
							}
							else
							{
								wageRegisterVM.WAR_Allowance_Calculated_1 = 0;
							}
						}
						if (cr.CRI_Allowance_2)
						{
							Wage_Register_Allowances_2 all_2 = new Wage_Register_Allowances_2();
							all_2 = allowanceManager.GetAllowances_ByCLE_2(wageProcess.WAG_Id, employee.CLE_Id, employee.CLI_Id);
							if (all_2 != null)
							{
								WAR_Allowance_Calculated_2 = all_2.WRA_Amount_2;
								wageRegisterVM.WRA_Id_2 = all_2.WRA_Id_2;
								wageRegisterVM.WAR_Allowance_Calculated_2 = WAR_Allowance_Calculated_2;
							}
							else
							{
								wageRegisterVM.WAR_Allowance_Calculated_2 = 0;
							}
						}
						if (cr.CRI_Allowance_3)
						{
							Wage_Register_Allowances_3 all_3 = new Wage_Register_Allowances_3();
							all_3 = allowanceManager.GetAllowances_ByCLE_3(wageProcess.WAG_Id, employee.CLE_Id, employee.CLI_Id);
							if (all_3 != null)
							{
								WAR_Allowance_Calculated_3 = all_3.WRA_Amount_3;
								wageRegisterVM.WRA_Id_3 = all_3.WRA_Id_3;
								wageRegisterVM.WAR_Allowance_Calculated_3 = WAR_Allowance_Calculated_3;
							}
							else
							{
								wageRegisterVM.WAR_Allowance_Calculated_3 = 0;
							}
						}
						if (cr.CRI_Allowance_4)
						{
							Wage_Register_Allowances_4 all_4 = new Wage_Register_Allowances_4();
							all_4 = allowanceManager.GetAllowances_ByCLE_4(wageProcess.WAG_Id, employee.CLE_Id, employee.CLI_Id);
							if (all_4 != null)
							{
								WAR_Allowance_Calculated_4 = all_4.WRA_Amount_4;
								wageRegisterVM.WRA_Id_4 = all_4.WRA_Id_4;
								wageRegisterVM.WAR_Allowance_Calculated_4 = WAR_Allowance_Calculated_4;
							}
							else
							{
								wageRegisterVM.WAR_Allowance_Calculated_4 = 0;
							}
						}
						if (cr.CRI_Allowance_5)
						{
							Wage_Register_Allowances_5 all_5 = new Wage_Register_Allowances_5();
							all_5 = allowanceManager.GetAllowances_ByCLE_5(wageProcess.WAG_Id, employee.CLE_Id, employee.CLI_Id);
							if (all_5 != null)
							{
								WAR_Allowance_Calculated_5 = all_5.WRA_Amount_5;
								wageRegisterVM.WRA_Id_5 = all_5.WRA_Id_5;
								wageRegisterVM.WAR_Allowance_Calculated_5 = WAR_Allowance_Calculated_5;
							}
							else
							{
								wageRegisterVM.WAR_Allowance_Calculated_5 = 0;
							}
						}
						if (cr.CRI_Allowance_6)
						{
							Wage_Register_Allowances_6 all_6 = allowanceManager.GetAllowances_ByCLE_6(wageProcess.WAG_Id, employee.CLE_Id, employee.CLI_Id);
							if (all_6 != null)
							{
								WAR_Allowance_Calculated_6 = all_6.WRA_Amount_6;
								wageRegisterVM.WRA_Id_6 = all_6.WRA_Id_6;
								wageRegisterVM.WAR_Allowance_Calculated_6 = WAR_Allowance_Calculated_6;
							}
							else
							{
								wageRegisterVM.WAR_Allowance_Calculated_6 = 0;
							}
						}
						if (cr.CRI_Allowance_7)
						{
							Wage_Register_Allowances_7 all_7 = allowanceManager.GetAllowances_ByCLE_7(wageProcess.WAG_Id, employee.CLE_Id, employee.CLI_Id);
							if (all_7 != null)
							{
								WAR_Allowance_Calculated_7 = all_7.WRA_Amount_7;
								wageRegisterVM.WRA_Id_7 = all_7.WRA_Id_7;
								wageRegisterVM.WAR_Allowance_Calculated_7 = WAR_Allowance_Calculated_7;
							}
							else
							{
								wageRegisterVM.WAR_Allowance_Calculated_7 = 0;
							}
						}
						if (cr.CRI_Allowance_8)
						{
							Wage_Register_Allowances_8 all_8 = allowanceManager.GetAllowances_ByCLE_8(wageProcess.WAG_Id, employee.CLE_Id, employee.CLI_Id);
							if (all_8 != null)
							{
								WAR_Allowance_Calculated_8 = all_8.WRA_Amount_8;
								wageRegisterVM.WRA_Id_8 = all_8.WRA_Id_8;
								wageRegisterVM.WAR_Allowance_Calculated_8 = WAR_Allowance_Calculated_8;
							}
							else
							{
								wageRegisterVM.WAR_Allowance_Calculated_8 = 0;
							}
						}
						if (cr.CRI_Allowance_9)
						{
							Wage_Register_Allowances_9 all_9 = allowanceManager.GetAllowances_ByCLE_9(wageProcess.WAG_Id, employee.CLE_Id, employee.CLI_Id);
							if (all_9 != null)
							{
								WAR_Allowance_Calculated_9 = all_9.WRA_Amount_9;
								wageRegisterVM.WRA_Id_9 = all_9.WRA_Id_9;
								wageRegisterVM.WAR_Allowance_Calculated_9 = WAR_Allowance_Calculated_9;
							}
							else
							{
								wageRegisterVM.WAR_Allowance_Calculated_9 = 0;
							}
						}
						if (cr.CRI_Allowance_10)
						{
							Wage_Register_Allowances_10 all_10 = allowanceManager.GetAllowances_ByCLE_10(wageProcess.WAG_Id, employee.CLE_Id, employee.CLI_Id);
							if (all_10 != null)
							{
								WAR_Allowance_Calculated_10 = all_10.WRA_Amount_10;
								wageRegisterVM.WRA_Id_10 = all_10.WRA_Id_10;
								wageRegisterVM.WAR_Allowance_Calculated_10 = WAR_Allowance_Calculated_10;
							}
							else
							{
								wageRegisterVM.WAR_Allowance_Calculated_10 = 0;
							}
						}
					}

					#endregion

					if (cr != null)
					{
						wageRegisterVM.clientRequirementVM = ClientRequirementMapper.mapMe(cr);
					}
					else
					{
						wageRegisterVM.clientRequirementVM = new ClientRequirementVM();
					}

					wageRegisterVM.WAR_TotalWorkingDays = totalWorkingDays;
					wageRegisterVM.WAR_TotalPaybleDays = totalPaybleDays;
					wageRegisterVM.WAR_ExtraWorkingHours = WAR_ExtraWorkingHours;

					if (cr != null)
					{
						CRI_Basic = Math.Round(cr.CRI_Basic.Value, MidpointRounding.AwayFromZero);
						CRI_DA = Math.Round(Convert.ToDecimal(cr.CRI_DA), MidpointRounding.AwayFromZero);
						BasicDa = Math.Round(Decimal.Add(CRI_Basic, Convert.ToDecimal(cr.CRI_DA)), MidpointRounding.AwayFromZero);
						CRI_HRA = Math.Round(cr.CRI_HRA_Fixed == null ? Convert.ToDecimal(cr.CRI_HRA_Percentage.Value) : cr.CRI_HRA_Fixed.Value, MidpointRounding.AwayFromZero);
						CRI_LeaveAndPH = Math.Round(cr.CRI_LeaveAndPH_Fixed == null ? Convert.ToDecimal(cr.CRI_LeaveAndPH_Percentage.Value) : cr.CRI_LeaveAndPH_Fixed.Value, MidpointRounding.AwayFromZero);
					}
					wageRegisterVM.WAR_Basic = CRI_Basic;
					wageRegisterVM.WAR_HRA = CRI_HRA;
					wageRegisterVM.WAR_LeaveAndPH = CRI_LeaveAndPH;
					wageRegisterVM.WAR_LastModifiedOn = ProjectUtils.DateNow();

					if (totalWorkingDays > 0)
					{
						double OvertimeInDay = WAR_ExtraWorkingHours / Convert.ToDouble(cli.CLI_WorkingHours_In_Day);

						CRI_DA_Calculated = Math.Round(Math.Round(CRI_DA * Convert.ToDecimal(totalPaybleDays), MidpointRounding.AwayFromZero) / totalWorkingDays, MidpointRounding.AwayFromZero);
						WAR_Basic_Calculated = Math.Round(Math.Round((Decimal.Multiply(CRI_Basic, Convert.ToDecimal(totalPaybleDays))) / totalWorkingDays, MidpointRounding.AwayFromZero), MidpointRounding.AwayFromZero);
						decimal CalculatedBasicDa = Math.Round(Decimal.Add(WAR_Basic_Calculated, Convert.ToDecimal(CRI_DA_Calculated)), MidpointRounding.AwayFromZero);
						if (cr != null)
						{
							CRI_HRA_Calculated = (cr.CRI_HRA_Fixed == null ? Math.Round((Math.Round(CalculatedBasicDa * Convert.ToDecimal(cr.CRI_HRA_Percentage))) / 100, MidpointRounding.AwayFromZero) : Math.Round((Decimal.Multiply(cr.CRI_HRA_Fixed.Value, Convert.ToDecimal(totalPaybleDays))) / totalWorkingDays, MidpointRounding.AwayFromZero));
							CRI_LeaveAndPH_Calculated = (cr.CRI_LeaveAndPH_Fixed == null ? Math.Round((Math.Round(CalculatedBasicDa * Convert.ToDecimal(cr.CRI_LeaveAndPH_Percentage))) / 100, MidpointRounding.AwayFromZero) : Math.Round((Decimal.Multiply(cr.CRI_LeaveAndPH_Fixed.Value, Convert.ToDecimal(totalPaybleDays))) / totalWorkingDays, MidpointRounding.AwayFromZero));
						}

						#region START PF-ESIC -OT CALCULATION  
						if (OvertimeInDay > 0)
						{
							if (cr != null)
							{
								if (!cr.CRI_OT_Calculate_Payableday)
								{
									if (cr.CRI_OT_Fixed_PerHour > 0)
									{
										WAR_OverTime_Calculated = Convert.ToDecimal(WAR_ExtraWorkingHours) * cr.CRI_OT_Fixed_PerHour.Value;
									}
									else if (cr.CRI_OT_Formula != null)
									{
										decimal OTsum = Math.Round(GetAmountBasedOnFormulaOT(
											cr.CRI_OT_Formula,
											WAR_Basic_Calculated,
											CRI_DA_Calculated,
											CRI_HRA_Calculated,
											CRI_LeaveAndPH_Calculated,
											cr.Client_Requirement_Allowances.ToList(),
											totalWorkingDays,
											totalPaybleDays,
											WAR_Outstation_Allowance_Calculated,
											WAR_Attendance_Allowance_Calculated,
											WAR_Nightshift_Allowance_Calculated,
											WAR_Performance_Allowance_Calculated,
											WAR_Allowance_Calculated_1, WAR_Allowance_Calculated_2, WAR_Allowance_Calculated_3, WAR_Allowance_Calculated_4, WAR_Allowance_Calculated_5, WAR_Allowance_Calculated_6, WAR_Allowance_Calculated_7, WAR_Allowance_Calculated_8, WAR_Allowance_Calculated_9, WAR_Allowance_Calculated_10), MidpointRounding.AwayFromZero);

										//decimal OTsum = Math.Round(GetAmountBasedOnFormulaOT(
										//    cr.CRI_OT_Formula,
										//    Convert.ToDecimal(cr.CRI_Basic),
										//    Convert.ToDecimal(cr.CRI_DA),
										//    cr.CRI_HRA_Fixed != null ? cr.CRI_HRA_Fixed.Value : ((Convert.ToDecimal(cr.CRI_Basic) + Convert.ToDecimal(cr.CRI_DA)) * Convert.ToDecimal(cr.CRI_HRA_Percentage) / 100),
										//    cr.Client_Requirement_Allowances.ToList(),
										//    totalWorkingDays,
										//    totalPaybleDays,
										//    WAR_Outstation_Allowance_Calculated,
										//    WAR_Attendance_Allowance_Calculated,
										//    WAR_Nightshift_Allowance_Calculated,
										//    WAR_Performance_Allowance_Calculated,
										//    WAR_Allowance_Calculated_1, WAR_Allowance_Calculated_2, WAR_Allowance_Calculated_3, WAR_Allowance_Calculated_4, WAR_Allowance_Calculated_5
										//    ), MidpointRounding.AwayFromZero);

										WAR_OverTime_Calculated = Math.Round(Convert.ToDecimal(((Convert.ToDouble(OTsum) / totalPaybleDays) * OvertimeInDay) * cr.CRI_OT_MultipleTimes), MidpointRounding.AwayFromZero);
									}
								}
							}

						}
						if (cr != null)
						{
							decimal PFsum = Math.Round(GetAmountBasedOnFormula(
							cr.CRI_PF_Formula, WAR_Basic_Calculated, CRI_DA_Calculated, CRI_HRA_Calculated, CRI_LeaveAndPH_Calculated,
							cr.Client_Requirement_Allowances.ToList(), totalWorkingDays, totalPaybleDays,
							WAR_OverTime_Calculated, WAR_Outstation_Allowance_Calculated, WAR_Attendance_Allowance_Calculated,
							WAR_Nightshift_Allowance_Calculated, WAR_Performance_Allowance_Calculated,
							 WAR_Allowance_Calculated_1, WAR_Allowance_Calculated_2, WAR_Allowance_Calculated_3, WAR_Allowance_Calculated_4, WAR_Allowance_Calculated_5, WAR_Allowance_Calculated_6, WAR_Allowance_Calculated_7, WAR_Allowance_Calculated_8, WAR_Allowance_Calculated_9, WAR_Allowance_Calculated_10), MidpointRounding.AwayFromZero);

							decimal ESICsum = Math.Round(GetAmountBasedOnFormula(
								cr.CRI_ESIC_Formula, WAR_Basic_Calculated, CRI_DA_Calculated, CRI_HRA_Calculated, CRI_LeaveAndPH_Calculated,
								cr.Client_Requirement_Allowances.ToList(), totalWorkingDays, totalPaybleDays, WAR_OverTime_Calculated,
								WAR_Outstation_Allowance_Calculated, WAR_Attendance_Allowance_Calculated, WAR_Nightshift_Allowance_Calculated,
								WAR_Performance_Allowance_Calculated, WAR_Allowance_Calculated_1, WAR_Allowance_Calculated_2, WAR_Allowance_Calculated_3, WAR_Allowance_Calculated_4, WAR_Allowance_Calculated_5, WAR_Allowance_Calculated_6, WAR_Allowance_Calculated_7, WAR_Allowance_Calculated_8, WAR_Allowance_Calculated_9, WAR_Allowance_Calculated_10), MidpointRounding.AwayFromZero);

							WAR_PF = Convert.ToDecimal(cr.CRI_PF_Percentage);
							if (cr.CRI_PF_ApplyMAX.HasValue && cr.CRI_PF_ApplyMAX.Value > 0)
							{
								if (PFsum >= cr.CRI_PF_ApplyMAX)
								{
									WAR_PF_Calculated = Math.Round(Decimal.Multiply(cr.CRI_PF_ApplyMAX.Value, WAR_PF) / 100, MidpointRounding.AwayFromZero);
								}
								else
								{
									WAR_PF_Calculated = Math.Round(Decimal.Multiply(PFsum, WAR_PF) / 100, MidpointRounding.AwayFromZero);
								}
							}
							else
							{
								WAR_PF_Calculated = Math.Round(Decimal.Multiply(PFsum, WAR_PF) / 100, MidpointRounding.AwayFromZero);
							}
							WAR_ESIC = Convert.ToDecimal(cr.CRI_ESIC_Percentage);
							WAR_ESIC_Calculated = Math.Ceiling(Decimal.Multiply(ESICsum, WAR_ESIC) / 100);
							#endregion

							wageRegisterVM.WAR_PF_Formula = cr.CRI_PF_Formula;
							wageRegisterVM.WAR_ESIC_Formula = cr.CRI_ESIC_Formula;
							wageRegisterVM.WAR_PF = WAR_PF;
							wageRegisterVM.WAR_PF_Calculated = WAR_PF_Calculated;
							wageRegisterVM.WAR_ESIC = WAR_ESIC;
							wageRegisterVM.WAR_ESIC_Calculated = WAR_ESIC_Calculated;
							wageRegisterVM.WAR_OverTime_Calculated = WAR_OverTime_Calculated;
							wageRegisterVM.WAR_OverTime_Formula = cr.CRI_OT_Formula;
							wageRegisterVM.WAR_WorkingHrs_In_Day = cli.CLI_WorkingHours_In_Day;
							wageRegisterVM.WAR_OverTime_Payment = Convert.ToInt16(cr.CRI_OT_MultipleTimes);
							wageRegisterVM.WAR_Basic_Calculated = WAR_Basic_Calculated;
							wageRegisterVM.WAR_HRA_Calculated = CRI_HRA_Calculated;
							wageRegisterVM.WAR_LeaveAndPH_Calculated = CRI_LeaveAndPH_Calculated;

						}

					}
					//************************** ALLOWANCES CALCULATION *****************************//
					List<WageRegisterAllowanceVM> allowances = new List<WageRegisterAllowanceVM>();
					decimal AllowancesTotal = 0;
					if (cr != null)
					{
						foreach (var allowance in cr.Client_Requirement_Allowances)
						{
							WageRegisterAllowanceVM all = new WageRegisterAllowanceVM();
							all.CRA_Id = allowance.CRA_Id;
							all.CRI_Id = allowance.CRI_Id;
							all.WAA_Amount = allowance.CRA_Amount;
							all.WAA_DayswiseOrFull = allowance.CRA_DayswiseOrFull;
							all.allowanceVM = AllowanceMapper.mapMe(allowance.ALL);
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
					}

					wageRegisterVM.WAR_Attendance_Allowance_Calculated = WAR_Attendance_Allowance_Calculated;
					wageRegisterVM.allowanceVMs = allowances;
					//************************** ALLOWANCES CALCULATION *****************************/
					decimal WAR_GrossTotal = Math.Round(WAR_Basic_Calculated + CRI_DA_Calculated + CRI_HRA_Calculated + CRI_LeaveAndPH_Calculated + WAR_OverTime_Calculated +
														AllowancesTotal + WAR_Outstation_Allowance_Calculated + WAR_Nightshift_Allowance_Calculated +
														WAR_Performance_Allowance_Calculated + WAR_Attendance_Allowance_Calculated +
														 WAR_Allowance_Calculated_1 + WAR_Allowance_Calculated_2 + WAR_Allowance_Calculated_3 + WAR_Allowance_Calculated_4 + WAR_Allowance_Calculated_5 + WAR_Allowance_Calculated_6 + WAR_Allowance_Calculated_7 + WAR_Allowance_Calculated_8 + WAR_Allowance_Calculated_9 + WAR_Allowance_Calculated_10, MidpointRounding.AwayFromZero);

					#region EMI Calculation

					decimal totalEMI = 0;
					if (employee.EMP.Wage_Register_Advances.Count() > 0)
					{
						bool flag = false;
						int clients = attManager.getAttendance_Wage_Employee(wageProcess.WAG_Id, employee.EMP_Id).Select(m => m.CLI_Id).Distinct().Count();
						if (clients > 1)
						{
							IEnumerable<Clients_Employee> clientList = clientsManager.listActiveClientsEmployees_clients(employee.EMP_Id, new DateTime(wageProcess.WAG_Month.Year, wageProcess.WAG_Month.Month, wageProcess.WAG_Month.Day));
							Client_Requirement requirementMax = null;
							decimal Basic_Paybaleday_Max = 0;

							foreach (Clients_Employee clients_Employee in clientList)
							{
								Client_Requirement requirement = clientsManager.getActiveClientRequirement(clients_Employee.CLI_Id, clients_Employee.DES_Id, new DateTime(wageProcess.WAG_Month.Year, wageProcess.WAG_Month.Month, wageProcess.WAG_Month.Day));
								IEnumerable<Attendance> atts = attManager.getAttendance_Wage_Client_Employee_Designation(wageProcess.WAG_Id, clients_Employee.CLI_Id, clients_Employee.EMP_Id, clients_Employee.DES_Id);

								#region payable days
								int totWeekOffs1 = 0, totalPublicHoliday1 = 0, totEarnLeave1 = 0, totNightShift1 = 0;
								double totPresentDays1 = 0, totHalfDays1 = 0, totExtraWorkingDays1 = 0, WAR_ExtraWorkingHours1 = 0, totNighthours1 = 0, totalPaybleDays1 = 0; ;

								totPresentDays1 = atts.Where(a => a.ATT_IsPresent == true).Count();
								totEarnLeave1 = atts.Where(a => a.ATT_IsEarnLeave == true).Count();
								totNightShift1 = atts.Where(a => a.ATT_NightShift == true).Count();
								totHalfDays1 = atts.Where(a => a.ATT_IsHalfday == true).Count();
								WAR_ExtraWorkingHours1 = atts.Sum(m => m.ATT_ExtraHoursWorked);
								totExtraWorkingDays1 = WAR_ExtraWorkingHours / cli.CLI_WorkingHours_In_Day;
								totWeekOffs1 = atts.Where(a => a.ATT_IsWeeklyOff == true).Count();
								totalPublicHoliday1 = atts.Where(a => a.ATT_IsPublicHoliday == true).Count();
								totNighthours1 = totNightShift * cli.CLI_WorkingHours_In_Day;
								totalPaybleDays1 = (totPresentDays1 - totHalfDays1) + (totHalfDays1 / 2) + totEarnLeave1;
								if (requirement != null)
								{
									if (requirement.CRI_IsPayable_WeeklyOff)
									{
										totalPaybleDays1 = totalPaybleDays1 + totWeekOffs1;
									}
									if (requirement.CRI_IsPayable_PublicHoliday)
									{
										totalPaybleDays1 = totalPaybleDays1 + totalPublicHoliday1;
									}
									if (requirement.CRI_OT_Calculate_Payableday)
									{
										totalPaybleDays1 = totalPaybleDays1 + totExtraWorkingDays1;
									}
								}
								#endregion

								decimal Basic_Paybaleday = Convert.ToDecimal(Convert.ToDouble(requirement.CRI_Basic) * totalPaybleDays1);

								if (Basic_Paybaleday_Max == 0)
								{
									Basic_Paybaleday_Max = Basic_Paybaleday;
									requirementMax = requirement;
								}
								else
								{
									if (Basic_Paybaleday_Max < Basic_Paybaleday)
									{
										Basic_Paybaleday_Max = Basic_Paybaleday;
										requirementMax = requirement;
									}
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
							var EMI = employee.EMP.Wage_Register_Advances.Where(m => (m.WAD_Status == false && m.WAG.WAG_Month <= wageProcess.WAG_Month && m.WAG_Id.Equals(wageProcess.WAG_Id)) || (m.WAD_Status.Equals(true) && m.WAG_Id.Equals(wageProcess.WAG_Id))).GroupBy(m => m.EMP_Id).Select(g => new
							{
								WAD_Amt = g.Sum(n => n.WAD_Amount)
							});
							totalEMI = Math.Round(EMI.Select(g => g.WAD_Amt).FirstOrDefault(), MidpointRounding.AwayFromZero);
						}
					}
					wageRegisterVM.WAR_Advance_Amount = totalEMI;
					#endregion

					#region ProfessionalTax Calculation
					decimal WAR_ProffesionalTax_Calculated = 0M, WAR_RevenueDeduction_Calculated = 0M, WAR_CanteenFacility_Calculation = 0M;
					if (cr != null)
					{
						if (cr.CRI_ProfessionalTax == true)
						{
							//ProfessionalTaxCalculationManager ptcManager = new ProfessionalTaxCalculationManager(_context);
							// WAR_ProffesionalTax_Calculated = Math.Round(ptcManager.GetPT((employee.EMP_.EMP_Gender ? "M" : "F"), WAR_GrossTotal), MidpointRounding.AwayFromZero);                           
							WAR_ProffesionalTax_Calculated = clientsManager.GetProffessionalTax(employee.EMP.EMP_Gender, WAR_GrossTotal, cr);
						}
					}

					#endregion

					#region LWF Calculation   
					if (totalPaybleDays > 0)
					{
						if (wageProcess.WAG_Month.Month == (int)Month.June || wageProcess.WAG_Month.Month == (int)Month.December)
						{
							if (!employee.DES.DES_Exclude_LWF)
							{
								if (WAR_GrossTotal < cr.CRI_MLWF_Employer_Base)
								{
									WAR_LWF_Deduction_Employer = (cr.CRI_MLWF_Employer_LThen != null ? cr.CRI_MLWF_Employer_LThen.Value : 0); //Rs.6
								}
								else if (WAR_GrossTotal >= cr.CRI_MLWF_Employer_Base)
								{
									WAR_LWF_Deduction_Employer = (cr.CRI_MLWF_Employer_GThen != null ? cr.CRI_MLWF_Employer_GThen.Value : 0); ;  //Rs.12
								}

								if (WAR_GrossTotal < cr.CRI_MLWF_Employee_Base)
								{
									WAR_LWF_Deduction_Employee = (cr.CRI_MLWF_Employee_LThen != null ? cr.CRI_MLWF_Employee_LThen.Value : 0); ; //Rs.6
								}
								else if (WAR_GrossTotal >= cr.CRI_MLWF_Employee_Base)
								{
									WAR_LWF_Deduction_Employee = (cr.CRI_MLWF_Employee_GThen != null ? cr.CRI_MLWF_Employee_GThen.Value : 0); ;  //Rs.12
								}
							}
						}
					}
					wageRegisterVM.WAR_LWF_Deduction_Employee = WAR_LWF_Deduction_Employee;
					wageRegisterVM.WAR_LWF_Deduction_Employer = WAR_LWF_Deduction_Employer;
					#endregion

					if (cr != null)
					{
						if (cr.CRI_RevenueDeduction == true)
						{
							if (totalPaybleDays > 0)
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
					}


					decimal WAR_FinalTotal = Math.Round(WAR_GrossTotal - (WAR_PF_Calculated + WAR_ESIC_Calculated + totalEMI + WAR_ProffesionalTax_Calculated + WAR_RevenueDeduction_Calculated + WAR_CanteenFacility_Calculation + WAR_LWF_Deduction_Employee), MidpointRounding.AwayFromZero);
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
				_context.Wage_Registers.AddRange(wage_Registers);

				if (!this.isWageAlredySave(WAG_Id, Convert.ToInt32(CLI_Id)))
				{
					Wage_Process_Client process_Client = new Wage_Process_Client();
					process_Client.WAG_Id = WAG_Id;
					process_Client.CLI_Id = Convert.ToInt32(CLI_Id);
					process_Client.WPC_WageRegisterSaved = true;
					process_Client.ADM_Id_SavedBy = AdminID;
					process_Client.WPC_SavedOn = ProjectUtils.DateNow();
					_context.Wage_Process_Clients.Add(process_Client);

					_context.SaveChanges();
				}

			}
			catch (Exception ex)
			{
				res = ex.Message;
			}
			return res;
		}

		public bool isWageAlredySave(int WAG_Id, int CLI_Id)
		{
			return _context.Wage_Registers.Where(m => m.WAG_Id.Equals(WAG_Id) && m.CLI_Id.Equals(CLI_Id)).Count() > 0;
		}

		public string ResetWageRegister(int WAG_Id, string CLI_Id)
		{
			string res = string.Empty;
			try
			{
				List<Wage_Register> lst = _context.Wage_Registers.Where(m => m.WAG_Id.Equals(WAG_Id) && m.CLI_Id.Equals(Convert.ToInt32(CLI_Id))).Include(m => m.Wage_Register_Allowances).ToList();
				List<Wage_Register_Allowance> wra = lst.SelectMany(m => m.Wage_Register_Allowances).ToList();
				if (wra.Count > 0)
				{
					_context.Wage_Register_Allowances.RemoveRange(wra);
				}
				_context.Wage_Registers.RemoveRange(lst);
				IEnumerable<Wage_Process_Client> wgs = _context.Wage_Process_Clients.Where(m => m.WAG_Id.Equals(WAG_Id) && m.CLI_Id.Equals(Convert.ToInt32(CLI_Id)));
				_context.Wage_Process_Clients.RemoveRange(wgs);
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
			return _context.Wage_Registers.Include(m => m.EMP).Include(m => m.CRI).ThenInclude(m => m.DES).SingleOrDefault(m => m.WAR_Id.Equals(WAR_Id)); ;
		}

		public List<Wage_Register_Allowance> GetWage_Register_Allowances(int WAR_Id)
		{
			List<Wage_Register_Allowance> wage_Register_Allowances = new List<Wage_Register_Allowance>();
			wage_Register_Allowances = _context.Wage_Register_Allowances.Include(m => m.CRA).ThenInclude(m => m.ALL).Where(m => m.WAR_Id.Equals(WAR_Id)).ToList();
			return wage_Register_Allowances;
		}

		public string UpdateWageRegister(Wage_Register wage_Reg)
		{
			string res = string.Empty;
			try
			{
				wage_Reg.WAR_LastModifiedOn = DateNow();
				_context.Entry(wage_Reg).State = EntityState.Modified;
				_context.SaveChanges();
			}
			catch (Exception ex)
			{
				res = ex.Message;
			}
			return res;
		}

		public List<Wage_Register_Advance> GetWageRegisterAdvances(int WAG_Id)
		{
			List<Wage_Register_Advance> wage_Register_Advances = new List<Wage_Register_Advance>();
			wage_Register_Advances = _context.Wage_Register_Advances.Where(m => m.WAG_Id.Equals(WAG_Id)).Include(m => m.EMP)
				.ToList();

			return wage_Register_Advances;
		}
		public List<Wage_Register_Advance> GetWageRegisterAdvances()
		{
			List<Wage_Register_Advance> wage_Register_Advances = new List<Wage_Register_Advance>();
			wage_Register_Advances = _context.Wage_Register_Advances.Include(m => m.WAG).Include(m => m.EMP)
				.ToList();

			return wage_Register_Advances;
		}

		public List<Wage_Register> GetWageRegistersByEmpId(int WAG_Id, int EMP_Id)
		{
			return _context.Wage_Registers.Where(r => r.WAG_Id == WAG_Id && r.EMP_Id == EMP_Id).Include(m => m.EMP).ThenInclude(m => m.Employee_Advances).Include(n => n.EMP).ThenInclude(n => n.Wage_Register_Advances).Include(m => m.CRI).ThenInclude(m => m.DES).Include(n => n.Wage_Register_Allowances).ThenInclude(n => n.CRA).ThenInclude(n => n.ALL).ToList();
		}

		public List<Wage_Register> GetWageRegistersForIDBI_To_Other(int WAG_Id, int[] CLI_Ids)
		{
			return _context.Wage_Registers.Where(m => m.WAG_Id == WAG_Id && CLI_Ids.Contains(m.CLI_Id) && m.EMP.EMP_Payment_Type.Equals((int)PAYMENT_TYPE.Bank_Account) && (m.EMP.CBA.CBA_Bank == ProjectUtils.GetStringValue(REGISTER_BANK.IDBI_BANK_LTD) && m.EMP.EMP_Bank != ProjectUtils.GetStringValue(REGISTER_BANK.IDBI_BANK_LTD)) && m.WAR_FinalTotal > 0).Include(m => m.CLI).Include(m => m.EMP).ToList();
		}

		public List<Wage_Register> GetWageRegistersForICICI_360(int WAG_Id, int[] CLI_Ids)
		{
			return _context.Wage_Registers.Where(m => m.WAG_Id == WAG_Id && CLI_Ids.Contains(m.CLI_Id) && m.EMP.EMP_Payment_Type.Equals((int)PAYMENT_TYPE.Bank_Account) && (m.EMP.CBA.CBA_Bank == ProjectUtils.GetStringValue(REGISTER_BANK.ICICI_BANK_LTD)) && m.WAR_FinalTotal > 0).Include(m => m.CLI).Include(m => m.EMP).Include(m => m.EMP.CBA).ToList();
		}
		public List<Wage_Register> GetWageRegistersForICICI_ADHOC(int WAG_Id, int[] CLI_Ids)
		{
			return _context.Wage_Registers.Where(m => m.WAG_Id == WAG_Id && CLI_Ids.Contains(m.CLI_Id) && m.EMP.EMP_Payment_Type.Equals((int)PAYMENT_TYPE.Bank_Account) && (m.EMP.CBA.CBA_Bank == ProjectUtils.GetStringValue(REGISTER_BANK.ICICI_BANK_LTD) && m.EMP.EMP_Bank != ProjectUtils.GetStringValue(REGISTER_BANK.ICICI_BANK_LTD)) && m.WAR_FinalTotal > 0).Include(m => m.CLI).Include(m => m.EMP).Include(m => m.EMP.CBA).ToList();
		}

		public List<Wage_Register> GetWageRegistersForHDFC_To_HDFC(int WAG_Id, int[] CLI_Ids)
		{
			return _context.Wage_Registers.Where(m => m.WAG_Id == WAG_Id && CLI_Ids.Contains(m.CLI_Id) && m.EMP.EMP_Payment_Type.Equals((int)PAYMENT_TYPE.Bank_Account) && (m.EMP.CBA.CBA_Bank == ProjectUtils.GetStringValue(REGISTER_BANK.HDFC_BANK_LTD) && m.EMP.EMP_Bank == ProjectUtils.GetStringValue(REGISTER_BANK.HDFC_BANK_LTD)) && m.WAR_FinalTotal > 0).Include(m => m.CLI).Include(m => m.EMP).ToList();
		}

		public List<Wage_Register> GetWageRegistersForHDFC_To_Other(int WAG_Id, int[] CLI_Ids)
		{
			return _context.Wage_Registers.Where(m => m.WAG_Id == WAG_Id && CLI_Ids.Contains(m.CLI_Id) && m.EMP.EMP_Payment_Type.Equals((int)PAYMENT_TYPE.Bank_Account) && (m.EMP.CBA.CBA_Bank == ProjectUtils.GetStringValue(REGISTER_BANK.HDFC_BANK_LTD) && m.EMP.EMP_Bank != ProjectUtils.GetStringValue(REGISTER_BANK.HDFC_BANK_LTD)) && m.WAR_FinalTotal > 0).Include(m => m.CLI).Include(m => m.EMP).ToList();
		}
		public List<Wage_Register> GetWageRegistersForChequeCash(int WAG_Id, int[] CLI_Ids)
		{
			return _context.Wage_Registers.Where(m => m.WAG_Id == WAG_Id && CLI_Ids.Contains(m.CLI_Id) && m.EMP.EMP_Payment_Type.Equals((int)PAYMENT_TYPE.Cheque_Cash) && m.WAR_FinalTotal > 0).Include(m => m.CLI).Include(m => m.EMP).ToList();
		}

		public List<Wage_Register> GetWageRegistersForInvoice(int CLI_Id)
		{
			return _context.Wage_Registers.Include(m => m.EMP).ThenInclude(m => m.Clients_Employees).Include(m => m.WAG).Include(m => m.CRI).ThenInclude(m => m.DES).Include(n => n.Wage_Register_Allowances).ThenInclude(n => n.CRA).ThenInclude(n => n.ALL).Where(r => r.CLI_Id == CLI_Id).ToList();
		}

		public int GetClient_EmployeeHavingMaxBasic(int EMP_Id, int WAG_Id)
		{
			int CLI_Id = 0;
			return CLI_Id;
		}

		public List<Wage_Register> GetWageRegistersForSalarySlip(int WAG_Id, int EMP_Id)
		{
			return _context.Wage_Registers.Where(r => r.WAG_Id == WAG_Id && r.EMP_Id == EMP_Id)
				.Include(m => m.CLI).ThenInclude(m => m.STA)
				.Include(m => m.WAG).ThenInclude(m => m.FRM).ThenInclude(m => m.STA)
				.Include(m => m.WAG)
				.Include(m => m.CRI).ThenInclude(m => m.DES)
				.Include(m => m.EMP).ThenInclude(m => m.Employee_Advances)
				.Include(n => n.EMP).ThenInclude(n => n.Wage_Register_Advances)
				.Include(m => m.CRI).ThenInclude(m => m.DES)
				.Include(n => n.Wage_Register_Allowances).ThenInclude(n => n.CRA).ThenInclude(n => n.ALL)
				.Include(m => m.EMP).ThenInclude(m => m.EMP_StateNavigation)
				.Include(m => m.CRI).ThenInclude(m => m.Client_Requirement_Allowances).ToList();
		}
		public List<Employee> GetEmployeesForSalarySlip(int WAG_Id)
		{
			return _context.Wage_Registers.Where(r => r.WAG_Id == WAG_Id).Select(m => m.EMP).ToList();
		}

		public IEnumerable<Wage_Register> GetWageFrom_WAG_Id_EMP_Id(int WAG_Id, int EMP_Id)
		{
			return _context.Wage_Registers.Where(m => m.WAG_Id.Equals(WAG_Id) && m.EMP_Id.Equals(EMP_Id)).Include(m => m.CLI);
		}

		public List<Employee> GetEmployeesForWage(int CLI_Id, int WAG_Id)
		{
			int[] EMP_Ids = _context.Wage_Registers.Where(m => m.CLI_Id.Equals(CLI_Id) && m.WAG_Id.Equals(WAG_Id)).Select(m => m.EMP_Id).ToArray();
			return _context.Employees.Where(m => EMP_Ids.Contains(m.EMP_Id)).Include(m => m.Wage_PaySlips).AsSplitQuery().ToList();
		}


		public IEnumerable<Client> GetDistinctClientsForWage(int WAG_Id)
		{
			return _context.Wage_Registers.Include(m => m.CLI).Where(m => m.WAG_Id.Equals(WAG_Id)).Select(m => m.CLI).Distinct();
		}

		public Client GetClientForWage(int WAG_Id, int CLI_Id)
		{
			return _context.Wage_Registers.Include(m => m.CLI).Where(m => m.WAG_Id.Equals(WAG_Id) && m.CLI_Id == CLI_Id).Select(m => m.CLI).FirstOrDefault();
		}

		#region NEW ADDED

		public List<ClientWageRegisterVM> GenerateWageClientsRegisterTable(int WAG_Id, int AdminID, int FRM_Id)
		{
			List<ClientWageRegisterVM> lst = [];
			ClientsManager clientsManager = new(_context, _configuration);
			WageProcessManager wageManager = new(_context);
			Wage_Process wageProcess = wageManager.GetWageProcessByWAG_Id(WAG_Id, true, true, true, true);
			List<Client> clients = clientsManager.GetActiveClientsOfMonthByFirmId(new DateTime(wageProcess.WAG_Month.Year, wageProcess.WAG_Month.Month, wageProcess.WAG_Month.Day), FRM_Id);

			foreach (Client client in clients)
			{
				Wage_Process_Client wage_Process_Clients = wageManager.GetWage_Process_Clients(WAG_Id, client.CLI_Id);

				List<WageRegisterVM> lstRegister;
				if (wage_Process_Clients != null)
					lstRegister = WageRegisterMapper.mapWageRegisters(GetWageRegisters(WAG_Id, client.CLI_Id));
				else
					lstRegister = GetWageClientRegisterCalculated(wageProcess, client.CLI_Id, AdminID);

				ClientWageRegisterVM clientWageRegisterVM = new()
				{
					wageProcessClientVM = wage_Process_Clients != null ? WageProcessClientsMapper.MapMe(wage_Process_Clients) : null,
					client = ClientMapper.MapMe(client),
					wageProcessVM = WageProcessMapper.MapMe2(wageProcess),
					wageRegisterVMs = lstRegister
				};

				lst.Add(clientWageRegisterVM);
			}
			return lst;
		}

		public ClientWageRegisterVM GenerateWageClientRegisterTable(int WAG_Id, int AdminID, int FRM_Id, int CLI_Id)
		{
			ClientWageRegisterVM table = new();
			ClientsManager clientsManager = new(_context, _configuration);
			WageProcessManager wageManager = new(_context);
			Wage_Process wageProcess = wageManager.GetWageProcessByWAG_Id(WAG_Id, true, true, true, true);
			Client client = clientsManager.GetActiveClientOfMonthByFRM_Id(new DateTime(wageProcess.WAG_Month.Year, wageProcess.WAG_Month.Month, wageProcess.WAG_Month.Day), FRM_Id, CLI_Id);

			Wage_Process_Client wage_Process_Clients = wageManager.GetWage_Process_Clients(WAG_Id, client.CLI_Id);

			List<WageRegisterVM> lstRegister;
			if (wage_Process_Clients != null)
				lstRegister = WageRegisterMapper.mapWageRegisters(GetWageRegisters(WAG_Id, client.CLI_Id));
			else
				lstRegister = GetWageClientRegisterCalculated(wageProcess, client.CLI_Id, AdminID);

			table.wageProcessClientVM = wage_Process_Clients != null ? WageProcessClientsMapper.MapMe(wage_Process_Clients) : null;
			table.client = ClientMapper.MapMe(client);
			table.wageProcessVM = WageProcessMapper.MapMe2(wageProcess);
			table.wageRegisterVMs = lstRegister;

			return table;
		}

		public List<WageRegisterVM> GetWageClientRegisterCalculated(Wage_Process wageProcess, int CLI_Id, int AdminID)
		{
			List<WageRegisterVM> lstRegister = [];
			ClientsManager clientsManager = new(_context, _configuration);
			AttendanceSummaryManager attManager = new(_context);
			CanteenManager canteenManager = new(_context);
			OutstationManager outstationManager = new(_context);
			PerformanceManager performanceManager = new(_context);
			AllowanceManager allowanceManager = new(_context);
			IEnumerable<Clients_Employee> clientsEmployees = clientsManager.listActiveClientsEmployees(CLI_Id, new DateTime(wageProcess.WAG_Month.Year, wageProcess.WAG_Month.Month, wageProcess.WAG_Month.Day));
			foreach (Clients_Employee employee in clientsEmployees)
			{
				Attendance_Summary attSummary = attManager.GetAttendance_Wage_Client_Employee(wageProcess.WAG_Id, CLI_Id, employee.EMP_Id);
				if (attSummary != null)
				{
					Client_Requirement cr = clientsManager.getActiveClientRequirement(CLI_Id, employee.DES.DES_Id, new DateTime(wageProcess.WAG_Month.Year, wageProcess.WAG_Month.Month, wageProcess.WAG_Month.Day));

					Client client = clientsManager.GetClientById(CLI_Id);
					Attendance_Parameter att_Param = clientsManager.GetAttendanceParameterByMonth(client.CLI_Id, ProjectUtils.DateToDateTime(wageProcess.WAG_Month));
					DateTime[] Period = ProjectUtils.GetStartEndDatePeriodForAttendance(att_Param, ProjectUtils.DateToDateTime(wageProcess.WAG_Month));

					int totalWorkingDays = 0;
					double totalPaybleDays = 0;
					decimal CRI_Basic = 0M, WAR_Basic_Calculated = 0M, BasicDa = 0M, WAR_OverTime_Calculated = 0M, WAR_PF, WAR_PF_Calculated = 0M, WAR_ESIC = 0M, WAR_ESIC_Calculated = 0M, WAR_LWF_Deduction_Employer = 0M, WAR_LWF_Deduction_Employee = 0M;
					decimal CRI_DA = 0M, CRI_DA_Calculated = 0M, CRI_HRA = 0M, CRI_LeaveAndPH = 0M, CRI_HRA_Calculated = 0M, CRI_LeaveAndPH_Calculated = 0M;
					decimal WAR_Attendance_Allowance_Calculated = 0M,
						WAR_Outstation_Allowance_Calculated = 0M,
						WAR_Performance_Allowance_Calculated = 0M,
						WAR_Nightshift_Allowance_Calculated = 0M,
						WAR_Allowance_Calculated_1 = 0M, WAR_Allowance_Calculated_2 = 0M, WAR_Allowance_Calculated_3 = 0M, WAR_Allowance_Calculated_4 = 0M, WAR_Allowance_Calculated_5 = 0M, WAR_Allowance_Calculated_6 = 0M, WAR_Allowance_Calculated_7 = 0M, WAR_Allowance_Calculated_8 = 0M, WAR_Allowance_Calculated_9 = 0M, WAR_Allowance_Calculated_10 = 0M;

					#region Total working day calculation                    
					int Totaldays = (int)(Period[1] - Period[0]).TotalDays;
					switch (client.CLI_Total_WorkingDays)
					{
						case 0: //  0:Consider_RealDays
							totalWorkingDays = Totaldays;
							break;
						case 1://   1:Excluding_WeeklyOff
							totalWorkingDays = Totaldays - (int)attSummary.ATS_WeekOff;
							break;
						case 2://   2:Consider StaticDays 
							totalWorkingDays = client.CLI_No_Reduce_Days.Value;
							break;
						default: break;
					}
					#endregion

					#region Paybale day counting

					int totWeekOffs = 0, totalPublicHoliday = 0, totEarnLeave = 0, totNightShift = 0, totAbsent = 0;
					double totPresentDays = 0, totHalfDays = 0, totExtraWorkingDays = 0, WAR_ExtraWorkingHours = 0;// totNighthours = 0;

					totPresentDays = attSummary.ATS_PresentDays;
					totEarnLeave = (int)attSummary.ATS_EarnLeaves;
					totNightShift = (int)attSummary.ATS_NightShifts;
					totHalfDays = attSummary.ATS_HalfDays;
					// totHalfDays = attendances.Where(a => a.ATT_IsHalfday == true).Count();

					WAR_ExtraWorkingHours = attSummary.ATS_ExtraHours;
					totExtraWorkingDays = WAR_ExtraWorkingHours / client.CLI_WorkingHours_In_Day;

					totWeekOffs = (int)attSummary.ATS_WeekOff;
					totalPublicHoliday = (int)attSummary.ATS_PublicHolidays;
					// totNighthours = totNightShift * cli.CLI_WorkingHours_In_Day;

					totalPaybleDays = (totPresentDays - totHalfDays) + (totHalfDays / 2) + totEarnLeave;

					if (cr != null)
					{
						if (cr.CRI_IsPayable_WeeklyOff)
						{
							totalPaybleDays += totWeekOffs;
						}
						if (cr.CRI_IsPayable_PublicHoliday)
						{
							totalPaybleDays += totalPublicHoliday;
						}
						if (cr.CRI_OT_Calculate_Payableday)
						{
							totalPaybleDays = totalPaybleDays + totExtraWorkingDays;
						}
					}

					//totAbsent = attendances.Where(m => m.ATT_IsPresent.Equals(false) && m.ATT_IsEarnLeave.Equals(false) && m.ATT_IsPublicHoliday.Equals(false) && m.ATT_IsWeeklyOff.Equals(false)).Count();

					totAbsent = Totaldays - (int)(attSummary.ATS_WeekOff + attSummary.ATS_PublicHolidays + attSummary.ATS_EarnLeaves + attSummary.ATS_PresentDays);
					#endregion

					WageRegisterVM wageRegisterVM = new()
					{
						CLE_Id = employee.CLE_Id,
						WAG_Id = wageProcess.WAG_Id,
						CLI_Id = CLI_Id,
						EMP_Id = employee.EMP_Id,
						//  wageRegisterVM.wageProcessVM = WageProcessMapper.mapMe(wageProcess);
						employeeVM = EmployeesMapper.MapMe(employee.EMP),
						designation = employee.DES
					};

					#region New Allowances 
					if (cr != null)
					{
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
							// WAR_Nightshift_Allowance_Calculated = Convert.ToDecimal(Convert.ToDouble(cr.CRI_Nightshift_Allowance_Rate.Value) * totNighthours);
							WAR_Nightshift_Allowance_Calculated = Convert.ToDecimal(Convert.ToDouble(cr.CRI_Nightshift_Allowance_Rate.Value) * totNightShift);
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
						if (cr.CRI_Allowance_1)
						{
							Wage_Register_Allowances_1 all_1 = allowanceManager.GetAllowances_ByCLE_1(wageProcess.WAG_Id, employee.CLE_Id, employee.CLI_Id);
							if (all_1 != null)
							{
								WAR_Allowance_Calculated_1 = all_1.WRA_Amount_1;
								wageRegisterVM.WRA_Id_1 = all_1.WRA_Id_1;
								wageRegisterVM.WAR_Allowance_Calculated_1 = WAR_Allowance_Calculated_1;
							}
							else
							{
								wageRegisterVM.WAR_Allowance_Calculated_1 = 0;
							}
						}
						if (cr.CRI_Allowance_2)
						{
							Wage_Register_Allowances_2 all_2 = allowanceManager.GetAllowances_ByCLE_2(wageProcess.WAG_Id, employee.CLE_Id, employee.CLI_Id);
							if (all_2 != null)
							{
								WAR_Allowance_Calculated_2 = all_2.WRA_Amount_2;
								wageRegisterVM.WRA_Id_2 = all_2.WRA_Id_2;
								wageRegisterVM.WAR_Allowance_Calculated_2 = WAR_Allowance_Calculated_2;
							}
							else
							{
								wageRegisterVM.WAR_Allowance_Calculated_2 = 0;
							}
						}
						if (cr.CRI_Allowance_3)
						{
							Wage_Register_Allowances_3 all_3 = allowanceManager.GetAllowances_ByCLE_3(wageProcess.WAG_Id, employee.CLE_Id, employee.CLI_Id);
							if (all_3 != null)
							{
								WAR_Allowance_Calculated_3 = all_3.WRA_Amount_3;
								wageRegisterVM.WRA_Id_3 = all_3.WRA_Id_3;
								wageRegisterVM.WAR_Allowance_Calculated_3 = WAR_Allowance_Calculated_3;
							}
							else
							{
								wageRegisterVM.WAR_Allowance_Calculated_3 = 0;
							}
						}
						if (cr.CRI_Allowance_4)
						{
							Wage_Register_Allowances_4 all_4 = allowanceManager.GetAllowances_ByCLE_4(wageProcess.WAG_Id, employee.CLE_Id, employee.CLI_Id);
							if (all_4 != null)
							{
								WAR_Allowance_Calculated_4 = all_4.WRA_Amount_4;
								wageRegisterVM.WRA_Id_4 = all_4.WRA_Id_4;
								wageRegisterVM.WAR_Allowance_Calculated_4 = WAR_Allowance_Calculated_4;
							}
							else
							{
								wageRegisterVM.WAR_Allowance_Calculated_4 = 0;
							}
						}
						if (cr.CRI_Allowance_5)
						{
							Wage_Register_Allowances_5 all_5 = allowanceManager.GetAllowances_ByCLE_5(wageProcess.WAG_Id, employee.CLE_Id, employee.CLI_Id);
							if (all_5 != null)
							{
								WAR_Allowance_Calculated_5 = all_5.WRA_Amount_5;
								wageRegisterVM.WRA_Id_5 = all_5.WRA_Id_5;
								wageRegisterVM.WAR_Allowance_Calculated_5 = WAR_Allowance_Calculated_5;
							}
							else
							{
								wageRegisterVM.WAR_Allowance_Calculated_5 = 0;
							}
						}
						if (cr.CRI_Allowance_6)
						{
							Wage_Register_Allowances_6 all_6 = allowanceManager.GetAllowances_ByCLE_6(wageProcess.WAG_Id, employee.CLE_Id, employee.CLI_Id);
							if (all_6 != null)
							{
								WAR_Allowance_Calculated_6 = all_6.WRA_Amount_6;
								wageRegisterVM.WRA_Id_6 = all_6.WRA_Id_6;
								wageRegisterVM.WAR_Allowance_Calculated_6 = WAR_Allowance_Calculated_6;
							}
							else
							{
								wageRegisterVM.WAR_Allowance_Calculated_6 = 0;
							}
						}
						if (cr.CRI_Allowance_7)
						{
							Wage_Register_Allowances_7 all_7 = allowanceManager.GetAllowances_ByCLE_7(wageProcess.WAG_Id, employee.CLE_Id, employee.CLI_Id);
							if (all_7 != null)
							{
								WAR_Allowance_Calculated_7 = all_7.WRA_Amount_7;
								wageRegisterVM.WRA_Id_7 = all_7.WRA_Id_7;
								wageRegisterVM.WAR_Allowance_Calculated_7 = WAR_Allowance_Calculated_7;
							}
							else
							{
								wageRegisterVM.WAR_Allowance_Calculated_7 = 0;
							}
						}
						if (cr.CRI_Allowance_8)
						{
							Wage_Register_Allowances_8 all_8 = allowanceManager.GetAllowances_ByCLE_8(wageProcess.WAG_Id, employee.CLE_Id, employee.CLI_Id);
							if (all_8 != null)
							{
								WAR_Allowance_Calculated_8 = all_8.WRA_Amount_8;
								wageRegisterVM.WRA_Id_8 = all_8.WRA_Id_8;
								wageRegisterVM.WAR_Allowance_Calculated_8 = WAR_Allowance_Calculated_8;
							}
							else
							{
								wageRegisterVM.WAR_Allowance_Calculated_8 = 0;
							}
						}
						if (cr.CRI_Allowance_9)
						{
							Wage_Register_Allowances_9 all_9 = allowanceManager.GetAllowances_ByCLE_9(wageProcess.WAG_Id, employee.CLE_Id, employee.CLI_Id);
							if (all_9 != null)
							{
								WAR_Allowance_Calculated_9 = all_9.WRA_Amount_9;
								wageRegisterVM.WRA_Id_9 = all_9.WRA_Id_9;
								wageRegisterVM.WAR_Allowance_Calculated_9 = WAR_Allowance_Calculated_9;
							}
							else
							{
								wageRegisterVM.WAR_Allowance_Calculated_9 = 0;
							}
						}
						if (cr.CRI_Allowance_10)
						{
							Wage_Register_Allowances_10 all_10 = allowanceManager.GetAllowances_ByCLE_10(wageProcess.WAG_Id, employee.CLE_Id, employee.CLI_Id);
							if (all_10 != null)
							{
								WAR_Allowance_Calculated_10 = all_10.WRA_Amount_10;
								wageRegisterVM.WRA_Id_10 = all_10.WRA_Id_10;
								wageRegisterVM.WAR_Allowance_Calculated_10 = WAR_Allowance_Calculated_10;
							}
							else
							{
								wageRegisterVM.WAR_Allowance_Calculated_10 = 0;
							}
						}

					}

					#endregion

					if (cr != null)
					{
						wageRegisterVM.clientRequirementVM = ClientRequirementMapper.mapMe(cr);
					}
					else
					{
						wageRegisterVM.clientRequirementVM = new ClientRequirementVM();
					}

					wageRegisterVM.WAR_TotalWorkingDays = totalWorkingDays;
					wageRegisterVM.WAR_TotalPaybleDays = totalPaybleDays;
					wageRegisterVM.WAR_ExtraWorkingHours = WAR_ExtraWorkingHours;

					if (cr != null)
					{
						CRI_Basic = Math.Round(cr.CRI_Basic.Value, MidpointRounding.AwayFromZero);
						CRI_DA = Math.Round(Convert.ToDecimal(cr.CRI_DA), MidpointRounding.AwayFromZero);
						BasicDa = Math.Round(Decimal.Add(CRI_Basic, Convert.ToDecimal(cr.CRI_DA)), MidpointRounding.AwayFromZero);
						CRI_HRA = Math.Round(cr.CRI_HRA_Fixed == null ? Convert.ToDecimal(cr.CRI_HRA_Percentage.Value) : cr.CRI_HRA_Fixed.Value, MidpointRounding.AwayFromZero);
						CRI_LeaveAndPH = Math.Round(cr.CRI_LeaveAndPH_Fixed == null ? Convert.ToDecimal(cr.CRI_LeaveAndPH_Percentage.Value) : cr.CRI_LeaveAndPH_Fixed.Value, MidpointRounding.AwayFromZero);
					}
					wageRegisterVM.WAR_Basic = CRI_Basic;
					wageRegisterVM.WAR_HRA = CRI_HRA;
					wageRegisterVM.WAR_LeaveAndPH = CRI_LeaveAndPH;
					wageRegisterVM.WAR_LastModifiedOn = ProjectUtils.DateNow();

					if (totalWorkingDays > 0)
					{
						double OvertimeInDay = WAR_ExtraWorkingHours / Convert.ToDouble(client.CLI_WorkingHours_In_Day);

						CRI_DA_Calculated = Math.Round(Math.Round(CRI_DA * Convert.ToDecimal(totalPaybleDays), MidpointRounding.AwayFromZero) / totalWorkingDays, MidpointRounding.AwayFromZero);
						WAR_Basic_Calculated = Math.Round(Math.Round((Decimal.Multiply(CRI_Basic, Convert.ToDecimal(totalPaybleDays))) / totalWorkingDays, MidpointRounding.AwayFromZero), MidpointRounding.AwayFromZero);
						decimal CalculatedBasicDa = Math.Round(Decimal.Add(WAR_Basic_Calculated, Convert.ToDecimal(CRI_DA_Calculated)), MidpointRounding.AwayFromZero);
						if (cr != null)
						{
							CRI_HRA_Calculated = (cr.CRI_HRA_Fixed == null ? Math.Round((Math.Round(CalculatedBasicDa * Convert.ToDecimal(cr.CRI_HRA_Percentage))) / 100, MidpointRounding.AwayFromZero) : Math.Round((Decimal.Multiply(cr.CRI_HRA_Fixed.Value, Convert.ToDecimal(totalPaybleDays))) / totalWorkingDays, MidpointRounding.AwayFromZero));
							CRI_LeaveAndPH_Calculated = (cr.CRI_LeaveAndPH_Fixed == null ? Math.Round((Math.Round(CalculatedBasicDa * Convert.ToDecimal(cr.CRI_LeaveAndPH_Percentage))) / 100, MidpointRounding.AwayFromZero) : Math.Round((Decimal.Multiply(cr.CRI_LeaveAndPH_Fixed.Value, Convert.ToDecimal(totalPaybleDays))) / totalWorkingDays, MidpointRounding.AwayFromZero));
						}

						#region START PF-ESIC -OT CALCULATION  
						if (OvertimeInDay > 0)
						{
							if (cr != null)
							{
								if (!cr.CRI_OT_Calculate_Payableday)
								{
									if (cr.CRI_OT_Fixed_PerHour > 0)
									{
										WAR_OverTime_Calculated = Convert.ToDecimal(WAR_ExtraWorkingHours) * cr.CRI_OT_Fixed_PerHour.Value;
									}
									else if (cr.CRI_OT_Formula != null)
									{
										decimal OTsum = Math.Round(GetAmountBasedOnFormulaOT(
											cr.CRI_OT_Formula,
											WAR_Basic_Calculated,
											CRI_DA_Calculated,
											CRI_HRA_Calculated,
											CRI_LeaveAndPH_Calculated,
											cr.Client_Requirement_Allowances.ToList(),
											totalWorkingDays,
											totalPaybleDays,
											WAR_Outstation_Allowance_Calculated,
											WAR_Attendance_Allowance_Calculated,
											WAR_Nightshift_Allowance_Calculated,
											WAR_Performance_Allowance_Calculated,
											WAR_Allowance_Calculated_1, WAR_Allowance_Calculated_2, WAR_Allowance_Calculated_3, WAR_Allowance_Calculated_4, WAR_Allowance_Calculated_5, WAR_Allowance_Calculated_6, WAR_Allowance_Calculated_7, WAR_Allowance_Calculated_8, WAR_Allowance_Calculated_9, WAR_Allowance_Calculated_10), MidpointRounding.AwayFromZero);

										//decimal OTsum = Math.Round(GetAmountBasedOnFormulaOT(
										//    cr.CRI_OT_Formula,
										//    Convert.ToDecimal(cr.CRI_Basic),
										//    Convert.ToDecimal(cr.CRI_DA),
										//    cr.CRI_HRA_Fixed != null ? cr.CRI_HRA_Fixed.Value : ((Convert.ToDecimal(cr.CRI_Basic) + Convert.ToDecimal(cr.CRI_DA)) * Convert.ToDecimal(cr.CRI_HRA_Percentage) / 100),
										//    cr.Client_Requirement_Allowances.ToList(),
										//    totalWorkingDays,
										//    totalPaybleDays,
										//    WAR_Outstation_Allowance_Calculated,
										//    WAR_Attendance_Allowance_Calculated,
										//    WAR_Nightshift_Allowance_Calculated,
										//    WAR_Performance_Allowance_Calculated,
										//    WAR_Allowance_Calculated_1, WAR_Allowance_Calculated_2, WAR_Allowance_Calculated_3, WAR_Allowance_Calculated_4, WAR_Allowance_Calculated_5
										//    ), MidpointRounding.AwayFromZero);

										WAR_OverTime_Calculated = Math.Round(Convert.ToDecimal(((Convert.ToDouble(OTsum) / totalPaybleDays) * OvertimeInDay) * cr.CRI_OT_MultipleTimes), MidpointRounding.AwayFromZero);
									}
								}
							}

						}
						if (cr != null)
						{
							decimal PFsum = Math.Round(GetAmountBasedOnFormula(
							cr.CRI_PF_Formula, WAR_Basic_Calculated, CRI_DA_Calculated, CRI_HRA_Calculated, CRI_LeaveAndPH_Calculated,
							cr.Client_Requirement_Allowances.ToList(), totalWorkingDays, totalPaybleDays,
							WAR_OverTime_Calculated, WAR_Outstation_Allowance_Calculated, WAR_Attendance_Allowance_Calculated,
							WAR_Nightshift_Allowance_Calculated, WAR_Performance_Allowance_Calculated, WAR_Allowance_Calculated_1, WAR_Allowance_Calculated_2, WAR_Allowance_Calculated_3, WAR_Allowance_Calculated_4, WAR_Allowance_Calculated_5, WAR_Allowance_Calculated_6, WAR_Allowance_Calculated_7, WAR_Allowance_Calculated_8, WAR_Allowance_Calculated_9, WAR_Allowance_Calculated_10), MidpointRounding.AwayFromZero);

							decimal ESICsum = Math.Round(GetAmountBasedOnFormula(
								cr.CRI_ESIC_Formula, WAR_Basic_Calculated, CRI_DA_Calculated, CRI_HRA_Calculated, CRI_LeaveAndPH_Calculated,
								cr.Client_Requirement_Allowances.ToList(), totalWorkingDays, totalPaybleDays, WAR_OverTime_Calculated,
								WAR_Outstation_Allowance_Calculated, WAR_Attendance_Allowance_Calculated, WAR_Nightshift_Allowance_Calculated,
								WAR_Performance_Allowance_Calculated, WAR_Allowance_Calculated_1, WAR_Allowance_Calculated_2, WAR_Allowance_Calculated_3, WAR_Allowance_Calculated_4, WAR_Allowance_Calculated_5, WAR_Allowance_Calculated_6, WAR_Allowance_Calculated_7, WAR_Allowance_Calculated_8, WAR_Allowance_Calculated_9, WAR_Allowance_Calculated_10), MidpointRounding.AwayFromZero);

							WAR_PF = Convert.ToDecimal(cr.CRI_PF_Percentage);
							if (cr.CRI_PF_ApplyMAX.HasValue && cr.CRI_PF_ApplyMAX.Value > 0)
							{
								if(PFsum >= cr.CRI_PF_ApplyMAX)
								{
									WAR_PF_Calculated = Math.Round(Decimal.Multiply(cr.CRI_PF_ApplyMAX.Value, WAR_PF) / 100, MidpointRounding.AwayFromZero);
								}
								else
								{
									WAR_PF_Calculated = Math.Round(Decimal.Multiply(PFsum, WAR_PF) / 100, MidpointRounding.AwayFromZero);
								}
									
							}
							else
							{
								WAR_PF_Calculated = Math.Round(Decimal.Multiply(PFsum, WAR_PF) / 100, MidpointRounding.AwayFromZero);
							}
							WAR_ESIC = Convert.ToDecimal(cr.CRI_ESIC_Percentage);
							WAR_ESIC_Calculated = Math.Ceiling(Decimal.Multiply(ESICsum, WAR_ESIC) / 100);
							#endregion
							wageRegisterVM.WAR_PF_Formula = cr.CRI_PF_Formula;
							wageRegisterVM.WAR_ESIC_Formula = cr.CRI_ESIC_Formula;
							wageRegisterVM.WAR_PF = WAR_PF;
							wageRegisterVM.WAR_PF_Calculated = WAR_PF_Calculated;
							wageRegisterVM.WAR_ESIC = WAR_ESIC;
							wageRegisterVM.WAR_ESIC_Calculated = WAR_ESIC_Calculated;
							wageRegisterVM.WAR_OverTime_Calculated = WAR_OverTime_Calculated;
							wageRegisterVM.WAR_OverTime_Formula = cr.CRI_OT_Formula;
							wageRegisterVM.WAR_WorkingHrs_In_Day = client.CLI_WorkingHours_In_Day;
							wageRegisterVM.WAR_OverTime_Payment = Convert.ToInt16(cr.CRI_OT_MultipleTimes);
							wageRegisterVM.WAR_Basic_Calculated = WAR_Basic_Calculated;
							wageRegisterVM.WAR_HRA_Calculated = CRI_HRA_Calculated;
							wageRegisterVM.WAR_LeaveAndPH_Calculated = CRI_LeaveAndPH_Calculated;
						}
					}
					//************************** ALLOWANCES CALCULATION *****************************//
					List<WageRegisterAllowanceVM> allowances = new List<WageRegisterAllowanceVM>();
					decimal AllowancesTotal = 0;
					if (cr != null)
					{
						foreach (var allowance in cr.Client_Requirement_Allowances)
						{
							WageRegisterAllowanceVM all = new WageRegisterAllowanceVM();
							all.CRA_Id = allowance.CRA_Id;
							all.CRI_Id = allowance.CRI_Id;
							all.WAA_Amount = allowance.CRA_Amount;
							all.WAA_DayswiseOrFull = allowance.CRA_DayswiseOrFull;
							all.allowanceVM = AllowanceMapper.mapMe(allowance.ALL);
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
					}

					wageRegisterVM.WAR_Attendance_Allowance_Calculated = WAR_Attendance_Allowance_Calculated;
					wageRegisterVM.allowanceVMs = allowances;

					//************************** ALLOWANCES CALCULATION *****************************/
					decimal WAR_GrossTotal = Math.Round(WAR_Basic_Calculated + CRI_DA_Calculated + CRI_HRA_Calculated + CRI_LeaveAndPH_Calculated + WAR_OverTime_Calculated +
														AllowancesTotal + WAR_Outstation_Allowance_Calculated + WAR_Nightshift_Allowance_Calculated +
														WAR_Performance_Allowance_Calculated + WAR_Attendance_Allowance_Calculated +
														 WAR_Allowance_Calculated_1 + WAR_Allowance_Calculated_2 + WAR_Allowance_Calculated_3 + WAR_Allowance_Calculated_4 + WAR_Allowance_Calculated_5 + WAR_Allowance_Calculated_6 + WAR_Allowance_Calculated_7 + WAR_Allowance_Calculated_8 + WAR_Allowance_Calculated_9 + WAR_Allowance_Calculated_10, MidpointRounding.AwayFromZero);

					#region EMI Calculation

					decimal totalEMI = 0;
					if (employee.EMP.Wage_Register_Advances.Count() > 0)
					{
						bool flag = false;
						int clients = attManager.GetAttendanceSummary_WageEmployee(wageProcess.WAG_Id, employee.EMP_Id).Select(m => m.CLI_Id).Distinct().Count();
						if (clients > 1)
						{
							IEnumerable<Clients_Employee> clientList = clientsManager.listActiveClientsEmployees_clients(employee.EMP_Id, new DateTime(wageProcess.WAG_Month.Year, wageProcess.WAG_Month.Month, wageProcess.WAG_Month.Day));
							Client_Requirement requirementMax = null;
							decimal Basic_Paybaleday_Max = 0;

							foreach (Clients_Employee clients_Employee in clientList)
							{
								Client_Requirement requirement = clientsManager.getActiveClientRequirement(clients_Employee.CLI_Id, clients_Employee.DES_Id, new DateTime(wageProcess.WAG_Month.Year, wageProcess.WAG_Month.Month, wageProcess.WAG_Month.Day));
								Attendance_Summary atts = attManager.GetAttendanceSummary_WageClientEmployee_Designation(wageProcess.WAG_Id, clients_Employee.CLI_Id, clients_Employee.EMP_Id, clients_Employee.DES_Id);

								#region payable days
								int totWeekOffs1 = 0, totalPublicHoliday1 = 0, totEarnLeave1 = 0, totNightShift1 = 0;
								double totPresentDays1 = 0, totHalfDays1 = 0, totExtraWorkingDays1 = 0, WAR_ExtraWorkingHours1 = 0, totNighthours1 = 0, totalPaybleDays1 = 0; ;

								totPresentDays1 = atts.ATS_PresentDays;
								totEarnLeave1 = (int)atts.ATS_EarnLeaves;
								totNightShift1 = (int)atts.ATS_NightShifts;
								//totHalfDays1 = atts.Where(a => a.ATT_IsHalfday == true).Count();
								WAR_ExtraWorkingHours1 = atts.ATS_ExtraHours;
								totExtraWorkingDays1 = WAR_ExtraWorkingHours / client.CLI_WorkingHours_In_Day;
								totWeekOffs1 = (int)atts.ATS_WeekOff;
								totalPublicHoliday1 = (int)atts.ATS_PublicHolidays;
								totNighthours1 = totNightShift * client.CLI_WorkingHours_In_Day;
								totalPaybleDays1 = (totPresentDays1 - totHalfDays1) + (totHalfDays1 / 2) + totEarnLeave1;
								if (requirement != null)
								{
									if (requirement.CRI_IsPayable_WeeklyOff)
									{
										totalPaybleDays1 = totalPaybleDays1 + totWeekOffs1;
									}
									if (requirement.CRI_IsPayable_PublicHoliday)
									{
										totalPaybleDays1 = totalPaybleDays1 + totalPublicHoliday1;
									}
									if (requirement.CRI_OT_Calculate_Payableday)
									{
										totalPaybleDays1 = totalPaybleDays1 + totExtraWorkingDays1;
									}
								}
								#endregion

								decimal Basic_Paybaleday = Convert.ToDecimal(Convert.ToDouble(requirement.CRI_Basic) * totalPaybleDays1);

								if (Basic_Paybaleday_Max == 0)
								{
									Basic_Paybaleday_Max = Basic_Paybaleday;
									requirementMax = requirement;
								}
								else
								{
									if (Basic_Paybaleday_Max < Basic_Paybaleday)
									{
										Basic_Paybaleday_Max = Basic_Paybaleday;
										requirementMax = requirement;
									}
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
							var EMI = employee.EMP.Wage_Register_Advances.Where(m => (m.WAD_Status == false && m.WAG.WAG_Month <= wageProcess.WAG_Month && m.WAG_Id.Equals(wageProcess.WAG_Id)) || (m.WAD_Status.Equals(true) && m.WAG_Id.Equals(wageProcess.WAG_Id))).GroupBy(m => m.EMP_Id).Select(g => new
							{
								WAD_Amt = g.Sum(n => n.WAD_Amount)
							});
							totalEMI = Math.Round(EMI.Select(g => g.WAD_Amt).FirstOrDefault(), MidpointRounding.AwayFromZero);
						}
					}
					wageRegisterVM.WAR_Advance_Amount = totalEMI;
					#endregion

					#region ProfessionalTax Calculation
					decimal WAR_ProffesionalTax_Calculated = 0M, WAR_RevenueDeduction_Calculated = 0M, WAR_CanteenFacility_Calculation = 0M;
					if (cr != null)
					{
						if (cr.CRI_ProfessionalTax == true)
						{
							WAR_ProffesionalTax_Calculated = clientsManager.GetProffessionalTax(employee.EMP.EMP_Gender, WAR_GrossTotal, cr);
						}
					}

					#endregion

					#region LWF Calculation   
					if (totalPaybleDays > 0)
					{
						if (wageProcess.WAG_Month.Month == (int)Month.June || wageProcess.WAG_Month.Month == (int)Month.December)
						{
							if (!employee.DES.DES_Exclude_LWF)
							{
								if (WAR_GrossTotal < cr.CRI_MLWF_Employer_Base)
								{
									WAR_LWF_Deduction_Employer = (cr.CRI_MLWF_Employer_LThen != null ? cr.CRI_MLWF_Employer_LThen.Value : 0); //Rs.6
								}
								else if (WAR_GrossTotal >= cr.CRI_MLWF_Employer_Base)
								{
									WAR_LWF_Deduction_Employer = (cr.CRI_MLWF_Employer_GThen != null ? cr.CRI_MLWF_Employer_GThen.Value : 0); ;  //Rs.12
								}

								if (WAR_GrossTotal < cr.CRI_MLWF_Employee_Base)
								{
									WAR_LWF_Deduction_Employee = (cr.CRI_MLWF_Employee_LThen != null ? cr.CRI_MLWF_Employee_LThen.Value : 0); ; //Rs.6
								}
								else if (WAR_GrossTotal >= cr.CRI_MLWF_Employee_Base)
								{
									WAR_LWF_Deduction_Employee = (cr.CRI_MLWF_Employee_GThen != null ? cr.CRI_MLWF_Employee_GThen.Value : 0); ;  //Rs.12
								}
							}
						}
					}
					wageRegisterVM.WAR_LWF_Deduction_Employee = WAR_LWF_Deduction_Employee;
					wageRegisterVM.WAR_LWF_Deduction_Employer = WAR_LWF_Deduction_Employer;
					#endregion

					if (cr != null)
					{
						if (cr.CRI_RevenueDeduction == true)
						{
							if (totalPaybleDays > 0)
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
					}


					decimal WAR_FinalTotal = Math.Round(WAR_GrossTotal - (WAR_PF_Calculated + WAR_ESIC_Calculated + totalEMI + WAR_ProffesionalTax_Calculated + WAR_RevenueDeduction_Calculated + WAR_CanteenFacility_Calculation + WAR_LWF_Deduction_Employee), MidpointRounding.AwayFromZero);
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

		public List<Wage_Register> Get_WageRegisterCalculated(Wage_Process wageProcess, int CLI_Id, int AdminID)
		{
			List<Wage_Register> wage_Registers = [];
			ClientsManager clientsManager = new(_context, _configuration);
			AttendanceSummaryManager attManager = new(_context);
			CanteenManager canteenManager = new(_context);
			OutstationManager outstationManager = new(_context);
			PerformanceManager performanceManager = new(_context);
			AllowanceManager allowanceManager = new(_context);

			IEnumerable<Clients_Employee> clientsEmployees = clientsManager.listActiveClientsEmployees(CLI_Id, new DateTime(wageProcess.WAG_Month.Year, wageProcess.WAG_Month.Month, wageProcess.WAG_Month.Day));
			Client client = clientsManager.GetClientById(CLI_Id);
			foreach (Clients_Employee employee in clientsEmployees)
			{
				Attendance_Summary attSummary = attManager.GetAttendanceSummary_WageClient_EmployeeDesignation(wageProcess.WAG_Id, CLI_Id, employee.EMP_Id, employee.DES.DES_Id);
				if (attSummary != null)
				{
					Client_Requirement cr = clientsManager.getActiveClientRequirement(CLI_Id, employee.DES.DES_Id, new DateTime(wageProcess.WAG_Month.Year, wageProcess.WAG_Month.Month, wageProcess.WAG_Month.Day));
					Attendance_Parameter att_Param = clientsManager.GetAttendanceParameterByMonth(CLI_Id, ProjectUtils.DateToDateTime(wageProcess.WAG_Month));
					DateTime[] Period = ProjectUtils.GetStartEndDatePeriodForAttendance(att_Param, ProjectUtils.DateToDateTime(wageProcess.WAG_Month));

					int totalWorkingDays = 0;
					double totalPaybleDays = 0;
					decimal CRI_Basic = 0M, WAR_Basic_Calculated = 0M, BasicDa = 0M, WAR_OverTime_Calculated = 0M, WAR_PF, WAR_PF_Calculated = 0M, WAR_ESIC = 0M, WAR_ESIC_Calculated = 0M, WAR_LWF_Deduction_Employer = 0M, WAR_LWF_Deduction_Employee = 0M;
					decimal CRI_DA = 0M, CRI_DA_Calculated = 0M, CRI_HRA = 0M, CRI_LeaveAndPH = 0M, CRI_HRA_Calculated = 0M, CRI_LeaveAndPH_Calculated =0M;
					decimal WAR_Attendance_Allowance_Calculated = 0M, WAR_Outstation_Allowance_Calculated = 0M, WAR_Performance_Allowance_Calculated = 0M, WAR_Nightshift_Allowance_Calculated = 0M, WAR_Allowance_Calculated_1 = 0M, WAR_Allowance_Calculated_2 = 0M, WAR_Allowance_Calculated_3 = 0M, WAR_Allowance_Calculated_4 = 0M, WAR_Allowance_Calculated_5 = 0M, WAR_Allowance_Calculated_6 = 0M, WAR_Allowance_Calculated_7 = 0M, WAR_Allowance_Calculated_8 = 0M, WAR_Allowance_Calculated_9 = 0M, WAR_Allowance_Calculated_10 = 0M;

					#region TOTAL WORKING DAY CALCULATION
					int Totaldays = (int)(Period[1] - Period[0]).TotalDays;
					//int days = attSummaries.Count();
					switch (client.CLI_Total_WorkingDays)
					{
						case 0: //  0:Consider_RealDays
							totalWorkingDays = Totaldays;
							break;
						case 1://   1:Excluding_WeeklyOff
							totalWorkingDays = Totaldays - (int)attSummary.ATS_WeekOff;
							break;
						case 2://   2:Consider StaticDays 
							totalWorkingDays = client.CLI_No_Reduce_Days.Value;
							break;
						default: break;
					}
					#endregion

					#region PAYBALE DAY COUNTING

					int totWeekOffs = 0, totalPublicHoliday = 0, totEarnLeave = 0, totNightShift = 0, totAbsent = 0;
					double totPresentDays = 0, totHalfDays = 0, totExtraWorkingDays = 0, WAR_ExtraWorkingHours = 0;// totNighthours = 0;

					totPresentDays = attSummary.ATS_PresentDays;
					totEarnLeave = (int)attSummary.ATS_EarnLeaves;
					totNightShift = (int)attSummary.ATS_NightShifts;
					totHalfDays = attSummary.ATS_HalfDays;
					//totHalfDays = attendances.Where(a => a.ATT_IsHalfday == true).Count();

					WAR_ExtraWorkingHours = attSummary.ATS_ExtraHours;
					totExtraWorkingDays = WAR_ExtraWorkingHours / client.CLI_WorkingHours_In_Day;

					totWeekOffs = (int)attSummary.ATS_WeekOff;
					totalPublicHoliday = (int)attSummary.ATS_PublicHolidays;
					// totNighthours = totNightShift * cli.CLI_WorkingHours_In_Day;

					totalPaybleDays = (totPresentDays - totHalfDays) + (totHalfDays / 2) + totEarnLeave;

					if (cr != null)
					{
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
					}

					//totAbsent = attendances.Where(m => m.ATT_IsPresent.Equals(false) && m.ATT_IsEarnLeave.Equals(false) && m.ATT_IsPublicHoliday.Equals(false) && m.ATT_IsWeeklyOff.Equals(false)).Count();

					totAbsent = Totaldays - (int)(attSummary.ATS_WeekOff + attSummary.ATS_PublicHolidays + attSummary.ATS_EarnLeaves + attSummary.ATS_PresentDays);
					#endregion

					WageRegisterVM wageRegisterVM = new WageRegisterVM
					{
						CRI_Id = cr.CRI_Id,
						CLE_Id = employee.CLE_Id,
						WAG_Id = wageProcess.WAG_Id,
						CLI_Id = CLI_Id,
						EMP_Id = employee.EMP_Id,
						employeeVM = EmployeesMapper.MapMe(employee.EMP),
						designation = employee.DES
					};

					#region NEW ALLOWANCES
					if (cr != null)
					{
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
							// WAR_Nightshift_Allowance_Calculated = Convert.ToDecimal(Convert.ToDouble(cr.CRI_Nightshift_Allowance_Rate.Value) * totNighthours);
							WAR_Nightshift_Allowance_Calculated = Convert.ToDecimal(Convert.ToDouble(cr.CRI_Nightshift_Allowance_Rate.Value) * totNightShift);
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
						if (cr.CRI_Allowance_1)
						{
							Wage_Register_Allowances_1 all_1 = new Wage_Register_Allowances_1();
							all_1 = allowanceManager.GetAllowances_ByCLE_1(wageProcess.WAG_Id, employee.CLE_Id, employee.CLI_Id);
							if (all_1 != null)
							{
								WAR_Allowance_Calculated_1 = all_1.WRA_Amount_1;
								wageRegisterVM.WRA_Id_1 = all_1.WRA_Id_1;
								wageRegisterVM.WAR_Allowance_Calculated_1 = WAR_Allowance_Calculated_1;
							}
							else
							{
								wageRegisterVM.WAR_Allowance_Calculated_1 = 0;
							}
						}
						if (cr.CRI_Allowance_2)
						{
							Wage_Register_Allowances_2 all_2 = new Wage_Register_Allowances_2();
							all_2 = allowanceManager.GetAllowances_ByCLE_2(wageProcess.WAG_Id, employee.CLE_Id, employee.CLI_Id);
							if (all_2 != null)
							{
								WAR_Allowance_Calculated_2 = all_2.WRA_Amount_2;
								wageRegisterVM.WRA_Id_2 = all_2.WRA_Id_2;
								wageRegisterVM.WAR_Allowance_Calculated_2 = WAR_Allowance_Calculated_2;
							}
							else
							{
								wageRegisterVM.WAR_Allowance_Calculated_2 = 0;
							}
						}
						if (cr.CRI_Allowance_3)
						{
							Wage_Register_Allowances_3 all_3 = new Wage_Register_Allowances_3();
							all_3 = allowanceManager.GetAllowances_ByCLE_3(wageProcess.WAG_Id, employee.CLE_Id, employee.CLI_Id);
							if (all_3 != null)
							{
								WAR_Allowance_Calculated_3 = all_3.WRA_Amount_3;
								wageRegisterVM.WRA_Id_3 = all_3.WRA_Id_3;
								wageRegisterVM.WAR_Allowance_Calculated_3 = WAR_Allowance_Calculated_3;
							}
							else
							{
								wageRegisterVM.WAR_Allowance_Calculated_3 = 0;
							}
						}
						if (cr.CRI_Allowance_4)
						{
							Wage_Register_Allowances_4 all_4 = new Wage_Register_Allowances_4();
							all_4 = allowanceManager.GetAllowances_ByCLE_4(wageProcess.WAG_Id, employee.CLE_Id, employee.CLI_Id);
							if (all_4 != null)
							{
								WAR_Allowance_Calculated_4 = all_4.WRA_Amount_4;
								wageRegisterVM.WRA_Id_4 = all_4.WRA_Id_4;
								wageRegisterVM.WAR_Allowance_Calculated_4 = WAR_Allowance_Calculated_4;
							}
							else
							{
								wageRegisterVM.WAR_Allowance_Calculated_4 = 0;
							}
						}
						if (cr.CRI_Allowance_5)
						{
							Wage_Register_Allowances_5 all_5 = new Wage_Register_Allowances_5();
							all_5 = allowanceManager.GetAllowances_ByCLE_5(wageProcess.WAG_Id, employee.CLE_Id, employee.CLI_Id);
							if (all_5 != null)
							{
								WAR_Allowance_Calculated_5 = all_5.WRA_Amount_5;
								wageRegisterVM.WRA_Id_5 = all_5.WRA_Id_5;
								wageRegisterVM.WAR_Allowance_Calculated_5 = WAR_Allowance_Calculated_5;
							}
							else
							{
								wageRegisterVM.WAR_Allowance_Calculated_5 = 0;
							}
						}
						if (cr.CRI_Allowance_6)
						{
							Wage_Register_Allowances_6 all_6 = allowanceManager.GetAllowances_ByCLE_6(wageProcess.WAG_Id, employee.CLE_Id, employee.CLI_Id);
							if (all_6 != null)
							{
								WAR_Allowance_Calculated_6 = all_6.WRA_Amount_6;
								wageRegisterVM.WRA_Id_6 = all_6.WRA_Id_6;
								wageRegisterVM.WAR_Allowance_Calculated_6 = WAR_Allowance_Calculated_6;
							}
							else
							{
								wageRegisterVM.WAR_Allowance_Calculated_6 = 0;
							}
						}
						if (cr.CRI_Allowance_7)
						{
							Wage_Register_Allowances_7 all_7 = allowanceManager.GetAllowances_ByCLE_7(wageProcess.WAG_Id, employee.CLE_Id, employee.CLI_Id);
							if (all_7 != null)
							{
								WAR_Allowance_Calculated_7 = all_7.WRA_Amount_7;
								wageRegisterVM.WRA_Id_7 = all_7.WRA_Id_7;
								wageRegisterVM.WAR_Allowance_Calculated_7 = WAR_Allowance_Calculated_7;
							}
							else
							{
								wageRegisterVM.WAR_Allowance_Calculated_7 = 0;
							}
						}
						if (cr.CRI_Allowance_8)
						{
							Wage_Register_Allowances_8 all_8 = allowanceManager.GetAllowances_ByCLE_8(wageProcess.WAG_Id, employee.CLE_Id, employee.CLI_Id);
							if (all_8 != null)
							{
								WAR_Allowance_Calculated_8 = all_8.WRA_Amount_8;
								wageRegisterVM.WRA_Id_8 = all_8.WRA_Id_8;
								wageRegisterVM.WAR_Allowance_Calculated_8 = WAR_Allowance_Calculated_8;
							}
							else
							{
								wageRegisterVM.WAR_Allowance_Calculated_8 = 0;
							}
						}
						if (cr.CRI_Allowance_9)
						{
							Wage_Register_Allowances_9 all_9 = allowanceManager.GetAllowances_ByCLE_9(wageProcess.WAG_Id, employee.CLE_Id, employee.CLI_Id);
							if (all_9 != null)
							{
								WAR_Allowance_Calculated_9 = all_9.WRA_Amount_9;
								wageRegisterVM.WRA_Id_9 = all_9.WRA_Id_9;
								wageRegisterVM.WAR_Allowance_Calculated_9 = WAR_Allowance_Calculated_9;
							}
							else
							{
								wageRegisterVM.WAR_Allowance_Calculated_9 = 0;
							}
						}
						if (cr.CRI_Allowance_10)
						{
							Wage_Register_Allowances_10 all_10 = allowanceManager.GetAllowances_ByCLE_10(wageProcess.WAG_Id, employee.CLE_Id, employee.CLI_Id);
							if (all_10 != null)
							{
								WAR_Allowance_Calculated_10 = all_10.WRA_Amount_10;
								wageRegisterVM.WRA_Id_10 = all_10.WRA_Id_10;
								wageRegisterVM.WAR_Allowance_Calculated_10 = WAR_Allowance_Calculated_10;
							}
							else
							{
								wageRegisterVM.WAR_Allowance_Calculated_10 = 0;
							}
						}
					}

					#endregion

					//if (cr != null)
					//{
					//    wageRegisterVM.clientRequirementVM = ClientRequirementMapper.mapMe(cr);
					//}
					//else
					//{
					//    wageRegisterVM.clientRequirementVM = new ClientRequirementVM();
					//}

					wageRegisterVM.WAR_TotalWorkingDays = totalWorkingDays;
					wageRegisterVM.WAR_TotalPaybleDays = totalPaybleDays;
					wageRegisterVM.WAR_ExtraWorkingHours = WAR_ExtraWorkingHours;

					if (cr != null)
					{
						CRI_Basic = Math.Round(cr.CRI_Basic.Value, MidpointRounding.AwayFromZero);
						CRI_DA = Math.Round(Convert.ToDecimal(cr.CRI_DA), MidpointRounding.AwayFromZero);
						BasicDa = Math.Round(Decimal.Add(CRI_Basic, Convert.ToDecimal(cr.CRI_DA)), MidpointRounding.AwayFromZero);
						CRI_HRA = Math.Round(cr.CRI_HRA_Fixed == null ? Convert.ToDecimal(cr.CRI_HRA_Percentage.Value) : cr.CRI_HRA_Fixed.Value, MidpointRounding.AwayFromZero);
						CRI_LeaveAndPH = Math.Round(cr.CRI_LeaveAndPH_Fixed == null ? Convert.ToDecimal(cr.CRI_LeaveAndPH_Percentage.Value) : cr.CRI_LeaveAndPH_Fixed.Value, MidpointRounding.AwayFromZero);
					}
					wageRegisterVM.WAR_Basic = CRI_Basic;
					wageRegisterVM.WAR_HRA = CRI_HRA;
					wageRegisterVM.WAR_LeaveAndPH = CRI_LeaveAndPH;
					wageRegisterVM.WAR_LastModifiedOn = ProjectUtils.DateNow();

					if (totalWorkingDays > 0)
					{
						double OvertimeInDay = WAR_ExtraWorkingHours / Convert.ToDouble(client.CLI_WorkingHours_In_Day);

						CRI_DA_Calculated = Math.Round(Math.Round(CRI_DA * Convert.ToDecimal(totalPaybleDays), MidpointRounding.AwayFromZero) / totalWorkingDays, MidpointRounding.AwayFromZero);
						WAR_Basic_Calculated = Math.Round(Math.Round((Decimal.Multiply(CRI_Basic, Convert.ToDecimal(totalPaybleDays))) / totalWorkingDays, MidpointRounding.AwayFromZero), MidpointRounding.AwayFromZero);
						decimal CalculatedBasicDa = Math.Round(Decimal.Add(WAR_Basic_Calculated, Convert.ToDecimal(CRI_DA_Calculated)), MidpointRounding.AwayFromZero);
						if (cr != null)
						{
							CRI_HRA_Calculated = (cr.CRI_HRA_Fixed == null ? Math.Round((Math.Round(CalculatedBasicDa * Convert.ToDecimal(cr.CRI_HRA_Percentage))) / 100, MidpointRounding.AwayFromZero) : Math.Round((Decimal.Multiply(cr.CRI_HRA_Fixed.Value, Convert.ToDecimal(totalPaybleDays))) / totalWorkingDays, MidpointRounding.AwayFromZero));
							CRI_LeaveAndPH_Calculated = (cr.CRI_LeaveAndPH_Fixed == null ? Math.Round((Math.Round(CalculatedBasicDa * Convert.ToDecimal(cr.CRI_LeaveAndPH_Percentage))) / 100, MidpointRounding.AwayFromZero) : Math.Round((Decimal.Multiply(cr.CRI_LeaveAndPH_Fixed.Value, Convert.ToDecimal(totalPaybleDays))) / totalWorkingDays, MidpointRounding.AwayFromZero));
						}
						#region START PF-ESIC -OT CALCULATION  
						if (OvertimeInDay > 0)
						{
							if (cr != null)
							{
								if (!cr.CRI_OT_Calculate_Payableday)
								{
									if (cr.CRI_OT_Fixed_PerHour > 0)
									{
										WAR_OverTime_Calculated = Convert.ToDecimal(WAR_ExtraWorkingHours) * cr.CRI_OT_Fixed_PerHour.Value;
									}
									else if (cr.CRI_OT_Formula != null)
									{
										decimal OTsum = Math.Round(GetAmountBasedOnFormulaOT(
											cr.CRI_OT_Formula,
											WAR_Basic_Calculated,
											CRI_DA_Calculated,
											CRI_HRA_Calculated,
											CRI_LeaveAndPH_Calculated,
											cr.Client_Requirement_Allowances.ToList(),
											totalWorkingDays,
											totalPaybleDays,
											WAR_Outstation_Allowance_Calculated,
											WAR_Attendance_Allowance_Calculated,
											WAR_Nightshift_Allowance_Calculated,
											WAR_Performance_Allowance_Calculated,
											WAR_Allowance_Calculated_1, WAR_Allowance_Calculated_2, WAR_Allowance_Calculated_3, WAR_Allowance_Calculated_4, WAR_Allowance_Calculated_5, WAR_Allowance_Calculated_6, WAR_Allowance_Calculated_7, WAR_Allowance_Calculated_8, WAR_Allowance_Calculated_9, WAR_Allowance_Calculated_10
											), MidpointRounding.AwayFromZero);
										WAR_OverTime_Calculated = Math.Round(Convert.ToDecimal(((Convert.ToDouble(OTsum) / totalPaybleDays) * OvertimeInDay) * cr.CRI_OT_MultipleTimes), MidpointRounding.AwayFromZero);
									}
								}
							}
						}
						if (cr != null)
						{
							decimal PFsum = Math.Round(GetAmountBasedOnFormula(
							cr.CRI_PF_Formula, WAR_Basic_Calculated, CRI_DA_Calculated, CRI_HRA_Calculated, CRI_LeaveAndPH_Calculated,
							cr.Client_Requirement_Allowances.ToList(), totalWorkingDays, totalPaybleDays,
							WAR_OverTime_Calculated, WAR_Outstation_Allowance_Calculated, WAR_Attendance_Allowance_Calculated,
							WAR_Nightshift_Allowance_Calculated, WAR_Performance_Allowance_Calculated,
							 WAR_Allowance_Calculated_1, WAR_Allowance_Calculated_2, WAR_Allowance_Calculated_3, WAR_Allowance_Calculated_4, WAR_Allowance_Calculated_5, WAR_Allowance_Calculated_6, WAR_Allowance_Calculated_7, WAR_Allowance_Calculated_8, WAR_Allowance_Calculated_9, WAR_Allowance_Calculated_10), MidpointRounding.AwayFromZero);

							decimal ESICsum = Math.Round(GetAmountBasedOnFormula(
								cr.CRI_ESIC_Formula, WAR_Basic_Calculated, CRI_DA_Calculated, CRI_HRA_Calculated, CRI_LeaveAndPH_Calculated,
								cr.Client_Requirement_Allowances.ToList(), totalWorkingDays, totalPaybleDays, WAR_OverTime_Calculated,
								WAR_Outstation_Allowance_Calculated, WAR_Attendance_Allowance_Calculated, WAR_Nightshift_Allowance_Calculated,
								WAR_Performance_Allowance_Calculated, WAR_Allowance_Calculated_1, WAR_Allowance_Calculated_2, WAR_Allowance_Calculated_3, WAR_Allowance_Calculated_4, WAR_Allowance_Calculated_5, WAR_Allowance_Calculated_6, WAR_Allowance_Calculated_7, WAR_Allowance_Calculated_8, WAR_Allowance_Calculated_9, WAR_Allowance_Calculated_10), MidpointRounding.AwayFromZero);

							WAR_PF = Convert.ToDecimal(cr.CRI_PF_Percentage);
							if (cr.CRI_PF_ApplyMAX.HasValue && cr.CRI_PF_ApplyMAX.Value > 0)
							{
								if (PFsum >= cr.CRI_PF_ApplyMAX)
								{
									WAR_PF_Calculated = Math.Round(Decimal.Multiply(cr.CRI_PF_ApplyMAX.Value, WAR_PF) / 100, MidpointRounding.AwayFromZero);
								}
								else
								{
									WAR_PF_Calculated = Math.Round(Decimal.Multiply(PFsum, WAR_PF) / 100, MidpointRounding.AwayFromZero);
								}
							}
							else
							{
								WAR_PF_Calculated = Math.Round(Decimal.Multiply(PFsum, WAR_PF) / 100, MidpointRounding.AwayFromZero);
							}
							WAR_ESIC = Convert.ToDecimal(cr.CRI_ESIC_Percentage);
							WAR_ESIC_Calculated = Math.Ceiling(Decimal.Multiply(ESICsum, WAR_ESIC) / 100);
							#endregion

							wageRegisterVM.WAR_PF_Formula = cr.CRI_PF_Formula;
							wageRegisterVM.WAR_ESIC_Formula = cr.CRI_ESIC_Formula;
							wageRegisterVM.WAR_PF = WAR_PF;
							wageRegisterVM.WAR_PF_Calculated = WAR_PF_Calculated;
							wageRegisterVM.WAR_ESIC = WAR_ESIC;
							wageRegisterVM.WAR_ESIC_Calculated = WAR_ESIC_Calculated;
							wageRegisterVM.WAR_OverTime_Calculated = WAR_OverTime_Calculated;
							wageRegisterVM.WAR_OverTime_Formula = cr.CRI_OT_Formula;
							wageRegisterVM.WAR_WorkingHrs_In_Day = client.CLI_WorkingHours_In_Day;
							wageRegisterVM.WAR_OverTime_Payment = Convert.ToInt16(cr.CRI_OT_MultipleTimes);
							wageRegisterVM.WAR_Basic_Calculated = WAR_Basic_Calculated;
							wageRegisterVM.WAR_HRA_Calculated = CRI_HRA_Calculated;
							wageRegisterVM.WAR_LeaveAndPH_Calculated = CRI_LeaveAndPH_Calculated;

						}

					}
					//************************** ALLOWANCES CALCULATION *****************************//
					List<WageRegisterAllowanceVM> allowances = new List<WageRegisterAllowanceVM>();
					decimal AllowancesTotal = 0;
					if (cr != null)
					{
						foreach (var allowance in cr.Client_Requirement_Allowances)
						{
							WageRegisterAllowanceVM all = new WageRegisterAllowanceVM();
							all.CRA_Id = allowance.CRA_Id;
							all.CRI_Id = allowance.CRI_Id;
							all.WAA_Amount = allowance.CRA_Amount;
							all.WAA_DayswiseOrFull = allowance.CRA_DayswiseOrFull;
							all.allowanceVM = AllowanceMapper.mapMe(allowance.ALL);
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
					}

					wageRegisterVM.WAR_Attendance_Allowance_Calculated = WAR_Attendance_Allowance_Calculated;
					wageRegisterVM.allowanceVMs = allowances;
					//************************** ALLOWANCES CALCULATION *****************************/
					decimal WAR_GrossTotal = Math.Round(WAR_Basic_Calculated + CRI_DA_Calculated + CRI_HRA_Calculated + CRI_LeaveAndPH_Calculated + WAR_OverTime_Calculated +
														AllowancesTotal + WAR_Outstation_Allowance_Calculated + WAR_Nightshift_Allowance_Calculated +
														WAR_Performance_Allowance_Calculated + WAR_Attendance_Allowance_Calculated +
														 WAR_Allowance_Calculated_1 + WAR_Allowance_Calculated_2 + WAR_Allowance_Calculated_3 + WAR_Allowance_Calculated_4 + WAR_Allowance_Calculated_5 + WAR_Allowance_Calculated_6 + WAR_Allowance_Calculated_7 + WAR_Allowance_Calculated_8 + WAR_Allowance_Calculated_9 + WAR_Allowance_Calculated_10, MidpointRounding.AwayFromZero);

					#region EMI Calculation

					decimal totalEMI = 0;
					if (employee.EMP.Wage_Register_Advances.Count() > 0)
					{
						bool flag = false;
						int clients = attManager.GetAttendanceSummary_WageEmployee(wageProcess.WAG_Id, employee.EMP_Id).Select(m => m.CLI_Id).Distinct().Count();
						if (clients > 1)
						{
							IEnumerable<Clients_Employee> clientList = clientsManager.listActiveClientsEmployees_clients(employee.EMP_Id, new DateTime(wageProcess.WAG_Month.Year, wageProcess.WAG_Month.Month, wageProcess.WAG_Month.Day));
							Client_Requirement requirementMax = null;
							decimal Basic_Paybaleday_Max = 0;

							foreach (Clients_Employee clients_Employee in clientList)
							{
								Client_Requirement requirement = clientsManager.getActiveClientRequirement(clients_Employee.CLI_Id, clients_Employee.DES_Id, new DateTime(wageProcess.WAG_Month.Year, wageProcess.WAG_Month.Month, wageProcess.WAG_Month.Day));
								Attendance_Summary atts = attManager.GetAttendanceSummary_WageClientEmployee_Designation(wageProcess.WAG_Id, clients_Employee.CLI_Id, clients_Employee.EMP_Id, clients_Employee.DES_Id);

								#region payable days
								int totWeekOffs1 = 0, totalPublicHoliday1 = 0, totEarnLeave1 = 0, totNightShift1 = 0;
								double totPresentDays1 = 0, totHalfDays1 = 0, totExtraWorkingDays1 = 0, WAR_ExtraWorkingHours1 = 0, totNighthours1 = 0, totalPaybleDays1 = 0;

								totPresentDays1 = atts.ATS_PresentDays;
								totEarnLeave1 = (int)atts.ATS_EarnLeaves;
								totNightShift1 = (int)atts.ATS_NightShifts;
								//totHalfDays1 = atts.Where(a => a.ATT_IsHalfday == true).Count();
								WAR_ExtraWorkingHours1 = atts.ATS_ExtraHours;
								totExtraWorkingDays1 = WAR_ExtraWorkingHours / client.CLI_WorkingHours_In_Day;
								totWeekOffs1 = (int)atts.ATS_WeekOff;
								totalPublicHoliday1 = (int)atts.ATS_PublicHolidays;
								totNighthours1 = totNightShift * client.CLI_WorkingHours_In_Day;
								totalPaybleDays1 = (totPresentDays1 - totHalfDays1) + (totHalfDays1 / 2) + totEarnLeave1;
								if (requirement != null)
								{
									if (requirement.CRI_IsPayable_WeeklyOff)
									{
										totalPaybleDays1 = totalPaybleDays1 + totWeekOffs1;
									}
									if (requirement.CRI_IsPayable_PublicHoliday)
									{
										totalPaybleDays1 = totalPaybleDays1 + totalPublicHoliday1;
									}
									if (requirement.CRI_OT_Calculate_Payableday)
									{
										totalPaybleDays1 = totalPaybleDays1 + totExtraWorkingDays1;
									}
								}
								#endregion

								decimal Basic_Paybaleday = Convert.ToDecimal(Convert.ToDouble(requirement.CRI_Basic) * totalPaybleDays1);

								if (Basic_Paybaleday_Max == 0)
								{
									Basic_Paybaleday_Max = Basic_Paybaleday;
									requirementMax = requirement;
								}
								else
								{
									if (Basic_Paybaleday_Max < Basic_Paybaleday)
									{
										Basic_Paybaleday_Max = Basic_Paybaleday;
										requirementMax = requirement;
									}
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
							var EMI = employee.EMP.Wage_Register_Advances.Where(m => (m.WAD_Status == false && m.WAG.WAG_Month <= wageProcess.WAG_Month && m.WAG_Id.Equals(wageProcess.WAG_Id)) || (m.WAD_Status.Equals(true) && m.WAG_Id.Equals(wageProcess.WAG_Id))).GroupBy(m => m.EMP_Id).Select(g => new
							{
								WAD_Amt = g.Sum(n => n.WAD_Amount)
							});
							totalEMI = Math.Round(EMI.Select(g => g.WAD_Amt).FirstOrDefault(), MidpointRounding.AwayFromZero);
						}
					}
					wageRegisterVM.WAR_Advance_Amount = totalEMI;
					#endregion

					#region ProfessionalTax Calculation
					decimal WAR_ProffesionalTax_Calculated = 0M, WAR_RevenueDeduction_Calculated = 0M, WAR_CanteenFacility_Calculation = 0M;
					if (cr != null)
					{
						if (cr.CRI_ProfessionalTax == true)
						{
							//ProfessionalTaxCalculationManager ptcManager = new ProfessionalTaxCalculationManager(_context);
							// WAR_ProffesionalTax_Calculated = Math.Round(ptcManager.GetPT((employee.EMP_.EMP_Gender ? "M" : "F"), WAR_GrossTotal), MidpointRounding.AwayFromZero);                           
							WAR_ProffesionalTax_Calculated = clientsManager.GetProffessionalTax(employee.EMP.EMP_Gender, WAR_GrossTotal, cr);
						}
					}

					#endregion

					#region LWF Calculation   
					if (totalPaybleDays > 0)
					{
						if (wageProcess.WAG_Month.Month == (int)Month.June || wageProcess.WAG_Month.Month == (int)Month.December)
						{
							if (!employee.DES.DES_Exclude_LWF)
							{
								if (WAR_GrossTotal < cr.CRI_MLWF_Employer_Base)
								{
									WAR_LWF_Deduction_Employer = (cr.CRI_MLWF_Employer_LThen != null ? cr.CRI_MLWF_Employer_LThen.Value : 0); //Rs.6
								}
								else if (WAR_GrossTotal >= cr.CRI_MLWF_Employer_Base)
								{
									WAR_LWF_Deduction_Employer = (cr.CRI_MLWF_Employer_GThen != null ? cr.CRI_MLWF_Employer_GThen.Value : 0); ;  //Rs.12
								}

								if (WAR_GrossTotal < cr.CRI_MLWF_Employee_Base)
								{
									WAR_LWF_Deduction_Employee = (cr.CRI_MLWF_Employee_LThen != null ? cr.CRI_MLWF_Employee_LThen.Value : 0); ; //Rs.6
								}
								else if (WAR_GrossTotal >= cr.CRI_MLWF_Employee_Base)
								{
									WAR_LWF_Deduction_Employee = (cr.CRI_MLWF_Employee_GThen != null ? cr.CRI_MLWF_Employee_GThen.Value : 0); ;  //Rs.12
								}
							}
						}
					}
					wageRegisterVM.WAR_LWF_Deduction_Employee = WAR_LWF_Deduction_Employee;
					wageRegisterVM.WAR_LWF_Deduction_Employer = WAR_LWF_Deduction_Employer;
					#endregion

					if (cr != null)
					{
						if (cr.CRI_RevenueDeduction == true)
						{
							if (totalPaybleDays > 0)
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
					}

					decimal WAR_FinalTotal = Math.Round(WAR_GrossTotal - (WAR_PF_Calculated + WAR_ESIC_Calculated + totalEMI + WAR_ProffesionalTax_Calculated + WAR_RevenueDeduction_Calculated + WAR_CanteenFacility_Calculation + WAR_LWF_Deduction_Employee), MidpointRounding.AwayFromZero);
					wageRegisterVM.WAR_GrossTotal = WAR_GrossTotal;
					wageRegisterVM.WAR_FinalTotal = WAR_FinalTotal;
					wageRegisterVM.WAR_DA = CRI_DA;
					wageRegisterVM.WAR_DA_Calculated = CRI_DA_Calculated;
					wageRegisterVM.ADM_LastModifiedBy = AdminID;

					wage_Registers.Add(WageRegisterMapper.mapMe(wageRegisterVM));
					//lstRegister.Add(wageRegisterVM);
				}
			}
			return wage_Registers;
		}

		#endregion

		#region REPORTS

		public List<Wage_Register> GetWageRegistersForBank(int WAG_Id, int CLI_Id)
		{
			return _context.Wage_Registers.Where(r => r.WAG_Id == WAG_Id && r.CLI_Id == CLI_Id && r.WAR_FinalTotal > 0).Include(n => n.EMP).AsSplitQuery().ToList();
		}

		public List<Wage_Register> GetWageRegistersForIDBI_To_IDBI(int WAG_Id, int[] CLI_Ids)
		{
			return [.. _context.Wage_Registers.Where(m => m.WAG_Id == WAG_Id && CLI_Ids.Contains(m.CLI_Id) && m.EMP.EMP_Payment_Type.Equals((int)PAYMENT_TYPE.Bank_Account) && (m.EMP.CBA.CBA_Bank == ProjectUtils.GetStringValue(REGISTER_BANK.IDBI_BANK_LTD) && m.EMP.EMP_Bank == ProjectUtils.GetStringValue(REGISTER_BANK.IDBI_BANK_LTD)) && m.WAR_FinalTotal > 0)
				.Include(m => m.EMP)];
		}

		public IQueryable<Wage_Register> GetWageRegister_EmployeesOnlyRegistered(int WAG_Id, int[] CLI_Ids)
		{
			return _context.Wage_Registers.Where(r => r.WAG_Id == WAG_Id && CLI_Ids.Contains(r.CLI_Id) && r.EMP.EMP_UAN_Number != null && r.EMP.EMP_UAN_Number != "Pending" && r.EMP.EMP_UAN_Number != "")
				.Include(m => m.CLI)
				.Include(m => m.EMP)
				.Include(m => m.CRI)
				.Include(n => n.Wage_Register_Allowances).AsSplitQuery();
		}

		public IQueryable<Wage_Register> GetWageRegister_EmployeesPendingForRegistration(int WAG_Id, int[] CLI_Ids)
		{
			return _context.Wage_Registers.Where(r => r.WAG_Id == WAG_Id && CLI_Ids.Contains(r.CLI_Id) && (r.EMP.EMP_UAN_Number == null || r.EMP.EMP_UAN_Number.Equals("Pending") || r.EMP.EMP_UAN_Number.Equals("")))
				.Include(m => m.CLI)
				.Include(m => m.EMP).AsSplitQuery();
		}

		public List<Wage_Register> GetWageRegisters_ClientsWiseReports(int WAG_Id, int CLI_Id)
		{
			return _context.Wage_Registers.Where(r => r.WAG_Id == WAG_Id && r.CLI_Id == CLI_Id)
				 .Include(m => m.EMP)
				 .Include(m => m.CRI)
				 .Include(n => n.Wage_Register_Allowances).ThenInclude(c => c.CRA).ThenInclude(a => a.ALL).AsSplitQuery().ToList();
		}

		public IQueryable<Wage_Register> GetRegisterFor_PayTaxReports(int WAG_Id)
		{
			return _context.Wage_Registers.Where(r => r.WAG_Id == WAG_Id && r.CRI != null && r.CRI.CRI_ProfessionalTax).Include(m => m.CLI).Include(m => m.EMP).Include(m => m.CRI);
		}

		#endregion
	}
}
