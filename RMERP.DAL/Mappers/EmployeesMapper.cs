using System;
using System.Collections.Generic;
using System.Linq;
using RMERP.DAL.Helpers;
using RMERP.DAL.ManagerClasses;
using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;
using static RMERP.DAL.Helpers.ProjectUtils;

namespace RMERP.DAL.Mappers
{
    public class EmployeesMapper
    {
        public static EmployeeVM MapMe(Employee employee, RMERPContext _context = null)
        {

            EmployeeVM emp = new EmployeeVM
            {
                EMP_Id = employee.EMP_Id,
                EMP_FirstName = employee.EMP_FirstName.Trim(),
                EMP_MiddleName = (employee.EMP_MiddleName != null ? employee.EMP_MiddleName.Trim() : employee.EMP_MiddleName),
                EMP_SurName = (employee.EMP_SurName != null ? employee.EMP_SurName.Trim() : employee.EMP_SurName),
                EMP_Aadhar_Name = employee.EMP_Aadhar_Name,
                EMP_Aadhar_Number = employee.EMP_Aadhar_Number,
                EMP_DOB = ProjectUtils.DateToDateTime(employee.EMP_DOB),
                EMP_Married = employee.EMP_Married.ToString(),
                EMP_DateOfJoining = ProjectUtils.DateToDateTime(employee.EMP_DateOfJoining),
                EMP_Gender = employee.EMP_Gender.ToString(),
                EMP_Contact_Primary = employee.EMP_Contact_Primary,
                EMP_Contact_Secondry = employee.EMP_Contact_Secondry,
                EMP_Temporary_Address = employee.EMP_Temporary_Address,
                EMP_Permanent_Address = employee.EMP_Permanent_Address,
                EMP_Designation = employee.EMP_Designation,
                EMP_Pan_Number = employee.EMP_Pan_Number,
                EMP_ESIC_Number = employee.EMP_ESIC_Number,
                EMP_UAN_Number = employee.EMP_UAN_Number,
                EMP_EmployeeNumber_Office = employee.EMP_EmployeeNumber_Office,
                EMP_TPC_EmployeeId = employee.EMP_TPC_EmployeeId,

                EMP_Account_Name = employee.EMP_Account_Name,
                EMP_Account_Number = employee.EMP_Account_Number,
				EMP_Bank = employee.EMP_Bank == null ? null : ((int)Enum.GetValues(typeof(REGISTER_BANK)).Cast<REGISTER_BANK>().FirstOrDefault(x => ProjectUtils.GetStringValue(x) == employee.EMP_Bank)).ToString(),
				EMP_Branch = employee.EMP_Branch,
                EMP_Bank_IFSC = employee.EMP_Bank_IFSC,

                EMP_RegisteredOn = employee.EMP_RegisteredOn,
                ADM_Id_RegisteredBy = employee.ADM_Id_RegisteredBy,
                EMP_IsActive = Convert.ToBoolean(employee.EMP_IsActive),
                EMP_InactivatedOn = employee.EMP_InactivatedOn,
                ADM_Id_InactivatedBy = employee.ADM_Id_InactivatedBy,
                EMP_RejoinOn = employee.EMP_RejoinOn
            };

            if (employee.Employee_Advances.Count > 0)
                emp.advances = EmployeeAdvanceMapper.mapAdvances(employee.Employee_Advances.ToList());
            if (employee.Wage_Register_Advances.Count > 0)
                emp.wageRegisterAdvances = WageRegisterAdvancesMapper.mapMeModels(employee.Wage_Register_Advances.ToList());
            if (employee.Employee_Documents.Count > 0)
                emp.employee_Documents = employee.Employee_Documents.ToList();
            if (employee.FRM != null)
                emp.FRM_ = employee.FRM;

            emp.FRM_Id = employee.FRM_Id;
            if (_context != null)
            {
                EmployeeManager employeeManager = new EmployeeManager(_context);
                emp.IsAssigned = employeeManager.IsAssignedEmployee(emp.EMP_Id);
            }
            emp.EMP_Payment_Type = employee.EMP_Payment_Type;
			emp.EMP_Is_IDBI_Other = employee.EMP_Is_IDBI_Other;
            emp.CBA_Id = employee.CBA_Id;

            emp.EMP_UAN_Remark = employee.EMP_UAN_Remark;
            emp.EMP_ESIC_Remark = employee.EMP_ESIC_Remark;
            emp.EMP_LIN_Number = employee.EMP_LIN_Number;
            emp.EMP_LIN_Remark = employee.EMP_LIN_Remark;
            if (employee.EMP_ReasonCode != null)
                emp.EMP_ReasonCode = employee.EMP_ReasonCode.Value;
            if (employee.EMP_State != null)
                emp.EMP_State = employee.EMP_State.Value;
            if (employee.EMP_City != null)
                emp.EMP_City = employee.EMP_City.Value;


            return emp;
        }

        public static List<EmployeeVM> MapEmployees(List<Employee> employees, RMERPContext _context = null)
        {
            List<EmployeeVM> lst = new List<EmployeeVM>();
            foreach (Employee employee in employees)
                lst.Add(MapMe(employee, _context));
            return lst;
        }

