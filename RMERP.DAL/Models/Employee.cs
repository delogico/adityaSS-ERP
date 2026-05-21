using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models;

public partial class Employee
{
    public int EMP_Id { get; set; }

    public string EMP_FirstName { get; set; }

    public string EMP_MiddleName { get; set; }

    public string EMP_SurName { get; set; }

    public string EMP_Aadhar_Name { get; set; }

    public string EMP_Aadhar_Number { get; set; }

    public DateOnly EMP_DOB { get; set; }

    public byte EMP_Married { get; set; }

    public DateOnly EMP_DateOfJoining { get; set; }

    /// <summary>
    /// 1:Male;0:Female
    /// </summary>
    public bool EMP_Gender { get; set; }

    public string EMP_Contact_Primary { get; set; }

    public string EMP_Contact_Secondry { get; set; }

    public string EMP_Permanent_Address { get; set; }

    public string EMP_Temporary_Address { get; set; }

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

    public bool EMP_IsActive { get; set; }

    public DateTime? EMP_InactivatedOn { get; set; }

    public int? ADM_Id_InactivatedBy { get; set; }

    public int FRM_Id { get; set; }

    /// <summary>
    /// 0:BankAccount; 1:Cheque &amp;Cash
    /// </summary>
    public int EMP_Payment_Type { get; set; }

    /// <summary>
    /// 0: IDBI to IDBI ;1: IDBI to Other;
    /// </summary>
    public int EMP_Is_IDBI_Other { get; set; }

    public string EMP_UAN_Remark { get; set; }

    public string EMP_ESIC_Remark { get; set; }

    public int? EMP_ReasonCode { get; set; }

    public int? EMP_State { get; set; }

    public int? EMP_City { get; set; }

    public DateTime? EMP_RejoinOn { get; set; }

    public string EMP_LIN_Number { get; set; }

    public string EMP_LIN_Remark { get; set; }

    public int? CBA_Id { get; set; }

    public virtual ICollection<Attendance_Summary> Attendance_Summaries { get; set; } = new List<Attendance_Summary>();

    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    public virtual Company_Bank_Account CBA { get; set; }

    public virtual ICollection<Clients_Employee> Clients_Employees { get; set; } = new List<Clients_Employee>();

    public virtual Cities_all EMP_CityNavigation { get; set; }

    public virtual State EMP_StateNavigation { get; set; }

    public virtual ICollection<Employee_Advance> Employee_Advances { get; set; } = new List<Employee_Advance>();

    public virtual ICollection<Employee_Document> Employee_Documents { get; set; } = new List<Employee_Document>();

    public virtual Firm FRM { get; set; }

    public virtual ICollection<Wage_PaySlip> Wage_PaySlips { get; set; } = new List<Wage_PaySlip>();

    public virtual ICollection<Wage_Register_Advance> Wage_Register_Advances { get; set; } = new List<Wage_Register_Advance>();

    public virtual ICollection<Wage_Register> Wage_Registers { get; set; } = new List<Wage_Register>();
}
