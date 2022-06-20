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
        
        public IActionResult Index()
        {
            int FRM_Id = 0;
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            if (sessionUtils.GetLoggedFirmID().HasValue)
            {
                FRM_Id = sessionUtils.GetLoggedFirmID().Value;
            }
            FirmsManager firmsManager = new FirmsManager(_context);
            ViewBag.FirmList = firmsManager.getFirmList();

            EmployeeManager employeeManager = new EmployeeManager(_context);
            IEnumerable<EMPLOYEE_LEFT_REASON_CODE> REASON_CODE = Enum.GetValues(typeof(EMPLOYEE_LEFT_REASON_CODE))
                                                       .Cast<EMPLOYEE_LEFT_REASON_CODE>();
            ViewBag.Reason_Code = from action in REASON_CODE
                                       select new SelectListItem
                                       {
                                           Text = ProjectUtils.GetStringValue(action),
                                           Value = ((int)action).ToString()
                                       };           
         
            return View(new Tuple<IEnumerable<EmployeeVM>, int>(EmployeesMapper.MapEmployees(employeeManager.GetEmployees(FRM_Id).ToList(), _context), FRM_Id));


        }
        [HttpGet]
        public JsonResult GetCity(int STA_Id)
        {
            EmployeeManager employeeManager = new EmployeeManager(_context);
            var citylist = new SelectList(employeeManager.GetCities(STA_Id), "CITY_Id", "CITY_Name");
            return Json(citylist);

        }
        [HttpPost]
        public IActionResult SearchEmployee(int FRM_Id, bool EMP_UAN_Number, bool EMP_ESIC_Number,string EMP_Aadhar_Number)
        {
            EmployeeManager employeeManager = new EmployeeManager(_context);
            List<EmployeeVM> listVM = EmployeesMapper.MapEmployees(employeeManager.SearchEmployees(FRM_Id, EMP_UAN_Number, EMP_ESIC_Number, EMP_Aadhar_Number));
            FirmsManager firmsManager = new FirmsManager(_context);
            ViewBag.FirmList = firmsManager.getFirmList();
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            int Firm_Id=0;
            if (sessionUtils.GetLoggedFirmID().HasValue)
            {
                Firm_Id = sessionUtils.GetLoggedFirmID().Value;
            }
            IEnumerable<EMPLOYEE_LEFT_REASON_CODE> REASON_CODE = Enum.GetValues(typeof(EMPLOYEE_LEFT_REASON_CODE))
                                                       .Cast<EMPLOYEE_LEFT_REASON_CODE>();
            ViewBag.Reason_Code = from action in REASON_CODE
                                  select new SelectListItem
                                  {
                                      Text = ProjectUtils.GetStringValue(action),
                                      Value = ((int)action).ToString()
                                  };
            return View("Index", new Tuple<IEnumerable<EmployeeVM>, int>(listVM, Firm_Id));
        }

        [HttpGet]        
        public ActionResult AddEditEmployee(int EMP_Id = 0)
        {          
            EmployeeManager employeeManager = new EmployeeManager(_context);
            DocumentTypesManager typesManager = new DocumentTypesManager(_context);
            FirmsManager firmsManager = new FirmsManager(_context);
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            IEnumerable<Firms> listFirms = new List<Firms>();
            listFirms = firmsManager.getFirmList();
            ViewBag.firmList = listFirms;
            EmployeeVM employeeVM = new EmployeeVM();
            employeeVM.EMP_Payment_Type = (int)PAYMENT_TYPE.Cheque_Cash;
            employeeVM.EMP_Is_IDBI_Other = (int)PAYMENT_BANK_TYPE.IDBI_To_Others;
            employeeVM.EMP_DateOfJoining = ProjectUtils.DateNow();
            employeeVM.EMP_DOB = ProjectUtils.DateNow();
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
            ViewBag.States = employeeManager.GetStates();            
            return View(employeeVM);
        }      

        [HttpPost]
        public ActionResult AddEditEmployees(EmployeeVM employeeVM)
        {
            string res = string.Empty;
            EmployeeManager employeeManager = new EmployeeManager(_context);
            SessionUtils sessionUtils = new SessionUtils(Request, Response);            
            if (ModelState.IsValid)
            {
                if(!employeeManager.CheckExistingAadhar(employeeVM.EMP_Aadhar_Number, employeeVM.EMP_Id, employeeVM.FRM_Id))
                {
                    Employees employee = new Employees();
                    employee = EmployeesMapper.MapMeModel(employeeVM);
                    #region Bank Info
                    employee.EMP_Payment_Type = employeeVM.EMP_Payment_Type;
                    employee.EMP_Is_IDBI_Other = employeeVM.EMP_Is_IDBI_Other;
                    if (employeeVM.EMP_Payment_Type == (int)PAYMENT_TYPE.Cheque_Cash)
                    {
                        employee.EMP_Bank = null;
                        employee.EMP_Account_Name = null;
                        employee.EMP_Account_Number = null;
                        employee.EMP_Branch = null;
                        employee.EMP_Bank_IFSC = null;
                        employee.EMP_Is_IDBI_Other = (int)PAYMENT_BANK_TYPE.IDBI_To_IDBI;
                    }
                    else
                    {
                        employee.EMP_Bank = employeeVM.EMP_Bank;
                        employee.EMP_Account_Name = employeeVM.EMP_Account_Name;
                        employee.EMP_Account_Number = employeeVM.EMP_Account_Number;
                        employee.EMP_Branch = employeeVM.EMP_Branch;
                        employee.EMP_Bank_IFSC = employeeVM.EMP_Bank_IFSC;
                    }
                    if (employeeVM.EMP_Is_IDBI_Other == (int)PAYMENT_BANK_TYPE.IDBI_To_IDBI)
                    {
                        employee.EMP_Bank = "IDBI BANK";
                    }
                    #endregion
                    employee.ADM_Id_RegisteredBy = sessionUtils.GetLoggedAdminID();
                    res = employeeManager.AddEditEmployee(employee);
                }                
            }
            if (res == string.Empty)
            {
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Employee data can not Inserted.";
                DocumentTypesManager typesManager = new DocumentTypesManager(_context);
                FirmsManager firmsManager = new FirmsManager(_context);
                IEnumerable<Firms> listFirms = new List<Firms>();
                listFirms = firmsManager.getFirmList();
                ViewBag.firmList = listFirms;
                ViewBag.DocumentTypes = typesManager.GetDocumentTypes();
                ViewBag.States = employeeManager.GetStates();
                return View("AddEditEmployee",employeeVM);
            }

        }

        [HttpPost]
        public ActionResult AddEditEmployeesPaymentInfo(EmployeePaymentVM employeePaymentVM)
        {
            string res = string.Empty;
            EmployeeManager employeeManager = new EmployeeManager(_context);
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            if (ModelState.IsValid)
            {
                Employees employee = employeeManager.GetEmployeeById(employeePaymentVM.EMP_Id);                
                employee.EMP_Payment_Type = employeePaymentVM.EMP_Payment_Type;
                employee.EMP_Is_IDBI_Other = employeePaymentVM.EMP_Is_IDBI_Other;
                if (employeePaymentVM.EMP_Payment_Type == (int)PAYMENT_TYPE.Cheque_Cash)
                {
                    employee.EMP_Bank = null;
                    employee.EMP_Account_Name = null;
                    employee.EMP_Account_Number = null;
                    employee.EMP_Branch = null;
                    employee.EMP_Bank_IFSC = null;
                    employee.EMP_Is_IDBI_Other = (int)PAYMENT_BANK_TYPE.IDBI_To_IDBI;
                }
                else
                {
                    employee.EMP_Bank = employeePaymentVM.EMP_Bank;
                    employee.EMP_Account_Name = employeePaymentVM.EMP_Account_Name;
                    employee.EMP_Account_Number = employeePaymentVM.EMP_Account_Number;
                    employee.EMP_Branch = employeePaymentVM.EMP_Branch;
                    employee.EMP_Bank_IFSC = employeePaymentVM.EMP_Bank_IFSC;
                }
                if (employeePaymentVM.EMP_Is_IDBI_Other == (int)PAYMENT_BANK_TYPE.IDBI_To_IDBI)
                {
                    employee.EMP_Bank = "IDBI BANK";
                }
                employee.ADM_Id_RegisteredBy = sessionUtils.GetLoggedAdminID();
                res = employeeManager.AddEditEmployee(employee);
            }
            if (res == string.Empty)
            {
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Employee payment data can not Inserted";
                return RedirectToAction("AddEditEmployee", new { EMP_Id = employeePaymentVM.EMP_Id });
            }

        }

        
        public ActionResult LeftEmployee(int EMP_Id,DateTime EMP_Left_Date, int EMP_Reason_Code)
        {
            EmployeeManager employeeManager = new EmployeeManager(_context);
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            employeeManager.ActiveEmployee(EMP_Id, EMP_Reason_Code, EMP_Left_Date, sessionUtils.GetLoggedAdminID(), out string actionMessage);
            if (actionMessage != string.Empty)
            {
                TempData["message"] = actionMessage;
            }
            return RedirectToAction("Index");
        }


        [HttpGet]        
        public ActionResult AddEditAdvance(int EMP_Id, int ADV_Id = -1)
        {
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

        public ActionResult DeleteAdvanceModel(int ADV_Id, int EMP_Id)
        {
            EmployeeManager employeeManager = new EmployeeManager(_context);
            Employee_Advance employee_Advance = employeeManager.GetEmployeeAdvanceById(ADV_Id);
            //return PartialView("_DeleteAdvance", new Tuple<IEnumerable<Wage_Register_Advances>, Employee_Advance>(employeeManager.ActiveAdvances(EMP_Id), employee_Advance));
            return PartialView("_DeleteAdvance",new Tuple<IEnumerable<Wage_Register_Advances>, Employee_Advance>(employeeManager.ActiveAdvances(EMP_Id), employee_Advance));
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
        
        public ActionResult AdvanceRptForBank(DateTime WAG_Month,int FRM_Id)
        {
            AdvanceWageRegisterManager advance = new AdvanceWageRegisterManager(_context);
            FirmsManager firmsManager = new FirmsManager(_context);
            List<EmployeeAdvanceVM> advancesVM = EmployeeAdvanceMapper.mapAdvances(advance.AdvanceRptForBank(WAG_Month, FRM_Id).ToList());
            ViewBag.WAG_Month = WAG_Month.ToString("MMMM") + "-" + WAG_Month.ToString("yyyy");
            ViewBag.FRM_Name = firmsManager.GetFirm(FRM_Id).FRM_ShortName;
            ViewBag.FRM_Id = FRM_Id;
            return View(advancesVM);
        }        

        public ActionResult UpdateAdvanceEMI(DateTime WAG_Month, int WAG_Id, int FRM_Id)
        {
            WageProcessManager processManager = new WageProcessManager(_context);
            FirmsManager firmsManager = new FirmsManager(_context);
            UpdateAdvancesVM updateAdvancesVM = new UpdateAdvancesVM();          
            updateAdvancesVM.WAG_ = processManager.getWageProcessById(WAG_Id);
            Firms firm = firmsManager.GetFirm(FRM_Id);
            updateAdvancesVM.FRM_Name = firm.FRM_ShortName;
            updateAdvancesVM.FRM_Id = firm.FRM_Id;            
            List<AdvanceVM> AdvanceVMs = GetAdvanceVMs(WAG_Month, WAG_Id, FRM_Id);           
            updateAdvancesVM.AdvanceVMs = AdvanceVMs;
            return View(updateAdvancesVM);
        }

        public DateTime FirstDayOfMonth(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, 1);
        }

        public DateTime LastDayOfMonth(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, 1).AddMonths(1).AddDays(-1);
        }

        public List<AdvanceVM> GetAdvanceVMs(DateTime WAG_Month, int WAG_Id, int FRM_Id)
        {
            WageRegisterManager wageRegisterManager = new WageRegisterManager(_context);
            AdvanceWageRegisterManager advance = new AdvanceWageRegisterManager(_context);
            List<Employee_Advance> advancesVM = advance.NotCompletedAdvanceLst(WAG_Month, FRM_Id, WAG_Id);
            List<Wage_Register_Advances> wage_Register_Advances = wageRegisterManager.GetWageRegisterAdvances();
            var advances = advancesVM.GroupBy(l => l.EMP_Id)
                                             .Select(cl => new EmployeeAdvanceVM
                                             {
                                                 EMP_Id = cl.First().EMP_Id,
                                                 EmployeeName = cl.First().EMP_.EMP_FirstName+" "+ cl.First().EMP_.EMP_MiddleName+" "+ cl.First().EMP_.EMP_SurName,
                                                 ADV_Amount = cl.Sum(c => c.ADV_Amount),
                                                 minDateAdvanceTaken = cl.Min(c=>c.ADV_RegisteredOn)
                                             }).ToList();

            List<AdvanceVM> AdvanceVMs = new List<AdvanceVM>();
            foreach (var item in advances)
            {
                Wage_Register_Advances register_Advance = wage_Register_Advances.Where(m => m.EMP_Id.Equals(item.EMP_Id) && m.WAG_Id.Equals(WAG_Id)).FirstOrDefault();
                               
                var TotalPaid = wage_Register_Advances.Where(m => m.EMP_Id.Equals(item.EMP_Id) && LastDayOfMonth(m.WAG_.WAG_Month) >= item.minDateAdvanceTaken && FirstDayOfMonth(m.WAG_.WAG_Month) <= LastDayOfMonth(WAG_Month)).Sum(m => m.WAD_Amount);
                AdvanceVM advanceVM = new AdvanceVM();
                if (register_Advance != null)
                {
                    advanceVM.WAD_Id = register_Advance.WAD_Id;
                    advanceVM.WAG_Id = register_Advance.WAG_Id;
                    advanceVM.WAD_Amount = register_Advance.WAD_Amount;
                    advanceVM.WAD_Is_LoanCompleted = register_Advance.WAD_Is_LoanCompleted;
                    advanceVM.WAD_Status = register_Advance.WAD_Status;
                }
                else
                {
                    advanceVM.WAD_Id = -1;
                    advanceVM.WAD_Amount = 0;
                    advanceVM.WAD_Is_LoanCompleted = false;                    
                }
              

                advanceVM.EMP_Id = item.EMP_Id;
                advanceVM.EMP_Name = item.EmployeeName;
                advanceVM.Total_Advance = item.ADV_Amount;
                advanceVM.Paid_WAD_Amount = Convert.ToDecimal(TotalPaid);


                AdvanceVMs.Add(advanceVM);
            }
            return AdvanceVMs;
        }
        public string AddAdvanceEMI(int WAD_Id, int EMP_id, int WAG_Id, decimal WAD_Amount, bool WAD_Is_LoanCompleted)
        {
            WageRegisterController wageRegisterController = new WageRegisterController(_context,_hostingEnvironment);
            AdvanceWageRegisterManager advanceManager = new AdvanceWageRegisterManager(_context);
            try
            {
                WageProcessManager wageProcessManager = new WageProcessManager(_context);
                ClientsManager clients = new ClientsManager(_context, _configuration);

                DateTime WAG_Month = wageProcessManager.getWageProcessById(WAG_Id).WAG_Month;
                int CLI_id = clients.GetClientIDByEmpID(EMP_id, WAG_Month);

                if(!wageRegisterController.IsWageSaved(EMP_id, CLI_id, WAG_Id))
                {
                    if (WAD_Id > 0)
                    {
                        advanceManager.editWageRegisterAdvances(WAD_Id, WAD_Amount, WAD_Is_LoanCompleted);
                    }
                    else
                    {
                        advanceManager.addWageRegisterAdvances(EMP_id, WAG_Id, CLI_id, WAD_Amount, WAG_Month, WAD_Is_LoanCompleted);
                    }
                    return "ok";
                }
                else
                {
                    return "You have to reset wage register first.";
                }                                
            }
            catch (Exception)
            {
                return "ko";
            }
            
                 
        }

        public string DeleteAdvanceEMI(int WAD_Id)
        {
            AdvanceWageRegisterManager advanceManager = new AdvanceWageRegisterManager(_context);
            try
            {
                advanceManager.DeleteAdvanceEMI(WAD_Id);
                return "ok";
            }
            catch (Exception)
            {
                //TempData["message"] = "Employee Advance can not deleted";
                return "ko";
            }

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
            return RedirectToAction("AddEditEmployee", new { EMP_Id = employeeVM.EMP_Id, tab = "AddEditDocuments" });
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

            return File(memory, ProjectUtils.GetContentType(DocumentPath), document.EMD_Name);

        }
        
                
        public IActionResult CheckExistingAadhar(string EMP_Aadhar_Number, int EMP_Id,int FRM_Id)
        {
            EMP_Aadhar_Number=EMP_Aadhar_Number.Trim().Replace(@"\", string.Empty);
            EmployeeManager employeeManager = new EmployeeManager(_context);
            bool ExistingAadhar = false;
            try
            {
                ExistingAadhar = !employeeManager.CheckExistingAadhar(EMP_Aadhar_Number, EMP_Id, FRM_Id);
                return Json(ExistingAadhar);
            }
            catch (Exception)
            {
                return Json(false);

            }

        }

        public ActionResult RejoinEmployee(int EMP_Id, DateTime EMP_Rejoin_Date)
        {
            EmployeeManager employeeManager = new EmployeeManager(_context);
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            try
            {
                Employees emp = employeeManager.GetEmployeeById(EMP_Id);
                int count = _context.Employees.Where(m => m.EMP_Aadhar_Number.Equals(emp.EMP_Aadhar_Number) && m.FRM_Id == emp.FRM_Id && m.EMP_IsActive.Value).Count();
                if (count > 0)
                {
                    TempData["message"] = "Employee is already active in "+emp.FRM_.FRM_Name;
                }
                else
                {
                    if (!employeeManager.RejoinEmployee(emp, EMP_Rejoin_Date, sessionUtils.GetLoggedAdminID()))
                    {
                        TempData["message"] = "We can not rejoin employee on same month. Please Try Again!";
                    }
                }
                
            }
            catch (Exception)
            {
                TempData["message"] = "Try Again";
            }
            return RedirectToAction("Index");
        }

        public string IsWageSaved(DateTime date,int EMP_Id)
        {
            string res = string.Empty;
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            WageProcessManager wageProcessManager = new WageProcessManager(_context);
            WageRegisterManager wageRegisterManager = new WageRegisterManager(_context);

            IEnumerable<Wage_Process> wageProcesses = wageProcessManager.GetWagFromDate(date, (sessionUtils.GetLoggedFirmID().HasValue ? sessionUtils.GetLoggedFirmID().Value:0));
            if (wageProcesses.Count()>0)
            {
                foreach(var wag in wageProcesses)
                {
                    IEnumerable<Wage_Register> list = wageRegisterManager.GetWageFrom_WAG_Id_EMP_Id(wag.WAG_Id, EMP_Id);
                    if (list.ToList().Count() > 0)
                    {
                        res += "Wage of <b>" + wag.WAG_Month.ToString("MMM-yyyy") + " ("+wag.FRM_.FRM_ShortName+")</b> is already saved for client <b>" + string.Join(",", list.Select(m => m.CLI_.CLI_Name)) + "</b>.<br/>";
                        res += "So you have to reset wage register first.<br/><br/>";
                    }                  
                       
                }
                
            }            
            return res;
        }
        public ActionResult EmployeeHistory(int EMP_Id)
        {
            EmployeeManager employeeManager = new EmployeeManager(_context);
            IEnumerable<EmployeeVM> EmployeeVMs = EmployeesMapper.MapEmployees(employeeManager.EmployeeHistory(EMP_Id).ToList());
            return View(EmployeeVMs);
        }
    }
}