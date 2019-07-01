using System;
using System.Collections.Generic;
using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace RMERP.DAL.Mappers
{
    public class FirmMapper
    {
        public static FirmVM mapMe(Firms firm)
        {
            FirmVM firmVM = new FirmVM();
            firmVM.FRM_Id = firm.FRM_Id;
            firmVM.FRM_Name = firm.FRM_Name;
            firmVM.FRM_ShortName = firm.FRM_ShortName;
            return firmVM;
        }

        public static List<FirmVM> mapFirms(List<Firms> firms)
        {
            List<FirmVM> lst = new List<FirmVM>();
            foreach (Firms firm in firms){
                lst.Add(mapMe(firm));
            }
            return lst;
        }
    }
}
