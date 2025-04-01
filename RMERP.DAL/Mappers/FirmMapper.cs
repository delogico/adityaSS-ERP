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
        public static FirmVM mapMe(Firm firm)
        {
            FirmVM firmVM = new FirmVM();
            firmVM.FRM_Id = firm.FRM_Id;
            firmVM.FRM_Name = firm.FRM_Name;
            firmVM.FRM_ShortName = firm.FRM_ShortName;
            firmVM.FRM_InvoicingName = firm.FRM_InvoicingName;
            firmVM.FRM_Email = firm.FRM_Email;
            firmVM.FRM_Address1 = firm.FRM_Address1;
            firmVM.FRM_Address2 = firm.FRM_Address2;
            firmVM.FRM_GST_No = firm.FRM_GST_No;
            firmVM.FRM_BankName = firm.FRM_BankName;
            firmVM.FRM_AccountNumber = firm.FRM_AccountNumber;
            firmVM.FRM_IFSC_Code = firm.FRM_IFSC_Code;
            firmVM.STA_Id = firm.STA_Id;
            firmVM.STA_ = firm.STA;
            return firmVM;
        }
        public static Firm mapMeModel(FirmVM firmVM)
        {
            Firm firm = new Firm();
            firm.FRM_Id = firmVM.FRM_Id;
            firm.FRM_Name = firmVM.FRM_Name;
            firm.FRM_ShortName = firmVM.FRM_ShortName;
            firm.FRM_InvoicingName = firmVM.FRM_InvoicingName;
            firm.FRM_Email = firmVM.FRM_Email;
            firm.FRM_Address1 = firmVM.FRM_Address1;
            firm.FRM_Address2 = firmVM.FRM_Address2;
            firm.FRM_GST_No = firmVM.FRM_GST_No;
            firm.FRM_BankName = firmVM.FRM_BankName;
            firm.FRM_AccountNumber = firmVM.FRM_AccountNumber;
            firm.FRM_IFSC_Code = firmVM.FRM_IFSC_Code;
            firm.STA_Id = firmVM.STA_Id;
            firm.STA = firmVM.STA_;
            return firm;
        }


        public static List<FirmVM> mapFirms(List<Firm> firms)
        {
            List<FirmVM> lst = new List<FirmVM>();
            foreach (Firm firm in firms){
                lst.Add(mapMe(firm));
            }
            return lst;
        }
    }
}
