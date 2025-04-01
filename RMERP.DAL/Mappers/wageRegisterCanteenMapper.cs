using System;
using System.Collections.Generic;
using System.Text;
using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;


namespace RMERP.DAL.Mappers
{
    public class wageRegisterCanteenMapper
    {
        public static Wage_Register_CanteenVM mapMe(Wage_Register_Canteen canteen, int CRI_Id)
        {
            Wage_Register_CanteenVM canteenVM = new Wage_Register_CanteenVM();
            canteenVM.WRC_Id = canteen.WRC_Id;
            canteenVM.CLE_Id = canteen.CLE_Id;
            canteenVM.WAG_Id = canteen.WAG_Id;
            canteenVM.WRC_Amount = canteen.WRC_Amount;
            canteenVM.CRI_Id = CRI_Id;
            if (canteen.CLE != null)
            {
                if (canteen.CLE.EMP != null)
                {
                    canteenVM.Emp_ID = canteen.CLE.EMP_Id;
                    canteenVM.Emp_Name = canteen.CLE.EMP.EMP_FirstName + " " + canteen.CLE.EMP.EMP_MiddleName + " " + canteen.CLE.EMP.EMP_SurName;
                }
            }
            return canteenVM;
        }
        public static Wage_Register_Canteen mapMeModel(Wage_Register_CanteenVM canteenVM)
        {
            Wage_Register_Canteen canteen = new Wage_Register_Canteen();
            canteen.WRC_Id = canteenVM.WRC_Id;
            canteen.CLE_Id = canteenVM.CLE_Id;
            canteen.WAG_Id = canteenVM.WAG_Id;
            canteen.WRC_Amount = canteenVM.WRC_Amount;
            return canteen;
        }
        public static List<Wage_Register_Canteen> mapMeModels(List<Wage_Register_CanteenVM> canteenVMs)
        {
            List<Wage_Register_Canteen> lst = new List<Wage_Register_Canteen>();
            foreach(var item in canteenVMs)
            {
                Wage_Register_Canteen canteen = mapMeModel(item);
                lst.Add(canteen);
            }
            return lst;
        }
    }
}
