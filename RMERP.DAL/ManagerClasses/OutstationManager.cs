using RMERP.DAL.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace RMERP.DAL.ManagerClasses
{   
    public class OutstationManager
    {
        RMERPContext _context;
        public OutstationManager(RMERPContext context)
        {
            _context = context;
        }
        public Wage_Register_Outstation GetOutstationById(int WRO_Id)
        {
            Wage_Register_Outstation station = new Wage_Register_Outstation();
            station = _context.Wage_Register_Outstation.Include(M => M.CLE_).ThenInclude(M => M.EMP_).Where(m=>m.WRO_Id.Equals(WRO_Id)).FirstOrDefault();
            return station;
        }
        public Wage_Register_Outstation GetOutstationByCLE(int WAG_Id,int CLE_Id,int CLI_Id)
        {
            Wage_Register_Outstation outstation = new Wage_Register_Outstation();
            outstation = _context.Wage_Register_Outstation.Include(M => M.CLE_).ThenInclude(M => M.EMP_).Where(m => m.WAG_Id.Equals(WAG_Id) && m.CLE_Id.Equals(CLE_Id) && m.CLE_.CLI_Id.Equals(CLI_Id)).FirstOrDefault();
            return outstation;
        }
        public string UpdateAmount(List<Wage_Register_Outstation> outstations)
        {
            string res = string.Empty;
            try
            {
                foreach(Wage_Register_Outstation outstation in outstations)
                {
                    if (outstation.WRO_Id > 0)
                        _context.Wage_Register_Outstation.Update(outstation);
                    else
                        _context.Wage_Register_Outstation.Add(outstation);
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
