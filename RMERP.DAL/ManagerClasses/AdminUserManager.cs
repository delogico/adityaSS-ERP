using RMERP.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace RMERP.DAL.ManagerClasses
{
    public class AdminUserManager
    {
        RMERPContext _context;
        public AdminUserManager(RMERPContext context)
        {
            _context = context;
        }
        public AdminUsers Login(AdminUsers adminUsers)
        {
            var user = _context.AdminUsers.Where(m => m.ADM_EmailId.Equals(adminUsers.ADM_EmailId) && m.ADM_Password.Equals(adminUsers.ADM_Password)).FirstOrDefault();
            return user;
        }
        public IEnumerable<AdminUsers> getAdminUsersList()
        {
            var adminUserList = _context.AdminUsers.Include(m => m.FRM_);
            return adminUserList.ToList();
        }
        public string AddEditAdminUsers(AdminUsers adminUsers)
        {
            try
            {
                if (adminUsers.ADM_Id != 0)
                {
                    _context.Update(adminUsers);
                }
                else
                {
                    _context.Add(adminUsers);
                }
                _context.SaveChanges();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }           
        }
        public void deleteAdminUser(int id)
        {
            if (id > 0)
            {
                var user = _context.AdminUsers.Find(id);
                _context.AdminUsers.Remove(user);
                _context.SaveChanges();
            }
            
        }
        public AdminUsers EditAdminUser(int id)
        {
            AdminUsers adminUsers=new AdminUsers();
            var user = _context.AdminUsers.Find(id);
            return user;
        }
        public List<Firms> getFirmList()
        {
           return _context.Firms.ToList();
        }
        public List<Cities> getCityList()
        {
            return _context.Cities.ToList();
        }
    }
}
