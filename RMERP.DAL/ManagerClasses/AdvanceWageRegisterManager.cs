using Microsoft.EntityFrameworkCore;
using RMERP.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using RMERP.DAL.Helpers;

namespace RMERP.DAL.ManagerClasses
{

    public class AdvanceWageRegisterManager
    {
        RMERPContext _context;
        public AdvanceWageRegisterManager(RMERPContext context)
        {
            _context = context;
        }
        public List<Employee_Advance> AdvanceRptForBank(DateTime WAG_Month, int FRM_Id)
        {
            List<Employee_Advance> employee_Advances = _context.Employee_Advance.Include(m => m.EMP_).Where(m => m.ADV_RegisteredOn.Month.Equals(WAG_Month.Month) && m.EMP_.FRM_Id.Equals(FRM_Id)).ToList();
            return employee_Advances;
        }
        public List<Employee_Advance> NotCompletedAdvanceLst(DateTime WAG_Month, int FRM_Id,int WAG_Id)
        {
            DateTime lastDate = new DateTime(WAG_Month.Year, WAG_Month.Month, 1).AddMonths(1).AddDays(-1);

            var NotCompletedAdvanceEmp = _context.Employee_Advance.Include(m=>m.WAG_Id_Closed_OnNavigation).Include(m => m.EMP_)
                 .Where(m => m.ADV_RegisteredOn.Date <= lastDate.Date && m.EMP_.FRM_Id.Equals(FRM_Id) && (m.ADV_Status.Equals(false) || (m.ADV_Status.Equals(true) && m.WAG_Id_Closed_OnNavigation.WAG_Month >= WAG_Month)));
            
            return NotCompletedAdvanceEmp.ToList();
        }
        public string addWageRegisterAdvances(int EMP_id, int WAG_Id, int CLI_Id, decimal WAD_Amount, DateTime WAG_Month, bool WAD_Is_LoanCompleted)
        {
            string res = "";
            try
            {
                Wage_Register_Advances wra = new Wage_Register_Advances();
                wra.CLI_Id = CLI_Id;
                wra.WAG_Id = WAG_Id;
                wra.EMP_Id = EMP_id;
                wra.WAD_Amount = WAD_Amount;
                wra.WAD_Is_LoanCompleted = WAD_Is_LoanCompleted;
                wra.WAD_Status = false;
                _context.Wage_Register_Advances.Add(wra);

                if (WAD_Is_LoanCompleted == true)
                {
                    wra.WAD_Status = true;
                    wra.WAD_ClosedOn = ProjectUtils.DateNow();

                    List<Wage_Register_Advances> register_Advances = _context.Wage_Register_Advances.Where(m => m.EMP_Id.Equals(EMP_id) && m.WAD_Status.Equals(false)).ToList();
                    register_Advances.ForEach(m => m.WAD_Status=true);
                   
                    List<Employee_Advance> employee_Advances = _context.Employee_Advance.Where(m => m.EMP_Id.Equals(EMP_id) && m.ADV_Status.Equals(false)).ToList();
                    employee_Advances.ForEach(m => { m.ADV_Status = true;m.WAG_Id_Closed_On = WAG_Id; });
                }
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        }
        public void editWageRegisterAdvances(int WAD_Id,decimal WAD_Amount,bool WAD_Is_LoanCompleted)
        {
            Wage_Register_Advances register_Advance = _context.Wage_Register_Advances.Find(WAD_Id);
            register_Advance.WAD_Amount = WAD_Amount;
            register_Advance.WAD_Is_LoanCompleted = WAD_Is_LoanCompleted;
            if (register_Advance.WAD_Is_LoanCompleted==false && WAD_Is_LoanCompleted == true)
            {
                register_Advance.WAD_Status = true;
                register_Advance.WAD_ClosedOn = ProjectUtils.DateNow();

                List<Wage_Register_Advances> register_Advances = _context.Wage_Register_Advances.Where(m => m.EMP_Id.Equals(register_Advance.EMP_Id) && m.WAD_Status.Equals(false)).ToList();
                register_Advances.ForEach(m => m.WAD_Status.Equals(true));

                List<Employee_Advance> employee_Advances = _context.Employee_Advance.Where(m => m.EMP_Id.Equals(register_Advance.EMP_Id) && m.ADV_Status.Equals(false)).ToList();
                employee_Advances.ForEach(m => { m.ADV_Status = true; m.WAG_Id_Closed_On = register_Advance.WAG_Id; });
            }

            _context.SaveChanges();
        }

        public List<Employee_Advance> IDBI_TO_IDBI_AdvanceRpt(DateTime WAG_Month, int FRM_Id)
        {
            DateTime lastDate = new DateTime(WAG_Month.Year, WAG_Month.Month, 1).AddMonths(1).AddDays(-1);
            DateTime startDate = new DateTime(WAG_Month.Year, WAG_Month.Month, 1);

            List<Employee_Advance> result = _context.Employee_Advance.Include(m => m.EMP_).Where(m => m.ADV_RegisteredOn.Date <= lastDate.Date && m.ADV_RegisteredOn.Date >= startDate.Date && m.EMP_.FRM_Id.Equals(FRM_Id) && m.EMP_.EMP_Payment_Type.Equals((int)ProjectUtils.PAYMENT_TYPE.Bank_Account) && m.EMP_.EMP_Is_IDBI_Other.Equals((int)ProjectUtils.PAYMENT_BANK_TYPE.IDBI_To_IDBI))
                                            .GroupBy(l => l.EMP_Id)
                                            .Select(cl => new Employee_Advance
                                            {
                                                EMP_Id = cl.First().EMP_Id,
                                                ADV_Amount = cl.Sum(c => c.ADV_Amount),
                                                EMP_ = cl.First().EMP_
                                            }).ToList();
            return result;
        }
        public List<Employee_Advance> IDBI_TO_Other_AdvanceRpt(DateTime WAG_Month, int FRM_Id)
        {
            DateTime lastDate = new DateTime(WAG_Month.Year, WAG_Month.Month, 1).AddMonths(1).AddDays(-1);
            DateTime startDate = new DateTime(WAG_Month.Year, WAG_Month.Month, 1);
            List<Employee_Advance> result = _context.Employee_Advance.Include(m => m.EMP_).Where(m => m.ADV_RegisteredOn.Date <= lastDate.Date && m.ADV_RegisteredOn.Date >= startDate.Date && m.EMP_.FRM_Id.Equals(FRM_Id) && m.EMP_.EMP_Payment_Type.Equals((int)ProjectUtils.PAYMENT_TYPE.Bank_Account) && m.EMP_.EMP_Is_IDBI_Other.Equals((int)ProjectUtils.PAYMENT_BANK_TYPE.IDBI_To_Others))
                                            .GroupBy(l => l.EMP_Id)
                                            .Select(cl => new Employee_Advance
                                            {
                                                EMP_Id = cl.First().EMP_Id,
                                                ADV_Amount = cl.Sum(c => c.ADV_Amount),
                                                EMP_ = cl.First().EMP_
                                            }).ToList();
            return result;
        }
        public List<Employee_Advance> CHEQUE_CASH_AdvanceRpt(DateTime WAG_Month, int FRM_Id)
        {
            DateTime lastDate = new DateTime(WAG_Month.Year, WAG_Month.Month, 1).AddMonths(1).AddDays(-1);
            DateTime startDate = new DateTime(WAG_Month.Year, WAG_Month.Month, 1);
            List<Employee_Advance> result = _context.Employee_Advance.Include(m => m.EMP_).Where(m => m.ADV_RegisteredOn.Date <= lastDate.Date && m.ADV_RegisteredOn.Date >= startDate.Date && m.EMP_.FRM_Id.Equals(FRM_Id) && m.EMP_.EMP_Payment_Type.Equals((int)ProjectUtils.PAYMENT_TYPE.Cheque_Cash))
                                            .GroupBy(l => l.EMP_Id)
                                            .Select(cl => new Employee_Advance
                                            {
                                                EMP_Id = cl.First().EMP_Id,
                                                ADV_Amount = cl.Sum(c => c.ADV_Amount),
                                                EMP_ = cl.First().EMP_
                                            }).ToList();
            return result;
        }
    }
}
