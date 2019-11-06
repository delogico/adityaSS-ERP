using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models
{
    public partial class Clients
    {
        public Clients()
        {
            Attendance = new HashSet<Attendance>();
            Client_Contacts = new HashSet<Client_Contacts>();
            Client_Requirements = new HashSet<Client_Requirements>();
            Clients_Employees = new HashSet<Clients_Employees>();
            Invoices = new HashSet<Invoices>();
            Wage_Process_Clients = new HashSet<Wage_Process_Clients>();
            Wage_Register = new HashSet<Wage_Register>();
        }

        public int CLI_Id { get; set; }
        public int FRM_Id { get; set; }
        public string CLI_Name { get; set; }
        public byte CLI_International_Domestic { get; set; }
        public string CLI_Address { get; set; }
        public int CITY_Id { get; set; }
        public string CLI_Pincode { get; set; }
        public string CLI_Phone { get; set; }
        public string CLI_Fax { get; set; }
        public string CLI_Email { get; set; }
        public string CLI_Email_2 { get; set; }
        public string CLI_GST_Number { get; set; }
        public int CLI_GST_Rate { get; set; }
        public string CLI_HSN_Code { get; set; }
        public int CLI_TDS_Rate { get; set; }
        public int ADM_Id_RegisterBy { get; set; }
        public DateTime CLI_RegisteredOn { get; set; }
        public string CLI_Logo { get; set; }
        public bool? CLI_IsActive { get; set; }
        public DateTime? CLI_InActivatedOn { get; set; }
        public int? ADM_Id_InactivatedBy { get; set; }
        public bool? CLI_Att_MonthReal { get; set; }
        public int? CLI_Att_Month_Start { get; set; }
        public int? CLI_Att_Month_End { get; set; }
        public byte CLI_Total_WorkingDays { get; set; }
        public int? CLI_No_Reduce_Days { get; set; }
        public int CLI_WorkingHours_In_Day { get; set; }
        public string CLI_Invoicing_Name { get; set; }
        public string CLI_Invoicing_Address1 { get; set; }
        public string CLI_Invoicing_Address2 { get; set; }
        public string CLI_Invoicing_City { get; set; }
        public int? CLI_Invoicing_ZipCode { get; set; }
        public string CLI_Invoicing_Location { get; set; }
        public bool? CLI_IsIGST { get; set; }
        public double? CLI_IGST { get; set; }
        public bool? CLI_IsCGST { get; set; }
        public double? CLI_CGST { get; set; }
        public bool? CLI_IsSGST { get; set; }
        public double? CLI_SGST { get; set; }
        public string CLI_Place_Of_Supply { get; set; }
        public double? CLI_PF_Employer_Cont_Rate { get; set; }
        public double CLI_EPF_Rate { get; set; }
        public double CLI_EPS_Rate { get; set; }
        public double? CLI_ESIC_Employer_Cont_Rate { get; set; }
        public int STA_Id { get; set; }
        public decimal? CLI_MLWF_Contribution { get; set; }

        public Cities CITY_ { get; set; }
        public Firms FRM_ { get; set; }
        public States STA_ { get; set; }
        public ICollection<Attendance> Attendance { get; set; }
        public ICollection<Client_Contacts> Client_Contacts { get; set; }
        public ICollection<Client_Requirements> Client_Requirements { get; set; }
        public ICollection<Clients_Employees> Clients_Employees { get; set; }
        public ICollection<Invoices> Invoices { get; set; }
        public ICollection<Wage_Process_Clients> Wage_Process_Clients { get; set; }
        public ICollection<Wage_Register> Wage_Register { get; set; }
    }
}
