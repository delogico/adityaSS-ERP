using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RMERP.DAL.ViewModel
{
    public class AdminUsersModel
    {
        [Key]
        public int ADM_Id { get; set; }
        [Display(Name = "First Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "First name is required")]
        public string ADM_FirstName { get; set; }
        [Display(Name = "Middle Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Middle  name is required")]
        public string ADM_MiddleName { get; set; }
        [Display(Name = "Last Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Last name is required")]
        public string ADM_LastName { get; set; }
        [Display(Name = "Email")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Emai is required")]
        public string ADM_EmailId { get; set; }
        [Display(Name = "Password")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string ADM_Password { get; set; }
        [Display(Name = "Mobile")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Mobile is required")]
        public string ADM_Mobile { get; set; }
        [Display(Name = "Firm")]       
        public int? FRM_Id { get; set; }
        public string FrmName { get; set; }

       
    }
}
