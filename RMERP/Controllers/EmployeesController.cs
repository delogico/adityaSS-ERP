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
        EmployeesMapper employeesMapper = new EmployeesMapper();
        public EmployeesController(RMERPContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            EmployeeManager employeeManager = new EmployeeManager(_context);
            IEnumerable<Employees> listEmp = employeeManager.GetEmployees();
            return View(employeesMapper.MapMeList(listEmp));
        }
        [HttpGet]
        public ActionResult AddEditEmployee(int EmpID)
        {
            EmployeeManager employeeManager = new EmployeeManager(_context);
            DepartmentManager departmentManager = new DepartmentManager(_context);
            ViewBag.deptList = departmentManager.getDepartmentList();
            EmployeeViewModel employeeModel = new EmployeeViewModel();
            employeeModel.EMP_IsActive = true;
            if (EmpID > 0)
            {
                Employees emp = new Employees();
                emp = employeeManager.GetEmployeesById(EmpID);
                employeeModel = employeesMapper.MapMeModel(emp);
            }

            return View(employeeModel);
        }
        [HttpPost]
        public ActionResult AddEditEmployee(EmployeeViewModel employeeModel)
        {
            string res = string.Empty;
            EmployeeManager employeeManager = new EmployeeManager(_context);
            if (ModelState.IsValid)
            {
                SessionUtils sessionUtils = new SessionUtils(Request, Response);
                Employees employees = new Employees();
                employees.ADM_Id_RegisteredBy = sessionUtils.GetLoggedAdminID();
                employees = employeesMapper.MapMeOriginalModel(employeeModel);
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
            res = employeeManager.ActiveEmployee(EmpID, sessionUtils.GetLoggedAdminID());
            if (res != string.Empty)
            {
                TempData["message"] = "There is some problem! Please Try Again";
            }
            return RedirectToAction("Index");
        }
    }
}