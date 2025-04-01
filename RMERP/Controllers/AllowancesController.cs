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
using RMERP.DAL.App_Code;

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
            WageProcessManager wageProcessManager = new(_context);
            DateOnly WAG_Month = wageProcessManager.GetWageProcessByWAG_Id(WAG_Id).WAG_Month;
            ViewBag.Wag_Month = WAG_Month.ToString("MMMM") + "-" + WAG_Month.ToString("yyyy");
            ClientsManager clientsManager = new(_context, Configuration);
            AllowanceManager allowanceManager = new(_context);
            Client_Requirement client_Requirements = clientsManager.GetRequirementsById(CRI_Id);
            List<Wage_Register_AllowancesVM_1> wage_Register_allowances = [];
            IEnumerable<Clients_Employee> clientsEmployees = clientsManager.listActiveClientsEmployees(client_Requirements.CLI_Id, DAL.Helpers.ProjectUtils.DateToDateTime(WAG_Month), client_Requirements.DES_Id);
            foreach (Clients_Employee employee in clientsEmployees)
            {
                Wage_Register_AllowancesVM_1 allowanceVM = new();
                Wage_Register_Allowances_1 allowance = allowanceManager.GetAllowances_ByCLE_1(WAG_Id, employee.CLE_Id, employee.CLI_Id);
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
                    allowanceVM.Emp_Name = employee.EMP.EMP_FirstName + " " + employee.EMP.EMP_MiddleName + " " + employee.EMP.EMP_SurName;
                }
                allowanceVM.FRM_ID = FRM_Id;
                wage_Register_allowances.Add(allowanceVM);
            }
            updateWageRegisterAllowance_1 updateWageallowance = new()
            {
                CRI_Allowance_Name_1 = client_Requirements.CRI_Allowance_Name_1,
                AllowancesVMs = wage_Register_allowances,
                CLI_Id = client_Requirements.CLI_Id
            };
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
            //return RedirectToAction("WageRegister", "WageRegister", new { WAG_Id = list[0].WAG_Id, FRM_Id = list[0].FRM_ID });
            return RedirectToAction("WageProcessClientRegister", "WageRegister", new { WAG_Id = list[0].WAG_Id, FRM_Id = list[0].FRM_ID, CLI_Id = updateWageAllowance.CLI_Id });
        }

        public IActionResult UpdateAllowanceAmount_2(int CRI_Id, int WAG_Id, int FRM_Id)
        {
            WageProcessManager wageProcessManager = new WageProcessManager(_context);
            DateOnly WAG_Month = wageProcessManager.GetWageProcessByWAG_Id(WAG_Id).WAG_Month;
            ViewBag.Wag_Month = WAG_Month.ToString("MMMM") + "-" + WAG_Month.ToString("yyyy");
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            AllowanceManager allowanceManager = new AllowanceManager(_context);
            Client_Requirement client_Requirements = clientsManager.GetRequirementsById(CRI_Id);
            List<Wage_Register_AllowancesVM_2> wage_Register_allowances = new List<Wage_Register_AllowancesVM_2>();
            IEnumerable<Clients_Employee> clientsEmployees = clientsManager.listActiveClientsEmployees(client_Requirements.CLI_Id, DAL.Helpers.ProjectUtils.DateToDateTime(WAG_Month), client_Requirements.DES_Id);
            foreach (Clients_Employee employee in clientsEmployees)
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
                    allowanceVM.Emp_Name = employee.EMP.EMP_FirstName + " " + employee.EMP.EMP_MiddleName + " " + employee.EMP.EMP_SurName;
                }
                allowanceVM.FRM_ID = FRM_Id;
                wage_Register_allowances.Add(allowanceVM);
            }
            updateWageRegisterAllowance_2 updateWageallowance = new updateWageRegisterAllowance_2
            {
                CRI_Allowance_Name_2 = client_Requirements.CRI_Allowance_Name_2,
                AllowancesVMs = wage_Register_allowances,
                CLI_Id = client_Requirements.CLI_Id
            };
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
            //return RedirectToAction("WageRegister", "WageRegister", new { WAG_Id = list[0].WAG_Id, FRM_Id = list[0].FRM_ID });
            return RedirectToAction("WageProcessClientRegister", "WageRegister", new { WAG_Id = list[0].WAG_Id, FRM_Id = list[0].FRM_ID, CLI_Id = updateWageAllowance.CLI_Id });
        }

        public IActionResult UpdateAllowanceAmount_3(int CRI_Id, int WAG_Id, int FRM_Id)
        {
            WageProcessManager wageProcessManager = new WageProcessManager(_context);
            DateOnly WAG_Month = wageProcessManager.GetWageProcessByWAG_Id(WAG_Id).WAG_Month;
            ViewBag.Wag_Month = WAG_Month.ToString("MMMM") + "-" + WAG_Month.ToString("yyyy");
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            AllowanceManager allowanceManager = new AllowanceManager(_context);
            Client_Requirement client_Requirements = clientsManager.GetRequirementsById(CRI_Id);
            List<Wage_Register_AllowancesVM_3> wage_Register_allowances = new List<Wage_Register_AllowancesVM_3>();
            IEnumerable<Clients_Employee> clientsEmployees = clientsManager.listActiveClientsEmployees(client_Requirements.CLI_Id, DAL.Helpers.ProjectUtils.DateToDateTime(WAG_Month), client_Requirements.DES_Id);
            foreach (Clients_Employee employee in clientsEmployees)
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
                    allowanceVM.Emp_Name = employee.EMP.EMP_FirstName + " " + employee.EMP.EMP_MiddleName + " " + employee.EMP.EMP_SurName;
                }
                allowanceVM.FRM_ID = FRM_Id;
                wage_Register_allowances.Add(allowanceVM);
            }
            updateWageRegisterAllowance_3 updateWageallowance = new()
            {
                CRI_Allowance_Name_3 = client_Requirements.CRI_Allowance_Name_3,
                AllowancesVMs = wage_Register_allowances,
                CLI_Id = client_Requirements.CLI_Id
            };
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
            //return RedirectToAction("WageRegister", "WageRegister", new { WAG_Id = list[0].WAG_Id, FRM_Id = list[0].FRM_ID });
            return RedirectToAction("WageProcessClientRegister", "WageRegister", new { WAG_Id = list[0].WAG_Id, FRM_Id = list[0].FRM_ID, CLI_Id = updateWageAllowance.CLI_Id });
        }

        public IActionResult UpdateAllowanceAmount_4(int CRI_Id, int WAG_Id, int FRM_Id)
        {
            WageProcessManager wageProcessManager = new WageProcessManager(_context);
            DateOnly WAG_Month = wageProcessManager.GetWageProcessByWAG_Id(WAG_Id).WAG_Month;
            ViewBag.Wag_Month = WAG_Month.ToString("MMMM") + "-" + WAG_Month.ToString("yyyy");
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            AllowanceManager allowanceManager = new AllowanceManager(_context);
            Client_Requirement client_Requirements = clientsManager.GetRequirementsById(CRI_Id);
            List<Wage_Register_AllowancesVM_4> wage_Register_allowances = new List<Wage_Register_AllowancesVM_4>();
            IEnumerable<Clients_Employee> clientsEmployees = clientsManager.listActiveClientsEmployees(client_Requirements.CLI_Id, DAL.Helpers.ProjectUtils.DateToDateTime(WAG_Month), client_Requirements.DES_Id);
            foreach (Clients_Employee employee in clientsEmployees)
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
                    allowanceVM.Emp_Name = employee.EMP.EMP_FirstName + " " + employee.EMP.EMP_MiddleName + " " + employee.EMP.EMP_SurName;
                }
                allowanceVM.FRM_ID = FRM_Id;
                wage_Register_allowances.Add(allowanceVM);
            }
            updateWageRegisterAllowance_4 updateWageallowance = new()
            {
                CRI_Allowance_Name_4 = client_Requirements.CRI_Allowance_Name_4,
                AllowancesVMs = wage_Register_allowances,
                CLI_Id = client_Requirements.CLI_Id
            };
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
            //return RedirectToAction("WageRegister", "WageRegister", new { WAG_Id = list[0].WAG_Id, FRM_Id = list[0].FRM_ID });
            return RedirectToAction("WageProcessClientRegister", "WageRegister", new { WAG_Id = list[0].WAG_Id, FRM_Id = list[0].FRM_ID, CLI_Id = updateWageAllowance.CLI_Id });
        }

        public IActionResult UpdateAllowanceAmount_5(int CRI_Id, int WAG_Id, int FRM_Id)
        {
            WageProcessManager wageProcessManager = new WageProcessManager(_context);
            DateOnly WAG_Month = wageProcessManager.GetWageProcessByWAG_Id(WAG_Id).WAG_Month;
            ViewBag.Wag_Month = WAG_Month.ToString("MMMM") + "-" + WAG_Month.ToString("yyyy");
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            AllowanceManager allowanceManager = new AllowanceManager(_context);
            Client_Requirement client_Requirements = clientsManager.GetRequirementsById(CRI_Id);
            List<Wage_Register_AllowancesVM_5> wage_Register_allowances = new List<Wage_Register_AllowancesVM_5>();
            IEnumerable<Clients_Employee> clientsEmployees = clientsManager.listActiveClientsEmployees(client_Requirements.CLI_Id, DAL.Helpers.ProjectUtils.DateToDateTime(WAG_Month), client_Requirements.DES_Id);
            foreach (Clients_Employee employee in clientsEmployees)
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
                    allowanceVM.Emp_Name = employee.EMP.EMP_FirstName + " " + employee.EMP.EMP_MiddleName + " " + employee.EMP.EMP_SurName;
                }
                allowanceVM.FRM_ID = FRM_Id;
                wage_Register_allowances.Add(allowanceVM);
            }
            updateWageRegisterAllowance_5 updateWageallowance = new()
            {
                CRI_Allowance_Name_5 = client_Requirements.CRI_Allowance_Name_5,
                AllowancesVMs = wage_Register_allowances,
                CLI_Id = client_Requirements.CLI_Id
            };
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
            //return RedirectToAction("WageRegister", "WageRegister", new { WAG_Id = list[0].WAG_Id, FRM_Id = list[0].FRM_ID });
            return RedirectToAction("WageProcessClientRegister", "WageRegister", new { WAG_Id = list[0].WAG_Id, FRM_Id = list[0].FRM_ID, CLI_Id = updateWageAllowance.CLI_Id });
        }

        //-------------------------------------------------------------

        public IActionResult UpdateAllowanceAmount_6(int CRI_Id, int WAG_Id, int FRM_Id)
        {
            WageProcessManager wageProcessManager = new(_context);
            ClientsManager clientsManager = new(_context, Configuration);
            AllowanceManager allowanceManager = new(_context);

            Client_Requirement client_Requirements = clientsManager.GetRequirementsById(CRI_Id);
            DateOnly WAG_Month = wageProcessManager.GetWageProcessByWAG_Id(WAG_Id).WAG_Month;
            ViewBag.Wag_Month = WAG_Month.ToString("MMMM") + "-" + WAG_Month.ToString("yyyy");

            List<Wage_Register_AllowancesVM_6> wage_Register_allowances = [];
            IEnumerable<Clients_Employee> clientsEmployees = clientsManager.listActiveClientsEmployees(client_Requirements.CLI_Id, DAL.Helpers.ProjectUtils.DateToDateTime(WAG_Month), client_Requirements.DES_Id);

            foreach (Clients_Employee employee in clientsEmployees)
            {
                Wage_Register_Allowances_6 allowance = allowanceManager.GetAllowances_ByCLE_6(WAG_Id, employee.CLE_Id, employee.CLI_Id);
                Wage_Register_AllowancesVM_6 allowanceVM = new();
                if (allowance != null)
                {
                    allowance = allowanceManager.GetAllowancesById_6(allowance.WRA_Id_6);
                    allowanceVM = Wage_Register_AllowancesVM_6.mapMe(allowance, CRI_Id);
                }
                else
                {
                    allowanceVM.CLE_Id = employee.CLE_Id;
                    allowanceVM.WAG_Id = WAG_Id;
                    allowanceVM.Emp_ID = employee.EMP_Id;
                    allowanceVM.CRI_Id = CRI_Id;
                    allowanceVM.Emp_Name = employee.EMP.EMP_FirstName + " " + employee.EMP.EMP_MiddleName + " " + employee.EMP.EMP_SurName;
                }
                allowanceVM.FRM_ID = FRM_Id;
                wage_Register_allowances.Add(allowanceVM);
            }

            updateWageRegisterAllowance_6 updateWageallowance = new()
            {
                CRI_Allowance_Name_6 = client_Requirements.CRI_Allowance_Name_6,
                AllowancesVMs = wage_Register_allowances,
                CLI_Id = client_Requirements.CLI_Id
            };
            return View(updateWageallowance);
        }

        [HttpPost]
        public ActionResult UpdateAmount_6(updateWageRegisterAllowance_6 updateWageAllowance)
        {
            AllowanceManager allowanceManager = new(_context);
            List<Wage_Register_AllowancesVM_6> list = updateWageAllowance.AllowancesVMs;
            List<Wage_Register_Allowances_6> allowances = Wage_Register_AllowancesVM_6.mapMeModels(list);
            string res = allowanceManager.UpdateAmount_6(allowances);
            if (res != string.Empty)
            {
                TempData["message"] = "Try Again";
            }
            return RedirectToAction("WageProcessClientRegister", "WageRegister", new { WAG_Id = list[0].WAG_Id, FRM_Id = list[0].FRM_ID, CLI_Id = updateWageAllowance.CLI_Id });
        }

        public IActionResult UpdateAllowanceAmount_7(int CRI_Id, int WAG_Id, int FRM_Id)
        {
            WageProcessManager wageProcessManager = new(_context);
            ClientsManager clientsManager = new(_context, Configuration);
            AllowanceManager allowanceManager = new(_context);

            Client_Requirement client_Requirements = clientsManager.GetRequirementsById(CRI_Id);
            DateOnly WAG_Month = wageProcessManager.GetWageProcessByWAG_Id(WAG_Id).WAG_Month;
            ViewBag.Wag_Month = WAG_Month.ToString("MMMM") + "-" + WAG_Month.ToString("yyyy");

            List<Wage_Register_AllowancesVM_7> wage_Register_allowances = [];
            IEnumerable<Clients_Employee> clientsEmployees = clientsManager.listActiveClientsEmployees(client_Requirements.CLI_Id, DAL.Helpers.ProjectUtils.DateToDateTime(WAG_Month), client_Requirements.DES_Id);
            foreach (Clients_Employee employee in clientsEmployees)
            {
                Wage_Register_Allowances_7 allowance = allowanceManager.GetAllowances_ByCLE_7(WAG_Id, employee.CLE_Id, employee.CLI_Id);
                Wage_Register_AllowancesVM_7 allowanceVM = new();
                if (allowance != null)
                {
                    allowance = allowanceManager.GetAllowancesById_7(allowance.WRA_Id_7);
                    allowanceVM = Wage_Register_AllowancesVM_7.mapMe(allowance, CRI_Id);
                }
                else
                {
                    allowanceVM.CLE_Id = employee.CLE_Id;
                    allowanceVM.WAG_Id = WAG_Id;
                    allowanceVM.Emp_ID = employee.EMP_Id;
                    allowanceVM.CRI_Id = CRI_Id;
                    allowanceVM.Emp_Name = employee.EMP.EMP_FirstName + " " + employee.EMP.EMP_MiddleName + " " + employee.EMP.EMP_SurName;
                }
                allowanceVM.FRM_ID = FRM_Id;
                wage_Register_allowances.Add(allowanceVM);
            }
            updateWageRegisterAllowance_7 updateWageallowance = new()
            {
                CRI_Allowance_Name_7 = client_Requirements.CRI_Allowance_Name_7,
                AllowancesVMs = wage_Register_allowances,
                CLI_Id = client_Requirements.CLI_Id
            };
            return View(updateWageallowance);
        }

        [HttpPost]
        public ActionResult UpdateAmount_7(updateWageRegisterAllowance_7 updateWageAllowance)
        {
            AllowanceManager allowanceManager = new(_context);
            List<Wage_Register_AllowancesVM_7> list = updateWageAllowance.AllowancesVMs;
            List<Wage_Register_Allowances_7> allowances = Wage_Register_AllowancesVM_7.mapMeModels(list);
            string res = allowanceManager.UpdateAmount_7(allowances);
            if (res != string.Empty)
            {
                TempData["message"] = "Try Again";
            }
            //return RedirectToAction("WageRegister", "WageRegister", new { WAG_Id = list[0].WAG_Id, FRM_Id = list[0].FRM_ID });
            return RedirectToAction("WageProcessClientRegister", "WageRegister", new { WAG_Id = list[0].WAG_Id, FRM_Id = list[0].FRM_ID, CLI_Id = updateWageAllowance.CLI_Id });
        }

        public IActionResult UpdateAllowanceAmount_8(int CRI_Id, int WAG_Id, int FRM_Id)
        {
            WageProcessManager wageProcessManager = new(_context);
            ClientsManager clientsManager = new(_context, Configuration);
            AllowanceManager allowanceManager = new(_context);

            Client_Requirement client_Requirements = clientsManager.GetRequirementsById(CRI_Id);
            DateOnly WAG_Month = wageProcessManager.GetWageProcessByWAG_Id(WAG_Id).WAG_Month;
            ViewBag.Wag_Month = WAG_Month.ToString("MMMM") + "-" + WAG_Month.ToString("yyyy");

            List<Wage_Register_AllowancesVM_8> wage_Register_allowances = [];
            IEnumerable<Clients_Employee> clientsEmployees = clientsManager.listActiveClientsEmployees(client_Requirements.CLI_Id, DAL.Helpers.ProjectUtils.DateToDateTime(WAG_Month), client_Requirements.DES_Id);
            foreach (Clients_Employee employee in clientsEmployees)
            {
                Wage_Register_Allowances_8 allowance = allowanceManager.GetAllowances_ByCLE_8(WAG_Id, employee.CLE_Id, employee.CLI_Id);
                Wage_Register_AllowancesVM_8 allowanceVM = new();
                if (allowance != null)
                {
                    allowance = allowanceManager.GetAllowancesById_8(allowance.WRA_Id_8);
                    allowanceVM = Wage_Register_AllowancesVM_8.mapMe(allowance, CRI_Id);
                }
                else
                {
                    allowanceVM.CLE_Id = employee.CLE_Id;
                    allowanceVM.WAG_Id = WAG_Id;
                    allowanceVM.Emp_ID = employee.EMP_Id;
                    allowanceVM.CRI_Id = CRI_Id;
                    allowanceVM.Emp_Name = employee.EMP.EMP_FirstName + " " + employee.EMP.EMP_MiddleName + " " + employee.EMP.EMP_SurName;
                }
                allowanceVM.FRM_ID = FRM_Id;
                wage_Register_allowances.Add(allowanceVM);
            }
            updateWageRegisterAllowance_8 updateWageallowance = new()
            {
                CRI_Allowance_Name_8 = client_Requirements.CRI_Allowance_Name_8,
                AllowancesVMs = wage_Register_allowances,
                CLI_Id = client_Requirements.CLI_Id
            };
            return View(updateWageallowance);
        }

        [HttpPost]
        public ActionResult UpdateAmount_8(updateWageRegisterAllowance_8 updateWageAllowance)
        {
            AllowanceManager allowanceManager = new(_context);
            List<Wage_Register_AllowancesVM_8> list = updateWageAllowance.AllowancesVMs;
            List<Wage_Register_Allowances_8> allowances = Wage_Register_AllowancesVM_8.mapMeModels(list);
            string res = allowanceManager.UpdateAmount_8(allowances);
            if (res != string.Empty)
            {
                TempData["message"] = "Try Again";
            }
            return RedirectToAction("WageProcessClientRegister", "WageRegister", new { WAG_Id = list[0].WAG_Id, FRM_Id = list[0].FRM_ID, CLI_Id = updateWageAllowance.CLI_Id });
        }

        public IActionResult UpdateAllowanceAmount_9(int CRI_Id, int WAG_Id, int FRM_Id)
        {
            WageProcessManager wageProcessManager = new(_context);
            ClientsManager clientsManager = new(_context, Configuration);
            AllowanceManager allowanceManager = new(_context);

            Client_Requirement client_Requirements = clientsManager.GetRequirementsById(CRI_Id);
            DateOnly WAG_Month = wageProcessManager.GetWageProcessByWAG_Id(WAG_Id).WAG_Month;
            ViewBag.Wag_Month = WAG_Month.ToString("MMMM") + "-" + WAG_Month.ToString("yyyy");

            List<Wage_Register_AllowancesVM_9> wage_Register_allowances = [];
            IEnumerable<Clients_Employee> clientsEmployees = clientsManager.listActiveClientsEmployees(client_Requirements.CLI_Id, DAL.Helpers.ProjectUtils.DateToDateTime(WAG_Month), client_Requirements.DES_Id);
            foreach (Clients_Employee employee in clientsEmployees)
            {
                Wage_Register_Allowances_9 allowance = allowanceManager.GetAllowances_ByCLE_9(WAG_Id, employee.CLE_Id, employee.CLI_Id);
                Wage_Register_AllowancesVM_9 allowanceVM = new();
                if (allowance != null)
                {
                    allowance = allowanceManager.GetAllowancesById_9(allowance.WRA_Id_9);
                    allowanceVM = Wage_Register_AllowancesVM_9.mapMe(allowance, CRI_Id);
                }
                else
                {
                    allowanceVM.CLE_Id = employee.CLE_Id;
                    allowanceVM.WAG_Id = WAG_Id;
                    allowanceVM.Emp_ID = employee.EMP_Id;
                    allowanceVM.CRI_Id = CRI_Id;
                    allowanceVM.Emp_Name = employee.EMP.EMP_FirstName + " " + employee.EMP.EMP_MiddleName + " " + employee.EMP.EMP_SurName;
                }
                allowanceVM.FRM_ID = FRM_Id;
                wage_Register_allowances.Add(allowanceVM);
            }
            updateWageRegisterAllowance_9 updateWageallowance = new()
            {
                CRI_Allowance_Name_9 = client_Requirements.CRI_Allowance_Name_9,
                AllowancesVMs = wage_Register_allowances,
                CLI_Id = client_Requirements.CLI_Id
            };
            return View(updateWageallowance);
        }

        [HttpPost]
        public ActionResult UpdateAmount_9(updateWageRegisterAllowance_9 updateWageAllowance)
        {
            AllowanceManager allowanceManager = new(_context);
            List<Wage_Register_AllowancesVM_9> list = updateWageAllowance.AllowancesVMs;
            List<Wage_Register_Allowances_9> allowances = Wage_Register_AllowancesVM_9.mapMeModels(list);
            string res = allowanceManager.UpdateAmount_9(allowances);
            if (res != string.Empty)
            {
                TempData["message"] = "Try Again";
            }
            return RedirectToAction("WageProcessClientRegister", "WageRegister", new { WAG_Id = list[0].WAG_Id, FRM_Id = list[0].FRM_ID, CLI_Id = updateWageAllowance.CLI_Id });
        }

        public IActionResult UpdateAllowanceAmount_10(int CRI_Id, int WAG_Id, int FRM_Id)
        {
            WageProcessManager wageProcessManager = new(_context);
            ClientsManager clientsManager = new(_context, Configuration);
            AllowanceManager allowanceManager = new(_context);

            Client_Requirement client_Requirements = clientsManager.GetRequirementsById(CRI_Id);
            DateOnly WAG_Month = wageProcessManager.GetWageProcessByWAG_Id(WAG_Id).WAG_Month;
            ViewBag.Wag_Month = WAG_Month.ToString("MMMM") + "-" + WAG_Month.ToString("yyyy");

            List<Wage_Register_AllowancesVM_10> wage_Register_allowances = [];
            IEnumerable<Clients_Employee> clientsEmployees = clientsManager.listActiveClientsEmployees(client_Requirements.CLI_Id, DAL.Helpers.ProjectUtils.DateToDateTime(WAG_Month), client_Requirements.DES_Id);

            foreach (Clients_Employee employee in clientsEmployees)
            {
                Wage_Register_AllowancesVM_10 allowanceVM = new();
                Wage_Register_Allowances_10 allowance = allowanceManager.GetAllowances_ByCLE_10(WAG_Id, employee.CLE_Id, employee.CLI_Id);
                if (allowance != null)
                {
                    allowance = allowanceManager.GetAllowancesById_10(allowance.WRA_Id_10);
                    allowanceVM = Wage_Register_AllowancesVM_10.mapMe(allowance, CRI_Id);
                }
                else
                {
                    allowanceVM.CLE_Id = employee.CLE_Id;
                    allowanceVM.WAG_Id = WAG_Id;
                    allowanceVM.Emp_ID = employee.EMP_Id;
                    allowanceVM.CRI_Id = CRI_Id;
                    allowanceVM.Emp_Name = employee.EMP.EMP_FirstName + " " + employee.EMP.EMP_MiddleName + " " + employee.EMP.EMP_SurName;
                }
                allowanceVM.FRM_ID = FRM_Id;
                wage_Register_allowances.Add(allowanceVM);
            }

            updateWageRegisterAllowance_10 updateWageallowance = new()
            {
                CRI_Allowance_Name_10 = client_Requirements.CRI_Allowance_Name_10,
                AllowancesVMs = wage_Register_allowances,
                CLI_Id = client_Requirements.CLI_Id
            };
            return View(updateWageallowance);
        }

        [HttpPost]
        public ActionResult UpdateAmount_10(updateWageRegisterAllowance_10 updateWageAllowance)
        {
            AllowanceManager allowanceManager = new(_context);
            List<Wage_Register_AllowancesVM_10> list = updateWageAllowance.AllowancesVMs;
            List<Wage_Register_Allowances_10> allowances = Wage_Register_AllowancesVM_10.mapMeModels(list);
            string res = allowanceManager.UpdateAmount_10(allowances);
            if (res != string.Empty)
            {
                TempData["message"] = "Try Again";
            }
            return RedirectToAction("WageProcessClientRegister", "WageRegister", new { WAG_Id = list[0].WAG_Id, FRM_Id = list[0].FRM_ID, CLI_Id = updateWageAllowance.CLI_Id });
        }

        #region NEW CODE

        //public IActionResult NewUpdateAllowanceAmount_1(int CRI_Id, int WAG_Id, int FRM_Id)
        //{
        //    WageProcessManager wageProcessManager = new(_context);
        //    DateOnly WAG_Month = wageProcessManager.getWageProcessById(WAG_Id).WAG_Month;
        //    ViewBag.Wag_Month = WAG_Month.ToString("MMMM") + "-" + WAG_Month.ToString("yyyy");
        //    ClientsManager clientsManager = new(_context, Configuration);
        //    AllowanceManager allowanceManager = new(_context);
        //    Client_Requirement client_Requirements = clientsManager.GetRequirementsById(CRI_Id);
        //    List<Wage_Register_AllowancesVM_1> wage_Register_allowances = [];
        //    IEnumerable<Clients_Employee> clientsEmployees = clientsManager.listActiveClientsEmployees(client_Requirements.CLI_Id, DAL.Helpers.ProjectUtils.DateToDateTime(WAG_Month), client_Requirements.DES_Id);
        //    foreach (Clients_Employee employee in clientsEmployees)
        //    {
        //        Wage_Register_AllowancesVM_1 allowanceVM = new();
        //        Wage_Register_Allowances_1 allowance = allowanceManager.GetAllowances_ByCLE_1(WAG_Id, employee.CLE_Id, employee.CLI_Id);
        //        if (allowance != null)
        //        {
        //            allowance = allowanceManager.GetAllowancesById_1(allowance.WRA_Id_1);
        //            allowanceVM = Wage_Register_AllowancesVM_1.mapMe(allowance, CRI_Id);
        //        }
        //        else
        //        {
        //            allowanceVM.CLE_Id = employee.CLE_Id;
        //            allowanceVM.WAG_Id = WAG_Id;
        //            allowanceVM.Emp_ID = employee.EMP_Id;
        //            allowanceVM.CRI_Id = CRI_Id;
        //            allowanceVM.Emp_Name = employee.EMP.EMP_FirstName + " " + employee.EMP.EMP_MiddleName + " " + employee.EMP.EMP_SurName;
        //        }
        //        allowanceVM.FRM_ID = FRM_Id;
        //        wage_Register_allowances.Add(allowanceVM);
        //    }
        //    updateWageRegisterAllowance_1 updateWageallowance = new()
        //    {
        //        CRI_Allowance_Name_1 = client_Requirements.CRI_Allowance_Name_1,
        //        AllowancesVMs = wage_Register_allowances,
        //        CLI_Id = client_Requirements.CLI_Id
        //    };
        //    return View(updateWageallowance);
        //}

        //[HttpPost]
        //public ActionResult NewUpdateAmount_1(updateWageRegisterAllowance_1 updateWageAllowance)
        //{
        //    AllowanceManager allowanceManager = new AllowanceManager(_context);
        //    List<Wage_Register_Allowances_1> allowances = new List<Wage_Register_Allowances_1>();
        //    List<Wage_Register_AllowancesVM_1> list = updateWageAllowance.AllowancesVMs;
        //    allowances = Wage_Register_AllowancesVM_1.mapMeModels(list);
        //    string res = allowanceManager.UpdateAmount_1(allowances);
        //    if (res != string.Empty)
        //    {
        //        TempData["message"] = "Try Again";
        //    }
        //    return RedirectToAction("WageRegister", "WageRegister", new { WAG_Id = list[0].WAG_Id, FRM_Id = list[0].FRM_ID });
        //}

        //public IActionResult NewUpdateAllowanceAmount_2(int CRI_Id, int WAG_Id, int FRM_Id)
        //{
        //    WageProcessManager wageProcessManager = new WageProcessManager(_context);
        //    DateOnly WAG_Month = wageProcessManager.getWageProcessById(WAG_Id).WAG_Month;
        //    ViewBag.Wag_Month = WAG_Month.ToString("MMMM") + "-" + WAG_Month.ToString("yyyy");
        //    ClientsManager clientsManager = new ClientsManager(_context, Configuration);
        //    AllowanceManager allowanceManager = new AllowanceManager(_context);
        //    Client_Requirement client_Requirements = clientsManager.GetRequirementsById(CRI_Id);
        //    List<Wage_Register_AllowancesVM_2> wage_Register_allowances = new List<Wage_Register_AllowancesVM_2>();
        //    IEnumerable<Clients_Employee> clientsEmployees = clientsManager.listActiveClientsEmployees(client_Requirements.CLI_Id, DAL.Helpers.ProjectUtils.DateToDateTime(WAG_Month), client_Requirements.DES_Id);
        //    foreach (Clients_Employee employee in clientsEmployees)
        //    {
        //        Wage_Register_Allowances_2 allowance = new Wage_Register_Allowances_2();
        //        Wage_Register_AllowancesVM_2 allowanceVM = new Wage_Register_AllowancesVM_2();
        //        allowance = allowanceManager.GetAllowances_ByCLE_2(WAG_Id, employee.CLE_Id, employee.CLI_Id);
        //        if (allowance != null)
        //        {
        //            allowance = allowanceManager.GetAllowancesById_2(allowance.WRA_Id_2);
        //            allowanceVM = Wage_Register_AllowancesVM_2.mapMe(allowance, CRI_Id);
        //        }
        //        else
        //        {
        //            allowanceVM.CLE_Id = employee.CLE_Id;
        //            allowanceVM.WAG_Id = WAG_Id;
        //            allowanceVM.Emp_ID = employee.EMP_Id;
        //            allowanceVM.CRI_Id = CRI_Id;
        //            allowanceVM.Emp_Name = employee.EMP.EMP_FirstName + " " + employee.EMP.EMP_MiddleName + " " + employee.EMP.EMP_SurName;
        //        }
        //        allowanceVM.FRM_ID = FRM_Id;
        //        wage_Register_allowances.Add(allowanceVM);
        //    }
        //    updateWageRegisterAllowance_2 updateWageallowance = new()
        //    {
        //        CRI_Allowance_Name_2 = client_Requirements.CRI_Allowance_Name_2,
        //        AllowancesVMs = wage_Register_allowances,
        //        CLI_Id = client_Requirements.CLI_Id
        //    };
        //    return View(updateWageallowance);
        //}
        //[HttpPost]
        //public ActionResult NewUpdateAmount_2(updateWageRegisterAllowance_2 updateWageAllowance)
        //{
        //    AllowanceManager allowanceManager = new AllowanceManager(_context);
        //    List<Wage_Register_Allowances_2> allowances = new List<Wage_Register_Allowances_2>();
        //    List<Wage_Register_AllowancesVM_2> list = updateWageAllowance.AllowancesVMs;
        //    allowances = Wage_Register_AllowancesVM_2.mapMeModels(list);
        //    string res = allowanceManager.UpdateAmount_2(allowances);
        //    if (res != string.Empty)
        //    {
        //        TempData["message"] = "Try Again";
        //    }
        //    return RedirectToAction("WageRegister", "WageRegister", new { WAG_Id = list[0].WAG_Id, FRM_Id = list[0].FRM_ID });
        //}

        //public IActionResult NewUpdateAllowanceAmount_3(int CRI_Id, int WAG_Id, int FRM_Id)
        //{
        //    WageProcessManager wageProcessManager = new WageProcessManager(_context);
        //    DateOnly WAG_Month = wageProcessManager.getWageProcessById(WAG_Id).WAG_Month;
        //    ViewBag.Wag_Month = WAG_Month.ToString("MMMM") + "-" + WAG_Month.ToString("yyyy");
        //    ClientsManager clientsManager = new ClientsManager(_context, Configuration);
        //    AllowanceManager allowanceManager = new AllowanceManager(_context);
        //    Client_Requirement client_Requirements = clientsManager.GetRequirementsById(CRI_Id);
        //    List<Wage_Register_AllowancesVM_3> wage_Register_allowances = new List<Wage_Register_AllowancesVM_3>();
        //    IEnumerable<Clients_Employee> clientsEmployees = clientsManager.listActiveClientsEmployees(client_Requirements.CLI_Id, DAL.Helpers.ProjectUtils.DateToDateTime(WAG_Month), client_Requirements.DES_Id);
        //    foreach (Clients_Employee employee in clientsEmployees)
        //    {
        //        Wage_Register_Allowances_3 allowance = new Wage_Register_Allowances_3();
        //        Wage_Register_AllowancesVM_3 allowanceVM = new Wage_Register_AllowancesVM_3();
        //        allowance = allowanceManager.GetAllowances_ByCLE_3(WAG_Id, employee.CLE_Id, employee.CLI_Id);
        //        if (allowance != null)
        //        {
        //            allowance = allowanceManager.GetAllowancesById_3(allowance.WRA_Id_3);
        //            allowanceVM = Wage_Register_AllowancesVM_3.mapMe(allowance, CRI_Id);
        //        }
        //        else
        //        {
        //            allowanceVM.CLE_Id = employee.CLE_Id;
        //            allowanceVM.WAG_Id = WAG_Id;
        //            allowanceVM.Emp_ID = employee.EMP_Id;
        //            allowanceVM.CRI_Id = CRI_Id;
        //            allowanceVM.Emp_Name = employee.EMP.EMP_FirstName + " " + employee.EMP.EMP_MiddleName + " " + employee.EMP.EMP_SurName;
        //        }
        //        allowanceVM.FRM_ID = FRM_Id;
        //        wage_Register_allowances.Add(allowanceVM);
        //    }
        //    updateWageRegisterAllowance_3 updateWageallowance = new()
        //    {
        //        CRI_Allowance_Name_3 = client_Requirements.CRI_Allowance_Name_3,
        //        AllowancesVMs = wage_Register_allowances,
        //        CLI_Id = client_Requirements.CLI_Id
        //    };
        //    return View(updateWageallowance);
        //}
        //[HttpPost]
        //public ActionResult NewUpdateAmount_3(updateWageRegisterAllowance_3 updateWageAllowance)
        //{
        //    AllowanceManager allowanceManager = new AllowanceManager(_context);
        //    List<Wage_Register_Allowances_3> allowances = new List<Wage_Register_Allowances_3>();
        //    List<Wage_Register_AllowancesVM_3> list = updateWageAllowance.AllowancesVMs;
        //    allowances = Wage_Register_AllowancesVM_3.mapMeModels(list);
        //    string res = allowanceManager.UpdateAmount_3(allowances);
        //    if (res != string.Empty)
        //    {
        //        TempData["message"] = "Try Again";
        //    }
        //    return RedirectToAction("WageRegister", "WageRegister", new { WAG_Id = list[0].WAG_Id, FRM_Id = list[0].FRM_ID });
        //}

        //public IActionResult NewUpdateAllowanceAmount_4(int CRI_Id, int WAG_Id, int FRM_Id)
        //{
        //    WageProcessManager wageProcessManager = new WageProcessManager(_context);
        //    DateOnly WAG_Month = wageProcessManager.getWageProcessById(WAG_Id).WAG_Month;
        //    ViewBag.Wag_Month = WAG_Month.ToString("MMMM") + "-" + WAG_Month.ToString("yyyy");
        //    ClientsManager clientsManager = new ClientsManager(_context, Configuration);
        //    AllowanceManager allowanceManager = new AllowanceManager(_context);
        //    Client_Requirement client_Requirements = clientsManager.GetRequirementsById(CRI_Id);
        //    List<Wage_Register_AllowancesVM_4> wage_Register_allowances = new List<Wage_Register_AllowancesVM_4>();
        //    IEnumerable<Clients_Employee> clientsEmployees = clientsManager.listActiveClientsEmployees(client_Requirements.CLI_Id, DAL.Helpers.ProjectUtils.DateToDateTime(WAG_Month), client_Requirements.DES_Id);
        //    foreach (Clients_Employee employee in clientsEmployees)
        //    {
        //        Wage_Register_Allowances_4 allowance = new Wage_Register_Allowances_4();
        //        Wage_Register_AllowancesVM_4 allowanceVM = new Wage_Register_AllowancesVM_4();
        //        allowance = allowanceManager.GetAllowances_ByCLE_4(WAG_Id, employee.CLE_Id, employee.CLI_Id);
        //        if (allowance != null)
        //        {
        //            allowance = allowanceManager.GetAllowancesById_4(allowance.WRA_Id_4);
        //            allowanceVM = Wage_Register_AllowancesVM_4.mapMe(allowance, CRI_Id);
        //        }
        //        else
        //        {
        //            allowanceVM.CLE_Id = employee.CLE_Id;
        //            allowanceVM.WAG_Id = WAG_Id;
        //            allowanceVM.Emp_ID = employee.EMP_Id;
        //            allowanceVM.CRI_Id = CRI_Id;
        //            allowanceVM.Emp_Name = employee.EMP.EMP_FirstName + " " + employee.EMP.EMP_MiddleName + " " + employee.EMP.EMP_SurName;
        //        }
        //        allowanceVM.FRM_ID = FRM_Id;
        //        wage_Register_allowances.Add(allowanceVM);
        //    }
        //    updateWageRegisterAllowance_4 updateWageallowance = new()
        //    {
        //        CRI_Allowance_Name_4 = client_Requirements.CRI_Allowance_Name_4,
        //        AllowancesVMs = wage_Register_allowances,
        //        CLI_Id = client_Requirements.CLI_Id
        //    };
        //    return View(updateWageallowance);
        //}
        //[HttpPost]
        //public ActionResult NewUpdateAmount_4(updateWageRegisterAllowance_4 updateWageAllowance)
        //{
        //    AllowanceManager allowanceManager = new AllowanceManager(_context);
        //    List<Wage_Register_Allowances_4> allowances = new List<Wage_Register_Allowances_4>();
        //    List<Wage_Register_AllowancesVM_4> list = updateWageAllowance.AllowancesVMs;
        //    allowances = Wage_Register_AllowancesVM_4.mapMeModels(list);
        //    string res = allowanceManager.UpdateAmount_4(allowances);
        //    if (res != string.Empty)
        //    {
        //        TempData["message"] = "Try Again";
        //    }
        //    return RedirectToAction("WageRegister", "WageRegister", new { WAG_Id = list[0].WAG_Id, FRM_Id = list[0].FRM_ID });
        //}

        //public IActionResult NewUpdateAllowanceAmount_5(int CRI_Id, int WAG_Id, int FRM_Id)
        //{
        //    WageProcessManager wageProcessManager = new WageProcessManager(_context);
        //    DateOnly WAG_Month = wageProcessManager.getWageProcessById(WAG_Id).WAG_Month;
        //    ViewBag.Wag_Month = WAG_Month.ToString("MMMM") + "-" + WAG_Month.ToString("yyyy");
        //    ClientsManager clientsManager = new ClientsManager(_context, Configuration);
        //    AllowanceManager allowanceManager = new AllowanceManager(_context);
        //    Client_Requirement client_Requirements = clientsManager.GetRequirementsById(CRI_Id);
        //    List<Wage_Register_AllowancesVM_5> wage_Register_allowances = new List<Wage_Register_AllowancesVM_5>();
        //    IEnumerable<Clients_Employee> clientsEmployees = clientsManager.listActiveClientsEmployees(client_Requirements.CLI_Id, DAL.Helpers.ProjectUtils.DateToDateTime(WAG_Month), client_Requirements.DES_Id);
        //    foreach (Clients_Employee employee in clientsEmployees)
        //    {
        //        Wage_Register_Allowances_5 allowance = new Wage_Register_Allowances_5();
        //        Wage_Register_AllowancesVM_5 allowanceVM = new Wage_Register_AllowancesVM_5();
        //        allowance = allowanceManager.GetAllowances_ByCLE_5(WAG_Id, employee.CLE_Id, employee.CLI_Id);
        //        if (allowance != null)
        //        {
        //            allowance = allowanceManager.GetAllowancesById_5(allowance.WRA_Id_5);
        //            allowanceVM = Wage_Register_AllowancesVM_5.mapMe(allowance, CRI_Id);
        //        }
        //        else
        //        {
        //            allowanceVM.CLE_Id = employee.CLE_Id;
        //            allowanceVM.WAG_Id = WAG_Id;
        //            allowanceVM.Emp_ID = employee.EMP_Id;
        //            allowanceVM.CRI_Id = CRI_Id;
        //            allowanceVM.Emp_Name = employee.EMP.EMP_FirstName + " " + employee.EMP.EMP_MiddleName + " " + employee.EMP.EMP_SurName;
        //        }
        //        allowanceVM.FRM_ID = FRM_Id;
        //        wage_Register_allowances.Add(allowanceVM);
        //    }
        //    updateWageRegisterAllowance_5 updateWageallowance = new()
        //    {
        //        CRI_Allowance_Name_5 = client_Requirements.CRI_Allowance_Name_5,
        //        AllowancesVMs = wage_Register_allowances,
        //        CLI_Id = client_Requirements.CLI_Id
        //    };
        //    return View(updateWageallowance);
        //}
        //[HttpPost]
        //public ActionResult NewUpdateAmount_5(updateWageRegisterAllowance_5 updateWageAllowance)
        //{
        //    AllowanceManager allowanceManager = new AllowanceManager(_context);
        //    List<Wage_Register_Allowances_5> allowances = new List<Wage_Register_Allowances_5>();
        //    List<Wage_Register_AllowancesVM_5> list = updateWageAllowance.AllowancesVMs;
        //    allowances = Wage_Register_AllowancesVM_5.mapMeModels(list);
        //    string res = allowanceManager.UpdateAmount_5(allowances);
        //    if (res != string.Empty)
        //    {
        //        TempData["message"] = "Try Again";
        //    }
        //    return RedirectToAction("WageRegister", "WageRegister", new { WAG_Id = list[0].WAG_Id, FRM_Id = list[0].FRM_ID });
        //}

        #endregion

    }
}