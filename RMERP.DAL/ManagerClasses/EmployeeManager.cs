using RMERP.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using RMERP.DAL.App_Code;
using RMERP.DAL.ViewModel;

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
        public IEnumerable<Employees> GetEmployees(int FRM_Id)
        {
            try
            {
                IEnumerable<Employees> listEmployees = null;
                if (FRM_Id > 0)
                {
                    listEmployees = _context.Employees.Where(m => m.FRM_Id.Equals(FRM_Id)).OrderBy(m => m.EMP_FirstName).Include(m => m.FRM_).ToList();
                }
                else
                {
                    listEmployees = _context.Employees.OrderBy(m => m.EMP_FirstName).Include(m => m.FRM_).ToList();
                }
                return listEmployees;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Employees GetEmployeeById(int EMP_Id)
        {
            return _context.Employees.Where(m => m.EMP_Id.Equals(EMP_Id)).Include(e => e.Employee_Advance).Include(m => m.Employee_Documents).Include(m => m.FRM_).FirstOrDefault();
        }
        public void ActiveEmployee(int EmpId, DateTime date, int AdminId, out string actionMessage)
        {
            Employees employees = new Employees();
            try
            {
                employees = _context.Employees.Find(EmpId);
                  //employees.EMP_IsActive = (employees.EMP_IsActive == false ? true : false);
                    employees.EMP_IsActive = false;
                    employees.ADM_Id_InactivatedBy = AdminId;
                    employees.EMP_InactivatedOn = date;
                    _context.Employees.Update(employees);
                    actionMessage = string.Empty;
               
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                actionMessage = "There is some problem! Please Try Again";
            }
        }
        public string AddEditAdvance(Employee_Advance employee_Advance)
        {
            string res = string.Empty;
            try
            {
                if (employee_Advance.ADV_Id > 0)
                {
                    _context.Employee_Advance.Update(employee_Advance);
                }
                else
                {
                    employee_Advance.ADV_RegisteredOn = ProjectUtils.DateNow();
                    _context.Employee_Advance.Add(employee_Advance);
                }
                   

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
            employee_Advances = _context.Employee_Advance.Where(m => m.EMP_Id.Equals(EMP_Id)).Include(m => m.EMP_).ToList();
            return employee_Advances;
        }

        //public List<UpdateAdvanceEMI> UpdateAdvanceEMIs(DateTime WAG_Month)
        //{
        //    DateTime lastDate = new DateTime(WAG_Month.Year, WAG_Month.Month, 1).AddMonths(1).AddDays(-1);
        //    List<UpdateAdvanceEMI> UpdateAdvanceEMIs = new List<UpdateAdvanceEMI>();
        //    List<Employee_Advance> employee_Advances = _context.Employee_Advance.Include(m => m.EMP_)
        //        .Where(m => m.ADV_RegisteredOn.Date <= lastDate.Date && m.ADV_Status.Equals(false))
        //        .ToList();
        //    foreach(var item in employee_Advances)
        //    {
        //        UpdateAdvanceEMI update = new UpdateAdvanceEMI();
        //        update.EMP_Id = item.EMP_Id;
        //        update.EmployeeName = item.EMP_.EMP_FirstName;
        //        update.WAG_Month = WAG_Month;                
        //    }
        //    return UpdateAdvanceEMIs;
        //}
        public bool IsAssignedEmployee(int EMP_Id)
        {
            List<Clients_Employees> list = _context.Clients_Employees.Where(m => m.EMP_Id.Equals(EMP_Id) && m.CLE_UnassignedOn != null).ToList();
            return list.Count() > 0;
        }

        public bool CheckExistingAadhar(string AadharNumber,int EMP_Id)
        {
            return _context.Employees.Any(m => m.EMP_Aadhar_Number.Equals(AadharNumber) && m.EMP_Id!=EMP_Id);
        }

        public List<Employees> SearchEmployees(int FRM_Id, bool EMP_UAN_Number, bool EMP_ESIC_Number)
        {
            List<Employees> list = new List<Employees>();
           
            if (FRM_Id > 0)
            {
                list = _context.Employees.Where(m => m.FRM_Id.Equals(FRM_Id)).Include(m => m.FRM_).ToList();
            }
            else
            {
                list = _context.Employees.Include(m => m.FRM_).ToList();
            }
            if(EMP_UAN_Number==true)
            {
                list = list.Where(m => m.EMP_UAN_Number == null || m.EMP_UAN_Number == "").ToList();                
            }
            //else
            //{
            //    list = list.Where(m => m.EMP_UAN_Number != null && m.EMP_UAN_Number != "").ToList();
            //}            
            if (EMP_ESIC_Number == true)
            {
                list = list.Where(m => m.EMP_ESIC_Number == null || m.EMP_ESIC_Number == "").ToList();              
            }
            //else
            //{
            //    list = list.Where(m => m.EMP_ESIC_Number != null && m.EMP_ESIC_Number != "").ToList();
            //}
            return list;
        }

    }
}

