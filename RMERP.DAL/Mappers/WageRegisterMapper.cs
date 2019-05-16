using System;
using System.Collections.Generic;
using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace RMERP.DAL.Mappers
{
    public class WageRegisterMapper
    {
        public static WageRegisterVM mapMe(Wage_Register wageRegister)
        {
            WageRegisterVM wageRegisterVM = new WageRegisterVM();
            wageRegisterVM.WAR_Id = wageRegister.WAR_Id;
            wageRegisterVM.WAG_Id = wageRegister.WAG_Id;
            wageRegisterVM.EMP_Id = wageRegister.EMP_Id;
            wageRegisterVM.CLI_Id = wageRegister.CLI_Id;
            wageRegisterVM.CRI_Id = wageRegister.CRI_Id;
            wageRegisterVM.WAR_Basic = wageRegister.WAR_Basic;
            wageRegisterVM.WAR_Basic_Calculated = wageRegister.WAR_Basic_Calculated;
            wageRegisterVM.WAR_DA = wageRegister.WAR_DA;
            wageRegisterVM.WAR_DA_Calculated = wageRegister.WAR_DA_Calculated;
            wageRegisterVM.WAR_ESIC = wageRegister.WAR_ESIC;
            wageRegisterVM.WAR_ESIC_Calculated = wageRegister.WAR_ESIC_Calculated;
            wageRegisterVM.WAR_ExtraWorkingHours = wageRegister.WAR_ExtraWorkingHours;
            wageRegisterVM.WAR_FinalTotal = wageRegister.WAR_FinalTotal;
            wageRegisterVM.WAR_GrossTotal = wageRegister.WAR_GrossTotal;
            wageRegisterVM.WAR_HRA = wageRegister.WAR_HRA;
            wageRegisterVM.WAR_HRA_Calculated = wageRegister.WAR_HRA_Calculated;
            wageRegisterVM.WAR_LastModifiedOn = wageRegister.WAR_LastModifiedOn.Value;
            wageRegisterVM.WAR_OverTime_Calculated = wageRegister.WAR_OverTime_Calculated;
            wageRegisterVM.WAR_PF_Formula = wageRegister.WAR_PF_Formula;
            wageRegisterVM.WAR_ESIC_Formula = wageRegister.WAR_ESIC_Formula;
            wageRegisterVM.WAR_PF = wageRegister.WAR_PF;
            wageRegisterVM.WAR_PF_Calculated = wageRegister.WAR_PF_Calculated;
            wageRegisterVM.WAR_TotalPaybleDays = wageRegister.WAR_TotalPaybleDays;
            wageRegisterVM.WAR_TotalWorkingDays = wageRegister.WAR_TotalWorkingDays;

            if (wageRegister.WAR_ProffesionalTax_Calculated != null)
                wageRegisterVM.WAR_ProffesionalTax_Calculated = wageRegister.WAR_ProffesionalTax_Calculated;
            if (wageRegister.WAR_RevenueDeduction_Calculated != null)
                wageRegisterVM.WAR_RevenueDeduction_Calculated = wageRegister.WAR_RevenueDeduction_Calculated;
            if (wageRegister.WAR_CanteenFacility_Calculation != null)
                wageRegisterVM.WAR_CanteenFacility_Calculation = wageRegister.WAR_CanteenFacility_Calculation;

            if (wageRegister.WAR_Advance_Amount != null)
                wageRegisterVM.WAR_Advance_Amount = wageRegister.WAR_Advance_Amount.Value;
            if (wageRegister.CRI_ != null)
            {
                wageRegisterVM.clientRequirementVM = ClientRequirementMapper.mapMe(wageRegister.CRI_);
                if (wageRegister.CRI_.DES_ != null)
                    wageRegisterVM.designation = wageRegister.CRI_.DES_;
            }
            if (wageRegister.EMP_ != null)
                wageRegisterVM.employeeVM = EmployeesMapper.MapMe(wageRegister.EMP_);

            if (wageRegister.Wage_Register_Allowances != null)
                wageRegisterVM.allowanceVMs = mapWageAllowances(wageRegister.Wage_Register_Allowances.ToList());
            return wageRegisterVM;
        }

        public static List<WageRegisterVM> mapWageRegisters(List<Wage_Register> wageRegisters)
        {
            List<WageRegisterVM> lst = new List<WageRegisterVM>();
            foreach (Wage_Register wageRegister in wageRegisters)
                lst.Add(mapMe(wageRegister));
            return lst;
        }
        public static Wage_Register mapMe(WageRegisterVM wageRegisterVM)
        {
            Wage_Register wageRegister = new Wage_Register();
            wageRegister.CLI_Id = wageRegisterVM.CLI_Id;
            wageRegister.WAG_Id = wageRegisterVM.WAG_Id;
            wageRegister.EMP_Id = wageRegisterVM.EMP_Id;
            wageRegister.CRI_Id = wageRegisterVM.CRI_Id;
            wageRegister.WAR_Id = wageRegisterVM.WAR_Id;
            wageRegister.WAR_Basic = wageRegisterVM.WAR_Basic;
            wageRegister.WAR_Basic_Calculated = wageRegisterVM.WAR_Basic_Calculated;
            wageRegister.WAR_DA = wageRegisterVM.WAR_DA;
            wageRegister.WAR_DA_Calculated = wageRegisterVM.WAR_DA_Calculated;
            wageRegister.WAR_ESIC = wageRegisterVM.WAR_ESIC;
            wageRegister.WAR_ESIC_Formula = wageRegisterVM.WAR_ESIC_Formula;
            wageRegister.WAR_ESIC_Calculated = wageRegisterVM.WAR_ESIC_Calculated;
            wageRegister.WAR_ExtraWorkingHours = wageRegisterVM.WAR_ExtraWorkingHours;
            wageRegister.WAR_FinalTotal = wageRegisterVM.WAR_FinalTotal;
            wageRegister.WAR_GrossTotal = wageRegisterVM.WAR_GrossTotal;
            wageRegister.WAR_HRA = wageRegisterVM.WAR_HRA;
            wageRegister.WAR_HRA_Calculated = wageRegisterVM.WAR_HRA_Calculated;
            wageRegister.WAR_LastModifiedOn = wageRegisterVM.WAR_LastModifiedOn;
            wageRegister.ADM_LastModifiedBy = wageRegisterVM.ADM_LastModifiedBy;
            wageRegister.WAR_OverTime_Calculated = wageRegisterVM.WAR_OverTime_Calculated;
            wageRegister.WAR_PF = wageRegisterVM.WAR_PF;
            wageRegister.WAR_PF_Formula = wageRegisterVM.WAR_PF_Formula;
            wageRegister.WAR_PF_Calculated = wageRegisterVM.WAR_PF_Calculated;
            wageRegister.WAR_TotalPaybleDays = wageRegisterVM.WAR_TotalPaybleDays;
            wageRegister.WAR_TotalWorkingDays = wageRegisterVM.WAR_TotalWorkingDays;

            wageRegister.WAR_ProffesionalTax_Calculated = wageRegisterVM.WAR_ProffesionalTax_Calculated;
            wageRegister.WAR_RevenueDeduction_Calculated = wageRegisterVM.WAR_RevenueDeduction_Calculated;
            wageRegister.WAR_CanteenFacility_Calculation = wageRegisterVM.WAR_CanteenFacility_Calculation;

            if (wageRegisterVM.WAR_Advance_Amount != null)
                wageRegister.WAR_Advance_Amount = wageRegisterVM.WAR_Advance_Amount;
            if (wageRegisterVM.employeeVM != null)
                wageRegister.EMP_ = EmployeesMapper.MapMeModel(wageRegisterVM.employeeVM);
            if (wageRegisterVM.clientRequirementVM != null)
                wageRegister.CRI_ = ClientRequirementMapper.mapMeModel(wageRegisterVM.clientRequirementVM);
            if (wageRegisterVM.allowanceVMs != null)
                wageRegister.Wage_Register_Allowances = mapWageAllowancesList(wageRegisterVM.allowanceVMs);
            return wageRegister;
        }

        public static List<Wage_Register> mapWageRegisters(List<WageRegisterVM> WageRegisterVM)
        {
            List<Wage_Register> lst = new List<Wage_Register>();
            foreach (WageRegisterVM wageRegisterVM in WageRegisterVM)
                lst.Add(mapMe(wageRegisterVM));
            return lst;
        }

        public static WageRegisterAllowanceVM mapMeAllowance(Wage_Register_Allowances wage_Register_Allowances)
        {
            WageRegisterAllowanceVM wageRegisterAllowanceVM = new WageRegisterAllowanceVM();
            wageRegisterAllowanceVM.WAA_Amount = wage_Register_Allowances.WAA_Amount;
            wageRegisterAllowanceVM.WAA_Amount_Calculated = wage_Register_Allowances.WAA_Amount_Calculated;
            wageRegisterAllowanceVM.WAA_Id = wage_Register_Allowances.WAA_Id;
            if (wage_Register_Allowances.CRA_.ALL_ != null)
                wageRegisterAllowanceVM.allowanceVM = AllowanceMapper.mapMe(wage_Register_Allowances.CRA_.ALL_);
            return wageRegisterAllowanceVM;
        }

        public static Wage_Register_Allowances mapMeAllowanceM(WageRegisterAllowanceVM wage_Register_Allowances)
        {
            Wage_Register_Allowances wageRegisterAllowanceVM = new Wage_Register_Allowances();
            wageRegisterAllowanceVM.WAA_Amount = wage_Register_Allowances.WAA_Amount;
            wageRegisterAllowanceVM.WAA_Amount_Calculated = wage_Register_Allowances.WAA_Amount_Calculated;
            wageRegisterAllowanceVM.WAA_Id = wage_Register_Allowances.WAA_Id;
            wageRegisterAllowanceVM.CRA_Id = wage_Register_Allowances.CRA_Id;
            return wageRegisterAllowanceVM;
        }
        public static List<Wage_Register_Allowances> mapWageAllowancesList(List<WageRegisterAllowanceVM> wageRegisters)
        {
            List<Wage_Register_Allowances> lst = new List<Wage_Register_Allowances>();
            foreach (WageRegisterAllowanceVM wageReg in wageRegisters)
                lst.Add(mapMeAllowanceM(wageReg));
            return lst;
        }
        public static List<WageRegisterAllowanceVM> mapWageAllowances(List<Wage_Register_Allowances> wageRegisters)
        {
            List<WageRegisterAllowanceVM> lst = new List<WageRegisterAllowanceVM>();
            foreach (Wage_Register_Allowances wageReg in wageRegisters)
                lst.Add(mapMeAllowance(wageReg));
            return lst;
        }
    }
}
