using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMERP.DAL.Mappers
{
    public class InvoiceMapper
    {
        public static Invoices mapMeModel(InvoiceVM invoiceVM)
        {
            Invoices Invoice = new Invoices();
            Invoice.INV_Id = invoiceVM.INV_Id;
            Invoice.FRM_Id = invoiceVM.FRM_Id;
            Invoice.CLI_Id = invoiceVM.CLI_Id;
            Invoice.INV_Number = invoiceVM.INV_Number;
            Invoice.INV_Date = invoiceVM.INV_Date;
            Invoice.INV_ClientOrder_Number = invoiceVM.INV_ClientOrder_Number;
            Invoice.INV_ClientOrder_Date = invoiceVM.INV_ClientOrder_Date;
            if (invoiceVM.INV_ClientOrder_Date == DateTime.MinValue)
            {
                Invoice.INV_ClientOrder_Date = null;
            }

            Invoice.INV_HSN = invoiceVM.INV_HSN;
            Invoice.INV_Total = invoiceVM.INV_Total;
            Invoice.ADM_Id_CreatedBy = invoiceVM.ADM_Id_CreatedBy;
            Invoice.INV_CreatedOn = invoiceVM.INV_CreatedOn;
            Invoice.INV_CGST_Percentage = invoiceVM.INV_CGST_Percentage;
            Invoice.INV_SGST_Percentage = invoiceVM.INV_SGST_Percentage;
            Invoice.INV_IGST_Percentage = invoiceVM.INV_IGST_Percentage;
            Invoice.INV_CGST_Total = invoiceVM.INV_CGST_Total;
            Invoice.INV_SGST_Total = invoiceVM.INV_SGST_Total;
            Invoice.INV_IGST_Total = invoiceVM.INV_IGST_Total;
            Invoice.INV_Remark = invoiceVM.INV_Remark;
            return Invoice;
        }
        public static InvoiceVM mapMe(Invoices invoice)
        {
            InvoiceVM InvoiceVM = new InvoiceVM();
            InvoiceVM.INV_Id = invoice.INV_Id;
            InvoiceVM.FRM_Id = invoice.FRM_Id;
            InvoiceVM.CLI_Id = invoice.CLI_Id;
            InvoiceVM.INV_Number = invoice.INV_Number;
            InvoiceVM.INV_Date = invoice.INV_Date;
            InvoiceVM.INV_ClientOrder_Number = invoice.INV_ClientOrder_Number;
            
            if (invoice.INV_ClientOrder_Date == DateTime.MinValue)
            {
                InvoiceVM.INV_ClientOrder_Date = null;
            }
            else if(invoice.INV_ClientOrder_Date!=null)
            {
                InvoiceVM.INV_ClientOrder_Date = invoice.INV_ClientOrder_Date.Value.Date;
            }

            InvoiceVM.INV_HSN = invoice.INV_HSN;
            InvoiceVM.INV_Total = invoice.INV_Total;
            InvoiceVM.ADM_Id_CreatedBy = invoice.ADM_Id_CreatedBy;

            
            InvoiceVM.INV_CreatedOn = invoice.INV_CreatedOn.Date;
            InvoiceVM.INV_CGST_Percentage = invoice.INV_CGST_Percentage;
            InvoiceVM.INV_SGST_Percentage = invoice.INV_SGST_Percentage;
            InvoiceVM.INV_IGST_Percentage = invoice.INV_IGST_Percentage;
            InvoiceVM.INV_Remark = invoice.INV_Remark;
            if (invoice.INV_CGST_Total!=null)
                InvoiceVM.INV_CGST_Total = invoice.INV_CGST_Total.Value;
            if (invoice.INV_SGST_Total != null)
                InvoiceVM.INV_SGST_Total = invoice.INV_SGST_Total.Value;
            if (invoice.INV_IGST_Total != null)
                InvoiceVM.INV_IGST_Total = invoice.INV_IGST_Total.Value;
            if (invoice.Invoice_Concepts != null)
                InvoiceVM.Invoice_Concepts = InvoiceConceptsMapper.mapMe(invoice.Invoice_Concepts.ToList());
            if (invoice.CLI_ != null)
                InvoiceVM.CLI_ = invoice.CLI_;
            if (invoice.FRM_ != null)
                InvoiceVM.FRM_ = invoice.FRM_;
            return InvoiceVM;
        }

        public static List<InvoiceVM> mapMe(List<Invoices> invoices)
        {
            List<InvoiceVM> invoiceVMs = new List<InvoiceVM>();
            foreach(Invoices invoice in invoices)
            {
                invoiceVMs.Add(mapMe(invoice));
            }
            return invoiceVMs;
        }
    }
}
