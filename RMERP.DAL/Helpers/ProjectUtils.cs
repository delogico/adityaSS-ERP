using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using RMERP.DAL.ViewModel;
using RMERP.DAL.Models;
using System.Reflection;

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

        public enum PAYMENT_TYPE
        {
            [StringValue("Bank Account")]
            Bank_Account = 0,
            [StringValue("Cheque/Cash")]
            Cheque_Cash = 1
        }
        public enum PAYMENT_BANK_TYPE
        {
            [StringValue("IDBI To IDBI")]
            IDBI_To_IDBI = 0,
            [StringValue("IDBI To Other")]
            IDBI_To_Others = 1
        }
        public enum PF_REPORT_TYPE
        {
            [StringValue("Client Wise PF contribution Details.xlsx")]
            Client_Wise_PF_Details_Excel = 0,
            [StringValue("PF Employees Pending For Registration.xlsx")]
            Employees_Pending_For_Registration_Excel = 1,
            [StringValue("PF EXCEL.xlsx")]
            Employee_PF_Excel = 2,
            [StringValue("LIST OF EMPLOYEE ABOVE 58.xlsx")]
            PF_Report_Above_58 = 3,
            [StringValue("PF Report.txt")]
            PF_Report_Text = 4
        }
        public enum ESIC_REPORT_TYPE
        {
            [StringValue("Client Wise ESIC Details.xlsx")]
            Client_Wise_ESIC_Excel = 0,
            [StringValue("Employee Wise ESIC Details.xlsx")]
            Employee_Wise_Esic_Details_Excel = 1,
            [StringValue("ESIC Employees Pending For Registration.xlsx")]
            ESIC_Employees_Pending_For_Registration_Excel = 2

        }
        public enum BANK_REPORT_TYPE
        {
            [StringValue("Company Wise Transfer Report.xlsx")]
            Company_Wise_Transfer_Report = 0,
            [StringValue("IDBI Bank To IDBI Bank Report.xlsx")]
            IDBI_Bank_To_IDBI_Bank_Report = 1,
            [StringValue("IDBI Bank To Others Report.xlsx")]
            IDBI_Bank_To_Others_Report = 2,
            [StringValue("CHEQUE/CASH Report.xlsx")]
            CHEQUE_CASH_Report = 3
        }

        public enum EMPLOYEE_LEFT_REASON_CODE
        {
            [StringValue("Without Reason")]
            Without_Reason = 0,
            [StringValue("On Leave")]
            On_Leave = 1,
            [StringValue("Left Service")]
            Left_Service = 2,
            [StringValue("Retired")]
            Retired = 3,
            [StringValue("Out of Coverage")]
            Out_Of_Coverage = 4,
            [StringValue("Expired")]
            Expired = 5,
            [StringValue("Non Implemented area")]
            Non_Implemented_Area = 6,
            [StringValue("Compliance by Immediate Employer")]
            Compliance_By_Immediate_Employer = 7,
            [StringValue("Suspension of work")]
            Suspension_Of_Work = 8,
            [StringValue("Strike/Lockout")]
            Strike_Lockout = 9,
            [StringValue("Retrenchment")]
            Retrenchment = 10,
            [StringValue("No Work")]
            No_Work = 11,
            [StringValue("Doesnt Belong To This Employer")]
            Doesnt_Belong_To_This_Employer = 12,
            [StringValue("Duplicate IP")]
            Duplicate_IP = 13
        }
        public enum CRI_BILLING_TYPE
        {
            [StringValue("Billing On Lump-Sum Amount Per Month Per Person")]
            Lump_Sum_Amount = 1,
            [StringValue("Billing On Service Change Basic")]
            Service_Change_Basic = 2,
        }
        public enum INVOICE_TEMPLATE_TYPE
        {
            CONTRACT_BILL_FOR_PROVIDING_FACILITY_SERVICES=1,
            COMPANY_CONTRIBUTION_PF=2,
            COMPANY_CONTRIBUTION_ESIC = 3,
            FULL_AND_FINAL_SETTLEMENT =4
        }
        public enum Month
        {
            NotSet = 0,
            January = 1,
            February = 2,
            March = 3,
            April = 4,
            May = 5,
            June = 6,
            July = 7,
            August = 8,
            September = 9,
            October = 10,
            November = 11,
            December = 12
        }


        public enum PDFAction
        {
            Download = 0,
            View = 1
        }
        public enum Assign_Unassign
        {
            Assign = 0,
            Unassign = 1
        }
        public enum WagePaySlip
        {
            NotGenerated=0,
            Generated = 1
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

        public static string GetStringValue(Enum value)
        {
            // Get the type
            Type type = value.GetType();

            // Get fieldinfo for this type
            FieldInfo fieldInfo = type.GetField(value.ToString());

            // Get the stringvalue attributes
            StringValueAttribute[] attribs = fieldInfo.GetCustomAttributes(
                typeof(StringValueAttribute), false) as StringValueAttribute[];

            // Return the first if there was a match.
            return attribs.Length > 0 ? attribs[0].StringValue : null;
        }

        public static decimal GetAmountBasedOnFormula(string CRI_Formula, decimal WAR_Basic_Calculated, decimal CRI_DA_Calculated, decimal CRI_HRA_Calculated, List<Client_Requirement_Allowances> All, double totalWorkingDays, double totalPaybleDays, decimal WAR_OverTime_Calculated, decimal WAR_Outstation_Allowance_Calculated, decimal WAR_Attendance_Allowance_Calculated, decimal WAR_Nightshift_Allowance_Calculated, decimal WAR_Performance_Allowance_Calculated)
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
                        case "OT":
                            sum += Convert.ToDecimal(WAR_OverTime_Calculated);
                            break;
                        case "OSL":
                            sum += Convert.ToDecimal(WAR_Outstation_Allowance_Calculated);
                            break;
                        case "AAL":
                            sum += Convert.ToDecimal(WAR_Attendance_Allowance_Calculated);
                            break;
                        case "NSL":
                            sum += Convert.ToDecimal(WAR_Nightshift_Allowance_Calculated);
                            break;
                        case "PAL":
                            sum += Convert.ToDecimal(WAR_Performance_Allowance_Calculated);
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
                                            fullAmt = (Decimal.Multiply(amount, Convert.ToDecimal(totalPaybleDays))) / Convert.ToDecimal(totalWorkingDays);

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
        public static decimal GetAmountBasedOnFormula_Report(string CRI_Formula, decimal WAR_Basic_Calculated, decimal CRI_DA_Calculated, decimal CRI_HRA_Calculated, List<Wage_Register_Allowances> All, double totalWorkingDays, double totalPaybleDays, decimal WAR_OverTime_Calculated, decimal WAR_Outstation_Allowance_Calculated, decimal WAR_Attendance_Allowance_Calculated, decimal WAR_Nightshift_Allowance_Calculated, decimal WAR_Performance_Allowance_Calculated)
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
                        case "OT":
                            sum += Convert.ToDecimal(WAR_OverTime_Calculated);
                            break;
                        case "OSL":
                            sum += Convert.ToDecimal(WAR_Outstation_Allowance_Calculated);
                            break;
                        case "AAL":
                            sum += Convert.ToDecimal(WAR_Attendance_Allowance_Calculated);
                            break;
                        case "NSL":
                            sum += Convert.ToDecimal(WAR_Nightshift_Allowance_Calculated);
                            break;
                        case "PAL":
                            sum += Convert.ToDecimal(WAR_Performance_Allowance_Calculated);
                            break;
                        default:
                            {
                                foreach (var allowance in All)
                                {
                                    if (allowance.CRA_.ALL_.ALL_Shortform == item)
                                    {
                                        sum += allowance.WAA_Amount_Calculated;
                                    }
                                }
                            }
                            break;
                    }
                }
            }
            return sum;
        }

        public static string GetStringBasedOnFormula_Report(string CRI_Formula, List<Wage_Register_Allowances> All)
        {
            string ret = "";
            ret = CRI_Formula.Replace("+", ",");
            //string[] arr_CRI_Formula;

            //if (CRI_Formula != null)
            //{
            //    arr_CRI_Formula = CRI_Formula.Split("+");
            //    foreach (string item in arr_CRI_Formula)
            //    {
            //        switch (item)
            //        {
            //            case "BASIC":
            //                ret += "BASIC";
            //                break;
            //            case "DA":
            //                ret += ", DA";
            //                break;
            //            case "HRA":
            //                ret += ", HRA";
            //                break;
            //            case "OT":
            //                ret += ", Extra Work Wages";
            //                break;
            //            case "OSL":
            //                ret += ", Outstation Wages";
            //                break;
            //            case "AAL":
            //                ret += ", Attendance Wages";
            //                break;
            //            case "NSL":
            //                ret += ", Night shift Wages";
            //                break;
            //            case "PAL":
            //                ret += ", Performance Wages";
            //                break;
            //            default:
            //                {
            //                    foreach (var allowance in All)
            //                    {
            //                        if (allowance.CRA_.ALL_.ALL_Shortform == item)
            //                        {
            //                            ret +=", "+ allowance.CRA_.ALL_.ALL_Title;
            //                        }
            //                    }
            //                }
            //                break;
            //        }
            //    }
            //}
            return ret;
        }

        public static decimal GetAmountBasedOnFormula_Edit(string CRI_Formula, decimal WAR_Basic_Calculated, decimal CRI_DA_Calculated, decimal CRI_HRA_Calculated, List<CalculatedAllowanceVM> All, double totalWorkingDays, double totalPaybleDays, decimal WAR_OverTime_Calculated, decimal WAR_Outstation_Allowance_Calculated, decimal WAR_Attendance_Allowance_Calculated, decimal WAR_Nightshift_Allowance_Calculated, decimal WAR_Performance_Allowance_Calculated)
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
                        case "OT":
                            sum += Convert.ToDecimal(WAR_OverTime_Calculated);
                            break;
                        case "OSL":
                            sum += Convert.ToDecimal(WAR_Outstation_Allowance_Calculated);
                            break;
                        case "AAL":
                            sum += Convert.ToDecimal(WAR_Attendance_Allowance_Calculated);
                            break;
                        case "NSL":
                            sum += Convert.ToDecimal(WAR_Nightshift_Allowance_Calculated);
                            break;
                        case "PAL":
                            sum += Convert.ToDecimal(WAR_Performance_Allowance_Calculated);
                            break;
                        default:
                            {
                                foreach (var allowance in All)
                                {   
                                    if(allowance.ALL_Shortform==item)
                                        sum += allowance.WAA_Amount_Calculated;                                   
                                }
                            }
                            break;
                    }
                }
            }
            return sum;
        }

        public static decimal GetAmountBasedOnFormulaOT(string CRI_Formula, decimal WAR_Basic_Calculated, decimal CRI_DA_Calculated, decimal CRI_HRA_Calculated, List<Client_Requirement_Allowances> All, int totalWorkingDays, double totalPaybleDays, decimal WAR_Outstation_Allowance_Calculated, decimal WAR_Attendance_Allowance_Calculated, decimal WAR_Nightshift_Allowance_Calculated, decimal WAR_Performance_Allowance_Calculated)
        {
            decimal sum = 0M;
            string[] Add_Formula;
            string[] Deduct_Formula;

            var List_CRI_Formula = new List<string>();

            if (CRI_Formula != null)
            {
                string[] arr_CRI_Formula;
                if (CRI_Formula.Contains("-"))
                {
                    string Add = CRI_Formula.Substring(0, CRI_Formula.IndexOf("-"));
                    string Deduct = CRI_Formula.Substring(CRI_Formula.IndexOf("-") + 1);
                    Add_Formula = Add.Split("+");
                    Deduct_Formula = Deduct.Split("-");
                    List_CRI_Formula.AddRange(Add_Formula);
                    List_CRI_Formula.AddRange(Deduct_Formula);
                    arr_CRI_Formula = List_CRI_Formula.ToArray();
                }
                else
                {
                    arr_CRI_Formula = CRI_Formula.Split("+");
                }

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
                        case "OSL":
                            sum += Convert.ToDecimal(WAR_Outstation_Allowance_Calculated);
                            break;
                        case "AAL":
                            sum += Convert.ToDecimal(WAR_Attendance_Allowance_Calculated);
                            break;
                        case "NSL":
                            sum += Convert.ToDecimal(WAR_Nightshift_Allowance_Calculated);
                            break;
                        case "PAL":
                            sum += Convert.ToDecimal(WAR_Performance_Allowance_Calculated);
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

        public static int intRoundFigure(double value = 12.5)
        {
            int val = 0;
            if (value % 1 <= 0.5)
            {
                val = (int)value;
            }
            else
            {
                val = (int)value + 1;
            }
            return val;
        }
        public static decimal RoundFigure(decimal oldValue)
        {
            double value = (double)oldValue;
            decimal Finalval = 0;
            if (value % 1 <= 0.5)
            {
                Finalval =(int)value;
            }
            else
            {
                Finalval =(int)value + 1;
            }
            return Finalval;
        }

        public static string ConvertNumbertoWords(decimal price)
        {
            int number = (int)price;
            if (number == 0) return "Zero";
            if (number < 0) return "minus " + ConvertNumbertoWords(Math.Abs(number));
            string words = "";
            if ((number / 1000000) > 0)
            {
                words += ConvertNumbertoWords(number / 100000) + " Lakes ";
                number %= 1000000;
            }
            if ((number / 1000) > 0)
            {
                words += ConvertNumbertoWords(number / 1000) + " Thousand ";
                number %= 1000;
            }
            if ((number / 100) > 0)
            {
                words += ConvertNumbertoWords(number / 100) + " Hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "") words += "And ";
                var unitsMap = new[]
                {
            "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen"
        };
                var tensMap = new[]
                {
            "Zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety"
        };
                if (number < 20) words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0) words += " " + unitsMap[number % 10];
                }
            }
            return words;
        }

        public static DateTime GetLastDateOfMonth(DateTime date)
        {
            DateTime lastDate = new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1);
            return lastDate;
        }
        public static DateTime GetFirstDateOfMonth(DateTime date)
        {
            DateTime firstDate = new DateTime(date.Year, date.Month, 1);
            return firstDate;
        }
        public static string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        public static Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }
    }
}
