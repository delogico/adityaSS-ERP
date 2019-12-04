using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMERP.DAL.Mappers
{
    public class WagePaySlipMapper
    {
        public static EmployeePaySlipVM mapMe(Wage_PaySlips Wage_PaySlip)
        {
            EmployeePaySlipVM paySlipVM = new EmployeePaySlipVM();
            
            return paySlipVM;
        }
    }
    
    
}
