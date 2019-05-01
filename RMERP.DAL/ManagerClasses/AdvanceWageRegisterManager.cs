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
        public List<Employee_Advance> AdvanceRptForBank(DateTime WAG_Month)
        {
            List<Employee_Advance> employee_Advances = _context.Employee_Advance.Include(m => m.EMP_).Where(m => m.ADV_RegisteredOn.Month.Equals(WAG_Month.Month)).ToList();
            return employee_Advances;
        }
        public List<Employee_Advance> NotCompletedAdvanceLst(DateTime WAG_Month)
        {
            DateTime lastDate = new DateTime(WAG_Month.Year, WAG_Month.Month, 1).AddMonths(1).AddDays(-1);
            List<Employee_Advance> employee_Advances = _context.Employee_Advance.Include(m => m.EMP_)
                .Where(m => m.ADV_RegisteredOn.Date <= lastDate.Date && m.ADV_Status.Equals(false))
                .ToList();
            return employee_Advances;
        }
        public string addWageRegisterAdvances(int EMP_id, int WAG_Id, int CLI_Id, decimal WAD_Amount, DateTime WAG_Month, bool WAD_Status)
        {
            string res = "";
            try
            {
                Wage_Register_Advances wra = new Wage_Register_Advances();

                wra.CLI_Id = CLI_Id;
                wra.WAG_Id = WAG_Id;
                wra.EMP_Id = EMP_id;
                wra.WAD_Amount = WAD_Amount;
                wra.WAD_Status = WAD_Status;
               
                _context.Wage_Register_Advances.Add(wra);
                if (WAD_Status == true)
                {
                    wra.WAD_ClosedOn = ProjectUtils.DateNow();
                    List<Employee_Advance> employee_Advances = _context.Employee_Advance.Where(m => m.EMP_Id.Equals(EMP_id) && m.ADV_RegisteredOn.Month.Equals(WAG_Month.Month)).ToList();
                    employee_Advances.ForEach(m => m.ADV_Status = true);
                }
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        }
        public string addWageRegisterAdvances(int EMP_id,decimal WAD_Amount,bool WAD_Status, DateTime WAG_Month, int WAD_Id)
        {
            string res = "";
            try
            {
                Wage_Register_Advances wra = new Wage_Register_Advances();
                wra = _context.Wage_Register_Advances.Find(WAD_Id);
                wra.WAD_Amount = WAD_Amount;
                if (WAD_Id > 0)
                    _context.Wage_Register_Advances.Update(wra);
                else
                    _context.Wage_Register_Advances.Add(wra);
                if (WAD_Status == true)
                {
                    wra.WAD_ClosedOn = ProjectUtils.DateNow();
                    List<Employee_Advance> employee_Advances = _context.Employee_Advance.Where(m => m.EMP_Id.Equals(EMP_id) && m.ADV_RegisteredOn.Month.Equals(WAG_Month.Month)).ToList();
                    employee_Advances.ForEach(m => m.ADV_Status =true);
                }
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        }
    }
}
