using RMERP.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMERP.DAL.ManagerClasses
{
    public class DocumentTypesManager
    {
        RMERPContext _context;
        public DocumentTypesManager(RMERPContext context)
        {
            _context = context;
        }
        public List<Document_Types> GetDocumentTypes()
        {
            List<Document_Types> types = new List<Document_Types>();
            types = _context.Document_Types.ToList();
            return types;
        }
    }
}
