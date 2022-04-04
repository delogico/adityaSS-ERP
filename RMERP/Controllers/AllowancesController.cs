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
using Microsoft.AspNetCore.Authorization;

namespace RMERP.Controllers
{
    [Authorize]
    public class AllowancesController : Controller
    {
        private readonly RMERPContext _context;
        public IConfiguration Configuration;
        public AllowancesController(RMERPContext context, IConfiguration configuration)
        {
            _context = context;
            Configuration = configuration;
        }
        
        public IActionResult UpdateAllowanceAmount_1(int CRI_Id, int WAG_Id, int FRM_Id)
        {
            WageProcessManager wageProcessManager = new WageProcessManager(_context);
            DateTime WAG_Month = wageProcessManager.getWageProcessById(WAG_Id).WAG_Month;
            ViewBag.Wag_Month = WAG_Month.ToString("MMMM") + "-" + WAG_Month.ToString("yyyy");
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            AllowanceManager allowanceManager = new AllowanceManager(_context);
            Client_Requirements client_Requirements = clientsManager.GetRequirementsById(CRI_Id);
            List<Wage_Register_AllowancesVM_1> wage_Register_allowances = new List<Wage_Register_AllowancesVM_1>();
            IEnumerable<Clients_Employees> clientsEmployees = clientsManager.listActiveClientsEmployees(client_Requirements.CLI_Id, WAG_Month, client_Requirements.DES_Id);
            foreach (Clients_Employees employee in clientsEmployees)
            {
                Wage_Register_Allowances_1 allowance = new Wage_Register_Allowances_1();
                Wage_Register_AllowancesVM_1 allowanceVM = new Wage_Register_AllowancesVM_1();
                allowance = allowanceManager.GetAllowances_ByCLE_1(WAG_Id, employee.CLE_Id, employee.CLI_Id);
                if (allowance != null)
                {
                    allowance = allowanceManager.GetAllowancesById_1(allowance.WRA_Id_1);
                    allowanceVM = Wage_Register_AllowancesVM_1.mapMe(allowance, CRI_Id);
                }
                else
                {
                    allowanceVM.CLE_Id = employee.CLE_Id;
                    allowanceVM.WAG_Id = WAG_Id;
                    allowanceVM.Emp_ID = employee.EMP_Id;
                    allowanceVM.CRI_Id = CRI_Id;
                    allowanceVM.Emp_Name = employee.EMP_.EMP_FirstName + " " + employee.EMP_.EMP_MiddleName + " " + employee.EMP_.EMP_SurName;
                }
                allowanceVM.FRM_ID = FRM_Id;
                wage_Register_allowances.Add(allowanceVM);
            }
            updateWageRegisterAllowance_1 updateWageallowance = new updateWageRegisterAllowance_1();
            updateWageallowance.CRI_Allowance_Name_1 = client_Requirements.CRI_Allowance_Name_1;
            updateWageallowance.AllowancesVMs = wage_Register_allowances;
            return View(updateWageallowance);
        }
        [HttpPost]
        public ActionResult UpdateAmount_1(updateWageRegisterAllowance_1 updateWageAllowance)
        {
            AllowanceManager allowanceManager = new AllowanceManager(_context);
            List<Wage_Register_Allowances_1> allowances = new List<Wage_Register_Allowances_1>();
            List<Wage_Register_AllowancesVM_1> list = updateWageAllowance.AllowancesVMs;
            allowances = Wage_Register_AllowancesVM_1.mapMeModels(list);
            string res = allowanceManager.UpdateAmount_1(allowances);
            if (res != string.Empty)
            {
                TempData["message"] = "Try Again";
            }
            return RedirectToAction("WageRegister", "WageRegister", new { WAG_Id = list[0].WAG_Id, FRM_Id = list[0].FRM_ID });
        }

