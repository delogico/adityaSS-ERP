using Microsoft.Extensions.Configuration;
using RMERP.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMERP.DAL.ManagerClasses
{
    public class EmployeeAllowanceManager
    {
        RMERPContext _contaxt;
        public IConfiguration Configuration;
        public EmployeeAllowanceManager(RMERPContext contaxt)
        {
            _contaxt = contaxt;
        }
        public List<Allowances> GetAllowanceList()
        {
            List<Allowances> list = new List<Allowances>();
            list = _contaxt.Allowances.ToList();
            return list;
        }

        public string AddEditRequirement_Allowances(List<Client_Requirement_Allowances> CRA)
        {
            string res = string.Empty;
            try
            {
                foreach(var item in CRA)
                {
                    _contaxt.Client_Requirement_Allowances.Add(item);
                    _contaxt.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        }
    }
}
