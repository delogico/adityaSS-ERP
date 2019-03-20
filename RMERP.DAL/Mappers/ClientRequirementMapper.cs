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
            clientRequirementVM.CRI_BasicDA = requirement.CRI_BasicDA;
            clientRequirementVM.CRI_HRA_Fixed = requirement.CRI_HRA_Fixed;
            clientRequirementVM.DES_Title = requirement.DES_.DES_Title;
            clientRequirementVM.CRI_HRA_Percentage = requirement.CRI_HRA_Percentage;
            clientRequirementVM.CRI_Allowance_UpKeep = requirement.CRI_Allowance_UpKeep;
            clientRequirementVM.CRI_Allowance_Grade = requirement.CRI_Allowance_Grade;
            clientRequirementVM.CRI_Allowance_Conveyance = requirement.CRI_Allowance_Conveyance;
            clientRequirementVM.CRI_Allowance_Attention = requirement.CRI_Allowance_Attention;
            clientRequirementVM.CRI_PF_Percentage = requirement.CRI_PF_Percentage;
            clientRequirementVM.CRI_ESIC_Percentage = requirement.CRI_ESIC_Percentage;
            clientRequirementVM.CRI_ESIC_Area = requirement.CRI_ESIC_Area;
            clientRequirementVM.CRI_OT_Rate = requirement.CRI_OT_Rate;
            clientRequirementVM.CRI_OT_MultipleTimes = requirement.CRI_OT_MultipleTimes;
            clientRequirementVM.CRI_WageCalculationOnWeeklyOffPlus = requirement.CRI_WageCalculationOnWeeklyOffPlus;
            clientRequirementVM.CRI_Active = requirement.CRI_Active;
            clientRequirementVM.CRI_InactivatedOn = requirement.CRI_InactivatedOn;
            clientRequirementVM.CRI_RegisteredOn = requirement.CRI_RegisteredOn;
            return clientRequirementVM;
        }

        public static List<ClientRequirementVM> mapRequirements(List<Client_Requirements> requirements)
        {
            List<ClientRequirementVM> lst = new List<ClientRequirementVM>();
            foreach (Client_Requirements requirement in requirements)
                lst.Add(mapMe(requirement));
            return lst;
        }
    }
}
