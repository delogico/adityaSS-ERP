using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMERP.DAL.ViewModel
{
	public class CompanyBankAccountVM
	{
		public int CBA_Id { get; set; }
		public int FRM_Id { get; set; }

		[Display(Name = "Bank")]
		[Required(AllowEmptyStrings = false, ErrorMessage = "Please add bank")]
		public string CBA_Bank { get; set; }

		[Display(Name = "Account Number")]
		[Required(AllowEmptyStrings = false, ErrorMessage = "Please add account number")]
		public string CBA_Account_Number { get; set; }

		[Display(Name = "IFSC Code")]
		[Required(AllowEmptyStrings = false, ErrorMessage = "Please add IFSC code")]
		public string CBA_Bank_IFSC { get; set; }

	}
}
