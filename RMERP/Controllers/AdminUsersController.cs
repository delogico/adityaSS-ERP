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
using SmartBreadcrumbs.Attributes;

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
        [DefaultBreadcrumb("Dashboard")]
        public IActionResult DashBoard()
        {
            ClientsManager clientsManager = new ClientsManager(_context);
            EmployeeManager employeeManager = new EmployeeManager(_context);
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            if (sessionUtils.GetLoggedFirmID().HasValue)
            {
                int FRM_Id = sessionUtils.GetLoggedFirmID().Value;
                ViewBag.clients = clientsManager.GetTotalClient(FRM_Id);
                ViewBag.employees = employeeManager.GetTotalEmployees(FRM_Id);
            }
            else
            {
                ViewBag.clients = clientsManager.GetTotalClient();
                ViewBag.employees = employeeManager.GetTotalEmployees();
            }          
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
        [Breadcrumb("Admin Users")]
        public IActionResult Index()
        {
            AdminUsersManager adminUsersManager = new AdminUsersManager(_context);
            IEnumerable<AdminUsers> AdminUsersList = adminUsersManager.GetAdminUsers();
            return View(AdminUsersList);
        }
        [HttpGet]
        [Breadcrumb("Add-Edit AdminUsers", FromAction = "Index")]
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
                adminUsersModel.ADM_Id = AdminId;
                adminUsersModel.ADM_FirstName = adminUsers.ADM_FirstName;
                adminUsersModel.ADM_MiddleName = adminUsers.ADM_MiddleName;
                adminUsersModel.ADM_LastName = adminUsers.ADM_LastName;
                adminUsersModel.ADM_EmailId = adminUsers.ADM_EmailId;
                adminUsersModel.ADM_Password = adminUsers.ADM_Password;
                adminUsersModel.ADM_Mobile = adminUsers.ADM_Mobile;
                adminUsersModel.FRM_Id = adminUsers.FRM_Id;                           
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
                adminUsers.ADM_Id = adminUsersModel.ADM_Id;
                adminUsers.ADM_FirstName = adminUsersModel.ADM_FirstName;
                adminUsers.ADM_MiddleName = adminUsersModel.ADM_MiddleName;
                adminUsers.ADM_LastName = adminUsersModel.ADM_LastName;
                adminUsers.ADM_EmailId = adminUsersModel.ADM_EmailId;
                adminUsers.ADM_Password = adminUsersModel.ADM_Password;
                adminUsers.ADM_Mobile = adminUsersModel.ADM_Mobile;
                adminUsers.FRM_Id = adminUsersModel.FRM_Id;

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