using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using RMERP.DAL.Models;
//using RMERP.DAL.App_Code;
using RMERP.DAL.Helpers;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Threading.Tasks;
using System.Text;
using static RMERP.DAL.Helpers.ProjectUtils;

namespace RMERP.DAL.ManagerClasses
{
    public class ClientsManager
    {
        RMERPContext _contaxt;
        public IConfiguration Configuration;
        private static int CRI_Id;
        public ClientsManager(RMERPContext contaxt, IConfiguration configuration = null)
        {
            _contaxt = contaxt;
            Configuration = configuration;
        }

        public Tuple<string, int> saveAddEditClients(Clients clients)
        {
            string res = string.Empty;
            try
            {
                clients.CLI_IsActive = true;
                if (clients.CLI_Id > 0)
                {
                    _contaxt.Clients.Update(clients);
                }
                else
                {
                    // clients.CLI_RegisteredOn = ProjectUtils.DateNow();
                    _contaxt.Clients.Add(clients);
                }
                _contaxt.SaveChanges();
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return new Tuple<string, int>(res, clients.CLI_Id);
        }
        public IEnumerable<Clients> listClients(bool IsActive, int? FirmId, string Client = "")
        {
            IQueryable<Clients> ClientList = _contaxt.Clients.Include(m => m.CITY_).Include(m => m.FRM_).Include(m => m.Client_Contacts);
            if (FirmId.HasValue && FirmId != 0)
            {
                ClientList = ClientList.Where(m => m.FRM_Id == FirmId.Value);
            }

            if (string.IsNullOrEmpty(Client) || FirmId == 0)
            {
                ClientList = ClientList.Where(m => m.CLI_IsActive.Equals(IsActive));
            }
            else
            {
                ClientList = from s in ClientList
                             where EF.Functions.Like(s.CLI_Name, "%" + Client + "%")
                             select s;
            }
            return ClientList.ToList();
        }
        public Clients GetClientById(int CLI_Id)
        {
            Clients clients = new Clients();
            try
            {
                clients = _contaxt.Clients.Where(m => m.CLI_Id.Equals(CLI_Id)).Include(m => m.FRM_).First();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return clients;
        }
        public Clients GetClientByIdNoTracking(int CLI_Id)
        {
            Clients clients = new Clients();
            try
            {
                clients = _contaxt.Clients.Where(m => m.CLI_Id.Equals(CLI_Id)).Include(m => m.FRM_).AsNoTracking().First();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return clients;
        }
        public Client_Contacts GetClientContactsById(int CON_Id)
        {
            return _contaxt.Client_Contacts.Where(c => c.CON_Id.Equals(CON_Id)).Include(c => c.CLI_).FirstOrDefault();
        }
        public string saveAddEditContacts(Client_Contacts clientContacts)
        {
            string res = string.Empty;
            try
            {
                if (clientContacts.CON_isPrimary == true)
                {
                    List<Client_Contacts> list = _contaxt.Client_Contacts.Where(m => m.CLI_Id.Equals(clientContacts.CLI_Id) && m.CON_Id != clientContacts.CON_Id).ToList();
                    list.ForEach(m => m.CON_isPrimary = false);
                    _contaxt.SaveChanges();
                }
                if (clientContacts.CON_Id > 0)
                {
                    _contaxt.Client_Contacts.Update(clientContacts);
                }
                else
                {
                    _contaxt.Client_Contacts.Add(clientContacts);
                }
                _contaxt.SaveChanges();
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        }
        public List<Client_Contacts> GetClientContactsListById(int CLI_Id)
        {
            List<Client_Contacts> clientContactsList = new List<Client_Contacts>();
            try
            {
                clientContactsList = _contaxt.Client_Contacts.Include(m => m.CLI_).Where(m => m.CLI_Id.Equals(CLI_Id)).OrderByDescending(m => m.CON_isPrimary).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return clientContactsList;
        }
        public string deleteContacts(int ContactId)
        {
            string res = string.Empty;
            try
            {
                var ClientContact = _contaxt.Client_Contacts.Find(ContactId);
                _contaxt.Client_Contacts.Remove(ClientContact);
                _contaxt.SaveChanges();
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        }
        public string InActiveClient(int ClientId, int AdminId, bool Active, DateTime On)
        {
            string res = string.Empty;
            try
            {
                Clients client = new Clients();
                client = _contaxt.Clients.Find(ClientId);
                client.ADM_Id_InactivatedBy = AdminId;
                if (Active)
                {
                    client.CLI_IsActive = !Active;
                    client.CLI_InActivatedOn = On;
                }
                else
                {
                    client.CLI_InActivatedOn = On;
                }
                // client.CLI_IsActive = Convert.ToBoolean((Active.ToLower() == "false" ? "true" : "false"));
                _contaxt.Clients.Update(client);
                _contaxt.SaveChanges();

            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        }
        public void AddEditActivationHistory(Client_ActivationHistory activationHistory)
        {
            if (activationHistory.CAH_Id > 0)
            {
                _contaxt.Client_ActivationHistory.Update(activationHistory);
            }
            else
            {
                _contaxt.Client_ActivationHistory.Add(activationHistory);
            }
            _contaxt.SaveChanges();
        }

        public Client_ActivationHistory GetLatestActiveHistory(int CLI_Id)
        {
            return _contaxt.Client_ActivationHistory.Where(m => m.CLI_Id.Equals(CLI_Id) && m.CAH_InactiveOn == null).OrderByDescending(m => m.CAH_ActiveOn).FirstOrDefault();
        }
        public Client_ActivationHistory GetLatestHistory(int CLI_Id)
        {
            return _contaxt.Client_ActivationHistory.Where(m => m.CLI_Id.Equals(CLI_Id)).OrderByDescending(m => m.CAH_ActiveOn).FirstOrDefault();
        }
        public string ReActiveClient(int ClientId, DateTime On)
        {
            string res = string.Empty;
            try
            {
                Clients client = _contaxt.Clients.Find(ClientId);
                client.ADM_Id_InactivatedBy = null;

                client.CLI_IsActive = true;
                client.CLI_RegisteredOn = On;
                client.CLI_InActivatedOn = null;

                _contaxt.Clients.Update(client);
                _contaxt.SaveChanges();
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        }
        public string AddEditRequirement(Client_Requirements clientRequirements, List<Client_Requirement_Allowances> lst, int ADM_Id)
        {
            string res = string.Empty;
            try
            {
                if (clientRequirements.CRI_Id > 0)
                {
                    List<Client_Requirements> list = _contaxt.Client_Requirements.Where(m => m.CLI_Id.Equals(clientRequirements.CLI_Id) && m.DES_Id.Equals(clientRequirements.DES_Id) && m.CRI_Active == true).ToList();
                    list.ForEach(m =>
                    {
                        m.CRI_Active = false;
                        m.CRI_InactivatedOn = clientRequirements.CRI_RegisteredOn.AddDays(-1);
                        m.ADM_Id_InactivatedBy = ADM_Id;
                    });
                    _contaxt.SaveChanges();
                    clientRequirements.CRI_Id = 0;
                    _contaxt.Client_Requirements.Add(clientRequirements);
                    _contaxt.SaveChanges();
                }
                else
                {
                    clientRequirements.CRI_Id = 0;
                    _contaxt.Client_Requirements.Add(clientRequirements);
                    _contaxt.SaveChanges();
                }
                CRI_Id = clientRequirements.CRI_Id;

                foreach (var item in lst)
                {
                    Client_Requirement_Allowances cra = new Client_Requirement_Allowances();
                    cra.CRI_Id = CRI_Id;
                    cra.ALL_Id = item.ALL_Id;
                    cra.CRA_Amount = item.CRA_Amount;
                    cra.CRA_DayswiseOrFull = item.CRA_DayswiseOrFull;
                    if (item.CRA_Id > 0)
                    {
                        cra.CRA_Id = item.CRA_Id;
                        _contaxt.Client_Requirement_Allowances.Update(cra);
                    }
                    else
                        _contaxt.Client_Requirement_Allowances.Add(cra);
                    _contaxt.SaveChanges();
                    cra = null;
                }
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }

            return res;
        }
        public void EditRequirementRegDate(int CRI_Id, DateTime RegDate)
        {
            Client_Requirements requirement = _contaxt.Client_Requirements.Find(CRI_Id);
            requirement.CRI_RegisteredOn = RegDate;
            _contaxt.Update(requirement);
            _contaxt.SaveChanges();
        }

        public string EditHistoryRequirement(Client_Requirements clientRequirements, List<Client_Requirement_Allowances> lst, List<Client_Requirement_Allowances> Removelst, int ADM_Id)
        {
            string res = string.Empty;
            try
            {
                if (clientRequirements.CRI_Id > 0)
                {
                    _contaxt.Client_Requirements.Update(clientRequirements);
                    _contaxt.SaveChanges();
                }
                CRI_Id = clientRequirements.CRI_Id;


                int[] existingId = _contaxt.Client_Requirement_Allowances.Where(m => m.CRI_Id.Equals(clientRequirements.CRI_Id)).Select(m => m.CRA_Id).ToArray();
                int[] NeedToRemoveId = existingId.Intersect(Removelst.Select(m => m.CRA_Id).ToArray()).ToArray();
                List<Client_Requirement_Allowances> needToRemove = _contaxt.Client_Requirement_Allowances.Where(m => NeedToRemoveId.Contains(m.CRA_Id)).ToList();

                if (needToRemove.Count() > 0)
                {
                    _contaxt.Client_Requirement_Allowances.RemoveRange(needToRemove);
                    _contaxt.SaveChanges();
                }

                foreach (var item in lst)
                {
                    Client_Requirement_Allowances cra = new Client_Requirement_Allowances();
                    cra.CRI_Id = CRI_Id;
                    cra.ALL_Id = item.ALL_Id;
                    cra.CRA_Amount = item.CRA_Amount;
                    cra.CRA_DayswiseOrFull = item.CRA_DayswiseOrFull;
                    if (item.CRA_Id > 0)
                    {
                        cra.CRA_Id = item.CRA_Id;
                        _contaxt.Client_Requirement_Allowances.Update(cra);
                    }
                    else
                        _contaxt.Client_Requirement_Allowances.Add(cra);
                    _contaxt.SaveChanges();
                    cra = null;
                }
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        }
        public Client_Requirements GetRequirementsById(int CRI_Id)
        {
            return _contaxt.Client_Requirements.Where(c => c.CRI_Id == CRI_Id).Include(m => m.Wage_Register).Include(c => c.DES_).Include(m => m.Client_Requirement_Allowances).FirstOrDefault();
        }

        public IEnumerable<Client_Requirements> GetClient_RequirementsList(int desId, int cliId, bool active = true)
        {
            IEnumerable<Client_Requirements> list = _contaxt.Client_Requirements.Include(m => m.CLI_).Include(m => m.DES_).Where(m => m.CLI_Id.Equals(cliId) && m.CRI_Active.Equals(active) && m.DES_Id.Equals(desId)).OrderByDescending(m => m.CRI_RegisteredOn).ThenByDescending(m => m.CRI_Id).ToList();
            return list;
        }
        public IEnumerable<Client_Requirements> GetClientRequirementsList(int desId, int cliId)
        {
            IEnumerable<Client_Requirements> list = _contaxt.Client_Requirements.Include(m => m.CLI_).Include(m => m.DES_).Where(m => m.CLI_Id.Equals(cliId) && m.DES_Id.Equals(desId)).OrderByDescending(m => m.CRI_RegisteredOn).ToList();
            return list;
        }

        public IEnumerable<Client_Requirements> GetClient_RequirementsofClient(int CLI_Id, bool active = true)
        {
            IEnumerable<Client_Requirements> list = _contaxt.Client_Requirements.Include(m => m.DES_).Where(m => m.CLI_Id.Equals(CLI_Id) && m.CRI_Active.Equals(active)).OrderByDescending(m => m.CRI_RegisteredOn).ToList();
            return list;
        }


        public string UpdateParameters(Clients clients)
        {
            string res = string.Empty;
            try
            {
                Clients client = _contaxt.Clients.Find(clients.CLI_Id);

                //client.CLI_Att_Month_End = clients.CLI_Att_Month_End;
                //client.CLI_Att_Month_Start = clients.CLI_Att_Month_Start;
                //client.CLI_Att_MonthReal = true;

                client.CLI_GST_Number = clients.CLI_GST_Number;
                client.CLI_GST_Rate = clients.CLI_GST_Rate;
                client.CLI_TDS_Rate = clients.CLI_TDS_Rate;
                client.CLI_HSN_Code = clients.CLI_HSN_Code;
                client.CLI_Total_WorkingDays = clients.CLI_Total_WorkingDays;
                client.CLI_No_Reduce_Days = clients.CLI_No_Reduce_Days;
                client.CLI_WorkingHours_In_Day = clients.CLI_WorkingHours_In_Day;

                client.CLI_Invoicing_Name = clients.CLI_Invoicing_Name;
                client.CLI_Invoicing_Address1 = clients.CLI_Invoicing_Address1;
                client.CLI_Invoicing_Address2 = clients.CLI_Invoicing_Address2;
                client.STA_Id = clients.STA_Id;
                client.CLI_Invoicing_City = clients.CLI_Invoicing_City;
                client.CLI_Invoicing_ZipCode = clients.CLI_Invoicing_ZipCode;
                client.CLI_Invoicing_Location = clients.CLI_Invoicing_Location;
                client.CLI_IsIGST = clients.CLI_IsIGST;
                client.CLI_IGST = clients.CLI_IGST;
                client.CLI_IsCGST = clients.CLI_IsCGST;
                client.CLI_CGST = clients.CLI_CGST;
                client.CLI_IsSGST = clients.CLI_IsSGST;
                client.CLI_SGST = clients.CLI_SGST;
                client.CLI_Place_Of_Supply = clients.CLI_Place_Of_Supply;

                client.CLI_PF_Employer_Cont_Rate = clients.CLI_PF_Employer_Cont_Rate;
                client.CLI_ESIC_Employer_Cont_Rate = clients.CLI_ESIC_Employer_Cont_Rate;
                client.CLI_EPF_Rate = clients.CLI_EPF_Rate;
                client.CLI_EPS_Rate = clients.CLI_EPS_Rate;
                client.CLI_MLWF_Contribution = clients.CLI_MLWF_Contribution;
                _contaxt.Clients.Update(client);
                _contaxt.SaveChanges();
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;

        }

        public IEnumerable<Employees> getActiveEmployeeList_NotAssignedYet(int CLI_Id, int FRM_Id)
        {
            return from t1 in _contaxt.Employees.Where(m => m.EMP_IsActive.Equals(true) && m.FRM_Id.Equals(FRM_Id) &&
                        !(from t2 in _contaxt.Clients_Employees.Where(ce => ce.CLI_Id.Equals(CLI_Id) && ce.CLE_UnassignedOn == null)
                          select t2.EMP_Id).Contains(m.EMP_Id))
                   select t1;
        }

        public IEnumerable<Employees> getEmployeeList(int FRM_Id)
        {
            return _contaxt.Employees.Where(m => m.FRM_Id.Equals(FRM_Id));
        }

        public string ClientEmployee(Clients_Employees clientsEmployees, int Old_DES_Id, int AdminId)
        {
            string res = string.Empty;
            if (clientsEmployees.CLE_RegisteredOn == null)
                clientsEmployees.CLE_RegisteredOn = ProjectUtils.DateNow();
            clientsEmployees.ADM_Id_RegisteredBy = AdminId;
            try
            {
                if (clientsEmployees.CLE_Id > 0 && (Old_DES_Id != clientsEmployees.DES_Id))
                {
                    List<Clients_Employees> list = _contaxt.Clients_Employees.Where(m => m.CLI_Id.Equals(clientsEmployees.CLI_Id) && m.EMP_Id.Equals(clientsEmployees.EMP_Id)).ToList();
                    list.ForEach(m =>
                    {
                        m.CLE_UnassignedOn = ProjectUtils.DateNow(); ;
                        m.ADM_Id_UnassignedBy = AdminId;
                    });
                    _contaxt.SaveChanges();
                    clientsEmployees.CLE_Id = 0;
                    _contaxt.Clients_Employees.Add(clientsEmployees);
                }
                else if (clientsEmployees.CLE_Id > 0 && Old_DES_Id.Equals(clientsEmployees.DES_Id))
                {
                    _contaxt.Clients_Employees.Update(clientsEmployees);
                }
                else
                {
                    _contaxt.Clients_Employees.Add(clientsEmployees);
                }
                _contaxt.SaveChanges();
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        }
        public IEnumerable<Clients_Employees> listClientsEmployees(int ClientId, string assign = null)
        {
            IEnumerable<Clients_Employees> list = null;
            if (assign == null)
            {
                list = _contaxt.Clients_Employees.Where(m => m.CLI_Id.Equals(ClientId)).Include(m => m.EMP_).Include(m => m.DES_).ToList();
            }
            else if (assign == Convert.ToString((int)Assign_Unassign.Assign))
            {
                list = _contaxt.Clients_Employees.Where(m => m.CLI_Id.Equals(ClientId) && m.CLE_UnassignedOn == null).Include(m => m.EMP_).Include(m => m.DES_).ToList();
            }
            else if (assign == Convert.ToString((int)Assign_Unassign.Unassign))
            {
                list = _contaxt.Clients_Employees.Where(m => m.CLI_Id.Equals(ClientId) && m.CLE_UnassignedOn != null).Include(m => m.EMP_).Include(m => m.DES_).ToList();
            }

            return list;
        }
        public IEnumerable<Clients_Employees> listActiveClientsEmployees(int ClientId, DateTime monthDate)
        {
            DateTime lastDate = new DateTime(monthDate.Year, monthDate.Month, 1).AddMonths(1).AddDays(-1);
            DateTime firstDate = new DateTime(monthDate.Year, monthDate.Month, 1);
            IEnumerable<Clients_Employees> list = _contaxt.Clients_Employees
                                                .Where(m => m.CLI_Id.Equals(ClientId)
                                                && m.CLE_RegisteredOn.Date <= lastDate.Date
                                                && (m.CLE_UnassignedOn == null || m.CLE_UnassignedOn >= lastDate.Date)
                                                && (m.EMP_.EMP_IsActive == true || (m.EMP_.EMP_IsActive == false && m.EMP_.EMP_InactivatedOn != null && (m.EMP_.EMP_InactivatedOn.Value.Date >= firstDate.Date))))
                                                .Include(m => m.EMP_).ThenInclude(m => m.Wage_Register_Advances).ThenInclude(m => m.WAG_)
                                                .Include(m => m.DES_).ToList();
            return list;
        }

        public IEnumerable<Clients_Employees> listActiveClientsEmployees_clients(int EMP_Id, DateTime monthDate)
        {
            DateTime lastDate = new DateTime(monthDate.Year, monthDate.Month, 1).AddMonths(1).AddDays(-1);
            DateTime firstDate = new DateTime(monthDate.Year, monthDate.Month, 1);
            IEnumerable<Clients_Employees> list = _contaxt.Clients_Employees
                                                .Where(m => m.EMP_Id.Equals(EMP_Id)
                                                && m.CLE_RegisteredOn.Date <= lastDate.Date
                                                && (m.CLE_UnassignedOn == null || m.CLE_UnassignedOn >= lastDate.Date)
                                                && (m.EMP_.EMP_IsActive == true || (m.EMP_.EMP_IsActive == false && m.EMP_.EMP_InactivatedOn != null && (m.EMP_.EMP_InactivatedOn.Value.Date >= firstDate.Date))))
                                                .Include(m => m.EMP_)
                                                .Include(m => m.DES_).ToList();
            return list;
        }

        //public IEnumerable<Clients_Employees> listClientsEmployees(int ClientId, int DES_Id)
        //{
        //    IEnumerable<Clients_Employees> list = _contaxt.Clients_Employees.Where(m => m.CLI_Id.Equals(ClientId) && m.DES_Id.Equals(DES_Id)).Include(m => m.EMP_).Include(m => m.DES_).ToList();
        //    return list;
        //}

        public IEnumerable<Clients_Employees> listActiveClientsEmployees(int ClientId, DateTime monthDate, int DES_Id)
        {
            DateTime lastDate = new DateTime(monthDate.Year, monthDate.Month, 1).AddMonths(1).AddDays(-1);
            DateTime firstDate = new DateTime(monthDate.Year, monthDate.Month, 1);
            IEnumerable<Clients_Employees> list = _contaxt.Clients_Employees
                                                .Where(m => m.CLI_Id.Equals(ClientId) && m.DES_Id.Equals(DES_Id)
                                                && m.CLE_RegisteredOn.Date <= lastDate.Date
                                                && (m.CLE_UnassignedOn == null || m.CLE_UnassignedOn >= lastDate.Date)
                                                && (m.EMP_.EMP_IsActive == true || (m.EMP_.EMP_IsActive == false && m.EMP_.EMP_InactivatedOn != null && (m.EMP_.EMP_InactivatedOn.Value.Date >= firstDate.Date))))
                                                .Include(m => m.EMP_).ThenInclude(m => m.Wage_Register_Advances).ThenInclude(m => m.WAG_)
                                                .Include(m => m.DES_).ToList();
            return list;
        }

        public Clients_Employees ClientEmployeeById(int CleId)
        {
            Clients_Employees clientsEmployees = _contaxt.Clients_Employees.Find(CleId);
            return clientsEmployees;
        }
        public string UnassignClientEmployee(int id, DateTime UnassignedOn, int Adm_Id)
        {
            string res = string.Empty;
            try
            {
                Clients_Employees clients_Employees = _contaxt.Clients_Employees.Find(id);
                clients_Employees.CLE_UnassignedOn = UnassignedOn;
                clients_Employees.ADM_Id_UnassignedBy = Adm_Id;
                _contaxt.Clients_Employees.Update(clients_Employees);
                _contaxt.SaveChanges();
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        }

        public string ReassignClientEmployee(int id)
        {
            string res = string.Empty;
            try
            {
                Clients_Employees clients_Employees = _contaxt.Clients_Employees.Find(id);
                clients_Employees.CLE_UnassignedOn = null;
                clients_Employees.ADM_Id_UnassignedBy = null;
                clients_Employees.CLE_ReassignedOn = ProjectUtils.DateNow();
                _contaxt.Clients_Employees.Update(clients_Employees);
                _contaxt.SaveChanges();
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        }

        #region commented by rinku on 14th feb 2020 as It seems like now no need for this functions
        //public string GetDesTitleByCriId(int criId)
        //{
        //    if (criId > 0)
        //    {
        //        var DesT = from a in _contaxt.Client_Requirements
        //                   join b in _contaxt.Designations on a.DES_Id equals b.DES_Id
        //                   where a.CRI_Id == criId
        //                   select new
        //                   {
        //                       b.DES_Title
        //                   };

        //        return DesT.ToList()[0].DES_Title;
        //    }
        //    else
        //    {
        //        return string.Empty;
        //    }

        //}
        //public IEnumerable<Clients_Employees> listClientsPerWag(bool IsActive, int wagId, int AdminId, int? firmId)
        //{
        //    IEnumerable<Clients_Employees> ClientList = null;
        //    int month = _contaxt.Wage_Process.Find(wagId).WAG_Month.Month;
        //    ClientList = _contaxt.Clients_Employees.Include(m => m.CLI_).Where(m => m.ADM_Id_RegisteredBy.Equals(AdminId) && m.CLI_.CLI_IsActive.Equals(IsActive) && m.CLE_RegisteredOn.Month.Equals(month));
        //    if (firmId != null)
        //    {
        //        ClientList = ClientList.Where(m => m.CLI_.FRM_Id.Equals(firmId));
        //    }

        //    var ClientList1 = from P in ClientList.Distinct().ToList() select new { P };

        //    return ClientList.ToList();
        //}

        //public IEnumerable<Clients> listClientsForAttandance(int? FirmId, bool IsActive, string Client = "")
        //{
        //    IQueryable<Clients> ClientList = null;
        //    return ClientList;
        //}
        //public Tuple<int, int> GetDateByClientID(int cliID)
        //{
        //    return new Tuple<int, int>(0, 0);
        //}
        //public List<Clients> GetClientsListByMonth(DateTime WageMonthDate)
        //{
        //    DateTime LastDate = new DateTime(WageMonthDate.Year, WageMonthDate.Month, 1).AddMonths(1).AddDays(-1);
        //    List<Clients> lstClient = new List<Clients>();

        //    var cliList = from a in _contaxt.Clients
        //                  where a.CLI_RegisteredOn.Date <= WageMonthDate.Date
        //                  select a;

        //    foreach (var cli in cliList)
        //    {
        //        if (cli.CLI_IsActive == false && cli.CLI_InActivatedOn >= WageMonthDate)
        //        {
        //            lstClient.Add(cli);
        //        }
        //        else if (cli.CLI_IsActive == true)
        //        {
        //            lstClient.Add(cli);
        //        }
        //    }

        //    return lstClient;
        //}
        //public int getClientRequirementId(int CLI_Id, int EMP_Id)
        //{
        //    var query =
        //        from cri in _contaxt.Client_Requirements
        //        join cle in _contaxt.Clients_Employees on cri.CLI_Id equals cle.CLI_Id
        //        where cle.EMP_Id == EMP_Id && cri.CLI_Id == CLI_Id
        //        select cri;
        //    return query.FirstOrDefault().CRI_Id;
        //}
        //public List<Client_Requirements> getClientRequirements()
        //{
        //    List<Client_Requirements> list = new List<Client_Requirements>();
        //    list = _contaxt.Client_Requirements.Include(m => m.Client_Requirement_Allowances).Where(m => m.CRI_Active.Equals(true)).ToList();
        //    return list;
        //}
        //public List<Clients> GetActiveClientForAttandanceReg(DateTime monthStartDate)
        //{
        //    DateTime lastDate = new DateTime(monthStartDate.Year, monthStartDate.Month, 1).AddMonths(1).AddDays(-1);
        //    IQueryable<Clients> cliList = from a in _contaxt.Clients.Include(m => m.Clients_Employees).Include(m => m.Attendance).Include(m => m.Client_Requirements)
        //                                  where
        //                                  a.CLI_RegisteredOn.Date <= monthStartDate.Date
        //                                  && ((a.CLI_IsActive == true) || (a.CLI_IsActive == false && a.CLI_InActivatedOn.Value.Date >= lastDate.Date))
        //                                  select a;

        //    return cliList.ToList();
        //}
        //public List<Clients> GetActiveClientByFirmId(DateTime wageDate, int FirmId)
        //{
        //    DateTime lastDate = new DateTime(wageDate.Year, wageDate.Month, 1).AddMonths(1).AddDays(-1);
        //    IQueryable<Clients> cliList = from a in _contaxt.Clients
        //                                  where a.CLI_RegisteredOn.Date <= lastDate.Date
        //                                  && a.FRM_Id.Equals(FirmId)
        //                                  && ((a.CLI_IsActive == true) || (a.CLI_IsActive == false && a.CLI_InActivatedOn.Value.Date >= lastDate.Date))
        //                                  select a;
        //    return cliList.ToList();
        //}
        #endregion

        public List<Clients> GetActiveClientOfMonthByFirmId(DateTime wageDate, int FirmId)
        {
            DateTime lastDate = new DateTime(wageDate.Year, wageDate.Month, 1).AddMonths(1).AddDays(-1);
            DateTime startdate = new DateTime(wageDate.Year, wageDate.Month, 1);
            IQueryable<Clients> cliList = from a in _contaxt.Clients
                                          join b in _contaxt.Client_ActivationHistory on a.CLI_Id equals b.CLI_Id
                                          where b.CAH_ActiveOn.Date <= lastDate.Date
                                          && a.FRM_Id.Equals(FirmId)
                                          && ((b.CAH_InactiveOn == null) || !(b.CAH_InactiveOn.Value.Date < startdate.Date))
                                          select a;
            return cliList.ToList();
        }

        public Client_Requirements getActiveClientRequirement(int CLI_Id, int DES_Id, DateTime WAG_Month)
        {
            DateTime LastDate = ProjectUtils.GetLastDateOfMonth(WAG_Month);
            return _contaxt.Client_Requirements.Where(r => r.CLI_Id == CLI_Id && r.DES_Id == DES_Id && r.CRI_RegisteredOn.Date <= LastDate.Date).OrderByDescending(m => m.CRI_RegisteredOn).ThenByDescending(m => m.CRI_Id).Take(1).Include(c => c.Client_Requirement_Allowances).ThenInclude(a => a.ALL_).FirstOrDefault();
        }

        public int GetClientIDByEmpID(int EMP_Id, DateTime WAG_Month)
        {
            int CLI_Id = 0;
            if (_contaxt.Clients_Employees.Where(m => m.EMP_Id.Equals(EMP_Id)).Count() > 0)
                CLI_Id = _contaxt.Clients_Employees.Where(m => m.EMP_Id.Equals(EMP_Id)).First().CLI_Id;
            return CLI_Id;
        }

        public int GetTotalClient()
        {
            return _contaxt.Clients.Where(m => m.CLI_IsActive.Equals(true)).Count();
        }
        public int GetTotalClient(int FRM_ID)
        {
            return _contaxt.Clients.Where(m => m.CLI_IsActive.Equals(true) && m.FRM_Id.Equals(FRM_ID)).Count();
        }

        public bool IsEmployeeWagedForClient(int EMP_Id, int CLI_Id, int DES_Id)
        {
            // int count = _contaxt.Attendance.Where(m => m.CLI_Id.Equals(CLI_Id) && m.EMP_Id.Equals(EMP_Id) && m.DES_Id.Equals(DES_Id)).Count();
            int count = _contaxt.Wage_Register.Where(m => m.CLI_Id.Equals(CLI_Id) && m.EMP_Id.Equals(EMP_Id)).Count();
            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string DeleteAssignEmployee(int CLE_Id, int CLI_Id, int EMP_Id, int DES_Id)
        {
            string res = string.Empty;
            try
            {
                List<Wage_Register_Canteen> canteen = _contaxt.Wage_Register_Canteen.Where(m => m.CLE_Id.Equals(CLE_Id)).ToList();
                if (canteen.Count() > 0)
                {
                    _contaxt.Wage_Register_Canteen.RemoveRange(canteen);
                }
                List<Wage_Register_Outstation> outstation = _contaxt.Wage_Register_Outstation.Where(m => m.CLE_Id.Equals(CLE_Id)).ToList();
                if (outstation.Count() > 0)
                {
                    _contaxt.Wage_Register_Outstation.RemoveRange(outstation);
                }
                List<Wage_Register_Performance> performance = _contaxt.Wage_Register_Performance.Where(m => m.CLE_Id.Equals(CLE_Id)).ToList();
                if (performance.Count() > 0)
                {
                    _contaxt.Wage_Register_Performance.RemoveRange(performance);
                }
                List<Attendance> attendance = _contaxt.Attendance.Where(m => m.CLI_Id.Equals(CLI_Id) && m.EMP_Id.Equals(EMP_Id) && m.DES_Id.Equals(DES_Id)).ToList();
                if (attendance.Count() > 0)
                {
                    _contaxt.Attendance.RemoveRange(attendance);
                }

                var employee = _contaxt.Clients_Employees.Find(CLE_Id);
                _contaxt.Clients_Employees.Remove(employee);
                _contaxt.SaveChanges();
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        }

        public string InactiveRequirement(int CRI_Id, int ADM_Id)
        {
            string res = string.Empty;
            try
            {
                Client_Requirements requirement = _contaxt.Client_Requirements.Find(CRI_Id);
                if (!this.IsActiveAssignedEmployee(requirement.CLI_Id, requirement.DES_Id))
                {
                    requirement.CRI_Active = false;
                    requirement.CRI_InactivatedOn = ProjectUtils.DateNow();
                    requirement.ADM_Id_InactivatedBy = ADM_Id;
                    _contaxt.SaveChanges();
                }
                else
                {
                    res = "Requirement can not deleted! Please remove assigned employees of this requirement.";
                }

            }
            catch (Exception ex)
            {
                res = "Requirement can not deleted!";
            }
            return res;
        }
        private bool IsActiveAssignedEmployee(int CLI_Id, int DES_Id)
        {
            int count = _contaxt.Clients_Employees.Where(m => m.CLI_Id.Equals(CLI_Id) && m.DES_Id.Equals(DES_Id) && m.CLE_UnassignedOn == null).Count();
            return (count > 0);
        }

        public string GetAssignEmployeeDependancy(int CLE_Id, int CLI_Id, int EMP_Id, int DES_Id)
        {
            string res = string.Empty;
            List<Wage_Register_Canteen> register_Canteens = _contaxt.Wage_Register_Canteen.Include(m => m.WAG_).Where(m => m.CLE_Id.Equals(CLE_Id)).ToList();
            if (register_Canteens.Count() > 0)
            {
                string WAG_Month = string.Empty;
                foreach (var month in register_Canteens)
                {
                    WAG_Month += month.WAG_.WAG_Month.ToString("MMMM") + "-" + month.WAG_.WAG_Month.ToString("yyyy");
                    WAG_Month += ",";
                }

                res += "<p> Employee's Canteen record is available for <b>" + WAG_Month.Remove(WAG_Month.Length - 1) + "</b></p> <br/>";
            }
            List<Wage_Register_Outstation> register_Outstations = _contaxt.Wage_Register_Outstation.Include(m => m.WAG_).Where(m => m.CLE_Id.Equals(CLE_Id)).ToList();
            if (register_Outstations.Count() > 0)
            {
                string WAG_Month = string.Empty;
                foreach (var month in register_Outstations)
                {
                    WAG_Month += month.WAG_.WAG_Month.ToString("MMMM") + "-" + month.WAG_.WAG_Month.ToString("yyyy");
                    WAG_Month += ",";
                }

                res += "<p> Employee's  outstation hours record is available for <b>" + WAG_Month.Remove(WAG_Month.Length - 1) + "</b></p> <br/>";
            }
            List<Wage_Register_Performance> register_Performances = _contaxt.Wage_Register_Performance.Include(m => m.WAG_).Where(m => m.CLE_Id.Equals(CLE_Id)).ToList();
            if (register_Performances.Count() > 0)
            {
                string WAG_Month = string.Empty;
                foreach (var month in register_Performances)
                {
                    WAG_Month += month.WAG_.WAG_Month.ToString("MMMM") + "-" + month.WAG_.WAG_Month.ToString("yyyy");
                    WAG_Month += ",";
                }

                res += "<p> Employee's performance record is available for <b>" + WAG_Month.Remove(WAG_Month.Length - 1) + "</b></p> <br/>";
            }

            List<Attendance> attendance = new List<Attendance>();
            attendance = _contaxt.Attendance.Include(m => m.WAG_).Where(m => m.EMP_Id.Equals(EMP_Id) && m.CLI_Id.Equals(CLI_Id) && m.DES_Id.Equals(DES_Id)).ToList();
            if (attendance.Count() > 0)
            {
                string WAG_Month = string.Empty;
                foreach (var month in attendance.GroupBy(m => m.WAG_Id).Select(g => new { WAG_ = g.Select(m => m.WAG_) }))
                {
                    DateTime WAGMonth = month.WAG_.Select(m => m.WAG_Month).FirstOrDefault();
                    WAG_Month += WAGMonth.ToString("MMMM") + "-" + WAGMonth.ToString("yyyy");
                    WAG_Month += ",";
                }
                res += "<p> Employee's Attendance record is available for <b>" + WAG_Month.Remove(WAG_Month.Length - 1) + "</b></p> <br/>";
            }

            return res;
        }

        public List<Client_Requirements> getClientRequirements(DateTime date, int CLI_Id)
        {
            List<Client_Requirements> cli_Req_List = new List<Client_Requirements>();
            DateTime LastDate = ProjectUtils.GetLastDateOfMonth(date);
            DateTime FirstDate = GetFirstDateOfMonth(date);
            var query = from n in _contaxt.Client_Requirements.Where(r => r.CLI_Id == CLI_Id && r.CRI_RegisteredOn.Date <= LastDate.Date && (r.CRI_InactivatedOn==null || r.CRI_InactivatedOn.Value.Date > FirstDate.Date))
                        group n by n.DES_Id into g
                        select g.OrderByDescending(t => t.CRI_RegisteredOn).ThenByDescending(m => m.CRI_Id).FirstOrDefault();

            return query.ToList();
        }

        public void RevertAssignEmployee(int CLE_Id)
        {
            Clients_Employees clientEmployee = _contaxt.Clients_Employees.Where(m => m.CLE_Id.Equals(CLE_Id)).FirstOrDefault();
            if (clientEmployee != null)
            {
                clientEmployee.CLE_UnassignedOn = null;
                clientEmployee.ADM_Id_UnassignedBy = null;
                _contaxt.Clients_Employees.Update(clientEmployee);
                _contaxt.SaveChanges();
            }
        }

        public decimal GetProffessionalTax(bool EMP_Gender, decimal WAR_GrossTotal, Client_Requirements cr)
        {
            decimal ProffessionalTax = 0;
            if (EMP_Gender)
            {
                if (WAR_GrossTotal >= cr.CRI_ProffTax_M_From_1 && WAR_GrossTotal <= cr.CRI_ProffTax_M_To_1)
                {
                    ProffessionalTax = cr.CRI_ProffTax_M_Amount_1;
                }
                else if (WAR_GrossTotal >= cr.CRI_ProffTax_M_From_2 && WAR_GrossTotal <= cr.CRI_ProffTax_M_To_2)
                {
                    ProffessionalTax = cr.CRI_ProffTax_M_Amount_2;
                }
                else if (WAR_GrossTotal >= cr.CRI_ProffTax_M_From_3 && WAR_GrossTotal <= cr.CRI_ProffTax_M_To_3)
                {
                    ProffessionalTax = cr.CRI_ProffTax_M_Amount_3;
                }
            }
            else
            {
                if (WAR_GrossTotal >= cr.CRI_ProffTax_F_From_1 && WAR_GrossTotal <= cr.CRI_ProffTax_F_To_1)
                {
                    ProffessionalTax = cr.CRI_ProffTax_F_Amount_1;
                }
                else if (WAR_GrossTotal >= cr.CRI_ProffTax_F_From_2 && WAR_GrossTotal <= cr.CRI_ProffTax_F_To_2)
                {
                    ProffessionalTax = cr.CRI_ProffTax_F_Amount_2;
                }
                else if (WAR_GrossTotal >= cr.CRI_ProffTax_F_From_3 && WAR_GrossTotal <= cr.CRI_ProffTax_F_To_3)
                {
                    ProffessionalTax = cr.CRI_ProffTax_F_Amount_3;
                }
            }
            return ProffessionalTax;
        }

        public Attendance_Parameter GetLatestAttendanceParameter(int CLI_Id)
        {
            return _contaxt.Attendance_Parameter.Where(m => m.CLI_Id.Equals(CLI_Id)).OrderByDescending(m => m.ATP_RegisteredOn).ThenByDescending(m => m.ATP_Id).FirstOrDefault();
        }
        public Attendance_Parameter GetAttendanceParameterByMonth(int CLI_Id, DateTime date)
        {
            return _contaxt.Attendance_Parameter.Where(m => m.CLI_Id.Equals(CLI_Id) &&
            (GetFirstDateOfMonth(m.ATP_RegisteredOn)).Date <= (GetFirstDateOfMonth(date).Date)
            ).OrderByDescending(m => m.ATP_RegisteredOn).ThenByDescending(m => m.ATP_Id).FirstOrDefault();
        }
        public IEnumerable<Attendance_Parameter> getAttendanceParameters(int CLI_Id)
        {
            return _contaxt.Attendance_Parameter.Where(m => m.CLI_Id.Equals(CLI_Id));
        }
        public Attendance_Parameter getAttendanceParameter(int ATP_Id)
        {
            return _contaxt.Attendance_Parameter.Find(ATP_Id);
        }

        public string AddAttendanceParameter(Attendance_Parameter attendanceParameter)
        {
            string str = string.Empty;
            try
            {
                _contaxt.Attendance_Parameter.Add(attendanceParameter);
                _contaxt.SaveChanges();
            }
            catch (Exception ex)
            {
                str = ex.Message;
            }
            return str;
        }
        public void EditAttendanceParameter(Attendance_Parameter attendanceParameter)
        {
            _contaxt.Attendance_Parameter.Update(attendanceParameter);
            _contaxt.SaveChanges();

        }

    }
}
