using RMERP.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using RMERP.DAL.App_Code;

namespace RMERP.DAL.ManagerClasses
{
    public class EmployeeManager
    {
        RMERPContext _context;
        public EmployeeManager(RMERPContext context)
        {
            _context = context;
        }
        public string AddEditEmployee(Employees employees)
        {
            string res = string.Empty;
            try
            {
                employees.EMP_RegisteredOn = ProjectUtils.DateNow();
                if (employees.EMP_Id > 0)
                {
                    _context.Employees.Update(employees);
                }
                else
                {
                    _context.Employees.Add(employees);
                }
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        }
        public IEnumerable<Employees> GetEmployees()
        {
            try
            {
                IEnumerable<Employees> listEmployees = _context.Employees.Include(m=>m.DEPT_).OrderBy(m => m.EMP_FirstName).ToList();
                return listEmployees;
            }
            catch (Exception ex)
            {

                throw ex;
            }
           
        }
        public Employees GetEmployeesById(int EmpID)
        {
            try
            {
                Employees employees = new Employees();
                employees = _context.Employees.Where(m => m.EMP_Id.Equals(EmpID)).FirstOrDefault();
                return employees;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
        public string ActiveEmployee(int EmpId, int AdminId)
        {
            string res = string.Empty;
            Employees employees = new Employees();
            try
            {
                employees = _context.Employees.Find(EmpId);
                employees.EMP_IsActive=(employees.EMP_IsActive == false? true:false);
                employees.ADM_Id_InactivatedBy = AdminId;
                _context.Employees.Update(employees);
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
