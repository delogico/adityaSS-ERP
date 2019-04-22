using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RMERP.DAL.ViewModel
{
    public class EmployeeAdvanceVM
    {
        [Key]
        public int ADV_Id { get; set; }
        [Display(Name = "Employee")]
        [Required(ErrorMessage = "Employee is required")]
        public int EMP_Id { get; set; }
        [Display(Name ="Amount Taken")]
        [Required(ErrorMessage = "Advance amount is required")]
        [RegularExpression(@"^(0|-?\d{0,16}(\.\d{0,2})?)$")]
        public decimal ADV_Amount { get; set; }
        [Display(Name = "Amount taken on")]
        [Required(ErrorMessage = "Date is required")]
        [DataType(DataType.Date)]
        public DateTime ADV_RegisteredOn { get; set; } = DateTime.Today;
        public int ADM_Id_RegisteredBy { get; set; }
        public bool ADV_Status { get; set; }
        public string EmployeeName { get; set; }
    }
}
