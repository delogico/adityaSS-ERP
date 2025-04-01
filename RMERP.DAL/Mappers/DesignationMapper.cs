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
        public static DesignationVM mapMe(Designation designations)
        {
            DesignationVM designationVM = new DesignationVM();
            designationVM.DES_Id = designations.DES_Id;
            designationVM.DES_Title = designations.DES_Title;
            designationVM.DES_Exclude_LWF = designations.DES_Exclude_LWF;
            return designationVM;
        }
        public static Designation mapMeModel(DesignationVM designations)
        {
            Designation designation = new Designation();
            designation.DES_Id = designations.DES_Id;
            designation.DES_Title = designations.DES_Title;
            designation.DES_Exclude_LWF = designations.DES_Exclude_LWF;
            return designation;
        }
        public static List<DesignationVM> mapDesignations(List<Designation> designations)
        {
            List<DesignationVM> lst = new List<DesignationVM>();
            foreach (Designation designation in designations)
            {
                lst.Add(mapMe(designation));
            }
            return lst;
        }
    }
}
