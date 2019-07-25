using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RMERP.DAL.ManagerClasses;
using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;
using RMERP.DAL.Helpers;
using RMERP.DAL.Mappers;
using Microsoft.AspNetCore.Authorization;
using RMERP.Helpers;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartBreadcrumbs.Attributes;

namespace RMERP.Controllers
{
    [Authorize]
    public class ClientsController : Controller
    { 
        private readonly RMERPContext _context;
        public IConfiguration Configuration;
        public static int ClientId;
        public static bool IsActive;
        private IHostingEnvironment _hostingEnvironment;

        public ClientsController(RMERPContext context, IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            Configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }
        [HttpGet]
        [Breadcrumb("Clients")]
        public IActionResult Index(bool IsActive=true)
        {
            ClientId = -1;
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            ClientsViewModel cvm = new ClientsViewModel();
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            cvm.Listclients = clientsManager.listClients(IsActive,sessionUtils.GetLoggedFirmID());            
            ViewBag.List = cvm.Listclients;
            ViewBag.clients = clientsManager.GetTotalClient();
            if (sessionUtils.GetLoggedFirmID().HasValue)
            {
               int FRM_Id = sessionUtils.GetLoggedFirmID().Value;
                ViewBag.totalClients = clientsManager.GetTotalClient(FRM_Id);
            }
            else{
                ViewBag.totalClients = clientsManager.GetTotalClient();
            }
            return View(cvm);
        }
        [HttpGet]
        public IActionResult GetClientList(bool IsActive = true,string Client="")
        {
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            SessionUtils sessionUtils = new SessionUtils(Request,Response);
            IEnumerable<Clients> listClient = clientsManager.listClients(IsActive,sessionUtils.GetLoggedFirmID(), Client);
            ViewBag.ActiveClient = "IsActive";
            return PartialView("_ClientList", listClient);
        }
        [HttpGet]
        [Breadcrumb("Client Info", FromAction = "Index")]
        public ActionResult AddEditClients(int id=-1)
        {
            ClientId = (id <= 0 ? ClientId : id);
            id = ClientId;
            int FRM_Id = 0;
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            if (sessionUtils.GetLoggedFirmID().HasValue)
            {
                FRM_Id = sessionUtils.GetLoggedFirmID().Value;
            }
            AdminUserManager adminUserManager = new AdminUserManager(_context);
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            FirmsManager firmsManager = new FirmsManager(_context);
            ClientsViewModel cv = new ClientsViewModel();
            cv.clientsModel = new ClientsModel();            
            cv.ParametersClientsModel = new ParametersClientsModel();
            cv.ParametersClientsModel.clientsModel = new ClientsModel();
            Clients clients = new Clients();
            cv.clientsModel.CLI_RegisteredOn = ProjectUtils.DateNow();
            cv.clientsModel.FRM_Id = FRM_Id;
            if (id > 0)
            {                
                clients=clientsManager.GetClientById(id);
                ClientId = id;
                cv.clientsModel.CLI_Id= clients.CLI_Id;
                cv.clientsModel.CLI_IsActive = clients.CLI_IsActive;
                cv.clientsModel.FRM_Id = clients.FRM_Id;
                cv.clientsModel.CLI_Name = clients.CLI_Name;
                cv.clientsModel.CLI_International_Domestic = clients.CLI_International_Domestic;
                cv.clientsModel.CLI_Address = clients.CLI_Address;
                cv.clientsModel.CITY_Id = clients.CITY_Id;
                cv.clientsModel.CLI_Pincode = clients.CLI_Pincode;
                cv.clientsModel.CLI_Phone = clients.CLI_Phone;
                cv.clientsModel.CLI_Fax = clients.CLI_Fax;
                cv.clientsModel.CLI_Email = clients.CLI_Email;
                cv.clientsModel.CLI_Email_2 = clients.CLI_Email_2;
                cv.clientsModel.CLI_Total_WorkingDays = clients.CLI_Total_WorkingDays;
                cv.clientsModel.CLI_No_Reduce_Days = clients.CLI_No_Reduce_Days;
                cv.clientsModel.CLI_WorkingHours_In_Day = clients.CLI_WorkingHours_In_Day;

                cv.ParametersClientsModel.clientsModel.CLI_GST_Number = clients.CLI_GST_Number;
                cv.ParametersClientsModel.clientsModel.CLI_GST_Rate = clients.CLI_GST_Rate;
                cv.ParametersClientsModel.clientsModel.CLI_HSN_Code = clients.CLI_HSN_Code;
                cv.ParametersClientsModel.clientsModel.CLI_TDS_Rate = clients.CLI_TDS_Rate;
                cv.ParametersClientsModel.CLI_Att_Month_Start = clients.CLI_Att_Month_Start;
                cv.ParametersClientsModel.CLI_Att_Month_End = clients.CLI_Att_Month_End;
                cv.ParametersClientsModel.CLI_Att_MonthReal = clients.CLI_Att_MonthReal;

                cv.clientsModel.CLI_GST_Number = "";
                cv.clientsModel.CLI_GST_Rate = 0;
                cv.clientsModel.CLI_HSN_Code = "";
                cv.clientsModel.CLI_TDS_Rate = 0;

                cv.clientsModel.ADM_Id_RegisterBy = clients.ADM_Id_RegisterBy;
                cv.clientsModel.CLI_RegisteredOn = clients.CLI_RegisteredOn;
                cv.clientsModel.CliLogoImage = clients.CLI_Logo;
                cv.contacts = ClientContactMapper.mapContacts(clientsManager.GetClientContactsListById(id).ToList());
                cv.requirements = ClientRequirementMapper.mapRequirements(clientsManager.GetClient_RequirementsofClient(id, true).ToList());
                cv.employees = ClientEmployeeMapper.mapEmployees(clientsManager.listClientsEmployees(id).ToList(), _context);
            }

            IEnumerable<ProjectUtils.Total_WorkingDyas_In_Month> WorkingDays = Enum.GetValues(typeof(ProjectUtils.Total_WorkingDyas_In_Month))
                                                       .Cast<ProjectUtils.Total_WorkingDyas_In_Month>();
            ViewBag.TotalWorkingDays = from action in WorkingDays
                                select new SelectListItem
                                {
                                    Text = GetFullEnumString(action.ToString()),
                                    Value = ((int)action).ToString()
                                };           
            IEnumerable<Firms> listFirms = new List<Firms>();
            List<Cities> listCity = new List<Cities>();          
           
            listFirms = firmsManager.getFirmList();
            listCity = adminUserManager.getCityList();
            ViewBag.firmList = listFirms;
            ViewBag.cityList = listCity;
           
            return View(cv);
        }
        public string GetFullEnumString(string action)
        {
            switch (action)
            {
                case "Consider_RealDays_In_Month":
                    return "Consider Real Days In Month";
                case "Exclude_WeeklyOff":
                    return "Exclude Weekly Off";
                case "Reduce_Fixed_Days":
                    return "Fixed Days";                
                default:
                    return "";
            }
        }
        [HttpPost]
        public async Task<ActionResult> AddEditClient(ClientsViewModel cv)
        {            
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            int clientID = 0;
            if (ModelState.IsValid)
            {
                SessionUtils sessionUtils = new SessionUtils(Request, Response);
                cv.clientsModel.ADM_Id_RegisterBy = sessionUtils.GetLoggedAdminID();
                Clients clients = new Clients();
                IFormFile file = cv.clientsModel.CLI_Logo;
                clients.CLI_Id = cv.clientsModel.CLI_Id;
                clients.FRM_Id = cv.clientsModel.FRM_Id;
                clients.CLI_Name = cv.clientsModel.CLI_Name;
                clients.CLI_International_Domestic = cv.clientsModel.CLI_International_Domestic;
                clients.CLI_Address = cv.clientsModel.CLI_Address;
                clients.CITY_Id = cv.clientsModel.CITY_Id;
                clients.CLI_Pincode = cv.clientsModel.CLI_Pincode;
                clients.CLI_Phone = cv.clientsModel.CLI_Phone;
                clients.CLI_Fax = cv.clientsModel.CLI_Fax;
                clients.CLI_Email = cv.clientsModel.CLI_Email;
                clients.CLI_Email_2 = cv.clientsModel.CLI_Email_2;

                clients.CLI_GST_Number = "";
                clients.CLI_GST_Rate = 0;
                clients.CLI_HSN_Code = "";
                clients.CLI_TDS_Rate =0;

                clients.CLI_Total_WorkingDays = cv.clientsModel.CLI_Total_WorkingDays;                
                clients.CLI_No_Reduce_Days = cv.clientsModel.CLI_No_Reduce_Days;
                clients.CLI_WorkingHours_In_Day = cv.clientsModel.CLI_WorkingHours_In_Day;

                clients.ADM_Id_RegisterBy = cv.clientsModel.ADM_Id_RegisterBy;
                clients.CLI_RegisteredOn = cv.clientsModel.CLI_RegisteredOn;

                if (cv.clientsModel.CLI_Logo != null)
                {
                    clients.CLI_Logo = cv.clientsModel.CLI_Logo.FileName;
                }
                else
                {
                    clients.CLI_Logo = cv.clientsModel.CliLogoImage;
                }
                var tuple = clientsManager.saveAddEditClients(clients);
                if (tuple.Item1 != "")
                {
                    TempData["message"] = "Client data can not Inserted";
                }
                else
                {
                    clientID = tuple.Item2;
                    TempData["message"] = "Successfull Done!";
                }
                #region
                string newPath = ProjectUtils.GetTempFolderPath(_hostingEnvironment.WebRootPath);
                string ImagePath = Configuration.GetSection("DEFAULT_FOLDER_PATH").Value + Configuration.GetSection("CLIENTS_LOGO_PATH").Value;

                if (!System.IO.Directory.Exists(ImagePath + "/" + clientID))
                {
                    System.IO.Directory.CreateDirectory(ImagePath + "/" + clientID);
                }
                else
                {                   
                    string[] files = System.IO.Directory.GetFiles(ImagePath + "/" + clientID);
                    if (file != null)
                    {
                        foreach (string s in files)
                        {
                            string fileName = System.IO.Path.GetFileName(s);
                            System.IO.File.Delete(ImagePath + "/" + clientID + "/" + fileName);
                        }
                    }                    
                }
                if (file == null || file.Length <= 0)
                {
                }
                else
                {
                    var path = Path.Combine(ImagePath + "\\" + clientID, file.FileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
                #endregion            
            }            
            return RedirectToAction("AddEditClients",new { id= clientID });
        }
        [HttpGet]
        [Breadcrumb("Add-Edit Contact", FromAction = "AddEditClients")]
        public ActionResult AddEditContacts(int CLI_Id, int CON_Id=-1)
        {
            ClientContactVM contactVM = new ClientContactVM();
            if (CLI_Id>0)
            {
                ClientId = CLI_Id;
                ClientsManager clientsManager = new ClientsManager(_context, Configuration);
                Clients clients = clientsManager.GetClientById(CLI_Id);
                ViewBag.ClientName = clients.CLI_Name;
                if (CON_Id > 0)
                {                                           
                    Client_Contacts contact = clientsManager.GetClientContactsById(CON_Id);
                    contactVM = ClientContactMapper.mapMe(contact);
                }else
                {
                    contactVM = new ClientContactVM();
                    contactVM.CON_Id = 0;
                    contactVM.CLI_Id = CLI_Id;
                    contactVM.CON_RegisteredOn = ProjectUtils.DateNow();
                    SessionUtils sessionUtils = new SessionUtils(Request, Response);
                    contactVM.ADM_Id_RegisteredBy = sessionUtils.GetLoggedAdminID();
                                   }
            }
           
            return View(contactVM);
        }
        [HttpPost]
        public ActionResult AddEditContacts(ClientContactVM contactVM)
        {
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            if (ModelState.IsValid)
            {
                if (ClientId > 0)
                {
                    Client_Contacts clientContacts = new Client_Contacts();
                    clientContacts.CLI_Id = ClientId;
                    clientContacts.CON_Id = contactVM.CON_Id;
                    clientContacts.CON_FirstName = contactVM.CON_FirstName;
                    clientContacts.CON_SurName = contactVM.CON_SurName;
                    clientContacts.CON_Designation = contactVM.CON_Designation;
                    clientContacts.CON_Mobile = contactVM.CON_Mobile;
                    clientContacts.CON_Email = contactVM.CON_Email;
                    clientContacts.CON_isPrimary = contactVM.CON_isPrimary;
                    clientContacts.CON_RegisteredOn = ProjectUtils.DateNow();
                    SessionUtils sessionUtils = new SessionUtils(Request, Response);
                    clientContacts.ADM_Id_RegisteredBy = sessionUtils.GetLoggedAdminID();
                    string res = clientsManager.saveAddEditContacts(clientContacts);
                    if (res != "")
                    {
                        TempData["message"] = "Contacts data can not Inserted";
                        return RedirectToAction("AddEditContacts", "Clients");
                    }
                }                
            }
            return RedirectToAction("AddEditClients",new { id = ClientId,tab= "ContactInfo" });
        }
        public ActionResult DeleteContact(int id=-1)
        {
            ClientsViewModel clientsViewModel = new ClientsViewModel();
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            if (ModelState.IsValid)
            {
                if (id > 0)
                {
                  string res=  clientsManager.deleteContacts(id);
                    if(res != string.Empty)
                    {
                        TempData["message"] = "Contacts data can not Deleted";
                    }
                }                
            }
            return RedirectToAction("AddEditClients", new { id = ClientId, tab = "ContactInfo" });
        }
        [HttpPost]
        public string InActiveClient(int ClientID,string Active)
        {
            string res = string.Empty;
            ClientsViewModel clientsViewModel = new ClientsViewModel();
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            if (ModelState.IsValid)
            {
                if (ClientID > 0)
                {
                        res = clientsManager.InActiveClient(ClientID, Convert.ToInt32(Request.Cookies["AdminID"]), Active);
                        if (res != string.Empty)
                        {
                            TempData["message"] = "Client data can not Deleted";
                        }                                  
                }
            }
            return res;
           // return RedirectToAction("Index","Clients",true);
        }
        [HttpGet]
        [Breadcrumb("Add-Edit Requirement", FromAction = "AddEditClients")]
        public ActionResult AddEditRequirement(int CLI_Id, int CRI_Id =-1)
        {
            ClientId = CLI_Id;
            DesignationManager designationManager = new DesignationManager(_context);
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            AllowanceManager AllowanceManager = new AllowanceManager(_context);
            ClientRequirementVM clientRequirement = new ClientRequirementVM();
            Clients client = clientsManager.GetClientById(CLI_Id);
            ViewBag.client = client;
            List<Client_Requirement_Allowances> listClientReqAllowances = new List<Client_Requirement_Allowances>();
            if (CRI_Id > 0)
            {
                clientRequirement=ClientRequirementMapper.mapMe(clientsManager.GetRequirementsById(CRI_Id));
                listClientReqAllowances = AllowanceManager.GetClient_Requirement_AllowanceList(CRI_Id);
            }
            else
            {
                ViewBag.Designation = designationManager.getRemainingDesignationsList(CLI_Id);
                clientRequirement.CRI_Id = -1;
                clientRequirement.CLI_Id = CLI_Id;
                clientRequirement.CRI_Active = true;
            }
           
            clientRequirement.allAllowances = AllowanceMapper.mapMeAllowancesWithClientReq(AllowanceManager.GetAllowanceList(), listClientReqAllowances);
            ViewBag.ADList = designationManager.getRemainingDesignationsList(CLI_Id).Select(m => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = Convert.ToString(m.DES_Id), Text = m.DES_Title });
            

            return View(clientRequirement);
        }       
        [HttpPost]
        public ActionResult AddEditRequirement(ClientRequirementVM clientRequirementVM)
        {
            string res = string.Empty;
            Client_Requirements cr = new Client_Requirements();
            List<Client_Requirement_Allowances> lst = AllowanceMapper.mapMeClientReqAllowances(clientRequirementVM.allAllowances);
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            SessionUtils sessionUtils = new SessionUtils(Request, Response);

            if (ModelState.IsValid)
            {
                if (clientRequirementVM.CRI_OT_Calculate_Payableday)
                {
                    clientRequirementVM.CRI_OT_Fixed_PerHour = 0;
                    clientRequirementVM.CRI_OT_Formula = null;
                }
                if (!clientRequirementVM.CRI_OT_Calculate_Payableday)
                {
                    if(clientRequirementVM.CRI_OT_Fixed_PerHour > 0 && clientRequirementVM.CRI_OT_Calculate_Differently)
                    {
                        clientRequirementVM.CRI_OT_Formula = null;
                    }
                    else if (!clientRequirementVM.CRI_OT_Calculate_Differently)
                    {
                        clientRequirementVM.CRI_OT_Fixed_PerHour = 0;
                    }                                    
                }
                if (!clientRequirementVM.CRI_Attendance_Allowance)
                {
                    clientRequirementVM.CRI_Attendance_Allowance_Rate = 0;
                    clientRequirementVM.CRI_Attendance_Allowance_MaximumDays = 0;
                }
                if (!clientRequirementVM.CRI_OutStation_Allowance)
                {
                    clientRequirementVM.CRI_OutStation_Allowance_Rate = 0;                    
                }
                cr = ClientRequirementMapper.mapMeModel(clientRequirementVM);                
                res = clientsManager.AddEditRequirement(cr, lst,sessionUtils.GetLoggedAdminID());
            }
            if (res != "")
            {
                TempData["message"] = "Error In Client Requirement! Please Check";
                return View();
            }
            return RedirectToAction("AddEditClients", new { id = ClientId, tab = "ClientRequirement" });
        }
        [Breadcrumb("Requirement History", FromAction = "AddEditClients")]
        public ActionResult HistoryRequirement(int DES_Id,int CLI_Id)
        {
            ClientId = CLI_Id; 
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            DesignationManager designationManager = new DesignationManager(_context);
            List<ClientRequirementVM> lst = ClientRequirementMapper.mapRequirements(clientsManager.GetClient_RequirementsList(DES_Id, CLI_Id, false).ToList());
            ViewBag.DES_Title = designationManager.GetDesignationsById(DES_Id);
            ViewBag.CLI_Name = clientsManager.GetClientById(CLI_Id).CLI_Name;
            return View(lst);
        }

        [HttpPost]
        public ActionResult Parameters(ClientsViewModel cvm)
        {
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            if (cvm.clientsModel.CLI_Id > 0)
            {
                Clients clients = new Clients();
                clients.CLI_Id = cvm.clientsModel.CLI_Id;
                clients.CLI_GST_Number = cvm.ParametersClientsModel.clientsModel.CLI_GST_Number;
                clients.CLI_GST_Rate = cvm.ParametersClientsModel.clientsModel.CLI_GST_Rate;
                clients.CLI_TDS_Rate = cvm.ParametersClientsModel.clientsModel.CLI_TDS_Rate;
                clients.CLI_HSN_Code = cvm.ParametersClientsModel.clientsModel.CLI_HSN_Code;
                clients.CLI_Total_WorkingDays = cvm.clientsModel.CLI_Total_WorkingDays;
                clients.CLI_No_Reduce_Days = cvm.clientsModel.CLI_No_Reduce_Days;
                clients.CLI_WorkingHours_In_Day = cvm.clientsModel.CLI_WorkingHours_In_Day;
                if (cvm.ParametersClientsModel.CLI_Att_MonthReal ==true)
                {
                    clients.CLI_Att_MonthReal = true;
                    clients.CLI_Att_Month_Start = null;
                    clients.CLI_Att_Month_End = null;
                }
                else
                {
                    clients.CLI_Att_MonthReal = false;
                    clients.CLI_Att_Month_Start = cvm.ParametersClientsModel.CLI_Att_Month_Start;
                    clients.CLI_Att_Month_End = cvm.ParametersClientsModel.CLI_Att_Month_End;
                }
                clientsManager.UpdateParameters(clients);
            }
            return RedirectToAction("AddEditClients", new { id = ClientId, tab = "Parameters" });
        }

        [HttpGet]
        [Breadcrumb("Assign Employee", FromAction = "AddEditClients")]
        public ActionResult AddEmployee(int CLI_Id,int CLE_Id=-1)
        {
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            ClientEmployeeVM cvm = new ClientEmployeeVM();
            int FRM_Id = clientsManager.GetClientById(CLI_Id).FRM_Id;
            DesignationManager designationManager = new DesignationManager(_context);
            IEnumerable<AssignEmployeeVM> listDesignations = designationManager.getDesignationsListInVM(CLI_Id);
            IEnumerable<EmployeeVM> listEmployee = null;
            if (CLE_Id > 0)
            {
                listEmployee = EmployeesMapper.MapEmployees(clientsManager.getEmployeeList(FRM_Id).ToList());
                Clients_Employees clientEmployee = clientsManager.ClientEmployeeById(CLE_Id);
                cvm.DES_Id = clientEmployee.DES_Id;
                cvm.EMP_Id = clientEmployee.EMP_Id;
                cvm.CLE_RegisteredOn = clientEmployee.CLE_RegisteredOn;
            }
            else
            {
                listEmployee = EmployeesMapper.MapEmployees(clientsManager.getActiveEmployeeList_NotAssignedYet(CLI_Id,FRM_Id).ToList());
            }

            ViewBag.EmployeeList = listEmployee;
            ViewBag.designationList = listDesignations;
            cvm.CLI_Id = CLI_Id;
            ViewBag.ClientName = clientsManager.GetClientById(CLI_Id).CLI_Name;            
            return View(cvm);
        }
        [HttpPost]
        public ActionResult ClientEmployee(ClientEmployeeVM cvm)
        {
            string res = string.Empty;
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            if (ModelState.IsValid)
            {
                Clients_Employees clientsEmployees = new Clients_Employees();
                clientsEmployees.CLE_Id = cvm.CLE_Id;
                clientsEmployees.CLI_Id = cvm.CLI_Id;
                clientsEmployees.EMP_Id = cvm.EMP_Id;
                clientsEmployees.DES_Id = cvm.DES_Id;
                clientsEmployees.CLE_RegisteredOn = cvm.CLE_RegisteredOn;
                //clientsEmployees.CLE_UnassignedOn = cvm.CLE_UnassignedOn;
                SessionUtils sessionUtils = new SessionUtils(Request, Response);
                res = clientsManager.ClientEmployee(clientsEmployees, sessionUtils.GetLoggedAdminID());
                if (res != string.Empty)
                {
                    TempData["message"] = "ClientEmployee data can not Inserted";
                }
            }
            return RedirectToAction("AddEditClients", new { id = cvm.CLI_Id, tab = "ClientEmployee" });
        }
        public ActionResult UnassignClientEmployee(DateTime UnassignedOn, int CLE_Id = -1)
        {
            ClientsViewModel clientsViewModel = new ClientsViewModel();
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            if (ModelState.IsValid)
            {
                if (CLE_Id > 0)
                {
                    string res = clientsManager.UnassignClientEmployee(CLE_Id, UnassignedOn, sessionUtils.GetLoggedAdminID());
                    if (res != string.Empty)
                    {
                        TempData["message"] = "Employee can not deleted";
                    }
                }
            }
            return RedirectToAction("AddEditClients", new { id = ClientId, tab = "ClientEmployee" });
        }
               
        public ActionResult DeleteAssignEmployee(int CLE_Id)
        {
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            string res = clientsManager.DeleteAssignEmployee(CLE_Id);
            if (res != string.Empty)
            {
                TempData["message"] = "Assign employee can not deleted!";
            }
            return RedirectToAction("AddEditClients", new { id = ClientId, tab = "ClientEmployee" });
        }

        //public FileResult GenerateExcelTemplate_TwoRow()
        //{
        //    ClientsManager clientsManager = new ClientsManager(_context, Configuration);
        //    Clients client = clientsManager.GetClientById(ClientId);
        //    IEnumerable<Clients_Employees> employees = clientsManager.listClientsEmployees(ClientId);
        //    string newPath = ProjectUtils.GetTempFolderPath(_hostingEnvironment.WebRootPath);
        //    string fileName = "Template_" + DateTime.Now.ToString("ddMMyyyyHHmm") + "_" + client.CLI_Name + "_TwoRow.xlsx";
        //    string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, fileName);
        //    FileInfo file = new FileInfo(Path.Combine(newPath, fileName));
        //    var memory = new MemoryStream();
        //    using (var fs = new FileStream(Path.Combine(newPath, fileName), FileMode.Create, FileAccess.Write))
        //    {
        //        IWorkbook workbook;
        //        workbook = new XSSFWorkbook();
        //        ISheet excelSheet = workbook.CreateSheet("Template");
        //        IFont font = workbook.CreateFont();
        //        font.IsBold = true;
        //        font.FontHeightInPoints = ((short)24);
        //        font.FontName = ("Cambria");

        //        ICellStyle styleHeader = workbook.CreateCellStyle();
        //        styleHeader.FillBackgroundColor = HSSFColor.BlueGrey.Index;
        //        styleHeader.SetFont(font);

        //        // Grey25Percent background
        //        ICellStyle style = workbook.CreateCellStyle();
        //        style.FillForegroundColor = IndexedColors.Grey25Percent.Index;
        //        style.FillPattern = FillPattern.SolidForeground;
        //        style.FillBackgroundColor = IndexedColors.Grey25Percent.Index;

        //        // Style the cell with borders all around.
        //        style.BorderBottom = (BorderStyle.Thin);
        //        style.BottomBorderColor = (IndexedColors.Black.Index);
        //        style.BorderLeft = (BorderStyle.Thin);
        //        style.LeftBorderColor = (IndexedColors.Black.Index);
        //        style.BorderRight = (BorderStyle.Thin);
        //        style.RightBorderColor = (IndexedColors.Black.Index);
        //        style.BorderTop = (BorderStyle.Thin);
        //        style.TopBorderColor = (IndexedColors.Black.Index);

        //        IRow row = excelSheet.CreateRow(0);
        //        row.Height = 500;

        //        DateTime[] period = DateHelper.getStartEndDatePeriodForAttendance(client, DateTime.Now);
        //        int TotalDays = Convert.ToInt32((period[1] - period[0]).TotalDays) + 4;


        //        ICell CellHeader = row.CreateCell(0);
        //        CellHeader.SetCellValue(client.CLI_Name);
        //        CellHeader.CellStyle = styleHeader;
        //        CellUtil.SetAlignment(CellHeader, workbook, (short)HorizontalAlignment.Center);
        //        excelSheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, TotalDays - 7));

        //        ICell CellMonth = row.CreateCell(TotalDays - 6);
        //        string fullMonthName = DateTime.Now.ToString("MMM", CultureInfo.CreateSpecificCulture("IN"));
        //        CellMonth.SetCellValue(fullMonthName + "-" + DateTime.Now.ToString("yy"));
        //        CellMonth.CellStyle = styleHeader;
        //        CellUtil.SetAlignment(CellMonth, workbook, (short)HorizontalAlignment.Center);
        //        excelSheet.AddMergedRegion(new CellRangeAddress(0, 0, TotalDays - 6, TotalDays));

        //        row = excelSheet.CreateRow(1);
        //        row.HeightInPoints = ((5 * excelSheet.DefaultRowHeightInPoints));
        //        //excelSheet.AutoSizeColumn(1);


        //        ICell cell0 = row.CreateCell(0);
        //        cell0.SetCellValue("SR.NO");
        //        cell0.CellStyle = style;
        //        ICell cell1 = row.CreateCell(1);
        //        cell1.SetCellValue("EMP_Id");
        //        cell1.CellStyle = style;
        //        ICell cell2 = row.CreateCell(2);
        //        cell2.SetCellValue("Designation");
        //        cell2.CellStyle = style;
        //        ICell cell3 = row.CreateCell(3);
        //        cell3.SetCellValue("NAME");
        //        cell3.CellStyle = style;
        //        int i = 4;
        //        DateTime tmpDate = period[0];
        //        while (period[1] >= tmpDate)
        //        {
        //            ICell c = row.CreateCell(i);
        //            c.SetCellValue(tmpDate.Day);
        //            c.CellStyle = style;
        //            excelSheet.AutoSizeColumn(i);
        //            tmpDate = tmpDate.AddDays(1);
        //            i++;
        //        }

        //        int rowCount = 2;
        //        int j = 1;
        //        foreach (var item in employees)
        //        {
        //            row = excelSheet.CreateRow(rowCount);
        //            row.CreateCell(0).SetCellValue(j);

        //            //row.CreateCell(1).SetCellValue(ProjectUtils.convertDigit(item.EMP_Id));
        //            row.CreateCell(1).SetCellValue(item.EMP_Id.ToString("D5"));
        //            row.CreateCell(2).SetCellValue(item.DES_.DES_Title);
        //            //row.CreateCell(3).SetCellValue(item.EMP_.EMP_FullName);
        //            row.CreateCell(3).SetCellValue(item.EMP_.EMP_FirstName + " " + item.EMP_.EMP_MiddleName + " " + item.EMP_.EMP_SurName);

        //            excelSheet.SetColumnWidth(2, 6000);
        //            excelSheet.SetColumnWidth(3, 6000);


        //            excelSheet.AddMergedRegion(new CellRangeAddress(rowCount, rowCount + 1, 0, 0));
        //            excelSheet.AddMergedRegion(new CellRangeAddress(rowCount, rowCount + 1, 1, 1));
        //            excelSheet.AddMergedRegion(new CellRangeAddress(rowCount, rowCount + 1, 2, 2));
        //            excelSheet.AddMergedRegion(new CellRangeAddress(rowCount, rowCount + 1, 3, 3));

        //            int k = 4;
        //            DateTime tmp1Date = period[0];
        //            while (period[1] >= tmp1Date)
        //            {
        //                ICell c = row.CreateCell(k);
        //                tmp1Date = tmp1Date.AddDays(1);
        //                k++;
        //            }
        //            rowCount = rowCount + 2;
        //            j++;
        //        }

        //        workbook.Write(fs);
        //    }
        //    using (var stream = new FileStream(Path.Combine(newPath, fileName), FileMode.Open))
        //    {
        //        stream.CopyToAsync(memory);
        //    }
        //    memory.Position = 0;
        //    new FileInfo(Path.Combine(newPath, fileName)).Delete();
        //    return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        //}

        //public FileResult GenerateExcelTemplate_OneRow()
        //{
        //    ClientsManager clientsManager = new ClientsManager(_context, Configuration);
        //    Clients client = clientsManager.GetClientById(ClientId);
        //    IEnumerable<Clients_Employees> employees = clientsManager.listClientsEmployees(ClientId);
        //    string newPath = ProjectUtils.GetTempFolderPath(_hostingEnvironment.WebRootPath);
        //    string fileName = "Template_" + DateTime.Now.ToString("ddMMyyyyHHmm") + "_" + client.CLI_Name + "_OneRow.xlsx";
        //    string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, fileName);
        //    FileInfo file = new FileInfo(Path.Combine(newPath, fileName));
        //    var memory = new MemoryStream();
        //    using (var fs = new FileStream(Path.Combine(newPath, fileName), FileMode.Create, FileAccess.Write))
        //    {
        //        IWorkbook workbook;
        //        workbook = new XSSFWorkbook();
        //        ISheet excelSheet = workbook.CreateSheet("Template");
        //        IFont font = workbook.CreateFont();
        //        font.IsBold = true;
        //        font.FontHeightInPoints = ((short)24);
        //        font.FontName = ("Cambria");

        //        ICellStyle styleHeader = workbook.CreateCellStyle();
        //        styleHeader.FillBackgroundColor = HSSFColor.BlueGrey.Index;
        //        styleHeader.SetFont(font);

        //        // Grey25Percent background
        //        ICellStyle style = workbook.CreateCellStyle();
        //        style.FillForegroundColor = IndexedColors.Grey25Percent.Index;
        //        style.FillPattern = FillPattern.SolidForeground;
        //        style.FillBackgroundColor = IndexedColors.Grey25Percent.Index;

        //        // Style the cell with borders all around.
        //        style.BorderBottom = (BorderStyle.Thin);
        //        style.BottomBorderColor = (IndexedColors.Black.Index);
        //        style.BorderLeft = (BorderStyle.Thin);
        //        style.LeftBorderColor = (IndexedColors.Black.Index);
        //        style.BorderRight = (BorderStyle.Thin);
        //        style.RightBorderColor = (IndexedColors.Black.Index);
        //        style.BorderTop = (BorderStyle.Thin);
        //        style.TopBorderColor = (IndexedColors.Black.Index);

        //        IRow row = excelSheet.CreateRow(0);
        //        row.Height = 500;

        //        DateTime[] period = DateHelper.getStartEndDatePeriodForAttendance(client, DateTime.Now);
        //        int TotalDays = Convert.ToInt32((period[1] - period[0]).TotalDays) + 4;


        //        ICell CellHeader = row.CreateCell(0);
        //        CellHeader.SetCellValue(client.CLI_Name);
        //        CellHeader.CellStyle = styleHeader;
        //        CellUtil.SetAlignment(CellHeader, workbook, (short)HorizontalAlignment.Center);
        //        excelSheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, TotalDays - 7));

        //        ICell CellMonth = row.CreateCell(TotalDays - 6);
        //        string fullMonthName = DateTime.Now.ToString("MMM", CultureInfo.CreateSpecificCulture("IN"));
        //        CellMonth.SetCellValue(fullMonthName + "-" + DateTime.Now.ToString("yy"));
        //        CellMonth.CellStyle = styleHeader;
        //        CellUtil.SetAlignment(CellMonth, workbook, (short)HorizontalAlignment.Center);
        //        excelSheet.AddMergedRegion(new CellRangeAddress(0, 0, TotalDays - 6, TotalDays));

        //        row = excelSheet.CreateRow(1);
        //        row.HeightInPoints = ((5 * excelSheet.DefaultRowHeightInPoints));
        //        //excelSheet.AutoSizeColumn(1);


        //        ICell cell0 = row.CreateCell(0);
        //        cell0.SetCellValue("SR.NO");
        //        cell0.CellStyle = style;
        //        ICell cell1 = row.CreateCell(1);
        //        cell1.SetCellValue("EMP_Id");
        //        cell1.CellStyle = style;
        //        ICell cell2 = row.CreateCell(2);
        //        cell2.SetCellValue("Designation");
        //        cell2.CellStyle = style;
        //        ICell cell3 = row.CreateCell(3);
        //        cell3.SetCellValue("NAME");
        //        cell3.CellStyle = style;
        //        int i = 4;
        //        DateTime tmpDate = period[0];
        //        while (period[1] >= tmpDate)
        //        {
        //            ICell c = row.CreateCell(i);
        //            c.SetCellValue(tmpDate.Day);
        //            c.CellStyle = style;
        //            excelSheet.SetColumnWidth(i, 1000);
        //            tmpDate = tmpDate.AddDays(1);
        //            i++;
        //        }

        //        int rowCount = 2;
        //        int j = 1;
        //        foreach (var item in employees)
        //        {
        //            row = excelSheet.CreateRow(rowCount);
        //            row.CreateCell(0).SetCellValue(j);
        //            row.HeightInPoints = (float)(1.5 * excelSheet.DefaultRowHeightInPoints);
        //            row.CreateCell(1).SetCellValue(item.EMP_Id.ToString("D5"));
        //            row.CreateCell(2).SetCellValue(item.DES_.DES_Title);
        //            row.CreateCell(3).SetCellValue(item.EMP_.EMP_FirstName + " " + item.EMP_.EMP_MiddleName + " " + item.EMP_.EMP_SurName);

        //            excelSheet.SetColumnWidth(2, 6000);
        //            excelSheet.SetColumnWidth(3, 6000);

        //            int k = 4;
        //            DateTime tmp1Date = period[0];
        //            while (period[1] >= tmp1Date)
        //            {
        //                row.CreateCell(k);
        //                tmp1Date = tmp1Date.AddDays(1);
        //                k++;
        //            }
        //            rowCount++;
        //            j++;
        //        }

        //        workbook.Write(fs);
        //    }
        //    using (var stream = new FileStream(Path.Combine(newPath, fileName), FileMode.Open))
        //    {
        //        stream.CopyToAsync(memory);
        //    }
        //    memory.Position = 0;
        //    new FileInfo(Path.Combine(newPath, fileName)).Delete();
        //    return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        //}

        public FileResult BASIC_WithShifts()
        {
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            Clients client = clientsManager.GetClientById(ClientId);
            int totalEmployee = clientsManager.listClientsEmployees(ClientId).Count();
            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            string fileName = DateTime.Now.ToString("ddMMyyyyHHmm") + "_ClientId_" + ClientId + ".xlsx";
            string sFileName = fileName;
            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            var memory = new MemoryStream();
            using (var fs = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook;
                workbook = new XSSFWorkbook();
                ISheet excelSheet = workbook.CreateSheet("BASIC_WithShifts");

                IFont font = workbook.CreateFont();
                font.IsBold = true;
                font.FontHeightInPoints = ((short)16);
                font.FontName = ("Trebuchet MS");

                ICellStyle styleHeader = workbook.CreateCellStyle();
                styleHeader.FillBackgroundColor = HSSFColor.BlueGrey.Index;
                styleHeader.SetFont(font);
                ICellStyle style = workbook.CreateCellStyle();
                style.BorderBottom = (BorderStyle.Thin);
                style.BottomBorderColor = (IndexedColors.Black.Index);
                style.BorderLeft = (BorderStyle.Thin);
                style.LeftBorderColor = (IndexedColors.Black.Index);
                style.BorderRight = (BorderStyle.Thin);
                style.RightBorderColor = (IndexedColors.Black.Index);
                style.BorderTop = (BorderStyle.Thin);
                style.TopBorderColor = (IndexedColors.Black.Index);

                IRow row = excelSheet.CreateRow(0);
                row.Height = 500;

                DateTime startDate = DateTime.Now, endDate = DateTime.Now;

                if (client.CLI_Att_MonthReal == true)
                {
                    startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    endDate = startDate.AddMonths(1).AddDays(-1);
                }
                else if (client.CLI_Att_MonthReal == false)
                {
                    startDate = new DateTime(DateTime.Now.AddMonths(-1).Year, DateTime.Now.AddMonths(-1).Month, client.CLI_Att_Month_Start.Value);
                    endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, client.CLI_Att_Month_End.Value); ;
                }
                int TotalDays = Convert.ToInt32((endDate - startDate).TotalDays) + 6;


                string fullMonthName = DateTime.Now.ToString("MMMM", CultureInfo.CreateSpecificCulture("IN"));
                ICell CellHeader = row.CreateCell(0);
                CellHeader.SetCellValue(client.CLI_Name);
                CellHeader.CellStyle = styleHeader;
                CellUtil.SetAlignment(CellHeader, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, TotalDays - 2));

                ICell CellMonth = row.CreateCell(TotalDays - 1);
                CellMonth.SetCellValue(fullMonthName + "," + DateTime.Now.ToString("yyyy"));
                CellMonth.CellStyle = styleHeader;
                CellUtil.SetAlignment(CellMonth, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(0, 0, TotalDays - 1, TotalDays));

                row = excelSheet.CreateRow(1);

                ICell cell0 = row.CreateCell(0);
                cell0.SetCellValue("SR.NO");
                cell0.CellStyle = style;
                ICell cell1 = row.CreateCell(1);
                cell1.SetCellValue("EMP_Id");
                cell1.CellStyle = style;
                ICell cell2 = row.CreateCell(2);
                cell2.SetCellValue("Designation");
                cell2.CellStyle = style;
                ICell cell3 = row.CreateCell(3);
                cell3.SetCellValue("NAME");
                cell3.CellStyle = style;
                int i = 4;
                DateTime tmpDate = startDate;
                while (endDate >= tmpDate)
                {
                    ICell c = row.CreateCell(i);
                    c.SetCellValue(tmpDate.Day);
                    c.CellStyle = style;
                    excelSheet.AutoSizeColumn(i);
                    tmpDate = tmpDate.AddDays(1);
                    i++;
                }

                ICell cellx = row.CreateCell(i);
                cellx.SetCellValue("TOTAL DAYS");
                cellx.CellStyle = style;
                excelSheet.AutoSizeColumn(i);
                ICell celly = row.CreateCell(i + 1);
                celly.SetCellValue("FULL OT(HRS.)");
                celly.CellStyle = style;
                excelSheet.AutoSizeColumn(i+1);

                int rowCount = 2;
                int j = 1;
                foreach (var item in clientsManager.listClientsEmployees(ClientId))
                {
                    row = excelSheet.CreateRow(rowCount);
                    row.CreateCell(0).SetCellValue(j);
                    //row.CreateCell(1).SetCellValue(ProjectUtils.convertDigit(item.EMP_Id));
                    row.CreateCell(1).SetCellValue(item.EMP_Id.ToString("D5"));
                    row.CreateCell(2).SetCellValue(item.DES_.DES_Title);
                    // row.CreateCell(3).SetCellValue(item.EMP_.EMP_FullName);
                    row.CreateCell(3).SetCellValue(item.EMP_.EMP_FirstName + " " + item.EMP_.EMP_MiddleName + " " + item.EMP_.EMP_SurName);

                    excelSheet.SetColumnWidth(2, 6000);
                    excelSheet.SetColumnWidth(3, 6000);

                    excelSheet.AddMergedRegion(new CellRangeAddress(rowCount, rowCount + 1, 0, 0));
                    excelSheet.AddMergedRegion(new CellRangeAddress(rowCount, rowCount + 1, 1, 1));
                    excelSheet.AddMergedRegion(new CellRangeAddress(rowCount, rowCount + 1, 2, 2));
                    excelSheet.AddMergedRegion(new CellRangeAddress(rowCount, rowCount + 1, 3, 3));

                    int k = 4;
                    DateTime tmp1Date = startDate;
                    while (endDate >= tmp1Date)
                    {
                        ICell c = row.CreateCell(k);
                        tmp1Date = tmp1Date.AddDays(1);
                        k++;
                    }
                    excelSheet.AddMergedRegion(new CellRangeAddress(rowCount, rowCount + 1, k, k));
                    excelSheet.AddMergedRegion(new CellRangeAddress(rowCount, rowCount + 1, k + 1, k + 1));
                    rowCount = rowCount + 2;

                    j++;
                }

                workbook.Write(fs);
            }
            using (var stream = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open))
            {
                stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
        }


