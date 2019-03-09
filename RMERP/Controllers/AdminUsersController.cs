using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;
using RMERP.DAL.ManagerClasses;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using RMERP.Helpers;

namespace RMERP.Controllers
{
    [Authorize]
    public class AdminUsersController : Controller
    {
        private readonly RMERPContext _context;

        public AdminUsersController(RMERPContext context)
        {
            _context = context;
        }
        public IActionResult DashBoard()
        {
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            ViewBag.Welcome = "Welcome " + sessionUtils.GetLoggedAdminFullName();
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
            AdminUsersManager _adminClass = new AdminUsersManager(_context);
            if (ModelState.IsValid)
            {
                var user = _adminClass.Login(adminUsers);
                if (user != null)
                {
                    SessionUtils sessionUtils = new SessionUtils(Request,Response);
                    sessionUtils.Login(user);
                    return RedirectToAction("DashBoard", "AdminUsers");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login credentials.");
                }
            }
            return View();

        }
        public IActionResult Logout()
        {
            var login = HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "AdminUsers");
        }
        public IActionResult Index()
        {
            AdminUsersManager adminUsersManager = new AdminUsersManager(_context);
            IEnumerable<AdminUsers> AdminUsersList = adminUsersManager.GetAdminUsers();
            return View(AdminUsersList);
        }
        [HttpGet]
        public ActionResult AddEditAdminUsers(int AdminId)
        {
            AdminUsersManager adminUsersManager = new AdminUsersManager(_context);
            FirmsManager firmsManager = new FirmsManager(_context);
            ViewBag.FrmList = firmsManager.getFirmList();
            AdminUsersModel adminUsersModel = new AdminUsersModel();
            if (AdminId > 0)
            {
                AdminUsers adminUsers = new AdminUsers();
                adminUsers = adminUsersManager.GetAdminUsersById(AdminId);
                adminUsersModel.AdmId = AdminId;
                adminUsersModel.AdmFirstName = adminUsers.AdmFirstName;
                adminUsersModel.AdmMiddleName = adminUsers.AdmMiddleName;
                adminUsersModel.AdmLastName = adminUsers.AdmLastName;
                adminUsersModel.AdmEmailId = adminUsers.AdmEmailId;
                adminUsersModel.AdmPassword = adminUsers.AdmPassword;
                adminUsersModel.AdmMobile = adminUsers.AdmMobile;
                adminUsersModel.FrmId = adminUsers.FrmId;
            }           
            return View(adminUsersModel);
        }
        [HttpPost]
        public ActionResult AddEditAdminUsers(AdminUsersModel adminUsersModel)
        {
            string res = string.Empty;
            AdminUsersManager adminUsersManager = new AdminUsersManager(_context);
            if (ModelState.IsValid)
            {
                AdminUsers adminUsers = new AdminUsers();
                adminUsers.AdmId = adminUsersModel.AdmId;
                adminUsers.AdmFirstName = adminUsersModel.AdmFirstName;
                adminUsers.AdmMiddleName = adminUsersModel.AdmMiddleName;
                adminUsers.AdmLastName = adminUsersModel.AdmLastName;
                adminUsers.AdmEmailId = adminUsersModel.AdmEmailId;
                adminUsers.AdmPassword = adminUsersModel.AdmPassword;
                adminUsers.AdmMobile = adminUsersModel.AdmMobile;
                adminUsers.FrmId = adminUsersModel.FrmId;

                res = adminUsersManager.AddEditAdminUsers(adminUsers);
            }            
            if (res == string.Empty)
            {
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Admin Users can not Inserted";
                return RedirectToAction("AddEditAdminUsers");
            }
           
        }

        public ActionResult DeleteAdminUsers(int AdminId)
        {
            string res = string.Empty;
            AdminUsersManager adminUsersManager = new AdminUsersManager(_context);
            res= adminUsersManager.deleteAdminUser(AdminId);
            if (res != string.Empty)
            {
                TempData["message"] = "There is some problem! Please Try Again";
            }            
            return RedirectToAction("Index");
        }
    }
}