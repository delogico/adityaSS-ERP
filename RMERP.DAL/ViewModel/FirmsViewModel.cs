using RMERP.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RMERP.DAL.ViewModel
{
    public partial class FirmVM
    {        
        public int FRM_Id { get; set; }
        [Display(Name ="Title")]
        [Required(ErrorMessage ="Please add firm title")]
        public string FRM_Name { get; set; }
        [Display(Name = "Short Title")]
        [Required(ErrorMessage = "Please add firm short title")]
        public string FRM_ShortName { get; set; }
    }
}

