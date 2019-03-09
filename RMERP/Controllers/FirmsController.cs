using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;
using RMERP.DAL.ManagerClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

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
            return View(firmsManager.getFirmList());
        }       
        public ActionResult AddEditFirms(int id=-1)
        {
            FirmsManager firmsManager = new FirmsManager(_context);
            FirmsViewModel vm = new FirmsViewModel();
            if(id > 0)
            {
                Firms firms = new Firms();
                firms = firmsManager.GetFirms(id);
                vm.FrmId = firms.FrmId;
                vm.FrmName = firms.FrmName;
            }            
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddEditFirms(FirmsViewModel firmsViewModel)
        {
            FirmsManager firmsManager = new FirmsManager(_context);
            Firms firms = new Firms();
            firms.FrmId = firmsViewModel.FrmId;
            firms.FrmName = firmsViewModel.FrmName;
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
        public ActionResult DeleteFirms(int id=-1)
        {
            FirmsManager firmsManager = new FirmsManager(_context);
            if (id > 0)
            {
                if (ModelState.IsValid)
                {
                    string res = firmsManager.DeleteFirms(id);
                    if (res != "")
                    {
                        TempData["message"] = "Can not deleted";
                    }
                }                    
            }                     
            return RedirectToAction("Index", "Firms");
        }
    }
}