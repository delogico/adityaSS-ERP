using System;
using System.Collections.Generic;
using System.Text;

namespace RMERP.DAL.ViewModel
{
    public class MLWF_ContributionVM
    {
        public int CLI_Id { get; set; }
        public string CLI_Name { get; set; }
        public int EMP_BELOW_3K  { get; set; }
        public int EMP_ABOVE_3K { get; set; }


        public decimal CRI_MLWF_Employer_GThen { get; set; }
        public decimal CRI_MLWF_Employer_LThen { get; set; }
        public decimal CRI_MLWF_Employee_GThen { get; set; }
        public decimal CRI_MLWF_Employee_LThen { get; set; }


        //public decimal EMP_CONTR_BELOW_3K() { return this.EMP_BELOW_3K * CRI_MLWF_Employee_LThen; } //RS.6
        //public decimal EMP_CONTR_ABOVE_3K() { return this.EMP_ABOVE_3K * CRI_MLWF_Employee_GThen; } //RS.12
        //public decimal EMPLOYER_CONTR_BELOW_3K() { return this.EMP_BELOW_3K * CRI_MLWF_Employer_LThen; } //RS.18
        //public decimal EMPLOYER_CONTR_ABOVE_3K() { return this.EMP_ABOVE_3K * CRI_MLWF_Employer_GThen; } //RS.36


        public decimal EMP_DEDUCTION_BELOW { get; set; }
        public decimal EMP_DEDUCTION_ABOVE { get; set; }
        public decimal EMPLOYER_CONTR_BELOW{ get; set; }
        public decimal EMPLOYER_CONTR_ABOVE { get; set; }


        public decimal MLWF_Employee_Base { get; set; }
        public decimal MLWF_Employer_Base { get; set; }


    }
}
