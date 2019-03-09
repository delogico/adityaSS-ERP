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
        public AdminUsers Login(AdminUsers adminUsers)
        {
            var user = _context.AdminUsers.Where(m => m.AdmEmailId.Equals(adminUsers.AdmEmailId) && m.AdmPassword.Equals(adminUsers.AdmPassword)).FirstOrDefault();
            return user;
        }
        public string AddEditAdminUsers(AdminUsers adminUsers)
        {
            string res = string.Empty;
            try
            {
                if (adminUsers.AdmId > 0)
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
        public IEnumerable<AdminUsers> GetAdminUsers()
        {
            try
            {
                IEnumerable<AdminUsers> listAdminUsers = _context.AdminUsers.Include(m=>m.Frm).OrderBy(m => m.AdmFirstName).ToList();
                return listAdminUsers;
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }
        public AdminUsers GetAdminUsersById(int AdminId)
        {
            try
            {
                AdminUsers adminUsers = new AdminUsers();
                adminUsers = _context.AdminUsers.Where(m => m.AdmId.Equals(AdminId)).FirstOrDefault();
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
