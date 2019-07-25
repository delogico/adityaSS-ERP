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
        [Display(Name = "Registered On")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Registeration date is required")]
        public DateTime CLE_RegisteredOn { get; set; } = DateTime.Now;

        public DateTime? CLE_UnassignedOn { get; set; }
        public int? ADM_Id_UnassignedBy { get; set; }

        public int ADM_Id_RegisteredBy { get; set; }       
        public string DES_Title { get; set; }
        public EmployeeVM employee { get; set; }

        public bool IsEmployeeWagedForClient { get; set; }
    }
}
