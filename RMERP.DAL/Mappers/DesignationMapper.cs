using System;
using System.Collections.Generic;
using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace RMERP.DAL.Mappers
{
    public class DesignationMapper
    {
        public static DesignationVM mapMe(Designations designations)
        {
            DesignationVM designationVM = new DesignationVM();
            designationVM.DES_Id = designations.DES_Id;
            designationVM.DES_Title = designations.DES_Title;
            designationVM.DES_Exclude_LWF = designations.DES_Exclude_LWF;
            return designationVM;
        }
        public static Designations mapMeModel(DesignationVM designations)
        {
            Designations designation = new Designations();
            designation.DES_Id = designations.DES_Id;
            designation.DES_Title = designations.DES_Title;
            designation.DES_Exclude_LWF = designations.DES_Exclude_LWF;
            return designation;
        }
        public static List<DesignationVM> mapDesignations(List<Designations> designations)
        {
            List<DesignationVM> lst = new List<DesignationVM>();
            foreach (Designations designation in designations)
            {
                lst.Add(mapMe(designation));
            }
            return lst;
        }
    }
}
