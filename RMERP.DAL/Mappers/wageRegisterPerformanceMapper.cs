using System;
using System.Collections.Generic;
using System.Text;
using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;


namespace RMERP.DAL.Mappers
{
    public class wageRegisterPerformanceMapper
    {
        public static Wage_Register_PerformanceVM mapMe(Wage_Register_Performance Performance, int CRI_Id)
        {
            Wage_Register_PerformanceVM PerformanceVM = new Wage_Register_PerformanceVM();
            PerformanceVM.WRP_Id = Performance.WRP_Id;
            PerformanceVM.CLE_Id = Performance.CLE_Id;
            PerformanceVM.WAG_Id = Performance.WAG_Id;
            PerformanceVM.WRP_Amount = Performance.WRP_Amount;
            PerformanceVM.CRI_Id = CRI_Id;
            if (Performance.CLE_ != null)
            {
                if (Performance.CLE_.EMP_ != null)
                {
                    PerformanceVM.Emp_ID = Performance.CLE_.EMP_Id;
                    PerformanceVM.Emp_Name = Performance.CLE_.EMP_.EMP_FirstName + " " + Performance.CLE_.EMP_.EMP_MiddleName + " " + Performance.CLE_.EMP_.EMP_SurName;
                }
            }
            return PerformanceVM;
        }
        public static Wage_Register_Performance mapMeModel(Wage_Register_PerformanceVM PerformanceVM)
        {
            Wage_Register_Performance Performance = new Wage_Register_Performance();
            Performance.WRP_Id = PerformanceVM.WRP_Id;
            Performance.CLE_Id = PerformanceVM.CLE_Id;
            Performance.WAG_Id = PerformanceVM.WAG_Id;
            Performance.WRP_Amount = PerformanceVM.WRP_Amount;
            return Performance;
        }
        public static List<Wage_Register_Performance> mapMeModels(List<Wage_Register_PerformanceVM> PerformanceVMs)
        {
            List<Wage_Register_Performance> lst = new List<Wage_Register_Performance>();
            foreach(var item in PerformanceVMs)
            {
                Wage_Register_Performance Performance = mapMeModel(item);
                lst.Add(Performance);
            }
            return lst;
        }
    }
}
