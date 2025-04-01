using System;
using System.Collections.Generic;
using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace RMERP.DAL.Mappers
{
    public class AttendanceMapper
    {
        #region ADDED ON 19/12/2024

        public static AttendanceSummaryVM MapMe(Attendance_Summary attendance)
        {
            AttendanceSummaryVM attendanceVM = new()
            {
                WAG_Id = attendance.WAG_Id,
                ATS_Id = attendance.ATS_Id,
                EMP_Id = attendance.EMP_Id,
                CLI_Id = attendance.CLI_Id,
                ATS_ImportedOn = attendance.ATS_ImportedOn,
                ATS_PresentDays = attendance.ATS_PresentDays,
                ATS_WeekOff = attendance.ATS_WeekOff,
                ATS_PublicHolidays = attendance.ATS_PublicHolidays,
                ATS_EarnLeaves = attendance.ATS_EarnLeaves,
                ATS_NightShifts = attendance.ATS_NightShifts,
                ATS_ExtraHours = attendance.ATS_ExtraHours
            };

            if (attendance.EMP != null)
                attendanceVM.Employee = EmployeesMapper.MapMe(attendance.EMP);
            if (attendance.WAG != null)
                attendanceVM.Wage_Process = WageProcessMapper.MapMe(attendance.WAG);
            return attendanceVM;
        }

        public static List<AttendanceSummaryVM> MapAttendances(IQueryable<Attendance_Summary> attendances)
        {
            List<AttendanceSummaryVM> lst = [];
            foreach (Attendance_Summary attendance in attendances)
                lst.Add(MapMe(attendance));
            return lst;
        }

        #endregion

        #region OLD
        public static AttendanceVM mapMe(Attendance attendance)
        {
            AttendanceVM attendanceVM = new AttendanceVM();
            attendanceVM.WAG_Id = attendance.WAG_Id;
            attendanceVM.ATT_Id = attendance.ATT_Id;
            attendanceVM.EMP_Id = attendance.EMP_Id;
            attendanceVM.DES_Id = attendance.DES_Id;
            attendanceVM.CLI_Id = attendance.CLI_Id;
            attendanceVM.ATT_Date = new DateTime(attendance.ATT_Date.Year, attendance.ATT_Date.Month, attendance.ATT_Date.Day);
            attendanceVM.ATT_IsPresent = attendance.ATT_IsPresent;
            attendanceVM.ATT_IsHalfday = attendance.ATT_IsHalfday;
            attendanceVM.ATT_IsPublicHoliday = attendance.ATT_IsPublicHoliday;
            attendanceVM.ATT_Shift = attendance.ATT_Shift;
            attendanceVM.ATT_IsWeeklyOff = attendance.ATT_IsWeeklyOff;
            attendanceVM.ATT_IsEarnLeave = attendance.ATT_IsEarnLeave;
            attendanceVM.ATT_ExtraHoursWorked = attendance.ATT_ExtraHoursWorked;
            //attendanceVM.ATT_IsHoliday = attendance.ATT_IsHoliday;
            //attendanceVM.ATT_EarnedExtraDay = attendance.ATT_EarnedExtraDay;
            //attendanceVM.ATT_IsCompensatoryOff = attendance.ATT_IsCompensatoryOff;
            //attendanceVM.ATT_IsPaidLeave = attendance.ATT_IsPaidLeave;
            attendanceVM.ATT_NightShift = attendance.ATT_NightShift;
            attendanceVM.ATT_Orignal_Row1 = attendance.ATT_Orignal_Row1;
            attendanceVM.ATT_Orignal_Row2 = attendance.ATT_Orignal_Row2;

            if (attendance.EMP != null)
                attendanceVM.employee = EmployeesMapper.MapMe(attendance.EMP);
            if (attendance.WAG != null)
                attendanceVM.wage_Process = WageProcessMapper.MapMe(attendance.WAG);
            if (attendance.DES != null)
                attendanceVM.designation = attendance.DES;
            return attendanceVM;
        }

        public static List<AttendanceVM> mapAttendances(List<Attendance> attendances)
        {
            List<AttendanceVM> lst = new List<AttendanceVM>();
            foreach (Attendance attendance in attendances)
            {
                lst.Add(mapMe(attendance));
            }
            return lst;
        }
        #endregion
    }
}
