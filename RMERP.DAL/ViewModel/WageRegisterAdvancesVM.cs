using RMERP.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RMERP.DAL.ViewModel
{
    public class WageRegisterAdvancesVM
    {
        [Key]
        public int WAD_Id { get; set; }
        public int WAG_Id { get; set; }
        public int EMP_Id { get; set; }
        [Display(Name ="Amount")]
        [Required(ErrorMessage ="Amount is required")]
        public decimal WAD_Amount { get; set; }
        public int? CLI_Id { get; set; }
        [Display(Name = "Status")]
        public bool WAD_Status { get; set; } = false;
        public DateTime? WAD_ClosedOn { get; set; }
        public Employees EMP_ { get; set; }
    }
}
