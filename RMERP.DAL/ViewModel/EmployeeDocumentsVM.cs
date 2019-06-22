using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RMERP.DAL.ViewModel
{
    public class EmployeeDocumentsVM
    {
        public int EMD_Id { get; set; }
        [Required]
        public int EMP_Id { get; set; }
        [Display(Name ="Document type")]
        [Required(ErrorMessage ="Please select document type")]
        public int DOT_Id { get; set; }
        [Display(Name ="Document")]
        [Required(ErrorMessage ="Document is required")]
        public IFormFile EMD_Document { get; set; }
        public string EMD_Name { get; set; }
        public DateTime EMD_UploadedOn { get; set; }
    }
}
