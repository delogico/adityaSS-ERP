using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RMERP.DAL.ManagerClasses;
using RMERP.DAL.Models;
using RMERP.Helpers;
using RMERP.DAL.ViewModel;

namespace RMERP.Controllers
{
    [Authorize]
    public class WageProcessController : Controller
    {
        private readonly RMERPContext _context;
        public IConfiguration Configuration;
        WageProcessManager wpm;

        public WageProcessController(RMERPContext context, IConfiguration configuration)
        {
            _context = context;
            Configuration = configuration;
            wpm = new WageProcessManager(context);

        }

        public IActionResult Index()
        {
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            DateTime nextMonth = wpm.nextWageMonth(sessionUtils.GetLoggedAdminID());
            ViewBag.month = nextMonth.ToString("MMMM", CultureInfo.CreateSpecificCulture("IN"));
            return View(wpm.getWageProcessList(sessionUtils.GetLoggedAdminID()));
        }
        public IActionResult CreateNextMonthWage()
        {
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            string rse = wpm.CreateNextMonthWage(sessionUtils.GetLoggedAdminID());
            return RedirectToAction("Index");
        }
        public IActionResult DeleteWageProcess(int WagId)
        {
            WageProcessManager wpm = new WageProcessManager(_context);
            string res = wpm.DeleteWageProcess(WagId);
            if (res != string.Empty)
            {
                TempData["message"] = "Wage Process can not Deleted";
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult nextWageMonth()
        {
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            DateTime nextMonth = wpm.nextWageMonth(sessionUtils.GetLoggedAdminID());
            return Content(nextMonth.ToString("MMMM", CultureInfo.CreateSpecificCulture("IN")));
        }

        public ActionResult WageProcessList()
        {
            SessionUtils sessionUtils = new SessionUtils(Request, Response);

            var model = wpm.getWageProcessList(sessionUtils.GetLoggedAdminID());
            return PartialView("_WageProcessList", model);
        }
        public ActionResult AttendancesStatus(int wagId)
        {
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            ClientsViewModel cvm = new ClientsViewModel();
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            cvm.Listclients = clientsManager.listClients(sessionUtils.GetLoggedFirmID(), true);
            cvm.WageID = wagId;
            return View(cvm);
        }
        public ActionResult UploadPage(int wagId, int CliId)
        {
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            UploadPageViewModel upvm = new UploadPageViewModel();
            upvm.WageMonth = wpm.GetMonthFromID(wagId);
            upvm.ClientName = clientsManager.GetClientById(CliId).CliName;
            upvm.WageId = wagId;
            upvm.ClientId = CliId;
            return View(upvm);
        }
        [HttpPost]
        public ActionResult UploadPage(UploadPageViewModel uvm,IHttppostedFileBase)
        {
            return View();
        }
    }
}