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
            var dess = _context.Client_Requirements.Where(m=>m.CRI_Active.Equals(true)).Include(m => m.DES_).Where(m => m.CLI_Id.Equals(clientID)).Select(m => new Designations() { DES_Id = m.DES_Id, DES_Title = m.DES_.DES_Title });
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
        public Designations GetDesignationById(int desId)
        {
            return _context.Designations.Find(desId);
        }
        public int getDesignationIdForAttandance(int CLI_Id,int EMP_Id,DateTime monthDate)
        {
            DateTime lastDate = new DateTime(monthDate.Year, monthDate.Month, 1).AddMonths(1).AddDays(-1);
            int i = 0;
            try
            {
                i = _context.Clients_Employees.Where(m => m.CLI_Id.Equals(CLI_Id) && m.EMP_Id.Equals(EMP_Id) && m.CLE_RegisteredOn.Date <= lastDate.Date && (m.CLE_UnassignedOn == null || m.CLE_UnassignedOn >= lastDate.Date)).FirstOrDefault().DES_Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return i;
        }
        public string saveEditDesignation(Designations designations)
        {
            string res = string.Empty;
            try
            {
                if (designations.DES_Id > 0)
                {
                    _context.Designations.Update(designations);
                }
                else
                {
                    _context.Designations.Add(designations);
                }
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
