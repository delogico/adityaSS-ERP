using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RMERP.DAL.Models;

namespace RMERP.DAL.ViewModel
{
    public class AllowanceVM
    {
        public int ALL_Id { get; set; }
        public string ALL_Title { get; set; }
        public string ALL_Alias { get; set; }
        public string ALL_Shortform { get; set; }
        public decimal total { get; set; }

        public static AllowanceVM MapMe(Allowance allowance)
        {
            AllowanceVM obj = new AllowanceVM();
            obj.ALL_Id = allowance.ALL_Id;
            obj.ALL_Alias = allowance.ALL_Alias;
            obj.total = 0;
            return obj;
        }

        public static List<AllowanceVM> MapMes(List<Allowance> allowances)
        {
            List<AllowanceVM> ret = new List<AllowanceVM>();
            foreach (Allowance allowance in allowances)
                ret.Add(MapMe(allowance));
            return ret;
        }
    }

    #region Wage_Register_Allowances_1

    public class Wage_Register_AllowancesVM_1
    {
        public int WRA_Id_1 { get; set; }
        public int WAG_Id { get; set; }
        public int CLE_Id { get; set; }
        public int CRI_Id { get; set; }
        public decimal WRA_Amount_1 { get; set; }
        public int Emp_ID { get; set; }
        public int FRM_ID { get; set; }
        public string Emp_Name { get; set; }

        public static Wage_Register_AllowancesVM_1 mapMe(Wage_Register_Allowances_1 allowances_1, int CRI_Id)
        {
            Wage_Register_AllowancesVM_1 allowancesVM_1 = new Wage_Register_AllowancesVM_1();
            allowancesVM_1.WRA_Id_1 = allowances_1.WRA_Id_1;
            allowancesVM_1.CLE_Id = allowances_1.CLE_Id;
            allowancesVM_1.WAG_Id = allowances_1.WAG_Id;
            allowancesVM_1.WRA_Amount_1 = allowances_1.WRA_Amount_1;
            allowancesVM_1.CRI_Id = CRI_Id;
            if (allowances_1.CLE != null)
            {
                if (allowances_1.CLE.EMP != null)
                {
                    allowancesVM_1.Emp_ID = allowances_1.CLE.EMP_Id;
                    allowancesVM_1.Emp_Name = allowances_1.CLE.EMP.EMP_FirstName + " " + allowances_1.CLE.EMP.EMP_MiddleName + " " + allowances_1.CLE.EMP.EMP_SurName;
                }
            }
            return allowancesVM_1;
        }
        public static Wage_Register_Allowances_1 mapMeModel(Wage_Register_AllowancesVM_1 allowanceVM)
        {
            Wage_Register_Allowances_1 allowance = new Wage_Register_Allowances_1();
            allowance.WRA_Id_1 = allowanceVM.WRA_Id_1;
            allowance.CLE_Id = allowanceVM.CLE_Id;
            allowance.WAG_Id = allowanceVM.WAG_Id;
            allowance.WRA_Amount_1 = allowanceVM.WRA_Amount_1;
            return allowance;
        }
        public static List<Wage_Register_Allowances_1> mapMeModels(List<Wage_Register_AllowancesVM_1> allowanceVMs)
        {
            List<Wage_Register_Allowances_1> lst = new List<Wage_Register_Allowances_1>();
            foreach (var item in allowanceVMs)
            {
                lst.Add(mapMeModel(item));
            }
            return lst;
        }
    }
    public class updateWageRegisterAllowance_1
    {
        public string CRI_Allowance_Name_1 { get; set; }
        public List<Wage_Register_AllowancesVM_1> AllowancesVMs { get; set; }
        public int CLI_Id { get; set; }
    }

    #endregion

    #region Wage_Register_Allowances_2

    public class Wage_Register_AllowancesVM_2
    {
        public int WRA_Id_2 { get; set; }
        public int WAG_Id { get; set; }
        public int CLE_Id { get; set; }
        public int CRI_Id { get; set; }
        public decimal WRA_Amount_2 { get; set; }
        public int Emp_ID { get; set; }
        public int FRM_ID { get; set; }
        public string Emp_Name { get; set; }

