using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using RMERP.DAL.Helpers;
using RMERP.DAL.ManagerClasses;
using RMERP.DAL.Mappers;
using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;
using RMERP.Helpers;
using Microsoft.AspNetCore.Hosting;
using Rotativa.AspNetCore;
using static RMERP.DAL.Helpers.ProjectUtils;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RMERP.Controllers
{
    public class InvoicesController : Controller
    {
        public IConfiguration _configuration;
        private readonly RMERPContext _context;
        private IHostingEnvironment _hostingEnvironment;
        public InvoicesController(RMERPContext context, IHostingEnvironment hostingEnvironment, IConfiguration configuration)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _configuration=configuration;
        }

        public IActionResult Index()
        {           
            ClientsManager clientsManager = new ClientsManager(_context);
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            InvoicesManager invoicesManager = new InvoicesManager(_context);                      
            InvoiceMasterVM masterVM = new InvoiceMasterVM();
            int FRM_Id = (sessionUtils.GetLoggedFirmID()!=null? sessionUtils.GetLoggedFirmID().Value:0);           
            List<Invoices> invoices = invoicesManager.GetInvoices(FRM_Id);
            masterVM.InvoiceVMs = InvoiceMapper.mapMe(invoices.Where(m => DateTime.Compare(m.INV_CreatedOn.Date, DateTime.Now.Date.AddMonths(-2)) > 0).ToList());           
            ViewBag.ClientList =clientsManager.listClients(true, FRM_Id).OrderBy(m=>m.CLI_Name);

            DateTime dt = DateTime.Now;          

            ViewBag.linktoYearId = GetYears(dt.Year);
            ViewBag.linktoMonthId = GetMonths(dt.Year);
            return View(masterVM);
        }
                      
       
        public ActionResult DeleteInvoice(int INV_Id)
        {
            InvoicesManager invoicesManager = new InvoicesManager(_context);
            try
            {
                invoicesManager.DeleteInvoice(INV_Id);
            }
            catch (Exception)
            {
                TempData["message"] = "First remove its incidence concepts!";
            }
            return RedirectToAction("Index");
        }

        public IActionResult AddEditInvoice(int INV_Id = 0)
        {
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            ClientsManager clientsManager = new ClientsManager(_context);
            WageProcessManager wagManager = new WageProcessManager(_context);
            InvoicesManager invoicesManager = new InvoicesManager(_context);
            ViewBag.Clients = clientsManager.listClients(true, sessionUtils.GetLoggedFirmID()).OrderBy(m=>m.CLI_Name);
            InvoiceVM invoiceVM = new InvoiceVM();

            invoiceVM.INV_Number = invoicesManager.GetNextInvoiceNumber(); 
            
            invoiceVM.INV_Date = DateNow();            
            invoiceVM.Invoice_Concepts = new List<Invoice_ConceptsVM>();
            if (INV_Id > 0)
            {
                Invoices invoice = invoicesManager.GetInvoice(INV_Id);
                invoiceVM = InvoiceMapper.mapMe(invoice);
                invoiceVM.totTAX = invoice.INV_IGST_Total.Value + invoice.INV_CGST_Total.Value + invoice.INV_SGST_Total.Value;
                invoiceVM.All_INC_Total = invoice.Invoice_Concepts.Sum(m => m.INC_Total);
            }           
            return View(invoiceVM);
        }
        [HttpPost]
        public JsonResult AddEditInvoice_1([FromBody]InvoiceVM InvoiceVM)
        {
            ClientsManager clientsManager = new ClientsManager(_context);
            InvoicesManager invoicesManager = new InvoicesManager(_context);
            int i = 0;
            try
            {
                SessionUtils sessionUtils = new SessionUtils(Request, Response);
                InvoiceVM.FRM_Id = clientsManager.GetClientById(InvoiceVM.CLI_Id).FRM_Id;
                InvoiceVM.ADM_Id_CreatedBy = sessionUtils.GetLoggedAdminID();
                List<Invoice_Concepts> Invoice_Concepts = new List<Invoice_Concepts>();
                int INV_Id = invoicesManager.AddEditInvoice(InvoiceMapper.mapMeModel(InvoiceVM));

                if (InvoiceVM.Invoice_Concepts != null)
                {
                    foreach (Invoice_ConceptsVM invoice_Concept in InvoiceVM.Invoice_Concepts)
                    {
                        invoice_Concept.INV_Id = INV_Id;
                        Invoice_Concepts.Add(InvoiceConceptsMapper.mapMeModel(invoice_Concept));
                    }
                    i = invoicesManager.AddInvoiceConcept(Invoice_Concepts, INV_Id);
                }
                else
                {
                    invoicesManager.DeleteIncidenceConcepts(INV_Id);
                }

            }
            catch (Exception)
            {
                return Json("Ko");
            }
            return Json("Ok");
        }
        [HttpPost]
        public IActionResult AddEditInvoice(string Invoice)
        {            
            var format = "yyyy-MM-dd"; // your datetime format
            var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = format };
            var InvoiceVM = JsonConvert.DeserializeObject<InvoiceVM>(Invoice, dateTimeConverter);

            ClientsManager clientsManager = new ClientsManager(_context);
            InvoicesManager invoicesManager = new InvoicesManager(_context);
            int i = 0;
            try
            {
                SessionUtils sessionUtils = new SessionUtils(Request, Response);
                InvoiceVM.FRM_Id = clientsManager.GetClientById(InvoiceVM.CLI_Id).FRM_Id;
                InvoiceVM.ADM_Id_CreatedBy = sessionUtils.GetLoggedAdminID();
                List<Invoice_Concepts> Invoice_Concepts = new List<Invoice_Concepts>();
                int INV_Id = invoicesManager.AddEditInvoice(InvoiceMapper.mapMeModel(InvoiceVM));

                if (InvoiceVM.Invoice_Concepts != null)
                {
                    foreach (Invoice_ConceptsVM invoice_Concept in InvoiceVM.Invoice_Concepts)
                    {                        
                        invoice_Concept.INV_Id = INV_Id;
                        Invoice_Concepts.Add(InvoiceConceptsMapper.mapMeModel(invoice_Concept));
                    }
                    i = invoicesManager.AddInvoiceConcept(Invoice_Concepts, INV_Id);
                }
                else
                {
                    invoicesManager.DeleteIncidenceConcepts(INV_Id);
                }

            }
            catch (Exception)
            {
                return Content("Ko");
            }

            return Content("Ok");
        }

        public ActionResult GetInvoiceTemplate(int CLI_Id,DateTime INV_Date)
        {
            InvoiceTypeVM invoiceTypeVM = new InvoiceTypeVM();
            ClientsManager clientsManager = new ClientsManager(_context);
            WageRegisterManager registerManager = new WageRegisterManager(_context);
            WageProcessManager wagManager = new WageProcessManager(_context);
            IEnumerable<INVOICE_TEMPLATE_TYPE> INVOICE_TEMPLATE_TYPE = Enum.GetValues(typeof(INVOICE_TEMPLATE_TYPE))
                                                       .Cast<INVOICE_TEMPLATE_TYPE>();

            ViewBag.InvoiceType = from action in INVOICE_TEMPLATE_TYPE
                                  select new SelectListItem
                                  {
                                      Text = GetStringValue(action),
                                      Value = ((int)action).ToString()
                                  };

            int FRM_Id = clientsManager.GetClientById(CLI_Id).FRM_Id;
            var cc = wagManager.getDistinctWageMonths(FRM_Id);

            ViewBag.Months = from c in cc
                             select new SelectListItem
                             {
                                 Text = c.WAG_Month.ToString("MMM-yyyy"),
                                 Value = c.WAG_Id.ToString()
                             };
            DateTime Prev_INV_Date = INV_Date.AddMonths(-1);
            Wage_Process PrevWage = cc.Where(m => m.WAG_Month.Month == Prev_INV_Date.Month && m.WAG_Month.Year == Prev_INV_Date.Year).FirstOrDefault();
            if (PrevWage != null)
            {
                invoiceTypeVM.WAG_Id = PrevWage.WAG_Id;
            }
            else
            {
                invoiceTypeVM.WAG_Id = cc.FirstOrDefault().WAG_Id;
            }
            List<Employees> emps = registerManager.GetWageRegistersForInvoice(CLI_Id).Where(m => m.EMP_.Clients_Employees.Where(cle => cle.CLI_Id.Equals(CLI_Id)).Any(mb => mb.CLE_UnassignedOn != null)).Select(m => m.EMP_).Distinct().ToList();
            ViewBag.LeftEmps = EmployeesMapper.MapEmployees(emps); 

            return PartialView("_InvoiceType", invoiceTypeVM);
        }
        public JsonResult GetRequirenmentTypes(int Wag_Id, int CLI_Id)
        {            
            return Json(new SelectList(GetRequirements(Wag_Id, CLI_Id), "Value", "Text"));
        }
        private List<SelectListItem> GetRequirements(int Wag_Id, int CLI_Id)
        {           
            ClientsManager clientsManager = new ClientsManager(_context);
            WageProcessManager wagManager = new WageProcessManager(_context);
            List<SelectListItem> Requirements = new List<SelectListItem>();
            DateTime date = wagManager.getWageProcessById(Wag_Id).WAG_Month;
            List<Client_Requirements> list = clientsManager.getClientRequirements(date, CLI_Id);
            foreach (var item in list.Where(m => m.CRI_Billing_Type == (int)CRI_BILLING_TYPE.Lump_Sum_Amount && m.CRI_Billing_Amount != null).Select(m => new { m.CRI_Billing_Amount }).Distinct())
            {
                string Text = "CR With " + item.CRI_Billing_Amount + " Fixed Amount";
                Requirements.Add(new SelectListItem { Text = Text, Value = "fixed_"+item.CRI_Billing_Amount.ToString() });
            }
            foreach (var item in list.Where(m => m.CRI_Billing_Type == (int)CRI_BILLING_TYPE.Service_Change_Basic && m.CRI_Billing_ServiceCharge != null).Select(m => new { m.CRI_Billing_ServiceCharge }).Distinct())
            {
                string Text = "CR With Service Charge Of " + item.CRI_Billing_ServiceCharge + "%";
                Requirements.Add(new SelectListItem { Text = Text, Value = "service_"+item.CRI_Billing_ServiceCharge.ToString() });
            }
            return Requirements;
        }
        public IActionResult DownloadInvoice(int INV_Id,int ActionId)
        {
            InvoicesManager invoicesManager = new InvoicesManager(_context);
            Invoices invoices = invoicesManager.GetInvoice(INV_Id);
            if (ActionId == (int)PDFAction.View)
            {
                return View(invoices);
            }
            else 
            {
                string FileName = "Invoice_" + invoices.INV_Number + "_" + DateTime.Now.ToString("ddMMyyyy") + ".pdf";
                DateTime toDay = DateNow();
                string PaySlipPath = _configuration.GetSection("DEFAULT_FOLDER_PATH").Value + _configuration.GetSection("RMERP_CLIENTS_INVOICE_PATH").Value;
                var root = PaySlipPath + "\\" + toDay.Year + "\\" + toDay.Month;
                if (!Directory.Exists(root))
                {
                    Directory.CreateDirectory(root);
                }
                else
                {
                    string[] fileList = Directory.GetFiles(root, FileName);
                    if (fileList != null)
                    {
                        foreach (string s in fileList)
                        {
                            string fileName = Path.GetFileName(s);
                            System.IO.File.Delete(root + "/" + fileName);
                        }
                    }
                }
                var path = Path.Combine(root, FileName);
                path = Path.GetFullPath(path);
                var view = new ViewAsPdf(invoices)
                {
                    FileName = FileName,
                    SaveOnServerPath = path,
                    PageSize = Rotativa.AspNetCore.Options.Size.A4
                };
                return view;
            }
        }

        public ActionResult SearchInvoice(int CLI_Id,int Year,int Month)
        {
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            InvoicesManager invoicesManager = new InvoicesManager(_context);
            int FRM_Id = (sessionUtils.GetLoggedFirmID() != null ? sessionUtils.GetLoggedFirmID().Value : 0);
            List<InvoiceVM> invoiceVMs = new List<InvoiceVM>();
            if (CLI_Id >0)
            {                
                invoiceVMs = InvoiceMapper.mapMe(invoicesManager.GetInvoicesByClientId(CLI_Id));                
            }
            else
            {
                invoiceVMs = InvoiceMapper.mapMe(invoicesManager.GetInvoices(FRM_Id).Where(m => DateTime.Compare(m.INV_CreatedOn.Date, DateTime.Now.Date.AddMonths(-2)) > 0).ToList());
            }
            if (Year > 0)
            {
                invoiceVMs=invoiceVMs.Where(m => m.INV_Date.Year == Year).ToList();
            }
            if (Month > 0)
            {
                invoiceVMs=invoiceVMs.Where(m => m.INV_Date.Month == Month).ToList();
            }
            return PartialView("_InvoiceList", invoiceVMs);
        }

        public Invoice_Concepts GetInvoiceData(int CLI_Id,string Type_Id,int WAG_Id,int Type, string EMPs)
        {
            
            Invoice_Concepts concept = new Invoice_Concepts();                        
            switch (Type)
            {
                case (int)INVOICE_TEMPLATE_TYPE.CONTRACT_BILL_FOR_PROVIDING_FACILITY_SERVICES:
                    concept = Get_Billing_Data_T1(CLI_Id,Type_Id,WAG_Id);
                    break;
                case (int)INVOICE_TEMPLATE_TYPE.COMPANY_CONTRIBUTION_PF:
                    concept = Get_Billing_Data_T2_PF(CLI_Id, WAG_Id);
                    break;
                case (int)INVOICE_TEMPLATE_TYPE.COMPANY_CONTRIBUTION_ESIC:
                    concept = Get_Billing_Data_T2_ESIC(CLI_Id, WAG_Id);
                    break;
                case (int)INVOICE_TEMPLATE_TYPE.FULL_AND_FINAL_SETTLEMENT:
                    concept = Get_Billing_Data_T3(CLI_Id,EMPs);
                    break;
                default:               
                   
                    break;
            }

            return concept;
        }
        private Invoice_Concepts Get_Billing_Data_T1(int CLI_Id, string Type_Id, int WAG_Id)
        {
            InvoicesManager invoicesManager = new InvoicesManager(_context);
            Invoice_Concepts concept = new Invoice_Concepts();
            if (!string.IsNullOrEmpty(Type_Id)&& Type_Id!="null")
            {
                if (Type_Id.Split("_")[0] == "service")
                {
                    int BillingType = (int)CRI_BILLING_TYPE.Service_Change_Basic;
                    var CRI_Billing_ServiceCharge = Type_Id.Split("_")[1];
                    concept = invoicesManager.Get_Billing_Data_T1(CLI_Id, WAG_Id, Convert.ToDouble(CRI_Billing_ServiceCharge), 0, BillingType);
                }
                else
                {
                    int BillingType = (int)CRI_BILLING_TYPE.Lump_Sum_Amount;
                    var CRI_Billing_Amount = Type_Id.Split("_")[1];
                    concept = invoicesManager.Get_Billing_Data_T1(CLI_Id, WAG_Id, 0, Convert.ToDecimal(CRI_Billing_Amount), BillingType);
                }
            }
            return concept;
        }

        private Invoice_Concepts Get_Billing_Data_T2_PF(int CLI_Id, int WAG_Id)
        {
            InvoicesManager invoicesManager = new InvoicesManager(_context);
            Invoice_Concepts concept = new Invoice_Concepts();
            concept = invoicesManager.Get_Billing_Data_T2_PF(CLI_Id, WAG_Id);
            return concept;
        }
        private Invoice_Concepts Get_Billing_Data_T2_ESIC(int CLI_Id, int WAG_Id)
        {
            InvoicesManager invoicesManager = new InvoicesManager(_context);
            Invoice_Concepts concept = new Invoice_Concepts();
            concept = invoicesManager.Get_Billing_Data_T2_ESIC(CLI_Id,WAG_Id);
            return concept;
        }
        private Invoice_Concepts Get_Billing_Data_T3(int CLI_Id, string EMPs)
        {
            InvoicesManager invoicesManager = new InvoicesManager(_context);
            Invoice_Concepts concept = invoicesManager.Get_Billing_Data_T3(CLI_Id, EMPs);
            return concept;
        }
    }
    
}