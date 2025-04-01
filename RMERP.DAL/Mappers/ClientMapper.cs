using System;
using System.Collections.Generic;
using System.Linq;
using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;

namespace RMERP.DAL.Mappers
{
    public class ClientMapper
    {
        public static ClientsWagModel MapMe(Client client)
        {
            return new ClientsWagModel
            {
                CLI_Id = client.CLI_Id,
                CLI_Name = client.CLI_Name
            };
        }
    }
}
