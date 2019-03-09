using RMERP.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RMERP.DAL.ViewModel
{
    public partial class FirmsViewModel
    {        
        public int FrmId { get; set; }

        [Required(ErrorMessage ="Please add firm name")]
        public string FrmName { get; set; }
    }
}
