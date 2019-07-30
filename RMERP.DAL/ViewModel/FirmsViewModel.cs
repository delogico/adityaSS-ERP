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
        [Required(ErrorMessage ="Please add title")]
        public string FRM_Name { get; set; }

        [Display(Name = "Short Title")]
        [Required(ErrorMessage = "Please add short title")]
        public string FRM_ShortName { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Please add email")]
        [DataType(DataType.EmailAddress)]
        public string FRM_Email { get; set; }

        [Display(Name = "Address 1")]
        [Required(ErrorMessage = "Please add address")]
        public string FRM_Address1 { get; set; }

        [Display(Name = "Address 2")]
        [Required(ErrorMessage = "Please add address")]
        public string FRM_Address2 { get; set; }

        [Display(Name = "Invoicing Name")]
        [Required(ErrorMessage = "Please add invoicing name")]
        public string FRM_InvoicingName { get; set; }

        [Display(Name = "GST No")]
        public string FRM_GST_No { get; set; }

        [Display(Name = "Bank")]       
        public string FRM_BankName { get; set; }

        [Display(Name = "Account Number")]
        public string FRM_AccountNumber { get; set; }

        [Display(Name = "IFSC Code")]
        public string FRM_IFSC_Code { get; set; }
    }
}

