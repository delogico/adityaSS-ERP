using Microsoft.AspNetCore.Http;
using RMERP.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;


namespace RMERP.DAL.ViewModel
{
    public class ClientsViewModel
    {
        public ClientsModel clientsModel { get; set; }
        public ParametersClientsModel ParametersClientsModel { get; set; }
        public IEnumerable<Clients> Listclients { get; set; }
        public List<ClientContactVM> contacts { get; set; }
        public List<ClientRequirementVM> requirements { get; set; }
        public List<ClientEmployeeVM> employees { get; set; }        
        public int WageID { get; set; }
      
    }
    public class ClientsModel
    {
        [Key]
        public int CLI_Id { get; set; }
        [Display(Name = "Firm")]
        [Required(ErrorMessage = "Please select firm")]
        public int FRM_Id { get; set; }
        [Display(Name = "Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter name")]
        public string CLI_Name { get; set; }
        [Display(Name = "Domain")]
        [Required(ErrorMessage = "Please choose country domain")]
        public byte CLI_International_Domestic { get; set; }
        [Display(Name = "Address")]
        [Required(ErrorMessage = "Please enter address")]
        public string CLI_Address { get; set; }
        [Display(Name = "City")]
        [Required(ErrorMessage = "Please select city")]
        public int CITY_Id { get; set; }
        [Display(Name = "Pincode")]
        [Required(ErrorMessage = "Please enter pincode")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Pincode should have 6 digit ")]
        public string CLI_Pincode { get; set; }
        [Display(Name = "Phone")]
        [Required(ErrorMessage = "Please enter phone")]
        public string CLI_Phone { get; set; }
        [Display(Name = "Fax")]
        public string CLI_Fax { get; set; }
        [Display(Name = "Primary email")]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Please enter email")]
        public string CLI_Email { get; set; }
        [Display(Name = "Secondary email")]
        [EmailAddress]
        public string CLI_Email_2 { get; set; }
        [Display(Name = "GST no")]
        public string CLI_GST_Number { get; set; }
        [Display(Name = "GST rate")]
        public int CLI_GST_Rate { get; set; }
        [Display(Name = "HSN code")]
        public string CLI_HSN_Code { get; set; }
        [Display(Name = "TDS rate")]
        public int CLI_TDS_Rate { get; set; }
        public int ADM_Id_RegisterBy { get; set; }
        public DateTime CLI_RegisteredOn { get; set; }
        public bool? CLI_IsActive { get; set; }

        [Display(Name = "Logo")]
        public IFormFile CLI_Logo { get; set; }
        public string CliLogoImage { get; set; }
        public int totalEmployee { get; set; }
        [Display(Name = "Select Working Days Count Method")]
        [Required(ErrorMessage = "Please select working days count method")]
        public byte CLI_Total_WorkingDays { get; set; }
        [Display(Name = "Number Of Reduce Days")]
        [Required(ErrorMessage = "Add Reduce Days")]
        public int? CLI_No_Reduce_Days { get; set; } = 1;
    }
    public class ParametersClientsModel
    {
        public ClientsModel clientsModel { get; set; }
        public bool? CLI_Att_MonthReal { get; set; }
        public int? CLI_Att_Month_Start { get; set; }
        public int? CLI_Att_Month_End { get; set; }
    }
}