        public static Wage_Register_AllowancesVM_2 mapMe(Wage_Register_Allowances_2 allowances_2, int CRI_Id)
        {
            Wage_Register_AllowancesVM_2 allowancesVM_2 = new Wage_Register_AllowancesVM_2();
            allowancesVM_2.WRA_Id_2 = allowances_2.WRA_Id_2;
            allowancesVM_2.CLE_Id = allowances_2.CLE_Id;
            allowancesVM_2.WAG_Id = allowances_2.WAG_Id;
            allowancesVM_2.WRA_Amount_2 = allowances_2.WRA_Amount_2;
            allowancesVM_2.CRI_Id = CRI_Id;
            if (allowances_2.CLE != null)
            {
                if (allowances_2.CLE.EMP != null)
                {
                    allowancesVM_2.Emp_ID = allowances_2.CLE.EMP_Id;
                    allowancesVM_2.Emp_Name = allowances_2.CLE.EMP.EMP_FirstName + " " + allowances_2.CLE.EMP.EMP_MiddleName + " " + allowances_2.CLE.EMP.EMP_SurName;
                }
            }
            return allowancesVM_2;
        }
        public static Wage_Register_Allowances_2 mapMeModel(Wage_Register_AllowancesVM_2 allowanceVM)
        {
            Wage_Register_Allowances_2 allowance = new Wage_Register_Allowances_2();
            allowance.WRA_Id_2 = allowanceVM.WRA_Id_2;
            allowance.CLE_Id = allowanceVM.CLE_Id;
            allowance.WAG_Id = allowanceVM.WAG_Id;
            allowance.WRA_Amount_2 = allowanceVM.WRA_Amount_2;
            return allowance;
        }
        public static List<Wage_Register_Allowances_2> mapMeModels(List<Wage_Register_AllowancesVM_2> allowanceVMs)
        {
            List<Wage_Register_Allowances_2> lst = new List<Wage_Register_Allowances_2>();
            foreach (var item in allowanceVMs)
            {
                lst.Add(mapMeModel(item));
            }
            return lst;
        }
    }
    public class updateWageRegisterAllowance_2
    {
        public string CRI_Allowance_Name_2 { get; set; }
        public List<Wage_Register_AllowancesVM_2> AllowancesVMs { get; set; }
        public int CLI_Id { get; set; }
    }

    #endregion

    #region Wage_Register_Allowances_3
    public class Wage_Register_AllowancesVM_3
    {
        public int WRA_Id_3 { get; set; }
        public int WAG_Id { get; set; }
        public int CLE_Id { get; set; }
        public int CRI_Id { get; set; }
        public decimal WRA_Amount_3 { get; set; }
        public int Emp_ID { get; set; }
        public int FRM_ID { get; set; }
        public string Emp_Name { get; set; }

        public static Wage_Register_AllowancesVM_3 mapMe(Wage_Register_Allowances_3 allowances_3, int CRI_Id)
        {
            Wage_Register_AllowancesVM_3 allowancesVM_3 = new Wage_Register_AllowancesVM_3();
            allowancesVM_3.WRA_Id_3 = allowances_3.WRA_Id_3;
            allowancesVM_3.CLE_Id = allowances_3.CLE_Id;
            allowancesVM_3.WAG_Id = allowances_3.WAG_Id;
            allowancesVM_3.WRA_Amount_3 = allowances_3.WRA_Amount_3;
            allowancesVM_3.CRI_Id = CRI_Id;
            if (allowances_3.CLE != null)
            {
                if (allowances_3.CLE.EMP != null)
                {
                    allowancesVM_3.Emp_ID = allowances_3.CLE.EMP_Id;
                    allowancesVM_3.Emp_Name = allowances_3.CLE.EMP.EMP_FirstName + " " + allowances_3.CLE.EMP.EMP_MiddleName + " " + allowances_3.CLE.EMP.EMP_SurName;
                }
            }
            return allowancesVM_3;
        }
        public static Wage_Register_Allowances_3 mapMeModel(Wage_Register_AllowancesVM_3 allowanceVM)
        {
            Wage_Register_Allowances_3 allowance = new Wage_Register_Allowances_3();
            allowance.WRA_Id_3 = allowanceVM.WRA_Id_3;
            allowance.CLE_Id = allowanceVM.CLE_Id;
            allowance.WAG_Id = allowanceVM.WAG_Id;
            allowance.WRA_Amount_3 = allowanceVM.WRA_Amount_3;
            return allowance;
        }
        public static List<Wage_Register_Allowances_3> mapMeModels(List<Wage_Register_AllowancesVM_3> allowanceVMs)
        {
            List<Wage_Register_Allowances_3> lst = new List<Wage_Register_Allowances_3>();
            foreach (var item in allowanceVMs)
            {
                lst.Add(mapMeModel(item));
            }
            return lst;
        }
    }
    public class updateWageRegisterAllowance_3
    {
        public string CRI_Allowance_Name_3 { get; set; }
        public List<Wage_Register_AllowancesVM_3> AllowancesVMs { get; set; }
        public int CLI_Id { get; set; }
    }

