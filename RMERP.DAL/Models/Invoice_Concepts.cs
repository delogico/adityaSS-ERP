using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models
{
    public partial class Invoice_Concepts
    {
        public int INC_Id { get; set; }
        public int INV_Id { get; set; }
        public int INC_Serial_Number { get; set; }
        public string INC_Description { get; set; }
        public decimal INC_Total { get; set; }

        public Invoices INV_ { get; set; }
    }
}
