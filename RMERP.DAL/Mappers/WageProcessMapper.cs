using System;
using System.Collections.Generic;
using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;

namespace RMERP.DAL.Mappers
{
    public class WageProcessMapper
    {
        public static WageProcessVM mapMe(Wage_Process wageProcess)
        {
            WageProcessVM wageProcessVM = new WageProcessVM();
            wageProcessVM.WAG_Id = wageProcess.WAG_Id;
            wageProcessVM.WAG_Month = wageProcess.WAG_Month;
            return wageProcessVM;
        }
    }
}
