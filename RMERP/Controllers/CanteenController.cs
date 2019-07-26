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

namespace RMERP.Controllers
{
    public class CanteenController : Controller
    {
        private readonly RMERPContext _context;
        public IConfiguration Configuration;

        public CanteenController(RMERPContext context, IConfiguration configuration)
        {
            _context = context;
            Configuration = configuration;
        }

        public IActionResult UpdateCanteenAmount(int CRI_Id, int WAG_Id, int FRM_Id)
        {
            WageProcessManager wageProcessManager = new WageProcessManager(_context);
            DateTime WAG_Month = wageProcessManager.getWageProcessById(WAG_Id).WAG_Month;
            ViewBag.Wag_Month = WAG_Month.ToString("MMMM") + "-" + WAG_Month.ToString("yyyy");
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            CanteenManager canteenManager = new CanteenManager(_context);
            Client_Requirements client_Requirements = clientsManager.GetRequirementsById(CRI_Id);
            List<Wage_Register_CanteenVM> wage_Register_Canteens = new List<Wage_Register_CanteenVM>();
            IEnumerable<Clients_Employees> clientsEmployees = clientsManager.listClientsEmployees(client_Requirements.CLI_Id, client_Requirements.DES_Id);
            foreach (Clients_Employees employee in clientsEmployees)
            {
                Wage_Register_Canteen canteen = new Wage_Register_Canteen();
                Wage_Register_CanteenVM canteenVM = new Wage_Register_CanteenVM();
                canteen = canteenManager.GetCanteenByCLE(WAG_Id, employee.CLE_Id, employee.CLI_Id);
                if (canteen != null)
                {
                    canteen = canteenManager.GetCanteenById(canteen.WRC_Id);
                    canteenVM = wageRegisterCanteenMapper.mapMe(canteen, CRI_Id);
                }
                else
                {
                    canteenVM.CLE_Id = employee.CLE_Id;
                    canteenVM.WAG_Id = WAG_Id;
                    canteenVM.Emp_ID = employee.EMP_Id;
                    canteenVM.CRI_Id = CRI_Id;
                    canteenVM.Emp_Name = employee.EMP_.EMP_FirstName + " " + employee.EMP_.EMP_MiddleName + " " + employee.EMP_.EMP_SurName;
                }
                canteenVM.FRM_ID = FRM_Id;
                wage_Register_Canteens.Add(canteenVM);
            }
            updateWageRegisterCanteen updateWageCanteen = new updateWageRegisterCanteen();
            updateWageCanteen.canteenVMs = wage_Register_Canteens;
            return View(updateWageCanteen);
        }

        [HttpPost]
        public ActionResult UpdateAmount(updateWageRegisterCanteen updateWageCanteen)
        {
            CanteenManager canteenManager = new CanteenManager(_context);
            List<Wage_Register_Canteen> canteens = new List<Wage_Register_Canteen>();
            List<Wage_Register_CanteenVM> list = updateWageCanteen.canteenVMs;
            canteens = wageRegisterCanteenMapper.mapMeModels(list);
            string res = canteenManager.UpdateAmount(canteens);
            if (res != string.Empty)
            {
                TempData["message"] = "Try Again";
            }
            return RedirectToAction("WageRegister", "WageRegister", new { WAG_Id = list[0].WAG_Id, FRM_Id = list[0].FRM_ID });
        }
    }
}