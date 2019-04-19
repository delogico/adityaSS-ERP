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
using static RMERP.DAL.Helpers.ProjectUtils;
using SmartBreadcrumbs.Attributes;

namespace RMERP.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly RMERPContext _context;        
       // private static int Emp_Id=0;
        public EmployeesController(RMERPContext context)
        {
            _context = context;
        }
        [Breadcrumb("Employees")]
        public IActionResult Index()
        {
            EmpId = 0;
            EmployeeManager employeeManager = new EmployeeManager(_context);
            return View(EmployeesMapper.MapEmployees(employeeManager.GetEmployees().ToList()));
        }
        [HttpGet]
        [Breadcrumb("Employee Info", FromAction = "Index")]
        public ActionResult AddEditEmployee(int EMP_Id=0)
        {
            EMP_Id = (EMP_Id <= 0 ? EmpId : EMP_Id);
            EmployeeManager employeeManager = new EmployeeManager(_context);
            DepartmentManager departmentManager = new DepartmentManager(_context);
            ViewBag.deptList = departmentManager.getDepartmentList();
            EmployeeVM employeeVM = new EmployeeVM();
            if (EMP_Id > 0)
            {
                Employees emp = employeeManager.GetEmployeeById(EMP_Id);
                employeeVM = EmployeesMapper.MapMe(emp);
            }
            else
            {
                employeeVM.EMP_IsActive = true;
            }
            
            return View(employeeVM);
        }
        [HttpPost]
        public ActionResult AddEditEmployees(EmployeeVM employeeVM)
        {
            string res = string.Empty;
            EmployeeManager employeeManager = new EmployeeManager(_context);
            if (ModelState.IsValid)
            {
                SessionUtils sessionUtils = new SessionUtils(Request, Response);
                Employees employee = new Employees();
                employee = EmployeesMapper.MapMeModel(employeeVM);
                employee.ADM_Id_RegisteredBy = sessionUtils.GetLoggedAdminID();
                res = employeeManager.AddEditEmployee(employee);
            }            
            if (res == string.Empty)
            {
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Employee data can not Inserted";
                return RedirectToAction("AddEditEmployee", new { EMP_Id = employeeVM.EMP_Id});
            }
           
        }

        public ActionResult ActiveEmployee(int EMP_Id)
        {
            string res = string.Empty;
            EmployeeManager employeeManager = new EmployeeManager(_context);
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            res = employeeManager.ActiveEmployee(EMP_Id, sessionUtils.GetLoggedAdminID());
            if (res != string.Empty)
            {
                TempData["message"] = "There is some problem! Please Try Again";
            }            
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Breadcrumb("Advance", FromAction = "AddEditEmployee")]
        public ActionResult AddEditAdvance(int EMP_Id, int ADV_Id=-1)
        {
            EmpId = (EMP_Id <= 0 ? EmpId : EMP_Id);           
            EmployeeAdvanceVM employeeAdvanceVM = new EmployeeAdvanceVM();
            EmployeeManager employeeManager = new EmployeeManager(_context);
            Employee_Advance employee_Advance = new Employee_Advance();
            if (ADV_Id > 0)
            {                
                employee_Advance = employeeManager.GetEmployeeAdvanceById(ADV_Id);
                employeeAdvanceVM = EmployeeAdvanceMapper.mapMe(employee_Advance);
            }
            if (EMP_Id > 0)
            {
                employeeAdvanceVM.EMP_Id = EMP_Id;
                employeeAdvanceVM.EmployeeName = EmployeesMapper.MapMe(employeeManager.GetEmployeeById(EMP_Id)).EMP_FullName;
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
                employee_Advance = EmployeeAdvanceMapper.mapMeModel(employeeAdvanceVM);
                employee_Advance.ADM_Id_RegisteredBy = sessionUtils.GetLoggedAdminID();
                res = employeeManager.AddEditAdvance(employee_Advance);
            }
            if (res != string.Empty)
            {
                TempData["message"] = "Advance data can not Inserted";
            }
            
            return RedirectToAction("AddEditEmployee",new{ EMP_Id = employeeAdvanceVM.EMP_Id,tab = "AddEditAdvance" });
        }
        public ActionResult DeleteAdvance(int ADV_Id, int EMP_Id)
        {
            string res = string.Empty;            
            EmployeeManager employeeManager = new EmployeeManager(_context);
            res = employeeManager.DeleteAdvance(ADV_Id);
            if (res != string.Empty)
            {
                TempData["message"] = "Advance data can not Deleted";
            }
            return RedirectToAction("AddEditEmployee", new { EMP_Id = EMP_Id, tab = "AddEditAdvance" });
        }
        [Breadcrumb("Advance Report For Bank", FromAction = "Index", FromController = typeof(WageProcessController))]
        public ActionResult AdvanceRptForBank(DateTime WAG_Month)
        {
            EmployeeManager employeeManager = new EmployeeManager(_context);
            List<EmployeeAdvanceVM> advancesVM = EmployeeAdvanceMapper.mapAdvances(employeeManager.AdvanceRptForBank((WAG_Month)));
            ViewBag.WAG_Month = WAG_Month.ToString("MMMM") + "-" + WAG_Month.ToString("yyyy");
            return View(advancesVM);
        }
        [Breadcrumb("Pending Advance List", FromAction = "Index", FromController = typeof(WageProcessController))]
        public ActionResult NotCompletedAdvanceLst(DateTime WAG_Month)
        {
            EmployeeManager employeeManager = new EmployeeManager(_context);
            List<EmployeeAdvanceVM> advancesVM = EmployeeAdvanceMapper.mapAdvances(employeeManager.NotCompletedAdvanceLst((WAG_Month)));
            ViewBag.WAG_Month = WAG_Month.ToString("MMMM") + "-" + WAG_Month.ToString("yyyy");
            return View(advancesVM);
        }
    }
}