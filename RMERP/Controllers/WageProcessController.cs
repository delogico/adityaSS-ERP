using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RMERP.DAL.ManagerClasses;
using RMERP.DAL.Models;
using RMERP.Helpers;
using RMERP.DAL.ViewModel;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace RMERP.Controllers
{
    [Authorize]
    public class WageProcessController : Controller
    {
        private readonly RMERPContext _context;
        public IConfiguration Configuration;
        WageProcessManager wpm;
        private IHostingEnvironment _hostingEnvironment;
        private static int clientID,WagID;

        public WageProcessController(RMERPContext context, IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            Configuration = configuration;
            wpm = new WageProcessManager(context);
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            DateTime nextMonth = wpm.nextWageMonth(sessionUtils.GetLoggedAdminID());
            ViewBag.month = nextMonth.ToString("MMMM", CultureInfo.CreateSpecificCulture("IN"));
            return View(wpm.getWageProcessList(sessionUtils.GetLoggedAdminID()));
        }
        public IActionResult CreateNextMonthWage()
        {
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            string rse = wpm.CreateNextMonthWage(sessionUtils.GetLoggedAdminID());
            return RedirectToAction("Index");
        }
        public IActionResult DeleteWageProcess(int WagId)
        {
            WageProcessManager wpm = new WageProcessManager(_context);
            string res = wpm.DeleteWageProcess(WagId);
            if (res != string.Empty)
            {
                TempData["message"] = "Wage Process can not Deleted";
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult nextWageMonth()
        {
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            DateTime nextMonth = wpm.nextWageMonth(sessionUtils.GetLoggedAdminID());
            return Content(nextMonth.ToString("MMMM", CultureInfo.CreateSpecificCulture("IN")));
        }

        public ActionResult WageProcessList()
        {
            SessionUtils sessionUtils = new SessionUtils(Request, Response);

            var model = wpm.getWageProcessList(sessionUtils.GetLoggedAdminID());
            return PartialView("_WageProcessList", model);
        }
        public ActionResult AttendancesStatus(int wagId)
        {
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            ClientsViewModel cvm = new ClientsViewModel();
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            cvm.Listclients = clientsManager.listClients(true, null);
            List<Clients> clientListForMonth = clientsManager.GetClientsListByMonth(DateTime.Now);
            cvm.WageID = wagId;
            return View(cvm);
        }
        [HttpGet]
        public ActionResult ViewAttendance(int wagId,int CliId)
        {
            clientID = CliId;
            WagID = wagId;
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);            
            IEnumerable <Attendance> list = wpm.GetAttendanceList(wagId, CliId);

            Clients client = clientsManager.GetClientById(CliId);
            DateTime startDate = DateTime.Now, endDate = DateTime.Now;
            ViewBag.ClientName = client.CLI_Name;
            if (client.CLI_Att_MonthReal == true)
            {
                startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                endDate = startDate.AddMonths(1).AddDays(-1);
            }
            else if (client.CLI_Att_MonthReal == false)
            {
                startDate = new DateTime(DateTime.Now.AddMonths(-1).Year, DateTime.Now.AddMonths(-1).Month, client.CLI_Att_Month_Start.Value);
                endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, client.CLI_Att_Month_End.Value); ;
            }
            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;

            //List<AttendanceListViewModel> listModel = new List<AttendanceListViewModel>();
            //List<int> lst = list.Select(m => m.EmpId).Distinct().ToList();
            //foreach (var item in lst)
            //{
            //    var tempEmployeeList = list.Where(m => m.EmpId == item).ToList();
            //    AttendanceListViewModel vm = new AttendanceListViewModel();
            //    vm.EmpName = tempEmployeeList[0].Emp.EmpFirstName;
            //    vm.EmpDesignation = tempEmployeeList[0].Emp.EmpDesignation;
            //    vm.ExtraWorkingHours = tempEmployeeList[0].AttExtraHoursWorked;
            //    vm.UserDataList = new List<UserData>();
            //    foreach (var itemEmp in tempEmployeeList)
            //    {
            //        UserData userData = new UserData();
            //        userData.AttendanceDate = itemEmp.AttDate;
            //        vm.UserDataList.Add(userData);
            //    }
            //    listModel.Add(vm);
            //}
            return View(list);
        }
        [HttpGet]
        public ActionResult EditAttendance(int empId)
        {
            AttendanceListViewModel alvm = new AttendanceListViewModel();
            alvm.attendanceViewModel = new AttendanceViewModel();        

            List<Attendance> list = wpm.GetAttendanceList(WagID, clientID, empId);          
            alvm.attendancesList = list;
            alvm.EmpID = empId;
            alvm.EmpName = list.FirstOrDefault().EMP_.EMP_FirstName;
            alvm.EmpDesignation = list.FirstOrDefault().EMP_.EMP_Designation;
            return View(alvm);
        }
        [HttpPost]
        public ActionResult EditAttendance(AttendanceListViewModel alvm)
        {
            string res = string.Empty;
            foreach(var item in alvm.attendancesList)
            {
                Attendance attendance = new Attendance();
                attendance.ATT_Id = item.ATT_Id;
                attendance.ATT_IsPresent = item.ATT_IsPresent;
                attendance.ATT_IsWeeklyOff = item.ATT_IsWeeklyOff;
                attendance.ATT_ExtraHoursWorked = item.ATT_ExtraHoursWorked;
                res=wpm.UpdateAttendance(attendance);
            }

            return RedirectToAction("ViewAttendance", new { wagId = WagID , CliId = clientID });
        }
        public ActionResult ImportWageProcessData(UploadExcelViewModel uvm)
        {
            IFormFile file = uvm.ExcelFile;
            string folderName = "RMERP_Data";
            string webRootPath = _hostingEnvironment.WebRootPath;
            string newPath = Path.Combine(webRootPath, folderName);
            StringBuilder sb = new StringBuilder();
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }
            if (file.Length > 0)
            {
                string sFileExtension = Path.GetExtension(file.FileName).ToLower();
                ISheet sheet;
                string fullPath = Path.Combine(newPath, file.FileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                    stream.Position = 0;
                    if (sFileExtension == ".xls")
                    {
                        HSSFWorkbook hssfwb = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats  
                        sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook  
                    }
                    else
                    {
                        XSSFWorkbook hssfwb = new XSSFWorkbook(stream); //This will read 2007 Excel format  
                        sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook   
                    }
                    IRow headerRow = sheet.GetRow(0); //Get Header Row
                    int cellCount = headerRow.LastCellNum;
                    sb.Append("<table class='table'><tr>");
                    for (int j = 0; j < cellCount; j++)
                    {
                        NPOI.SS.UserModel.ICell cell = headerRow.GetCell(j);
                        if (cell == null || string.IsNullOrWhiteSpace(cell.ToString())) continue;
                        sb.Append("<th>" + cell.ToString() + "</th>");
                    }
                    sb.Append("</tr>");
                    sb.AppendLine("<tr>");
                    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue;
                        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                        for (int j = row.FirstCellNum; j < cellCount; j++)
                        {
                            if (row.GetCell(j) != null)
                                sb.Append("<td>" + row.GetCell(j).ToString() + "</td>");
                        }
                        sb.AppendLine("</tr>");
                    }
                    sb.Append("</table>");
                }
            }
            return this.Content(sb.ToString());
        }

        public ActionResult EditAttendanceRecord(int attID)
        {
            AttendanceListViewModel alvm = new AttendanceListViewModel();
            alvm.attendanceViewModel = new AttendanceViewModel();
           
            Attendance att = new Attendance();
            if (attID > 0)
            {
                att = wpm.GetAttendanceById(attID);
                alvm.attendanceViewModel.ATT_Id = attID;
                alvm.attendanceViewModel.CRI_Id = att.CRI_Id;
                alvm.attendanceViewModel.CLI_Id = att.CLI_Id;
                alvm.attendanceViewModel.EMP_Id = att.EMP_Id;
                alvm.attendanceViewModel.WAG_Id = att.WAG_Id;
                alvm.attendanceViewModel.ATT_ImportedOn = att.ATT_ImportedOn;
                alvm.attendanceViewModel.ATT_IsEarnLeave = att.ATT_IsEarnLeave;
                alvm.attendanceViewModel.ATT_IsPaidHoliday = att.ATT_IsPaidHoliday;
                alvm.attendanceViewModel.ATT_IsPresent = att.ATT_IsPresent;
                alvm.attendanceViewModel.ATT_IsWeeklyOff = att.ATT_IsWeeklyOff;
                alvm.attendanceViewModel.ATT_Shift = att.ATT_Shift;
                alvm.attendanceViewModel.ATT_Date = att.ATT_Date;        
                alvm.attendanceViewModel.ADM_Id_ImportedBy = att.ADM_Id_ImportedBy;
                alvm.attendanceViewModel.ATT_ExtraHoursWorked = att.ATT_ExtraHoursWorked;
            }
            return View(alvm);
        }
        [HttpPost]
        public ActionResult EditAttendanceRecord(AttendanceListViewModel alvm)
        {
            string res = string.Empty;
            Attendance atta = new Attendance();

            if (ModelState.IsValid)
            {
                atta.ATT_Id = alvm.attendanceViewModel.ATT_Id;
                atta.ATT_ImportedOn = alvm.attendanceViewModel.ATT_ImportedOn;
                atta.ATT_IsEarnLeave = alvm.attendanceViewModel.ATT_IsEarnLeave;
                atta.ATT_IsPaidHoliday = alvm.attendanceViewModel.ATT_IsPaidHoliday;
                atta.ATT_IsPresent = alvm.attendanceViewModel.ATT_IsPresent;
                atta.ATT_IsWeeklyOff = alvm.attendanceViewModel.ATT_IsWeeklyOff;
                atta.ATT_Date = alvm.attendanceViewModel.ATT_Date;
                atta.ADM_Id_ImportedBy = alvm.attendanceViewModel.ADM_Id_ImportedBy;
                atta.ATT_Shift = alvm.attendanceViewModel.ATT_Shift;
                atta.CLI_Id = alvm.attendanceViewModel.CLI_Id;
                atta.EMP_Id = alvm.attendanceViewModel.EMP_Id;
                atta.CRI_Id = alvm.attendanceViewModel.CRI_Id;
                atta.WAG_Id = alvm.attendanceViewModel.WAG_Id;
                atta.ATT_ExtraHoursWorked = alvm.attendanceViewModel.ATT_ExtraHoursWorked;

                res = wpm.UpdateAttendance(atta);
            }
            

            return RedirectToAction("ViewAttendance", new { wagId = WagID, CliId = clientID });
        }
    }
}