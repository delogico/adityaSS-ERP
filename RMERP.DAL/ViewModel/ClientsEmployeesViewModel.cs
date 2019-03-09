using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RMERP.DAL.ViewModel
{
    public class ClientsEmployeesViewModel
    {
 
        public int CleId { get; set; }
        public int CliId { get; set; }
        [Display(Name = "Employee")]
        [Required(ErrorMessage ="Employee is required")]
        public int EmpId { get; set; }
        [Display(Name = "Designation")]
        [Required(ErrorMessage = "Designation is required")]
        public int DesId { get; set; }
        public DateTime CleRegisteredOn { get; set; }
        public int AdmIdRegisteredBy { get; set; }
    }
}
