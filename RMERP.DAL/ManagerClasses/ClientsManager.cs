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
                clients.CliAttMonthReal = true;
                string ImagePath = Configuration.GetSection("ImagePath").GetSection("APP_DEFAULT_FILE_FOLDER_PATH").Value+"/"+ Configuration.GetSection("ImagePath").GetSection("APP_CLIENTS_LOGO_FOLDER_PATH").Value;
                if (!System.IO.Directory.Exists(ImagePath + "/" + clients.CliId))
                {
                    System.IO.Directory.CreateDirectory(ImagePath + "/" + clients.CliId);
                }
                if (file == null || file.Length == 0)
                {
                    clients.CliLogo = "user.jpg";
                }
                else
                {
                    var path = Path.Combine(
                          Directory.GetCurrentDirectory(), ImagePath + "/" + clients.CliId,
                          file.FileName);
                 
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyToAsync(stream);
                    }
                    clients.CliLogo = file.FileName;
                }
                clients.CliRegisteredOn = ProjectUtils.DateNow();
                clients.CliIsActive = true;
                if (clients.CliId > 0)
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
            return new Tuple<string, int>(res, clients.CliId);
        }
        public Tuple<string, int> saveAddEditClients1(IFormFile file, Clients clients)
        {
            string res = string.Empty;
            try
            {
                clients.CliAttMonthReal = true;
                string ImagePath = Configuration.GetSection("ImagePath").GetSection("APP_DEFAULT_FILE_FOLDER_PATH").Value + "/" + Configuration.GetSection("ImagePath").GetSection("APP_CLIENTS_LOGO_FOLDER_PATH").Value;
                if (!System.IO.Directory.Exists(ImagePath + "/" + clients.CliId))
                {
                    System.IO.Directory.CreateDirectory(ImagePath + "/" + clients.CliId);
                }
                if (file == null || file.Length == 0)
                {
                    clients.CliLogo = "user.jpg";
                }
                else
                {
                    var path = Path.Combine(
                          Directory.GetCurrentDirectory(), ImagePath + "/" + clients.CliId,
                          file.FileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyToAsync(stream);
                    }
                    clients.CliLogo = file.FileName;
                }
                clients.CliRegisteredOn = ProjectUtils.DateNow();
                clients.CliIsActive = true;
                if (clients.CliId > 0)
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
            return new Tuple<string, int>(res, clients.CliId);
        }
        public IEnumerable<Clients> listClients(int? FirmId,bool IsActive,string Client="")
        {           
            IQueryable<Clients> ClientList = _contaxt.Clients.Include(m => m.City).Include(m => m.Frm).Include(m => m.ClientContacts);
            if (FirmId.HasValue)
            {
                ClientList = ClientList.Where(m => m.FrmId==FirmId.Value);
            }

            if (string.IsNullOrEmpty(Client))
            {
                ClientList = ClientList.Where(m => m.CliIsActive.Equals(IsActive));
            }
            else
            {
                ClientList = from s in ClientList
                             where EF.Functions.Like(s.CliName, "%" + Client + "%")
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
        public ClientContacts GetClientContactsById(int id)
        {
            ClientContacts clientContacts = new ClientContacts();
            try
            {
                clientContacts = _contaxt.ClientContacts.Find(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return clientContacts;
        }
        public string saveAddEditContacts(ClientContacts clientContacts)
        {
            string res = string.Empty;
            try
            {
                if (clientContacts.ConIsPrimary == true)
                {
                    List<ClientContacts> list = _contaxt.ClientContacts.Where(m => m.CliId.Equals(clientContacts.CliId) && m.ConId!=clientContacts.ConId).ToList();
                    list.ForEach(m => m.ConIsPrimary = false);
                    _contaxt.SaveChanges();
                }
                if (clientContacts.ConId > 0)
                {
                    _contaxt.ClientContacts.Update(clientContacts);
                }
                else
                {                    
                    _contaxt.ClientContacts.Add(clientContacts);
                }
                _contaxt.SaveChanges();
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        }
        public List<ClientContacts> GetClientContactsListById(int Clientid)
        {
            List<ClientContacts> clientContactsList = new List<ClientContacts>();
            try
            {
                clientContactsList = _contaxt.ClientContacts.Include(m=>m.Cli).Where(m => m.CliId.Equals(Clientid)).OrderByDescending(m=>m.ConIsPrimary).ToList();
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
                var ClientContact = _contaxt.ClientContacts.Find(ContactId);
                _contaxt.ClientContacts.Remove(ClientContact);
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
                client.CliInActivatedOn = ProjectUtils.DateNow();
                client.AdmIdInactivatedBy = AdminId;
                client.CliIsActive =Convert.ToBoolean((Active.ToLower() == "false" ? "true" : "false"));
                _contaxt.Clients.Update(client);
                _contaxt.SaveChanges();                
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        }
        public string AddEditRequirement(ClientRequirements clientRequirements)
        {
            string res = string.Empty;
            try
            {
                clientRequirements.CriRegisteredOn = ProjectUtils.DateNow();
                if (clientRequirements.CriId > 0)
                {
                    List<ClientRequirements> list = _contaxt.ClientRequirements.Where(m => m.CliId.Equals(clientRequirements.CliId) && m.DesId.Equals(clientRequirements.DesId)).ToList();
                    list.ForEach(m => m.CriActive = false);
                    _contaxt.SaveChanges();                   
                    clientRequirements.CriId = 0;
                    _contaxt.ClientRequirements.Add(clientRequirements);
                }
                else
                {
                    _contaxt.ClientRequirements.Add(clientRequirements);
                }
                _contaxt.SaveChanges();
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            
            return res;
        }
        public ClientRequirements GetRequirementsById(int criId)
        {
            ClientRequirements clientRequirements = new ClientRequirements();
            try
            {               
                clientRequirements = _contaxt.ClientRequirements.Find(criId);
            }
            catch (Exception ex)
            {
                throw ex;
            }            
            return clientRequirements;
        }

        public IEnumerable<ClientRequirements> GetClientRequirementsList(int desId, int cliId, bool active = true)
        {
            IEnumerable<ClientRequirements> list = _contaxt.ClientRequirements.Include(m => m.Cli).Include(m => m.Des).Where(m => m.CliId.Equals(cliId) && m.CriActive.Equals(active) && m.DesId.Equals(desId)).OrderByDescending(m=>m.CriRegisteredOn).ToList();           
            return list;
        }

        public IEnumerable<ClientRequirements> GetClientRequirementsListByClientId(int cliId, bool active = true)
        {
            IEnumerable<ClientRequirements> list = _contaxt.ClientRequirements.Include(m => m.Cli).Include(m => m.Des).Where(m => m.CliId.Equals(cliId) && m.CriActive.Equals(active)).OrderByDescending(m => m.CriRegisteredOn).ToList();
            return list;
        }

        public string GetDesTitleByCriId(int criId)
        {
            if (criId > 0)
            {
                var DesT = from a in _contaxt.ClientRequirements
                           join b in _contaxt.Designations on a.DesId equals b.DesId
                           where a.CriId == criId
                           select new
                           {                               
                              b.DesTitle
                           };

                return DesT.ToList()[0].DesTitle;
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
                Clients client = _contaxt.Clients.Find(clients.CliId);
                client.CliAttMonthEnd = clients.CliAttMonthEnd;
                client.CliAttMonthStart = clients.CliAttMonthStart;
                client.CliAttMonthReal = clients.CliAttMonthReal;
                client.CliGstNumber = clients.CliGstNumber;
                client.CliGstRate = clients.CliGstRate;
                client.CliTdsRate = clients.CliTdsRate;
                client.CliHsnCode = clients.CliHsnCode;
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
            listEmp = _contaxt.Employees.Where(m => m.EmpIsActive.Equals(true));
            if (ClientId > 0)
            {        
                listEmp = from t1 in listEmp
                              where !(from t2 in _contaxt.ClientsEmployees.Where(m=>m.CliId.Equals(ClientId))
                                      select t2.EmpId).Contains(t1.EmpId)
                              select t1;
            }                     
            return listEmp.ToList();
        }
        public string ClientEmployee(ClientsEmployees clientsEmployees,int AdminId)
        {
            string res = string.Empty;
            clientsEmployees.CleRegisteredOn = ProjectUtils.DateNow();
            clientsEmployees.AdmIdRegisteredBy = AdminId;
            try
            {
                if (clientsEmployees.CleId > 0)
                {
                    _contaxt.ClientsEmployees.Update(clientsEmployees);
                }
                else
                {
                    _contaxt.ClientsEmployees.Add(clientsEmployees);
                }               
                _contaxt.SaveChanges();
            }
            catch (Exception ex)
            {
                res= ex.Message;
            }
            return res;
        }
        public IEnumerable<ClientsEmployees> listClientsEmployees(int ClientId)
        {
            IEnumerable<ClientsEmployees> list=_contaxt.ClientsEmployees.Where(m=>m.CliId.Equals(ClientId)).Include(m=>m.Emp).Include(m => m.Des).ToList();
            return list;
        }
        public ClientsEmployees ClientEmployeeById(int CleId)
        {
            ClientsEmployees clientsEmployees = _contaxt.ClientsEmployees.Find(CleId);
            return clientsEmployees;
        }
        public string deleteClientEmployee(int id)
        {
            string res = string.Empty;
            try
            {
                var ClientEmp = _contaxt.ClientsEmployees.Find(id);
                _contaxt.ClientsEmployees.Remove(ClientEmp);
                _contaxt.SaveChanges();
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        }

        public IEnumerable<ClientsEmployees> listClientsPerWag(bool IsActive, int wagId, int AdminId,int? firmId)
        {
            IEnumerable<ClientsEmployees> ClientList = null;
            int month = _contaxt.WageProcess.Find(wagId).WagMonth.Month;
            ClientList = _contaxt.ClientsEmployees.Include(m=>m.Cli).Where(m => m.AdmIdRegisteredBy.Equals(AdminId) && m.Cli.CliIsActive.Equals(IsActive) && m.CleRegisteredOn.Month.Equals(month));
            if (firmId != null)
            {
                ClientList = ClientList.Where(m => m.Cli.FrmId.Equals(firmId));
            }

            var ClientList1 = from P in ClientList.Distinct().ToList() select new { P };

            return ClientList.ToList();
        }
    }
}
