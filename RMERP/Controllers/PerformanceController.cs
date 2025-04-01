using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;
using RMERP.DAL.Mappers;
using RMERP.DAL.ManagerClasses;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using SmartBreadcrumbs.Attributes;
using static RMERP.DAL.Helpers.ProjectUtils;
using Microsoft.AspNetCore.Authorization;

namespace RMERP.Controllers
{
    [Authorize]
    public class PerformanceController : Controller
    {
        private readonly RMERPContext _context;
        public IConfiguration Configuration;
        public PerformanceController(RMERPContext context, IConfiguration configuration)
        {
            _context = context;
            Configuration = configuration;
        }

        #region NEW CODE

        public IActionResult NewUpdatePerformanceAmount(int CRI_Id, int WAG_Id, int FRM_Id)
        {
            WageProcessManager wageProcessManager = new WageProcessManager(_context);
            DateOnly WAG_Month = wageProcessManager.getWageProcessById(WAG_Id).WAG_Month;
            ViewBag.Wag_Month = WAG_Month.ToString("MMMM") + "-" + WAG_Month.ToString("yyyy");
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            PerformanceManager performanceManager = new PerformanceManager(_context);
            Client_Requirement client_Requirements = clientsManager.GetRequirementsById(CRI_Id);
            List<Wage_Register_PerformanceVM> wage_Register_Performances = new List<Wage_Register_PerformanceVM>();
            IEnumerable<Clients_Employee> clientsEmployees = clientsManager.listActiveClientsEmployees(client_Requirements.CLI_Id, DAL.Helpers.ProjectUtils.DateToDateTime(WAG_Month), client_Requirements.DES_Id);
            foreach (Clients_Employee employee in clientsEmployees)
            {
                Wage_Register_Performance performance = new Wage_Register_Performance();
                Wage_Register_PerformanceVM performanceVM = new Wage_Register_PerformanceVM();
                performance = performanceManager.GetPerformanceByCLE(WAG_Id, employee.CLE_Id, employee.CLI_Id);
                if (performance != null)
                {
                    performance = performanceManager.GetPerformanceById(performance.WRP_Id);
                    performanceVM = wageRegisterPerformanceMapper.mapMe(performance, CRI_Id);
                }
                else
                {
                    performanceVM.CLE_Id = employee.CLE_Id;
                    performanceVM.WAG_Id = WAG_Id;
                    performanceVM.Emp_ID = employee.EMP_Id;
                    performanceVM.CRI_Id = CRI_Id;
                    performanceVM.Emp_Name = employee.EMP.EMP_FirstName + " " + employee.EMP.EMP_MiddleName + " " + employee.EMP.EMP_SurName;
                }
                performanceVM.FRM_ID = FRM_Id;
                wage_Register_Performances.Add(performanceVM);
            }
            updateWageRegisterPerformance ret = new()
            {
                PerformanceVMs = wage_Register_Performances,
                CLI_Id = client_Requirements.CLI_Id
            };
            return View(ret);
        }

        [HttpPost]
        public ActionResult NewUpdateAmount(updateWageRegisterPerformance updateWagePerformance)
        {
            PerformanceManager PerformanceManager = new PerformanceManager(_context);
            List<Wage_Register_Performance> Performances = new List<Wage_Register_Performance>();
            List<Wage_Register_PerformanceVM> list = updateWagePerformance.PerformanceVMs;
            Performances = wageRegisterPerformanceMapper.mapMeModels(list);
            string res = PerformanceManager.UpdateAmount(Performances);
            if (res != string.Empty)
            {
                TempData["message"] = "Try Again";
            }
            return RedirectToAction("WageRegister", "WageRegister", new { WAG_Id = list[0].WAG_Id, FRM_Id = list[0].FRM_ID });
        }

        #endregion

        public IActionResult UpdatePerformanceAmount(int CRI_Id, int WAG_Id, int FRM_Id)
        {
            WageProcessManager wageProcessManager = new WageProcessManager(_context);
            DateOnly WAG_Month = wageProcessManager.getWageProcessById(WAG_Id).WAG_Month;
            ViewBag.Wag_Month = WAG_Month.ToString("MMMM") + "-" + WAG_Month.ToString("yyyy");
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            PerformanceManager performanceManager = new PerformanceManager(_context);
            Client_Requirement client_Requirements = clientsManager.GetRequirementsById(CRI_Id);
            List<Wage_Register_PerformanceVM> wage_Register_Performances = new List<Wage_Register_PerformanceVM>();
            IEnumerable<Clients_Employee> clientsEmployees = clientsManager.listActiveClientsEmployees(client_Requirements.CLI_Id, DAL.Helpers.ProjectUtils.DateToDateTime(WAG_Month), client_Requirements.DES_Id);
            foreach (Clients_Employee employee in clientsEmployees)
            {
                Wage_Register_Performance performance = new Wage_Register_Performance();
                Wage_Register_PerformanceVM performanceVM = new Wage_Register_PerformanceVM();
                performance = performanceManager.GetPerformanceByCLE(WAG_Id, employee.CLE_Id, employee.CLI_Id);
                if (performance != null)
                {
                    performance = performanceManager.GetPerformanceById(performance.WRP_Id);
                    performanceVM = wageRegisterPerformanceMapper.mapMe(performance, CRI_Id);
                }
                else
                {
                    performanceVM.CLE_Id = employee.CLE_Id;
                    performanceVM.WAG_Id = WAG_Id;
                    performanceVM.Emp_ID = employee.EMP_Id;
                    performanceVM.CRI_Id = CRI_Id;
                    performanceVM.Emp_Name = employee.EMP.EMP_FirstName + " " + employee.EMP.EMP_MiddleName + " " + employee.EMP.EMP_SurName;
                }
                performanceVM.FRM_ID = FRM_Id;
                wage_Register_Performances.Add(performanceVM);
            }
            updateWageRegisterPerformance updateWagePerformance = new updateWageRegisterPerformance();
            updateWagePerformance.PerformanceVMs = wage_Register_Performances;
            return View(updateWagePerformance);
        }

        [HttpPost]
        public ActionResult UpdateAmount(updateWageRegisterPerformance updateWagePerformance)
        {
            PerformanceManager PerformanceManager = new PerformanceManager(_context);
            List<Wage_Register_Performance> Performances = new List<Wage_Register_Performance>();
            List<Wage_Register_PerformanceVM> list = updateWagePerformance.PerformanceVMs;
            Performances = wageRegisterPerformanceMapper.mapMeModels(list);
            string res = PerformanceManager.UpdateAmount(Performances);
            if (res != string.Empty)
            {
                TempData["message"] = "Try Again";
            }
            return RedirectToAction("WageRegister", "WageRegister", new { WAG_Id = list[0].WAG_Id, FRM_Id = list[0].FRM_ID });
        }
    }
}