        public static Employee MapMeModel(EmployeeVM employee)
        {
            Employee emp = new Employee
            {
                EMP_Id = employee.EMP_Id,
                EMP_FirstName = employee.EMP_FirstName.Trim(),
                EMP_MiddleName = (employee.EMP_MiddleName != null ? employee.EMP_MiddleName.Trim() : employee.EMP_MiddleName),
                EMP_SurName = (employee.EMP_SurName != null ? employee.EMP_SurName.Trim() : employee.EMP_SurName),
                EMP_Aadhar_Name = employee.EMP_Aadhar_Name,
                EMP_Aadhar_Number = employee.EMP_Aadhar_Number,
                EMP_DOB = DateOnly.FromDateTime(employee.EMP_DOB),
                EMP_DateOfJoining = DateOnly.FromDateTime(employee.EMP_DateOfJoining),
                EMP_Gender = Convert.ToBoolean(employee.EMP_Gender),
                EMP_Contact_Primary = employee.EMP_Contact_Primary,
                EMP_Contact_Secondry = employee.EMP_Contact_Secondry,
                EMP_Temporary_Address = employee.EMP_Temporary_Address,
                EMP_Permanent_Address = employee.EMP_Permanent_Address,
                EMP_Designation = employee.EMP_Designation,
                EMP_Pan_Number = employee.EMP_Pan_Number,
                EMP_ESIC_Number = employee.EMP_ESIC_Number,
                EMP_UAN_Number = employee.EMP_UAN_Number,
                EMP_EmployeeNumber_Office = employee.EMP_EmployeeNumber_Office,
                EMP_TPC_EmployeeId = employee.EMP_TPC_EmployeeId,
                EMP_Account_Name = employee.EMP_Account_Name,
                EMP_Account_Number = employee.EMP_Account_Number,
                EMP_Bank = employee.EMP_Bank,
                EMP_Branch = employee.EMP_Branch,
                EMP_Bank_IFSC = employee.EMP_Bank_IFSC,
                EMP_RegisteredOn = employee.EMP_RegisteredOn,
                ADM_Id_RegisteredBy = employee.ADM_Id_RegisteredBy,
                EMP_IsActive = Convert.ToBoolean(employee.EMP_IsActive),
                EMP_InactivatedOn = employee.EMP_InactivatedOn,
                ADM_Id_InactivatedBy = employee.ADM_Id_InactivatedBy,
                FRM_Id = employee.FRM_Id,
                EMP_Payment_Type = employee.EMP_Payment_Type,
                EMP_Is_IDBI_Other = employee.EMP_Is_IDBI_Other,
                EMP_UAN_Remark = employee.EMP_UAN_Remark,
                EMP_ESIC_Remark = employee.EMP_ESIC_Remark,
                EMP_ReasonCode = employee.EMP_ReasonCode,
                EMP_State = employee.EMP_State,
                EMP_City = employee.EMP_City,
                EMP_RejoinOn = employee.EMP_RejoinOn,
                EMP_LIN_Number = employee.EMP_LIN_Number,
                EMP_LIN_Remark = employee.EMP_LIN_Remark
            };

            if (!string.IsNullOrEmpty(employee.EMP_Married)) emp.EMP_Married = Convert.ToByte(employee.EMP_Married);
            if (employee.FRM_ != null) emp.FRM = employee.FRM_; return emp;
        }

        public static Employee MapMeObject(Employee employee, int ADM_Id)
        {
            Employee emp = new Employee
            {
                EMP_FirstName = employee.EMP_FirstName.Trim(),
                EMP_MiddleName = employee.EMP_MiddleName,
                EMP_SurName = employee.EMP_SurName,
                EMP_Aadhar_Name = employee.EMP_Aadhar_Name,
                EMP_Aadhar_Number = employee.EMP_Aadhar_Number,
                EMP_DOB = employee.EMP_DOB,
                EMP_Married = employee.EMP_Married,
                EMP_Gender = employee.EMP_Gender,
                EMP_Contact_Primary = employee.EMP_Contact_Primary,
                EMP_Contact_Secondry = employee.EMP_Contact_Secondry,
                EMP_Temporary_Address = employee.EMP_Temporary_Address,
                EMP_Permanent_Address = employee.EMP_Permanent_Address,
                EMP_Designation = employee.EMP_Designation,
                EMP_Pan_Number = employee.EMP_Pan_Number,
                EMP_ESIC_Number = employee.EMP_ESIC_Number,
                EMP_UAN_Number = employee.EMP_UAN_Number,
                EMP_EmployeeNumber_Office = employee.EMP_EmployeeNumber_Office,
                EMP_TPC_EmployeeId = employee.EMP_TPC_EmployeeId,
                EMP_Account_Name = employee.EMP_Account_Name,
                EMP_Account_Number = employee.EMP_Account_Number,
                EMP_Bank = employee.EMP_Bank,
                EMP_Branch = employee.EMP_Branch,
                EMP_Bank_IFSC = employee.EMP_Bank_IFSC,

                FRM_Id = employee.FRM_Id,
                EMP_Payment_Type = employee.EMP_Payment_Type,
                EMP_Is_IDBI_Other = employee.EMP_Is_IDBI_Other,
                EMP_UAN_Remark = employee.EMP_UAN_Remark,
                EMP_ESIC_Remark = employee.EMP_ESIC_Remark,
                EMP_State = employee.EMP_State,
                EMP_City = employee.EMP_City,

                EMP_DateOfJoining = employee.EMP_DateOfJoining,
                EMP_RegisteredOn = DateTime.Now,
                ADM_Id_RegisteredBy = ADM_Id,
                EMP_RejoinOn = null,
                EMP_IsActive = true,
                EMP_InactivatedOn = null,
                ADM_Id_InactivatedBy = null,
                EMP_ReasonCode = null
            };
            return emp;
        }
    }
}