    #endregion

    #region Wage_Register_Allowances_4
    public class Wage_Register_AllowancesVM_4
    {
        public int WRA_Id_4 { get; set; }
        public int WAG_Id { get; set; }
        public int CLE_Id { get; set; }
        public int CRI_Id { get; set; }
        public decimal WRA_Amount_4 { get; set; }
        public int Emp_ID { get; set; }
        public int FRM_ID { get; set; }
        public string Emp_Name { get; set; }

        public static Wage_Register_AllowancesVM_4 mapMe(Wage_Register_Allowances_4 allowances_4, int CRI_Id)
        {
            Wage_Register_AllowancesVM_4 allowancesVM_4 = new Wage_Register_AllowancesVM_4();
            allowancesVM_4.WRA_Id_4 = allowances_4.WRA_Id_4;
            allowancesVM_4.CLE_Id = allowances_4.CLE_Id;
            allowancesVM_4.WAG_Id = allowances_4.WAG_Id;
            allowancesVM_4.WRA_Amount_4 = allowances_4.WRA_Amount_4;
            allowancesVM_4.CRI_Id = CRI_Id;
            if (allowances_4.CLE != null)
            {
                if (allowances_4.CLE.EMP != null)
                {
                    allowancesVM_4.Emp_ID = allowances_4.CLE.EMP_Id;
                    allowancesVM_4.Emp_Name = allowances_4.CLE.EMP.EMP_FirstName + " " + allowances_4.CLE.EMP.EMP_MiddleName + " " + allowances_4.CLE.EMP.EMP_SurName;
                }
            }
            return allowancesVM_4;
        }
        public static Wage_Register_Allowances_4 mapMeModel(Wage_Register_AllowancesVM_4 allowanceVM)
        {
            Wage_Register_Allowances_4 allowance = new Wage_Register_Allowances_4();
            allowance.WRA_Id_4 = allowanceVM.WRA_Id_4;
            allowance.CLE_Id = allowanceVM.CLE_Id;
            allowance.WAG_Id = allowanceVM.WAG_Id;
            allowance.WRA_Amount_4 = allowanceVM.WRA_Amount_4;
            return allowance;
        }
        public static List<Wage_Register_Allowances_4> mapMeModels(List<Wage_Register_AllowancesVM_4> allowanceVMs)
        {
            List<Wage_Register_Allowances_4> lst = new List<Wage_Register_Allowances_4>();
            foreach (var item in allowanceVMs)
            {
                lst.Add(mapMeModel(item));
            }
            return lst;
        }
    }
    public class updateWageRegisterAllowance_4
    {
        public string CRI_Allowance_Name_4 { get; set; }
        public List<Wage_Register_AllowancesVM_4> AllowancesVMs { get; set; }
        public int CLI_Id { get; set; }
    }

    #endregion

