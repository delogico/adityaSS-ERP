using System;
using System.Collections.Generic;
using System.Text;
using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;

namespace RMERP.DAL.Mappers
{
    public class EmployeeAdvanceMapper
    {
        public static EmployeeAdvanceVM mapMeInVM(Employee_Advance employee_Advance)
        {
            EmployeeAdvanceVM employeeAdvanceVM = new EmployeeAdvanceVM();
            employeeAdvanceVM.ADV_Id = employee_Advance.ADV_Id;
            employeeAdvanceVM.EMP_Id = employee_Advance.EMP_Id;
            employeeAdvanceVM.ADV_Amount = employee_Advance.ADV_Amount;
            employeeAdvanceVM.ADV_RegisteredOn = employee_Advance.ADV_RegisteredOn;
            employeeAdvanceVM.ADM_Id_RegisteredBy = employee_Advance.ADM_Id_RegisteredBy;
            return employeeAdvanceVM;
        }
        public static Employee_Advance mapMeInOriginal(EmployeeAdvanceVM employee_AdvanceVM)
        {
            Employee_Advance employeeAdvance = new Employee_Advance();
            employeeAdvance.ADV_Id = employee_AdvanceVM.ADV_Id;
            employeeAdvance.EMP_Id = employee_AdvanceVM.EMP_Id;
            employeeAdvance.ADV_Amount = employee_AdvanceVM.ADV_Amount;
            employeeAdvance.ADV_RegisteredOn = employee_AdvanceVM.ADV_RegisteredOn;
            employeeAdvance.ADM_Id_RegisteredBy = employee_AdvanceVM.ADM_Id_RegisteredBy;
            return employeeAdvance;
        }
        public static List<EmployeeAdvanceVM> mapMeInVMList(List<Employee_Advance> employee_Advances)
        {
            List<EmployeeAdvanceVM> employeeAdvancesVM = new List<EmployeeAdvanceVM>();
            foreach(var item in employee_Advances)
            {
                EmployeeAdvanceVM employeeAdvanceVM = new EmployeeAdvanceVM();
                employeeAdvanceVM.ADV_Id = item.ADV_Id;
                employeeAdvanceVM.EMP_Id = item.EMP_Id;
                employeeAdvanceVM.ADV_Amount = item.ADV_Amount;
                employeeAdvanceVM.ADV_RegisteredOn = item.ADV_RegisteredOn;
                employeeAdvanceVM.ADM_Id_RegisteredBy = item.ADM_Id_RegisteredBy;
                employeeAdvancesVM.Add(employeeAdvanceVM);
            }
           
            return employeeAdvancesVM;
        }
    }
}
