using System;
using System.Collections.Generic;
using static RMERP.DAL.Helpers.ProjectUtils;

namespace RMERP.Helpers
{
	public class BankReportMapping
	{

		public static readonly Dictionary<REGISTER_BANK, List<BANK_REPORT_TYPE>> Map= new Dictionary<REGISTER_BANK, List<BANK_REPORT_TYPE>>
		{
			{
				REGISTER_BANK.ICICI_BANK_LTD,
				new List<BANK_REPORT_TYPE>
				{
					BANK_REPORT_TYPE.ICICI_360_Report,
					BANK_REPORT_TYPE.ICICI_ADHOC_Report
				}
			},
			{
				REGISTER_BANK.HDFC_BANK_LTD,
				new List<BANK_REPORT_TYPE>
				{
					BANK_REPORT_TYPE.HDFC_Bank_To_HDFC_Bank_Report,
					BANK_REPORT_TYPE.HDFC_Bank_To_Others_Report
				}
			},
			{
				REGISTER_BANK.IDBI_BANK_LTD,
				new List<BANK_REPORT_TYPE>
				{
					BANK_REPORT_TYPE.IDBI_Bank_To_IDBI_Bank_Report,
					BANK_REPORT_TYPE.IDBI_Bank_To_Others_Report
				}
			}
		};
	}
}
