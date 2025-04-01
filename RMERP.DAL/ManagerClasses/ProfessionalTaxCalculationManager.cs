using RMERP.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace RMERP.DAL.ManagerClasses
{
    public class ProfessionalTaxCalculationManager
    {
        RMERPContext _context;
        public ProfessionalTaxCalculationManager(RMERPContext context)
        {
            _context = context;
        }
        public decimal GetPT(string MF, decimal GrossTotal)
        {
            return _context.ProfessionalTaxCalculations.Where(m => m.PTC_MF.Equals(MF) && GrossTotal >= m.PTC_From && GrossTotal <= m.PTC_To).FirstOrDefault().PTC_Amount;
        }
    }
}
