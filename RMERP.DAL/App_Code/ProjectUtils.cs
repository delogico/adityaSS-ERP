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
            BASIC_WithoutShifts = 0,
            BASIC_WithShifts = 1,
            COMPLEX_WithoutShift=2
        }
        public enum AttendanceShift
        {
            //O=0,
            I=1,
            II=2,
            III=3
        }
        public static DateTime DateNow()
        {
            DateTime utcTime = DateTime.UtcNow;
            TimeZoneInfo myZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime custDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, myZone);
            return custDateTime;
        }
        public static string convertDigit(int ID)
        {
            string no = Convert.ToString(ID);
            int length = ID.ToString().Length;
            switch (length)
            {
                case 1:
                    no = ("0000" + no);
                    break;
                case 2:
                    no = ("000" + no);
                    break;
                case 3:
                    no = ("00" + no);
                    break;
                case 4:
                    no = ("0" + no);
                    break;
                default:
                    no = ("" + no);
                    break;
            }
            return no;
        }
  
    }
}
