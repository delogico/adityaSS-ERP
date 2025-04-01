using RMERP.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using RMERP.DAL.App_Code;

namespace RMERP.DAL.ManagerClasses
{
    public class AdminUsersManager
    {
        RMERPContext _context;
        public AdminUsersManager(RMERPContext context)
        {
            _context = context;
        }
        public AdminUser Login(AdminUser adminUsers)
        {
            var user = _context.AdminUsers.Where(m => m.ADM_EmailId.Equals(adminUsers.ADM_EmailId) && m.ADM_Password.Equals(adminUsers.ADM_Password)).FirstOrDefault();
            return user;
        }
        public string AddEditAdminUsers(AdminUser adminUsers)
        {
            string res = string.Empty;
            try
            {
                if (adminUsers.ADM_Id > 0)
                {
                    _context.AdminUsers.Update(adminUsers);
                }
                else
                {
                    _context.AdminUsers.Add(adminUsers);
                }
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        }
        public IEnumerable<AdminUser> GetAdminUsers()
        {
            try
            {
                IEnumerable<AdminUser> listAdminUsers = _context.AdminUsers.Include(m=>m.FRM).OrderBy(m => m.ADM_FirstName).ToList();
                return listAdminUsers;
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }
        public AdminUser GetAdminUsersById(int AdminId)
        {
            try
            {
                AdminUser adminUsers = new AdminUser();
                adminUsers = _context.AdminUsers.Where(m => m.ADM_Id.Equals(AdminId)).FirstOrDefault();
                return adminUsers;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
        public string deleteAdminUser(int id)
        {
            string res = string.Empty;
            try
            {
                if (id > 0)
                {
                    var user = _context.AdminUsers.Find(id);
                    _context.AdminUsers.Remove(user);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                res=ex.Message
;
            }
            return res;
        }
    }
}
