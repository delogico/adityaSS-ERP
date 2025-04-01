using System;
using System.Collections.Generic;
using System.Linq;
using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;

namespace RMERP.DAL.Mappers
{
    public class AllowanceMapper
    {
        public static AllowanceVM mapMe(Allowance allowance)
        {
            AllowanceVM allowancesVM = new AllowanceVM();
            allowancesVM.ALL_Id = allowance.ALL_Id;
            allowancesVM.ALL_Title = allowance.ALL_Title;
            allowancesVM.ALL_Alias = allowance.ALL_Alias;
            allowancesVM.ALL_Shortform = allowance.ALL_Shortform;
            return allowancesVM;
        }
        public static Allowance mapMeModel(AllowanceVM allowanceVM)//
        {
            Allowance allowances = new Allowance();
            allowances.ALL_Id = allowanceVM.ALL_Id;
            allowances.ALL_Title = allowanceVM.ALL_Title;
            allowances.ALL_Alias = allowanceVM.ALL_Alias;
            allowances.ALL_Shortform = allowanceVM.ALL_Shortform;
            return allowances;
        }

        public static ClientReqAllowanceVM mapMe(Client_Requirement_Allowance cRAllowance)
        {
            ClientReqAllowanceVM CRAVM = new ClientReqAllowanceVM();
            CRAVM.CRA_Id = cRAllowance.CRA_Id;
            CRAVM.CRI_Id = cRAllowance.CRI_Id;
            CRAVM.ALL_Id = cRAllowance.ALL_Id;
            CRAVM.CRA_Amount = Convert.ToDecimal(cRAllowance.CRA_Amount);
            CRAVM.CRA_DayswiseOrFull = cRAllowance.CRA_DayswiseOrFull;
            if (cRAllowance.ALL != null)
                CRAVM.allowance = mapMe(cRAllowance.ALL);
            return CRAVM;
        }

        public static Client_Requirement_Allowance mapMeModel(ClientReqAllowanceVM cRAllowanceVM) //
        {
            Client_Requirement_Allowance CRA = new Client_Requirement_Allowance();
            CRA.CRA_Id = cRAllowanceVM.CRA_Id;
            CRA.CRI_Id = cRAllowanceVM.CRI_Id;
            CRA.ALL_Id = cRAllowanceVM.ALL_Id;
            CRA.CRA_Amount = Convert.ToDecimal(cRAllowanceVM.CRA_Amount);
            CRA.CRA_DayswiseOrFull = cRAllowanceVM.CRA_DayswiseOrFull;
            if (cRAllowanceVM.allowance != null)
                CRA.ALL = mapMeModel(cRAllowanceVM.allowance);
            return CRA;
        }

        public static List<AllowanceVM> mapMeAllowances(List<Allowance> allowances)
        {
            List<AllowanceVM> lst = new List<AllowanceVM>();
            foreach(var item in allowances)
            {
                lst.Add(mapMe(item));
            }
            return lst;
        }
        public static List<Client_Requirement_Allowance> mapMeClientReqAllowances(List<ClientReqAllowanceVM> clientReqAllowanceVMs)
        {
            List<Client_Requirement_Allowance> lst = new List<Client_Requirement_Allowance>();
            foreach (var item in clientReqAllowanceVMs)
            {
                if (item.flagClientRequirement == true)
                {
                    lst.Add(mapMeModel(item));
                }               
            }
            return lst;
        }
         public static List<Client_Requirement_Allowance> mapMeClientReqAllowancesRemove(List<ClientReqAllowanceVM> clientReqAllowanceVMs)
        {
            List<Client_Requirement_Allowance> lst = new List<Client_Requirement_Allowance>();
            foreach (var item in clientReqAllowanceVMs)
            {
                if (!item.flagClientRequirement)
                {
                    lst.Add(mapMeModel(item));
                }               
            }
            return lst;
        }
        public static List<ClientReqAllowanceVM> mapMeAllowancesWithClientReq(List<Allowance> allowances, IEnumerable<Client_Requirement_Allowance> cRAllowances)
        {
            List<ClientReqAllowanceVM> lst = new List<ClientReqAllowanceVM>();
            foreach (var item in allowances)
            {
                ClientReqAllowanceVM vm = new ClientReqAllowanceVM();
                vm.ALL_Id = item.ALL_Id;
                vm.allowance = mapMe(item);
                Client_Requirement_Allowance cr = cRAllowances.Where(c => c.ALL_Id == item.ALL_Id).FirstOrDefault();
                vm.flagClientRequirement = false;
                if (cr != null)
                {
                    vm.CRI_Id = cr.CRI_Id;
                    vm.CRA_Id = cr.CRA_Id;
                    vm.CRA_Amount = cr.CRA_Amount;
                    vm.CRA_DayswiseOrFull = cr.CRA_DayswiseOrFull;
                    vm.flagClientRequirement = true;
                }
                lst.Add(vm);
            }
            return lst;
        }


    }
}
