using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models
{
    public partial class WageProcess
    {
        public WageProcess()
        {
            Attendance = new HashSet<Attendance>();
        }

        public int WagId { get; set; }
        public DateTime WagMonth { get; set; }
        public DateTime WagRegisteredOn { get; set; }
        public int AdmIdRegisteredBy { get; set; }

        public ICollection<Attendance> Attendance { get; set; }
    }
}
