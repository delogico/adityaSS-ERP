using Microsoft.EntityFrameworkCore;
using RMERP.DAL.Helpers;
using RMERP.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMERP.DAL.ManagerClasses
{
    public class InvoicesManager
    {
        RMERPContext _context;
        public InvoicesManager(RMERPContext context)
        {
            _context = context;
        }
        public List<Invoices> GetInvoices(int FRM_Id)
        {
            if (FRM_Id == 0)
            {
                return _context.Invoices.Include(m=>m.CLI_).ThenInclude(m=>m.FRM_).Include(m => m.Invoice_Concepts).OrderByDescending(m=>m.INV_Date).ToList();
            }
            else
            {
                return _context.Invoices.Include(m => m.CLI_).ThenInclude(m => m.FRM_).Include(m => m.Invoice_Concepts).Where(m => m.FRM_Id.Equals(FRM_Id)).OrderByDescending(m => m.INV_Date).ToList();
            }            
        }

        public List<Invoices> GetInvoicesByClientId(int CLI_Id)
        {           
                return _context.Invoices.Include(m => m.CLI_).ThenInclude(m => m.FRM_).Include(m => m.Invoice_Concepts).Where(m => m.CLI_Id.Equals(CLI_Id)).OrderByDescending(m => m.INV_Date).ToList();           
        }

        public Invoices GetInvoice(int INV_Id)
        {
            return _context.Invoices.Include(m=>m.FRM_).ThenInclude(m=>m.STA_).Include(m=>m.CLI_).ThenInclude(m=>m.STA_).Include(m => m.CLI_).ThenInclude(m=>m.CITY_).Include(m=>m.Invoice_Concepts).Where(m=>m.INV_Id.Equals(INV_Id)).FirstOrDefault();
        }
        public int AddEditInvoice(Invoices invoice)
        {            
            if (invoice.INV_Id > 0)
            {
                _context.Invoices.Update(invoice);
            }
            else
            {
                invoice.INV_CreatedOn = ProjectUtils.DateNow();
                _context.Invoices.Add(invoice);
            }           
            _context.SaveChanges();
            return invoice.INV_Id;
        }
        public int AddInvoiceConcept(List<Invoice_Concepts> invoice_Concepts,int INV_Id)
        {
            List<Invoice_Concepts> invoiceConcepts = _context.Invoice_Concepts.Where(m => m.INV_Id.Equals(INV_Id)).ToList();
            _context.Invoice_Concepts.RemoveRange(invoiceConcepts);
            _context.Invoice_Concepts.AddRange(invoice_Concepts);
            _context.SaveChanges();
            return invoice_Concepts.Count();
        }
        public string GetNextInvoiceNumber()
        {
            string InvoiceNumber = string.Empty;
            int number = 1;
            Invoices invoice = _context.Invoices.OrderByDescending(m => m.INV_Id).FirstOrDefault();
            if (invoice != null)
            {
                number = Convert.ToInt32(invoice.INV_Number)+1;
            }
            InvoiceNumber = number.ToString("D6");
            return InvoiceNumber;
        }

        public void DeleteInvoice(int INV_Id)
        {
            _context.Invoices.Remove(_context.Invoices.Find(INV_Id));
            _context.SaveChanges();
        }

        public void DeleteIncidenceConcepts(int INV_Id)
        {
            List<Invoice_Concepts> invoiceConcepts = _context.Invoice_Concepts.Where(m => m.INV_Id.Equals(INV_Id)).ToList();
            if(invoiceConcepts!=null)
                _context.Invoice_Concepts.RemoveRange(invoiceConcepts);
            _context.SaveChanges();
        }

        public Invoice_Concepts GetTotalDaysInvoiceData(int CLI_Id,int WAG_Id, int DES_Id)
        {
            ClientsManager clientsManager = new ClientsManager(_context);
            DesignationManager designationManager = new DesignationManager(_context);
            Invoice_Concepts invoice_Concept = new Invoice_Concepts();
            string DES_Title = designationManager.GetDesignationById(DES_Id).DES_Title;
            var list = _context.Wage_Register.Include(M=>M.CRI_).Where(m =>m.WAG_Id.Equals(WAG_Id) && m.CLI_Id.Equals(CLI_Id) && m.CRI_.DES_Id.Equals(DES_Id));
            double totalPayableDay = 0;
            decimal totalAmount = 0M;
            Clients client = clientsManager.GetClientById(CLI_Id);
            foreach (Wage_Register wage_Register in list.ToList())
            {
                totalPayableDay = totalPayableDay + wage_Register.WAR_TotalPaybleDays;
                totalAmount = totalAmount + wage_Register.WAR_FinalTotal;
            }
            invoice_Concept.INC_Description ="<b>"+client.CLI_Name+ "</b><br/>Post : <b>"+ DES_Title + "</b><br/>Payable days : "+ totalPayableDay + "<br/>Employees : "+ list.Select(m=>m.EMP_Id).Count();
            invoice_Concept.INC_Total = Convert.ToDecimal(totalPayableDay * list.Select(m => m.EMP_Id).Count());
            return invoice_Concept;
        }

        public Invoice_Concepts Get_Billing_Data_T1(int CLI_Id, int WAG_Id, double CRI_Billing_ServiceCharge,decimal CRI_Billing_Amount, int BillingType)
        {
            ClientsManager clientsManager = new ClientsManager(_context);
            WageProcessManager processManager = new WageProcessManager(_context);            
            Invoice_Concepts invoice_Concept = new Invoice_Concepts();
            Clients client = clientsManager.GetClientById(CLI_Id);
            List<Wage_Register> list = _context.Wage_Register.Include(m=>m.WAG_).Include(m => m.CRI_).ThenInclude(m=>m.DES_).Where(m => m.WAG_Id==WAG_Id && m.CLI_Id.Equals(CLI_Id) && m.CRI_.CRI_Billing_Type==BillingType && m.CRI_.CRI_Billing_ServiceCharge==CRI_Billing_ServiceCharge).ToList();
            decimal INC_Total = list.Select(m => m.WAR_FinalTotal).Sum();
            decimal HRA = (INC_Total * 5)/ 100;
            decimal ServiceCharge = (INC_Total * Convert.ToDecimal(CRI_Billing_ServiceCharge)) / 100;
            decimal MLWF = (client.CLI_MLWF_Contribution!=null? client.CLI_MLWF_Contribution.Value : 0);
            int Nos = list.Where(m=>m.CRI_.DES_.DES_Exclude_LWF==false).Select(m => m.EMP_Id).Count();

            Wage_Process wage = list[0].WAG_;
            DateTime startDate = DateTime.Now;
            DateTime EndDate = DateTime.Now;
            if (client.CLI_Att_MonthReal.Value)
            {
                //startDate= client.CLI_Att_Month_Start.
                //Real
            }
            else
            {
                //Custmize
            }
            

            StringBuilder sb = new StringBuilder();
            sb.Append("<b>Contract Receipt</b></br><i>");
            sb.AppendLine("Contract Bill For Providing Security Service</br>");
            if (BillingType == (int)ProjectUtils.CRI_BILLING_TYPE.Service_Change_Basic)
            {
                sb.Append("For The Month Of " + wage.WAG_Month.ToString("MMM-yyyy") + "</br>");
                sb.Append("(A) Salary Wages Including HRA @5% = " + HRA + "/-</br>");
                sb.Append("Extra Work Wages and Upkeep Allowances");
                sb.Append("(B) Service Charges @ 12%= " + ServiceCharge + "/-</br>");

                if (wage.WAG_Month.Month == (int)ProjectUtils.Month.June || wage.WAG_Month.Month == (int)ProjectUtils.Month.December)
                {
                    if (MLWF != 0)
                    {
                        sb.Append("(C) MLWF Contribution </br>");
                        sb.Append("[Rs." + MLWF + " x " + Nos + " Nos = " + (MLWF * Nos) + "/-]");
                    }
                }

                sb.Append("</i>");
            }
            else
            {
                
               
                sb.Append("From");
            }
            

            invoice_Concept.INC_Description =sb.ToString();
            invoice_Concept.INC_Total = INC_Total;
            return invoice_Concept;
        }


    }
}
