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
            List<Firms> listFirms = new List<Firms>();
            listFirms = _context.Firms.OrderBy(m=>m.FrmName).ToList();
            return listFirms;
        }  
        public string saveEditFirm(Firms firms)
        {
            string res = string.Empty;
            try
            {
                if (firms.FrmId > 0)
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
        public Firms GetFirms(int ?id)
        {
            return _context.Firms.Find(id);
        }
        public string DeleteFirms(int id)
        {
            string res="";
            try
            {
              var firms=  _context.Firms.Find(id);
                _context.Firms.Remove(firms);
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
