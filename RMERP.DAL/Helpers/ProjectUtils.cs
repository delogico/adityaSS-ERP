using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using RMERP.DAL.ViewModel;
using RMERP.DAL.Models;
namespace RMERP.DAL.Helpers
{
    public static class ProjectUtils
    {
        public enum CountryRegion
        {
            International = 0,
            Domestic = 1
        }
        public enum Gender
        {
            Male = 0,
            Female = 1
        }
        public enum MaritalStatus
        {
            UnMarried = 0,
            Married = 1
        }
        public enum ESIC_AREA
        {
            NEW_IMP = 0,
            OLD_IMP = 1
        }

        public enum ATTENDANCE_EXCEL_TEMPLATE
        {
            TWOROW_WithoutShifts = 0,
            TWOROW_WithShifts = 1,
            ONEROW_WithoutShift = 2
        }
        public enum Total_WorkingDyas_In_Month
        {
            Consider_RealDays_In_Month = 0,
            Exclude_WeeklyOff = 1,
            Reduce_Fixed_Days = 2
        }

        public enum DEDUCTION_TAX
        {
            Proffesional_Tax = 0,
            Revenue_Deduction = 1,
            Canteen_Facility = 2
        }

        public static DateTime DateNow()
        {
            DateTime utcTime = DateTime.UtcNow;
            TimeZoneInfo myZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime custDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, myZone);
            return custDateTime;
        }

        public static string GetTempFolderPath(string webRootPath)
        {
            string folderName = "RMERP_Data";
            string newPath = Path.Combine(webRootPath, folderName);
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }
            return newPath;
        }

        public static string GetTempFileName()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmss");
        }
       // public static int WagId, CliId, ClientId, EmpId, Frm_Id;

        public static string GetTooltipBasedOnFormula(string CRI_Formula, decimal WAR_Basic_Calculated, decimal CRI_DA_Calculated, decimal CRI_HRA_Calculated, List<WageRegisterAllowanceVM> All)
        {
            string str = "";
            string[] arr_CRI_Formula;

            if (CRI_Formula != null)
            {
                arr_CRI_Formula = CRI_Formula.Split("+");
                foreach (string item in arr_CRI_Formula)
                {
                    switch (item)
                    {
                        case "BASIC":
                            str = String.Format("{0:0.##}", Convert.ToDecimal(WAR_Basic_Calculated));
                            break;
                        case "DA":
                            str = str + "+" + String.Format("{0:0.##}", Convert.ToDecimal(CRI_DA_Calculated));
                            break;
                        case "HRA":
                            str = str + "+" + String.Format("{0:0.##}", Convert.ToDecimal(CRI_HRA_Calculated));
                            break;
                        default:
                            {
                                foreach (var allowance in All)
                                {
                                    if (allowance.allowanceVM.ALL_Shortform == item)
                                    {
                                        str = str + "+" + String.Format("{0:0.##}", allowance.WAA_Amount_Calculated);
                                    }
                                }
                            }
                            break;
                    }
                }
            }
            return str;
        }

        public static decimal GetGrossAmountBasedOnFormula(string CRI_Formula, Wage_Register wage_Register)
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
                            sum += Convert.ToDecimal(wage_Register.WAR_Basic_Calculated);
                            break;
                        case "DA":
                            sum += Convert.ToDecimal(wage_Register.WAR_DA_Calculated);
                            break;
                        case "HRA":
                            sum += Convert.ToDecimal(wage_Register.WAR_HRA_Calculated);
                            break;
                        default:
                            {
                                List<Client_Requirement_Allowances> All = wage_Register.CRI_.Client_Requirement_Allowances.ToList();
                                foreach (var allowance in All)
                                {
                                    if (allowance.ALL_.ALL_Shortform == item)
                                    {
                                        decimal amount = allowance.CRA_Amount;
                                        decimal fullAmt = 0M;
                                        if (wage_Register.WAR_TotalWorkingDays != 0)
                                            fullAmt = (Decimal.Multiply(amount, Convert.ToDecimal(wage_Register.WAR_TotalPaybleDays))) / Convert.ToDecimal(wage_Register.WAR_TotalWorkingDays);

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
    }
}
