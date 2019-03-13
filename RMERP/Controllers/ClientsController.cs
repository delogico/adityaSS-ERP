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
using RMERP.DAL.App_Code;
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
        public IActionResult Index(bool IsActive=true)
        {
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            ClientsViewModel cvm = new ClientsViewModel();
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            cvm.Listclients = clientsManager.listClients(sessionUtils.GetLoggedFirmID(), IsActive);            
            ViewBag.List = cvm.Listclients;
            return View(cvm);
        }
        [HttpGet]
        public IActionResult GetClientList(bool IsActive = true,string Client="")
        {
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            SessionUtils sessionUtils = new SessionUtils(Request,Response);
            IEnumerable<Clients> listClient = clientsManager.listClients(sessionUtils.GetLoggedFirmID(), IsActive, Client);
            ViewBag.ActiveClient = "IsActive";
            return PartialView("_ClientList", listClient);
        }
        [HttpGet]
        public ActionResult AddEditClients(int id=-1)
        {           
            AdminUserManager adminUserManager = new AdminUserManager(_context);
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            FirmsManager firmsManager = new FirmsManager(_context);
            ClientsViewModel cv = new ClientsViewModel();
            cv.clientsModel = new ClientsModel();
            cv.ParametersClientsModel = new ParametersClientsModel();
            if (id > 0)
            {
                Clients clients = new Clients();
                clients=clientsManager.GetClientById(id);
                ClientId = id;
                cv.clientsModel.CliId= clients.CliId;
                cv.clientsModel.CliIsActive = clients.CliIsActive;
                cv.clientsModel.FrmId = clients.FrmId;
                cv.clientsModel.CliName = clients.CliName;
                cv.clientsModel.CliInternationalDomestic = clients.CliInternationalDomestic;
                cv.clientsModel.CliAddress = clients.CliAddress;
                cv.clientsModel.CityId = clients.CityId;
                cv.clientsModel.CliPincode = clients.CliPincode;
                cv.clientsModel.CliPhone = clients.CliPhone;
                cv.clientsModel.CliFax = clients.CliFax;
                cv.clientsModel.CliEmail = clients.CliEmail;
                cv.clientsModel.CliEmail2 = clients.CliEmail2;

                cv.ParametersClientsModel.CliGstNumber = clients.CliGstNumber;
                cv.ParametersClientsModel.CliGstRate = clients.CliGstRate;
                cv.ParametersClientsModel.CliHsnCode = clients.CliHsnCode;
                cv.ParametersClientsModel.CliTdsRate = clients.CliTdsRate;
                cv.ParametersClientsModel.CliAttMonthStart = clients.CliAttMonthStart;
                cv.ParametersClientsModel.CliAttMonthEnd = clients.CliAttMonthEnd;
                cv.ParametersClientsModel.CliAttMonthReal = clients.CliAttMonthReal;

                cv.clientsModel.CliGstNumber = "";
                cv.clientsModel.CliGstRate = 0;
                cv.clientsModel.CliHsnCode = "";
                cv.clientsModel.CliTdsRate = 0;

                cv.clientsModel.AdmIdRegisterBy = clients.AdmIdRegisterBy;
                cv.clientsModel.CliRegisteredOn = clients.CliRegisteredOn;
                cv.clientsModel.CliLogoImage = clients.CliLogo;
                IEnumerable<ClientContacts> listClientContacts = clientsManager.GetClientContactsListById(id);
                cv.ListClientRequirements = clientsManager.GetClientRequirementsListByClientId(id,true);
                cv.ListClientContact = listClientContacts;
                IEnumerable<ClientsEmployees> listClientsEmployees= clientsManager.listClientsEmployees(ClientId);
                cv.ClientsEmployeesList = listClientsEmployees;
            }
            IEnumerable<Firms> listFirms = new List<Firms>();
            List<Cities> listCity = new List<Cities>();          
           
            listFirms = firmsManager.getFirmList();
            listCity = adminUserManager.getCityList();
            ViewBag.firmList = listFirms;
            ViewBag.cityList = listCity;
           
            return View(cv);
        }
        [HttpPost]
        public ActionResult AddEditClients(ClientsViewModel cv)
        {            
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            int clientID = 0;
            if (ModelState.IsValid)
            {
                SessionUtils sessionUtils = new SessionUtils(Request, Response);
                cv.clientsModel.AdmIdRegisterBy = sessionUtils.GetLoggedAdminID();
                Clients clients = new Clients();
                IFormFile file = cv.clientsModel.CliLogo;
                clients.CliId = cv.clientsModel.CliId;
                clients.FrmId = cv.clientsModel.FrmId;
                clients.CliName = cv.clientsModel.CliName;
                clients.CliInternationalDomestic = cv.clientsModel.CliInternationalDomestic;
                clients.CliAddress = cv.clientsModel.CliAddress;
                clients.CityId = cv.clientsModel.CityId;
                clients.CliPincode = cv.clientsModel.CliPincode;
                clients.CliPhone = cv.clientsModel.CliPhone;
                clients.CliFax = cv.clientsModel.CliFax;
                clients.CliEmail = cv.clientsModel.CliEmail;
                clients.CliEmail2 = cv.clientsModel.CliEmail2;

                clients.CliGstNumber = "";
                clients.CliGstRate = 0;
                clients.CliHsnCode = "";
                clients.CliTdsRate =0;
                clients.AdmIdRegisterBy = cv.clientsModel.AdmIdRegisterBy;
                clients.CliRegisteredOn = cv.clientsModel.CliRegisteredOn;

                if (cv.clientsModel.CliLogo != null)
                {
                    clients.CliLogo = cv.clientsModel.CliLogo.FileName;
                }
                else
                {
                    clients.CliLogo = cv.clientsModel.CliLogoImage;
                }
                var tuple = clientsManager.saveAddEditClients(file, clients);
                if (tuple.Item1 != "")
                {
                    TempData["message"] = "Client data can not Inserted";
                }
                else
                {
                    clientID = tuple.Item2;
                    TempData["message"] = "Successfull Done!";
                }
            }            
            return RedirectToAction("AddEditClients",new { id= clientID });
        }
        [HttpGet]
        public ActionResult AddEditContacts(int id=-1)
        {
            ClientContactModel clientContactModel = new ClientContactModel();
            if (ClientId>0)
            {
                ClientsManager clientsManager = new ClientsManager(_context, Configuration);
                ClientContacts clientContacts = new ClientContacts();
                Clients clients = new Clients();
                clients = clientsManager.GetClientById(ClientId);
                clientContactModel.ClientName = clients.CliName;
                clients = null;
                if (ModelState.IsValid)
                {
                    if (id > 0)
                    {                                           
                        clientContacts = clientsManager.GetClientContactsById(id);
                        clientsManager = null;
                        clientContactModel.ConId = clientContacts.ConId;
                        clientContactModel.CliId = clientContacts.CliId;
                        clientContactModel.ConFirstName = clientContacts.ConFirstName;
                        clientContactModel.ConSurName = clientContacts.ConSurName;
                        clientContactModel.ConDesignation = clientContacts.ConDesignation;
                        clientContactModel.ConMobile = clientContacts.ConMobile;
                        clientContactModel.ConEmail = clientContacts.ConEmail;
                        clientContactModel.ConIsPrimary = clientContacts.ConIsPrimary;
                        clientContactModel.ConRegisteredOn = ProjectUtils.DateNow();
                        SessionUtils sessionUtils = new SessionUtils(Request, Response);
                        clientContactModel.AdmIdRegisteredBy = sessionUtils.GetLoggedAdminID();
                        clientContacts = null;
                    }
                }
            }                    
            return View(clientContactModel);
        }
        [HttpPost]
        public ActionResult AddEditContacts(ClientContactModel clientContactModel)
        {
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            if (ModelState.IsValid)
            {
                if (ClientId > 0)
                {
                    ClientContacts clientContacts = new ClientContacts();
                    clientContacts.CliId = ClientId;
                    clientContacts.ConId = clientContactModel.ConId;
                    clientContacts.ConFirstName = clientContactModel.ConFirstName;
                    clientContacts.ConSurName = clientContactModel.ConSurName;
                    clientContacts.ConDesignation = clientContactModel.ConDesignation;
                    clientContacts.ConMobile = clientContactModel.ConMobile;
                    clientContacts.ConEmail = clientContactModel.ConEmail;
                    clientContacts.ConIsPrimary = clientContactModel.ConIsPrimary;
                    clientContacts.ConRegisteredOn = ProjectUtils.DateNow();
                    SessionUtils sessionUtils = new SessionUtils(Request, Response);
                    clientContacts.AdmIdRegisteredBy = sessionUtils.GetLoggedAdminID();
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
        public ActionResult AddEditRequirement(ClientsViewModel cvm,int criId =-1,string tab="")
        {
            DesignationManager designationManager = new DesignationManager(_context);
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            ClientRequirementsModel clientRequirementsModel = new ClientRequirementsModel();
            ViewBag.Designation = designationManager.getDesignationsList();
            designationManager = null;
            Clients clients = new Clients();
           
            clients = clientsManager.GetClientById(ClientId);
            clientRequirementsModel.CliName = clients.CliName;
            clientRequirementsModel.CliId = clients.CliId;
           clientRequirementsModel.CriActive = true;
            clients = null;
            if (criId > 0)
            {
                if (ClientId > 0)
                {                                                      
                    ClientRequirements clientRequirements = new ClientRequirements();
                    clientRequirements=clientsManager.GetRequirementsById(criId);
                    clientRequirementsModel.CriId = clientRequirements.CriId;
                    clientRequirementsModel.CliId = clientRequirements.CliId;
                    clientRequirementsModel.DesId = clientRequirements.DesId;
                    clientRequirementsModel.CriBasic = clientRequirements.CriBasic;
                    clientRequirementsModel.CriDa = clientRequirements.CriDa;
                    clientRequirementsModel.CriBasicDa = clientRequirements.CriBasicDa;
                    clientRequirementsModel.CriHraFixed = clientRequirements.CriHraFixed;
                    clientRequirementsModel.CriHraPercentage = clientRequirements.CriHraPercentage;
                    clientRequirementsModel.CriAllowanceUpKeep = clientRequirements.CriAllowanceUpKeep;
                    clientRequirementsModel.CriAllowanceGrade = clientRequirements.CriAllowanceGrade;
                    clientRequirementsModel.CriAllowanceConveyance = clientRequirements.CriAllowanceConveyance;
                    clientRequirementsModel.CriAllowanceAttention = clientRequirements.CriAllowanceAttention;
                    clientRequirementsModel.CriPfPercentage = clientRequirements.CriPfPercentage;
                    clientRequirementsModel.CriEsicPercentage = clientRequirements.CriEsicPercentage;
                    clientRequirementsModel.CriEsicArea = clientRequirements.CriEsicArea;
                    clientRequirementsModel.CriOtRate = clientRequirements.CriOtRate;
                    clientRequirementsModel.CriOtMultipleTimes = clientRequirements.CriOtMultipleTimes;
                    clientRequirementsModel.CriWageCalculationOnWeeklyOffPlus = clientRequirements.CriWageCalculationOnWeeklyOffPlus;
                    clientRequirementsModel.CriActive = clientRequirements.CriActive;
                    if (!string.IsNullOrEmpty(tab))
                    {
                        clientRequirementsModel.tabName = tab;
                    }
                    clientsManager = null;
                }
            }            
            return View(clientRequirementsModel);
        }
        [HttpPost]
        public ActionResult AddEditRequirement(ClientRequirementsModel clientRequirementsModel)
        {
            string res = string.Empty;
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            if (ModelState.IsValid)
            {
                ClientRequirements cr = new ClientRequirements();
                cr.CriId = clientRequirementsModel.CriId;
                cr.CliId = clientRequirementsModel.CliId;
                cr.DesId = clientRequirementsModel.DesId;
                cr.CriBasic = clientRequirementsModel.CriBasic;
                cr.CriDa = clientRequirementsModel.CriDa;
                cr.CriBasicDa = clientRequirementsModel.CriBasicDa;
                cr.CriHraFixed = clientRequirementsModel.CriHraFixed;
                cr.CriHraPercentage = clientRequirementsModel.CriHraPercentage;
                cr.CriAllowanceUpKeep = clientRequirementsModel.CriAllowanceUpKeep;
                cr.CriAllowanceGrade = clientRequirementsModel.CriAllowanceGrade;
                cr.CriAllowanceConveyance = clientRequirementsModel.CriAllowanceConveyance;
                cr.CriAllowanceAttention = clientRequirementsModel.CriAllowanceAttention;
                cr.CriPfPercentage = clientRequirementsModel.CriPfPercentage;
                cr.CriEsicPercentage = clientRequirementsModel.CriEsicPercentage;
                cr.CriEsicArea = clientRequirementsModel.CriEsicArea;
                cr.CriOtRate = clientRequirementsModel.CriOtRate;
                cr.CriOtMultipleTimes = clientRequirementsModel.CriOtMultipleTimes;
                cr.CriWageCalculationOnWeeklyOffPlus = clientRequirementsModel.CriWageCalculationOnWeeklyOffPlus;
                cr.CriActive = clientRequirementsModel.CriActive;
                SessionUtils sessionUtils = new SessionUtils(Request, Response);
                cr.AdmIdInactivatedBy =sessionUtils.GetLoggedAdminID();

                res =clientsManager.AddEditRequirement(cr);
            }
            if (res != "")
            {
                TempData["message"] = "Error In Client Requirement! Please Check";
                return View();                
            }
            return RedirectToAction("AddEditClients", new { id = ClientId, tab = "ClientRequirement" });
        }

        public ActionResult HistoryRequirement(int desId = -1,int cliId=-1)
        {
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            DesignationManager designationManager = new DesignationManager(_context);
            ClientRequirementsViewModel crvm = new ClientRequirementsViewModel();
            crvm.ClientRequirementsModel = new ClientRequirementsModel();
            crvm.ClientRequirementsList = clientsManager.GetClientRequirementsList(desId, cliId, false);
            crvm.ClientRequirementsModel.DesTitle = designationManager.GetDesignationsById(desId);
            crvm.ClientRequirementsModel.CliName = clientsManager.GetClientById(ClientId).CliName;
            return View(crvm);
        }

        [HttpPost]
        public ActionResult Parameters(ClientsViewModel cvm)
        {
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            if (cvm.clientsModel.CliId > 0)
            {
                Clients clients = new Clients();
                clients.CliId = cvm.clientsModel.CliId;
                clients.CliGstNumber = cvm.ParametersClientsModel.CliGstNumber;
                clients.CliGstRate = cvm.ParametersClientsModel.CliGstRate;
                clients.CliTdsRate = cvm.ParametersClientsModel.CliTdsRate;
                clients.CliHsnCode = cvm.ParametersClientsModel.CliHsnCode;

                if (cvm.ParametersClientsModel.CliAttMonthReal ==true)
                {
                    clients.CliAttMonthReal = true;
                    clients.CliAttMonthStart = null;
                    clients.CliAttMonthEnd = null;
                }
                else
                {
                    clients.CliAttMonthReal = false;
                    clients.CliAttMonthStart = cvm.ParametersClientsModel.CliAttMonthStart;
                    clients.CliAttMonthEnd = cvm.ParametersClientsModel.CliAttMonthEnd;
                }
                clientsManager.UpdateParameters(clients);
            }
            return RedirectToAction("AddEditClients", new { id = ClientId, tab = "Parameters" });
        }

        [HttpGet]
        public ActionResult ClientEmployee(int CleId = -1)
        {
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            ClientsEmployeesViewModel cvm = new ClientsEmployeesViewModel();
            DesignationManager designationManager = new DesignationManager(_context);
            IEnumerable<Designations> listDesignations = designationManager.getDesignationsListByClientID(ClientId);

            IEnumerable<Employees> listEmployee = clientsManager.getEmployeeList(ClientId);
            ViewBag.EmployeeList = listEmployee;

            ViewBag.designationList = listDesignations;
            cvm.CliId = ClientId;
            ViewBag.ClientName = clientsManager.GetClientById(ClientId).CliName;
            //if (CleId > 0)
            //{
            //    ClientsEmployees ce = new ClientsEmployees();
            //    ce=clientsManager.ClientEmployeeById(CleId);
            //    cvm.DesId = ce.DesId;
            //    cvm.EmpId = ce.EmpId;
            //}            
            return View(cvm);
        }
        [HttpPost]
        public ActionResult ClientEmployee(ClientsEmployeesViewModel cvm)
        {
            string res = string.Empty;
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            if (ModelState.IsValid)
            {
                ClientsEmployees clientsEmployees = new ClientsEmployees();
                clientsEmployees.CleId = cvm.CleId;
                clientsEmployees.CliId = cvm.CliId;
                clientsEmployees.DesId = cvm.DesId;
                clientsEmployees.EmpId = cvm.EmpId;
                SessionUtils sessionUtils = new SessionUtils(Request, Response);
                res = clientsManager.ClientEmployee(clientsEmployees, sessionUtils.GetLoggedAdminID());
                if (res != string.Empty)
                {
                    TempData["message"] = "ClientEmployee data can not Inserted";
                }
            }
            return RedirectToAction("AddEditClients", new { id = ClientId, tab = "ClientEmployee" });
        }
        public ActionResult DeleteClientEmployee(int CleId = -1)
        {
            ClientsViewModel clientsViewModel = new ClientsViewModel();
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            if (ModelState.IsValid)
            {
                if (CleId > 0)
                {
                    string res = clientsManager.deleteClientEmployee(CleId);
                    if (res != string.Empty)
                    {
                        TempData["message"] = "Employee can not deleted";
                    }
                }
            }
            return RedirectToAction("AddEditClients", new { id = ClientId, tab = "ClientEmployee" });
        }

        public FileResult BASIC_WithoutShifts()
        {
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            Clients client = clientsManager.GetClientById(ClientId);
            int totalEmployee = clientsManager.listClientsEmployees(ClientId).Count();
            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            string fileName = DateTime.Now.ToString("ddMMyyyyHHmm") + "_ClientId" + ClientId+".xlsx";
            string sFileName = fileName;
            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            var memory = new MemoryStream();
            using (var fs = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook;
                workbook = new XSSFWorkbook();
                ISheet excelSheet = workbook.CreateSheet("BASIC_WithoutShifts");                

                
                // excelSheet.AddMergedRegion(new CellRangeAddress(0, 0, 35, 40));
                IFont font = workbook.CreateFont();
                font.IsBold = true;
                font.FontHeightInPoints=((short)24);
                font.FontName=("Cambria");

                ICellStyle styleHeader = workbook.CreateCellStyle();
                styleHeader.FillBackgroundColor = HSSFColor.BlueGrey.Index;
                styleHeader.SetFont(font);

                // Grey25Percent background
                ICellStyle style = workbook.CreateCellStyle();
                style.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                style.FillPattern = FillPattern.SolidForeground;
                style.FillBackgroundColor = IndexedColors.Grey25Percent.Index;

                // Style the cell with borders all around.
                style.BorderBottom=(BorderStyle.Thin);
                style.BottomBorderColor=(IndexedColors.Black.Index);
                style.BorderLeft=(BorderStyle.Thin);
                style.LeftBorderColor=(IndexedColors.Black.Index);
                style.BorderRight=(BorderStyle.Thin);
                style.RightBorderColor=(IndexedColors.Black.Index);
                style.BorderTop=(BorderStyle.Thin);
                style.TopBorderColor= (IndexedColors.Black.Index);               

                IRow row = excelSheet.CreateRow(0);
                row.Height = 500;

                DateTime startDate = DateTime.Now, endDate = DateTime.Now;

                if (client.CliAttMonthReal == true)
                {
                    startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    endDate = startDate.AddMonths(1).AddDays(-1);
                }
                else if (client.CliAttMonthReal == false)
                {
                    startDate = new DateTime(DateTime.Now.AddMonths(-1).Year, DateTime.Now.AddMonths(-1).Month, client.CliAttMonthStart.Value);
                    endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, client.CliAttMonthEnd.Value); ;
                }
                int TotalDays = Convert.ToInt32((endDate - startDate).TotalDays) + 8;


                string fullMonthName = DateTime.Now.ToString("MMMM", CultureInfo.CreateSpecificCulture("IN"));
                ICell CellHeader = row.CreateCell(0);
                CellHeader.SetCellValue(client.CliName);
                CellHeader.CellStyle = styleHeader;
                CellUtil.SetAlignment(CellHeader, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, TotalDays - 4));

                ICell CellMonth = row.CreateCell(TotalDays - 3);
                CellMonth.SetCellValue(fullMonthName + "," + DateTime.Now.ToString("yyyy"));
                CellMonth.CellStyle = styleHeader;
                CellUtil.SetAlignment(CellMonth, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(0, 0, TotalDays - 3, TotalDays));

                row = excelSheet.CreateRow(1);
                row.HeightInPoints=((5 * excelSheet.DefaultRowHeightInPoints));
                //excelSheet.AutoSizeColumn(1);
            

                ICell cell0 = row.CreateCell(0);
                cell0.SetCellValue("SR.NO");
                cell0.CellStyle= style;
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
                while(endDate >= tmpDate)
                {
                    ICell c = row.CreateCell(i);                    
                    c.SetCellValue(tmpDate.Day);
                    c.CellStyle = style;
                    excelSheet.AutoSizeColumn(i);
                    tmpDate = tmpDate.AddDays(1);
                    i++;
                }
                
                ICell cellx = row.CreateCell(i);
                cellx.SetCellValue("PRE.DAYS");
                cellx.CellStyle = style;
                ICell celly = row.CreateCell(i + 1);
                celly.SetCellValue("PH");
                celly.CellStyle = style;
                ICell cellz = row.CreateCell(i + 2);
                cellz.SetCellValue("EXTRA WORK DAYS");
                excelSheet.AutoSizeColumn(i+2);
                cellz.CellStyle = style;
                ICell cellw = row.CreateCell(i + 3);
                cellw.SetCellValue("TOTAL DAYS");
                excelSheet.AutoSizeColumn(i + 3);
                cellw.CellStyle = style;


                int rowCount = 2;
                int j = 1 ;
                foreach (var item in clientsManager.listClientsEmployees(ClientId))
                {
                    row = excelSheet.CreateRow(rowCount);
                    row.CreateCell(0).SetCellValue(j);                 

                    row.CreateCell(1).SetCellValue(ProjectUtils.convertDigit(item.EmpId));
                    row.CreateCell(2).SetCellValue(item.Des.DesTitle);
                    row.CreateCell(3).SetCellValue(item.Emp.EmpFirstName+" "+ item.Emp.EmpMiddleName+" "+ item.Emp.EmpSurName);

                    excelSheet.SetColumnWidth(2, 6000);
                    excelSheet.SetColumnWidth(3, 6000);
                   
                    excelSheet.AddMergedRegion(new CellRangeAddress(rowCount, rowCount+1, 0, 0));
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
                    excelSheet.AddMergedRegion(new CellRangeAddress(rowCount, rowCount + 1, k+1, k+1));
                    excelSheet.AddMergedRegion(new CellRangeAddress(rowCount, rowCount + 1, k + 2, k + 2));
                    excelSheet.AddMergedRegion(new CellRangeAddress(rowCount, rowCount + 1, k + 3, k + 3));
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

                if (client.CliAttMonthReal == true)
                {
                    startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    endDate = startDate.AddMonths(1).AddDays(-1);
                }
                else if (client.CliAttMonthReal == false)
                {
                    startDate = new DateTime(DateTime.Now.AddMonths(-1).Year, DateTime.Now.AddMonths(-1).Month, client.CliAttMonthStart.Value);
                    endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, client.CliAttMonthEnd.Value); ;
                }
                int TotalDays = Convert.ToInt32((endDate - startDate).TotalDays) + 6;


                string fullMonthName = DateTime.Now.ToString("MMMM", CultureInfo.CreateSpecificCulture("IN"));
                ICell CellHeader = row.CreateCell(0);
                CellHeader.SetCellValue(client.CliName);
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
                    row.CreateCell(1).SetCellValue(ProjectUtils.convertDigit(item.EmpId));
                    row.CreateCell(2).SetCellValue(item.Des.DesTitle);
                    row.CreateCell(3).SetCellValue(item.Emp.EmpFirstName + " " + item.Emp.EmpMiddleName + " " + item.Emp.EmpSurName);

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
    }
}