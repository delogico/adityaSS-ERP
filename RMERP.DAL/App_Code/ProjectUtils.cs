using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMERP.DAL.App_Code
{
    public class ProjectUtils
    {
        public enum CountryRegion
        {
            International = 0,
            Domestic=1
        }
        public enum Gender
        {
            Male = 0,
            Female = 1
        }
        public enum MaritalStatus
        {
            UnMarried = 0,
            Married = 1
        }
        public enum ESIC_AREA
        {
            NEW_IMP = 0,
            OLD_IMP = 1
        }
        public enum Execl_BasicShifts
        {
            TwoRows_WithoutShifts = 0,
            TwoRow_WithShifts = 1,
            OneRow_WithoutShifts=2
        }
        public enum AttendanceShift
        {
            //O=0,
            I=1,
            II=2,
            III=3,
            G=4
        }
        public enum WageRegister_Saved
        {
            Saved = 0,
            Pending = 1
        }
       
        public static DateTime DateNow()
        {
            DateTime utcTime = DateTime.UtcNow;
            TimeZoneInfo myZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime custDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, myZone);
            return custDateTime;
        }       
  
    }
}
