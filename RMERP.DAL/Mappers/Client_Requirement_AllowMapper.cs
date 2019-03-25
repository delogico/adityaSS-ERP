using RMERP.DAL.ViewModel;
using RMERP.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMERP.DAL.Mappers
{
    public class Client_Requirement_AllowMapper
    {
        public static Client_Requirement_Allowances mapMe (Client_Requirement_Allow_VM CRAVM)
        {
            Client_Requirement_Allowances CRA = new Client_Requirement_Allowances();
            CRA.CRA_Id = CRAVM.CRA_Id;
            CRA.CRI_Id = CRAVM.CRI_Id;
            CRA.ALL_Id = CRAVM.ALL_Id;
            CRA.CRA_Amount =Convert.ToDecimal(CRAVM.CRA_Amount);
            CRA.CRA_DayswiseOrFull = CRAVM.CRA_DayswiseOrFull;
            return CRA;
        }
        public static List<Client_Requirement_Allowances> mapMeList(List<Client_Requirement_Allow_VM> CRAVM)
        {
            List<Client_Requirement_Allowances> list = new List<Client_Requirement_Allowances>();
            foreach(var item in CRAVM)
            {
                Client_Requirement_Allowances CRA = new Client_Requirement_Allowances();
                CRA=mapMe(item);
                list.Add(CRA);
            }
            return list;
        }
    }
}
