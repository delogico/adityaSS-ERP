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
            cvm.Listclients = clientsManager.listClients(IsActive,sessionUtils.GetLoggedFirmID());            
            ViewBag.List = cvm.Listclients;
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
        public ActionResult AddEditClients(int id=-1)
        {           
            AdminUserManager adminUserManager = new AdminUserManager(_context);
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            FirmsManager firmsManager = new FirmsManager(_context);
            ClientsViewModel cv = new ClientsViewModel();
            cv.clientsModel = new ClientsModel();
            
            cv.ParametersClientsModel = new ParametersClientsModel();
            cv.ParametersClientsModel.clientsModel = new ClientsModel();
            if (id > 0)
            {
                Clients clients = new Clients();
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
                IEnumerable<Client_Contacts> listClientContacts = clientsManager.GetClientContactsListById(id);
                cv.ListClientContact = listClientContacts;
                cv.requirements = ClientRequirementMapper.mapRequirements(clientsManager.GetClient_RequirementsofClient(id, true).ToList());
                cv.employees = ClientEmployeeMapper.mapEmployees(clientsManager.listClientsEmployees(id).ToList());
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
                Client_Contacts clientContacts = new Client_Contacts();
                Clients clients = new Clients();
                clients = clientsManager.GetClientById(ClientId);
                clientContactModel.ClientName = clients.CLI_Name;
                clients = null;
                if (ModelState.IsValid)
                {
                    if (id > 0)
                    {                                           
                        clientContacts = clientsManager.GetClientContactsById(id);
                        clientsManager = null;
                        clientContactModel.CON_Id = clientContacts.CON_Id;
                        clientContactModel.CLI_Id = clientContacts.CLI_Id;
                        clientContactModel.CON_FirstName = clientContacts.CON_FirstName;
                        clientContactModel.CON_SurName = clientContacts.CON_SurName;
                        clientContactModel.CON_Designation = clientContacts.CON_Designation;
                        clientContactModel.CON_Mobile = clientContacts.CON_Mobile;
                        clientContactModel.CON_Email = clientContacts.CON_Email;
                        clientContactModel.CON_isPrimary = clientContacts.CON_isPrimary;
                        clientContactModel.CON_RegisteredOn = ProjectUtils.DateNow();
                        SessionUtils sessionUtils = new SessionUtils(Request, Response);
                        clientContactModel.ADM_Id_RegisteredBy = sessionUtils.GetLoggedAdminID();
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
                    Client_Contacts clientContacts = new Client_Contacts();
                    clientContacts.CLI_Id = ClientId;
                    clientContacts.CON_Id = clientContactModel.CON_Id;
                    clientContacts.CON_FirstName = clientContactModel.CON_FirstName;
                    clientContacts.CON_SurName = clientContactModel.CON_SurName;
                    clientContacts.CON_Designation = clientContactModel.CON_Designation;
                    clientContacts.CON_Mobile = clientContactModel.CON_Mobile;
                    clientContacts.CON_Email = clientContactModel.CON_Email;
                    clientContacts.CON_isPrimary = clientContactModel.CON_isPrimary;
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
        public ActionResult AddEditRequirement(int CLI_Id, int CRI_Id =-1)
        {
            DesignationManager designationManager = new DesignationManager(_context);
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            ClientRequirementVM clientRequirement = new ClientRequirementVM();
            Clients client = clientsManager.GetClientById(CLI_Id);
            ViewBag.client = client;
            if (CRI_Id > 0)
            {
                clientRequirement=ClientRequirementMapper.mapMe(clientsManager.GetRequirementsById(CRI_Id));
            }
            else
            {
                ViewBag.Designation = designationManager.getRemainingDesignationsList(CLI_Id);
                clientRequirement.CRI_Id = -1;
                clientRequirement.CLI_Id = CLI_Id;
                clientRequirement.CRI_Active = true;
            }
            return View(clientRequirement);
        }

        [HttpPost]
        public ActionResult AddEditRequirement(ClientRequirementVM clientRequirementVM)
        {
            string res = string.Empty;
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            if (ModelState.IsValid)
            {
                Client_Requirements cr = new Client_Requirements();
                cr.CRI_Id = clientRequirementVM.CRI_Id;
                cr.CLI_Id = clientRequirementVM.CLI_Id;
                cr.DES_Id = clientRequirementVM.DES_Id;
                cr.CRI_Total = clientRequirementVM.CRI_Total;
                cr.CRI_Basic = clientRequirementVM.CRI_Basic;
                cr.CRI_DA = clientRequirementVM.CRI_DA;
                cr.CRI_BasicDA = clientRequirementVM.CRI_BasicDA;
                cr.CRI_HRA_Fixed = clientRequirementVM.CRI_HRA_Fixed;
                cr.CRI_HRA_Percentage = clientRequirementVM.CRI_HRA_Percentage;
                cr.CRI_Allowance_UpKeep = clientRequirementVM.CRI_Allowance_UpKeep;
                cr.CRI_Allowance_Grade = clientRequirementVM.CRI_Allowance_Grade;
                cr.CRI_Allowance_Conveyance = clientRequirementVM.CRI_Allowance_Conveyance;
                cr.CRI_Allowance_Attention = clientRequirementVM.CRI_Allowance_Attention;
                cr.CRI_PF_Percentage = clientRequirementVM.CRI_PF_Percentage;
                cr.CRI_ESIC_Percentage = clientRequirementVM.CRI_ESIC_Percentage;
                cr.CRI_ESIC_Area = clientRequirementVM.CRI_ESIC_Area;
                cr.CRI_OT_Rate = clientRequirementVM.CRI_OT_Rate;
                cr.CRI_OT_MultipleTimes = clientRequirementVM.CRI_OT_MultipleTimes;
                cr.CRI_WageCalculationOnWeeklyOffPlus = clientRequirementVM.CRI_WageCalculationOnWeeklyOffPlus;
                cr.CRI_Active = clientRequirementVM.CRI_Active;
                res =clientsManager.AddEditRequirement(cr, sessionUtils.GetLoggedAdminID());
            }
            if (res != "")
            {
                TempData["message"] = "Error In Client Requirement! Please Check";
                return View();                
            }
            return RedirectToAction("AddEditClients", new { id = ClientId, tab = "ClientRequirement" });
        }

        public ActionResult HistoryRequirement(int DES_Id,int CLI_Id)
        {
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
        public ActionResult AddEmployee(int CLI_Id)
        {
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            ClientEmployeeVM cvm = new ClientEmployeeVM();
            DesignationManager designationManager = new DesignationManager(_context);
            IEnumerable<AssignEmployeeVM> listDesignations = designationManager.getDesignationsListInVM(CLI_Id);
            IEnumerable<EmployeeVM> listEmployee = EmployeesMapper.MapEmployees(clientsManager.getEmployeeList(CLI_Id).ToList());
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
                SessionUtils sessionUtils = new SessionUtils(Request, Response);
                res = clientsManager.ClientEmployee(clientsEmployees, sessionUtils.GetLoggedAdminID());
                if (res != string.Empty)
                {
                    TempData["message"] = "ClientEmployee data can not Inserted";
                }
            }
            return RedirectToAction("AddEditClients", new { id = cvm.CLI_Id, tab = "ClientEmployee" });
        }
        public ActionResult DeleteClientEmployee(int CLE_Id = -1)
        {
            ClientsViewModel clientsViewModel = new ClientsViewModel();
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            if (ModelState.IsValid)
            {
                if (CLE_Id > 0)
                {
                    string res = clientsManager.deleteClientEmployee(CLE_Id);
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
                int TotalDays = Convert.ToInt32((endDate - startDate).TotalDays) + 8;


                string fullMonthName = DateTime.Now.ToString("MMMM", CultureInfo.CreateSpecificCulture("IN"));
                ICell CellHeader = row.CreateCell(0);
                CellHeader.SetCellValue(client.CLI_Name);
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

                    //row.CreateCell(1).SetCellValue(ProjectUtils.convertDigit(item.EMP_Id));
                    row.CreateCell(1).SetCellValue(item.EMP_Id.ToString("D5"));
                    row.CreateCell(2).SetCellValue(item.CLI_.CLI_Id);
                    //row.CreateCell(3).SetCellValue(item.EMP_.EMP_FullName);
                    row.CreateCell(3).SetCellValue(item.EMP_.EMP_FirstName+""+ item.EMP_.EMP_MiddleName+""+ item.EMP_.EMP_SurName);

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
                    row.CreateCell(2).SetCellValue(item.CLI_.CLI_Id);
                    // row.CreateCell(3).SetCellValue(item.EMP_.EMP_FullName);
                    row.CreateCell(3).SetCellValue(item.EMP_.EMP_FirstName + "" + item.EMP_.EMP_MiddleName + "" + item.EMP_.EMP_SurName);

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