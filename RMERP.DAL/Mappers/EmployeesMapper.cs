using System;
using System.Collections.Generic;
using System.Linq;
using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;

namespace RMERP.DAL.Mappers
{
    public class EmployeesMapper
    {
        public static EmployeeVM MapMe(Employees employee)
        {
            EmployeeVM emp = new EmployeeVM();
            emp.EMP_Id = employee.EMP_Id;
            emp.EMP_FirstName = employee.EMP_FirstName;
            emp.EMP_MiddleName = employee.EMP_MiddleName;
            emp.EMP_SurName = employee.EMP_SurName;
            emp.EMP_Aadhar_Name = employee.EMP_Aadhar_Name;
            emp.EMP_Aadhar_Number = employee.EMP_Aadhar_Number;
            emp.EMP_DOB = employee.EMP_DOB;
            emp.EMP_Married = employee.EMP_Married;
            emp.EMP_DateOfJoining = employee.EMP_DateOfJoining;
            emp.EMP_Gender = employee.EMP_Gender;
            emp.EMP_Contact_Primary = employee.EMP_Contact_Primary;
            emp.EMP_Contact_Secondry = employee.EMP_Contact_Secondry;
            emp.EMP_Address = employee.EMP_Address;
            emp.EMP_Designation = employee.EMP_Designation;
            emp.EMP_Pan_Number = employee.EMP_Pan_Number;
            emp.EMP_ESIC_Number = employee.EMP_ESIC_Number;
            emp.EMP_UAN_Number = employee.EMP_UAN_Number;
            emp.DEPT_Id = employee.DEPT_Id;
            emp.EMP_EmployeeNumber_Office = employee.EMP_EmployeeNumber_Office;
            emp.EMP_TPC_EmployeeId = employee.EMP_TPC_EmployeeId;
            emp.EMP_RegisteredOn = employee.EMP_RegisteredOn;
            emp.ADM_Id_RegisteredBy = employee.ADM_Id_RegisteredBy;
            emp.EMP_IsActive = Convert.ToBoolean(employee.EMP_IsActive);
            emp.EMP_InactivatedOn = employee.EMP_InactivatedOn;
            emp.ADM_Id_InactivatedBy = employee.ADM_Id_InactivatedBy;
            if (employee.DEPT_ != null)
                emp.DEPT_ = employee.DEPT_;
            if (employee.Employee_Advance.Count > 0)
                emp.advances = EmployeeAdvanceMapper.mapAdvances(employee.Employee_Advance.ToList());
            if (employee.Wage_Register_Advances.Count > 0)
                emp.wageRegisterAdvances = WageRegisterAdvancesMapper.mapMeModels(employee.Wage_Register_Advances.ToList());
            return emp;
        }

        public static List<EmployeeVM> MapEmployees(List<Employees> employees)
        {
            List<EmployeeVM> lst = new List<EmployeeVM>();
            foreach (Employees employee in employees)
                lst.Add(MapMe(employee));
            return lst;
        }

        public static Employees MapMeModel(EmployeeVM employee)
        {
            Employees emp = new Employees();
            emp.EMP_Id = employee.EMP_Id;
            emp.EMP_FirstName = employee.EMP_FirstName;
            emp.EMP_MiddleName = employee.EMP_MiddleName;
            emp.EMP_SurName = employee.EMP_SurName;
            emp.EMP_Aadhar_Name = employee.EMP_Aadhar_Name;
            emp.EMP_Aadhar_Number = employee.EMP_Aadhar_Number;
            emp.EMP_DOB = employee.EMP_DOB;
            emp.EMP_Married = employee.EMP_Married;
            emp.EMP_DateOfJoining = employee.EMP_DateOfJoining;
            emp.EMP_Gender = employee.EMP_Gender;
            emp.EMP_Contact_Primary = employee.EMP_Contact_Primary;
            emp.EMP_Contact_Secondry = employee.EMP_Contact_Secondry;
            emp.EMP_Address = employee.EMP_Address;
            emp.EMP_Designation = employee.EMP_Designation;
            emp.EMP_Pan_Number = employee.EMP_Pan_Number;
            emp.EMP_ESIC_Number = employee.EMP_ESIC_Number;
            emp.EMP_UAN_Number = employee.EMP_UAN_Number;
            emp.DEPT_Id = employee.DEPT_Id;
            emp.EMP_EmployeeNumber_Office = employee.EMP_EmployeeNumber_Office;
            emp.EMP_TPC_EmployeeId = employee.EMP_TPC_EmployeeId;
            emp.EMP_RegisteredOn = employee.EMP_RegisteredOn;
            emp.ADM_Id_RegisteredBy = employee.ADM_Id_RegisteredBy;
            emp.EMP_IsActive = Convert.ToBoolean(employee.EMP_IsActive);
            emp.EMP_InactivatedOn = employee.EMP_InactivatedOn;
            emp.ADM_Id_InactivatedBy = employee.ADM_Id_InactivatedBy;
            return emp;
        }
    }
}