        public IActionResult UpdateAllowanceAmount_2(int CRI_Id, int WAG_Id, int FRM_Id)
        {
            WageProcessManager wageProcessManager = new WageProcessManager(_context);
            DateTime WAG_Month = wageProcessManager.getWageProcessById(WAG_Id).WAG_Month;
            ViewBag.Wag_Month = WAG_Month.ToString("MMMM") + "-" + WAG_Month.ToString("yyyy");
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            AllowanceManager allowanceManager = new AllowanceManager(_context);
            Client_Requirements client_Requirements = clientsManager.GetRequirementsById(CRI_Id);
            List<Wage_Register_AllowancesVM_2> wage_Register_allowances = new List<Wage_Register_AllowancesVM_2>();
            IEnumerable<Clients_Employees> clientsEmployees = clientsManager.listActiveClientsEmployees(client_Requirements.CLI_Id, WAG_Month, client_Requirements.DES_Id);
            foreach (Clients_Employees employee in clientsEmployees)
            {
                Wage_Register_Allowances_2 allowance = new Wage_Register_Allowances_2();
                Wage_Register_AllowancesVM_2 allowanceVM = new Wage_Register_AllowancesVM_2();
                allowance = allowanceManager.GetAllowances_ByCLE_2(WAG_Id, employee.CLE_Id, employee.CLI_Id);
                if (allowance != null)
                {
                    allowance = allowanceManager.GetAllowancesById_2(allowance.WRA_Id_2);
                    allowanceVM = Wage_Register_AllowancesVM_2.mapMe(allowance, CRI_Id);
                }
                else
                {
                    allowanceVM.CLE_Id = employee.CLE_Id;
                    allowanceVM.WAG_Id = WAG_Id;
                    allowanceVM.Emp_ID = employee.EMP_Id;
                    allowanceVM.CRI_Id = CRI_Id;
                    allowanceVM.Emp_Name = employee.EMP_.EMP_FirstName + " " + employee.EMP_.EMP_MiddleName + " " + employee.EMP_.EMP_SurName;
                }
                allowanceVM.FRM_ID = FRM_Id;
                wage_Register_allowances.Add(allowanceVM);
            }
            updateWageRegisterAllowance_2 updateWageallowance = new updateWageRegisterAllowance_2();
            updateWageallowance.CRI_Allowance_Name_2 = client_Requirements.CRI_Allowance_Name_2;
            updateWageallowance.AllowancesVMs = wage_Register_allowances;
            return View(updateWageallowance);
        }
        [HttpPost]
        public ActionResult UpdateAmount_2(updateWageRegisterAllowance_2 updateWageAllowance)
        {
            AllowanceManager allowanceManager = new AllowanceManager(_context);
            List<Wage_Register_Allowances_2> allowances = new List<Wage_Register_Allowances_2>();
            List<Wage_Register_AllowancesVM_2> list = updateWageAllowance.AllowancesVMs;
            allowances = Wage_Register_AllowancesVM_2.mapMeModels(list);
            string res = allowanceManager.UpdateAmount_2(allowances);
            if (res != string.Empty)
            {
                TempData["message"] = "Try Again";
            }
            return RedirectToAction("WageRegister", "WageRegister", new { WAG_Id = list[0].WAG_Id, FRM_Id = list[0].FRM_ID });
        }

