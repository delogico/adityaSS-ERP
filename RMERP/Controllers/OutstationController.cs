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
    public class OutstationController : Controller
    {
        private readonly RMERPContext _context;
        public IConfiguration Configuration;
        public OutstationController(RMERPContext context, IConfiguration configuration)
        {
            _context = context;
            Configuration = configuration;
        }

        public IActionResult UpdateOutstationHours(int CRI_Id, int WAG_Id, int FRM_Id)
        {
            WageProcessManager wageProcessManager = new WageProcessManager(_context);
            DateTime WAG_Month = wageProcessManager.getWageProcessById(WAG_Id).WAG_Month;
            ViewBag.Wag_Month = WAG_Month.ToString("MMMM") + "-" + WAG_Month.ToString("yyyy");
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            OutstationManager outstationManager = new OutstationManager(_context);
            Client_Requirements client_Requirements = clientsManager.GetRequirementsById(CRI_Id);
            List<Wage_Register_OutstationVM> wage_Register_Outstations = new List<Wage_Register_OutstationVM>();
            IEnumerable<Clients_Employees> clientsEmployees = clientsManager.listActiveClientsEmployees(client_Requirements.CLI_Id, WAG_Month, client_Requirements.DES_Id);
            foreach (Clients_Employees employee in clientsEmployees)
            {
                Wage_Register_Outstation outstation = new Wage_Register_Outstation();
                Wage_Register_OutstationVM outstationVM = new Wage_Register_OutstationVM();
                outstation = outstationManager.GetOutstationByCLE(WAG_Id, employee.CLE_Id, employee.CLI_Id);
                if (outstation != null)
                {
                    outstation = outstationManager.GetOutstationById(outstation.WRO_Id);
                    outstationVM = wageRegisterOutstationMapper.mapMe(outstation, CRI_Id);
                }
                else
                {
                    outstationVM.CLE_Id = employee.CLE_Id;
                    outstationVM.WAG_Id = WAG_Id;
                    outstationVM.Emp_ID = employee.EMP_Id;
                    outstationVM.CRI_Id = CRI_Id;
                    outstationVM.Emp_Name = employee.EMP_.EMP_FirstName + " " + employee.EMP_.EMP_MiddleName + " " + employee.EMP_.EMP_SurName;
                }
                outstationVM.FRM_ID = FRM_Id;
                wage_Register_Outstations.Add(outstationVM);
            }
            updateWageRegisterOutstation updateWageOutstation = new updateWageRegisterOutstation();
            updateWageOutstation.outstationVMs = wage_Register_Outstations;
            return View(updateWageOutstation);
        }

        [HttpPost]
        public ActionResult UpdateAmount(updateWageRegisterOutstation updateWageOutstation)
        {
            OutstationManager outstationManager = new OutstationManager(_context);
            List<Wage_Register_Outstation> outstations = new List<Wage_Register_Outstation>();
            List<Wage_Register_OutstationVM> list = updateWageOutstation.outstationVMs;
            outstations = wageRegisterOutstationMapper.mapMeModels(list);
            string res = outstationManager.UpdateAmount(outstations);
            if (res != string.Empty)
            {
                TempData["message"] = "Try Again";
            }
            return RedirectToAction("WageRegister", "WageRegister", new { WAG_Id = list[0].WAG_Id, FRM_Id = list[0].FRM_ID });
        }
       
    }
}