    #region Wage_Register_Allowances_5
    public class Wage_Register_AllowancesVM_5
    {
        public int WRA_Id_5 { get; set; }
        public int WAG_Id { get; set; }
        public int CLE_Id { get; set; }
        public int CRI_Id { get; set; }
        public decimal WRA_Amount_5 { get; set; }
        public int Emp_ID { get; set; }
        public int FRM_ID { get; set; }
        public string Emp_Name { get; set; }

        public static Wage_Register_AllowancesVM_5 mapMe(Wage_Register_Allowances_5 allowances_5, int CRI_Id)
        {
            Wage_Register_AllowancesVM_5 allowancesVM_5 = new Wage_Register_AllowancesVM_5();
            allowancesVM_5.WRA_Id_5 = allowances_5.WRA_Id_5;
            allowancesVM_5.CLE_Id = allowances_5.CLE_Id;
            allowancesVM_5.WAG_Id = allowances_5.WAG_Id;
            allowancesVM_5.WRA_Amount_5 = allowances_5.WRA_Amount_5;
            allowancesVM_5.CRI_Id = CRI_Id;
            if (allowances_5.CLE != null)
            {
                if (allowances_5.CLE.EMP != null)
                {
                    allowancesVM_5.Emp_ID = allowances_5.CLE.EMP_Id;
                    allowancesVM_5.Emp_Name = allowances_5.CLE.EMP.EMP_FirstName + " " + allowances_5.CLE.EMP.EMP_MiddleName + " " + allowances_5.CLE.EMP.EMP_SurName;
                }
            }
            return allowancesVM_5;
        }
        public static Wage_Register_Allowances_5 mapMeModel(Wage_Register_AllowancesVM_5 allowanceVM)
        {
            Wage_Register_Allowances_5 allowance = new Wage_Register_Allowances_5();
            allowance.WRA_Id_5 = allowanceVM.WRA_Id_5;
            allowance.CLE_Id = allowanceVM.CLE_Id;
            allowance.WAG_Id = allowanceVM.WAG_Id;
            allowance.WRA_Amount_5 = allowanceVM.WRA_Amount_5;
            return allowance;
        }
        public static List<Wage_Register_Allowances_5> mapMeModels(List<Wage_Register_AllowancesVM_5> allowanceVMs)
        {
            List<Wage_Register_Allowances_5> lst = new List<Wage_Register_Allowances_5>();
            foreach (var item in allowanceVMs)
            {
                lst.Add(mapMeModel(item));
            }
            return lst;
        }
    }
    public class updateWageRegisterAllowance_5
    {
        public string CRI_Allowance_Name_5 { get; set; }
        public List<Wage_Register_AllowancesVM_5> AllowancesVMs { get; set; }
        public int CLI_Id { get; set; }
    }

    #endregion

    #region Wage_Register_Allowances_6
    public class Wage_Register_AllowancesVM_6
    {
        public int WRA_Id_6 { get; set; }
        public int WAG_Id { get; set; }
        public int CLE_Id { get; set; }
        public int CRI_Id { get; set; }
        public decimal WRA_Amount_6 { get; set; }
        public int Emp_ID { get; set; }
        public int FRM_ID { get; set; }
        public string Emp_Name { get; set; }

