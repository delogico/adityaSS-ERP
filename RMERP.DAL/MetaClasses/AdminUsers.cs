using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RMERP.DAL.MetaClasses
{
    public partial class AdminUsers
    {
        public int AdmId { get; set; } = 0;
        [Required]
        [Display(Name ="First Name")]
        public string AdmFirstName { get; set; }
        [Display(Name = "Middle Name")]
        [Required]
        public string AdmMiddleName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string AdmLastName { get; set; }
        [Required]
        [Display(Name = "Email Address")]
        [DataType(DataType.EmailAddress)]
        public string AdmEmailId { get; set; }
        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string AdmPassword { get; set; }
        [Display(Name = "Confirm Password")]
        [Required]
        [Compare("AdmPassword")]
        public string AdmConfirmPassword { get; set; }
        [Required]
        [Display(Name = "Mobile No")]
        public string AdmMobile { get; set; }
    }
    
}
