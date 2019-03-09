using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models
{
    public partial class ClientsEmployees
    {
        public int CleId { get; set; }
        public int CliId { get; set; }
        public int EmpId { get; set; }
        public int DesId { get; set; }
        public DateTime CleRegisteredOn { get; set; }
        public int AdmIdRegisteredBy { get; set; }

        public Clients Cli { get; set; }
        public Designations Des { get; set; }
        public Employees Emp { get; set; }
    }
}
