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
                IEnumerable<Employees> listEmployees = _context.Employees.Include(m => m.DEPT_).OrderBy(m => m.EMP_FirstName).ToList();
                return listEmployees;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public Employees GetEmployeeById(int EMP_Id)
        {
            return _context.Employees.Where(m => m.EMP_Id.Equals(EMP_Id)).Include(e=>e.DEPT_).Include(e=>e.Employee_Advance).FirstOrDefault();
        }
        public string ActiveEmployee(int EmpId, int AdminId)
        {
            string res = string.Empty;
            Employees employees = new Employees();
            try
            {
                employees = _context.Employees.Find(EmpId);
                employees.EMP_IsActive = (employees.EMP_IsActive == false ? true : false);
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
        public string AddEditAdvance(Employee_Advance employee_Advance)
        {
            string res = string.Empty;
            employee_Advance.ADV_RegisteredOn = ProjectUtils.DateNow();
            try
            {
                if (employee_Advance.ADV_Id > 0)
                {
                    _context.Employee_Advance.Update(employee_Advance);
                }
                else
                    _context.Employee_Advance.Add(employee_Advance);

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        }

        public Employee_Advance GetEmployeeAdvanceById(int ADV_Id)
        {
            Employee_Advance employee_Advance = new Employee_Advance();
            employee_Advance = _context.Employee_Advance.Find(ADV_Id);
            return employee_Advance;
        }
        public string DeleteAdvance(int ADV_Id)
        {
            string res = string.Empty;
            try
            {
                var adv = _context.Employee_Advance.Find(ADV_Id);
                _context.Employee_Advance.Remove(adv);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        }
        public List<Employee_Advance> GetEmployee_Advances(int EMP_Id)
        {
            List<Employee_Advance> employee_Advances = new List<Employee_Advance>();
            employee_Advances = _context.Employee_Advance.Where(m=>m.EMP_Id.Equals(EMP_Id)).Include(m=>m.EMP_).ToList();
            return employee_Advances;
        }
        public List<Employee_Advance> AdvanceRptForBank(DateTime WAG_Month)
        {
            List<Employee_Advance> employee_Advances = _context.Employee_Advance.Include(m => m.EMP_).Where(m => m.ADV_RegisteredOn.Month.Equals(WAG_Month.Month)).ToList();  
            return employee_Advances;
        }
        public List<Employee_Advance> NotCompletedAdvanceLst(DateTime WAG_Month)
        {
            DateTime lastDate = new DateTime(WAG_Month.Year, WAG_Month.Month, 1).AddMonths(1).AddDays(-1);
            List<Employee_Advance> employee_Advances = _context.Employee_Advance.Include(m=>m.EMP_)
                .Where(m => m.ADV_RegisteredOn.Date <= lastDate.Date && m.ADV_Status.Equals(false))
                .ToList();
            return employee_Advances;
        }
        
    }
}
