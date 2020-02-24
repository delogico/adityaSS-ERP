using System;
using System.Collections.Generic;
using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace RMERP.DAL.Mappers
{
    public class ClientRequirementMapper
    {
        public static ClientRequirementVM mapMe(Client_Requirements requirement)
        {
            ClientRequirementVM clientRequirementVM = new ClientRequirementVM();
            clientRequirementVM.CRI_Id = requirement.CRI_Id;
            clientRequirementVM.CLI_Id = requirement.CLI_Id;
            clientRequirementVM.DES_Id = requirement.DES_Id;
            clientRequirementVM.CRI_Total = requirement.CRI_Total;
            clientRequirementVM.CRI_Basic = requirement.CRI_Basic;
            clientRequirementVM.CRI_DA = requirement.CRI_DA;
            clientRequirementVM.CRI_HRA_Fixed = requirement.CRI_HRA_Fixed;
            clientRequirementVM.DES_Title = requirement.DES_.DES_Title;
            clientRequirementVM.CRI_HRA_Percentage = requirement.CRI_HRA_Percentage;
            clientRequirementVM.CRI_PF_Percentage = requirement.CRI_PF_Percentage;
            clientRequirementVM.CRI_ESIC_Percentage = requirement.CRI_ESIC_Percentage;
            clientRequirementVM.CRI_ESIC_Area = requirement.CRI_ESIC_Area;
            clientRequirementVM.CRI_OT_Formula = requirement.CRI_OT_Formula;
            clientRequirementVM.CRI_OT_Rate = requirement.CRI_OT_Rate;
            clientRequirementVM.CRI_OT_MultipleTimes = requirement.CRI_OT_MultipleTimes;
            clientRequirementVM.CRI_IsPayable_WeeklyOff = requirement.CRI_IsPayable_WeeklyOff;
            clientRequirementVM.CRI_IsPayable_PublicHoliday = requirement.CRI_IsPayable_PublicHoliday;
            clientRequirementVM.CRI_Active = requirement.CRI_Active;
            clientRequirementVM.CRI_InactivatedOn = requirement.CRI_InactivatedOn;
            clientRequirementVM.CRI_RegisteredOn = requirement.CRI_RegisteredOn;
            clientRequirementVM.CRI_PF_Formula = requirement.CRI_PF_Formula;
            clientRequirementVM.CRI_ESIC_Formula = requirement.CRI_ESIC_Formula;

            clientRequirementVM.CRI_ProfessionalTax = requirement.CRI_ProfessionalTax;
            clientRequirementVM.CRI_RevenueDeduction =requirement.CRI_RevenueDeduction;
            clientRequirementVM.CRI_CanteenFacility = requirement.CRI_CanteenFacility;

            clientRequirementVM.CRI_OT_Calculate_Payableday = requirement.CRI_OT_Calculate_Payableday;
            clientRequirementVM.CRI_OT_Fixed_PerHour = requirement.CRI_OT_Fixed_PerHour;

            clientRequirementVM.CRI_OutStation_Allowance = requirement.CRI_OutStation_Allowance;
            clientRequirementVM.CRI_OutStation_Allowance_Rate = requirement.CRI_OutStation_Allowance_Rate;
            clientRequirementVM.CRI_Attendance_Allowance = requirement.CRI_Attendance_Allowance;
            clientRequirementVM.CRI_Attendance_Allowance_Rate = requirement.CRI_Attendance_Allowance_Rate;
            clientRequirementVM.CRI_Attendance_Allowance_MaximumDays = requirement.CRI_Attendance_Allowance_MaximumDays;

            clientRequirementVM.CRI_Performance_Allowance = requirement.CRI_Performance_Allowance;            
            clientRequirementVM.CRI_Nightshift_Allowance = requirement.CRI_Nightshift_Allowance;
            clientRequirementVM.CRI_Nightshift_Allowance_Rate = requirement.CRI_Nightshift_Allowance_Rate;

            if (clientRequirementVM.CRI_OT_Formula!=null)
            {
                clientRequirementVM.CRI_OT_Calculate_Differently = false;
            }
            
            if (requirement.CLI_ != null)
                clientRequirementVM.CLI_Name = requirement.CLI_.CLI_Name;

            if (requirement.CRI_HRA_Fixed != null)
                clientRequirementVM.HRAselection = true;
            else if(requirement.CRI_HRA_Percentage !=null)
                clientRequirementVM.HRAselection = false;

            clientRequirementVM.CRI_Billing_Amount = requirement.CRI_Billing_Amount;
            clientRequirementVM.CRI_Billing_ServiceCharge = requirement.CRI_Billing_ServiceCharge;
            clientRequirementVM.CRI_Billing_ServiceCharge_Formula = requirement.CRI_Billing_ServiceCharge_Formula;
            clientRequirementVM.CRI_Billing_Type = requirement.CRI_Billing_Type;

            clientRequirementVM.CRI_PF_Employer_Cont_Rate = requirement.CRI_PF_Employer_Cont_Rate;
            clientRequirementVM.CRI_ESIC_Employer_Cont_Rate = requirement.CRI_ESIC_Employer_Cont_Rate;
            clientRequirementVM.CRI_EPS_Rate = requirement.CRI_EPS_Rate;

            clientRequirementVM.CRI_MLWF_Employer_GThen = requirement.CRI_MLWF_Employer_GThen;
            clientRequirementVM.CRI_MLWF_Employer_LThen = requirement.CRI_MLWF_Employer_LThen;
            clientRequirementVM.CRI_MLWF_Employee_GThen = requirement.CRI_MLWF_Employee_GThen;
            clientRequirementVM.CRI_MLWF_Employee_LThen = requirement.CRI_MLWF_Employee_LThen;
            clientRequirementVM.CRI_MLWF_Employer_Base = requirement.CRI_MLWF_Employer_Base;
            clientRequirementVM.CRI_MLWF_Employee_Base = requirement.CRI_MLWF_Employee_Base;
            clientRequirementVM.CRI_MLWF_Employer_Max_Base = requirement.CRI_MLWF_Employer_Base;
            clientRequirementVM.CRI_MLWF_Employee_Max_Base = requirement.CRI_MLWF_Employee_Base;

            clientRequirementVM.CRI_ProffTax_M_From_1 = requirement.CRI_ProffTax_M_From_1;
            clientRequirementVM.CRI_ProffTax_M_To_1 = requirement.CRI_ProffTax_M_To_1;
            clientRequirementVM.CRI_ProffTax_M_Amount_1 = requirement.CRI_ProffTax_M_Amount_1;
            clientRequirementVM.CRI_ProffTax_M_From_2 = requirement.CRI_ProffTax_M_From_2;
            clientRequirementVM.CRI_ProffTax_M_To_2 = requirement.CRI_ProffTax_M_To_2;
            clientRequirementVM.CRI_ProffTax_M_Amount_2 = requirement.CRI_ProffTax_M_Amount_2;
            clientRequirementVM.CRI_ProffTax_M_From_3 = requirement.CRI_ProffTax_M_From_3;
            clientRequirementVM.CRI_ProffTax_M_To_3 = requirement.CRI_ProffTax_M_To_3;
            clientRequirementVM.CRI_ProffTax_M_Amount_3 = requirement.CRI_ProffTax_M_Amount_3;

            clientRequirementVM.CRI_ProffTax_F_From_1 = requirement.CRI_ProffTax_F_From_1;
            clientRequirementVM.CRI_ProffTax_F_To_1 = requirement.CRI_ProffTax_F_To_1;
            clientRequirementVM.CRI_ProffTax_F_Amount_1 = requirement.CRI_ProffTax_F_Amount_1;
            clientRequirementVM.CRI_ProffTax_F_From_2 = requirement.CRI_ProffTax_F_From_2;
            clientRequirementVM.CRI_ProffTax_F_To_2 = requirement.CRI_ProffTax_F_To_2;
            clientRequirementVM.CRI_ProffTax_F_Amount_2 = requirement.CRI_ProffTax_F_Amount_2;
            clientRequirementVM.CRI_ProffTax_F_From_3 = requirement.CRI_ProffTax_F_From_3;
            clientRequirementVM.CRI_ProffTax_F_To_3 = requirement.CRI_ProffTax_F_To_3;
            clientRequirementVM.CRI_ProffTax_F_Amount_3 = requirement.CRI_ProffTax_F_Amount_3;

            return clientRequirementVM;
        }       
        public static Client_Requirements mapMeModel(ClientRequirementVM requirementVM)
        {
            Client_Requirements requirement = new Client_Requirements();
            requirement.CRI_Id = requirementVM.CRI_Id;
            requirement.CLI_Id = requirementVM.CLI_Id;
            requirement.DES_Id = requirementVM.DES_Id;
            requirement.CRI_Total = requirementVM.CRI_Total;
            requirement.CRI_Basic = requirementVM.CRI_Basic;
            requirement.CRI_DA = requirementVM.CRI_DA;
          
            requirement.CRI_PF_Percentage = requirementVM.CRI_PF_Percentage;
            requirement.CRI_ESIC_Percentage = requirementVM.CRI_ESIC_Percentage;
            requirement.CRI_ESIC_Area = requirementVM.CRI_ESIC_Area;
            requirement.CRI_OT_Formula = requirementVM.CRI_OT_Formula;
            requirement.CRI_OT_Rate = requirementVM.CRI_OT_Rate;
            requirement.CRI_OT_MultipleTimes = requirementVM.CRI_OT_MultipleTimes;
            requirement.CRI_IsPayable_WeeklyOff = requirementVM.CRI_IsPayable_WeeklyOff;
            requirement.CRI_IsPayable_PublicHoliday = requirementVM.CRI_IsPayable_PublicHoliday;
            requirement.CRI_Active = requirementVM.CRI_Active;
            requirement.CRI_InactivatedOn = requirementVM.CRI_InactivatedOn;
            requirement.CRI_RegisteredOn = requirementVM.CRI_RegisteredOn;
            requirement.CRI_PF_Formula = requirementVM.CRI_PF_Formula;
            requirement.CRI_ESIC_Formula = requirementVM.CRI_ESIC_Formula;

            requirement.CRI_ProfessionalTax = requirementVM.CRI_ProfessionalTax;
            requirement.CRI_RevenueDeduction = requirementVM.CRI_RevenueDeduction;
            requirement.CRI_CanteenFacility = requirementVM.CRI_CanteenFacility;

            requirement.CRI_OT_Calculate_Payableday = requirementVM.CRI_OT_Calculate_Payableday;
            requirement.CRI_OT_Fixed_PerHour = requirementVM.CRI_OT_Fixed_PerHour;

            requirement.CRI_OutStation_Allowance = requirementVM.CRI_OutStation_Allowance;
            requirement.CRI_OutStation_Allowance_Rate = requirementVM.CRI_OutStation_Allowance_Rate;
            requirement.CRI_Attendance_Allowance = requirementVM.CRI_Attendance_Allowance;
            requirement.CRI_Attendance_Allowance_Rate = requirementVM.CRI_Attendance_Allowance_Rate;
            requirement.CRI_Attendance_Allowance_MaximumDays = requirementVM.CRI_Attendance_Allowance_MaximumDays;

            requirement.CRI_Performance_Allowance = requirementVM.CRI_Performance_Allowance;            
            requirement.CRI_Nightshift_Allowance = requirementVM.CRI_Nightshift_Allowance;
            requirement.CRI_Nightshift_Allowance_Rate = requirementVM.CRI_Nightshift_Allowance_Rate;

            if (requirementVM.HRAselection ==true)
                requirement.CRI_HRA_Fixed = requirementVM.CRI_HRA_Fixed;
            else if(requirementVM.HRAselection == false)
                requirement.CRI_HRA_Percentage = requirementVM.CRI_HRA_Percentage;

            requirement.CRI_Billing_Amount = requirementVM.CRI_Billing_Amount;
            requirement.CRI_Billing_ServiceCharge = requirementVM.CRI_Billing_ServiceCharge;
            requirement.CRI_Billing_ServiceCharge_Formula = requirementVM.CRI_Billing_ServiceCharge_Formula;
            requirement.CRI_Billing_Type = requirementVM.CRI_Billing_Type;

            requirement.CRI_PF_Employer_Cont_Rate = requirementVM.CRI_PF_Employer_Cont_Rate;
            requirement.CRI_ESIC_Employer_Cont_Rate = requirementVM.CRI_ESIC_Employer_Cont_Rate;
            requirement.CRI_EPS_Rate = requirementVM.CRI_EPS_Rate;

            requirement.CRI_MLWF_Employer_GThen = requirementVM.CRI_MLWF_Employer_GThen;
            requirement.CRI_MLWF_Employer_LThen = requirementVM.CRI_MLWF_Employer_LThen;
            requirement.CRI_MLWF_Employee_GThen = requirementVM.CRI_MLWF_Employee_GThen;
            requirement.CRI_MLWF_Employee_LThen = requirementVM.CRI_MLWF_Employee_LThen;
            requirement.CRI_MLWF_Employer_Base = requirementVM.CRI_MLWF_Employer_Base;
            requirement.CRI_MLWF_Employee_Base = requirementVM.CRI_MLWF_Employee_Base;

            requirement.CRI_ProffTax_M_From_1 = requirementVM.CRI_ProffTax_M_From_1;
            requirement.CRI_ProffTax_M_To_1 = requirementVM.CRI_ProffTax_M_To_1;
            requirement.CRI_ProffTax_M_Amount_1 = requirementVM.CRI_ProffTax_M_Amount_1;
            requirement.CRI_ProffTax_M_From_2 = requirementVM.CRI_ProffTax_M_From_2;
            requirement.CRI_ProffTax_M_To_2 = requirementVM.CRI_ProffTax_M_To_2;
            requirement.CRI_ProffTax_M_Amount_2 = requirementVM.CRI_ProffTax_M_Amount_2;
            requirement.CRI_ProffTax_M_From_3 = requirementVM.CRI_ProffTax_M_From_3;
            requirement.CRI_ProffTax_M_To_3 = requirementVM.CRI_ProffTax_M_To_3;
            requirement.CRI_ProffTax_M_Amount_3 = requirementVM.CRI_ProffTax_M_Amount_3;

            requirement.CRI_ProffTax_F_From_1 = requirementVM.CRI_ProffTax_F_From_1;
            requirement.CRI_ProffTax_F_To_1 = requirementVM.CRI_ProffTax_F_To_1;
            requirement.CRI_ProffTax_F_Amount_1 = requirementVM.CRI_ProffTax_F_Amount_1;
            requirement.CRI_ProffTax_F_From_2 = requirementVM.CRI_ProffTax_F_From_2;
            requirement.CRI_ProffTax_F_To_2 = requirementVM.CRI_ProffTax_F_To_2;
            requirement.CRI_ProffTax_F_Amount_2 = requirementVM.CRI_ProffTax_F_Amount_2;
            requirement.CRI_ProffTax_F_From_3 = requirementVM.CRI_ProffTax_F_From_3;
            requirement.CRI_ProffTax_F_To_3 = requirementVM.CRI_ProffTax_F_To_3;
            requirement.CRI_ProffTax_F_Amount_3 = requirementVM.CRI_ProffTax_F_Amount_3;

            return requirement;
        }        
        public static List<ClientRequirementVM> mapRequirements(List<Client_Requirements> requirements)
        {
            List<ClientRequirementVM> lst = new List<ClientRequirementVM>();
            foreach (Client_Requirements requirement in requirements)
                lst.Add(mapMe(requirement));
            return lst;
        }

        public static List<Client_Requirements> mapRequirements(List<ClientRequirementVM> requirements)
        {
            List<Client_Requirements> lst = new List<Client_Requirements>();
            foreach (ClientRequirementVM requirement in requirements)
                lst.Add(mapMeModel(requirement));
            return lst;
        }
    }
}