        public IActionResult UpdateAllowanceAmount_3(int CRI_Id, int WAG_Id, int FRM_Id)
        {
            WageProcessManager wageProcessManager = new WageProcessManager(_context);
            DateTime WAG_Month = wageProcessManager.getWageProcessById(WAG_Id).WAG_Month;
            ViewBag.Wag_Month = WAG_Month.ToString("MMMM") + "-" + WAG_Month.ToString("yyyy");
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            AllowanceManager allowanceManager = new AllowanceManager(_context);
            Client_Requirements client_Requirements = clientsManager.GetRequirementsById(CRI_Id);
            List<Wage_Register_AllowancesVM_3> wage_Register_allowances = new List<Wage_Register_AllowancesVM_3>();
            IEnumerable<Clients_Employees> clientsEmployees = clientsManager.listActiveClientsEmployees(client_Requirements.CLI_Id, WAG_Month, client_Requirements.DES_Id);
            foreach (Clients_Employees employee in clientsEmployees)
            {
                Wage_Register_Allowances_3 allowance = new Wage_Register_Allowances_3();
                Wage_Register_AllowancesVM_3 allowanceVM = new Wage_Register_AllowancesVM_3();
                allowance = allowanceManager.GetAllowances_ByCLE_3(WAG_Id, employee.CLE_Id, employee.CLI_Id);
                if (allowance != null)
                {
                    allowance = allowanceManager.GetAllowancesById_3(allowance.WRA_Id_3);
                    allowanceVM = Wage_Register_AllowancesVM_3.mapMe(allowance, CRI_Id);
                }
                else
                {
                    allowanceVM.CLE_Id = employee.CLE_Id;
                    allowanceVM.WAG_Id = WAG_Id;
                    allowanceVM.Emp_ID = employee.EMP_Id;
                    allowanceVM.CRI_Id = CRI_Id;
                    allowanceVM.Emp_Name = employee.EMP_.EMP_FirstName + " " + employee.EMP_.EMP_MiddleName + " " + employee.EMP_.EMP_SurName;
                }
                allowanceVM.FRM_ID = FRM_Id;
                wage_Register_allowances.Add(allowanceVM);
            }
            updateWageRegisterAllowance_3 updateWageallowance = new updateWageRegisterAllowance_3();
            updateWageallowance.CRI_Allowance_Name_3 = client_Requirements.CRI_Allowance_Name_3;
            updateWageallowance.AllowancesVMs = wage_Register_allowances;
            return View(updateWageallowance);
        }
        [HttpPost]
        public ActionResult UpdateAmount_3(updateWageRegisterAllowance_3 updateWageAllowance)
        {
            AllowanceManager allowanceManager = new AllowanceManager(_context);
            List<Wage_Register_Allowances_3> allowances = new List<Wage_Register_Allowances_3>();
            List<Wage_Register_AllowancesVM_3> list = updateWageAllowance.AllowancesVMs;
            allowances = Wage_Register_AllowancesVM_3.mapMeModels(list);
            string res = allowanceManager.UpdateAmount_3(allowances);
            if (res != string.Empty)
            {
                TempData["message"] = "Try Again";
            }
            return RedirectToAction("WageRegister", "WageRegister", new { WAG_Id = list[0].WAG_Id, FRM_Id = list[0].FRM_ID });
        }

        public IActionResult UpdateAllowanceAmount_4(int CRI_Id, int WAG_Id, int FRM_Id)
        {
            WageProcessManager wageProcessManager = new WageProcessManager(_context);
            DateTime WAG_Month = wageProcessManager.getWageProcessById(WAG_Id).WAG_Month;
            ViewBag.Wag_Month = WAG_Month.ToString("MMMM") + "-" + WAG_Month.ToString("yyyy");
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            AllowanceManager allowanceManager = new AllowanceManager(_context);
            Client_Requirements client_Requirements = clientsManager.GetRequirementsById(CRI_Id);
            List<Wage_Register_AllowancesVM_4> wage_Register_allowances = new List<Wage_Register_AllowancesVM_4>();
            IEnumerable<Clients_Employees> clientsEmployees = clientsManager.listActiveClientsEmployees(client_Requirements.CLI_Id, WAG_Month, client_Requirements.DES_Id);
            foreach (Clients_Employees employee in clientsEmployees)
            {
                Wage_Register_Allowances_4 allowance = new Wage_Register_Allowances_4();
                Wage_Register_AllowancesVM_4 allowanceVM = new Wage_Register_AllowancesVM_4();
                allowance = allowanceManager.GetAllowances_ByCLE_4(WAG_Id, employee.CLE_Id, employee.CLI_Id);
                if (allowance != null)
                {
                    allowance = allowanceManager.GetAllowancesById_4(allowance.WRA_Id_4);
                    allowanceVM = Wage_Register_AllowancesVM_4.mapMe(allowance, CRI_Id);
                }
                else
                {
                    allowanceVM.CLE_Id = employee.CLE_Id;
                    allowanceVM.WAG_Id = WAG_Id;
                    allowanceVM.Emp_ID = employee.EMP_Id;
                    allowanceVM.CRI_Id = CRI_Id;
                    allowanceVM.Emp_Name = employee.EMP_.EMP_FirstName + " " + employee.EMP_.EMP_MiddleName + " " + employee.EMP_.EMP_SurName;
                }
                allowanceVM.FRM_ID = FRM_Id;
                wage_Register_allowances.Add(allowanceVM);
            }
            updateWageRegisterAllowance_4 updateWageallowance = new updateWageRegisterAllowance_4();
            updateWageallowance.CRI_Allowance_Name_4 = client_Requirements.CRI_Allowance_Name_4;
            updateWageallowance.AllowancesVMs = wage_Register_allowances;
            return View(updateWageallowance);
        }
        [HttpPost]
        public ActionResult UpdateAmount_4(updateWageRegisterAllowance_4 updateWageAllowance)
        {
            AllowanceManager allowanceManager = new AllowanceManager(_context);
            List<Wage_Register_Allowances_4> allowances = new List<Wage_Register_Allowances_4>();
            List<Wage_Register_AllowancesVM_4> list = updateWageAllowance.AllowancesVMs;
            allowances = Wage_Register_AllowancesVM_4.mapMeModels(list);
            string res = allowanceManager.UpdateAmount_4(allowances);
            if (res != string.Empty)
            {
                TempData["message"] = "Try Again";
            }
            return RedirectToAction("WageRegister", "WageRegister", new { WAG_Id = list[0].WAG_Id, FRM_Id = list[0].FRM_ID });
        }

