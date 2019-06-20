using RMERP.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RMERP.DAL.ViewModel
{
    public partial class CitiesVM
    {        
        public int CITY_Id { get; set; }
        [Display(Name ="Title")]
        [Required(ErrorMessage ="Please add City")]
        public string CITY_Name { get; set; }

    }
}