        public static Wage_Register_AllowancesVM_6 mapMe(Wage_Register_Allowances_6 allowances_6, int CRI_Id)
        {
            Wage_Register_AllowancesVM_6 allowancesVM_6 = new Wage_Register_AllowancesVM_6();
            allowancesVM_6.WRA_Id_6 = allowances_6.WRA_Id_6;
            allowancesVM_6.CLE_Id = allowances_6.CLE_Id;
            allowancesVM_6.WAG_Id = allowances_6.WAG_Id;
            allowancesVM_6.WRA_Amount_6 = allowances_6.WRA_Amount_6;
            allowancesVM_6.CRI_Id = CRI_Id;
            if (allowances_6.CLE != null)
            {
                if (allowances_6.CLE.EMP != null)
                {
                    allowancesVM_6.Emp_ID = allowances_6.CLE.EMP_Id;
                    allowancesVM_6.Emp_Name = allowances_6.CLE.EMP.EMP_FirstName + " " + allowances_6.CLE.EMP.EMP_MiddleName + " " + allowances_6.CLE.EMP.EMP_SurName;
                }
            }
            return allowancesVM_6;
        }
        public static Wage_Register_Allowances_6 mapMeModel(Wage_Register_AllowancesVM_6 allowanceVM)
        {
            Wage_Register_Allowances_6 allowance = new Wage_Register_Allowances_6();
            allowance.WRA_Id_6 = allowanceVM.WRA_Id_6;
            allowance.CLE_Id = allowanceVM.CLE_Id;
            allowance.WAG_Id = allowanceVM.WAG_Id;
            allowance.WRA_Amount_6 = allowanceVM.WRA_Amount_6;
            return allowance;
        }
        public static List<Wage_Register_Allowances_6> mapMeModels(List<Wage_Register_AllowancesVM_6> allowanceVMs)
        {
            List<Wage_Register_Allowances_6> lst = new List<Wage_Register_Allowances_6>();
            foreach (var item in allowanceVMs)
            {
                lst.Add(mapMeModel(item));
            }
            return lst;
        }
    }
    public class updateWageRegisterAllowance_6
    {
        public string CRI_Allowance_Name_6 { get; set; }
        public List<Wage_Register_AllowancesVM_6> AllowancesVMs { get; set; }
        public int CLI_Id { get; set; }
    }

    #endregion

    #region Wage_Register_Allowances_7

    public class Wage_Register_AllowancesVM_7
    {
        public int WRA_Id_7 { get; set; }
        public int WAG_Id { get; set; }
        public int CLE_Id { get; set; }
        public int CRI_Id { get; set; }
        public decimal WRA_Amount_7 { get; set; }
        public int Emp_ID { get; set; }
        public int FRM_ID { get; set; }
        public string Emp_Name { get; set; }

        public static Wage_Register_AllowancesVM_7 mapMe(Wage_Register_Allowances_7 allowances_7, int CRI_Id)
        {
            Wage_Register_AllowancesVM_7 allowancesVM = new Wage_Register_AllowancesVM_7();
            allowancesVM.WRA_Id_7 = allowances_7.WRA_Id_7;
            allowancesVM.CLE_Id = allowances_7.CLE_Id;
            allowancesVM.WAG_Id = allowances_7.WAG_Id;
            allowancesVM.WRA_Amount_7 = allowances_7.WRA_Amount_7;
            allowancesVM.CRI_Id = CRI_Id;
            if (allowances_7.CLE != null)
            {
                if (allowances_7.CLE.EMP != null)
                {
                    allowancesVM.Emp_ID = allowances_7.CLE.EMP_Id;
                    allowancesVM.Emp_Name = allowances_7.CLE.EMP.EMP_FirstName + " " + allowances_7.CLE.EMP.EMP_MiddleName + " " + allowances_7.CLE.EMP.EMP_SurName;
                }
            }
            return allowancesVM;
        }
        public static Wage_Register_Allowances_7 mapMeModel(Wage_Register_AllowancesVM_7 allowanceVM)
        {
            Wage_Register_Allowances_7 allowance = new Wage_Register_Allowances_7();
            allowance.WRA_Id_7 = allowanceVM.WRA_Id_7;
            allowance.CLE_Id = allowanceVM.CLE_Id;
            allowance.WAG_Id = allowanceVM.WAG_Id;
            allowance.WRA_Amount_7 = allowanceVM.WRA_Amount_7;
            return allowance;
        }
        public static List<Wage_Register_Allowances_7> mapMeModels(List<Wage_Register_AllowancesVM_7> allowanceVMs)
        {
            List<Wage_Register_Allowances_7> lst = new List<Wage_Register_Allowances_7>();
            foreach (var item in allowanceVMs)
            {
                lst.Add(mapMeModel(item));
            }
            return lst;
        }
    }

    public class updateWageRegisterAllowance_7
    {
        public string CRI_Allowance_Name_7 { get; set; }
        public List<Wage_Register_AllowancesVM_7> AllowancesVMs { get; set; }
        public int CLI_Id { get; set; }
    }

