using System;
using System.Collections.Generic;
using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;

namespace RMERP.DAL.Mappers
{
    public class WageProcessMapper
    {
        public static WageProcessVM mapMe(WageProcess wageProcess)
        {
            WageProcessVM wageProcessVM = new WageProcessVM();
            wageProcessVM.WagId = wageProcess.WagId;
            wageProcessVM.WagMonth = wageProcess.WagMonth;
            return wageProcessVM;
        }
    }
}
