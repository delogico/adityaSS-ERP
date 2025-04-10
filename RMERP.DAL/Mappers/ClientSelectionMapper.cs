using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMERP.DAL.Mappers
{
    public class ClientSelectionMapper
    {
        public static SelectionVM mapMe(Client client, int WAG_Id)
        {
            SelectionVM clientSelectionVM = new()
            {
                CLI_Name = client.CLI_Name,
                CLI_Id = client.CLI_Id,
                IsSelect = false,
                WAG_Id = WAG_Id
            };

            return clientSelectionVM;
        }
        public static List<SelectionVM> mapMe(List<Client> clients, int WAG_Id)
        {
            List<SelectionVM> clientSelectionVMs = new List<SelectionVM>();
            foreach (var item in clients)
            {
                clientSelectionVMs.Add(mapMe(item, WAG_Id));
            }
            return clientSelectionVMs;
        }
    }
}
