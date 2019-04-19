using System;
using System.Collections.Generic;
using System.Text;
using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;

namespace RMERP.DAL.Mappers
{
    public class EmployeeAdvanceMapper
    {
        public static EmployeeAdvanceVM mapMe(Employee_Advance employee_Advance)
        {
            EmployeeAdvanceVM employeeAdvanceVM = new EmployeeAdvanceVM();
            employeeAdvanceVM.ADV_Id = employee_Advance.ADV_Id;
            employeeAdvanceVM.EMP_Id = employee_Advance.EMP_Id;
            employeeAdvanceVM.ADV_Amount = employee_Advance.ADV_Amount;
            employeeAdvanceVM.ADV_Status = employee_Advance.ADV_Status;
            employeeAdvanceVM.ADV_RegisteredOn = employee_Advance.ADV_RegisteredOn;
            employeeAdvanceVM.ADM_Id_RegisteredBy = employee_Advance.ADM_Id_RegisteredBy;
            if (employee_Advance.EMP_ != null)
                employeeAdvanceVM.EmployeeName = employee_Advance.EMP_.EMP_FirstName + " " + employee_Advance.EMP_.EMP_MiddleName + " " + employee_Advance.EMP_.EMP_SurName;
            return employeeAdvanceVM;
        }
        public static Employee_Advance mapMeModel(EmployeeAdvanceVM employee_AdvanceVM)
        {
            Employee_Advance employeeAdvance = new Employee_Advance();
            employeeAdvance.ADV_Id = employee_AdvanceVM.ADV_Id;
            employeeAdvance.EMP_Id = employee_AdvanceVM.EMP_Id;
            employeeAdvance.ADV_Amount = employee_AdvanceVM.ADV_Amount;
            employeeAdvance.ADV_Status = employee_AdvanceVM.ADV_Status;
            employeeAdvance.ADV_RegisteredOn = employee_AdvanceVM.ADV_RegisteredOn;
            employeeAdvance.ADM_Id_RegisteredBy = employee_AdvanceVM.ADM_Id_RegisteredBy;
            return employeeAdvance;
        }
        public static List<EmployeeAdvanceVM> mapAdvances(List<Employee_Advance> employee_Advances)
        {
            List<EmployeeAdvanceVM> lst = new List<EmployeeAdvanceVM>();
            foreach(Employee_Advance item in employee_Advances)
            {
                lst.Add(mapMe(item));
            }
            return lst;
        }
    }
}
