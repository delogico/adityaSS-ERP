using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models;

public partial class ProfessionalTaxCalculation
{
    public int PTC_Id { get; set; }

    public string PTC_MF { get; set; }

    public decimal PTC_From { get; set; }

    public decimal PTC_To { get; set; }

    public decimal PTC_Amount { get; set; }
}
