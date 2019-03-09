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
            employeeModel.EmpIsActive = true;
            if (EmpID > 0)
            {
                Employees emp = new Employees();
                emp = employeeManager.GetEmployeesById(EmpID);
                employeeModel.EmpId = emp.EmpId;
                employeeModel.EmpFirstName = emp.EmpFirstName;
                employeeModel.EmpMiddleName = emp.EmpMiddleName;
                employeeModel.EmpSurName = emp.EmpSurName;
                employeeModel.EmpAadharName = emp.EmpAadharName;
                employeeModel.EmpAadharNumber = emp.EmpAadharNumber;
                employeeModel.EmpDob = emp.EmpDob;
                employeeModel.EmpMarried = emp.EmpMarried;
                employeeModel.EmpDateOfJoining = emp.EmpDateOfJoining;
                employeeModel.EmpGender = emp.EmpGender;
                employeeModel.EmpContactPrimary = emp.EmpContactPrimary;
                employeeModel.EmpContactSecondry = emp.EmpContactSecondry;
                employeeModel.EmpAddress = emp.EmpAddress;
                employeeModel.EmpDesignation = emp.EmpDesignation;
                employeeModel.EmpPanNumber = emp.EmpPanNumber;
                employeeModel.EmpEsicNumber = emp.EmpEsicNumber;
                employeeModel.EmpUanNumber = emp.EmpUanNumber;
                employeeModel.DeptId = emp.DeptId;
                employeeModel.EmpEmployeeNumberOffice = emp.EmpEmployeeNumberOffice;
                employeeModel.EmpTpcEmployeeId = emp.EmpTpcEmployeeId;
                employeeModel.EmpIsActive =Convert.ToBoolean(emp.EmpIsActive);
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
                employees.EmpId = employeeModel.EmpId;
                employees.EmpFirstName = employeeModel.EmpFirstName;
                employees.EmpMiddleName = employeeModel.EmpMiddleName;
                employees.EmpSurName = employeeModel.EmpSurName;
                employees.EmpAadharName = employeeModel.EmpAadharName;
                employees.EmpAadharNumber = employeeModel.EmpAadharNumber;
                employees.EmpDob = employeeModel.EmpDob;
                employees.EmpMarried = employeeModel.EmpMarried;
                employees.EmpDateOfJoining = employeeModel.EmpDateOfJoining;
                employees.EmpGender = employeeModel.EmpGender;
                employees.EmpContactPrimary = employeeModel.EmpContactPrimary;
                employees.EmpContactSecondry = employeeModel.EmpContactSecondry;
                employees.EmpAddress = employeeModel.EmpAddress;
                employees.EmpDesignation = employeeModel.EmpDesignation;
                employees.EmpPanNumber = employeeModel.EmpPanNumber;
                employees.EmpEsicNumber = employeeModel.EmpEsicNumber;
                employees.EmpUanNumber = employeeModel.EmpUanNumber;
                employees.DeptId = employeeModel.DeptId;
                employees.EmpEmployeeNumberOffice = employeeModel.EmpEmployeeNumberOffice;
                employees.EmpTpcEmployeeId = employeeModel.EmpTpcEmployeeId;
                SessionUtils sessionUtils = new SessionUtils(Request, Response);
                employees.AdmIdRegisteredBy = sessionUtils.GetLoggedAdminID();
                employees.EmpIsActive = employeeModel.EmpIsActive;

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