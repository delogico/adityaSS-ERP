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
        public List<Allowance> GetAllowanceList()
        {
            List<Allowance> list = new List<Allowance>();
            list = _contaxt.Allowances.ToList();
            return list;
        }
        public List<Client_Requirement_Allowance> GetClient_Requirement_AllowanceList(int CRI_Id)
        {
            List<Client_Requirement_Allowance> list = new List<Client_Requirement_Allowance>();
            list = _contaxt.Client_Requirement_Allowances.Where(m => m.CRI_Id.Equals(CRI_Id)).ToList();
            return list;
        }
        public Allowance GetAllowanceById(int ALL_Id)
        {
            Allowance allowances = new Allowance();
            allowances = _contaxt.Allowances.Find(ALL_Id);
            return allowances;
        }
        public Wage_Register_Allowances_1 GetAllowances_ByCLE_1(int WAG_Id, int CLE_Id, int CLI_Id)
        {
            Wage_Register_Allowances_1 allowances_1 = new Wage_Register_Allowances_1();
            allowances_1 = _contaxt.Wage_Register_Allowances_1s.Include(M => M.CLE).ThenInclude(M => M.EMP).Where(m => m.WAG_Id.Equals(WAG_Id) && m.CLE_Id.Equals(CLE_Id) && m.CLE.CLI_Id.Equals(CLI_Id)).FirstOrDefault();
            return allowances_1;
        }
        public Wage_Register_Allowances_2 GetAllowances_ByCLE_2(int WAG_Id, int CLE_Id, int CLI_Id)
        {
            Wage_Register_Allowances_2 allowances_2 = new Wage_Register_Allowances_2();
            allowances_2 = _contaxt.Wage_Register_Allowances_2s.Include(M => M.CLE).ThenInclude(M => M.EMP).Where(m => m.WAG_Id.Equals(WAG_Id) && m.CLE_Id.Equals(CLE_Id) && m.CLE.CLI_Id.Equals(CLI_Id)).FirstOrDefault();
            return allowances_2;
        }
        public Wage_Register_Allowances_3 GetAllowances_ByCLE_3(int WAG_Id, int CLE_Id, int CLI_Id)
        {
            Wage_Register_Allowances_3 allowances_3 = new Wage_Register_Allowances_3();
            allowances_3 = _contaxt.Wage_Register_Allowances_3s.Include(M => M.CLE).ThenInclude(M => M.EMP).Where(m => m.WAG_Id.Equals(WAG_Id) && m.CLE_Id.Equals(CLE_Id) && m.CLE.CLI_Id.Equals(CLI_Id)).FirstOrDefault();
            return allowances_3;
        }
        public Wage_Register_Allowances_4 GetAllowances_ByCLE_4(int WAG_Id, int CLE_Id, int CLI_Id)
        {
            Wage_Register_Allowances_4 allowances_4 = new Wage_Register_Allowances_4();
            allowances_4 = _contaxt.Wage_Register_Allowances_4s.Include(M => M.CLE).ThenInclude(M => M.EMP).Where(m => m.WAG_Id.Equals(WAG_Id) && m.CLE_Id.Equals(CLE_Id) && m.CLE.CLI_Id.Equals(CLI_Id)).FirstOrDefault();
            return allowances_4;
        }
        public Wage_Register_Allowances_5 GetAllowances_ByCLE_5(int WAG_Id, int CLE_Id, int CLI_Id)
        {
            Wage_Register_Allowances_5 allowances_5 = new Wage_Register_Allowances_5();
            allowances_5 = _contaxt.Wage_Register_Allowances_5s.Include(M => M.CLE).ThenInclude(M => M.EMP).Where(m => m.WAG_Id.Equals(WAG_Id) && m.CLE_Id.Equals(CLE_Id) && m.CLE.CLI_Id.Equals(CLI_Id)).FirstOrDefault();
            return allowances_5;
        }
        public Wage_Register_Allowances_6 GetAllowances_ByCLE_6(int WAG_Id, int CLE_Id, int CLI_Id)
        {
            return _contaxt.Wage_Register_Allowances_6s.Include(M => M.CLE).ThenInclude(M => M.EMP).Where(m => m.WAG_Id.Equals(WAG_Id) && m.CLE_Id.Equals(CLE_Id) && m.CLE.CLI_Id.Equals(CLI_Id)).FirstOrDefault();
        }
        public Wage_Register_Allowances_7 GetAllowances_ByCLE_7(int WAG_Id, int CLE_Id, int CLI_Id)
        {
            return _contaxt.Wage_Register_Allowances_7s.Include(M => M.CLE).ThenInclude(M => M.EMP).Where(m => m.WAG_Id.Equals(WAG_Id) && m.CLE_Id.Equals(CLE_Id) && m.CLE.CLI_Id.Equals(CLI_Id)).FirstOrDefault();
        }
        public Wage_Register_Allowances_8 GetAllowances_ByCLE_8(int WAG_Id, int CLE_Id, int CLI_Id)
        {
            return _contaxt.Wage_Register_Allowances_8s.Include(M => M.CLE).ThenInclude(M => M.EMP).Where(m => m.WAG_Id.Equals(WAG_Id) && m.CLE_Id.Equals(CLE_Id) && m.CLE.CLI_Id.Equals(CLI_Id)).FirstOrDefault();
        }
        public Wage_Register_Allowances_9 GetAllowances_ByCLE_9(int WAG_Id, int CLE_Id, int CLI_Id)
        {
            return _contaxt.Wage_Register_Allowances_9s.Include(M => M.CLE).ThenInclude(M => M.EMP).Where(m => m.WAG_Id.Equals(WAG_Id) && m.CLE_Id.Equals(CLE_Id) && m.CLE.CLI_Id.Equals(CLI_Id)).FirstOrDefault();
        }
        public Wage_Register_Allowances_10 GetAllowances_ByCLE_10(int WAG_Id, int CLE_Id, int CLI_Id)
        {
            return _contaxt.Wage_Register_Allowances_10s.Include(M => M.CLE).ThenInclude(M => M.EMP).Where(m => m.WAG_Id.Equals(WAG_Id) && m.CLE_Id.Equals(CLE_Id) && m.CLE.CLI_Id.Equals(CLI_Id)).FirstOrDefault();
        }

        public Wage_Register_Allowances_1 GetAllowancesById_1(int WRA_Id_1)
        {
            return _contaxt.Wage_Register_Allowances_1s.Include(M => M.CLE).ThenInclude(M => M.EMP).Where(m => m.WRA_Id_1.Equals(WRA_Id_1)).FirstOrDefault();
        }
        public Wage_Register_Allowances_2 GetAllowancesById_2(int WRA_Id_2)
        {
            return _contaxt.Wage_Register_Allowances_2s.Include(M => M.CLE).ThenInclude(M => M.EMP).Where(m => m.WRA_Id_2.Equals(WRA_Id_2)).FirstOrDefault();
        }
        public Wage_Register_Allowances_3 GetAllowancesById_3(int WRA_Id_3)
        {
            return _contaxt.Wage_Register_Allowances_3s.Include(M => M.CLE).ThenInclude(M => M.EMP).Where(m => m.WRA_Id_3.Equals(WRA_Id_3)).FirstOrDefault();
        }
        public Wage_Register_Allowances_4 GetAllowancesById_4(int WRA_Id_4)
        {
            return _contaxt.Wage_Register_Allowances_4s.Include(M => M.CLE).ThenInclude(M => M.EMP).Where(m => m.WRA_Id_4.Equals(WRA_Id_4)).FirstOrDefault();
        }
        public Wage_Register_Allowances_5 GetAllowancesById_5(int WRA_Id_5)
        {
            return _contaxt.Wage_Register_Allowances_5s.Include(M => M.CLE).ThenInclude(M => M.EMP).Where(m => m.WRA_Id_5.Equals(WRA_Id_5)).FirstOrDefault();
        }
        public Wage_Register_Allowances_6 GetAllowancesById_6(int WRA_Id_6)
        {
            return _contaxt.Wage_Register_Allowances_6s.Include(M => M.CLE).ThenInclude(M => M.EMP).Where(m => m.WRA_Id_6.Equals(WRA_Id_6)).FirstOrDefault();
        }
        public Wage_Register_Allowances_7 GetAllowancesById_7(int WRA_Id_7)
        {
            return _contaxt.Wage_Register_Allowances_7s.Include(M => M.CLE).ThenInclude(M => M.EMP).Where(m => m.WRA_Id_7.Equals(WRA_Id_7)).FirstOrDefault();
        }
        public Wage_Register_Allowances_8 GetAllowancesById_8(int WRA_Id_8)
        {
            return _contaxt.Wage_Register_Allowances_8s.Include(M => M.CLE).ThenInclude(M => M.EMP).Where(m => m.WRA_Id_8.Equals(WRA_Id_8)).FirstOrDefault();
        }
        public Wage_Register_Allowances_9 GetAllowancesById_9(int WRA_Id_9)
        {
            return _contaxt.Wage_Register_Allowances_9s.Include(M => M.CLE).ThenInclude(M => M.EMP).Where(m => m.WRA_Id_9.Equals(WRA_Id_9)).FirstOrDefault();
        }
        public Wage_Register_Allowances_10 GetAllowancesById_10(int WRA_Id_10)
        {
            return _contaxt.Wage_Register_Allowances_10s.Include(M => M.CLE).ThenInclude(M => M.EMP).Where(m => m.WRA_Id_10.Equals(WRA_Id_10)).FirstOrDefault();
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
                        _contaxt.Wage_Register_Allowances_1s.Update(allowance);
                    else
                        _contaxt.Wage_Register_Allowances_1s.Add(allowance);
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
                        _contaxt.Wage_Register_Allowances_2s.Update(allowance);
                    else
                        _contaxt.Wage_Register_Allowances_2s.Add(allowance);
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
                        _contaxt.Wage_Register_Allowances_3s.Update(allowance);
                    else
                        _contaxt.Wage_Register_Allowances_3s.Add(allowance);
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
                        _contaxt.Wage_Register_Allowances_4s.Update(allowance);
                    else
                        _contaxt.Wage_Register_Allowances_4s.Add(allowance);
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
                        _contaxt.Wage_Register_Allowances_5s.Update(allowance);
                    else
                        _contaxt.Wage_Register_Allowances_5s.Add(allowance);
                }
                _contaxt.SaveChanges();
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        }
        public string UpdateAmount_6(List<Wage_Register_Allowances_6> allowances)
        {
            string res = string.Empty;
            try
            {
                foreach (Wage_Register_Allowances_6 allowance in allowances)
                {
                    if (allowance.WRA_Id_6 > 0)
                        _contaxt.Wage_Register_Allowances_6s.Update(allowance);
                    else
                        _contaxt.Wage_Register_Allowances_6s.Add(allowance);
                }
                _contaxt.SaveChanges();
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        }
        public string UpdateAmount_7(List<Wage_Register_Allowances_7> allowances)
        {
            string res = string.Empty;
            try
            {
                foreach (Wage_Register_Allowances_7 allowance in allowances)
                {
                    if (allowance.WRA_Id_7 > 0)
                        _contaxt.Wage_Register_Allowances_7s.Update(allowance);
                    else
                        _contaxt.Wage_Register_Allowances_7s.Add(allowance);
                }
                _contaxt.SaveChanges();
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        }
        public string UpdateAmount_8(List<Wage_Register_Allowances_8> allowances)
        {
            string res = string.Empty;
            try
            {
                foreach (Wage_Register_Allowances_8 allowance in allowances)
                {
                    if (allowance.WRA_Id_8 > 0)
                        _contaxt.Wage_Register_Allowances_8s.Update(allowance);
                    else
                        _contaxt.Wage_Register_Allowances_8s.Add(allowance);
                }
                _contaxt.SaveChanges();
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        }
        public string UpdateAmount_9(List<Wage_Register_Allowances_9> allowances)
        {
            string res = string.Empty;
            try
            {
                foreach (Wage_Register_Allowances_9 allowance in allowances)
                {
                    if (allowance.WRA_Id_9 > 0)
                        _contaxt.Wage_Register_Allowances_9s.Update(allowance);
                    else
                        _contaxt.Wage_Register_Allowances_9s.Add(allowance);
                }
                _contaxt.SaveChanges();
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        }
        public string UpdateAmount_10(List<Wage_Register_Allowances_10> allowances)
        {
            string res = string.Empty;
            try
            {
                foreach (Wage_Register_Allowances_10 allowance in allowances)
                {
                    if (allowance.WRA_Id_10 > 0)
                        _contaxt.Wage_Register_Allowances_10s.Update(allowance);
                    else
                        _contaxt.Wage_Register_Allowances_10s.Add(allowance);
                }
                _contaxt.SaveChanges();
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        }


        public List<Wage_Register_Allowance> GetEmployeeAllowances_WAR_Id(int WAR_Id)
        {
            return _contaxt.Wage_Register_Allowances.Where(wra => wra.WAR_Id.Equals(WAR_Id)).ToList();
        }
    }
}
