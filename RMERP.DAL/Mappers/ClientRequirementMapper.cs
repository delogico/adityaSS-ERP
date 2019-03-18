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
            ClientRequirementVM clientRequirementVMVM = new ClientRequirementVM();
            clientRequirementVMVM.CRI_Id = requirement.CRI_Id;
            clientRequirementVMVM.DES_Id = requirement.DES_Id;
            clientRequirementVMVM.CRI_Basic = requirement.CRI_Basic;
            clientRequirementVMVM.CRI_BasicDA = requirement.CRI_BasicDA;
            clientRequirementVMVM.CRI_HRA_Fixed = requirement.CRI_HRA_Fixed;
            clientRequirementVMVM.DES_Title = requirement.DES_.DES_Title;
            // to be completed
            return clientRequirementVMVM;
        }
    }
}
