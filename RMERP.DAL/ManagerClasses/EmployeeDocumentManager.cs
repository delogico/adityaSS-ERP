using RMERP.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMERP.DAL.ManagerClasses
{
    public class EmployeeDocumentManager
    {
        RMERPContext _context;
        public EmployeeDocumentManager(RMERPContext context)
        {
            _context = context;
        }
        public List<Employee_Document> GetEmployeeDocuments()
        {
            return _context.Employee_Documents.ToList();
        }
        public List<Employee_Document> GetDocumentsByEmpId(int EMP_Id)
        {
            return _context.Employee_Documents.Where(m => m.EMP_Id.Equals(EMP_Id)).ToList();
        }
        public Employee_Document GetEmployeeDocumenyById(int EMD_Id)
        {
            return _context.Employee_Documents.Find(EMD_Id);
        }
        public int AddEmployeeDocument(Employee_Document document)
        {
            int EMD_Id = 0;
            try
            {
                _context.Employee_Documents.Add(document);
                _context.SaveChanges();
                EMD_Id = document.EMD_Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return EMD_Id;
        }
        public string DeleteEmployeeDocument(int EMD_Id)
        {
            string res = string.Empty;
            try
            {
                Employee_Document doc = _context.Employee_Documents.Find(EMD_Id);
                _context.Employee_Documents.Remove(doc);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        }
    }
}
