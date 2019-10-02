using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;
using RMERP.DAL.ManagerClasses;
using RMERP.DAL.Mappers;
using Microsoft.AspNetCore.Http;
using static RMERP.DAL.Helpers.ProjectUtils;
using SmartBreadcrumbs.Attributes;

namespace RMERP.Controllers
{
    public class FirmsController : Controller
    {
        private readonly RMERPContext _context;

        public FirmsController(RMERPContext context)
        {
            _context = context;
        }
        
        public IActionResult Index()
        {
            FirmsManager firmsManager = new FirmsManager(_context);            
            return View(FirmMapper.mapFirms(firmsManager.getFirmList().ToList()));
        }
        
        public ActionResult AddEditFirm(int FRM_Id=0)
        {
            EmployeeManager employeeManager = new EmployeeManager(_context);
            FirmsManager firmsManager = new FirmsManager(_context);
            FirmVM vm = new FirmVM();
            ViewBag.States = employeeManager.GetStates();
            if (FRM_Id > 0)
            {
                vm = FirmMapper.mapMe(firmsManager.GetFirm(FRM_Id));
            }            
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddEditFirms(FirmVM firmVM)
        {
            FirmsManager firmsManager = new FirmsManager(_context);
            Firms firms = FirmMapper.mapMeModel(firmVM);            
            if (ModelState.IsValid)
            {
                string res = firmsManager.saveEditFirm(firms);
                if (res != "")
                {
                    TempData["message"] = "Can not Inserted";
                }
            }
                     
            return RedirectToAction("Index", "Firms");
            
        }
    }
}