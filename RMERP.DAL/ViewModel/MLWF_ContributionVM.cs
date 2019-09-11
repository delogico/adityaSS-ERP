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

        public decimal EMP_CONTR_BELOW_3K() { return this.EMP_BELOW_3K * 6; } //RS.6
        public decimal EMP_CONTR_ABOVE_3K() { return this.EMP_ABOVE_3K * 12; } //RS.12
        public decimal EMPLOYER_CONTR_BELOW_3K() { return this.EMP_BELOW_3K * 18; } //RS.18
        public decimal EMPLOYER_CONTR_ABOVE_3K() { return this.EMP_ABOVE_3K * 36; } //RS.36

    }
}
