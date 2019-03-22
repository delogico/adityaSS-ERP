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
using RMERP.DAL.Mappers;

namespace RMERP.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly RMERPContext _context;
        EmployeesMapper employeesMapper=new EmployeesMapper();
        private static int Emp_Id=0;
        public EmployeesController(RMERPContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            EmployeeManager employeeManager = new EmployeeManager(_context);
            IEnumerable<Employees> listEmp= employeeManager.GetEmployees();
            return View(employeesMapper.MapMeList(listEmp));           
        }
        [HttpGet]
        public ActionResult AddEditEmployee(int EmpID=-1)
        {
            EmployeeManager employeeManager = new EmployeeManager(_context);
            DepartmentManager departmentManager = new DepartmentManager(_context);
            ViewBag.deptList = departmentManager.getDepartmentList();
            EmployeeAddEditVM employeeAddEditVM = new EmployeeAddEditVM();
            employeeAddEditVM.employeeViewModel = new EmployeeViewModel();
            employeeAddEditVM.employeeViewModel.EMP_IsActive = true;
            if (EmpID > 0)
            {
                Employees emp = new Employees();
                emp = employeeManager.GetEmployeesById(EmpID);
                Emp_Id = EmpID;
                employeeAddEditVM.employeeViewModel = employeesMapper.MapMeModel(emp);                
            }
            employeeAddEditVM.ListEmployee_Advance = employeeManager.GetEmployee_Advances(EmpID);
            return View(employeeAddEditVM);
        }
        [HttpPost]
        public ActionResult AddEditEmployee(EmployeeAddEditVM employeeAddEditVM)
        {
            string res = string.Empty;
            EmployeeManager employeeManager = new EmployeeManager(_context);
           
            if (ModelState.IsValid)
            {
                SessionUtils sessionUtils = new SessionUtils(Request, Response);
                Employees employees = new Employees();
                Emp_Id = employeeAddEditVM.employeeViewModel.EMP_Id;
                employees.ADM_Id_RegisteredBy = sessionUtils.GetLoggedAdminID();
                employees= employeesMapper.MapMeOriginalModel(employeeAddEditVM.employeeViewModel);
                res = employeeManager.AddEditEmployee(employees);
            }            
            if (res == string.Empty)
            {
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Employee data can not Inserted";
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

        [HttpGet]
        public ActionResult AddEditAdvance(int ADV_Id=-1)
        {
            EmployeeAdvanceVM employeeAdvanceVM = new EmployeeAdvanceVM();
            EmployeeManager employeeManager = new EmployeeManager(_context);
            Employee_Advance employee_Advance = new Employee_Advance();
            employeeAdvanceVM.EMP_Id = Emp_Id;
            
            if (ADV_Id > 0)
            {                
                employee_Advance = employeeManager.GetEmployeeAdvanceById(ADV_Id);
                employeeAdvanceVM = EmployeeAdvanceMapper.mapMeInVM(employee_Advance);
            }
            if (Emp_Id > 0)
            {
                Employees emp = employeeManager.GetEmployeesById(Emp_Id);
                employeeAdvanceVM.EmployeeName = emp.EMP_FirstName + " " + emp.EMP_MiddleName + " " + emp.EMP_SurName;
            }
            return View(employeeAdvanceVM);
        }
        [HttpPost]
        public ActionResult AddEditAdvance(EmployeeAdvanceVM employeeAdvanceVM)
        {
            string res = string.Empty;
            SessionUtils sessionUtils = new SessionUtils(Request, Response);           
            EmployeeManager employeeManager = new EmployeeManager(_context);
            if (ModelState.IsValid)
            {
                Employee_Advance employee_Advance = new Employee_Advance();               
                employee_Advance = EmployeeAdvanceMapper.mapMeInOriginal(employeeAdvanceVM);
                employee_Advance.ADM_Id_RegisteredBy = sessionUtils.GetLoggedAdminID();
                res = employeeManager.AddEditAdvance(employee_Advance);
            }
            if (res != string.Empty)
            {
                TempData["message"] = "Advance data can not Inserted";
            }
            
            return RedirectToAction("AddEditEmployee",new{ EmpID = Emp_Id,tab = "AddEditAdvance" });
        }
        public ActionResult DeleteAdvance(int ADV_Id)
        {
            string res = string.Empty;            
            EmployeeManager employeeManager = new EmployeeManager(_context);
            res = employeeManager.DeleteAdvance(ADV_Id);
            if (res != string.Empty)
            {
                TempData["message"] = "Advance data can not Deleted";
            }
            return RedirectToAction("AddEditEmployee", new { EmpID = Emp_Id, tab = "AddEditAdvance" });
        }
    }
}