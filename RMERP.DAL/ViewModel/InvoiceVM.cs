using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using RMERP.DAL.Models;

namespace RMERP.DAL.ViewModel
{
    public class InvoiceMasterVM
    {
        public int CLI_Id { get; set; }      
        public List<InvoiceVM> InvoiceVMs { get; set; }
    }
    public class InvoiceVM
    {
        public int INV_Id { get; set; }
        public int FRM_Id { get; set; }
        [Display(Name = "Clients")]
        [Required(ErrorMessage ="Client is required.")]
        public int CLI_Id { get; set; }
        [Display(Name = "Invoice Number")]
        [Required(ErrorMessage = "Invoice Number is required.")]
        public string INV_Number { get; set; }
        [Display(Name = "Invoice Date")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Date is required.")]
        public DateTime INV_Date { get; set; }
        [Display(Name = "Order Number")]
        public string INV_ClientOrder_Number { get; set; }
        [Display(Name = "Order Date")]
        [DataType(DataType.Date)]
        public DateTime? INV_ClientOrder_Date { get; set; }
        [Display(Name = "HSN")]
        public string INV_HSN { get; set; }
        [Display(Name = "Total")]
        [Required(ErrorMessage = "Total is required.")]
        public decimal INV_Total { get; set; }
        [Required]
        public int ADM_Id_CreatedBy { get; set; }
        [Required(ErrorMessage = "Date is required.")]
        public DateTime INV_CreatedOn { get; set; }
        [Display(Name = "CGST(%)")]
        [Required(ErrorMessage = "CGST is required.")]
        public double INV_CGST_Percentage { get; set; }
        [Display(Name = "SGST(%)")]
        [Required(ErrorMessage = "SGST is required.")]
        public double INV_SGST_Percentage { get; set; }
        [Display(Name = "IGST(%)")]
        [Required(ErrorMessage = "IGST is required.")]
        public double INV_IGST_Percentage { get; set; }
        [Display(Name = "Total CGST")]
        public decimal INV_CGST_Total { get; set; }
        [Display(Name = "Total SGST")]
        public decimal INV_SGST_Total { get; set; }
        [Display(Name = "Total IGST")]
        public decimal INV_IGST_Total { get; set; }

        public List<Invoice_ConceptsVM> Invoice_Concepts { get; set; }

        public decimal All_INC_Total { get; set; }
        public decimal totTAX { get; set; }

        public Clients CLI_ { get; set; }
        public Firms FRM_ { get; set; }
    }
    public class Invoice_ConceptsVM
    {
        public int INC_Id { get; set; }
        public int INV_Id { get; set; }
        public int INC_Serial_Number { get; set; }
        public string INC_Description { get; set; }
        public decimal INC_Total { get; set; }
        public int hdnId { get; set; }
        public Invoices INV_ { get; set; }
    }
    
}
