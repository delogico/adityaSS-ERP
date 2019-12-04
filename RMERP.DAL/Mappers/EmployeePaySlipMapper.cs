using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMERP.DAL.Mappers
{
    public class EmployeePaySlipMapper
    {
        public static EmployeePaySlipVM mapMe(Employees employee)
        {
            EmployeePaySlipVM paySlipVM = new EmployeePaySlipVM();
            paySlipVM.EMP_Id = employee.EMP_Id;
            paySlipVM.EMP_FirstName = employee.EMP_FirstName;
            paySlipVM.EMP_MiddleName = employee.EMP_MiddleName;
            paySlipVM.EMP_SurName = employee.EMP_SurName;
            if (employee.Wage_PaySlips != null)
            {
                paySlipVM.IsPaySlipGenerated = true;
              //  paySlipVM.WPS_GeneratedOn = employee.Wage_PaySlips;
            }
            else
            {
                paySlipVM.IsPaySlipGenerated = false;
            }
            
            return paySlipVM;
        }
        public static List<EmployeePaySlipVM> mapMe(List<Employees> employees)
        {
            List<EmployeePaySlipVM> paySlipVMs = new List<EmployeePaySlipVM>();
            foreach(Employees employee in employees)
            {
                paySlipVMs.Add(mapMe(employee));
            }
            return paySlipVMs;
        }
    }
}