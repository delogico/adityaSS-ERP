using System;
using System.Collections.Generic;
using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace RMERP.DAL.Mappers
{
    public class ClientContactMapper
    {
        public static ClientContactVM mapMe(Client_Contact contact)
        {
            ClientContactVM contactVM = new ClientContactVM();
            contactVM.CON_Id = contact.CON_Id;
            contactVM.CLI_Id = contact.CLI_Id;
            contactVM.CON_FirstName = contact.CON_FirstName;
            contactVM.CON_SurName = contact.CON_SurName;
            contactVM.CON_Designation = contact.CON_Designation;
            contactVM.CON_Mobile = contact.CON_Mobile;
            contactVM.CON_Email = contact.CON_Email;
            contactVM.CON_isPrimary = contact.CON_isPrimary;
            contactVM.CON_RegisteredOn = contact.CON_RegisteredOn;
            contactVM.ADM_Id_RegisteredBy = contact.ADM_Id_RegisteredBy;
            if (contact.CLI != null)
                contactVM.client = contact.CLI;
            
            return contactVM;
        }

        public static List<ClientContactVM> mapContacts(List<Client_Contact> contacts)
        {
            List<ClientContactVM> lst = new List<ClientContactVM>();
            foreach (Client_Contact contact in contacts)
                lst.Add(mapMe(contact));
            return lst;
        }
    }
}
