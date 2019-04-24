using System;
using System.Collections.Generic;
using System.Text;
using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;
namespace RMERP.DAL.Mappers
{
    public class WageRegisterAdvancesMapper
    {
        public static WageRegisterAdvancesVM mapMeModel(Wage_Register_Advances wra)
        {
            WageRegisterAdvancesVM eraVM = new WageRegisterAdvancesVM();
            eraVM.WAD_Id = wra.WAD_Id;
            eraVM.WAG_Id = wra.WAG_Id;
            eraVM.EMP_Id = wra.EMP_Id;
            eraVM.WAD_Amount = wra.WAD_Amount;
            eraVM.CLI_Id = wra.CLI_Id;
            eraVM.WAD_Status = wra.WAD_Status;
            eraVM.WAD_ClosedOn = wra.WAD_ClosedOn;                            
            return eraVM;
        }
        
        public static List<WageRegisterAdvancesVM> mapMeModels(List<Wage_Register_Advances> wra)
        {
            List<WageRegisterAdvancesVM> wageRegisterAdvancesVMs = new List<WageRegisterAdvancesVM>();
            foreach(var item in wra){
                WageRegisterAdvancesVM vm = mapMeModel(item);
                wageRegisterAdvancesVMs.Add(vm);
            }
            return wageRegisterAdvancesVMs;
        }
        public static Wage_Register_Advances mapMe(WageRegisterAdvancesVM wraVM)
        {
            Wage_Register_Advances Wra = new Wage_Register_Advances();
            Wra.WAD_Id = wraVM.WAD_Id;
            Wra.WAG_Id = wraVM.WAG_Id;
            Wra.EMP_Id = wraVM.EMP_Id;
            Wra.WAD_Amount = wraVM.WAD_Amount;
            Wra.CLI_Id = wraVM.CLI_Id;
            Wra.WAD_Status = wraVM.WAD_Status;
            Wra.WAD_ClosedOn = wraVM.WAD_ClosedOn;

            return Wra;
        }
        public static List<Wage_Register_Advances> mapMeList(List<WageRegisterAdvancesVM> wraVM)
        {
            List<Wage_Register_Advances> wageRegisterAdvances = new List<Wage_Register_Advances>();
            foreach (var item in wraVM)
            {
                Wage_Register_Advances vm = mapMe(item);
                wageRegisterAdvances.Add(vm);
            }
            return wageRegisterAdvances;
        }
    }
}
