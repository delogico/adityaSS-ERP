using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models
{
    public partial class Employees
    {
        public Employees()
        {
            Attendance = new HashSet<Attendance>();
            Clients_Employees = new HashSet<Clients_Employees>();
            Employee_Advance = new HashSet<Employee_Advance>();
            Employee_Documents = new HashSet<Employee_Documents>();
            Wage_PaySlips = new HashSet<Wage_PaySlips>();
            Wage_Register = new HashSet<Wage_Register>();
            Wage_Register_Advances = new HashSet<Wage_Register_Advances>();
        }

        public int EMP_Id { get; set; }
        public string EMP_FirstName { get; set; }
        public string EMP_MiddleName { get; set; }
        public string EMP_SurName { get; set; }
        public string EMP_Aadhar_Name { get; set; }
        public string EMP_Aadhar_Number { get; set; }
        public DateTime EMP_DOB { get; set; }
        public byte EMP_Married { get; set; }
        public DateTime EMP_DateOfJoining { get; set; }
        public bool EMP_Gender { get; set; }
        public string EMP_Contact_Primary { get; set; }
        public string EMP_Contact_Secondry { get; set; }
        public string EMP_Address { get; set; }
        public string EMP_Designation { get; set; }
        public string EMP_Pan_Number { get; set; }
        public string EMP_ESIC_Number { get; set; }
        public string EMP_UAN_Number { get; set; }
        public int EMP_EmployeeNumber_Office { get; set; }
        public string EMP_TPC_EmployeeId { get; set; }
        public string EMP_Account_Name { get; set; }
        public string EMP_Account_Number { get; set; }
        public string EMP_Bank { get; set; }
        public string EMP_Branch { get; set; }
        public string EMP_Bank_IFSC { get; set; }
        public DateTime EMP_RegisteredOn { get; set; }
        public int ADM_Id_RegisteredBy { get; set; }
        public bool? EMP_IsActive { get; set; }
        public DateTime? EMP_InactivatedOn { get; set; }
        public int? ADM_Id_InactivatedBy { get; set; }
        public int FRM_Id { get; set; }
        public int EMP_Payment_Type { get; set; }
        public int EMP_Is_IDBI_Other { get; set; }
        public string EMP_UAN_Remark { get; set; }
        public string EMP_ESIC_Remark { get; set; }
        public int? EMP_ReasonCode { get; set; }
        public int? EMP_State { get; set; }
        public int? EMP_City { get; set; }
        public DateTime? EMP_RejoinOn { get; set; }

        public Cities_all EMP_CityNavigation { get; set; }
        public States EMP_StateNavigation { get; set; }
        public Firms FRM_ { get; set; }
        public ICollection<Attendance> Attendance { get; set; }
        public ICollection<Clients_Employees> Clients_Employees { get; set; }
        public ICollection<Employee_Advance> Employee_Advance { get; set; }
        public ICollection<Employee_Documents> Employee_Documents { get; set; }
        public ICollection<Wage_PaySlips> Wage_PaySlips { get; set; }
        public ICollection<Wage_Register> Wage_Register { get; set; }
        public ICollection<Wage_Register_Advances> Wage_Register_Advances { get; set; }
    }
}
