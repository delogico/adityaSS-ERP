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
                return _context.Invoices.Include(m => m.CLI_).ThenInclude(m => m.FRM_).Include(m => m.Invoice_Concepts).OrderByDescending(m => m.INV_Date).ToList();
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
            return _context.Invoices.Include(m => m.FRM_).ThenInclude(m => m.STA_).Include(m => m.CLI_).ThenInclude(m => m.STA_).Include(m => m.CLI_).ThenInclude(m => m.CITY_).Include(m => m.Invoice_Concepts).Where(m => m.INV_Id.Equals(INV_Id)).FirstOrDefault();
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
        public int AddInvoiceConcept(List<Invoice_Concepts> invoice_Concepts, int INV_Id)
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
                number = Convert.ToInt32(invoice.INV_Number) + 1;
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
            if (invoiceConcepts != null)
                _context.Invoice_Concepts.RemoveRange(invoiceConcepts);
            _context.SaveChanges();
        }

        public Invoice_Concepts Get_Billing_Data_T1(int CLI_Id, int WAG_Id, double CRI_Billing_ServiceCharge, decimal CRI_Billing_Amount, int BillingType)
        {
            ClientsManager clientsManager = new ClientsManager(_context);
            WageProcessManager processManager = new WageProcessManager(_context);
            Invoice_Concepts invoice_Concept = new Invoice_Concepts();
            Clients client = clientsManager.GetClientById(CLI_Id);
            List<Wage_Register> list = new List<Wage_Register>();
            decimal TotalServiceCharge = 0;
            if (BillingType == (int)ProjectUtils.CRI_BILLING_TYPE.Lump_Sum_Amount)
            {
                list = _context.Wage_Register.Include(m => m.WAG_).Include(m => m.CRI_).ThenInclude(m => m.DES_).Where(m => m.WAG_Id == WAG_Id && m.CLI_Id.Equals(CLI_Id) && m.CRI_.CRI_Billing_Type == BillingType && m.CRI_.CRI_Billing_Amount == CRI_Billing_Amount).ToList();
            }
            else if (BillingType == (int)ProjectUtils.CRI_BILLING_TYPE.Service_Change_Basic)
            {
                list = _context.Wage_Register.Include(m => m.WAG_).Include(m => m.CRI_).ThenInclude(m => m.DES_).Where(m => m.WAG_Id == WAG_Id && m.CLI_Id.Equals(CLI_Id) && m.CRI_.CRI_Billing_Type == BillingType && m.CRI_.CRI_Billing_ServiceCharge == CRI_Billing_ServiceCharge).ToList();
                if (list.Count() > 0)
                {
                    foreach(Wage_Register wage in list)
                    {
                        TotalServiceCharge = TotalServiceCharge + ProjectUtils.GetAmountBasedOnFormula_Report(
                            wage.CRI_.CRI_Billing_ServiceCharge_Formula,
                            wage.WAR_Basic_Calculated,wage.WAR_DA_Calculated,wage.WAR_HRA_Calculated,
                            wage.Wage_Register_Allowances.ToList(),wage.WAR_TotalWorkingDays,
                            wage.WAR_TotalPaybleDays,wage.WAR_OverTime_Calculated,
                            (wage.WAR_OutStation_Allowance_Calculated!=null? wage.WAR_OutStation_Allowance_Calculated.Value:0), 
                            (wage.WAR_Attendance_Allowance_Calculated!=null?wage.WAR_Attendance_Allowance_Calculated.Value:0),
                            (wage.WAR_Nightshift_Allowance_Calculated!=null?wage.WAR_Nightshift_Allowance_Calculated.Value:0),
                            (wage.WAR_Performance_Allowance_Calculated!=null?wage.WAR_Performance_Allowance_Calculated.Value:0));
                    }
                }
            }
            if (list.Count() > 0)
            {
                double TotalPaybleDays = list.Select(m => m.WAR_TotalPaybleDays).Sum();
                decimal MLWF = (client.CLI_MLWF_Contribution != null ? client.CLI_MLWF_Contribution.Value : 0);
                int Nos = list.Where(m => m.CRI_.DES_.DES_Exclude_LWF == false).Select(m => m.EMP_Id).Count();

                Wage_Process wage = list[0].WAG_;
                string DatePeriod = "";
                decimal Total = 0;
                DateTime StartDate = DateTime.Now;
                DateTime EndDate = DateTime.Now;
                int daysInMonth = 0;
                if (client.CLI_Att_MonthReal.Value)
                {
                    DatePeriod = "For The Month Of "+wage.WAG_Month.ToString("MMM-yyyy");
                    daysInMonth = DateTime.DaysInMonth(wage.WAG_Month.Year, wage.WAG_Month.Month);
                }
                else
                {
                    StartDate = new DateTime(wage.WAG_Month.Year, wage.WAG_Month.Month-1, client.CLI_Att_Month_Start.Value);
                    EndDate = new DateTime(wage.WAG_Month.Year, wage.WAG_Month.Month, client.CLI_Att_Month_End.Value);
                    DatePeriod ="From "+ StartDate.ToString("dd-MMM-yyyy") + " TO " + EndDate.ToString("dd-MMM-yyyy");
                    daysInMonth = (int)(EndDate - StartDate).TotalDays - 1;
                }
                StringBuilder sb = new StringBuilder();
                sb.Append("<b>Contract Receipt</b></br><i>");
                sb.AppendLine("Contract Bill For Providing Security Service</br>");
                if (BillingType == (int)ProjectUtils.CRI_BILLING_TYPE.Service_Change_Basic)
                {
                    decimal GrossTotal = ProjectUtils.RoundFigure(TotalServiceCharge); 
                    decimal ServiceCharge = ProjectUtils.RoundFigure(TotalServiceCharge * (decimal)CRI_Billing_ServiceCharge / 100);
                    Total = Total + GrossTotal + ServiceCharge;
                    sb.Append(DatePeriod + "</br>");
                    sb.Append("(A) Salary Wages = " + String.Format("{0:0.##}", ProjectUtils.RoundFigure(GrossTotal)) + "/-</br>");
                    string str = ProjectUtils.GetStringBasedOnFormula_Report(list[0].CRI_.CRI_Billing_ServiceCharge_Formula, list[0].Wage_Register_Allowances.ToList());
                    sb.Append(str + "<br/>");
                    sb.Append("(B) Service Charges @ "+ CRI_Billing_ServiceCharge + "%= " + String.Format("{0:0.##}", ProjectUtils.RoundFigure(ServiceCharge)) + "/-</br>");

                    if (wage.WAG_Month.Month == (int)ProjectUtils.Month.June || wage.WAG_Month.Month == (int)ProjectUtils.Month.December)
                    {
                        if (MLWF != 0)
                        {
                            decimal TotMLWF = Convert.ToDecimal(String.Format("{0:0.##}", ProjectUtils.RoundFigure((MLWF * Nos))));
                            sb.Append("(C) MLWF Contribution </br>");
                            sb.Append("[Rs." + String.Format("{0:0.##}", ProjectUtils.RoundFigure(MLWF)  + " x " + Nos + " Nos = " + TotMLWF + "/-]"));
                            Total = Total + TotMLWF;
                        }
                    }
                }
                else
                {                  
                    decimal SingleDay = CRI_Billing_Amount / daysInMonth;
                    Total = SingleDay * (decimal)TotalPaybleDays;


                    sb.Append(DatePeriod + "</br>");
                    sb.Append("(A) " + TotalPaybleDays + "Duties Of Facility Staff</br>");
                    sb.Append("@Rs. " + String.Format("{0:0.##}", ProjectUtils.RoundFigure(CRI_Billing_Amount)) + "PM = " + String.Format("{0:0.##}", ProjectUtils.RoundFigure(Total)) + "/-<br/>");

                    if (wage.WAG_Month.Month == (int)ProjectUtils.Month.June || wage.WAG_Month.Month == (int)ProjectUtils.Month.December)
                    {
                        if (MLWF != 0)
                        {
                            decimal TotMLWF = Convert.ToDecimal(String.Format("{0:0.##}", ProjectUtils.RoundFigure(MLWF * Nos)));
                            sb.Append("(C) MLWF Contribution </br>");
                            sb.Append("[Rs." + String.Format("{0:0.##}", ProjectUtils.RoundFigure(MLWF))  + " x " + Nos + " Nos = " + TotMLWF + "/-]");
                            Total = Total + TotMLWF;
                        }
                    }
                }
                sb.Append("</i>");

                invoice_Concept.INC_Description = sb.ToString();
                invoice_Concept.INC_Total =Convert.ToDecimal(String.Format("{0:0.##}", ProjectUtils.RoundFigure(Total)));
            }
            return invoice_Concept;
        }

        public Invoice_Concepts Get_Billing_Data_T2_PF(int CLI_Id, int WAG_Id)
        {
            ClientsManager clientsManager = new ClientsManager(_context);
            WageProcessManager processManager = new WageProcessManager(_context);
            Invoice_Concepts invoice_Concept = new Invoice_Concepts();
            Clients client = clientsManager.GetClientById(CLI_Id);

            List<Wage_Register> list = _context.Wage_Register.Include(m => m.WAG_).Include(m => m.CRI_).ThenInclude(m => m.DES_).Where(m => m.WAG_Id == WAG_Id && m.CLI_Id.Equals(CLI_Id)).ToList();


            decimal PF_Calculated = 0;
            StringBuilder sb = new StringBuilder();
            if (list.Count() > 0)
            {
                decimal ApplicableSalary = 0M;
                foreach (var item in list)
                {
                    List<Client_Requirement_Allowances> All = item.CRI_.Client_Requirement_Allowances.ToList();
                    decimal AppSalary = ProjectUtils.GetAmountBasedOnFormula_Report(
                        item.WAR_PF_Formula,
                        item.WAR_Basic_Calculated,
                        item.WAR_DA_Calculated,
                        item.WAR_HRA_Calculated, item.Wage_Register_Allowances.ToList(),
                        item.WAR_TotalWorkingDays,
                        item.WAR_TotalPaybleDays,
                        item.WAR_OverTime_Calculated,
                        (item.WAR_OutStation_Allowance_Calculated != null ? item.WAR_OutStation_Allowance_Calculated.Value : 0),
                        (item.WAR_Attendance_Allowance_Calculated != null ? item.WAR_Attendance_Allowance_Calculated.Value : 0),
                        (item.WAR_Nightshift_Allowance_Calculated != null ? item.WAR_Nightshift_Allowance_Calculated.Value : 0),
                        (item.WAR_Performance_Allowance_Calculated != null ? item.WAR_Performance_Allowance_Calculated.Value : 0));
                                        
                    ApplicableSalary = Math.Round(AppSalary + ApplicableSalary, MidpointRounding.AwayFromZero);
                }
                PF_Calculated = ApplicableSalary;

                Wage_Process wage = list[0].WAG_;
                string DatePeriod = "";
                DateTime StartDate = DateTime.Now;
                DateTime EndDate = DateTime.Now;
                if (client.CLI_Att_MonthReal.Value)
                {
                    DatePeriod = "For Month Of " + wage.WAG_Month.ToString("MMM-yyyy");
                }
                else
                {
                    StartDate = new DateTime(wage.WAG_Month.Year, wage.WAG_Month.Month, client.CLI_Att_Month_Start.Value);
                    EndDate = new DateTime(wage.WAG_Month.Year, wage.WAG_Month.Month, client.CLI_Att_Month_End.Value);
                    DatePeriod ="From "+ StartDate.ToString("dd-MMM-yyyy") + " TO " + EndDate.ToString("dd-MMM-yyyy"); ;
                }
                //decimal PF_Calculated1 = list.Select(m => m.WAR_PF_Calculated).Sum();
                //INC_Total = PF_Calculated;
                sb.Append("<b>Company Contribution Towards PF @" + client.CLI_PF_Employer_Cont_Rate + "%</b><i></br>");
                sb.Append("Rembursment Of Company Contribution <br/>");
                //sb.Append("To The P.F @" + 12 + "% and Other Charges @" + 1 + "%<br/>");
                //sb.Append("As Per The Act " + DatePeriod + "</br/>");
                sb.Append("(Total=" + String.Format("{0:0.##}", ProjectUtils.RoundFigure(PF_Calculated * (decimal)client.CLI_PF_Employer_Cont_Rate / 100)) + "/-)");
                sb.Append("</i>");
            }

            invoice_Concept.INC_Description = sb.ToString();
            invoice_Concept.INC_Total = Convert.ToDecimal(String.Format("{0:0.##}", ProjectUtils.RoundFigure(PF_Calculated * (decimal)client.CLI_PF_Employer_Cont_Rate / 100)));

            return invoice_Concept;
        }

        public Invoice_Concepts Get_Billing_Data_T2_ESIC(int CLI_Id, int WAG_Id)
        {
            ClientsManager clientsManager = new ClientsManager(_context);
            WageProcessManager processManager = new WageProcessManager(_context);
            Invoice_Concepts invoice_Concept = new Invoice_Concepts();
            Clients client = clientsManager.GetClientById(CLI_Id);

            List<Wage_Register> list = _context.Wage_Register.Include(m => m.WAG_).Include(m => m.CRI_).ThenInclude(m => m.DES_).Where(m => m.WAG_Id == WAG_Id && m.CLI_Id.Equals(CLI_Id)).ToList();
            decimal ESIC_Calculated = 0M;
            StringBuilder sb = new StringBuilder();
            if (list.Count() > 0)
            {
                decimal ApplicableSalary = 0M;
                decimal GrossTotal = list.Select(m => m.WAR_GrossTotal).Sum();
                foreach (var item in list)
                {                   
                    decimal AppSalary = ProjectUtils.GetAmountBasedOnFormula_Report(
                        item.WAR_ESIC_Formula,
                        Math.Round(item.WAR_Basic_Calculated, MidpointRounding.AwayFromZero),
                        Math.Round(item.WAR_DA_Calculated, MidpointRounding.AwayFromZero),
                        Math.Round(item.WAR_HRA_Calculated, MidpointRounding.AwayFromZero),
                         item.Wage_Register_Allowances.ToList(),
                        item.WAR_TotalWorkingDays,
                        item.WAR_TotalPaybleDays,
                        Math.Round(item.WAR_OverTime_Calculated, MidpointRounding.AwayFromZero),
                        (item.WAR_OutStation_Allowance_Calculated != null ? Math.Round(item.WAR_OutStation_Allowance_Calculated.Value, MidpointRounding.AwayFromZero) : 0),
                        (item.WAR_Attendance_Allowance_Calculated != null ? Math.Round(item.WAR_Attendance_Allowance_Calculated.Value, MidpointRounding.AwayFromZero) : 0),
                        (item.WAR_Nightshift_Allowance_Calculated != null ? Math.Round(item.WAR_Nightshift_Allowance_Calculated.Value, MidpointRounding.AwayFromZero) : 0),
                        (item.WAR_Performance_Allowance_Calculated != null ? Math.Round(item.WAR_Performance_Allowance_Calculated.Value, MidpointRounding.AwayFromZero) : 0));

                    ApplicableSalary = Math.Round(AppSalary + ApplicableSalary, MidpointRounding.AwayFromZero);
                }
                ESIC_Calculated = ApplicableSalary;
                Wage_Process wage = list[0].WAG_;
                string DatePeriod = "";
                DateTime StartDate = DateTime.Now;
                DateTime EndDate = DateTime.Now;
                if (client.CLI_Att_MonthReal.Value)
                {
                    DatePeriod = "For The Month Of " + wage.WAG_Month.ToString("MMM-yyyy");
                }
                else
                {
                    StartDate = new DateTime(wage.WAG_Month.Year, wage.WAG_Month.Month, client.CLI_Att_Month_Start.Value);
                    EndDate = new DateTime(wage.WAG_Month.Year, wage.WAG_Month.Month, client.CLI_Att_Month_End.Value);
                    DatePeriod = "From "+StartDate.ToString("dd-MMM-yyyy") + " TO " + EndDate.ToString("dd-MMM-yyyy"); ;
                }                       
                sb.Append("<b>Company Contribution Towards ESIC @" + client.CLI_ESIC_Employer_Cont_Rate + "%</b><i></br>");
                sb.Append("Rembursment Of Company Contribution <br/>");
                sb.Append("On Gross Salary= Rs." + String.Format("{0:0.##}", ProjectUtils.RoundFigure(GrossTotal)  + "/-<br/>"));
                sb.Append("@" + client.CLI_ESIC_Employer_Cont_Rate + "% = " + String.Format("{0:0.##}", ProjectUtils.RoundFigure(ESIC_Calculated * (decimal)client.CLI_ESIC_Employer_Cont_Rate / 100)));
                //sb.Append("As Per The Act " + DatePeriod + "<br/>");
                sb.Append("</i>");
            }

            invoice_Concept.INC_Description = sb.ToString();
            invoice_Concept.INC_Total = Convert.ToDecimal(String.Format("{0:0.##}", ProjectUtils.RoundFigure(ESIC_Calculated * (decimal)client.CLI_ESIC_Employer_Cont_Rate / 100)));

            return invoice_Concept;
        }

        public Invoice_Concepts Get_Billing_Data_T3(int CLI_Id, string EMPs)
        {
            Invoice_Concepts invoice_Concept = new Invoice_Concepts();
            if (EMPs != null)
            {
                string[] EMP = EMPs.Split(",");
                ClientsManager clientsManager = new ClientsManager(_context);                
                Clients client = clientsManager.GetClientById(CLI_Id);
                List<Wage_Register> list = _context.Wage_Register.Include(m => m.WAG_).Include(m => m.EMP_).Include(m => m.CRI_).ThenInclude(m => m.DES_).Where(m => m.CLI_Id.Equals(CLI_Id)).ToList();
                StringBuilder sb = new StringBuilder();
                sb.Append("<b>Contract Receipt</b></br><i>");
                sb.AppendLine("Full & Final Settlement Of Left Employee</br>");
                decimal INC_Total = 0M;
                if (list.Count() > 0)
                {
                    foreach (var item in EMP)
                    {
                        var wag = list.Where(m => m.EMP_Id.ToString().Equals(item.ToString())).FirstOrDefault();
                        if (wag != null)
                        {
                            sb.Append("<b>" + (wag.EMP_.EMP_Gender.Equals(ProjectUtils.Gender.Female) ? "Miss. " : "Mr. ") + wag.EMP_.EMP_FirstName + " " + wag.EMP_.EMP_MiddleName + " " + wag.EMP_.EMP_SurName + "</br></b>");
                            sb.Append("Leave Encashment = <br/>");
                            sb.Append("Bonus =   <br/>");
                            sb.Append("Total = <br/>");
                        }
                    }
                }

                sb.Append("</i>");
                invoice_Concept.INC_Total = INC_Total;
                invoice_Concept.INC_Description = sb.ToString();
            }
            return invoice_Concept;
        }


    }
}
