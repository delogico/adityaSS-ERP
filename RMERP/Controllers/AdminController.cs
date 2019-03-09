using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Http;
using RMERP.CustomExtensions;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using RMERP.DAL.ManagerClasses;
using RMERP.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Web;

namespace RMERP.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private RMERPContext _context ;    

        public AdminController(RMERPContext context)
        {
            _context = context;           
        }
        public IActionResult Index()
        {           
            ViewBag.Welcome = "Welcome " + HttpContext.Session.GetString("AdmEmailID");                     
            return View();
        }
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public IActionResult Login(AdminUsers adminUsers)
        {
            AdminUserManager _adminClass = new AdminUserManager(_context);
            if (ModelState.IsValid)
            {
                var user = _adminClass.Login(adminUsers);
                if (user!= null)
                {
                    var claims = new List<Claim>();
                    try
                    {
                        // Setting  
                        claims.Add(new Claim(ClaimTypes.Name, user.AdmEmailId));
                        var claimIdenties = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var claimPrincipal = new ClaimsPrincipal(claimIdenties);
                        var authenticationManager = Request.HttpContext;

                        // Sign In.  
                        authenticationManager.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimPrincipal);

                        CookieOptions options = new CookieOptions();
                        options.Expires = DateTime.Now.AddDays(1);
                        Response.Cookies.Append("AdminID",Convert.ToString(user.AdmId), options);
                       

                    }
                    catch (Exception ex)
                    { 
                        throw ex;
                    }
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login credentials.");
                }
            }
            return View();

        }
        [HttpGet]
        public ActionResult AdminUsers()
        {
            AdminUserManager adminUserManager = new AdminUserManager(_context);            
            return View(adminUserManager.getAdminUsersList());
        }
        public IActionResult Logout()
        {
            var login = HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login","Admin");
        }          
        [HttpGet]
        public ActionResult AddEditAdmin(int id=0)
        {
            AdminUserManager adminUserManager = new AdminUserManager(_context);
            AdminUsers adminUsers = new AdminUsers();
            List<Firms> listFirms = new List<Firms>();
            listFirms = adminUserManager.getFirmList();            
            ViewBag.firmList = listFirms;
            if (id != 0)
            {
                adminUsers= adminUserManager.EditAdminUser(id);
            }
            return PartialView("_AddEditAdmin", adminUsers);
        }       
        [HttpPost]
        public ActionResult saveEditAdmin(AdminUsers adminUsers)
        {
            var res = "";               
            try
            {                           
                AdminUserManager adminUserManager = new AdminUserManager(_context);
                res = adminUserManager.AddEditAdminUsers(adminUsers);
            }
            catch (Exception ex)
            {
                res=ex.Message;
            }
            return Content(res);
        }
        [HttpGet]
        public ActionResult listAdminUsers()
        {
            AdminUserManager adminUserManager = new AdminUserManager(_context);
            //return PartialView("_listAdminUsers", adminUserManager.getAdminUsersList());
            return RedirectToAction("AdminUsers");
        }
        [HttpGet]
        public ActionResult deleteAdminUser(int id=0)
        {
            AdminUserManager adminUserManager = new AdminUserManager(_context);
            adminUserManager.deleteAdminUser(id);
            return RedirectToAction("AdminUsers");
        }       

    }
}