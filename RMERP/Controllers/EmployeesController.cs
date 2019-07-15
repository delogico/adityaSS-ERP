using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;
using RMERP.DAL.ManagerClasses;
using Microsoft.AspNetCore.Mvc.Rendering;
using RMERP.Helpers;
using RMERP.DAL.Mappers;
using static RMERP.DAL.Helpers.ProjectUtils;
using SmartBreadcrumbs.Attributes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using RMERP.DAL.Helpers;
using System.IO;
using System.Net;

namespace RMERP.Controllers
{
    [Authorize]
    public class EmployeesController : Controller
    {
        private readonly RMERPContext _context;
        public IConfiguration _configuration;
        private IHostingEnvironment _hostingEnvironment;
        private static int Employee_Id;
        public EmployeesController(RMERPContext context, IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        [Breadcrumb("Employees")]
        public IActionResult Index()
        {
            EmpId = 0;
            int FRM_Id = 0;
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            if (sessionUtils.GetLoggedFirmID().HasValue)
            {
                FRM_Id = sessionUtils.GetLoggedFirmID().Value;
            }
            FirmsManager firmsManager = new FirmsManager(_context);
            ViewBag.FirmList = firmsManager.getFirmList();

            EmployeeManager employeeManager = new EmployeeManager(_context);
            return View(new Tuple<IEnumerable<EmployeeVM>, int>(EmployeesMapper.MapEmployees(employeeManager.GetEmployees(FRM_Id).ToList(), _context), FRM_Id));


        }
        [HttpPost]
        public IActionResult SearchEmployee(int FRM_Id, bool EMP_UAN_Number, bool EMP_ESIC_Number)
        {
            EmployeeManager employeeManager = new EmployeeManager(_context);
            List<EmployeeVM> listVM = EmployeesMapper.MapEmployees(employeeManager.SearchEmployees(FRM_Id, EMP_UAN_Number, EMP_ESIC_Number));
            FirmsManager firmsManager = new FirmsManager(_context);
            ViewBag.FirmList = firmsManager.getFirmList();
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            int Firm_Id=0;
            if (sessionUtils.GetLoggedFirmID().HasValue)
            {
                Firm_Id = sessionUtils.GetLoggedFirmID().Value;
            }
            return View("Index", new Tuple<IEnumerable<EmployeeVM>, int>(listVM, Firm_Id));
        }

        [HttpGet]
        [Breadcrumb("Employee Info", FromAction = "Index")]
        public ActionResult AddEditEmployee(int EMP_Id = 0)
        {
            EMP_Id = (EMP_Id <= 0 ? EmpId : EMP_Id);
            EmployeeManager employeeManager = new EmployeeManager(_context);
            DocumentTypesManager typesManager = new DocumentTypesManager(_context);
            FirmsManager firmsManager = new FirmsManager(_context);
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            IEnumerable<Firms> listFirms = new List<Firms>();
            listFirms = firmsManager.getFirmList();
            ViewBag.firmList = listFirms;
            EmployeeVM employeeVM = new EmployeeVM();
            if (EMP_Id > 0)
            {
                Employee_Id = EMP_Id;
                Employees emp = employeeManager.GetEmployeeById(EMP_Id);
                employeeVM = EmployeesMapper.MapMe(emp);
            }
            else
            {
                employeeVM.EMP_IsActive = true;
            }
            if (sessionUtils.GetLoggedFirmID().HasValue)
            {
                employeeVM.FRM_Id = sessionUtils.GetLoggedFirmID().Value;
            }
            ViewBag.DocumentTypes = typesManager.GetDocumentTypes();
            return View(employeeVM);
        }

        [HttpPost]
        public ActionResult AddEditEmployees(EmployeeVM employeeVM)
        {

            string res = string.Empty;
            EmployeeManager employeeManager = new EmployeeManager(_context);
            if (ModelState.IsValid)
            {
                SessionUtils sessionUtils = new SessionUtils(Request, Response);
                Employees employee = new Employees();
                employee = EmployeesMapper.MapMeModel(employeeVM);
                employee.ADM_Id_RegisteredBy = sessionUtils.GetLoggedAdminID();
                res = employeeManager.AddEditEmployee(employee);
            }
            if (res == string.Empty)
            {
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Employee data can not Inserted";
                return RedirectToAction("AddEditEmployee", new { EMP_Id = employeeVM.EMP_Id });
            }

        }

        public ActionResult ActiveEmployee(int EMP_Id, DateTime date)
        {

            EmployeeManager employeeManager = new EmployeeManager(_context);
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            employeeManager.ActiveEmployee(EMP_Id, date, sessionUtils.GetLoggedAdminID(), out string actionMessage);
            if (actionMessage != string.Empty)
            {
                TempData["message"] = actionMessage;
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Breadcrumb("Advance", FromAction = "AddEditEmployee")]
        public ActionResult AddEditAdvance(int EMP_Id, int ADV_Id = -1)
        {
            EmpId = (EMP_Id <= 0 ? EmpId : EMP_Id);
            EmployeeAdvanceVM employeeAdvanceVM = new EmployeeAdvanceVM();
            EmployeeManager employeeManager = new EmployeeManager(_context);
            Employee_Advance employee_Advance = new Employee_Advance();
            if (ADV_Id > 0)
            {
                employee_Advance = employeeManager.GetEmployeeAdvanceById(ADV_Id);
                employeeAdvanceVM = EmployeeAdvanceMapper.mapMe(employee_Advance);
            }
            if (EMP_Id > 0)
            {
                employeeAdvanceVM.EMP_Id = EMP_Id;
                employeeAdvanceVM.EmployeeName = EmployeesMapper.MapMe(employeeManager.GetEmployeeById(EMP_Id)).EMP_FullName;
            }
            return View(employeeAdvanceVM);
        }

        [HttpPost]
        public ActionResult AddEditAdvance(EmployeeAdvanceVM employeeAdvanceVM)
        {
            string res = string.Empty;
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            EmployeeManager employeeManager = new EmployeeManager(_context);
            if (ModelState.IsValid)
            {
                Employee_Advance employee_Advance = new Employee_Advance();
                employee_Advance = EmployeeAdvanceMapper.mapMeModel(employeeAdvanceVM);
                employee_Advance.ADM_Id_RegisteredBy = sessionUtils.GetLoggedAdminID();
                res = employeeManager.AddEditAdvance(employee_Advance);
            }
            if (res != string.Empty)
            {
                TempData["message"] = "Advance data can not Inserted";
            }

            return RedirectToAction("AddEditEmployee", new { EMP_Id = employeeAdvanceVM.EMP_Id, tab = "AddEditAdvance" });
        }

        public ActionResult DeleteAdvance(int ADV_Id, int EMP_Id)
        {
            string res = string.Empty;
            EmployeeManager employeeManager = new EmployeeManager(_context);
            res = employeeManager.DeleteAdvance(ADV_Id);
            if (res != string.Empty)
            {
                TempData["message"] = "Advance data can not Deleted";
            }
            return RedirectToAction("AddEditEmployee", new { EMP_Id = EMP_Id, tab = "AddEditAdvance" });
        }

        [Breadcrumb("Advance Taken", FromAction = "Index", FromController = typeof(WageProcessController))]
        public ActionResult AdvanceRptForBank(DateTime WAG_Month)
        {
            AdvanceWageRegisterManager advance = new AdvanceWageRegisterManager(_context);
            List<EmployeeAdvanceVM> advancesVM = EmployeeAdvanceMapper.mapAdvances(advance.AdvanceRptForBank((WAG_Month)));
            ViewBag.WAG_Month = WAG_Month.ToString("MMMM") + "-" + WAG_Month.ToString("yyyy");
            return View(advancesVM);
        }
        //[Breadcrumb("Advance EMI", FromAction = "Index", FromController = typeof(WageProcessController))]
        //public ActionResult NotCompletedAdvanceLst(DateTime WAG_Month,int WAG_Id)
        //{
        //    AdvanceWageRegisterManager advance = new AdvanceWageRegisterManager(_context);
        //    List<EmployeeAdvanceVM> advancesVM = EmployeeAdvanceMapper.mapAdvances(advance.NotCompletedAdvanceLst((WAG_Month)));
        //    ViewBag.WAG_Month = WAG_Month.ToString("MMMM") + "-" + WAG_Month.ToString("yyyy");
        //    return View(advancesVM);
        //}

        [Breadcrumb("Advance EMI", FromAction = "Index", FromController = typeof(WageProcessController))]
        public ActionResult UpdateAdvanceEMI(DateTime WAG_Month, int WAG_Id)
        {
            AdvanceWageRegisterManager advance = new AdvanceWageRegisterManager(_context);
            WageRegisterManager wageRegisterManager = new WageRegisterManager(_context);
            UpdateAdvanceEMI updateAdvanceEMI = new UpdateAdvanceEMI();
            List<EmployeeAdvanceVM> advancesVM = EmployeeAdvanceMapper.mapAdvances(advance.NotCompletedAdvanceLst((WAG_Month)));
            List<WageRegisterAdvancesVM> wageRegisterAdvancesVMs = WageRegisterAdvancesMapper.mapMeModels(wageRegisterManager.GetWageRegisterAdvances(WAG_Month));
            updateAdvanceEMI.employeeAdvanceVMs = advancesVM;
            updateAdvanceEMI.wageRegisterAdvancesVMs = wageRegisterAdvancesVMs;
            updateAdvanceEMI.WAG_Month = WAG_Month;
            updateAdvanceEMI.WAG_Id = WAG_Id;
            ViewBag.WAG_Month = WAG_Month.ToString("MMMM") + "-" + WAG_Month.ToString("yyyy");
            return View(updateAdvanceEMI);
        }

        [HttpPost]
        public ActionResult addWageRegisterAdvances(int EMP_id, int WAG_Id, decimal WAD_Amount, bool WAD_Status)
        {
            WageProcessManager wageProcess = new WageProcessManager(_context);
            AdvanceWageRegisterManager advance = new AdvanceWageRegisterManager(_context);
            ClientsManager clients = new ClientsManager(_context, _configuration);
            DateTime WAG_Month = wageProcess.getWageProcessById(WAG_Id).WAG_Month;
            int CLI_id = clients.GetClientIDByEmpID(EMP_id, WAG_Month);
            string res = advance.addWageRegisterAdvances(EMP_id, WAG_Id, CLI_id, WAD_Amount, WAG_Month, WAD_Status);
            return RedirectToAction("UpdateAdvanceEMI", new { WAG_Month = WAG_Month, WAG_Id = WAG_Id });
        }

        [HttpPost]
        public ActionResult UpdateWageRegisterAdvances(int EMP_id, decimal WAD_Amount, int WAG_Id, bool WAD_Status, int WAD_Id = -1)
        {
            WageProcessManager wageProcess = new WageProcessManager(_context);
            ClientsManager clients = new ClientsManager(_context, _configuration);
            AdvanceWageRegisterManager advance = new AdvanceWageRegisterManager(_context);
            DateTime WAG_Month = wageProcess.getWageProcessById(WAG_Id).WAG_Month;
            string res = advance.addWageRegisterAdvances(EMP_id, WAD_Amount, WAD_Status, WAG_Month, WAD_Id);
            return RedirectToAction("UpdateAdvanceEMI", new { WAG_Month = WAG_Month, WAG_Id = WAG_Id });
        }

        [HttpPost]
        public async Task<ActionResult> AddEmployeeDocuments(EmployeeVM employeeVM)
        {
            EmployeeDocumentManager documentManager = new EmployeeDocumentManager(_context);
            IFormFile file = employeeVM.employee_Document.EMD_Document;

            employeeVM.employee_Document.EMD_UploadedOn = ProjectUtils.DateNow();
            employeeVM.employee_Document.EMP_Id = employeeVM.EMP_Id;
            employeeVM.employee_Document.EMD_Name = file.FileName;

            int EMD_Id = documentManager.AddEmployeeDocument(EmployeeDocumentMapper.mapMeModel(employeeVM.employee_Document));

            if (EMD_Id > 0)
            {
                string newPath = ProjectUtils.GetTempFolderPath(_hostingEnvironment.WebRootPath);
                string DocumentPath = _configuration.GetSection("DEFAULT_FOLDER_PATH").Value + _configuration.GetSection("EMPLOYEE_DOCUMENT_PATH").Value;

                if (!System.IO.Directory.Exists(DocumentPath + "/" + EMD_Id))
                {
                    System.IO.Directory.CreateDirectory(DocumentPath + "/" + EMD_Id);
                }
                if (file == null || file.Length <= 0)
                {
                }
                else
                {

                    var path = Path.Combine(DocumentPath + "\\" + EMD_Id, file.FileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                }

            }
            return RedirectToAction("AddEditEmployee", new { EMP_Id = employeeVM.EMP_Id });
        }

        public ActionResult DeleteDocument(int EMD_Id)
        {
            EmployeeDocumentManager documentManager = new EmployeeDocumentManager(_context);
            string res = documentManager.DeleteEmployeeDocument(EMD_Id);
            if (res == string.Empty)
            {
                string newPath = ProjectUtils.GetTempFolderPath(_hostingEnvironment.WebRootPath);
                string DocumentPath = _configuration.GetSection("DEFAULT_FOLDER_PATH").Value + _configuration.GetSection("EMPLOYEE_DOCUMENT_PATH").Value;
                string[] files = System.IO.Directory.GetFiles(DocumentPath + "/" + EMD_Id);
                System.IO.Directory.Delete(DocumentPath + "/" + EMD_Id, true);
            }
            else
            {
                TempData["message"] = "Employee data can not deleted";
            }

            return RedirectToAction("AddEditEmployee", new { EMP_Id = Employee_Id });
        }
        public async Task<FileResult> DownloadDocument(int EMD_Id)
        {
            EmployeeDocumentManager documentManager = new EmployeeDocumentManager(_context);
            Employee_Documents document = documentManager.GetEmployeeDocumenyById(EMD_Id);
            //  WebClient wc = new WebClient();
            string newPath = ProjectUtils.GetTempFolderPath(_hostingEnvironment.WebRootPath);
            string DocumentPath = _configuration.GetSection("DEFAULT_FOLDER_PATH").Value + _configuration.GetSection("EMPLOYEE_DOCUMENT_PATH").Value;
            DocumentPath += "\\" + EMD_Id + "\\" + document.EMD_Name;

            var memory = new MemoryStream();
            using (var stream = new FileStream(DocumentPath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, GetContentType(DocumentPath), document.EMD_Name);

        }
        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }

        //[AcceptVerbs("Get", "Post")]
        public IActionResult CheckExistingAadhar(string EMP_Aadhar_Number, int EMP_Id)
        {
            EmployeeManager employeeManager = new EmployeeManager(_context);
            bool ExistingAadhar = false;
            try
            {
                ExistingAadhar = !employeeManager.CheckExistingAadhar(EMP_Aadhar_Number, EMP_Id);
                return Json(ExistingAadhar);
            }

            catch (Exception ex)
            {
                return Json(false);

            }

        }

    }
}