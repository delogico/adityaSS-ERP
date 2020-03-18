using System;
using System.Collections.Generic;
using System.Text;

namespace RMERP.DAL.ViewModel
{
    public class PayTaxReportVM
    {
        public int CLI_Id { get; set; }
        public string CLI_Name { get; set; }               
        public int UpTo7500 { get; set; }
        public int UpTo7500Ladies { get; set; }
        public int UpTo10000 { get; set; }
        public int UpTo10000Ladies { get; set; }
        public int Above10000 { get; set; }
        public int Above10000Ladies { get; set; }
        public int STREGNTH() {
            return (this.UpTo7500 +this.UpTo7500Ladies+this.UpTo10000+this.UpTo10000Ladies+this.Above10000+this.Above10000Ladies);
        }
        public decimal AMOUNT { get; set; }
        //public decimal AMOUNT() {            
        //    return ((this.UpTo10000 * 175) + (this.Above10000 *200)+ (this.Above10000Ladies * 200));
        //}
    }
    
}
