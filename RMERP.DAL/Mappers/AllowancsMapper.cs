using System;
using System.Collections.Generic;
using System.Text;
using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;

namespace RMERP.DAL.Mappers
{
    public class AllowancsMapper
    {
        public static List<AllowancesVM> mapMeInVMlist(List<Allowances> allowances)
        {
            List<AllowancesVM> allowancesVMs = new List<AllowancesVM>();
            foreach(var item in allowances)
            {
                AllowancesVM allowancesVM = new AllowancesVM();
                allowancesVM.ALL_Id = item.ALL_Id;
                allowancesVM.ALL_Title = item.ALL_Title;
                allowancesVM.ALL_Alias = item.ALL_Alias;
                allowancesVMs.Add(allowancesVM);
            }
            return allowancesVMs;
        }

       
    }
}
