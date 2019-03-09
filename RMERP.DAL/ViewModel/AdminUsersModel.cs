using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RMERP.DAL.ViewModel
{
    public class AdminUsersModel
    {
        [Key]
        public int AdmId { get; set; }
        [Display(Name = "First Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "First name is required")]
        public string AdmFirstName { get; set; }
        [Display(Name = "Middle Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Middle  name is required")]
        public string AdmMiddleName { get; set; }
        [Display(Name = "Last Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Last name is required")]
        public string AdmLastName { get; set; }
        [Display(Name = "Email")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Emai is required")]
        public string AdmEmailId { get; set; }
        [Display(Name = "Password")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
        public string AdmPassword { get; set; }
        [Display(Name = "Mobile")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Mobile is required")]
        public string AdmMobile { get; set; }
        [Display(Name = "Firm")]       
        public int? FrmId { get; set; }
        public string FrmName { get; set; }
    }
}
