using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;
using RMERP.DAL.ManagerClasses;
using RMERP.DAL.Mappers;
using Microsoft.AspNetCore.Http;
using static RMERP.DAL.Helpers.ProjectUtils;
using SmartBreadcrumbs.Attributes;
using Microsoft.AspNetCore.Authorization;
using RMERP.DAL.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RMERP.Controllers
{
	[Authorize]
	public class FirmsController : Controller
	{
		private readonly RMERPContext _context;

		public FirmsController(RMERPContext context)
		{
			_context = context;
		}

		public IActionResult Index()
		{
			FirmsManager firmsManager = new FirmsManager(_context);
			return View(FirmMapper.mapFirms(firmsManager.getFirmList().ToList()));
		}

		public ActionResult AddEditFirm(int FRM_Id = 0)
		{
			EmployeeManager employeeManager = new EmployeeManager(_context);
			FirmsManager firmsManager = new FirmsManager(_context);
			FirmVM vm = new FirmVM();
			ViewBag.States = employeeManager.GetStates();
			if (FRM_Id > 0)
			{
				vm = FirmMapper.mapMe(firmsManager.GetFirm(FRM_Id));
			}
			return View(vm);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult AddEditFirms(FirmVM firmVM)
		{
			FirmsManager firmsManager = new FirmsManager(_context);
			Firm firms = FirmMapper.mapMeModel(firmVM);
			if (ModelState.IsValid)
			{
				string res = firmsManager.saveEditFirm(firms);
				if (res != "")
				{
					TempData["message"] = "Can not Inserted";
				}
			}

			return RedirectToAction("Index", "Firms");

		}

		#region "Company Bank Account"
		public IActionResult CompanyBankAccount(int FRM_Id)
		{
			FirmsManager firmsManager = new FirmsManager(_context);
			ViewBag.FRM_Id = FRM_Id;
			ViewBag.FRM_Name = firmsManager.GetFirm(FRM_Id).FRM_Name;
			return View(CompanyBankAccountMapper.mapCompanyBankAccounts(firmsManager.getCompanyBankAccountListOnFRM(FRM_Id).ToList()));
		}
		public ActionResult AddEditCompanyBankAccount(int CBA_Id = 0, int FRM_Id = 0)
		{
			FirmsManager firmsManager = new FirmsManager(_context);
			CompanyBankAccountVM vm = new CompanyBankAccountVM();
			if (CBA_Id > 0)
			{
				vm = CompanyBankAccountMapper.mapMe(firmsManager.getCompanyBankAccountListOnFRM(FRM_Id).Where(m => m.CBA_Id == CBA_Id).FirstOrDefault());
			}
			else
			{
				vm.FRM_Id = FRM_Id;
			}
			IEnumerable<ProjectUtils.REGISTER_BANK> BANK = Enum.GetValues(typeof(ProjectUtils.REGISTER_BANK)).Cast<ProjectUtils.REGISTER_BANK>();
			ViewBag.REGISTER_BANK = from action in BANK select new SelectListItem { Text = ProjectUtils.GetStringValue(action), Value = ProjectUtils.GetStringValue(action) };
			return View(vm);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult AddEditCompanyBankAccount(CompanyBankAccountVM companyBankAccountVM)
		{
			FirmsManager firmsManager = new FirmsManager(_context);
			Company_Bank_Account companyBankAccount = CompanyBankAccountMapper.mapMeModel(companyBankAccountVM);
			if (ModelState.IsValid)
			{
				string res = firmsManager.saveEditCompanyBankAccount(companyBankAccount);
				if (res != "")
				{
					TempData["message"] = "Can not Inserted";
				}
			}
			return RedirectToAction("CompanyBankAccount", "Firms", new { FRM_Id = companyBankAccountVM.FRM_Id });
		}
		#endregion
	}
}