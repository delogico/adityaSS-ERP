using RMERP.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RMERP.DAL.ViewModel
{
    public partial class FirmsViewModel
    {        
        public int FRM_Id { get; set; }

        [Required(ErrorMessage ="Please add firm name")]
        public string FRM_Name { get; set; }
    }
}

