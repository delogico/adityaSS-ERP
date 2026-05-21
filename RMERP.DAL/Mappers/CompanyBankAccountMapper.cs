using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMERP.DAL.Mappers
{
	public class CompanyBankAccountMapper
	{
		public static CompanyBankAccountVM mapMe(Company_Bank_Account companyBankAccount)
		{
			CompanyBankAccountVM companyBankAccountVM = new CompanyBankAccountVM();
			companyBankAccountVM.CBA_Id = companyBankAccount.CBA_Id;
			companyBankAccountVM.CBA_Bank = companyBankAccount.CBA_Bank;
			companyBankAccountVM.CBA_Account_Number = companyBankAccount.CBA_Account_Number;
			companyBankAccountVM.CBA_Bank_IFSC = companyBankAccount.CBA_Bank_IFSC;
			companyBankAccountVM.FRM_Id = companyBankAccount.FRM_Id.Value;
			return companyBankAccountVM;
		}

		public static Company_Bank_Account mapMeModel(CompanyBankAccountVM companyBankAccountVM)
		{
			Company_Bank_Account companyBankAccount = new Company_Bank_Account();
			companyBankAccount.CBA_Id = companyBankAccountVM.CBA_Id;
			companyBankAccount.CBA_Bank = companyBankAccountVM.CBA_Bank;
			companyBankAccount.CBA_Account_Number = companyBankAccountVM.CBA_Account_Number;
			companyBankAccount.CBA_Bank_IFSC = companyBankAccountVM.CBA_Bank_IFSC;
			companyBankAccount.FRM_Id = companyBankAccountVM.FRM_Id;
			return companyBankAccount;
		}

		public static List<CompanyBankAccountVM> mapCompanyBankAccounts(List<Company_Bank_Account> companyBankAccounts)
		{
			List<CompanyBankAccountVM> lst = new List<CompanyBankAccountVM>();
			foreach (Company_Bank_Account companyBankAccount in companyBankAccounts)
			{
				lst.Add(mapMe(companyBankAccount));
			}
			return lst;
		}
	}
}
