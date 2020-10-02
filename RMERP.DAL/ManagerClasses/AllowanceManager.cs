using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RMERP.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMERP.DAL.ManagerClasses
{
    public class AllowanceManager
    {
        RMERPContext _contaxt;
        public IConfiguration Configuration;
        public AllowanceManager(RMERPContext contaxt)
        {
            _contaxt = contaxt;
        }
        public List<Allowances> GetAllowanceList()
        {
            List<Allowances> list = new List<Allowances>();
            list = _contaxt.Allowances.ToList();
            return list;
        }
        public List<Client_Requirement_Allowances> GetClient_Requirement_AllowanceList(int CRI_Id)
        {
            List<Client_Requirement_Allowances> list = new List<Client_Requirement_Allowances>();
            list = _contaxt.Client_Requirement_Allowances.Where(m=>m.CRI_Id.Equals(CRI_Id)).ToList();
            return list;
        }
        public Allowances GetAllowanceById(int ALL_Id)
        {
            Allowances allowances = new Allowances();
            allowances = _contaxt.Allowances.Find(ALL_Id);
            return allowances;
        }
        public Wage_Register_Allowances_1 GetAllowances_ByCLE_1(int WAG_Id, int CLE_Id, int CLI_Id)
        {
            Wage_Register_Allowances_1 allowances_1 = new Wage_Register_Allowances_1();
            allowances_1 = _contaxt.Wage_Register_Allowances_1.Include(M => M.CLE_).ThenInclude(M => M.EMP_).Where(m => m.WAG_Id.Equals(WAG_Id) && m.CLE_Id.Equals(CLE_Id) && m.CLE_.CLI_Id.Equals(CLI_Id)).FirstOrDefault();
            return allowances_1;
        }
        public Wage_Register_Allowances_2 GetAllowances_ByCLE_2(int WAG_Id, int CLE_Id, int CLI_Id)
        {
            Wage_Register_Allowances_2 allowances_2 = new Wage_Register_Allowances_2();
            allowances_2 = _contaxt.Wage_Register_Allowances_2.Include(M => M.CLE_).ThenInclude(M => M.EMP_).Where(m => m.WAG_Id.Equals(WAG_Id) && m.CLE_Id.Equals(CLE_Id) && m.CLE_.CLI_Id.Equals(CLI_Id)).FirstOrDefault();
            return allowances_2;
        }
        public Wage_Register_Allowances_3 GetAllowances_ByCLE_3(int WAG_Id, int CLE_Id, int CLI_Id)
        {
            Wage_Register_Allowances_3 allowances_3 = new Wage_Register_Allowances_3();
            allowances_3 = _contaxt.Wage_Register_Allowances_3.Include(M => M.CLE_).ThenInclude(M => M.EMP_).Where(m => m.WAG_Id.Equals(WAG_Id) && m.CLE_Id.Equals(CLE_Id) && m.CLE_.CLI_Id.Equals(CLI_Id)).FirstOrDefault();
            return allowances_3;
        }
        public Wage_Register_Allowances_4 GetAllowances_ByCLE_4(int WAG_Id, int CLE_Id, int CLI_Id)
        {
            Wage_Register_Allowances_4 allowances_4 = new Wage_Register_Allowances_4();
            allowances_4 = _contaxt.Wage_Register_Allowances_4.Include(M => M.CLE_).ThenInclude(M => M.EMP_).Where(m => m.WAG_Id.Equals(WAG_Id) && m.CLE_Id.Equals(CLE_Id) && m.CLE_.CLI_Id.Equals(CLI_Id)).FirstOrDefault();
            return allowances_4;
        }
        public Wage_Register_Allowances_5 GetAllowances_ByCLE_5(int WAG_Id, int CLE_Id, int CLI_Id)
        {
            Wage_Register_Allowances_5 allowances_5 = new Wage_Register_Allowances_5();
            allowances_5 = _contaxt.Wage_Register_Allowances_5.Include(M => M.CLE_).ThenInclude(M => M.EMP_).Where(m => m.WAG_Id.Equals(WAG_Id) && m.CLE_Id.Equals(CLE_Id) && m.CLE_.CLI_Id.Equals(CLI_Id)).FirstOrDefault();
            return allowances_5;
        }

        public Wage_Register_Allowances_1 GetAllowancesById_1(int WRA_Id_1)
        {
            return _contaxt.Wage_Register_Allowances_1.Include(M => M.CLE_).ThenInclude(M => M.EMP_).Where(m => m.WRA_Id_1.Equals(WRA_Id_1)).FirstOrDefault();
        }
        public Wage_Register_Allowances_2 GetAllowancesById_2(int WRA_Id_2)
        {
            return _contaxt.Wage_Register_Allowances_2.Include(M => M.CLE_).ThenInclude(M => M.EMP_).Where(m => m.WRA_Id_2.Equals(WRA_Id_2)).FirstOrDefault();
        }
        public Wage_Register_Allowances_3 GetAllowancesById_3(int WRA_Id_3)
        {
            return _contaxt.Wage_Register_Allowances_3.Include(M => M.CLE_).ThenInclude(M => M.EMP_).Where(m => m.WRA_Id_3.Equals(WRA_Id_3)).FirstOrDefault();
        }
        public Wage_Register_Allowances_4 GetAllowancesById_4(int WRA_Id_4)
        {
            return _contaxt.Wage_Register_Allowances_4.Include(M => M.CLE_).ThenInclude(M => M.EMP_).Where(m => m.WRA_Id_4.Equals(WRA_Id_4)).FirstOrDefault();
        }
        public Wage_Register_Allowances_5 GetAllowancesById_5(int WRA_Id_5)
        {
            return _contaxt.Wage_Register_Allowances_5.Include(M => M.CLE_).ThenInclude(M => M.EMP_).Where(m => m.WRA_Id_5.Equals(WRA_Id_5)).FirstOrDefault();
        }
        //public T GetAllowancesById<T>(int id) 
        //{            
        //    return _contaxt.T.Where(m => m.WRA_Id_1.Equals(id)).FirstOrDefault();
        //}
        public string UpdateAmount_1(List<Wage_Register_Allowances_1> allowances)
        {
            string res = string.Empty;
            try
            {
                foreach (Wage_Register_Allowances_1 allowance in allowances)
                {
                    if (allowance.WRA_Id_1 > 0)
                        _contaxt.Wage_Register_Allowances_1.Update(allowance);
                    else
                        _contaxt.Wage_Register_Allowances_1.Add(allowance);
                }
                _contaxt.SaveChanges();
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        }
        public string UpdateAmount_2(List<Wage_Register_Allowances_2> allowances)
        {
            string res = string.Empty;
            try
            {
                foreach (Wage_Register_Allowances_2 allowance in allowances)
                {
                    if (allowance.WRA_Id_2 > 0)
                        _contaxt.Wage_Register_Allowances_2.Update(allowance);
                    else
                        _contaxt.Wage_Register_Allowances_2.Add(allowance);
                }
                _contaxt.SaveChanges();
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        }
        public string UpdateAmount_3(List<Wage_Register_Allowances_3> allowances)
        {
            string res = string.Empty;
            try
            {
                foreach (Wage_Register_Allowances_3 allowance in allowances)
                {
                    if (allowance.WRA_Id_3 > 0)
                        _contaxt.Wage_Register_Allowances_3.Update(allowance);
                    else
                        _contaxt.Wage_Register_Allowances_3.Add(allowance);
                }
                _contaxt.SaveChanges();
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        }
        public string UpdateAmount_4(List<Wage_Register_Allowances_4> allowances)
        {
            string res = string.Empty;
            try
            {
                foreach (Wage_Register_Allowances_4 allowance in allowances)
                {
                    if (allowance.WRA_Id_4 > 0)
                        _contaxt.Wage_Register_Allowances_4.Update(allowance);
                    else
                        _contaxt.Wage_Register_Allowances_4.Add(allowance);
                }
                _contaxt.SaveChanges();
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        }
        public string UpdateAmount_5(List<Wage_Register_Allowances_5> allowances)
        {
            string res = string.Empty;
            try
            {
                foreach (Wage_Register_Allowances_5 allowance in allowances)
                {
                    if (allowance.WRA_Id_5 > 0)
                        _contaxt.Wage_Register_Allowances_5.Update(allowance);
                    else
                        _contaxt.Wage_Register_Allowances_5.Add(allowance);
                }
                _contaxt.SaveChanges();
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        }
    }
}
