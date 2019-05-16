using RMERP.DAL.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace RMERP.DAL.ManagerClasses
{   
    public class CanteenManager
    {
        RMERPContext _context;
        public CanteenManager(RMERPContext context)
        {
            _context = context;
        }
        public Wage_Register_Canteen GetCanteenById(int WRC_Id)
        {
            Wage_Register_Canteen canteen = new Wage_Register_Canteen();
            canteen = _context.Wage_Register_Canteen.Include(M => M.CLE_).ThenInclude(M => M.EMP_).Where(m=>m.WRC_Id.Equals(WRC_Id)).FirstOrDefault();
            return canteen;
        }
        public Wage_Register_Canteen GetCanteenByCLE(int WAG_Id,int CLE_Id,int CLI_Id)
        {
            Wage_Register_Canteen canteen = new Wage_Register_Canteen();
            canteen = _context.Wage_Register_Canteen.Include(M => M.CLE_).ThenInclude(M => M.EMP_).Where(m => m.WAG_Id.Equals(WAG_Id) && m.CLE_Id.Equals(CLE_Id) && m.CLE_.CLI_Id.Equals(CLI_Id)).FirstOrDefault();
            return canteen;
        }
        public string UpdateAmount(List<Wage_Register_Canteen> canteens)
        {
            string res = string.Empty;
            try
            {
                foreach(Wage_Register_Canteen canteen in canteens)
                {
                    if (canteen.WRC_Id > 0)
                        _context.Wage_Register_Canteen.Update(canteen);
                    else
                        _context.Wage_Register_Canteen.Add(canteen);
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
