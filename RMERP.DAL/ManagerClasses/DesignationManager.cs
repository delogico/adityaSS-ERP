using Microsoft.EntityFrameworkCore;
using RMERP.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMERP.DAL.ViewModel;

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

        public IEnumerable<Designations> getRemainingDesignationsList(int CLI_Id)
        {
            IEnumerable<Designations> lstDesignationsOfClient = getDesignationsListByClientID(CLI_Id);
            IEnumerable<Designations> lstDesignations = getDesignationsList();
            return lstDesignations.Where(p => !lstDesignationsOfClient.Any(p2 => p2.DES_Id == p.DES_Id));
        }
        public IEnumerable<Designations> getDesignationsListByClientID(int clientID)
        {      
            var dess = _context.Client_Requirements.Include(m => m.DES_).Where(m => m.CLI_Id.Equals(clientID)).Select(m => new Designations() { DES_Id = m.DES_Id, DES_Title = m.DES_.DES_Title });
            IEnumerable<Designations> desList = dess.Distinct();
            return desList.ToList();
        }
        public IEnumerable<AssignEmployeeVM> getDesignationsListInVM(int clientID)
        {            
            var desListt = _context.Client_Requirements.Where(m=>m.CRI_Active==true).Include(m => m.DES_).Where(m => m.CLI_Id.Equals(clientID)).Select(m => new AssignEmployeeVM() { DES_Id = m.DES_Id, CRI_Id = m.CRI_Id, DES_Title = m.DES_.DES_Title });
            IEnumerable<AssignEmployeeVM> desList = desListt.Distinct();
            return desList.ToList();
        }
        public string GetDesignationsById(int desId)
        {
            return _context.Designations.Find(desId).DES_Title;
        }
       public int getDesignationIdForAttandance(int CLI_Id,int EMP_Id)
        {
            int i = 0;
            try
            {
                i = _context.Clients_Employees.Where(m => m.CLI_Id.Equals(CLI_Id) && m.EMP_Id.Equals(EMP_Id)).FirstOrDefault().DES_Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return i;
        }
    }
}
