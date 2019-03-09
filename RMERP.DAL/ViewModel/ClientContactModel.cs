using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RMERP.DAL.ViewModel
{
    public class ClientContactModel
    {
        [Key]
        public int ConId { get; set; }
        public int CliId { get; set; }
        public string ClientName { get; set; }
        [Display(Name = "First Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please insert first name")]
        public string ConFirstName { get; set; }
        [Display(Name = "Last Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please insert last name")]
        public string ConSurName { get; set; }
        [Required(ErrorMessage = "Please insert designation")]
        [Display(Name = "Designation")]
        public string ConDesignation { get; set; }
        [Display(Name = "Mobile")]
        [Required(ErrorMessage = "Please insert mobile")]
        public string ConMobile { get; set; }
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Please insert Email")]
        public string ConEmail { get; set; }
        public int AdmIdRegisteredBy { get; set; }
        public DateTime ConRegisteredOn { get; set; }
        [Display(Name = "Set as primary contact ?")]
        [Required(ErrorMessage = "Please select if you wants to make contact as primary")]
        public bool ConIsPrimary { get; set; }
    }
}
