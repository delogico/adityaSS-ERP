using Microsoft.EntityFrameworkCore;
using RMERP.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMERP.DAL.ManagerClasses
{
    public class FirmsManager
    {
        RMERPContext _context;
        public FirmsManager(RMERPContext context)
        {
            _context = context;
        }
        public IEnumerable<Firm> getFirmList(int? FRM_Id = null)
        {
            if (FRM_Id.HasValue && FRM_Id.Value > 0)
            {
                return _context.Firms.Where(f => f.FRM_Id == FRM_Id.Value).Include(m => m.STA).OrderBy(m => m.FRM_Name).ToList();
            }
            else
            {
                return _context.Firms.Include(m => m.STA).OrderBy(m => m.FRM_Name).ToList();
            }
        }
        public string saveEditFirm(Firm firms)
        {
            string res = string.Empty;
            try
            {
                if (firms.FRM_Id > 0)
                {
                    _context.Firms.Update(firms);
                }
                else
                {
                    _context.Firms.Add(firms);
                }
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        }
        public Firm GetFirm(int FRM_Id)
        {
            return _context.Firms.Find(FRM_Id);
        }

        #region "Company Bank Account"
        public IEnumerable<Company_Bank_Account> getCompanyBankAccountListOnFRM(int FRM_Id)
        {
            if (FRM_Id > 0)
            {
                return _context.Company_Bank_Accounts.Where(f => f.FRM_Id == FRM_Id).Include(m => m.FRM).OrderBy(m => m.CBA_Bank).ToList();
            }
            else
            {
                return _context.Company_Bank_Accounts.Include(m => m.FRM).OrderBy(m => m.CBA_Bank).ToList();
            }
        }
        public string saveEditCompanyBankAccount(Company_Bank_Account companyBankAccount)
        {
            string res = string.Empty;
            try
            {
                if (companyBankAccount.CBA_Id > 0)
                {
                    _context.Company_Bank_Accounts.Update(companyBankAccount);
                }
                else
                {
                    _context.Company_Bank_Accounts.Add(companyBankAccount);
                }
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        }

        public Company_Bank_Account GetCompanyBankAccount(int CBA_Id)
        {
            return _context.Company_Bank_Accounts.Find(CBA_Id);
        }

        #endregion
    }
}