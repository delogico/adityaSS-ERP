using System;
using System.Collections.Generic;
using System.Text;

namespace RMERP.DAL.ViewModel
{
    public class Wage_Register_OutstationVM
    {
        public int WRO_Id { get; set; }
        public int WAG_Id { get; set; }
        public int CLE_Id { get; set; }
        public int CRI_Id { get; set; }
        public double WRO_Hours { get; set; }
        public int Emp_ID { get; set; }
        public int FRM_ID { get; set; }
        public string Emp_Name { get; set; }
    }

    public class updateWageRegisterOutstation
    {
        public List<Wage_Register_OutstationVM> outstationVMs { get; set; }
    }
}
