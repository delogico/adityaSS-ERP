using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models;

public partial class Client
{
    public int CLI_Id { get; set; }

    public int FRM_Id { get; set; }

    public string CLI_Name { get; set; }

    /// <summary>
    /// 1: International; 2: Domestic
    /// </summary>
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

    public bool CLI_IsActive { get; set; }

    public DateTime? CLI_InActivatedOn { get; set; }

    public int? ADM_Id_InactivatedBy { get; set; }

    /// <summary>
    /// 0:Consider_RealDays,1:Excluding_WeeklyOff,2:Reduce_StaticDays
    /// </summary>
    public byte CLI_Total_WorkingDays { get; set; }

    public int? CLI_No_Reduce_Days { get; set; }

    public int CLI_WorkingHours_In_Day { get; set; }

    public string CLI_Invoicing_Name { get; set; }

    public string CLI_Invoicing_Address1 { get; set; }

    public string CLI_Invoicing_Address2 { get; set; }

    public string CLI_Invoicing_City { get; set; }

    public string CLI_Invoicing_ZipCode { get; set; }

    public string CLI_Invoicing_Location { get; set; }

    public bool? CLI_IsIGST { get; set; }

    public double? CLI_IGST { get; set; }

    public bool? CLI_IsCGST { get; set; }

    public double? CLI_CGST { get; set; }

    public bool? CLI_IsSGST { get; set; }

    public double? CLI_SGST { get; set; }

    public string CLI_GST_Info { get; set; }

    public string CLI_Place_Of_Supply { get; set; }

    public double? CLI_PF_Employer_Cont_Rate { get; set; }

    public double CLI_EPF_Rate { get; set; }

    public double CLI_EPS_Rate { get; set; }

    public double? CLI_ESIC_Employer_Cont_Rate { get; set; }

    public int STA_Id { get; set; }

    public decimal? CLI_MLWF_Contribution { get; set; }

    public virtual ICollection<Attendance_Summary> Attendance_Summaries { get; set; } = new List<Attendance_Summary>();

    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    public virtual City CITY { get; set; }

    public virtual ICollection<Client_ActivationHistory> Client_ActivationHistories { get; set; } = new List<Client_ActivationHistory>();

    public virtual ICollection<Client_Contact> Client_Contacts { get; set; } = new List<Client_Contact>();

    public virtual ICollection<Client_Requirement> Client_Requirements { get; set; } = new List<Client_Requirement>();

    public virtual ICollection<Clients_Employee> Clients_Employees { get; set; } = new List<Clients_Employee>();

    public virtual Firm FRM { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual State STA { get; set; }

    public virtual ICollection<Wage_Process_Client> Wage_Process_Clients { get; set; } = new List<Wage_Process_Client>();

    public virtual ICollection<Wage_Register> Wage_Registers { get; set; } = new List<Wage_Register>();
}
