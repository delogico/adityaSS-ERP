using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RMERP.DAL.ViewModel
{
    public class ClientEmployeeVM
    {
 
        public int CLE_Id { get; set; }
        public int CLI_Id { get; set; }
        [Display(Name = "Employee")]
        [Required(ErrorMessage ="Employee is required")]
        public int EMP_Id { get; set; }
        [Display(Name = "Designation")]
        [Required(ErrorMessage = "Designation is required")]
        public int DES_Id { get; set; } 
        public DateTime CLE_RegisteredOn { get; set; }
        public int ADM_Id_RegisteredBy { get; set; }
        public string DES_Title { get; set; }
        public EmployeeVM employee { get; set; }
    }
}
