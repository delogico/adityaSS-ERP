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
            clientRequirementVM.CRI_OT_Rate = requirement.CRI_OT_Rate;
            clientRequirementVM.CRI_OT_MultipleTimes = requirement.CRI_OT_MultipleTimes;
            clientRequirementVM.CRI_WageCalculationOnWeeklyOffPlus = requirement.CRI_WageCalculationOnWeeklyOffPlus;
            clientRequirementVM.CRI_Active = requirement.CRI_Active;
            clientRequirementVM.CRI_InactivatedOn = requirement.CRI_InactivatedOn;
            clientRequirementVM.CRI_RegisteredOn = requirement.CRI_RegisteredOn;
            clientRequirementVM.CRI_PF_Formula = requirement.CRI_PF_Formula;
            clientRequirementVM.CRI_ESIC_Formula = requirement.CRI_ESIC_Formula;
            if (requirement.CLI_ != null)
                clientRequirementVM.CLI_Name = requirement.CLI_.CLI_Name;

            if (requirement.CRI_HRA_Fixed != null)
                clientRequirementVM.HRAselection = true;
            else if(requirement.CRI_HRA_Percentage !=null)
                clientRequirementVM.HRAselection = false;
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
            requirement.CRI_OT_Rate = requirementVM.CRI_OT_Rate;
            requirement.CRI_OT_MultipleTimes = requirementVM.CRI_OT_MultipleTimes;
            requirement.CRI_WageCalculationOnWeeklyOffPlus = requirementVM.CRI_WageCalculationOnWeeklyOffPlus;
            requirement.CRI_Active = requirementVM.CRI_Active;
            requirement.CRI_InactivatedOn = requirementVM.CRI_InactivatedOn;
            requirement.CRI_RegisteredOn = requirementVM.CRI_RegisteredOn;
            requirement.CRI_PF_Formula = requirementVM.CRI_PF_Formula;
            requirement.CRI_ESIC_Formula = requirementVM.CRI_ESIC_Formula;
            if (requirementVM.HRAselection ==true)
                requirement.CRI_HRA_Fixed = requirementVM.CRI_HRA_Fixed;
            else if(requirementVM.HRAselection == false)
                requirement.CRI_HRA_Percentage = requirementVM.CRI_HRA_Percentage;

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
