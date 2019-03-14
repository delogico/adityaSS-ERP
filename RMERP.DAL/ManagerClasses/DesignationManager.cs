using Microsoft.EntityFrameworkCore;
using RMERP.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMERP.DAL.ManagerClasses
{
    public class DesignationManager
    {
        RMERPContext _context;
        public DesignationManager(RMERPContext context)
        {
            _context = context;
        }
        public IEnumerable<Designations> getDesignationsList()
        {
            IEnumerable<Designations> ListDesignations = _context.Designations.OrderBy(m => m.DES_Title).ToList();
            return ListDesignations;
        }
        public IEnumerable<Designations> getDesignationsListByClientID(int clientID)
        {      
            var dess = _context.Client_Requirements.Include(m => m.DES_).Where(m => m.CLI_Id.Equals(clientID)).Select(m => new Designations() { DES_Id = m.DES_Id, DES_Title = m.DES_.DES_Title });
            IEnumerable<Designations> desList = dess.Distinct();
            return desList.ToList();
        }
        public string GetDesignationsById(int desId)
        {
            return _context.Designations.Find(desId).DES_Title;
        }
    }
}
