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
        public int DEPT_Id { get; set; }
        public int EMP_EmployeeNumber_Office { get; set; }
        public string EMP_TPC_EmployeeId { get; set; }
        public DateTime EMP_RegisteredOn { get; set; }
        public int ADM_Id_RegisteredBy { get; set; }
        public bool? EMP_IsActive { get; set; }
        public DateTime? EMP_InactivatedOn { get; set; }
        public int? ADM_Id_InactivatedBy { get; set; }

        public Departments DEPT_ { get; set; }
        public ICollection<Attendance> Attendance { get; set; }
        public ICollection<Clients_Employees> Clients_Employees { get; set; }
        public ICollection<Employee_Advance> Employee_Advance { get; set; }
    }
}
