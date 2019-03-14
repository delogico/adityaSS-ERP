using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;
using RMERP.DAL.ManagerClasses;
using Microsoft.AspNetCore.Mvc.Rendering;
using RMERP.Helpers;

namespace RMERP.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly RMERPContext _context;

        public EmployeesController(RMERPContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            EmployeeManager employeeManager = new EmployeeManager(_context);
            IEnumerable<Employees> listEmp= employeeManager.GetEmployees();
            return View(listEmp);
        }
        [HttpGet]
        public ActionResult AddEditEmployee(int EmpID)
        {
            EmployeeManager employeeManager = new EmployeeManager(_context);
            DepartmentManager departmentManager = new DepartmentManager(_context);
            ViewBag.deptList = departmentManager.getDepartmentList();
            EmployeeModel employeeModel = new EmployeeModel();
            employeeModel.EMP_IsActive = true;
            if (EmpID > 0)
            {
                Employees emp = new Employees();
                emp = employeeManager.GetEmployeesById(EmpID);
                employeeModel.EMP_Id = emp.EMP_Id;
                employeeModel.EMP_FirstName = emp.EMP_FirstName;
                employeeModel.EMP_MiddleName = emp.EMP_MiddleName;
                employeeModel.EMP_SurName = emp.EMP_SurName;
                employeeModel.EMP_Aadhar_Name = emp.EMP_Aadhar_Name;
                employeeModel.EMP_Aadhar_Number = emp.EMP_Aadhar_Number;
                employeeModel.EMP_DOB = emp.EMP_DOB;
                employeeModel.EMP_Married = emp.EMP_Married;
                employeeModel.EMP_DateOfJoining = emp.EMP_DateOfJoining;
                employeeModel.EMP_Gender = emp.EMP_Gender;
                employeeModel.EMP_Contact_Primary = emp.EMP_Contact_Primary;
                employeeModel.EMP_Contact_Secondry = emp.EMP_Contact_Secondry;
                employeeModel.EMP_Address = emp.EMP_Address;
                employeeModel.EMP_Designation = emp.EMP_Designation;
                employeeModel.EMP_Pan_Number = emp.EMP_Pan_Number;
                employeeModel.EMP_ESIC_Number = emp.EMP_ESIC_Number;
                employeeModel.EMP_UAN_Number = emp.EMP_UAN_Number;
                employeeModel.DEPT_Id = emp.DEPT_Id;
                employeeModel.EMP_EmployeeNumber_Office = emp.EMP_EmployeeNumber_Office;
                employeeModel.EMP_TPC_EmployeeId = emp.EMP_TPC_EmployeeId;
                employeeModel.EMP_IsActive =Convert.ToBoolean(emp.EMP_IsActive);
            }
           
            return View(employeeModel);
        }
        [HttpPost]
        public ActionResult AddEditEmployee(EmployeeModel employeeModel)
        {
            string res = string.Empty;
            EmployeeManager employeeManager = new EmployeeManager(_context);
            if (ModelState.IsValid)
            {
                Employees employees = new Employees();
                employees.EMP_Id = employeeModel.EMP_Id;
                employees.EMP_FirstName = employeeModel.EMP_FirstName;
                employees.EMP_MiddleName = employeeModel.EMP_MiddleName;
                employees.EMP_SurName = employeeModel.EMP_SurName;
                employees.EMP_Aadhar_Name = employeeModel.EMP_Aadhar_Name;
                employees.EMP_Aadhar_Number = employeeModel.EMP_Aadhar_Number;
                employees.EMP_DOB = employeeModel.EMP_DOB;
                employees.EMP_Married = employeeModel.EMP_Married;
                employees.EMP_DateOfJoining = employeeModel.EMP_DateOfJoining;
                employees.EMP_Gender = employeeModel.EMP_Gender;
                employees.EMP_Contact_Primary = employeeModel.EMP_Contact_Primary;
                employees.EMP_Contact_Secondry = employeeModel.EMP_Contact_Secondry;
                employees.EMP_Address = employeeModel.EMP_Address;
                employees.EMP_Designation = employeeModel.EMP_Designation;
                employees.EMP_Pan_Number = employeeModel.EMP_Pan_Number;
                employees.EMP_ESIC_Number = employeeModel.EMP_ESIC_Number;
                employees.EMP_UAN_Number = employeeModel.EMP_UAN_Number;
                employees.DEPT_Id = employeeModel.DEPT_Id;
                employees.EMP_EmployeeNumber_Office = employeeModel.EMP_EmployeeNumber_Office;
                employees.EMP_TPC_EmployeeId = employeeModel.EMP_TPC_EmployeeId;
                SessionUtils sessionUtils = new SessionUtils(Request, Response);
                employees.ADM_Id_RegisteredBy = sessionUtils.GetLoggedAdminID();
                employees.EMP_IsActive = employeeModel.EMP_IsActive;

                res = employeeManager.AddEditEmployee(employees);
            }            
            if (res == string.Empty)
            {
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Client data can not Inserted";
                return RedirectToAction("AddEditEmployee");
            }
           
        }

        public ActionResult ActiveEmployee(int EmpID)
        {
            string res = string.Empty;
            EmployeeManager employeeManager = new EmployeeManager(_context);
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            res = employeeManager.ActiveEmployee(EmpID,sessionUtils.GetLoggedAdminID());
            if (res != string.Empty)
            {
                TempData["message"] = "There is some problem! Please Try Again";
            }            
            return RedirectToAction("Index");
        }
    }
}