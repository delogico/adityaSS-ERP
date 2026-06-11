using RMERP.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMERP.DAL.ViewModel
{
    public class EmployeePaySlipVM
    {        
        public Firm firm { get; set; }
        public DateTime WAG_Month { get; set; }
        public int EMP_Id { get; set; }
        public string EMP_FirstName { get; set; }
        public string EMP_MiddleName { get; set; }
        public string EMP_SurName { get; set; }
        public string EMP_FullName
        {
            get
            {
                return string.Concat(EMP_FirstName + " " + EMP_MiddleName + " " + EMP_SurName);
            }
        }
        public bool EMP_Gender { get; set; }
        public DateTime EMP_DateOfJoining { get; set; }
        public string EMP_Pan_Number { get; set; }
        public string EMP_UAN_Number { get; set; }        
        public string EMP_Region { get; set; }
        public double WAR_TotalPaybleDays { get; set; }
        public double WAR_TotalWorkingDays { get; set; }
        public string EMP_Bank { get; set; }
        public string EMP_Branch { get; set; }
        public string EMP_Account_Number { get; set; }
        public string EMP_PF_Number { get; set; }
        public string EMP_ESIC_Number { get; set; }
        public string EMP_Designation { get; set; }
        public string EMP_Location { get; set; }
        public int ArrearsDays { get; set; }
        public decimal WAR_Basic_Calculated { get; set; }
        public decimal WAR_HRA_Calculated { get; set; }
        public decimal WAR_LeaveAndPH_Calculated { get; set; }
        public decimal WAR_DA_Calculated { get; set; }
        public List<Wage_Register_Allowance> Wage_Register_Allowances { get; set; }
        public decimal WAR_GrossTotal { get; set; }
        public decimal WAR_PF_Calculated { get; set; }
        public decimal WAR_ESIC_Calculated { get; set; }
        public decimal WAR_ProffesionalTax_Calculated { get; set; }
        public decimal WAR_FinalTotal { get; set; }
        public decimal WAR_LWF_Deduction_Calculated { get; set; }
        public decimal WAR_Advance_Amount { get; set; }
        public decimal WAR_RevenueDeduction_Calculated { get; set; }
        public decimal WAR_CanteenFacility_Calculation { get; set; }        
        public decimal DeductTotal { get; set; }
        public decimal WAR_OverTime_Calculated { get; set; }
        public decimal WAR_Outstation_Allowance_Calculated { get; set; }
        public decimal WAR_Nightshift_Allowance_Calculated { get; set; }
        public decimal WAR_Performance_Allowance_Calculated { get; set; }
        public decimal WAR_Attendance_Allowance_Calculated { get; set; }


        public string CRI_Allowance_Name_1 { get; set; }       
        public string CRI_Allowance_Name_2 { get; set; }        
        public string CRI_Allowance_Name_3 { get; set; }       
        public string CRI_Allowance_Name_4 { get; set; }        
        public string CRI_Allowance_Name_5 { get; set; }
        public string CRI_Allowance_Name_6 { get; set; }       
        public string CRI_Allowance_Name_7 { get; set; }        
        public string CRI_Allowance_Name_8 { get; set; }       
        public string CRI_Allowance_Name_9 { get; set; }        
        public string CRI_Allowance_Name_10 { get; set; }

        public decimal WAR_Allowance_Calculated_1 { get; set; }
        public decimal WAR_Allowance_Calculated_2 { get; set; }
        public decimal WAR_Allowance_Calculated_3 { get; set; }
        public decimal WAR_Allowance_Calculated_4 { get; set; }
        public decimal WAR_Allowance_Calculated_5 { get; set; }
        public decimal WAR_Allowance_Calculated_6 { get; set; }
        public decimal WAR_Allowance_Calculated_7 { get; set; }
        public decimal WAR_Allowance_Calculated_8 { get; set; }
        public decimal WAR_Allowance_Calculated_9 { get; set; }
        public decimal WAR_Allowance_Calculated_10 { get; set; }

		public AttendanceSummaryVM AttendanceSummaries { get; set; }
	}
}