        public IActionResult UpdateAllowanceAmount_5(int CRI_Id, int WAG_Id, int FRM_Id)
        {
            WageProcessManager wageProcessManager = new WageProcessManager(_context);
            DateTime WAG_Month = wageProcessManager.getWageProcessById(WAG_Id).WAG_Month;
            ViewBag.Wag_Month = WAG_Month.ToString("MMMM") + "-" + WAG_Month.ToString("yyyy");
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            AllowanceManager allowanceManager = new AllowanceManager(_context);
            Client_Requirements client_Requirements = clientsManager.GetRequirementsById(CRI_Id);
            List<Wage_Register_AllowancesVM_5> wage_Register_allowances = new List<Wage_Register_AllowancesVM_5>();
            IEnumerable<Clients_Employees> clientsEmployees = clientsManager.listActiveClientsEmployees(client_Requirements.CLI_Id, WAG_Month, client_Requirements.DES_Id);
            foreach (Clients_Employees employee in clientsEmployees)
            {
                Wage_Register_Allowances_5 allowance = new Wage_Register_Allowances_5();
                Wage_Register_AllowancesVM_5 allowanceVM = new Wage_Register_AllowancesVM_5();
                allowance = allowanceManager.GetAllowances_ByCLE_5(WAG_Id, employee.CLE_Id, employee.CLI_Id);
                if (allowance != null)
                {
                    allowance = allowanceManager.GetAllowancesById_5(allowance.WRA_Id_5);
                    allowanceVM = Wage_Register_AllowancesVM_5.mapMe(allowance, CRI_Id);
                }
                else
                {
                    allowanceVM.CLE_Id = employee.CLE_Id;
                    allowanceVM.WAG_Id = WAG_Id;
                    allowanceVM.Emp_ID = employee.EMP_Id;
                    allowanceVM.CRI_Id = CRI_Id;
                    allowanceVM.Emp_Name = employee.EMP_.EMP_FirstName + " " + employee.EMP_.EMP_MiddleName + " " + employee.EMP_.EMP_SurName;
                }
                allowanceVM.FRM_ID = FRM_Id;
                wage_Register_allowances.Add(allowanceVM);
            }
            updateWageRegisterAllowance_5 updateWageallowance = new updateWageRegisterAllowance_5();
            updateWageallowance.CRI_Allowance_Name_5 = client_Requirements.CRI_Allowance_Name_5;
            updateWageallowance.AllowancesVMs = wage_Register_allowances;
            return View(updateWageallowance);
        }
        [HttpPost]
        public ActionResult UpdateAmount_5(updateWageRegisterAllowance_5 updateWageAllowance)
        {
            AllowanceManager allowanceManager = new AllowanceManager(_context);
            List<Wage_Register_Allowances_5> allowances = new List<Wage_Register_Allowances_5>();
            List<Wage_Register_AllowancesVM_5> list = updateWageAllowance.AllowancesVMs;
            allowances = Wage_Register_AllowancesVM_5.mapMeModels(list);
            string res = allowanceManager.UpdateAmount_5(allowances);
            if (res != string.Empty)
            {
                TempData["message"] = "Try Again";
            }
            return RedirectToAction("WageRegister", "WageRegister", new { WAG_Id = list[0].WAG_Id, FRM_Id = list[0].FRM_ID });
        }

        
    }
}