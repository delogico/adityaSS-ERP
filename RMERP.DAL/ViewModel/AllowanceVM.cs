using System;
using System.Collections.Generic;
using System.Text;
using RMERP.DAL.Models;

namespace RMERP.DAL.ViewModel
{
    public class AllowanceVM
    {
        public int ALL_Id { get; set; }
        public string ALL_Title { get; set; }
        public string ALL_Alias { get; set; }
        public string ALL_Shortform { get; set; }
    }

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
            if (allowances_1.CLE_ != null)
            {
                if (allowances_1.CLE_.EMP_ != null)
                {
                    allowancesVM_1.Emp_ID = allowances_1.CLE_.EMP_Id;
                    allowancesVM_1.Emp_Name = allowances_1.CLE_.EMP_.EMP_FirstName + " " + allowances_1.CLE_.EMP_.EMP_MiddleName + " " + allowances_1.CLE_.EMP_.EMP_SurName;
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
    }

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
            if (allowances_2.CLE_ != null)
            {
                if (allowances_2.CLE_.EMP_ != null)
                {
                    allowancesVM_2.Emp_ID = allowances_2.CLE_.EMP_Id;
                    allowancesVM_2.Emp_Name = allowances_2.CLE_.EMP_.EMP_FirstName + " " + allowances_2.CLE_.EMP_.EMP_MiddleName + " " + allowances_2.CLE_.EMP_.EMP_SurName;
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
    }

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
            if (allowances_3.CLE_ != null)
            {
                if (allowances_3.CLE_.EMP_ != null)
                {
                    allowancesVM_3.Emp_ID = allowances_3.CLE_.EMP_Id;
                    allowancesVM_3.Emp_Name = allowances_3.CLE_.EMP_.EMP_FirstName + " " + allowances_3.CLE_.EMP_.EMP_MiddleName + " " + allowances_3.CLE_.EMP_.EMP_SurName;
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
    }

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
            if (allowances_4.CLE_ != null)
            {
                if (allowances_4.CLE_.EMP_ != null)
                {
                    allowancesVM_4.Emp_ID = allowances_4.CLE_.EMP_Id;
                    allowancesVM_4.Emp_Name = allowances_4.CLE_.EMP_.EMP_FirstName + " " + allowances_4.CLE_.EMP_.EMP_MiddleName + " " + allowances_4.CLE_.EMP_.EMP_SurName;
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
    }

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
            if (allowances_5.CLE_ != null)
            {
                if (allowances_5.CLE_.EMP_ != null)
                {
                    allowancesVM_5.Emp_ID = allowances_5.CLE_.EMP_Id;
                    allowancesVM_5.Emp_Name = allowances_5.CLE_.EMP_.EMP_FirstName + " " + allowances_5.CLE_.EMP_.EMP_MiddleName + " " + allowances_5.CLE_.EMP_.EMP_SurName;
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
    }


    
}
