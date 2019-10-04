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
            int FRM_Id = (sessionUtils.GetLoggedFirmID() != null ? sessionUtils.GetLoggedFirmID().Value : 0);
            return View(invoicesManager.GetInvoices(FRM_Id));
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
            InvoicesManager invoicesManager = new InvoicesManager(_context);
            ViewBag.Clients = clientsManager.listClients(true, sessionUtils.GetLoggedFirmID());
            InvoiceVM invoiceVM = new InvoiceVM();
            invoiceVM.INV_Number = invoicesManager.GetNextInvoiceNumber();
            invoiceVM.INV_ClientOrder_Date = ProjectUtils.DateNow();
            invoiceVM.INV_Date = ProjectUtils.DateNow();
            invoiceVM.Invoice_Concepts = new List<Invoice_Concepts>();
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
                foreach (Invoice_Concepts invoice_Concept in InvoiceVM.Invoice_Concepts)
                {
                    invoice_Concept.INV_Id = INV_Id;
                    Invoice_Concepts.Add(invoice_Concept);
                }
                i = invoicesManager.AddInvoiceConcept(Invoice_Concepts, INV_Id);
            }
            catch (Exception)
            {
                TempData["message"] = "Try Again";
            }
            //return RedirectToAction("Index","Invoices");
            return Json(i);
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
            invoiceVM.Invoice_Concepts = new List<Invoice_Concepts>();
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
                foreach (Invoice_Concepts invoice_Concept in InvoiceVM.Invoice_Concepts)
                {
                    invoice_Concept.INV_Id = INV_Id;
                    Invoice_Concepts.Add(invoice_Concept);
                }
                i = invoicesManager.AddInvoiceConcept(Invoice_Concepts, INV_Id);
            }
            catch (Exception)
            {
                TempData["message"] = "Try Again";
            }
            return RedirectToAction("Index","Invoices");            
        }
    }

}