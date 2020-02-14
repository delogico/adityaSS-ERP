using RMERP.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using RMERP.DAL.App_Code;
using RMERP.DAL.ViewModel;
using RMERP.DAL.Mappers;

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
                res = ex.Message + " More => " + ex.InnerException.Message;
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
            return _context.Employees.Where(m => m.EMP_Id.Equals(EMP_Id)).Include(e => e.Employee_Advance).ThenInclude(m=>m.WAG_Id_Closed_OnNavigation).Include(m => m.Employee_Documents).Include(m => m.Wage_Register_Advances).Include(m => m.FRM_).FirstOrDefault();
        }
        public void ActiveEmployee(int EmpId, int EMP_Reason_Code, DateTime date, int AdminId, out string actionMessage)
        {
            Employees employees = new Employees();
            try
            {
                employees = _context.Employees.Find(EmpId);
                employees.EMP_ReasonCode = EMP_Reason_Code;
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
                    //employee_Advance.ADV_RegisteredOn = ProjectUtils.DateNow();
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

        public bool IsAssignedEmployee(int EMP_Id)
        {
            List<Clients_Employees> list = _context.Clients_Employees.Where(m => m.EMP_Id.Equals(EMP_Id) && m.CLE_UnassignedOn != null).ToList();
            return list.Count() > 0;
        }

        public bool CheckExistingAadhar(string AadharNumber, int EMP_Id,int FRM_Id)
        {
            return _context.Employees.Any(m => m.EMP_Aadhar_Number.Equals(AadharNumber) && m.EMP_Id != EMP_Id && m.FRM_Id== FRM_Id);
        }

        public List<Employees> SearchEmployees(int FRM_Id, bool EMP_UAN_Number, bool EMP_ESIC_Number,string EMP_Aadhar_Number)
        {
            IQueryable<Employees> list = null;          
            if (FRM_Id > 0)
            {
                list = _context.Employees.Where(m => m.FRM_Id.Equals(FRM_Id)).Include(m => m.FRM_);
            }
            else
            {
                list = _context.Employees.Include(m => m.FRM_);
            }
            if (EMP_UAN_Number == true)
            {
                list = list.Where(m => m.EMP_UAN_Number == null || m.EMP_UAN_Number == "");
            }                      
            if (EMP_ESIC_Number == true)
            {
                list = list.Where(m => m.EMP_ESIC_Number == null || m.EMP_ESIC_Number == "");
            }
            if (!string.IsNullOrEmpty(EMP_Aadhar_Number))
            {
                list = list.Where(m => m.EMP_Aadhar_Number.Contains(EMP_Aadhar_Number));
            }
            return list.ToList();
        }

        public int GetTotalEmployees()
        {
            return _context.Employees.Where(m => m.EMP_IsActive.Equals(true)).Count();
        }

        public int GetTotalEmployees(int FRM_ID)
        {
            return _context.Employees.Where(m => m.EMP_IsActive.Equals(true) && m.FRM_Id.Equals(FRM_ID)).Count();
        }

        public List<States> GetStates()
        {
            return _context.States.Where(m=>m.COU_Id.Equals(105)).ToList();
        }
        public List<Cities_all> GetCities(int STA_Id)
        {
            return _context.Cities_all.Where(m => m.STA_Id.Equals(STA_Id)).ToList();
        }

        public bool RejoinEmployee(int EMP_Id, DateTime EMP_Rejoin_Date, int ADM_Id)
        {
            bool rejoinSuccess = false;
            Employees emp = GetEmployeeById(EMP_Id);
            IEnumerable<Employees> emps = _context.Employees.Where(m => m.EMP_Aadhar_Number.Equals(emp.EMP_Aadhar_Number)).OrderByDescending(m => m.EMP_InactivatedOn);
            if (emps.Count() > 0)
            {
                if (emps.First().EMP_InactivatedOn.HasValue)
                {
                    DateTime EMP_InactivatedOn_first = new DateTime(emps.First().EMP_InactivatedOn.Value.Year, emps.First().EMP_InactivatedOn.Value.Month, 1);
                    DateTime EMP_Rejoin_Date_first = new DateTime(EMP_Rejoin_Date.Year, EMP_Rejoin_Date.Month, 1);
                    if (EMP_InactivatedOn_first != EMP_Rejoin_Date_first) //emp can't assign on same month bcz we can't diff on wage
                    {
                        emp.EMP_IsActive = false;
                        emp.EMP_RejoinOn = EMP_Rejoin_Date;
                        _context.Employees.Update(emp);

                        Employees employee = new Employees();
                        employee = EmployeesMapper.MapMeObject(emp, ADM_Id);
                        employee.EMP_DateOfJoining = EMP_Rejoin_Date;

                        _context.Employees.Add(employee);
                        _context.SaveChanges();
                        rejoinSuccess = true;
                    }
                }
            }
            return rejoinSuccess;
        }

        public IEnumerable<Employees> EmployeeHistory(int EMP_Id)
        {
            string EMP_AadharNo = GetEmployeeById(EMP_Id).EMP_Aadhar_Number;
            return _context.Employees.Include(m=>m.FRM_).Where(m => m.EMP_Aadhar_Number.Equals(EMP_AadharNo));
        }
    }
}

