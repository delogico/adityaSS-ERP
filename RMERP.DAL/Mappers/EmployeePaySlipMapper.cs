using RMERP.DAL.Helpers;
using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMERP.DAL.Mappers
{
    public class EmployeePaySlipMapper
    {
        public static EmpPaySlipVM mapMe(Employee employee, int CLI_Id, int WAG_Id)
        {
            EmpPaySlipVM paySlipVM = new();
            paySlipVM.CLI_Id = CLI_Id;
            paySlipVM.EMP_Id = employee.EMP_Id;
            paySlipVM.EMP_FirstName = employee.EMP_FirstName;
            paySlipVM.EMP_MiddleName = employee.EMP_MiddleName;
            paySlipVM.EMP_SurName = employee.EMP_SurName;
            Wage_PaySlip paySlip = employee.Wage_PaySlips.Where(m => m.WAG_Id.Equals(WAG_Id)).FirstOrDefault();
            if (paySlip != null)
            {
                paySlipVM.WPS_Id = paySlip.WPS_Id;
                if (paySlip.WPS_Status == (int)ProjectUtils.WagePaySlip.Generated)
                {
                    paySlipVM.IsPaySlipGenerated = true;
                }
                paySlipVM.WPS_GeneratedOn = paySlip.WPS_GeneratedOn;
            }
            else
            {
                paySlipVM.IsPaySlipGenerated = false;
            }

            return paySlipVM;
        }
        public static List<EmpPaySlipVM> mapMe(List<Employee> employees, int CLI_Id, int WAG_Id)
        {
            List<EmpPaySlipVM> paySlipVMs = new List<EmpPaySlipVM>();
            foreach (Employee employee in employees)
            {
                paySlipVMs.Add(mapMe(employee, CLI_Id, WAG_Id));
            }
            return paySlipVMs;
        }
    }
}