    #endregion

    #region Wage_Register_Allowances_8

    public class Wage_Register_AllowancesVM_8
    {
        public int WRA_Id_8 { get; set; }
        public int WAG_Id { get; set; }
        public int CLE_Id { get; set; }
        public int CRI_Id { get; set; }
        public decimal WRA_Amount_8 { get; set; }
        public int Emp_ID { get; set; }
        public int FRM_ID { get; set; }
        public string Emp_Name { get; set; }

        public static Wage_Register_AllowancesVM_8 mapMe(Wage_Register_Allowances_8 allowances_8, int CRI_Id)
        {
            Wage_Register_AllowancesVM_8 allowancesVM = new Wage_Register_AllowancesVM_8();
            allowancesVM.WRA_Id_8 = allowances_8.WRA_Id_8;
            allowancesVM.CLE_Id = allowances_8.CLE_Id;
            allowancesVM.WAG_Id = allowances_8.WAG_Id;
            allowancesVM.WRA_Amount_8 = allowances_8.WRA_Amount_8;
            allowancesVM.CRI_Id = CRI_Id;
            if (allowances_8.CLE != null)
            {
                if (allowances_8.CLE.EMP != null)
                {
                    allowancesVM.Emp_ID = allowances_8.CLE.EMP_Id;
                    allowancesVM.Emp_Name = allowances_8.CLE.EMP.EMP_FirstName + " " + allowances_8.CLE.EMP.EMP_MiddleName + " " + allowances_8.CLE.EMP.EMP_SurName;
                }
            }
            return allowancesVM;
        }
        public static Wage_Register_Allowances_8 mapMeModel(Wage_Register_AllowancesVM_8 allowanceVM)
        {
            Wage_Register_Allowances_8 allowance = new Wage_Register_Allowances_8();
            allowance.WRA_Id_8 = allowanceVM.WRA_Id_8;
            allowance.CLE_Id = allowanceVM.CLE_Id;
            allowance.WAG_Id = allowanceVM.WAG_Id;
            allowance.WRA_Amount_8 = allowanceVM.WRA_Amount_8;
            return allowance;
        }
        public static List<Wage_Register_Allowances_8> mapMeModels(List<Wage_Register_AllowancesVM_8> allowanceVMs)
        {
            List<Wage_Register_Allowances_8> lst = new List<Wage_Register_Allowances_8>();
            foreach (var item in allowanceVMs)
            {
                lst.Add(mapMeModel(item));
            }
            return lst;
        }
    }
    public class updateWageRegisterAllowance_8
    {
        public string CRI_Allowance_Name_8 { get; set; }
        public List<Wage_Register_AllowancesVM_8> AllowancesVMs { get; set; }
        public int CLI_Id { get; set; }
    }

    #endregion

    #region Wage_Register_Allowances_9

    public class Wage_Register_AllowancesVM_9
    {
        public int WRA_Id_9 { get; set; }
        public int WAG_Id { get; set; }
        public int CLE_Id { get; set; }
        public int CRI_Id { get; set; }
        public decimal WRA_Amount_9 { get; set; }
        public int Emp_ID { get; set; }
        public int FRM_ID { get; set; }
        public string Emp_Name { get; set; }

