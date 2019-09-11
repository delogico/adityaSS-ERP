using RMERP.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RMERP.DAL.ViewModel
{
    public partial class DesignationVM
    {
        public int DES_Id { get; set; }

        [Required(ErrorMessage ="Please add title")]
        [Display( Name ="Title")]
        public string DES_Title { get; set; }
        [Display(Name = "Exclude LWF ?")]
        public bool DES_Exclude_LWF { get; set; }
    }
}

