using Microsoft.AspNetCore.Mvc;
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

        [Display(Name = "Aadhar number")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Aadhar number is required")]
        [Remote("CheckExistingAadhar", "Employees", ErrorMessage = "Aadhar number is already exists!",AdditionalFields ="EMP_Id")]
        public string EMP_Aadhar_Number { get; set; }

        [Display(Name = "Date of birth")]
        [Required(ErrorMessage = "Date of birth is required")]
        [DataType(DataType.Date)]
        public DateTime EMP_DOB { get; set; }= DateTime.Today;

        [Display(Name = "Marital status")]
        [Required(ErrorMessage = "Marital status is required")]
        public byte EMP_Married { get; set; }

        [Display(Name = "Date of joining")]
        [Required(ErrorMessage = "Date of joining is required")]
        [DataType(DataType.Date)]
        public DateTime EMP_DateOfJoining { get; set; } = DateTime.Today;

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

        [Display(Name = "Pan number")]
        public string EMP_Pan_Number { get; set; }

        [Display(Name = "ESIC account number")]
        [Required(ErrorMessage = "ESIC account number is required")]
        public string EMP_ESIC_Number { get; set; }

        [Display(Name = "UAN number")]
        [Required(ErrorMessage = "UAN number is required")]
        public string EMP_UAN_Number { get; set; }

        [Display(Name = "Employee Number")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Employee number is required")]
        public int EMP_EmployeeNumber_Office { get; set; } = 0;

        [Display(Name = "TPC Employee ID")]
        public string EMP_TPC_EmployeeId { get; set; }

        [Display(Name = "Account Name")]
        public string EMP_Account_Name { get; set; }
        [Display(Name = "Account Number")]
        public string EMP_Account_Number { get; set; }
        [Display(Name = "Bank")]
        public string EMP_Bank { get; set; }
        [Display(Name = "Branch")]
        public string EMP_Branch { get; set; }
        [Display(Name = "IFSC Code")]
        public string EMP_Bank_IFSC { get; set; }

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
     
        public string EMP_MoF
        {
            get
            {
                return EMP_Gender == true ? "M" : "F";
            }
        }
        public ICollection<Attendance> Attendance { get; set; }
        public ICollection<Clients_Employees> Clients_Employees { get; set; }
        public List<EmployeeAdvanceVM> advances { get; set; }
        public List<WageRegisterAdvancesVM> wageRegisterAdvances { get; set; }

        public EmployeeDocumentsVM employee_Document { get; set; }
        public List<Document_Types> document_Types { get; set; }
        public List<Employee_Documents> employee_Documents { get; set; }

        [Required]
        [Display(Name ="Select Firm")]
        public int FRM_Id { get; set; }

        public Firms FRM_ { get; set; }

        public bool IsAssigned { get; set; }
    }
}

