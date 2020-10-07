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
using Microsoft.AspNetCore.Hosting;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.HSSF.Util;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Drawing;
using static RMERP.DAL.Helpers.ProjectUtils;

namespace RMERP.Controllers
{
    [Authorize]
    public class ClientsController : Controller
    {
        private readonly RMERPContext _context;
        public IConfiguration Configuration;
       // public static int ClientId;
       // public static bool IsActive;
        private IHostingEnvironment _hostingEnvironment;

        public ClientsController(RMERPContext context, IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            Configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public IActionResult Index(bool IsActive = true)
        {           
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            ClientsViewModel cvm = new ClientsViewModel();
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            cvm.Listclients = clientsManager.listClients(IsActive, sessionUtils.GetLoggedFirmID());
            ViewBag.List = cvm.Listclients;
            ViewBag.clients = clientsManager.GetTotalClient();
            if (sessionUtils.GetLoggedFirmID().HasValue)
            {
                int FRM_Id = sessionUtils.GetLoggedFirmID().Value;
                ViewBag.totalClients = clientsManager.GetTotalClient(FRM_Id);
            }
            else
            {
                ViewBag.totalClients = clientsManager.GetTotalClient();
            }
            return View(cvm);
        }

        [HttpGet]
        public IActionResult GetClientList(bool IsActive = true, string Client = "")
        {
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            IEnumerable<Clients> listClient = clientsManager.listClients(IsActive, sessionUtils.GetLoggedFirmID(), Client);
            ViewBag.ActiveClient = "IsActive";
            return PartialView("_ClientList", listClient);
        }

        [HttpGet]
        public ActionResult AddEditClients(int id = -1)
        {           
            int FRM_Id = 0;
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            if (sessionUtils.GetLoggedFirmID().HasValue)
            {
                FRM_Id = sessionUtils.GetLoggedFirmID().Value;
            }
            EmployeeManager employeeManager = new EmployeeManager(_context);
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
            cv.clientsModel.STA_Id = 12;
            ViewBag.states = employeeManager.GetStates();
            IEnumerable<Firms> listFirms = new List<Firms>();
            List<Cities> listCity = new List<Cities>();

            listFirms = firmsManager.getFirmList();
            listCity = adminUserManager.getCityList();
            ViewBag.firmList = listFirms;
            ViewBag.cityList = listCity;

            cv.attendanceParameter = new AttendanceParameterVM();
            cv.attendanceParameter.ATP_Att_MonthReal = true;
            cv.attendanceParameter.ATP_RegisteredOn = DateNow();
            
            cv.attendanceParameters = null;
            if (id > 0)
            {
                clients = clientsManager.GetClientById(id);               
                cv.clientsModel.CLI_Id = clients.CLI_Id;
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

                if(clients.CLI_InActivatedOn!=null)
                    cv.clientsModel.CLI_InActivatedOn = clients.CLI_InActivatedOn.Value;

                cv.ParametersClientsModel.clientsModel.CLI_GST_Number = clients.CLI_GST_Number;
                cv.ParametersClientsModel.clientsModel.CLI_GST_Rate = clients.CLI_GST_Rate;
                cv.ParametersClientsModel.clientsModel.CLI_HSN_Code = clients.CLI_HSN_Code;
                cv.ParametersClientsModel.clientsModel.CLI_TDS_Rate = clients.CLI_TDS_Rate;

                //cv.ParametersClientsModel.CLI_Att_Month_Start = clients.CLI_Att_Month_Start;
                //cv.ParametersClientsModel.CLI_Att_Month_End = clients.CLI_Att_Month_End;
                //cv.ParametersClientsModel.CLI_Att_MonthReal = clients.CLI_Att_MonthReal;

                cv.attendanceParameter.CLI_Id = id;
                Attendance_Parameter parameter = clientsManager.GetLatestAttendanceParameter(id);
                if (parameter != null)
                {
                    cv.attendanceParameter.ATP_Id = parameter.ATP_Id;
                    cv.attendanceParameter.ATP_Att_MonthReal = (parameter.ATP_Att_MonthReal!=null? parameter.ATP_Att_MonthReal.Value:false);        
                    cv.attendanceParameter.ATP_Att_Month_Start = parameter.ATP_Att_Month_Start;
                    cv.attendanceParameter.ATP_Att_Month_End = parameter.ATP_Att_Month_End;
                    cv.attendanceParameter.ATP_RegisteredOn = parameter.ATP_RegisteredOn;
                }
                cv.attendanceParameters = clientsManager.getAttendanceParameters(id);

                cv.ParametersClientsModel.clientsModel.CLI_Invoicing_Name = clients.CLI_Invoicing_Name;
                cv.ParametersClientsModel.clientsModel.STA_Id = clients.STA_Id;
                cv.clientsModel.STA_Id = clients.STA_Id;

                cv.ParametersClientsModel.clientsModel.CLI_Invoicing_Address1 = clients.CLI_Invoicing_Address1;
                cv.ParametersClientsModel.clientsModel.CLI_Invoicing_Address2 = clients.CLI_Invoicing_Address2;
                cv.ParametersClientsModel.clientsModel.CLI_Invoicing_City = clients.CLI_Invoicing_City;
                if (clients.CLI_Invoicing_ZipCode != null)
                    cv.ParametersClientsModel.clientsModel.CLI_Invoicing_ZipCode = clients.CLI_Invoicing_ZipCode;
                cv.ParametersClientsModel.clientsModel.CLI_Invoicing_Location = clients.CLI_Invoicing_Location;
                if (clients.CLI_IsIGST != null)
                    cv.ParametersClientsModel.clientsModel.CLI_IsIGST = clients.CLI_IsIGST.Value;
                if (clients.CLI_IGST != null)
                    cv.ParametersClientsModel.clientsModel.CLI_IGST = clients.CLI_IGST.Value;
                if (clients.CLI_IsCGST != null)
                    cv.ParametersClientsModel.clientsModel.CLI_IsCGST = clients.CLI_IsCGST.Value;
                if (clients.CLI_CGST != null)
                    cv.ParametersClientsModel.clientsModel.CLI_CGST = clients.CLI_CGST.Value;
                if (clients.CLI_IsSGST != null)
                    cv.ParametersClientsModel.clientsModel.CLI_IsSGST = clients.CLI_IsSGST.Value;
                if (clients.CLI_SGST != null)
                    cv.ParametersClientsModel.clientsModel.CLI_SGST = clients.CLI_SGST.Value;                
                cv.ParametersClientsModel.clientsModel.CLI_Place_Of_Supply = clients.CLI_Place_Of_Supply;


                cv.clientsModel.CLI_GST_Number = "";
                cv.clientsModel.CLI_GST_Rate = 0;
                cv.clientsModel.CLI_HSN_Code = "";
                cv.clientsModel.CLI_TDS_Rate = 0;

                cv.clientsModel.ADM_Id_RegisterBy = clients.ADM_Id_RegisterBy;
                cv.clientsModel.CLI_RegisteredOn = clients.CLI_RegisteredOn;
                cv.clientsModel.CliLogoImage = clients.CLI_Logo;


                cv.contacts = ClientContactMapper.mapContacts(clientsManager.GetClientContactsListById(id).ToList());
                cv.requirements = ClientRequirementMapper.mapRequirements(clientsManager.GetClient_RequirementsofClient(id, true).ToList());
                cv.employees = ClientEmployeeMapper.mapEmployees(clientsManager.listClientsEmployees(id, Convert.ToString((int)Assign_Unassign.Assign)).ToList(), _context);
            }

            IEnumerable<ProjectUtils.Total_WorkingDyas_In_Month> WorkingDays = Enum.GetValues(typeof(ProjectUtils.Total_WorkingDyas_In_Month))
                                                       .Cast<ProjectUtils.Total_WorkingDyas_In_Month>();
            ViewBag.TotalWorkingDays = from action in WorkingDays
                                       select new SelectListItem
                                       {
                                           Text = GetFullEnumString(action.ToString()),
                                           Value = ((int)action).ToString()
                                       };


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
            int newClientID = cv.clientsModel.CLI_Id;
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);            
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
                clients.CLI_TDS_Rate = 0;

                clients.CLI_Total_WorkingDays = cv.clientsModel.CLI_Total_WorkingDays;
                clients.CLI_No_Reduce_Days = cv.clientsModel.CLI_No_Reduce_Days;
                clients.CLI_WorkingHours_In_Day = cv.clientsModel.CLI_WorkingHours_In_Day;

                clients.ADM_Id_RegisterBy = cv.clientsModel.ADM_Id_RegisterBy;
                clients.CLI_RegisteredOn = cv.clientsModel.CLI_RegisteredOn;

                clients.STA_Id = cv.clientsModel.STA_Id;

                if (cv.clientsModel.CLI_Logo != null)
                {
                    clients.CLI_Logo = cv.clientsModel.CLI_Logo.FileName;
                }
                else
                {
                    clients.CLI_Logo = cv.clientsModel.CliLogoImage;
                }

                #region Invoicing parameter
                if (cv.clientsModel.CLI_Id > 0) { 
                Clients newClient = clientsManager.GetClientByIdNoTracking(cv.clientsModel.CLI_Id);
                clients.CLI_Invoicing_Name = newClient.CLI_Invoicing_Name;
                clients.STA_Id = newClient.STA_Id;
                clients.CLI_Invoicing_City = newClient.CLI_Invoicing_City;
                clients.CLI_Invoicing_Address1 = newClient.CLI_Invoicing_Address1;
                clients.CLI_Invoicing_Address2 = newClient.CLI_Invoicing_Address2;
                clients.CLI_Invoicing_ZipCode = newClient.CLI_Invoicing_ZipCode;
                clients.CLI_Invoicing_Location = newClient.CLI_Invoicing_Location;
                clients.CLI_GST_Number = newClient.CLI_GST_Number;
                clients.CLI_Place_Of_Supply = newClient.CLI_Place_Of_Supply;
                clients.CLI_HSN_Code = newClient.CLI_HSN_Code;
                clients.CLI_TDS_Rate = newClient.CLI_TDS_Rate;
                }
                #endregion
                var tuple = clientsManager.saveAddEditClients(clients);
                if (clientsManager.GetLatestAttendanceParameter(tuple.Item2)==null)
                {
                    Attendance_Parameter attendance_Parameter = new Attendance_Parameter();
                    attendance_Parameter.CLI_Id = tuple.Item2;
                    attendance_Parameter.ATP_Att_MonthReal = true;
                    attendance_Parameter.ATP_RegisteredOn = clients.CLI_RegisteredOn;
                    clientsManager.AddAttendanceParameter(attendance_Parameter);
                }
                if (tuple.Item1 != "")
                {
                    TempData["message"] = "Client data can not Inserted";
                }
                else
                {
                    newClientID = tuple.Item2;
                    TempData["message"] = "Successfull Done!";

                    if (cv.clientsModel.CLI_Id <= 0)
                    {
                        Client_ActivationHistory activationHistory = new Client_ActivationHistory();
                        //activationHistory.CAH_ActiveOn = DateNow();
                        activationHistory.CAH_ActiveOn = cv.clientsModel.CLI_RegisteredOn;
                        activationHistory.CLI_Id = newClientID;
                        clientsManager.AddEditActivationHistory(activationHistory);
                    }
                    else
                    {
                        Client_ActivationHistory activationHistory = clientsManager.GetLatestActiveHistory(newClientID);
                        activationHistory.CAH_ActiveOn = cv.clientsModel.CLI_RegisteredOn;
                        clientsManager.AddEditActivationHistory(activationHistory);
                    }

                    #region Image adding
                    //  string newPath = ProjectUtils.GetTempFolderPath(_hostingEnvironment.WebRootPath);
                    string ImagePath = Configuration.GetSection("DEFAULT_FOLDER_PATH").Value + Configuration.GetSection("CLIENTS_LOGO_PATH").Value;

                    if (!Directory.Exists(ImagePath + "/" + newClientID))
                    {
                        Directory.CreateDirectory(ImagePath + "/" + newClientID);
                    }
                    else
                    {
                        string[] files = Directory.GetFiles(ImagePath + "/" + newClientID);
                        if (file != null)
                        {
                            foreach (string s in files)
                            {
                                string fileName = Path.GetFileName(s);
                                System.IO.File.Delete(ImagePath + "/" + newClientID + "/" + fileName);
                            }
                        }
                    }
                    if (file == null || file.Length <= 0)
                    {
                    }
                    else
                    {
                        using (Image img = Image.FromStream(file.OpenReadStream()))
                        {
                            Stream ms = new MemoryStream(img.Resize(100, 100).ToByteArray());
                            var path = Path.Combine(ImagePath + "\\" + newClientID, file.FileName);
                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                ms.CopyTo(stream);
                                await file.CopyToAsync(stream);
                                stream.Flush();
                            }
                        }

                    }
                    #endregion
                }

            }
            return RedirectToAction("AddEditClients", new { id = newClientID });
        }

        [HttpPost]
        public ActionResult AttendanceParameters(ClientsViewModel cv)
        {
            try
            {
                ClientsManager clientsManager = new ClientsManager(_context);
                Attendance_Parameter attendance = new Attendance_Parameter();
                attendance.CLI_Id = cv.attendanceParameter.CLI_Id;

                Attendance_Parameter atp = clientsManager.GetAttendanceParameterByDate(cv.attendanceParameter.CLI_Id.Value, cv.attendanceParameter.ATP_RegisteredOn);

                if (atp != null)
                {
                    attendance.ATP_Id = atp.ATP_Id;
                }
                attendance.ATP_Att_MonthReal = cv.attendanceParameter.ATP_Att_MonthReal;
                attendance.ATP_Att_Month_End = cv.attendanceParameter.ATP_Att_Month_End;
                attendance.ATP_Att_Month_Start = cv.attendanceParameter.ATP_Att_Month_Start;
                if (cv.attendanceParameter.ATP_Att_MonthReal)
                {
                    attendance.ATP_Att_Month_End = null;
                    attendance.ATP_Att_Month_Start = null;
                }               
                
                attendance.ATP_RegisteredOn = cv.attendanceParameter.ATP_RegisteredOn;
                clientsManager.AddAttendanceParameter(attendance);
            }
            catch (Exception)
            {
                TempData["message"] = "Data can not Inserted";
            }
            
            return RedirectToAction("AddEditClients", new { id = cv.attendanceParameter.CLI_Id , tab = "Parameters" });
        }

        [HttpGet]
        public ActionResult AddEditContacts(int CLI_Id, int CON_Id = -1)
        {
            ClientContactVM contactVM = new ClientContactVM();
            if (CLI_Id > 0)
            {
                contactVM.CLI_Id = CLI_Id;
                ClientsManager clientsManager = new ClientsManager(_context, Configuration);
                Clients clients = clientsManager.GetClientById(CLI_Id);
                ViewBag.ClientName = clients.CLI_Name;
                if (CON_Id > 0)
                {
                    Client_Contacts contact = clientsManager.GetClientContactsById(CON_Id);
                    contactVM = ClientContactMapper.mapMe(contact);
                }
                else
                {
                    contactVM = new ClientContactVM();
                    contactVM.CON_Id = 0;                    
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
                if (contactVM.CLI_Id > 0)
                {
                    Client_Contacts clientContacts = new Client_Contacts();
                    clientContacts.CLI_Id = contactVM.CLI_Id;
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
            return RedirectToAction("AddEditClients", new { id = contactVM.CLI_Id, tab = "ContactInfo" });
        }

        public ActionResult DeleteContact(int id = -1)
        {
            int CLI_Id = 0;
            ClientsViewModel clientsViewModel = new ClientsViewModel();
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            if (ModelState.IsValid)
            {
                if (id > 0)
                {
                    var tuple = clientsManager.deleteContacts(id);
                    CLI_Id = tuple.Item2;
                    if (tuple.Item1 != string.Empty)
                    {
                        TempData["message"] = "Contacts data can not Deleted";
                    }
                }
            }
            return RedirectToAction("AddEditClients", new { id = CLI_Id, tab = "ContactInfo" });
        }

        [HttpPost]
        public string InActiveClient(int ClientID, bool Active,DateTime On,bool IsChangeDate=false)
        {
            string res = string.Empty;            
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            if (ModelState.IsValid)
            {
                if (ClientID > 0)
                {
                    res = clientsManager.InActiveClient(ClientID, Convert.ToInt32(Request.Cookies["AdminID"]), Active, On);
                    Client_ActivationHistory client_ActivationHistory = clientsManager.GetLatestActiveHistory(ClientID);
                    if (client_ActivationHistory == null)
                    {
                        client_ActivationHistory = clientsManager.GetLatestHistory(ClientID);
                    }
                    client_ActivationHistory.CAH_InactiveOn = On;                    
                    clientsManager.AddEditActivationHistory(client_ActivationHistory);
                    if (res != string.Empty)
                    {
                        TempData["message"] = "Client data can not Deleted";
                    }
                }
            }
            return res;
            // return RedirectToAction("Index","Clients",true);
        }
        [HttpPost]
        public string ReActiveClient(int ClientID,DateTime On)
        {
            string res = string.Empty;            
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            if (ModelState.IsValid)
            {
                if (ClientID > 0)
                {
                    res = clientsManager.ReActiveClient(ClientID, On);
                    Client_ActivationHistory client_ActivationHistory = new Client_ActivationHistory();
                    client_ActivationHistory.CAH_ActiveOn = On;
                    client_ActivationHistory.CLI_Id = ClientID;
                    clientsManager.AddEditActivationHistory(client_ActivationHistory);
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
        public ActionResult AddEditRequirement(int CLI_Id, int CRI_Id = -1, bool IsHistory = false,bool IsMajorModified=false)
        {            
            DesignationManager designationManager = new DesignationManager(_context);
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            AllowanceManager AllowanceManager = new AllowanceManager(_context);
            ClientRequirementVM clientRequirement = new ClientRequirementVM();
            Clients client = clientsManager.GetClientById(CLI_Id);
            ViewBag.client = client;
            List<Client_Requirement_Allowances> listClientReqAllowances = new List<Client_Requirement_Allowances>();

            clientRequirement.CRI_ProffTax_M_From_1 = 0;
            clientRequirement.CRI_ProffTax_M_To_1 = 7500;
            clientRequirement.CRI_ProffTax_M_Amount_1 = 0;
            clientRequirement.CRI_ProffTax_M_From_2 = 7501;
            clientRequirement.CRI_ProffTax_M_To_2 = 10000;
            clientRequirement.CRI_ProffTax_M_Amount_2 = 175;
            clientRequirement.CRI_ProffTax_M_From_3 = 10001;
            clientRequirement.CRI_ProffTax_M_To_3 = 1000000;
            clientRequirement.CRI_ProffTax_M_Amount_3 = 200;

            clientRequirement.CRI_ProffTax_F_From_1 = 0;
            clientRequirement.CRI_ProffTax_F_To_1 = 7500;
            clientRequirement.CRI_ProffTax_F_Amount_1 = 0;
            clientRequirement.CRI_ProffTax_F_From_2 = 7501;
            clientRequirement.CRI_ProffTax_F_To_2 = 10000;
            clientRequirement.CRI_ProffTax_F_Amount_2 = 0;
            clientRequirement.CRI_ProffTax_F_From_3 = 10001;
            clientRequirement.CRI_ProffTax_F_To_3 = 1000000;
            clientRequirement.CRI_ProffTax_F_Amount_3 = 200;
            if (CRI_Id > 0)
            {
                clientRequirement = ClientRequirementMapper.mapMe(clientsManager.GetRequirementsById(CRI_Id));
                listClientReqAllowances = AllowanceManager.GetClient_Requirement_AllowanceList(CRI_Id);
            }
            else
            {
                ViewBag.Designation = designationManager.getRemainingDesignationsList(CLI_Id);               
                clientRequirement.CRI_Id = -1;
                clientRequirement.CLI_Id = CLI_Id;
                clientRequirement.CRI_Active = true;
                clientRequirement.CRI_RegisteredOn = DateNow();
            }

            clientRequirement.allAllowances = AllowanceMapper.mapMeAllowancesWithClientReq(AllowanceManager.GetAllowanceList(), listClientReqAllowances);
            ViewBag.ADList = designationManager.getRemainingDesignationsList(CLI_Id).Select(m => new SelectListItem { Value = Convert.ToString(m.DES_Id), Text = m.DES_Title });

            clientRequirement.IsHistory = IsHistory;
            if (IsMajorModified)
            {
                clientRequirement.LastRecordRegOn = clientRequirement.CRI_RegisteredOn;
                clientRequirement.CRI_RegisteredOn = DateNow();
                clientRequirement.IsMajorModified = true;
            }
            return View(clientRequirement);
        }

        [HttpPost]
        public ActionResult AddEditRequirement(ClientRequirementVM clientRequirementVM)
        {
            string res = string.Empty;
            Client_Requirements cr = new Client_Requirements();
            List<Client_Requirement_Allowances> lst = AllowanceMapper.mapMeClientReqAllowances(clientRequirementVM.allAllowances);
            List<Client_Requirement_Allowances> Removelst = AllowanceMapper.mapMeClientReqAllowancesRemove(clientRequirementVM.allAllowances);
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
                    if (clientRequirementVM.CRI_OT_Fixed_PerHour > 0 && clientRequirementVM.CRI_OT_Calculate_Differently)
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
                if (!clientRequirementVM.CRI_Nightshift_Allowance)
                {
                    clientRequirementVM.CRI_Nightshift_Allowance_Rate = 0;
                }
                
                if (clientRequirementVM.CRI_Billing_Type == (int)ProjectUtils.CRI_BILLING_TYPE.Lump_Sum_Amount)
                {
                    clientRequirementVM.CRI_Billing_ServiceCharge_Formula = null;
                    clientRequirementVM.CRI_Billing_ServiceCharge = null;
                }else if (clientRequirementVM.CRI_Billing_Type == (int)ProjectUtils.CRI_BILLING_TYPE.Service_Change_Basic)
                {
                    clientRequirementVM.CRI_Billing_Amount = null;                    
                }
                cr = ClientRequirementMapper.mapMeModel(clientRequirementVM);

                if (clientRequirementVM.IsHistory)
                {
                    res = clientsManager.EditHistoryRequirement(cr, lst, Removelst, sessionUtils.GetLoggedAdminID());
                }
                else
                {
                    res = clientsManager.AddEditRequirement(cr, lst,  sessionUtils.GetLoggedAdminID());
                }
               
            }
            if (res != "")
            {
                TempData["message"] = "Error In Client Requirement Or Reset Wage Register!";
                return RedirectToAction("AddEditClients", new { id = clientRequirementVM.CLI_Id });
            }
            return RedirectToAction("AddEditClients", new { id = clientRequirementVM.CLI_Id, tab = "ClientRequirement" });
        }

        public ActionResult HistoryRequirement(int DES_Id, int CLI_Id)
        {         
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            DesignationManager designationManager = new DesignationManager(_context);
            List<ClientRequirementVM> lst = ClientRequirementMapper.mapRequirements(clientsManager.GetClient_RequirementsList(DES_Id, CLI_Id, false).ToList());
            ViewBag.DES_Title = designationManager.GetDesignationsById(DES_Id);
            ViewBag.CLI_Name = clientsManager.GetClientById(CLI_Id).CLI_Name;
            ViewBag.CLI_Id = CLI_Id;
            return View(lst);
        }

        public ActionResult InactiveRequirement(int CRI_Id)
        {
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            var tuple = clientsManager.InactiveRequirement(CRI_Id, sessionUtils.GetLoggedAdminID());
            if (tuple.Item1 != string.Empty)
            {
                TempData["message"] = "Requirement can not deleted! Please remove assigned employees of this requirement.";
            }
            return RedirectToAction("AddEditClients", new { id = tuple.Item2, tab = "ClientRequirement" });
        }

        public ActionResult EditRegistrationDate(int CRI_Id, string Act = "")
        {
            ClientsManager clientsManager = new ClientsManager(_context);
            ClientRequirementVM requirementVM = new ClientRequirementVM();
            Client_Requirements requirement = clientsManager.GetRequirementsById(CRI_Id);
            requirementVM = ClientRequirementMapper.mapMe(requirement);
            IEnumerable<Client_Requirements> lst = clientsManager.GetClientRequirementsList(requirement.DES_Id, requirement.CLI_Id).Take(2);
            if (lst.Count() > 1)
            {
                DateTime date = lst.Skip(1).First().CRI_RegisteredOn;
                requirementVM.LastRecordRegOn = date;
            }
            if (Act == "History")
            {
                requirementVM.Edit_History = Act;
            }

            return PartialView("_EditRegistrationDate", requirementVM);
        }
        [HttpPost]
        public ActionResult EditRegistrationDate(ClientRequirementVM requirementVM)
        {
            ClientsManager clientsManager = new ClientsManager(_context);
            try
            {
                clientsManager.EditRequirementRegDate(requirementVM.CRI_Id, requirementVM.CRI_RegisteredOn);
            }
            catch (Exception)
            {
                TempData["message"] = "Try Again";
            }
            if (requirementVM.Edit_History == "History")
            {
                return RedirectToAction("HistoryRequirement", new { DES_Id = requirementVM.DES_Id, CLI_Id = requirementVM.CLI_Id });
            }
            else
            {
                return RedirectToAction("AddEditClients", new { id = requirementVM.CLI_Id, tab = "ClientRequirement" });
            }

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

                clients.CLI_Invoicing_Name = cvm.ParametersClientsModel.clientsModel.CLI_Invoicing_Name;
                clients.CLI_Invoicing_Address1 = cvm.ParametersClientsModel.clientsModel.CLI_Invoicing_Address1;
                clients.CLI_Invoicing_Address2 = cvm.ParametersClientsModel.clientsModel.CLI_Invoicing_Address2;
                clients.STA_Id = cvm.ParametersClientsModel.clientsModel.STA_Id;
                clients.CLI_Invoicing_City = cvm.ParametersClientsModel.clientsModel.CLI_Invoicing_City;
                clients.CLI_Invoicing_ZipCode = cvm.ParametersClientsModel.clientsModel.CLI_Invoicing_ZipCode;
                clients.CLI_Invoicing_Location = cvm.ParametersClientsModel.clientsModel.CLI_Invoicing_Location;
                clients.CLI_IsIGST = cvm.ParametersClientsModel.clientsModel.CLI_IsIGST;
                clients.CLI_IGST = cvm.ParametersClientsModel.clientsModel.CLI_IGST;
                if (!cvm.ParametersClientsModel.clientsModel.CLI_IsIGST)
                {
                    clients.CLI_IGST = 0;
                }
                clients.CLI_IsCGST = cvm.ParametersClientsModel.clientsModel.CLI_IsCGST;
                clients.CLI_CGST = cvm.ParametersClientsModel.clientsModel.CLI_CGST;
                if (!cvm.ParametersClientsModel.clientsModel.CLI_IsCGST)
                {
                    clients.CLI_CGST = 0;
                }
                clients.CLI_IsSGST = cvm.ParametersClientsModel.clientsModel.CLI_IsSGST;
                clients.CLI_SGST = cvm.ParametersClientsModel.clientsModel.CLI_SGST;
                if (!cvm.ParametersClientsModel.clientsModel.CLI_IsSGST)
                {
                    clients.CLI_SGST = 0;
                }                
                clients.CLI_Place_Of_Supply = cvm.ParametersClientsModel.clientsModel.CLI_Place_Of_Supply;
                                
                string res = clientsManager.UpdateParameters(clients);
               // string att=clientsManager.AddAttendanceParameter(attendance);
                if (res != string.Empty)
                {
                    TempData["message"] = "data can not updated";
                }
            }
            return RedirectToAction("AddEditClients", new { id = cvm.clientsModel.CLI_Id, tab = "Parameters" });
        }

        [HttpGet]
        public ActionResult AddEmployee(int CLI_Id, int CLE_Id = -1)
        {
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            ClientEmployeeVM cvm = new ClientEmployeeVM();
            int FRM_Id = clientsManager.GetClientById(CLI_Id).FRM_Id;
            DesignationManager designationManager = new DesignationManager(_context);
            IEnumerable<AssignEmployeeVM> listDesignations = designationManager.getDesignationsListInVM(CLI_Id);
            IEnumerable<EmployeeVM> listEmployee = null;
            cvm.CLE_RegisteredOn = ProjectUtils.DateNow();
            if (CLE_Id > 0)
            {
                listEmployee = EmployeesMapper.MapEmployees(clientsManager.getEmployeeList(FRM_Id).ToList());
                Clients_Employees clientEmployee = clientsManager.ClientEmployeeById(CLE_Id);
                cvm.DES_Id = clientEmployee.DES_Id;
                cvm.EMP_Id = clientEmployee.EMP_Id;
                cvm.CLE_RegisteredOn = clientEmployee.CLE_RegisteredOn;
                cvm.CLE_ReassignedOn = clientEmployee.CLE_ReassignedOn;
                cvm.CLE_UnassignedOn = clientEmployee.CLE_UnassignedOn;
                cvm.ADM_Id_UnassignedBy = clientEmployee.ADM_Id_UnassignedBy;
                cvm.Old_DES_Id = clientEmployee.DES_Id;
            }
            else
            {
                listEmployee = EmployeesMapper.MapEmployees(clientsManager.getActiveEmployeeList_NotAssignedYet(CLI_Id, FRM_Id).ToList());
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
                SessionUtils sessionUtils = new SessionUtils(Request, Response);

                if (cvm.CLE_Id > 0)
                {
                    clientsEmployees = clientsManager.ClientEmployeeById(cvm.CLE_Id);
                    clientsEmployees.CLE_RegisteredOn = cvm.CLE_RegisteredOn;
                }
                else
                {                    
                    clientsEmployees.CLI_Id = cvm.CLI_Id;
                    clientsEmployees.EMP_Id = cvm.EMP_Id;
                    clientsEmployees.DES_Id = cvm.DES_Id;
                    clientsEmployees.CLE_RegisteredOn = (cvm.CLE_RegisteredOn!=null? cvm.CLE_RegisteredOn: DateNow());
                    clientsEmployees.ADM_Id_RegisteredBy= sessionUtils.GetLoggedAdminID();
                }        
                res = clientsManager.ClientEmployee(clientsEmployees, cvm.Old_DES_Id, sessionUtils.GetLoggedAdminID());
                if (res != string.Empty)
                {
                    TempData["message"] = "ClientEmployee data can not Inserted";
                }
            }
            return RedirectToAction("AddEditClients", new { id = cvm.CLI_Id, tab = "ClientEmployee" });
        }
              

        public ActionResult ReassignClientEmployee(int CLE_Id = -1)
        {
            int CLI_Id = 0;
            ClientsViewModel clientsViewModel = new ClientsViewModel();
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            if (ModelState.IsValid)
            {
                if (CLE_Id > 0)
                {
                    var tuple = clientsManager.ReassignClientEmployee(CLE_Id);
                    CLI_Id = tuple.Item2;
                    if (tuple.Item1 != string.Empty)
                    {
                        TempData["message"] = "Employee is not able to Resssign!Try Again";
                    }
                }
            }
            return RedirectToAction("AddEditClients", new { id = CLI_Id, tab = "ClientEmployee" });
        }

        public ActionResult DeleteAssignEmployee(int CLE_Id, int CLI_ID, int EMP_Id, int DES_Id)
        {
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            string res = clientsManager.DeleteAssignEmployee(CLE_Id, CLI_ID, EMP_Id, DES_Id);
            if (res != string.Empty)
            {
                TempData["message"] = "Assign employee can not deleted!";
            }
            return RedirectToAction("AddEditClients", new { id = CLI_ID, tab = "ClientEmployee" });
        }
              


        public async Task<FileResult> GenerateExcelTemplate_TwoRow(DateTime month,int CLI_Id)
        {
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            Clients client = clientsManager.GetClientById(CLI_Id);
            IEnumerable<Clients_Employees> employees = clientsManager.listActiveClientsEmployees(CLI_Id, month);
            string newPath = ProjectUtils.GetTempFolderPath(_hostingEnvironment.WebRootPath);
            string fileName = "Template_" + month.ToString("ddMMyyyyHHmm") + "_" + client.CLI_Name + "_TwoRow.xlsx";
            string fullMonthName = month.ToString("MMM", CultureInfo.CreateSpecificCulture("IN"));
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
                style.WrapText = true;
                
                IFont fontSubHead = workbook.CreateFont();
                fontSubHead.IsBold = true;
                fontSubHead.FontHeightInPoints = ((short)14);
                fontSubHead.FontName = ("Cambria");
                ICellStyle styleDesignation = workbook.CreateCellStyle();
                styleDesignation.FillBackgroundColor = HSSFColor.BlueGrey.Index;
                styleDesignation.SetFont(fontSubHead);

                IRow row = excelSheet.CreateRow(0);
                row.Height = 500;

                Attendance_Parameter attendance_Parameter = clientsManager.GetAttendanceParameterByMonth(client.CLI_Id, month);
                DateTime[] period = DateHelper.getStartEndDatePeriodForAttendance(client, attendance_Parameter, month);
                int TotalDays = Convert.ToInt32((period[1] - period[0]).TotalDays) + 4;


                ICell CellHeader = row.CreateCell(0);
                CellHeader.SetCellValue(client.FRM_.FRM_Name);
                CellHeader.CellStyle = styleHeader;
                CellUtil.SetAlignment(CellHeader, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, TotalDays));

                #region added new on 22 jan 2020
                IRow rowAdd1 = excelSheet.CreateRow(1);
                ICell CellAdd1 = rowAdd1.CreateCell(0);
                CellAdd1.SetCellValue(client.FRM_.FRM_Address1.ToUpper() + "," + client.FRM_.FRM_Address2.ToUpper() + ",");
                CellUtil.SetAlignment(CellAdd1, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(1,1, 0, TotalDays));

                IRow rowSubHeading = excelSheet.CreateRow(2);
                ICell CellSubHeading = rowSubHeading.CreateCell(0);
                CellSubHeading.SetCellValue("ATTENDANCE FOR THE MONTH OF " + fullMonthName.ToUpper() + "-" + month.ToString("yy"));
                CellSubHeading.CellStyle = styleDesignation;
                CellUtil.SetAlignment(CellSubHeading, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, TotalDays));

                IRow rowClient = excelSheet.CreateRow(3);
                ICell CellClient = rowClient.CreateCell(0);
                CellClient.SetCellValue(client.CLI_Name.ToString());
                CellClient.CellStyle = styleDesignation;
                CellUtil.SetAlignment(CellClient, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(3, 3, 0, TotalDays));

                #endregion

                row = excelSheet.CreateRow(4);
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
                    c.SetCellValue(tmpDate.ToString("ddd") + "\r\n" + tmpDate.Day);
                    c.CellStyle = style;
                    CellUtil.SetAlignment(c, workbook, (short)HorizontalAlignment.Center);
                    excelSheet.SetColumnWidth(i, (int)((5 + 0.72) * 256));
                    tmpDate = tmpDate.AddDays(1);
                    i++;
                }

                int rowCount = 5;
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

        public async Task<FileResult> GenerateExcelTemplate_OneRow(DateTime month,int CLI_Id)
        {
            try
            {
                ClientsManager clientsManager = new ClientsManager(_context, Configuration);
                Clients client = clientsManager.GetClientById(CLI_Id);
                //IEnumerable<Clients_Employees> employees = clientsManager.listClientsEmployees(ClientId);
                IEnumerable<Clients_Employees> employees = clientsManager.listActiveClientsEmployees(CLI_Id, month);
                string newPath = ProjectUtils.GetTempFolderPath(_hostingEnvironment.WebRootPath);
                string fileName = "Template_" + month.ToString("ddMMyyyyHHmm") + "_" + client.CLI_Name + "_OneRow.xlsx";
                string fullMonthName = month.ToString("MMM", CultureInfo.CreateSpecificCulture("IN"));
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
                    style.WrapText = true;

                    IFont fontSubHead = workbook.CreateFont();
                    fontSubHead.IsBold = true;
                    fontSubHead.FontHeightInPoints = ((short)14);
                    fontSubHead.FontName = ("Cambria");
                    ICellStyle styleDesignation = workbook.CreateCellStyle();
                    styleDesignation.FillBackgroundColor = HSSFColor.BlueGrey.Index;
                    styleDesignation.SetFont(fontSubHead);

                    IRow row = excelSheet.CreateRow(0);
                    row.Height = 500;

                    Attendance_Parameter attendance_Parameter = clientsManager.GetAttendanceParameterByMonth(client.CLI_Id, month);
                    DateTime[] period = DateHelper.getStartEndDatePeriodForAttendance(client, attendance_Parameter, month);
                    int TotalDays = Convert.ToInt32((period[1] - period[0]).TotalDays) + 4;


                    ICell CellHeader = row.CreateCell(0);
                    CellHeader.SetCellValue(client.FRM_.FRM_Name);
                    CellHeader.CellStyle = styleHeader;
                    CellUtil.SetAlignment(CellHeader, workbook, (short)HorizontalAlignment.Center);
                    excelSheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, TotalDays));

                    #region added new on 22 jan 2020
                    IRow rowAdd1 = excelSheet.CreateRow(1);
                    ICell CellAdd1 = rowAdd1.CreateCell(0);
                    CellAdd1.SetCellValue(client.FRM_.FRM_Address1.ToUpper() + "," + client.FRM_.FRM_Address2.ToUpper() + ",");
                    CellUtil.SetAlignment(CellAdd1, workbook, (short)HorizontalAlignment.Center);
                    excelSheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, TotalDays));

                    IRow rowSubHeading = excelSheet.CreateRow(2);
                    ICell CellSubHeading = rowSubHeading.CreateCell(0);
                    CellSubHeading.SetCellValue("ATTENDANCE FOR THE MONTH OF " + fullMonthName.ToUpper() + "-" + month.ToString("yy"));
                    CellSubHeading.CellStyle = styleDesignation;
                    CellUtil.SetAlignment(CellSubHeading, workbook, (short)HorizontalAlignment.Center);
                    excelSheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, TotalDays));

                    IRow rowClient = excelSheet.CreateRow(3);
                    ICell CellClient = rowClient.CreateCell(0);
                    CellClient.SetCellValue(client.CLI_Name.ToString());
                    CellClient.CellStyle = styleDesignation;
                    CellUtil.SetAlignment(CellClient, workbook, (short)HorizontalAlignment.Center);
                    excelSheet.AddMergedRegion(new CellRangeAddress(3, 3, 0, TotalDays));

                    #endregion

                    row = excelSheet.CreateRow(4);
                    row.HeightInPoints = ((5 * excelSheet.DefaultRowHeightInPoints));


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
                        c.SetCellValue(tmpDate.ToString("ddd") + "\r\n" + tmpDate.Day);
                        c.CellStyle = style;
                        CellUtil.SetAlignment(c, workbook, (short)HorizontalAlignment.Center);
                        excelSheet.SetColumnWidth(i, (int)((5 + 0.72) * 256));
                        tmpDate = tmpDate.AddDays(1);
                        i++;
                    }

                    int rowCount = 5;
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
            catch (Exception ex)
            {
                TempData["message"] = ex.InnerException;
                throw ex;
            }
            
        }

        public ActionResult GetAssignEmployeeDependancy(int CLE_Id, int CLI_Id, int EMP_Id, int DES_Id)
        {
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            string res = clientsManager.GetAssignEmployeeDependancy(CLE_Id, CLI_Id, EMP_Id, DES_Id);
            return Content(res);
        }

        public ActionResult GetClientEmployee(int CLI_Id,string assign)
        {
            ClientsManager clientsManager = new ClientsManager(_context);
            List<ClientEmployeeVM> ClientEmployeeVMs = new List<ClientEmployeeVM>();
            ClientEmployeeVMs = ClientEmployeeMapper.mapEmployees(clientsManager.listClientsEmployees(CLI_Id, assign).ToList(), _context);
            return PartialView("_ClientEmployee", ClientEmployeeVMs);
        }
                       
        public IActionResult EditUnassignDate(int CLE_Id,int CLI_Id,string act)
        {
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            UnassignVM unassignVM = new UnassignVM();
            unassignVM.CLI_Id = CLI_Id;
            if (CLE_Id > 0)
            {
                unassignVM.CLE_Id = CLE_Id;
                unassignVM.UnassignedOn = ProjectUtils.DateNow();
                if (act == "edit")
                {
                    Clients_Employees clientEmp = clientsManager.ClientEmployeeById(CLE_Id);                    
                    unassignVM.UnassignedOn = clientEmp.CLE_UnassignedOn.Value;                   
                }               
                
            }
            return PartialView("_EditUnAssignDate", unassignVM);
        }
        public ActionResult UnassignClientEmp(UnassignVM unassignVM)
        {            
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            if (ModelState.IsValid)
            {
                if (unassignVM.CLE_Id > 0)
                {
                    string res = clientsManager.UnassignClientEmployee(unassignVM.CLE_Id, unassignVM.UnassignedOn, sessionUtils.GetLoggedAdminID());
                    if (res != string.Empty)
                    {
                        TempData["message"] = "Employee is not able to Unassigned! Try Again";
                    }
                }
            }
            return RedirectToAction("AddEditClients", new { id = unassignVM.CLI_Id, tab = "ClientEmployee" });
        }
        public ActionResult RevertAssignEmployee(int CLE_Id)
        {
            int CLI_Id = 0;
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            try
            {
                Clients_Employees clientEmployee = clientsManager.RevertAssignEmployee(CLE_Id);
                CLI_Id = clientEmployee.CLI_Id;
            }
            catch (Exception)
            {
                TempData["message"] = "Employee is not able to Unassigned! Try Again";
            }
            return RedirectToAction("AddEditClients", new { id = CLI_Id, tab = "ClientEmployee" });
        }

        public ActionResult EditAttRegDate(int ATP_Id)
        {
            ClientsManager clientsManager = new ClientsManager(_context);            
            return PartialView("_EditAttRegistrationDate", AttendanceParameterVM.mapMeModel(clientsManager.getAttendanceParameter(ATP_Id)));
        }
        [HttpPost]
        public ActionResult EditAttRegDate(AttendanceParameterVM attendance_Parameter)
        {
            ClientsManager clientsManager = new ClientsManager(_context);
            try
            {
                clientsManager.EditAttendanceParameter(AttendanceParameterVM.mapMe(attendance_Parameter));
            }
            catch (Exception)
            {
                TempData["message"] = "Try Again";
            }
            return RedirectToAction("AddEditClients", new { id = attendance_Parameter.CLI_Id, tab = "Parameters" });
        }
    }
}