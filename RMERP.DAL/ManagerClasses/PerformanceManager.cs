using RMERP.DAL.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace RMERP.DAL.ManagerClasses
{   
    public class PerformanceManager
    {
        RMERPContext _context;
        public PerformanceManager(RMERPContext context)
        {
            _context = context;
        }
        public Wage_Register_Performance GetPerformanceById(int WRP_Id)
        {
            Wage_Register_Performance performance = new Wage_Register_Performance();
            performance = _context.Wage_Register_Performance.Include(M => M.CLE_).ThenInclude(M => M.EMP_).Where(m=>m.WRP_Id.Equals(WRP_Id)).FirstOrDefault();
            return performance;
        }
        public Wage_Register_Performance GetPerformanceByCLE(int WAG_Id,int CLE_Id,int CLI_Id)
        {
            Wage_Register_Performance performance = new Wage_Register_Performance();
            performance = _context.Wage_Register_Performance.Include(M => M.CLE_).ThenInclude(M => M.EMP_).Where(m => m.WAG_Id.Equals(WAG_Id) && m.CLE_Id.Equals(CLE_Id) && m.CLE_.CLI_Id.Equals(CLI_Id)).FirstOrDefault();
            return performance;
        }
        public string UpdateAmount(List<Wage_Register_Performance> performances)
        {
            string res = string.Empty;
            try
            {
                foreach(Wage_Register_Performance performance in performances)
                {
                    if (performance.WRP_Id > 0)
                        _context.Wage_Register_Performance.Update(performance);
                    else
                        _context.Wage_Register_Performance.Add(performance);
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
