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
        public static AttendanceVM mapMe(Attendance attendance)
        {
            AttendanceVM attendanceVM = new AttendanceVM();
            attendanceVM.WAG_Id = attendance.WAG_Id;
            attendanceVM.ATT_Id = attendance.ATT_Id;
            attendanceVM.EMP_Id = attendance.EMP_Id;
            attendanceVM.DES_Id = attendance.DES_Id;
            attendanceVM.CLI_Id = attendance.CLI_Id;
            attendanceVM.ATT_Date = attendance.ATT_Date;
            attendanceVM.ATT_IsPresent = attendance.ATT_IsPresent;
            attendanceVM.ATT_IsHalfday = attendance.ATT_IsHalfday;
            attendanceVM.ATT_IsPublicHoliday = attendance.ATT_IsPublicHoliday;
            attendanceVM.ATT_Shift = attendance.ATT_Shift;
            attendanceVM.ATT_IsWeeklyOff = attendance.ATT_IsWeeklyOff;
            attendanceVM.ATT_IsEarnLeave = attendance.ATT_IsEarnLeave;
            attendanceVM.ATT_ExtraHoursWorked = attendance.ATT_ExtraHoursWorked;
            attendanceVM.ATT_IsHoliday = attendance.ATT_IsHoliday;
            attendanceVM.ATT_EarnedExtraDay = attendance.ATT_EarnedExtraDay;
            attendanceVM.ATT_IsCompensatoryOff = attendance.ATT_IsCompensatoryOff;
            attendanceVM.ATT_IsPaidLeave = attendance.ATT_IsPaidLeave;
            attendanceVM.ATT_NightShift = attendance.ATT_NightShift;
            attendanceVM.ATT_Orignal_Row1 = attendance.ATT_Orignal_Row1;
            attendanceVM.ATT_Orignal_Row2 = attendance.ATT_Orignal_Row2;

            if (attendance.EMP_ != null)
                attendanceVM.employee = EmployeesMapper.MapMe(attendance.EMP_);
            if (attendance.WAG_ != null)
                attendanceVM.wage_Process = WageProcessMapper.mapMe(attendance.WAG_);
            if (attendance.DES_ != null)
                attendanceVM.designation = attendance.DES_;
            return attendanceVM;
        }

        public static List<AttendanceVM> mapAttendances(List<Attendance> attendances)
        {
            List<AttendanceVM> lst = new List<AttendanceVM>();
            foreach (Attendance attendance in attendances){
                lst.Add(mapMe(attendance));
            }
            return lst;
        }
    }
}
