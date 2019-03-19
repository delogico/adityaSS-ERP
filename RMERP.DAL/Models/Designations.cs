using System;
using System.Collections.Generic;

namespace RMERP.DAL.Models
{
    public partial class Designations
    {
        public Designations()
        {
            Client_Requirements = new HashSet<Client_Requirements>();
        }

        public int DES_Id { get; set; }
        public string DES_Title { get; set; }

        public ICollection<Client_Requirements> Client_Requirements { get; set; }
    }
}
