using System;
using System.Collections.Generic;
using System.Text;
using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;

namespace RMERP.DAL.Mappers
{
    public class WageProcessClientsMapper
    {
        public static WageProcessClientVM MapMe(Wage_Process_Client wageProcess)
        {
            return new WageProcessClientVM
            {
                WPC_Id = wageProcess.WPC_Id,
                WPC_SavedOn = wageProcess.WPC_SavedOn,
                WPC_WageRegisterSaved = wageProcess.WPC_WageRegisterSaved
            };
        }
    }
}
