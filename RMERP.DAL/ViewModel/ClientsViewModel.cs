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
        public IEnumerable<ClientContacts> ListClientContact { get; set; }
        public ClientRequirementsModel clientRequirementsModel { get; set; }
        public IEnumerable<ClientRequirements> ListClientRequirements { get; set; }
        public ClientsEmployeesViewModel ClientsEmployeesViewModel { get; set; }
        public IEnumerable<ClientsEmployees> ClientsEmployeesList { get; set; }
        public int WageID { get; set; }
    }
    public class ClientsModel
    {
        [Key]
        public int CliId { get; set; }
        [Display(Name = "Firm")]
        [Required(ErrorMessage = "Please select firm")]
        public int FrmId { get; set; }
        [Display(Name = "Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter name")]
        public string CliName { get; set; }
        [Display(Name = "Domain")]
        [Required(ErrorMessage = "Please choose country domain")]
        public byte CliInternationalDomestic { get; set; }
        [Display(Name = "Address")]
        [Required(ErrorMessage = "Please enter address")]
        public string CliAddress { get; set; }
        [Display(Name = "City")]
        [Required(ErrorMessage = "Please select city")]
        public int CityId { get; set; }
        [Display(Name = "Pincode")]
        [Required(ErrorMessage = "Please enter pincode")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Pincode should have 6 digit ")]
        public string CliPincode { get; set; }
        [Display(Name = "Phone")]
        [Required(ErrorMessage = "Please enter phone")]
        public string CliPhone { get; set; }
        [Display(Name = "Fax")]
        public string CliFax { get; set; }
        [Display(Name = "Primary email")]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Please enter email")]
        public string CliEmail { get; set; }
        [Display(Name = "Secondary email")]
        [EmailAddress]
        public string CliEmail2 { get; set; }
        [Display(Name = "GST no")]
        public string CliGstNumber { get; set; }
        [Display(Name = "GST rate")]
        public int CliGstRate { get; set; }
        [Display(Name = "HSN code")]
        public string CliHsnCode { get; set; }
        [Display(Name = "TDS rate")]
        public int CliTdsRate { get; set; }
        public int AdmIdRegisterBy { get; set; }
        public DateTime CliRegisteredOn { get; set; }
        public bool? CliIsActive { get; set; }

        [Display(Name = "Logo")]
        public IFormFile CliLogo { get; set; }
        public string CliLogoImage { get; set; }

    }

    public class ParametersClientsModel
    {
        [Key]
        public int CliId { get; set; }
        [Display(Name = "Firm")]
        [Required(ErrorMessage = "Please select firm")]
        public int FrmId { get; set; }
        [Display(Name = "Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter name")]
        public string CliName { get; set; }
        [Display(Name = "Domain")]
        [Required(ErrorMessage = "Please choose country domain")]
        public byte CliInternationalDomestic { get; set; }
        [Display(Name = "Address")]
        [Required(ErrorMessage = "Please enter address")]
        public string CliAddress { get; set; }
        [Display(Name = "City")]
        [Required(ErrorMessage = "Please select city")]
        public int CityId { get; set; }
        [Display(Name = "Pincode")]
        [Required(ErrorMessage = "Please enter pincode")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Pincode should have 6 digit ")]
        public string CliPincode { get; set; }
        [Display(Name = "Phone")]
        [Required(ErrorMessage = "Please enter phone")]
        public string CliPhone { get; set; }
        [Display(Name = "Fax")]
        public string CliFax { get; set; }
        [Display(Name = "Primary email")]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Please enter email")]
        public string CliEmail { get; set; }
        [Display(Name = "Secondary email")]
        [EmailAddress]
        public string CliEmail2 { get; set; }
        [Display(Name = "GST No")]
        [Required(ErrorMessage = "Please enter GST no")]
        public string CliGstNumber { get; set; }
        [Display(Name = "GST Rate")]
        [Required(ErrorMessage = "Please enter GST rate")]
        public int CliGstRate { get; set; }
        [Display(Name = "HSN Code")]
        [Required(ErrorMessage = "Please enter HSN code")]
        public string CliHsnCode { get; set; }
        [Display(Name = "TDS Rate")]
        [Required(ErrorMessage = "Please enter TDS rate")]
        public int CliTdsRate { get; set; }
        public int AdmIdRegisterBy { get; set; }
        public DateTime CliRegisteredOn { get; set; }
        public bool? CliIsActive { get; set; }

        [Display(Name = "Logo")]
        public IFormFile CliLogo { get; set; }
        public string CliLogoImage { get; set; }


        public bool? CliAttMonthReal { get; set; }

        public int? CliAttMonthStart { get; set; }
        public int? CliAttMonthEnd { get; set; }



    }

}
