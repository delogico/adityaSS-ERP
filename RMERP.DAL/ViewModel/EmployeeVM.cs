using RMERP.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RMERP.DAL.ViewModel
{
    public class EmployeeVM
    {
        [Key]
        public int EMP_Id { get; set; }

        [Display(Name = "Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "first name is required")]
        public string EMP_FirstName { get; set; }

        [Display(Name = "Middle name")]
        public string EMP_MiddleName { get; set; }

        [Display(Name = "Last name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Last name is required")]
        public string EMP_SurName { get; set; }

        [Display(Name = "Name as per aadhar card")]
        public string EMP_Aadhar_Name { get; set; }

        [Display(Name = "Aadhar card number")]
        public string EMP_Aadhar_Number { get; set; }

        [Display(Name = "Date of birth")]
        [Required(ErrorMessage = "Date of birth is required")]
        [DataType(DataType.Date)]
        public DateTime EMP_DOB { get; set; }

        [Display(Name = "Marital status")]
        [Required(ErrorMessage = "Marital status is required")]
        public byte EMP_Married { get; set; }

        [Display(Name = "Date of joining")]
        [Required(ErrorMessage = "Date of joining is required")]
        [DataType(DataType.Date)]
        public DateTime EMP_DateOfJoining { get; set; }

        [Display(Name = "Gender")]
        [Required(ErrorMessage = "Gender is required")]
        public bool EMP_Gender { get; set; }

        [Display(Name = "Primary contact number")]
        public string EMP_Contact_Primary { get; set; }

        [Display(Name = "Secondry contact number")]
        public string EMP_Contact_Secondry { get; set; }

        [Display(Name = "Address")]
        public string EMP_Address { get; set; }

        [Display(Name = "Designation")]
        public string EMP_Designation { get; set; }

        [Display(Name = "PanCard number")]
        public string EMP_Pan_Number { get; set; }

        [Display(Name = "ESIC account number")]
        public string EMP_ESIC_Number { get; set; }

        [Display(Name = "UAN number")]
        public string EMP_UAN_Number { get; set; }

        [Display(Name = "Department")]
        [Required(ErrorMessage = "Department is required")]
        public int DEPT_Id { get; set; }

        [Display(Name = "Employee Number")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Employee number is required")]
        public int EMP_EmployeeNumber_Office { get; set; }

        [Display(Name = "TPC Employee ID")]
        public string EMP_TPC_EmployeeId { get; set; }

        [Required]
        public DateTime EMP_RegisteredOn { get; set; }

        [Required]
        public int ADM_Id_RegisteredBy { get; set; }

        [Required]
        public bool EMP_IsActive { get; set; }
        [DataType(DataType.Date)]
        public DateTime? EMP_InactivatedOn { get; set; }
        public int? ADM_Id_InactivatedBy { get; set; }

        public string EMP_FullName
        {
            get
            {
                return string.Concat(EMP_FirstName + " " + EMP_MiddleName + " " + EMP_SurName);
            }
        }
        public Departments DEPT_ { get; set; }
        public ICollection<Attendance> Attendance { get; set; }
        public ICollection<Clients_Employees> Clients_Employees { get; set; }
    }
}

