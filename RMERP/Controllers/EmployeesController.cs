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
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;

namespace RMERP.Controllers
{
    [Authorize]
    public class EmployeesController : Controller
    {
        private readonly RMERPContext _context;
        public IConfiguration _configuration;
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
        [Breadcrumb("Advance Taken", FromAction = "Index", FromController = typeof(WageProcessController))]
        public ActionResult AdvanceRptForBank(DateTime WAG_Month)
        {
            AdvanceWageRegisterManager advance = new AdvanceWageRegisterManager(_context);
            List<EmployeeAdvanceVM> advancesVM = EmployeeAdvanceMapper.mapAdvances(advance.AdvanceRptForBank((WAG_Month)));
            ViewBag.WAG_Month = WAG_Month.ToString("MMMM") + "-" + WAG_Month.ToString("yyyy");
            return View(advancesVM);
        }
        //[Breadcrumb("Advance EMI", FromAction = "Index", FromController = typeof(WageProcessController))]
        //public ActionResult NotCompletedAdvanceLst(DateTime WAG_Month,int WAG_Id)
        //{
        //    AdvanceWageRegisterManager advance = new AdvanceWageRegisterManager(_context);
        //    List<EmployeeAdvanceVM> advancesVM = EmployeeAdvanceMapper.mapAdvances(advance.NotCompletedAdvanceLst((WAG_Month)));
        //    ViewBag.WAG_Month = WAG_Month.ToString("MMMM") + "-" + WAG_Month.ToString("yyyy");
        //    return View(advancesVM);
        //}

        [Breadcrumb("Advance EMI", FromAction = "Index", FromController = typeof(WageProcessController))]
        public ActionResult UpdateAdvanceEMI(DateTime WAG_Month, int WAG_Id)
        {
            AdvanceWageRegisterManager advance = new AdvanceWageRegisterManager(_context);
            WageRegisterManager wageRegisterManager = new WageRegisterManager(_context);
            UpdateAdvanceEMI updateAdvanceEMI = new UpdateAdvanceEMI();            
            List<EmployeeAdvanceVM> advancesVM = EmployeeAdvanceMapper.mapAdvances(advance.NotCompletedAdvanceLst((WAG_Month)));
            List<WageRegisterAdvancesVM> wageRegisterAdvancesVMs = WageRegisterAdvancesMapper.mapMeModels(wageRegisterManager.GetWageRegisterAdvances(WAG_Month));
            updateAdvanceEMI.employeeAdvanceVMs = advancesVM;
            updateAdvanceEMI.wageRegisterAdvancesVMs = wageRegisterAdvancesVMs;
            updateAdvanceEMI.WAG_Month = WAG_Month;
            updateAdvanceEMI.WAG_Id = WAG_Id;
            ViewBag.WAG_Month = WAG_Month.ToString("MMMM") + "-" + WAG_Month.ToString("yyyy");
            return View(updateAdvanceEMI);
        }
        [HttpPost]
        public ActionResult addWageRegisterAdvances(int EMP_id,int WAG_Id, decimal WAD_Amount,bool WAD_Status)
        {
            WageProcessManager wageProcess = new WageProcessManager(_context);
            AdvanceWageRegisterManager advance = new AdvanceWageRegisterManager(_context);
            ClientsManager clients = new ClientsManager(_context, _configuration);
            DateTime WAG_Month= wageProcess.getWageProcessById(WAG_Id).WAG_Month;
            int CLI_id = clients.GetClientIDByEmpID(EMP_id, WAG_Month);
            string res=advance.addWageRegisterAdvances(EMP_id, WAG_Id, CLI_id, WAD_Amount, WAG_Month, WAD_Status);
            return RedirectToAction("UpdateAdvanceEMI",new { WAG_Month= WAG_Month, WAG_Id= WAG_Id });
        }
        [HttpPost]
        public ActionResult UpdateWageRegisterAdvances(int EMP_id,decimal WAD_Amount, int WAG_Id, bool WAD_Status, int WAD_Id = -1)
        {
            WageProcessManager wageProcess = new WageProcessManager(_context);
            ClientsManager clients = new ClientsManager(_context, _configuration);
            AdvanceWageRegisterManager advance = new AdvanceWageRegisterManager(_context);
            DateTime WAG_Month = wageProcess.getWageProcessById(WAG_Id).WAG_Month;
            string res = advance.addWageRegisterAdvances(EMP_id, WAD_Amount, WAD_Status, WAG_Month, WAD_Id);
            return RedirectToAction("UpdateAdvanceEMI", new { WAG_Month = WAG_Month, WAG_Id = WAG_Id });
        }
            
    }
}