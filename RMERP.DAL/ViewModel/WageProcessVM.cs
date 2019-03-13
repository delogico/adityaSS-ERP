using System;
using System.Collections.Generic;
using System.Text;
using RMERP.DAL.Models;

namespace RMERP.DAL.ViewModel
{
    public class WageProcessVM
    {
        public int WagId { get; set; }
        public DateTime WagMonth { get; set; }
        public List<Attendance> Attendance { get; set; }

        public string WAG_Month()
        {
            return WagMonth.ToString("MMMM") + "-" + WagMonth.ToString("yyyy");
        }
    }
}
