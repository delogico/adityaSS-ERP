using System;
using System.Collections.Generic;
using System.Text;

namespace RMERP.DAL.ViewModel
{
    public class Wage_Register_CanteenVM
    {
        public int WRC_Id { get; set; }
        public int WAG_Id { get; set; }
        public int CLE_Id { get; set; }
        public int CRI_Id { get; set; }
        public decimal WRC_Amount { get; set; }
        public int Emp_ID { get; set; }
        public int FRM_ID { get; set; }
        public string Emp_Name { get; set; }
    }

    public class updateWageRegisterCanteen
    {
        public List<Wage_Register_CanteenVM> canteenVMs { get; set; }
        public int CLI_Id { get; set; }
    }
}
