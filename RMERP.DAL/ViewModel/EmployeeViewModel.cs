using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RMERP.DAL.ViewModel
{
    public class EmployeeViewModel
    {
        public EmployeeModel EmployeeModel { get; set; }
        public IEnumerable<EmployeeModel> ListEmployeeModels { get; set; }
    }
    public class EmployeeModel
    {
        [Key]
        public int EmpId { get; set; }

        [Display(Name ="Name")]
        [Required(AllowEmptyStrings =false, ErrorMessage ="first name is required")]
        public string EmpFirstName { get; set; }

        [Display(Name = "Middle name")]
        public string EmpMiddleName { get; set; }

        [Display(Name = "Last name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Last name is required")]
        public string EmpSurName { get; set; }

        [Display(Name = "Name as per aadhar card")]
        public string EmpAadharName { get; set; }

        [Display(Name = "Aadhar card number")]
        public string EmpAadharNumber { get; set; }

        [Display(Name = "Date of birth")]
        [Required(ErrorMessage = "Date of birth is required")]
        [DataType(DataType.Date)]
        public DateTime EmpDob { get; set; }

        [Display(Name = "Marital status")]
        [Required(ErrorMessage = "Marital status is required")]
        public byte EmpMarried { get; set; }

        [Display(Name = "Date of joining")]
        [Required(ErrorMessage = "Date of joining is required")]
        [DataType(DataType.Date)]
        public DateTime EmpDateOfJoining { get; set; }

        [Display(Name = "Gender")]
        [Required(ErrorMessage = "Gender is required")]
        public bool EmpGender { get; set; }

        [Display(Name = "Primary contact number")]
        public string EmpContactPrimary { get; set; }

        [Display(Name = "Secondry contact number")]
        public string EmpContactSecondry { get; set; }

        [Display(Name = "Address")]
        public string EmpAddress { get; set; }

        [Display(Name = "Designation")]
        public string EmpDesignation { get; set; }

        [Display(Name = "PanCard number")]
        public string EmpPanNumber { get; set; }

        [Display(Name = "ESIC account number")]
        public string EmpEsicNumber { get; set; }

        [Display(Name = "UAN number")]
        public string EmpUanNumber { get; set; }

        [Display(Name = "Department")]
        [Required(ErrorMessage = "Department is required")]
        public int DeptId { get; set; }

        [Display(Name = "Employee Number")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Employee number is required")]
        public int EmpEmployeeNumberOffice { get; set; }

        [Display(Name ="TPC Employee ID")]
        public string EmpTpcEmployeeId { get; set; }

        [Required]
        public DateTime EmpRegisteredOn { get; set; }

        [Required]
        public int AdmIdRegisteredBy { get; set; }

        [Required]
        public bool EmpIsActive { get; set; }
        [DataType(DataType.Date)]
        public DateTime? EmpInactivatedOn { get; set; }
        public int? AdmIdInactivatedBy { get; set; }

        public string EmpFullName {
            get
            {
                return string.Concat(EmpFirstName + "" + EmpMiddleName + "" + EmpSurName);
            }
        }

    }
}
