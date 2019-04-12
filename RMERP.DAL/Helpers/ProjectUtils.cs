using System;
using System.IO;
using System.Text;

namespace RMERP.DAL.Helpers
{
    public static class ProjectUtils
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

        public enum ATTENDANCE_EXCEL_TEMPLATE
        {
            BASIC_WithoutShifts = 0,
            BASIC_WithShifts = 1,
            COMPLEX_WithoutShift = 2
        }

        public static DateTime DateNow()
        {
            DateTime utcTime = DateTime.UtcNow;
            TimeZoneInfo myZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime custDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, myZone);
            return custDateTime;
        }        

        public static string GetTempFolderPath(string webRootPath)
        {
            string folderName = "RMERP_Data";
            string newPath = Path.Combine(webRootPath, folderName);
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }
            return newPath;
        }

        public static string GetTempFileName()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmss");
        }
        

    }
}
