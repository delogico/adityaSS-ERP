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
    }
}
