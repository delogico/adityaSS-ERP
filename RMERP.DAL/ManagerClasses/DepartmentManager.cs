using RMERP.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace RMERP.DAL.ManagerClasses
{
    public class DepartmentManager
    {
        RMERPContext _context;
        public DepartmentManager(RMERPContext context)
        {
            _context = context;
        }
        public IEnumerable<Departments> getDepartmentList()
        {
            IEnumerable<Departments> listDept = _context.Departments.OrderBy(m=>m.DEPT_Title).ToList();
            return listDept;
        }
    }
}
