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
using RMERP.DAL.App_Code;

namespace RMERP.Controllers
{
    [Authorize]
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
            WageProcessManager wageProcessManager = new(_context);
            DateOnly WAG_Month = wageProcessManager.GetWageProcessByWAG_Id(WAG_Id).WAG_Month;
            ViewBag.Wag_Month = WAG_Month.ToString("MMMM") + "-" + WAG_Month.ToString("yyyy");
            ClientsManager clientsManager = new(_context, Configuration);
            CanteenManager canteenManager = new(_context);
            Client_Requirement client_Requirements = clientsManager.GetRequirementsById(CRI_Id);
            List<Wage_Register_CanteenVM> wage_Register_Canteens = [];
            IEnumerable<Clients_Employee> clientsEmployees = clientsManager.listActiveClientsEmployees(client_Requirements.CLI_Id, DAL.Helpers.ProjectUtils.DateToDateTime(WAG_Month), client_Requirements.DES_Id);
            foreach (Clients_Employee employee in clientsEmployees)
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
                    canteenVM.Emp_Name = employee.EMP.EMP_FirstName + " " + employee.EMP.EMP_MiddleName + " " + employee.EMP.EMP_SurName;
                }
                canteenVM.FRM_ID = FRM_Id;
                wage_Register_Canteens.Add(canteenVM);
            }
            updateWageRegisterCanteen updateWageCanteen = new()
            {
                canteenVMs = wage_Register_Canteens,
                CLI_Id = client_Requirements.CLI_Id
            };
            return View(updateWageCanteen);
        }

        [HttpPost]
        public ActionResult UpdateAmount(updateWageRegisterCanteen updateWageCanteen)
        {
            CanteenManager canteenManager = new(_context);
            List<Wage_Register_Canteen> canteens = wageRegisterCanteenMapper.mapMeModels(updateWageCanteen.canteenVMs);
            string res = canteenManager.UpdateAmount(canteens);
            if (res != string.Empty)
            {
                TempData["message"] = "Try Again";
            }
            //return RedirectToAction("WageRegister", "WageRegister", new { WAG_Id = list[0].WAG_Id, FRM_Id = list[0].FRM_ID });
            return RedirectToAction("WageProcessClientRegister", "WageRegister", new { WAG_Id = updateWageCanteen.canteenVMs[0].WAG_Id, FRM_Id = updateWageCanteen.canteenVMs[0].FRM_ID, CLI_Id = updateWageCanteen.CLI_Id });
        }
    }
}