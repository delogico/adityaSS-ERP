using RMERP.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMERP.DAL.ManagerClasses
{
    public class CitiesManager
    {
        RMERPContext _context;
        public CitiesManager(RMERPContext context)
        {
            _context = context;
        }
        public IEnumerable<City> getCityList()
        {
            return _context.Cities.OrderBy(m=>m.CIT_Name).ToList();
        }
        public string saveEditCity(City cities)
        {
            string res = string.Empty;
            try
            {
                if (cities.CIT_Id > 0)
                {
                    _context.Cities.Update(cities);
                }
                else
                {
                    _context.Cities.Add(cities);
                }
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        }
        public City GetCity(int CIT_Id)
        {
            return _context.Cities.Find(CIT_Id);
        }
    }
}
