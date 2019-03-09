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
            IEnumerable<Designations> ListDesignations = _context.Designations.OrderBy(m => m.DesTitle).ToList();
            return ListDesignations;
        }
        public IEnumerable<Designations> getDesignationsListByClientID(int clientID)
        {      
            var dess = _context.ClientRequirements.Include(m => m.Des).Where(m => m.CliId.Equals(clientID)).Select(m => new Designations() { DesId = m.DesId, DesTitle = m.Des.DesTitle });
            IEnumerable<Designations> desList = dess.Distinct();
            return desList.ToList();
        }
        public string GetDesignationsById(int desId)
        {
            return _context.Designations.Find(desId).DesTitle;
        }
    }
}
