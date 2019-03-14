using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using RMERP.DAL.Models;
using RMERP.DAL.App_Code;
using System.Linq;
using Microsoft.EntityFrameworkCore;


namespace RMERP.DAL.ManagerClasses
{
    public class ClientsManager
    {
        RMERPContext _contaxt;
        public IConfiguration Configuration;
        public ClientsManager(RMERPContext contaxt,IConfiguration configuration)
        {
            _contaxt = contaxt;
            Configuration = configuration;
        }
        //public string saveAddEditClients(IFormFile file, Clients clients)
        //{
        //    string res = string.Empty;
        //    try
        //    {
        //        string ImagePath = Configuration.GetSection("ImagePath").GetSection("ClientLogo_Folder_Path").Value;
        //        if (!System.IO.Directory.Exists(ImagePath + "/" + clients.CliId))
        //        {
        //            System.IO.Directory.CreateDirectory(ImagePath + "/" + clients.CliId);
        //        }
        //        if (file == null || file.Length == 0)
        //        {
        //            clients.CliLogo = "user.jpg";
        //        }
        //        else
        //        {
        //            var path = Path.Combine(
        //                  Directory.GetCurrentDirectory(), ImagePath + "/" + clients.CliId,
        //                  file.FileName);
        //            System.IO.File.Move(file.FileName, clients.CliName + "_" + file.FileName);
        //            using (var stream = new FileStream(path, FileMode.Create))
        //            {
        //                file.CopyToAsync(stream);
        //            }                   
        //            clients.CliLogo = file.FileName;             
        //        }
        //        _contaxt.Clients.Add(clients);
        //        _contaxt.SaveChanges();
        //    }
        //    catch (Exception ex)
        //    {
        //        res = ex.Message;
        //    }            
        //    return res;
        //}
        public Tuple<string, int> saveAddEditClients(IFormFile file,Clients clients)
        {
            string res = string.Empty;
            try
            {
                clients.CLI_Att_MonthReal = true;
                string ImagePath = Configuration.GetSection("ImagePath").GetSection("APP_DEFAULT_FILE_FOLDER_PATH").Value+"/"+ Configuration.GetSection("ImagePath").GetSection("APP_CLIENTS_LOGO_FOLDER_PATH").Value;
                if (!System.IO.Directory.Exists(ImagePath + "/" + clients.CLI_Id))
                {
                    System.IO.Directory.CreateDirectory(ImagePath + "/" + clients.CLI_Id);
                }
                if (file == null || file.Length == 0)
                {
                    clients.CLI_Logo = "user.jpg";
                }
                else
                {
                    var path = Path.Combine(
                          Directory.GetCurrentDirectory(), ImagePath + "/" + clients.CLI_Id,
                          file.FileName);
                 
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyToAsync(stream);
                    }
                    clients.CLI_Logo = file.FileName;
                }
                clients.CLI_RegisteredOn = ProjectUtils.DateNow();
                clients.CLI_IsActive = true;
                if (clients.CLI_Id > 0)
                {
                    _contaxt.Clients.Update(clients);
                }
                else
                {
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
        public Tuple<string, int> saveAddEditClients1(IFormFile file, Clients clients)
        {
            string res = string.Empty;
            try
            {
                clients.CLI_Att_MonthReal = true;
                string ImagePath = Configuration.GetSection("ImagePath").GetSection("APP_DEFAULT_FILE_FOLDER_PATH").Value + "/" + Configuration.GetSection("ImagePath").GetSection("APP_CLIENTS_LOGO_FOLDER_PATH").Value;
                if (!System.IO.Directory.Exists(ImagePath + "/" + clients.CLI_Id))
                {
                    System.IO.Directory.CreateDirectory(ImagePath + "/" + clients.CLI_Id);
                }
                if (file == null || file.Length == 0)
                {
                    clients.CLI_Logo = "user.jpg";
                }
                else
                {
                    var path = Path.Combine(
                          Directory.GetCurrentDirectory(), ImagePath + "/" + clients.CLI_Id,
                          file.FileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyToAsync(stream);
                    }
                    clients.CLI_Logo = file.FileName;
                }
                clients.CLI_RegisteredOn = ProjectUtils.DateNow();
                clients.CLI_IsActive = true;
                if (clients.CLI_Id > 0)
                {
                    _contaxt.Clients.Update(clients);
                }
                else
                {
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
        public IEnumerable<Clients> listClients(int? FirmId,bool IsActive,string Client="")
        {           
            IQueryable<Clients> ClientList = _contaxt.Clients.Include(m => m.CITY_).Include(m => m.FRM_).Include(m => m.Client_Contacts);
            if (FirmId.HasValue)
            {
                ClientList = ClientList.Where(m => m.FRM_Id==FirmId.Value);
            }

            if (string.IsNullOrEmpty(Client))
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
        public Clients GetClientById(int id)
        {
            Clients clients = new Clients();
            try
            {               
                clients = _contaxt.Clients.Find(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
            return clients;
        }
        public Client_Contacts GetClientContactsById(int id)
        {
            Client_Contacts clientContacts = new Client_Contacts();
            try
            {
                clientContacts = _contaxt.Client_Contacts.Find(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return clientContacts;
        }
        public string saveAddEditContacts(Client_Contacts clientContacts)
        {
            string res = string.Empty;
            try
            {
                if (clientContacts.CON_isPrimary == true)
                {
                    List<Client_Contacts> list = _contaxt.Client_Contacts.Where(m => m.CLI_Id.Equals(clientContacts.CLI_Id) && m.CON_Id!=clientContacts.CON_Id).ToList();
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
        public List<Client_Contacts> GetClientContactsListById(int Clientid)
        {
            List<Client_Contacts> clientContactsList = new List<Client_Contacts>();
            try
            {
                clientContactsList = _contaxt.Client_Contacts.Include(m=>m.CLI_).Where(m => m.CLI_Id.Equals(Clientid)).OrderByDescending(m=>m.CON_isPrimary).ToList();
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
        public string InActiveClient(int ClientId,int AdminId,string Active)
        {
            string res = string.Empty;
            try
            {
                Clients client = new Clients();
                client = _contaxt.Clients.Find(ClientId);
                client.CLI_InActivatedOn = ProjectUtils.DateNow();
                client.ADM_Id_InactivatedBy = AdminId;
                client.CLI_IsActive =Convert.ToBoolean((Active.ToLower() == "false" ? "true" : "false"));
                _contaxt.Clients.Update(client);
                _contaxt.SaveChanges();                
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        }
        public string AddEditRequirement(Client_Requirements clientRequirements)
        {
            string res = string.Empty;
            try
            {
                clientRequirements.CRI_RegisteredOn = ProjectUtils.DateNow();
                if (clientRequirements.CRI_Id > 0)
                {
                    List<Client_Requirements> list = _contaxt.Client_Requirements.Where(m => m.CLI_Id.Equals(clientRequirements.CLI_Id) && m.DES_Id.Equals(clientRequirements.DES_Id)).ToList();
                    list.ForEach(m => m.CRI_Active = false);
                    _contaxt.SaveChanges();                   
                    clientRequirements.CRI_Id = 0;
                    _contaxt.Client_Requirements.Add(clientRequirements);
                }
                else
                {
                    _contaxt.Client_Requirements.Add(clientRequirements);
                }
                _contaxt.SaveChanges();
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            
            return res;
        }
        public Client_Requirements GetRequirementsById(int criId)
        {
            Client_Requirements clientRequirements = new Client_Requirements();
            try
            {               
                clientRequirements = _contaxt.Client_Requirements.Find(criId);
            }
            catch (Exception ex)
            {
                throw ex;
            }            
            return clientRequirements;
        }

        public IEnumerable<Client_Requirements> GetClient_RequirementsList(int desId, int cliId, bool active = true)
        {
            IEnumerable<Client_Requirements> list = _contaxt.Client_Requirements.Include(m => m.CLI_).Include(m => m.DES_).Where(m => m.CLI_Id.Equals(cliId) && m.CRI_Active.Equals(active) && m.DES_Id.Equals(desId)).OrderByDescending(m=>m.CRI_RegisteredOn).ToList();           
            return list;
        }

        public IEnumerable<Client_Requirements> GetClient_RequirementsListByClientId(int cliId, bool active = true)
        {
            IEnumerable<Client_Requirements> list = _contaxt.Client_Requirements.Include(m => m.CLI_).Include(m => m.DES_).Where(m => m.CLI_Id.Equals(cliId) && m.CRI_Active.Equals(active)).OrderByDescending(m => m.CRI_RegisteredOn).ToList();
            return list;
        }

        public string GetDesTitleByCriId(int criId)
        {
            if (criId > 0)
            {
                var DesT = from a in _contaxt.Client_Requirements
                           join b in _contaxt.Designations on a.DES_Id equals b.DES_Id
                           where a.CRI_Id == criId
                           select new
                           {                               
                              b.DES_Title
                           };

                return DesT.ToList()[0].DES_Title;
            }
            else
            {
                return string.Empty;
            }
           
        }
        public string UpdateParameters(Clients clients)
        {
            string res = string.Empty;
            try
            {
                Clients client = _contaxt.Clients.Find(clients.CLI_Id);
                client.CLI_Att_Month_End = clients.CLI_Att_Month_End;
                client.CLI_Att_Month_Start = clients.CLI_Att_Month_Start;
                client.CLI_Att_MonthReal = clients.CLI_Att_MonthReal;
                client.CLI_GST_Number = clients.CLI_GST_Number;
                client.CLI_GST_Rate = clients.CLI_GST_Rate;
                client.CLI_TDS_Rate = clients.CLI_TDS_Rate;
                client.CLI_HSN_Code = clients.CLI_HSN_Code;
                _contaxt.Clients.Update(client);
                _contaxt.SaveChanges();
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        
}
        public IEnumerable<Employees> getEmployeeList(int ClientId)
        {
            IEnumerable<Employees> listEmp=null;
            listEmp = _contaxt.Employees.Where(m => m.EMP_IsActive.Equals(true));
            if (ClientId > 0)
            {        
                listEmp = from t1 in listEmp
                              where !(from t2 in _contaxt.Clients_Employees.Where(m=>m.CLI_Id.Equals(ClientId))
                                      select t2.EMP_Id).Contains(t1.EMP_Id)
                              select t1;
            }                     
            return listEmp.ToList();
        }
        public string ClientEmployee(Clients_Employees clientsEmployees,int AdminId)
        {
            string res = string.Empty;
            clientsEmployees.CLE_RegisteredOn = ProjectUtils.DateNow();
            clientsEmployees.ADM_Id_RegisteredBy = AdminId;
            try
            {
                if (clientsEmployees.CLE_Id > 0)
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
                res= ex.Message;
            }
            return res;
        }
        public IEnumerable<Clients_Employees> listClientsEmployees(int ClientId)
        {
            IEnumerable<Clients_Employees> list=_contaxt.Clients_Employees.Where(m=>m.CLI_Id.Equals(ClientId)).Include(m=>m.EMP_).Include(m => m.DES_).ToList();
            return list;
        }
        public Clients_Employees ClientEmployeeById(int CleId)
        {
            Clients_Employees clientsEmployees = _contaxt.Clients_Employees.Find(CleId);
            return clientsEmployees;
        }
        public string deleteClientEmployee(int id)
        {
            string res = string.Empty;
            try
            {
                var ClientEmp = _contaxt.Clients_Employees.Find(id);
                _contaxt.Clients_Employees.Remove(ClientEmp);
                _contaxt.SaveChanges();
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        }

        public IEnumerable<Clients_Employees> listClientsPerWag(bool IsActive, int wagId, int AdminId,int? firmId)
        {
            IEnumerable<Clients_Employees> ClientList = null;
            int month = _contaxt.Wage_Process.Find(wagId).WAG_Month.Month;
            ClientList = _contaxt.Clients_Employees.Include(m=>m.CLI_).Where(m => m.ADM_Id_RegisteredBy.Equals(AdminId) && m.CLI_.CLI_IsActive.Equals(IsActive) && m.CLE_RegisteredOn.Month.Equals(month));
            if (firmId != null)
            {
                ClientList = ClientList.Where(m => m.CLI_.FRM_Id.Equals(firmId));
            }

            var ClientList1 = from P in ClientList.Distinct().ToList() select new { P };

            return ClientList.ToList();
        }
        public IEnumerable<Clients> listClientsForAttandance(int? FirmId, bool IsActive, string Client = "")
        {
            IQueryable<Clients> ClientList = null;
            return ClientList;
        }

        public Tuple<int, int> GetDateByClientID(int cliID)
        {
            return new Tuple<int, int>(0, 0);
        }

        public List<Clients> GetClientsListByMonth(DateTime WageMonthDate)
        {
            WageMonthDate = new DateTime(2019, 01, 01);
            DateTime LastDate = new DateTime(WageMonthDate.Year, WageMonthDate.Month, 1).AddMonths(1).AddDays(-1);
            List<Clients> lstClient = new List<Clients>();

            var cliList = from a in _contaxt.Clients
                          where a.CLI_RegisteredOn <= WageMonthDate                                         
                          select a;

            foreach(var cli in cliList)
            {
                if (cli.CLI_IsActive == false && cli.CLI_InActivatedOn >= WageMonthDate)
                {
                    lstClient.Add(cli);
                }
                else if(cli.CLI_IsActive == true)
                {
                    lstClient.Add(cli);
                }              
            }           
           
            return lstClient;
        }
    }
}
