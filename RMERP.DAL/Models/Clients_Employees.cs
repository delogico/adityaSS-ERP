using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models
{
    public partial class Clients_Employees
    {
        public int CLE_Id { get; set; }
        public int CLI_Id { get; set; }
        public int EMP_Id { get; set; }
        public int CRI_Id { get; set; }
        public DateTime CLE_RegisteredOn { get; set; }
        public int ADM_Id_RegisteredBy { get; set; }

        public Clients CLI_ { get; set; }
        public Client_Requirements CRI_ { get; set; }
        public Employees EMP_ { get; set; }
    }
}
