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
        [Display(Name ="Advance Amount")]
        [Required(ErrorMessage = "Advance amount is required")]
        [RegularExpression(@"^(0|-?\d{0,16}(\.\d{0,2})?)$")]
        public decimal ADV_Amount { get; set; }
        public DateTime ADV_RegisteredOn { get; set; }
        public int ADM_Id_RegisteredBy { get; set; }

        public string EmployeeName { get; set; }
    }
}
