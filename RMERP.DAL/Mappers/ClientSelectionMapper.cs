using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMERP.DAL.Mappers
{
    public class ClientSelectionMapper
    {
        public static SelectionVM mapMe(Client client,int WAG_Id)
        {
            SelectionVM clientSelectionVM = new SelectionVM();
            clientSelectionVM.CLI_Name = client.CLI_Name;
            clientSelectionVM.CLI_Id = client.CLI_Id;
            clientSelectionVM.IsSelect = false;
            clientSelectionVM.WAG_Id = WAG_Id;
            return clientSelectionVM;
        }
        public static List<SelectionVM> mapMe(List<Client> clients,int WAG_Id)
        {
            List<SelectionVM> clientSelectionVMs = new List<SelectionVM>();
            foreach(var item in clients)
            {
                clientSelectionVMs.Add(mapMe(item, WAG_Id));
            }
            return clientSelectionVMs;
        }
    }
}
