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
        public IEnumerable<Firms> getFirmList()
        {
            return _context.Firms.Include(m=>m.STA_).OrderBy(m=>m.FRM_Name).ToList();
        }  
        public string saveEditFirm(Firms firms)
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
                res= ex.Message;
            }
            return res;
        }
        public Firms GetFirm(int FRM_Id)
        {
            return _context.Firms.Find(FRM_Id);
        }
    }
}
