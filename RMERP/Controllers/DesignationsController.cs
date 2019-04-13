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

namespace RMERP.Controllers
{
    public class DesignationsController : Controller
    {
        private readonly RMERPContext _context;

        public DesignationsController(RMERPContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            DesignationManager designationManager = new DesignationManager(_context);
            return View(DesignationMapper.mapDesignations(designationManager.getDesignationsList().ToList()));
        }
        public ActionResult AddEditDesignation(int DES_Id = 0)
        {
            DesignationManager designationManager = new DesignationManager(_context);
            DesignationVM vm = new DesignationVM();
            if (DES_Id > 0)
            {
                vm = DesignationMapper.mapMe(designationManager.GetDesignationById(DES_Id));
            }
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddEditDesignation(DesignationVM designationVM)
        {
            DesignationManager designationManager = new DesignationManager(_context);
            Designations des = DesignationMapper.mapMeModel(designationVM);
            if (ModelState.IsValid)
            {
                string res = designationManager.saveEditDesignation(des);
                if (res != "")
                {
                    TempData["message"] = "Can not Inserted";
                }
            }

            return RedirectToAction("Index", "Designations");

        }
    }
}