using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models
{
    public partial class Employees
    {
        public Employees()
        {
            Attendance = new HashSet<Attendance>();
            ClientsEmployees = new HashSet<ClientsEmployees>();
        }

        public int EmpId { get; set; }
        public string EmpFirstName { get; set; }
        public string EmpMiddleName { get; set; }
        public string EmpSurName { get; set; }
        public string EmpAadharName { get; set; }
        public string EmpAadharNumber { get; set; }
        public DateTime EmpDob { get; set; }
        public byte EmpMarried { get; set; }
        public DateTime EmpDateOfJoining { get; set; }
        public bool EmpGender { get; set; }
        public string EmpContactPrimary { get; set; }
        public string EmpContactSecondry { get; set; }
        public string EmpAddress { get; set; }
        public string EmpDesignation { get; set; }
        public string EmpPanNumber { get; set; }
        public string EmpEsicNumber { get; set; }
        public string EmpUanNumber { get; set; }
        public int DeptId { get; set; }
        public int EmpEmployeeNumberOffice { get; set; }
        public string EmpTpcEmployeeId { get; set; }
        public DateTime EmpRegisteredOn { get; set; }
        public int AdmIdRegisteredBy { get; set; }
        public bool? EmpIsActive { get; set; }
        public DateTime? EmpInactivatedOn { get; set; }
        public int? AdmIdInactivatedBy { get; set; }

        public Departments Dept { get; set; }
        public ICollection<Attendance> Attendance { get; set; }
        public ICollection<ClientsEmployees> ClientsEmployees { get; set; }
    }
}