        public async Task<FileResult> GenerateExcelTemplate_TwoRow(DateTime month)
        {
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            Clients client = clientsManager.GetClientById(ClientId);
            IEnumerable<Clients_Employees> employees = clientsManager.listActiveClientsEmployees(ClientId, month);
            string newPath = ProjectUtils.GetTempFolderPath(_hostingEnvironment.WebRootPath);
            string fileName = "Template_" + month.ToString("ddMMyyyyHHmm") + "_" + client.CLI_Name + "_TwoRow.xlsx";
            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, fileName);
            FileInfo file = new FileInfo(Path.Combine(newPath, fileName));
            var memory = new MemoryStream();
            using (var fs = new FileStream(Path.Combine(newPath, fileName), FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook;
                workbook = new XSSFWorkbook();
                ISheet excelSheet = workbook.CreateSheet("Template");
                IFont font = workbook.CreateFont();
                font.IsBold = true;
                font.FontHeightInPoints = ((short)24);
                font.FontName = ("Cambria");

                ICellStyle styleHeader = workbook.CreateCellStyle();
                styleHeader.FillBackgroundColor = HSSFColor.BlueGrey.Index;
                styleHeader.SetFont(font);

                // Grey25Percent background
                ICellStyle style = workbook.CreateCellStyle();
                style.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                style.FillPattern = FillPattern.SolidForeground;
                style.FillBackgroundColor = IndexedColors.Grey25Percent.Index;

                // Style the cell with borders all around.
                style.BorderBottom = (BorderStyle.Thin);
                style.BottomBorderColor = (IndexedColors.Black.Index);
                style.BorderLeft = (BorderStyle.Thin);
                style.LeftBorderColor = (IndexedColors.Black.Index);
                style.BorderRight = (BorderStyle.Thin);
                style.RightBorderColor = (IndexedColors.Black.Index);
                style.BorderTop = (BorderStyle.Thin);
                style.TopBorderColor = (IndexedColors.Black.Index);

                IRow row = excelSheet.CreateRow(0);
                row.Height = 500;

                DateTime[] period = DateHelper.getStartEndDatePeriodForAttendance(client, month);
                int TotalDays = Convert.ToInt32((period[1] - period[0]).TotalDays) + 4;


                ICell CellHeader = row.CreateCell(0);
                CellHeader.SetCellValue(client.CLI_Name);
                CellHeader.CellStyle = styleHeader;
                CellUtil.SetAlignment(CellHeader, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, TotalDays - 7));

                ICell CellMonth = row.CreateCell(TotalDays - 6);
                string fullMonthName = month.ToString("MMM", CultureInfo.CreateSpecificCulture("IN"));
                CellMonth.SetCellValue(fullMonthName + "-" + month.ToString("yy"));
                CellMonth.CellStyle = styleHeader;
                CellUtil.SetAlignment(CellMonth, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(0, 0, TotalDays - 6, TotalDays));

                row = excelSheet.CreateRow(1);
                row.HeightInPoints = ((5 * excelSheet.DefaultRowHeightInPoints));
                //excelSheet.AutoSizeColumn(1);


                ICell cell0 = row.CreateCell(0);
                cell0.SetCellValue("SR.NO");
                cell0.CellStyle = style;
                ICell cell1 = row.CreateCell(1);
                cell1.SetCellValue("EMP_Id");
                cell1.CellStyle = style;
                ICell cell2 = row.CreateCell(2);
                cell2.SetCellValue("Designation");
                cell2.CellStyle = style;
                ICell cell3 = row.CreateCell(3);
                cell3.SetCellValue("NAME");
                cell3.CellStyle = style;
                int i = 4;
                DateTime tmpDate = period[0];
                while (period[1] >= tmpDate)
                {
                    ICell c = row.CreateCell(i);
                    c.SetCellValue(tmpDate.Day);
                    c.CellStyle = style;
                    excelSheet.AutoSizeColumn(i);
                    tmpDate = tmpDate.AddDays(1);
                    i++;
                }

                int rowCount = 2;
                int j = 1;
                foreach (var item in employees)
                {
                    row = excelSheet.CreateRow(rowCount);
                    row.CreateCell(0).SetCellValue(j);

                    //row.CreateCell(1).SetCellValue(ProjectUtils.convertDigit(item.EMP_Id));
                    row.CreateCell(1).SetCellValue(item.EMP_Id.ToString("D5"));
                    row.CreateCell(2).SetCellValue(item.DES_.DES_Title);
                    //row.CreateCell(3).SetCellValue(item.EMP_.EMP_FullName);
                    row.CreateCell(3).SetCellValue(item.EMP_.EMP_FirstName + " " + item.EMP_.EMP_MiddleName + " " + item.EMP_.EMP_SurName);

                    excelSheet.SetColumnWidth(2, 6000);
                    excelSheet.SetColumnWidth(3, 6000);


                    excelSheet.AddMergedRegion(new CellRangeAddress(rowCount, rowCount + 1, 0, 0));
                    excelSheet.AddMergedRegion(new CellRangeAddress(rowCount, rowCount + 1, 1, 1));
                    excelSheet.AddMergedRegion(new CellRangeAddress(rowCount, rowCount + 1, 2, 2));
                    excelSheet.AddMergedRegion(new CellRangeAddress(rowCount, rowCount + 1, 3, 3));

                    int k = 4;
                    DateTime tmp1Date = period[0];
                    while (period[1] >= tmp1Date)
                    {
                        ICell c = row.CreateCell(k);
                        tmp1Date = tmp1Date.AddDays(1);
                        k++;
                    }
                    rowCount = rowCount + 2;
                    j++;
                }

                workbook.Write(fs);
            }
            using (var stream = new FileStream(Path.Combine(newPath, fileName), FileMode.Open))
            {
                //stream.CopyToAsync(memory);
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            new FileInfo(Path.Combine(newPath, fileName)).Delete();
            return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        public async Task<FileResult> GenerateExcelTemplate_OneRow(DateTime month)
        {
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            Clients client = clientsManager.GetClientById(ClientId);
            //IEnumerable<Clients_Employees> employees = clientsManager.listClientsEmployees(ClientId);
            IEnumerable<Clients_Employees> employees = clientsManager.listActiveClientsEmployees(ClientId, month);
            string newPath = ProjectUtils.GetTempFolderPath(_hostingEnvironment.WebRootPath);
            string fileName = "Template_" + month.ToString("ddMMyyyyHHmm") + "_" + client.CLI_Name + "_OneRow.xlsx";
            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, fileName);
            FileInfo file = new FileInfo(Path.Combine(newPath, fileName));
            var memory = new MemoryStream();
            using (var fs = new FileStream(Path.Combine(newPath, fileName), FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook;
                workbook = new XSSFWorkbook();
                ISheet excelSheet = workbook.CreateSheet("Template");
                IFont font = workbook.CreateFont();
                font.IsBold = true;
                font.FontHeightInPoints = ((short)24);
                font.FontName = ("Cambria");

                ICellStyle styleHeader = workbook.CreateCellStyle();
                styleHeader.FillBackgroundColor = HSSFColor.BlueGrey.Index;
                styleHeader.SetFont(font);

                // Grey25Percent background
                ICellStyle style = workbook.CreateCellStyle();
                style.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                style.FillPattern = FillPattern.SolidForeground;
                style.FillBackgroundColor = IndexedColors.Grey25Percent.Index;

                // Style the cell with borders all around.
                style.BorderBottom = (BorderStyle.Thin);
                style.BottomBorderColor = (IndexedColors.Black.Index);
                style.BorderLeft = (BorderStyle.Thin);
                style.LeftBorderColor = (IndexedColors.Black.Index);
                style.BorderRight = (BorderStyle.Thin);
                style.RightBorderColor = (IndexedColors.Black.Index);
                style.BorderTop = (BorderStyle.Thin);
                style.TopBorderColor = (IndexedColors.Black.Index);

                IRow row = excelSheet.CreateRow(0);
                row.Height = 500;

                DateTime[] period = DateHelper.getStartEndDatePeriodForAttendance(client, month);
                int TotalDays = Convert.ToInt32((period[1] - period[0]).TotalDays) + 4;


                ICell CellHeader = row.CreateCell(0);
                CellHeader.SetCellValue(client.CLI_Name);
                CellHeader.CellStyle = styleHeader;
                CellUtil.SetAlignment(CellHeader, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, TotalDays - 7));

                ICell CellMonth = row.CreateCell(TotalDays - 6);
                string fullMonthName = month.ToString("MMM", CultureInfo.CreateSpecificCulture("IN"));
                CellMonth.SetCellValue(fullMonthName + "-" + month.ToString("yy"));
                CellMonth.CellStyle = styleHeader;
                CellUtil.SetAlignment(CellMonth, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(0, 0, TotalDays - 6, TotalDays));

                row = excelSheet.CreateRow(1);
                row.HeightInPoints = ((5 * excelSheet.DefaultRowHeightInPoints));
                //excelSheet.AutoSizeColumn(1);


                ICell cell0 = row.CreateCell(0);
                cell0.SetCellValue("SR.NO");
                cell0.CellStyle = style;
                ICell cell1 = row.CreateCell(1);
                cell1.SetCellValue("EMP_Id");
                cell1.CellStyle = style;
                ICell cell2 = row.CreateCell(2);
                cell2.SetCellValue("Designation");
                cell2.CellStyle = style;
                ICell cell3 = row.CreateCell(3);
                cell3.SetCellValue("NAME");
                cell3.CellStyle = style;
                int i = 4;
                DateTime tmpDate = period[0];
                while (period[1] >= tmpDate)
                {
                    ICell c = row.CreateCell(i);
                    c.SetCellValue(tmpDate.Day);
                    c.CellStyle = style;
                    excelSheet.SetColumnWidth(i, 1000);
                    tmpDate = tmpDate.AddDays(1);
                    i++;
                }

                int rowCount = 2;
                int j = 1;
                foreach (var item in employees)
                {
                    row = excelSheet.CreateRow(rowCount);
                    row.CreateCell(0).SetCellValue(j);
                    row.HeightInPoints = (float)(1.5 * excelSheet.DefaultRowHeightInPoints);
                    row.CreateCell(1).SetCellValue(item.EMP_Id.ToString("D5"));
                    row.CreateCell(2).SetCellValue(item.DES_.DES_Title);
                    row.CreateCell(3).SetCellValue(item.EMP_.EMP_FirstName + " " + item.EMP_.EMP_MiddleName + " " + item.EMP_.EMP_SurName);

                    excelSheet.SetColumnWidth(2, 6000);
                    excelSheet.SetColumnWidth(3, 6000);

                    int k = 4;
                    DateTime tmp1Date = period[0];
                    while (period[1] >= tmp1Date)
                    {
                        row.CreateCell(k);
                        tmp1Date = tmp1Date.AddDays(1);
                        k++;
                    }
                    rowCount++;
                    j++;
                }

                workbook.Write(fs);
            }
            using (var stream = new FileStream(Path.Combine(newPath, fileName), FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            new FileInfo(Path.Combine(newPath, fileName)).Delete();
            return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}