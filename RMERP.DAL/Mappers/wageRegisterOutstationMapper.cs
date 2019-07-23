using System;
using System.Collections.Generic;
using System.Text;
using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;


namespace RMERP.DAL.Mappers
{
    public class wageRegisterOutstationMapper
    {
        public static Wage_Register_OutstationVM mapMe(Wage_Register_Outstation outstation, int CRI_Id)
        {
            Wage_Register_OutstationVM outstationVM = new Wage_Register_OutstationVM();
            outstationVM.WRO_Id = outstation.WRO_Id;
            outstationVM.CLE_Id = outstation.CLE_Id;
            outstationVM.WAG_Id = outstation.WAG_Id;
            outstationVM.WRO_Hours = outstation.WRO_Hours;
            outstationVM.CRI_Id = CRI_Id;
            if (outstation.CLE_ != null)
            {
                if (outstation.CLE_.EMP_ != null)
                {
                    outstationVM.Emp_ID = outstation.CLE_.EMP_Id;
                    outstationVM.Emp_Name = outstation.CLE_.EMP_.EMP_FirstName + " " + outstation.CLE_.EMP_.EMP_MiddleName + " " + outstation.CLE_.EMP_.EMP_SurName;
                }
            }
            return outstationVM;
        }
        public static Wage_Register_Outstation mapMeModel(Wage_Register_OutstationVM outstationVM)
        {
            Wage_Register_Outstation outstation = new Wage_Register_Outstation();
            outstation.WRO_Id = outstationVM.WRO_Id;
            outstation.CLE_Id = outstationVM.CLE_Id;
            outstation.WAG_Id = outstationVM.WAG_Id;
            outstation.WRO_Hours = outstationVM.WRO_Hours;
            return outstation;
        }
        public static List<Wage_Register_Outstation> mapMeModels(List<Wage_Register_OutstationVM> outstationVMs)
        {
            List<Wage_Register_Outstation> lst = new List<Wage_Register_Outstation>();
            foreach(var item in outstationVMs)
            {
                Wage_Register_Outstation outstation = mapMeModel(item);
                lst.Add(outstation);
            }
            return lst;
        }
    }
}
