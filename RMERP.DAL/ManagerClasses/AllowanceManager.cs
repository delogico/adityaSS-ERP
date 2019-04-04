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
        
    }
}
