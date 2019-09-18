using System;
using System.Collections.Generic;
using System.Linq;
using RMERP.DAL.ManagerClasses;
using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;

namespace RMERP.DAL.Mappers
{
    public class EmployeesMapper
    {
        public static EmployeeVM MapMe(Employees employee, RMERPContext _context=null)
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
            emp.EMP_EmployeeNumber_Office = employee.EMP_EmployeeNumber_Office;
            emp.EMP_TPC_EmployeeId = employee.EMP_TPC_EmployeeId;

            emp.EMP_Account_Name = employee.EMP_Account_Name;
            emp.EMP_Account_Number = employee.EMP_Account_Number;
            emp.EMP_Bank = employee.EMP_Bank;
            emp.EMP_Branch = employee.EMP_Branch;
            emp.EMP_Bank_IFSC = employee.EMP_Bank_IFSC;

            emp.EMP_RegisteredOn = employee.EMP_RegisteredOn;
            emp.ADM_Id_RegisteredBy = employee.ADM_Id_RegisteredBy;
            emp.EMP_IsActive = Convert.ToBoolean(employee.EMP_IsActive);
            emp.EMP_InactivatedOn = employee.EMP_InactivatedOn;
            emp.ADM_Id_InactivatedBy = employee.ADM_Id_InactivatedBy;

            if (employee.Employee_Advance.Count > 0)
                emp.advances = EmployeeAdvanceMapper.mapAdvances(employee.Employee_Advance.ToList());
            if (employee.Wage_Register_Advances.Count > 0)
                emp.wageRegisterAdvances = WageRegisterAdvancesMapper.mapMeModels(employee.Wage_Register_Advances.ToList());
            if (employee.Employee_Documents.Count > 0)
                emp.employee_Documents = employee.Employee_Documents.ToList();
            if (employee.FRM_ != null)
                emp.FRM_ = employee.FRM_;

            emp.FRM_Id = employee.FRM_Id;            
            if (_context != null)
            {                
                EmployeeManager employeeManager = new EmployeeManager(_context);
                emp.IsAssigned = employeeManager.IsAssignedEmployee(emp.EMP_Id);
            }
            emp.EMP_Payment_Type = employee.EMP_Payment_Type;
            emp.EMP_Is_IDBI_Other = employee.EMP_Is_IDBI_Other;

            emp.EMP_UAN_Remark = employee.EMP_UAN_Remark;
            emp.EMP_ESIC_Remark = employee.EMP_ESIC_Remark;
            if(employee.EMP_ReasonCode!=null)
                emp.EMP_ReasonCode = employee.EMP_ReasonCode.Value;
            if(employee.EMP_State!=null)
                emp.EMP_State = employee.EMP_State.Value;
            if (employee.EMP_City != null)
                emp.EMP_City = employee.EMP_City.Value;
            return emp;
        }

        public static List<EmployeeVM> MapEmployees(List<Employees> employees,RMERPContext _context=null)
        {
            List<EmployeeVM> lst = new List<EmployeeVM>();
            foreach (Employees employee in employees)
                lst.Add(MapMe(employee, _context));
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
            emp.EMP_EmployeeNumber_Office = employee.EMP_EmployeeNumber_Office;
            emp.EMP_TPC_EmployeeId = employee.EMP_TPC_EmployeeId;
            emp.EMP_Account_Name = employee.EMP_Account_Name;
            emp.EMP_Account_Number = employee.EMP_Account_Number;
            emp.EMP_Bank = employee.EMP_Bank;
            emp.EMP_Branch = employee.EMP_Branch;
            emp.EMP_Bank_IFSC = employee.EMP_Bank_IFSC;
            emp.EMP_RegisteredOn = employee.EMP_RegisteredOn;
            emp.ADM_Id_RegisteredBy = employee.ADM_Id_RegisteredBy;
            emp.EMP_IsActive = Convert.ToBoolean(employee.EMP_IsActive);
            emp.EMP_InactivatedOn = employee.EMP_InactivatedOn;
            emp.ADM_Id_InactivatedBy = employee.ADM_Id_InactivatedBy;
            emp.FRM_Id = employee.FRM_Id;
            if (employee.FRM_!=null)
                emp.FRM_ = employee.FRM_;
            emp.EMP_Payment_Type = employee.EMP_Payment_Type;
            emp.EMP_Is_IDBI_Other = employee.EMP_Is_IDBI_Other;
            emp.EMP_UAN_Remark = employee.EMP_UAN_Remark;
            emp.EMP_ESIC_Remark = employee.EMP_ESIC_Remark;
            emp.EMP_ReasonCode = employee.EMP_ReasonCode;
            emp.EMP_State = employee.EMP_State;
            emp.EMP_City = employee.EMP_City;
            return emp;
        }
    }
}
