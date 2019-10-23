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

namespace RMERP.Controllers
{
    public class InvoicesController : Controller
    {
        private readonly RMERPContext _context;
        private IHostingEnvironment _hostingEnvironment;
        public InvoicesController(RMERPContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {            
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            InvoicesManager invoicesManager = new InvoicesManager(_context);                      
            InvoiceMasterVM masterVM = new InvoiceMasterVM();
            int FRM_Id = (sessionUtils.GetLoggedFirmID()!=null? sessionUtils.GetLoggedFirmID().Value:0);           
            List<Invoices> invoices = invoicesManager.GetInvoices(FRM_Id);
            masterVM.InvoiceVMs = InvoiceMapper.mapMe(invoices.Where(m => DateTime.Compare(m.INV_Date.Date, DateTime.Now.Date.AddMonths(-2)) > 0).ToList());
            ViewBag.ClientList = invoices.Select(m => m.CLI_).Distinct().ToList();
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
            ViewBag.Clients = clientsManager.listClients(true, sessionUtils.GetLoggedFirmID());
            InvoiceVM invoiceVM = new InvoiceVM();
            invoiceVM.INV_Number = invoicesManager.GetNextInvoiceNumber();
            invoiceVM.INV_ClientOrder_Date = DateNow();
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
        public JsonResult AddEditInvoice([FromBody]InvoiceVM InvoiceVM)
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
        public ActionResult GetInvoiceTemplate(int CLI_Id)
        {
            ClientsManager clientsManager = new ClientsManager(_context);
            WageRegisterManager registerManager = new WageRegisterManager(_context);
            WageProcessManager wagManager = new WageProcessManager(_context);
            IEnumerable<INVOICE_TEMPLATE_TYPE> INVOICE_TEMPLATE_TYPE = Enum.GetValues(typeof(INVOICE_TEMPLATE_TYPE))
                                                       .Cast<INVOICE_TEMPLATE_TYPE>();

            ViewBag.InvoiceType = from action in INVOICE_TEMPLATE_TYPE
                                  select new SelectListItem
                                  {
                                      Text = ProjectUtils.GetStringValue(action),
                                      Value = ((int)action).ToString()
                                  };

            int FRM_Id = clientsManager.GetClientById(CLI_Id).FRM_Id;
            var cc = wagManager.getWageProcessList(FRM_Id);
            ViewBag.Months = from c in cc
                             select new SelectListItem
                             {
                                 Text = c.WAG_Month.ToShortDateString(),
                                 Value = c.WAG_Id.ToString()
                             };
            ViewBag.Designation = registerManager.GetWageRegistersForInvoice(CLI_Id).Select(m => m.CRI_.DES_).Distinct();
            return PartialView("_InvoiceType");
        }
        public IActionResult DownloadInvoice(int INV_Id,int ActionId)
        {
            InvoicesManager invoicesManager = new InvoicesManager(_context);
            Invoices invoices = invoicesManager.GetInvoice(INV_Id);
            if (ActionId == (int)ProjectUtils.PDFAction.View)
            {
                return View(invoices);
            }
            else 
            {
                var view = new ViewAsPdf(invoices)
                {
                    FileName = "Invoice_" + invoices.INV_Number + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + ".pdf",
                };
                return view;
            }
        }


        public IActionResult AddEditInvoice1(int INV_Id = 0)
        {
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            ClientsManager clientsManager = new ClientsManager(_context);
            InvoicesManager invoicesManager = new InvoicesManager(_context);
            ViewBag.Clients = clientsManager.listClients(true, sessionUtils.GetLoggedFirmID());
            InvoiceVM invoiceVM = new InvoiceVM();
            invoiceVM.INV_Number = invoicesManager.GetNextInvoiceNumber();
            invoiceVM.INV_ClientOrder_Date = ProjectUtils.DateNow();
            invoiceVM.INV_Date = ProjectUtils.DateNow();
            invoiceVM.Invoice_Concepts = new List<Invoice_ConceptsVM>();
            if (INV_Id > 0)
            {
                Invoices invoice = invoicesManager.GetInvoice(INV_Id);
                invoiceVM = InvoiceMapper.mapMe(invoice);
                invoiceVM.totTAX = invoice.INV_IGST_Total.Value + invoice.INV_CGST_Total.Value + invoice.INV_SGST_Total.Value;
                invoiceVM.All_INC_Total = invoice.Invoice_Concepts.Sum(m => m.INC_Total);
            }
            IEnumerable<INVOICE_TEMPLATE_TYPE> INVOICE_TEMPLATE_TYPE = Enum.GetValues(typeof(INVOICE_TEMPLATE_TYPE))
                                                       .Cast<INVOICE_TEMPLATE_TYPE>();
            ViewBag.InvoiceType = from action in INVOICE_TEMPLATE_TYPE
                                  select new SelectListItem
                                  {
                                      Text = ProjectUtils.GetStringValue(action),
                                      Value = ((int)action).ToString()
                                  };
            return View(invoiceVM);
        }
        [HttpPost]
        public ActionResult AddEditInvoice1(InvoiceVM InvoiceVM)
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
                TempData["message"] = "Try Again";
            }
            return RedirectToAction("Index","Invoices");            
        }

        public ActionResult SearchInvoice(int CLI_Id)
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
                invoiceVMs = InvoiceMapper.mapMe(invoicesManager.GetInvoices(FRM_Id).Where(m => DateTime.Compare(m.INV_Date.Date, DateTime.Now.Date.AddMonths(-2)) > 0).ToList());
            }
            
            return PartialView("_InvoiceList", invoiceVMs);
        }

        public Invoice_Concepts GetInvoiceData(int CLI_Id,int Type,int WAG_Id,int DES_Id)
        {
            InvoicesManager invoicesManager = new InvoicesManager(_context);
            Invoice_Concepts concept = new Invoice_Concepts();
            switch (Type)
            {
                case (int)INVOICE_TEMPLATE_TYPE.MONTHLY_TOTAL_DAYS_FROM_OUR_EMPLOYEES : //MONTHLY_TOTAL_DAYS_FROM_OUR_EMPLOYEES
                    concept= invoicesManager.GetTotalDaysInvoiceData(CLI_Id, WAG_Id, DES_Id);
                    break;
                default:
                    concept = invoicesManager.GetTotalDaysInvoiceData(CLI_Id, WAG_Id, DES_Id);
                    concept.INC_Description = "Default";
                    break;
            }
            return concept;
        }
       
    }

}