        public static Wage_Register_AllowancesVM_9 mapMe(Wage_Register_Allowances_9 allowances, int CRI_Id)
        {
            Wage_Register_AllowancesVM_9 allowancesVM = new Wage_Register_AllowancesVM_9();
            allowancesVM.WRA_Id_9 = allowances.WRA_Id_9;
            allowancesVM.CLE_Id = allowances.CLE_Id;
            allowancesVM.WAG_Id = allowances.WAG_Id;
            allowancesVM.WRA_Amount_9 = allowances.WRA_Amount_9;
            allowancesVM.CRI_Id = CRI_Id;
            if (allowances.CLE != null)
            {
                if (allowances.CLE.EMP != null)
                {
                    allowancesVM.Emp_ID = allowances.CLE.EMP_Id;
                    allowancesVM.Emp_Name = allowances.CLE.EMP.EMP_FirstName + " " + allowances.CLE.EMP.EMP_MiddleName + " " + allowances.CLE.EMP.EMP_SurName;
                }
            }
            return allowancesVM;
        }
        public static Wage_Register_Allowances_9 mapMeModel(Wage_Register_AllowancesVM_9 allowanceVM)
        {
            Wage_Register_Allowances_9 allowance = new Wage_Register_Allowances_9();
            allowance.WRA_Id_9 = allowanceVM.WRA_Id_9;
            allowance.CLE_Id = allowanceVM.CLE_Id;
            allowance.WAG_Id = allowanceVM.WAG_Id;
            allowance.WRA_Amount_9 = allowanceVM.WRA_Amount_9;
            return allowance;
        }
        public static List<Wage_Register_Allowances_9> mapMeModels(List<Wage_Register_AllowancesVM_9> allowanceVMs)
        {
            List<Wage_Register_Allowances_9> lst = new List<Wage_Register_Allowances_9>();
            foreach (var item in allowanceVMs)
            {
                lst.Add(mapMeModel(item));
            }
            return lst;
        }
    }
    public class updateWageRegisterAllowance_9
    {
        public string CRI_Allowance_Name_9 { get; set; }
        public List<Wage_Register_AllowancesVM_9> AllowancesVMs { get; set; }
        public int CLI_Id { get; set; }
    }

    #endregion

    #region Wage_Register_Allowances_10

    public class Wage_Register_AllowancesVM_10
    {
        public int WRA_Id_10 { get; set; }
        public int WAG_Id { get; set; }
        public int CLE_Id { get; set; }
        public int CRI_Id { get; set; }
        public decimal WRA_Amount_10 { get; set; }
        public int Emp_ID { get; set; }
        public int FRM_ID { get; set; }
        public string Emp_Name { get; set; }

        public static Wage_Register_AllowancesVM_10 mapMe(Wage_Register_Allowances_10 allowances, int CRI_Id)
        {
            Wage_Register_AllowancesVM_10 allowancesVM = new Wage_Register_AllowancesVM_10();
            allowancesVM.WRA_Id_10 = allowances.WRA_Id_10;
            allowancesVM.CLE_Id = allowances.CLE_Id;
            allowancesVM.WAG_Id = allowances.WAG_Id;
            allowancesVM.WRA_Amount_10 = allowances.WRA_Amount_10;
            allowancesVM.CRI_Id = CRI_Id;
            if (allowances.CLE != null)
            {
                if (allowances.CLE.EMP != null)
                {
                    allowancesVM.Emp_ID = allowances.CLE.EMP_Id;
                    allowancesVM.Emp_Name = allowances.CLE.EMP.EMP_FirstName + " " + allowances.CLE.EMP.EMP_MiddleName + " " + allowances.CLE.EMP.EMP_SurName;
                }
            }
            return allowancesVM;
        }
        public static Wage_Register_Allowances_10 mapMeModel(Wage_Register_AllowancesVM_10 allowanceVM)
        {
            Wage_Register_Allowances_10 allowance = new Wage_Register_Allowances_10();
            allowance.WRA_Id_10 = allowanceVM.WRA_Id_10;
            allowance.CLE_Id = allowanceVM.CLE_Id;
            allowance.WAG_Id = allowanceVM.WAG_Id;
            allowance.WRA_Amount_10 = allowanceVM.WRA_Amount_10;
            return allowance;
        }
        public static List<Wage_Register_Allowances_10> mapMeModels(List<Wage_Register_AllowancesVM_10> allowanceVMs)
        {
            List<Wage_Register_Allowances_10> lst = new List<Wage_Register_Allowances_10>();
            foreach (var item in allowanceVMs)
            {
                lst.Add(mapMeModel(item));
            }
            return lst;
        }
    }
    public class updateWageRegisterAllowance_10
    {
        public string CRI_Allowance_Name_10 { get; set; }
        public List<Wage_Register_AllowancesVM_10> AllowancesVMs { get; set; }
        public int CLI_Id { get; set; }
    }

    #endregion


}
