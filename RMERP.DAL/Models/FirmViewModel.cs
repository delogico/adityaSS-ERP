using System;
using System.Collections.Generic;
using System.Text;

namespace RMERP.DAL.Models
{
    public class FirmViewModel
    {
        public IEnumerable<Firms> listFirm { get; set; }
        public Firms Firms { get; set; }
    }
}
