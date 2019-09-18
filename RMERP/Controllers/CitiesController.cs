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
    public class CitiesController : Controller
    {
        private readonly RMERPContext _context;

        public CitiesController(RMERPContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            CitiesManager citiesManager = new CitiesManager(_context);            
            return View(CitiesMapper.mapCities(citiesManager.getCityList().ToList()));
        }

        public ActionResult AddEditCity(int CITY_Id=0)
        {
            CitiesManager citiesManager = new CitiesManager(_context);
            CitiesVM vm = new CitiesVM();
            if(CITY_Id > 0)
            {
                vm = CitiesMapper.mapMe(citiesManager.GetCity(CITY_Id));
            }            
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddEditCity(CitiesVM citiesVM)
        {
            CitiesManager citiesManager = new CitiesManager(_context);
            Cities cities  = new Cities();
            cities.CIT_Id = citiesVM.CITY_Id;
            cities.CIT_Name = citiesVM.CITY_Name;
            if (ModelState.IsValid)
            {
                string res = citiesManager.saveEditCity(cities);
                if (res != "")
                {
                    TempData["message"] = "Can not Inserted";
                }
            }
                     
            return RedirectToAction("Index", "Cities");
            
        }
    }
}