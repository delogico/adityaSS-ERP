using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RMERP.DAL.Models;

namespace RMERP.DAL.ViewModel
{
    public class ClientContactVM
    {
        [Key]
        public int CON_Id { get; set; }
        public int CLI_Id { get; set; }
        [Display(Name = "First Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please insert first name")]
        public string CON_FirstName { get; set; }
        [Display(Name = "Last Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please insert last name")]
        public string CON_SurName { get; set; }
        [Required(ErrorMessage = "Please insert designation")]
        [Display(Name = "Designation")]
        public string CON_Designation { get; set; }
        [Display(Name = "Mobile")]
        [Required(ErrorMessage = "Please insert mobile")]
        public string CON_Mobile { get; set; }
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Please insert Email")]
        public string CON_Email { get; set; }
        public int ADM_Id_RegisteredBy { get; set; }
        public DateTime CON_RegisteredOn { get; set; }
        [Display(Name = "Set as primary contact ?")]
        [Required(ErrorMessage = "Please select if you wants to make contact as primary")]
        public bool CON_isPrimary { get; set; }

        public string CON_FullName
        {
            get
            {
                return string.Concat(CON_FirstName, " ", CON_SurName);
            }
        }
        public Clients client { get; set; }
    }    
}
