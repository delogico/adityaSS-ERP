using System;
using System.Collections.Generic;
using System.Linq;
using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;

namespace RMERP.DAL.Mappers
{
    public class ClientMapper
    {
        public static ClientsWagModel mapMe(Clients client)
        {
            ClientsWagModel clientVM = new ClientsWagModel();
            clientVM.CLI_Id = client.CLI_Id;
            clientVM.CLI_Name = client.CLI_Name;
            return clientVM;
        }
       

    }
}
