using System;
using System.Collections.Generic;
using System.Text;

namespace RMERP.DAL.ViewModel
{
    public class NEFT_BankReportVM
    {
        public int CLI_Id { get; set; }
        public string CLI_Name { get; set; }
        public List<NEFTBank_EMP_ReportVM> NEFTBank_EMP_ReportVMs { get; set; }
    }
    public class NEFTBank_EMP_ReportVM
    {
        
        public string EMP_FirstName { get; set; }       
        public string EMP_MiddleName { get; set; }       
        public string EMP_SurName { get; set; }
        public string EMP_FullName
        {
            get
            {
                return string.Concat(EMP_FirstName + " " + EMP_MiddleName + " " + EMP_SurName);
            }
        }
        public string EMP_Account_Number { get; set; }
        public string CURRENCY_CODE { get; set; }
        public string SERVICE_OUTLET { get; set; }
        public string PART_TRAN_TYPE { get; set; }
        public decimal TRANSACTION_AMOUNT { get; set; }
        public string TRANSACTION_PARTICULARS { get; set; }
    }
}
