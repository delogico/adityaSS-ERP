using System;
using System.Collections.Generic;
using System.Text;
using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;

namespace RMERP.DAL.Mappers
{
    public class WageProcessClientsMapper
    {
        public static WageProcessClientVM mapMe(Wage_Process_Clients wageProcess)
        {
            WageProcessClientVM wageProcessClientVM = new WageProcessClientVM();
            wageProcessClientVM.WPC_Id = wageProcess.WPC_Id;
            wageProcessClientVM.WPC_SavedOn = wageProcess.WPC_SavedOn;
            wageProcessClientVM.WPC_WageRegisterSaved = wageProcess.WPC_WageRegisterSaved;
            return wageProcessClientVM;
        }
    }
}
