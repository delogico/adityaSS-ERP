
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using RMERP.DAL.Models;

namespace RMERP.DAL.ViewModel
{
    public class ClientReqAllowanceVM
    {
        [Key]
        public int CRA_Id { get; set; }
        [Required]
        public int CRI_Id { get; set; }
        [Required(ErrorMessage = "Allowances is required")]
        [Display(Name ="Allowances")]
        public int ALL_Id { get; set; }
        [Required(ErrorMessage = "Allowance Amount is required")]
        [Display(Name = "Allowance Amount")]
        public decimal CRA_Amount { get; set; } = 0M;
        [Required(ErrorMessage = "Select calculation method")]
        [Display(Name = "Calculation on daywise OR full ?")]
        public bool CRA_DayswiseOrFull { get; set; }
        public AllowanceVM allowance { get; set; }
        public ClientRequirementVM requirement { get; set; }
        public Boolean flagClientRequirement { get; set; }
    }
}
