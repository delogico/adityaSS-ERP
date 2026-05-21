using RMERP.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static RMERP.DAL.Helpers.ProjectUtils;

namespace RMERP.DAL.ViewModel
{
    public class ClientSelectionVM
    {
        public List<SelectionVM> selectionVMs { get; set; }  
        [Display(Name ="Report Type")]
        public string Report { get; set; }
        public int FRM_Id { get; set; }
        public int WAG_Id { get; set; }
        public int TotalActiveClients { get; set; }

        public string Reference { get; set; }

		public List<BANK_REPORT_TYPE> AvailableBankReports { get; set; }
		public List<BANK_REPORT_TYPE> MappedBankReports { get; set; }
	}
    public class SelectionVM
    {
        public int CLI_Id { get; set; }
        public string CLI_Name { get; set; }
        public bool IsSelect { get; set; }
        public int WAG_Id { get; set; }
        public bool Is_WageRegisterSaved { get; set; }
    }
}
