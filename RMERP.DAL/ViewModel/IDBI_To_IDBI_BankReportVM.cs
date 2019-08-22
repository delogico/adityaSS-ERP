using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RMERP.DAL.ViewModel
{
    public class BankReportVM
    {        

        public string EMP_FirstName { get; set; }
        public string EMP_MiddleName { get; set; }
        public string EMP_SurName { get; set; }
        public string EMP_NAME
        {
            get
            {
                return string.Concat(EMP_FirstName + " " + EMP_MiddleName + " " + EMP_SurName);
            }
        }

        public string EMP_ACCOUNT_NUMBER { get; set; }
        public string EMP_CURRENCY_CODE { get; set; }
        public string EMP_SERVICE_OUTLET { get; set; }
        public string EMP_PART_TRAN_TYPE { get; set; }
        public decimal EMP_TRANSACTION_AMOUNT { get; set; }
        public string EMP_TRANSACTION_PARTICULARS { get; set; }

        public string ACCOUNT_SENDER_NUMBER { get; set; }
        public string ACCOUNT_IFSC_CODE { get; set; }
        public string ACCOUNT_RECEIVERS_NUMBER { get; set; }
        public string ACCOUNT_TYPE { get; set; }
        public string EMP_ADDRESS { get; set; }
        public string MESSAGE { get; set; }
        public string ORIGINETOR { get; set; }


        public string COMPANY_NAME { get; set; }

        public string CHEQUE_CASH { get; set; }
    }

}
