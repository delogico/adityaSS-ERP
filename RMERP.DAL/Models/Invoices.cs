using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models
{
    public partial class Invoices
    {
        public Invoices()
        {
            Invoice_Concepts = new HashSet<Invoice_Concepts>();
        }

        public int INV_Id { get; set; }
        public int FRM_Id { get; set; }
        public int CLI_Id { get; set; }
        public string INV_Number { get; set; }
        public DateTime INV_Date { get; set; }
        public string INV_ClientOrder_Number { get; set; }
        public DateTime? INV_ClientOrder_Date { get; set; }
        public string INV_HSN { get; set; }
        public decimal INV_Total { get; set; }
        public int ADM_Id_CreatedBy { get; set; }
        public DateTime INV_CreatedOn { get; set; }
        public double INV_CGST_Percentage { get; set; }
        public double INV_SGST_Percentage { get; set; }
        public double INV_IGST_Percentage { get; set; }
        public decimal? INV_CGST_Total { get; set; }
        public decimal? INV_SGST_Total { get; set; }
        public decimal? INV_IGST_Total { get; set; }

        public Clients CLI_ { get; set; }
        public Firms FRM_ { get; set; }
        public ICollection<Invoice_Concepts> Invoice_Concepts { get; set; }
    }
}
