using System;
using System.Collections.Generic;
using System.Text;
using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;

namespace RMERP.DAL.Mappers
{
    public class EmployeesMapper
    {
        public IEnumerable<EmployeeViewModel> MapMeList(IEnumerable<Employees> lstModel)
        {
            List<EmployeeViewModel> list = new List<EmployeeViewModel>();
            foreach(var item in lstModel)
            {
                EmployeeViewModel emp = new EmployeeViewModel();
                emp.EMP_Id = item.EMP_Id;
                emp.EMP_FirstName = item.EMP_FirstName;
                emp.EMP_MiddleName = item.EMP_MiddleName;
                emp.EMP_SurName = item.EMP_SurName;
                emp.EMP_Aadhar_Name = item.EMP_Aadhar_Name;
                emp.EMP_Aadhar_Number = item.EMP_Aadhar_Number;
                emp.EMP_DOB = item.EMP_DOB;
                emp.EMP_Married = item.EMP_Married;
                emp.EMP_DateOfJoining = item.EMP_DateOfJoining;
                emp.EMP_Gender = item.EMP_Gender;
                emp.EMP_Contact_Primary = item.EMP_Contact_Primary;
                emp.EMP_Contact_Secondry = item.EMP_Contact_Secondry;
                emp.EMP_Address = item.EMP_Address;
                emp.EMP_Designation = item.EMP_Designation;
                emp.EMP_Pan_Number = item.EMP_Pan_Number;
                emp.EMP_ESIC_Number = item.EMP_ESIC_Number;
                emp.EMP_UAN_Number = item.EMP_UAN_Number;
                emp.DEPT_Id = item.DEPT_Id;
                emp.EMP_EmployeeNumber_Office = item.EMP_EmployeeNumber_Office;
                emp.EMP_TPC_EmployeeId = item.EMP_TPC_EmployeeId;
                emp.EMP_RegisteredOn = item.EMP_RegisteredOn;
                emp.ADM_Id_RegisteredBy = item.ADM_Id_RegisteredBy;
                emp.EMP_IsActive =Convert.ToBoolean(item.EMP_IsActive);
                emp.EMP_InactivatedOn = item.EMP_InactivatedOn;
                emp.ADM_Id_InactivatedBy = item.ADM_Id_InactivatedBy;

                emp.DEPT_ = item.DEPT_;
                list.Add(emp);
            }
            IEnumerable<EmployeeViewModel> EmployeesList = list;
            return EmployeesList;
        }
        public EmployeeViewModel MapMeModel(Employees employees)
        {
            EmployeeViewModel emp = new EmployeeViewModel();
            emp.EMP_Id = employees.EMP_Id;
            emp.EMP_FirstName = employees.EMP_FirstName;
            emp.EMP_MiddleName = employees.EMP_MiddleName;
            emp.EMP_SurName = employees.EMP_SurName;
            emp.EMP_Aadhar_Name = employees.EMP_Aadhar_Name;
            emp.EMP_Aadhar_Number = employees.EMP_Aadhar_Number;
            emp.EMP_DOB = employees.EMP_DOB;
            emp.EMP_Married = employees.EMP_Married;
            emp.EMP_DateOfJoining = employees.EMP_DateOfJoining;
            emp.EMP_Gender = employees.EMP_Gender;
            emp.EMP_Contact_Primary = employees.EMP_Contact_Primary;
            emp.EMP_Contact_Secondry = employees.EMP_Contact_Secondry;
            emp.EMP_Address = employees.EMP_Address;
            emp.EMP_Designation = employees.EMP_Designation;
            emp.EMP_Pan_Number = employees.EMP_Pan_Number;
            emp.EMP_ESIC_Number = employees.EMP_ESIC_Number;
            emp.EMP_UAN_Number = employees.EMP_UAN_Number;
            emp.DEPT_Id = employees.DEPT_Id;
            emp.EMP_EmployeeNumber_Office = employees.EMP_EmployeeNumber_Office;
            emp.EMP_TPC_EmployeeId = employees.EMP_TPC_EmployeeId;
            emp.EMP_RegisteredOn = employees.EMP_RegisteredOn;
            emp.ADM_Id_RegisteredBy = employees.ADM_Id_RegisteredBy;
            emp.EMP_IsActive =Convert.ToBoolean(employees.EMP_IsActive);
            emp.EMP_InactivatedOn = employees.EMP_InactivatedOn;
            emp.ADM_Id_InactivatedBy = employees.ADM_Id_InactivatedBy;
            return emp;
        }

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
            return emp;
        }

        public Employees MapMeOriginalModel(EmployeeViewModel employees)
        {
            Employees emp = new Employees();
            emp.EMP_Id = employees.EMP_Id;
            emp.EMP_FirstName = employees.EMP_FirstName;
            emp.EMP_MiddleName = employees.EMP_MiddleName;
            emp.EMP_SurName = employees.EMP_SurName;
            emp.EMP_Aadhar_Name = employees.EMP_Aadhar_Name;
            emp.EMP_Aadhar_Number = employees.EMP_Aadhar_Number;
            emp.EMP_DOB = employees.EMP_DOB;
            emp.EMP_Married = employees.EMP_Married;
            emp.EMP_DateOfJoining = employees.EMP_DateOfJoining;
            emp.EMP_Gender = employees.EMP_Gender;
            emp.EMP_Contact_Primary = employees.EMP_Contact_Primary;
            emp.EMP_Contact_Secondry = employees.EMP_Contact_Secondry;
            emp.EMP_Address = employees.EMP_Address;
            emp.EMP_Designation = employees.EMP_Designation;
            emp.EMP_Pan_Number = employees.EMP_Pan_Number;
            emp.EMP_ESIC_Number = employees.EMP_ESIC_Number;
            emp.EMP_UAN_Number = employees.EMP_UAN_Number;
            emp.DEPT_Id = employees.DEPT_Id;
            emp.EMP_EmployeeNumber_Office = employees.EMP_EmployeeNumber_Office;
            emp.EMP_TPC_EmployeeId = employees.EMP_TPC_EmployeeId;
            emp.EMP_RegisteredOn = employees.EMP_RegisteredOn;
            emp.ADM_Id_RegisteredBy = employees.ADM_Id_RegisteredBy;
            emp.EMP_IsActive = Convert.ToBoolean(employees.EMP_IsActive);
            emp.EMP_InactivatedOn = employees.EMP_InactivatedOn;
            emp.ADM_Id_InactivatedBy = employees.ADM_Id_InactivatedBy;
            return emp;
        }
    }
}
