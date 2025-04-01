using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models;

public partial class Invoice
{
    public int INV_Id { get; set; }

    public int FRM_Id { get; set; }

    public int CLI_Id { get; set; }

    public string INV_Number { get; set; }

    public DateOnly INV_Date { get; set; }

    public string INV_ClientOrder_Number { get; set; }

    public DateOnly? INV_ClientOrder_Date { get; set; }

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

    public string INV_Remark { get; set; }

    public virtual Client CLI { get; set; }

    public virtual Firm FRM { get; set; }

    public virtual ICollection<Invoice_Concept> Invoice_Concepts { get; set; } = new List<Invoice_Concept>();
}
