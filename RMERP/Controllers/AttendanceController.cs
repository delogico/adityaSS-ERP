using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using RMERP.DAL.Helpers;
using RMERP.DAL.ManagerClasses;
using RMERP.DAL.Mappers;
using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;
using RMERP.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RMERP.Controllers.WageRegisterController;
using static RMERP.DAL.Helpers.ProjectUtils;

namespace RMERP.Controllers
{
    [Authorize]
    public class AttendanceController : Controller
    {
        private readonly RMERPContext _context;
        public IConfiguration _configuration;
        private IWebHostEnvironment _hostingEnvironment;

        public AttendanceController(RMERPContext context, IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        #region ------------------------------------    NEW FLOW | 06-01-2025

        //public ActionResult NewWageAttendanceList(int WAG_Id)
        //{
        //    ClientsManager clientsManager = new(_context, _configuration);
        //    AttendanceSummaryManager AttsummaryManager = new(_context);
        //    WageProcessManager wageManager = new(_context);
        //    Wage_Process wage = wageManager.getWageProcessById(WAG_Id);
        //    List<Client> clients = clientsManager.GetActiveClientsOfMonthByFirmId(ProjectUtils.DateToDateTime(wage.WAG_Month), wage.FRM_Id);
        //    WageProcessClientAttendancePageVM pageVM = new()
        //    {
        //        wageProcess = WageProcessMapper.MapMe2(wage),
        //        lstClient = WageProcessMapper.MapClientToAttendanceWages(clients, AttsummaryManager.GetAttendance_Wage(WAG_Id))
        //    };
        //    return View(pageVM);
        //}

        //[HttpGet]
        //public ActionResult NewViewAttendance(int WAG_Id, int CLI_Id)
        //{
        //    AttendanceSummaryManager AttsummaryManager = new(_context);
        //    ClientsManager clientsManager = new(_context, _configuration);
        //    WageProcessManager wagManager = new(_context);
        //    Wage_Process wage = wagManager.getWageProcessById(WAG_Id);
        //    ViewBag.WAG_Month = wage.WAG_Month.ToString("MMMM") + "-" + wage.WAG_Month.ToString("yyyy");
        //    Client client = clientsManager.GetClientById(CLI_Id);
        //    ViewBag.ClientName = client.CLI_Name;
        //    ViewBag.Wage_Process = wage;
        //    ViewBag.CLI_Id = client.CLI_Id;
        //    ViewBag.CLI_Total_WorkingDays = client.CLI_Total_WorkingDays;

        //    DateTime WAG_Month = ProjectUtils.DateToDateTime(wage.WAG_Month);
        //    Attendance_Parameter attendance_Parameter = clientsManager.GetAttendanceParameterByMonth(client.CLI_Id, WAG_Month);
        //    DateTime[] arrDate = DateHelper.GetStartEndDatePeriodForAttendance(client, attendance_Parameter, WAG_Month);
        //    ViewBag.startDate = arrDate[0];
        //    ViewBag.endDate = arrDate[1];
        //    List<AttendanceSummaryVM> list = AttendanceMapper.MapAttendances(AttsummaryManager.GetAttendance_Wage_Client(WAG_Id, CLI_Id));
        //    return View(new Tuple<List<AttendanceSummaryVM>, Client>(list, client));
        //}

        //public ActionResult NewUploadExcel(int WAG_Id, int CLI_Id)
        //{
        //    ClientsManager clientManager = new ClientsManager(_context, _configuration);
        //    WageProcessManager wageManager = new WageProcessManager(_context);
        //    Wage_Process wageProcess = wageManager.getWageProcessById(WAG_Id);
        //    UploadExcelViewModel upvm = new UploadExcelViewModel();
        //    upvm.WageProcessVM = WageProcessMapper.MapMe(wageProcess);
        //    upvm.Client = clientManager.GetClientById(CLI_Id);
        //    ViewBag.templateList = Enum.GetValues(typeof(ATTENDANCE_EXCEL_TEMPLATE));
        //    return View(upvm);
        //}

        //[HttpPost]
        //public ActionResult NewVerifyTemplate(UploadExcelVM uvm)
        //{
        //    ATT_ExcelVM excelVM = new();
        //    IFormFile file = uvm.ExcelFile;
        //    if (file != null)
        //    {
        //        if (file.Length > 0)
        //        {
        //            WageProcessManager wageManager = new(_context);
        //            ClientsManager clientsManager = new(_context, _configuration);

        //            Wage_Process wageProcess = wageManager.GetWageProcessByWAG_Id(uvm.WageProcessVM.WAG_Id);
        //            DateTime WAG_Month = ProjectUtils.DateToDateTime(wageProcess.WAG_Month);

        //            Client client = clientsManager.GetClientById(uvm.Client.CLI_Id);
        //            string sFileExtension = Path.GetExtension(file.FileName).ToLower();
        //            ISheet sheet;
        //            string fullPath = Path.Combine(GetTempFolderPath(_hostingEnvironment.WebRootPath, WAG_Month, client.CLI_Id), ProjectUtils.GetTempFileName() + sFileExtension);
        //            using (var stream = new FileStream(fullPath, FileMode.Create))
        //            {
        //                file.CopyTo(stream);
        //                stream.Position = 0;
        //                if (sFileExtension == ".xls")
        //                {
        //                    HSSFWorkbook hssfwb = new(stream);  //THIS WILL READ THE EXCEL 97-2000 FORMATS
        //                    sheet = hssfwb.GetSheetAt(0);       //GET FIRST SHEET FROM WORKBOOK
        //                }
        //                else
        //                {
        //                    XSSFWorkbook hssfwb = new(stream);  //THIS WILL READ 2007 EXCEL FORMAT
        //                    sheet = hssfwb.GetSheetAt(0);       //GET FIRST SHEET FROM WORKBOOK
        //                }

        //                Attendance_Parameter attendance_Parameter = clientsManager.GetAttendanceParameterByMonth(client.CLI_Id, WAG_Month);
        //                excelVM = GetAttendance(sheet, wageProcess, client, attendance_Parameter);
        //                excelVM.FRM_Id = uvm.WageProcessVM.FRM_Id;
        //                excelVM.WAG_Id = uvm.WageProcessVM.WAG_Id;
        //                excelVM.CLI_Id = uvm.Client.CLI_Id;
        //            }
        //            excelVM.FileName = fullPath;
        //            if (excelVM.ExcelRows != null && excelVM.ExcelRows.Count > 0)
        //            {
        //                excelVM.TotalEmployees = excelVM.ExcelRows.Count;

        //                #region CHECK EXTRA EMPLOYEES IN EXCEL

        //                List<int> Valid_EMPids = [];
        //                IEnumerable<Clients_Employee> AssignedEmployees = clientsManager.listActiveClientsEmployees(client.CLI_Id, WAG_Month);
        //                List<Employee> NotExistsInClient = [];
        //                foreach (ExcelRowVM row in excelVM.ExcelRows)
        //                {
        //                    if (!AssignedEmployees.Where(m => m.EMP_Id.Equals(Convert.ToInt32(row.EMP_Id))).Any())
        //                        NotExistsInClient.Add(new Employee() { EMP_Id = Convert.ToInt32(row.EMP_Id), EMP_FirstName = row.EMP_Name.Trim() });
        //                    else
        //                        Valid_EMPids.Add(Convert.ToInt32(row.EMP_Id));
        //                }

        //                #endregion

        //                #region CHECK REMAINING EMPLOYEES IN EXCEL
        //                EmployeeManager empManager = new(_context);
        //                List<Employee> RemainingEmployees = [];
        //                foreach (int EMP_Id in AssignedEmployees.Where(m => !Valid_EMPids.Contains(m.EMP_Id)).Select(i => i.EMP_Id).ToArray())
        //                    RemainingEmployees.Add(empManager.GetEmployeeById(Convert.ToInt32(EMP_Id)));

        //                #endregion

        //                excelVM.NotExistsInClient = NotExistsInClient;
        //                excelVM.RemainingEmployees = RemainingEmployees;
        //                excelVM.IsEnableExport = NotExistsInClient.Count == 0 && RemainingEmployees.Count == 0;
        //            }
        //        }
        //    }
        //    return View(excelVM);
        //}

        //[HttpPost]
        //public ActionResult NewImportExcel(IFormCollection frm)
        //{
        //    int WAG_Id = Convert.ToInt32(frm["WAG_Id"]);
        //    int CLI_Id = Convert.ToInt32(frm["CLI_Id"]);
        //    AttendanceSummaryManager AttsummaryManager = new(_context);
        //    if (!AttsummaryManager.IsAlreadyUploaded(WAG_Id, CLI_Id))
        //    {
        //        try
        //        {
        //            string strFilePath = frm["fileName"];
        //            ClientsManager clientManager = new(_context, _configuration);
        //            WageProcessManager wageManager = new(_context);
        //            Wage_Process wageProcess = wageManager.getWageProcessById(WAG_Id);
        //            Client client = clientManager.GetClientById(CLI_Id);
        //            ISheet sheet;
        //            using (var stream = new FileStream(strFilePath, FileMode.Open))
        //            {
        //                string sFileExtension = Path.GetExtension(strFilePath).ToLower();
        //                stream.Position = 0;
        //                if (sFileExtension == ".xls")
        //                {
        //                    HSSFWorkbook hssfwb = new(stream);  //THIS WILL READ THE EXCEL 97-2000 FORMATS
        //                    sheet = hssfwb.GetSheetAt(0);       //GET FIRST SHEET FROM WORKBOOK
        //                }
        //                else
        //                {
        //                    XSSFWorkbook hssfwb = new(stream);  //THIS WILL READ 2007 EXCEL FORMAT
        //                    sheet = hssfwb.GetSheetAt(0);       //GET FIRST SHEET FROM WORKBOOK
        //                }

        //                Attendance_Parameter attendance_Parameter = clientManager.GetAttendanceParameterByMonth(client.CLI_Id, ProjectUtils.DateToDateTime(wageProcess.WAG_Month));
        //                SaveAttendance(sheet, wageProcess, client.CLI_Id);
        //                new FileInfo(strFilePath).Delete();
        //            }
        //        }
        //        catch (Exception)
        //        {
        //            TempData["err"] = "Try Again";
        //        }
        //    }
        //    return RedirectToAction("NewWageAttendanceList", new { WAG_Id = WAG_Id });
        //}

        //public ActionResult DeleteWageClientAttendances(int WAG_Id, int CLI_Id)
        //{
        //    AttendanceSummaryManager AttSummaryManager = new(_context);
        //    AttSummaryManager.DeleteAllAttendanceOfWageClient(WAG_Id, CLI_Id);
        //    return RedirectToAction("NewWageAttendanceList", new { WAG_Id = WAG_Id });
        //}

        //[NonAction]
        //private ATT_ExcelVM GetAttendance(ISheet sheet, Wage_Process wageProcess, Client client, Attendance_Parameter attendance_Parameter)
        //{
        //    ATT_ExcelVM excelVM = new();
        //    List<Attendance_Summary> attandanceList = [];
        //    List<ExcelRowVM> rows = [];
        //    try
        //    {
        //        DateTime[] Period = DateHelper.GetStartEndDatePeriodForAttendance(client, attendance_Parameter, ProjectUtils.DateToDateTime(wageProcess.WAG_Month));
        //        int TotEmp = 0;
        //        for (int i = (sheet.FirstRowNum + 3); i <= sheet.LastRowNum; i++)
        //        {
        //            IRow row = sheet.GetRow(i);
        //            if (row == null) continue;

        //            Attendance_Summary _Summary = new()
        //            {
        //                WAG_Id = wageProcess.WAG_Id,
        //                EMP_Id = Convert.ToInt32(row.GetCell(1).ToString()),
        //                CLI_Id = client.CLI_Id,
        //                ATS_PresentDays = Convert.ToDouble(row.GetCell(4).ToString()),
        //                ATS_WeekOff = Convert.ToDouble(row.GetCell(5).ToString()),
        //                ATS_PublicHolidays = Convert.ToDouble(row.GetCell(6).ToString()),
        //                ATS_EarnLeaves = Convert.ToDouble(row.GetCell(7).ToString()),
        //                ATS_NightShifts = Convert.ToDouble(row.GetCell(8).ToString()),
        //                ATS_ExtraHours = Convert.ToDouble(row.GetCell(9).ToString())
        //            };

        //            ExcelRowVM excelRow = new()
        //            {
        //                EMP_Id = row.GetCell(1).ToString(),
        //                EMP_Name = row.GetCell(3).ToString(),
        //                Designation = row.GetCell(2).ToString(),
        //                TotalPresenceDays = Convert.ToDouble(row.GetCell(4).ToString()),
        //                TotalWeeklyOff = Convert.ToInt32(row.GetCell(5).ToString()),
        //                TotalHolidays = Convert.ToInt32(row.GetCell(6).ToString()),
        //                TotalEarnLeave = Convert.ToDouble(row.GetCell(7).ToString()),
        //                TotalNightshift = Convert.ToDouble(row.GetCell(8).ToString()),
        //                TotalExtraHours = Convert.ToInt32(row.GetCell(9).ToString())
        //            };

        //            rows.Add(excelRow);
        //            attandanceList.Add(_Summary);
        //            excelVM.TotalEmployees++;
        //        }
        //        excelVM.AttendanceSummary = attandanceList;
        //        excelVM.ExcelRows = rows;
        //        excelVM.TotalEmployees = TotEmp;
        //        excelVM.StartDate = Period[0];
        //        excelVM.EndDate = Period[1];
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["error"] = ex.Message;
        //    }
        //    return excelVM;
        //}

        //[NonAction]
        //private void SaveAttendance(ISheet sheet, Wage_Process wage_Process, int CLI_Id)
        //{
        //    AttendanceSummaryManager summaryManager = new(_context);
        //    DesignationManager designationManager = new(_context);
        //    SessionUtils sessionUtils = new(Request, Response);
        //    for (int i = (sheet.FirstRowNum + 3); i <= sheet.LastRowNum; i++)
        //    {
        //        IRow row = sheet.GetRow(i);
        //        if (row == null) continue;
        //        Attendance_Summary _Summary = new()
        //        {
        //            WAG_Id = wage_Process.WAG_Id,
        //            EMP_Id = Convert.ToInt32(row.GetCell(1).ToString()),
        //            CLI_Id = CLI_Id,
        //            DES_Id = designationManager.getDesignationIdForAttandance(CLI_Id, Convert.ToInt32(row.GetCell(1).ToString()), ProjectUtils.DateToDateTime(wage_Process.WAG_Month)),
        //            ATS_PresentDays = Convert.ToDouble(row.GetCell(4).ToString()),
        //            ATS_WeekOff = Convert.ToDouble(row.GetCell(5).ToString()),
        //            ATS_PublicHolidays = Convert.ToDouble(row.GetCell(6).ToString()),
        //            ATS_EarnLeaves = Convert.ToDouble(row.GetCell(7).ToString()),
        //            ATS_NightShifts = Convert.ToDouble(row.GetCell(8).ToString()),
        //            ATS_ExtraHours = Convert.ToDouble(row.GetCell(9).ToString()),
        //            ATS_ImportedOn = DateTime.Now,
        //            ADM_Id_ImportedBy = sessionUtils.GetLoggedAdminID(),
        //            ATS_TemplateReference = "ONEROW_Totals_WithoutShifts"
        //        };
        //        summaryManager.Save(_Summary);
        //    }
        //}

        #endregion --------------------------------------------------------------------------------------------------------------------------------------------

        public IActionResult Index()
        {
            return View();
        }

        public ActionResult WageAttendanceList(int WAG_Id)
        {
            //ClientsManager clientsManager = new(_context, _configuration);
            //AttendanceManager attManager = new(_context);
            //WageProcessManager wageManager = new(_context);
            //Wage_Process wage = wageManager.getWageProcessById(WAG_Id);
            //List<Client> clients = clientsManager.GetActiveClientsOfMonthByFirmId(ProjectUtils.DateToDateTime(wage.WAG_Month), wage.FRM_Id);
            //WageProcessClientAttendancePageVM pageVM = new WageProcessClientAttendancePageVM();
            //pageVM.wageProcess = WageProcessMapper.MapMe(wage);
            //pageVM.lstClient = WageProcessMapper.MapClientToAttendanceWages(clients, wage, attManager.getAttendance_Wage(WAG_Id));
            ////pageVM.lstClient = WageProcessMapper.MapClientToAttendanceWages(clients, wage, []);
            //return View(pageVM);

            ClientsManager clientsManager = new(_context, _configuration);
            AttendanceSummaryManager AttsummaryManager = new(_context);
            WageProcessManager wageManager = new(_context);
            Wage_Process wage = wageManager.GetWageProcessByWAG_Id(WAG_Id, true, true, true, true);
            List<Client> clients = clientsManager.GetActiveClientsOfMonthByFirmId(ProjectUtils.DateToDateTime(wage.WAG_Month), wage.FRM_Id);
            DateTime NewVersionDT = Convert.ToDateTime(_configuration.GetSection("NEW_VERSION_STARTED").Value);
            DateOnly dtOnly = new(NewVersionDT.Year, NewVersionDT.Month, NewVersionDT.Day);

            WageProcessClientAttendancePageVM pageVM = new()
            {
                wageProcess = WageProcessMapper.MapMe2(wage),
                lstClient = WageProcessMapper.MapClientToAttendanceWages(clients, AttsummaryManager.GetAttendance_Wage(WAG_Id)),
                IsOld = dtOnly > wage.WAG_Month
            };
            return View(pageVM);
        }

        [HttpGet]
        public ActionResult Attendances(int WAG_Id, int CLI_Id)
        {
            ViewAttendanceVM retVM = new();
            WageProcessManager wagManager = new(_context);
            Wage_Process wage = wagManager.GetWageProcessByWAG_Id(WAG_Id);
            ViewBag.Wage_Process = wage;
            ViewBag.WAG_Month = wage.WAG_Month.ToString("MMMM") + "-" + wage.WAG_Month.ToString("yyyy");
            DateTime WAG_Month = ProjectUtils.DateToDateTime(wage.WAG_Month);
            DateTime NewVersionDT = Convert.ToDateTime(_configuration.GetSection("NEW_VERSION_STARTED").Value);
            DateOnly dtOnly = new(NewVersionDT.Year, NewVersionDT.Month, NewVersionDT.Day);

            ClientsManager clientsManager = new(_context);
            Client client = clientsManager.GetClientById(CLI_Id);
            Attendance_Parameter attendance_Parameter = clientsManager.GetAttendanceParameterByMonth(client.CLI_Id, WAG_Month);
            DateTime[] arrDate = DateHelper.GetStartEndDatePeriodForAttendance(client, attendance_Parameter, WAG_Month);
            ViewBag.startDate = arrDate[0];
            ViewBag.endDate = arrDate[1];

            if (dtOnly > wage.WAG_Month)
            {
                AttendanceManager attendanceManager = new(_context);
                retVM.Attendances = AttendanceMapper.mapAttendances(attendanceManager.getAttendance_Wage_Client(WAG_Id, CLI_Id));
            }
            else
            {
                AttendanceSummaryManager AttsummaryManager = new(_context);
                retVM.AttendanceSummaries = AttendanceMapper.MapAttendances(AttsummaryManager.GetAttendance_Wage_Client(WAG_Id, CLI_Id));
            }
            return View(new Tuple<ViewAttendanceVM, Client>(retVM, client));
        }

        [HttpGet]
        public ActionResult ViewAttendanceDayWise(int WAG_Id, int CLI_Id, int EMP_Id)
        {
            ClientsManager clientsManager = new(_context, _configuration);
            AttendanceManager attendanceManager = new(_context);
            WageProcessManager wagManager = new(_context);
            Wage_Process wage = wagManager.GetWageProcessByWAG_Id(WAG_Id);
            ViewBag.WAG_Month = wage.WAG_Month.ToString("MMMM") + "-" + wage.WAG_Month.ToString("yyyy");
            Client client = clientsManager.GetClientById(CLI_Id);
            ViewBag.ClientName = client.CLI_Name;
            ViewBag.CLI_Id = client.CLI_Id;
            ViewBag.CLI_Total_WorkingDays = client.CLI_Total_WorkingDays;
            DateTime WAG_Month = ProjectUtils.DateToDateTime(wage.WAG_Month);
            Attendance_Parameter attendance_Parameter = clientsManager.GetAttendanceParameterByMonth(client.CLI_Id, WAG_Month);
            DateTime[] arrDate = DateHelper.GetStartEndDatePeriodForAttendance(client, attendance_Parameter, WAG_Month);
            ViewBag.startDate = arrDate[0];
            ViewBag.endDate = arrDate[1];
            List<AttendanceVM> list = AttendanceMapper.mapAttendances(attendanceManager.GetAttendance_WageClient_Employee(WAG_Id, CLI_Id, EMP_Id));
            return PartialView(new Tuple<List<AttendanceVM>, Client>(list, client));
        }

        public ActionResult UploadExcel(int WAG_Id, int CLI_Id)
        {
            ClientsManager clientManager = new(_context, _configuration);
            WageProcessManager wageManager = new(_context);
            Wage_Process wageProcess = wageManager.GetWageProcessByWAG_Id(WAG_Id, true, true, true, true);
            UploadExcelViewModel upvm = new()
            {
                WageProcessVM = WageProcessMapper.MapMe2(wageProcess),
                Client = clientManager.GetClientById(CLI_Id)
            };
            ViewBag.templateList = Enum.GetValues(typeof(ATTENDANCE_EXCEL_TEMPLATE));
            return View(upvm);
        }

        [HttpPost]
        public ActionResult VerifyTemplate(UploadExcelViewModel uvm)
        {
            ExcelViewModel excelViewModel = new();
            WageProcessManager wageManager = new(_context);
            ClientsManager clientsManager = new(_context, _configuration);

            //try
            //{
            Wage_Process wageProcess = wageManager.GetWageProcessByWAG_Id(uvm.WageProcessVM.WAG_Id);
            DateTime WAG_Month = ProjectUtils.DateToDateTime(wageProcess.WAG_Month);
            IFormFile file = uvm.ExcelFile;
            if (file != null)
            {
                if (file.Length > 0)
                {
                    Client client = clientsManager.GetClientById(uvm.Client.CLI_Id);
                    Attendance_Parameter attendance_Parameter = clientsManager.GetAttendanceParameterByMonth(client.CLI_Id, WAG_Month);
                    string sFileExtension = Path.GetExtension(file.FileName).ToLower();
                    ISheet sheet;
                    string fullPath = Path.Combine(GetTempFolderPath(_hostingEnvironment.WebRootPath, WAG_Month, client.CLI_Id), ProjectUtils.GetTempFileName() + sFileExtension);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                        stream.Position = 0;

                        if (sFileExtension == ".xls")
                        {
                            HSSFWorkbook hssfwb = new(stream);  //THIS WILL READ THE EXCEL 97-2000 FORMATS  
                            sheet = hssfwb.GetSheetAt(0);       //GET FIRST SHEET FROM WORKBOOK  
                        }
                        else
                        {
                            XSSFWorkbook hssfwb = new(stream);  //THIS WILL READ 2007 EXCEL FORMAT  
                            sheet = hssfwb.GetSheetAt(0);       //GET FIRST SHEET FROM WORKBOOK
                        }
                        switch (uvm.Template)
                        {
                            case "0":
                                excelViewModel = GetAttendance_BASIC_WithoutShifts(sheet, wageProcess, client);
                                break;
                            case "1":
                                excelViewModel = GetAttendance_BASIC_WithShifts(sheet, wageProcess, client);
                                break;
                            case "2":
                                excelViewModel = GetAttendance_OneRow_WithoutShift(sheet, wageProcess, client);
                                break;
                        }
                    }
                    excelViewModel.fileName = fullPath;
                    excelViewModel.Template = uvm.Template;
                    excelViewModel.WAG_Id = uvm.WageProcessVM.WAG_Id;
                    excelViewModel.CLI_Id = uvm.Client.CLI_Id;
                    if (excelViewModel.excelRows.Count > 0)
                    {
                        #region CHECK DATETIME MATCH

                        DateTime[] arr = DateHelper.GetStartEndDatePeriodForAttendance(client, attendance_Parameter, WAG_Month);
                        if (excelViewModel.startDate.Date != arr[0].Date || excelViewModel.endDate.Date != arr[1].Date)
                            excelViewModel.datePeriod = false;

                        #endregion

                        IEnumerable<Clients_Employee> assignEmployeeList = clientsManager.listActiveClientsEmployees(client.CLI_Id, WAG_Month);
                        List<_EmpID> _empID = [];
                        List<Employee> empListExtraInExcel = [], empListExtraInDb = [];

                        #region CHECK EXTRA EMPLOYEES IN EXCEL

                        foreach (ExcelRowViewModel row in excelViewModel.excelRows)
                        {
                            Clients_Employee emp = assignEmployeeList.Where(m => m.EMP_Id.Equals(Convert.ToInt32(row.EMP_Id))).FirstOrDefault();
                            if (emp == null)
                                empListExtraInExcel.Add(new Employee { EMP_Id = Convert.ToInt32(row.EMP_Id), EMP_FirstName = row.EMP_Name.Trim() });
                            else
                                _empID.Add(new() { Id = emp.EMP_Id });
                        }

                        #endregion

                        #region CHECK EMPLOYEES REMAINING EXCEL

                        EmployeeManager empManager = new(_context);
                        HashSet<int> diffids = new(_empID.Select(s => s.Id));
                        var EmployeeIdList = assignEmployeeList.Where(m => !diffids.Contains(m.EMP_Id)).ToList();
                        foreach (var EMP_Id in EmployeeIdList)
                            empListExtraInDb.Add(empManager.GetEmployeeById(Convert.ToInt32(EMP_Id.EMP_Id)));

                        if (empListExtraInExcel.Count > 0 || empListExtraInDb.Count > 0 || excelViewModel.datePeriod == false)
                            excelViewModel.btnExportToDatabase = false;

                        excelViewModel.empListExtraInExcel = empListExtraInExcel;
                        excelViewModel.EmpListExtraInDb = empListExtraInDb;

                        #endregion
                    }
                }
            }
            excelViewModel.FRM_Id = uvm.WageProcessVM.FRM_Id;
            return View(excelViewModel);
            //}
            //catch (Exception ex)
            //{
            //    TempData["err"]="Try Again....";
            //    return RedirectToAction("WageAttendanceList", new { WAG_Id = uvm.wageProcessVM.WAG_Id });
            //}
        }

        #region CALCULATE & VERIFY UPLOADED EXCEL DATA

        [NonAction]
        private ExcelViewModel GetAttendance_BASIC_WithoutShifts(ISheet sheet, Wage_Process wageProcess, Client client)
        {
            List<string> errors = [];
            ExcelViewModel excelViewModel = new();
            List<Attendance> attandanceList = [];
            List<ExcelRowViewModel> rows = [];
            try
            {
                IRow headerRow = sheet.GetRow(0), secondRow = sheet.GetRow(4);

                if (secondRow != null)
                {
                    int cellCount = secondRow.LastCellNum, TotEmp = 0, EMP_Id = 0, totalPublicHolidays = 0,
                        intStartDate = GetNumberFromString(secondRow.GetCell(4).ToString());    //ADDED BCZ MON-TUE ADDED IN EXCEL

                    DateTime startDate = DateTime.Now, endDate = DateTime.Now;
                    if (intStartDate > 1)
                    {
                        DateOnly lastMonth = wageProcess.WAG_Month.AddMonths(-1);
                        startDate = new DateTime(lastMonth.Year, lastMonth.Month, intStartDate);
                    }
                    else
                        startDate = new DateTime(wageProcess.WAG_Month.Year, wageProcess.WAG_Month.Month, intStartDate);

                    endDate = new DateTime(wageProcess.WAG_Month.Year, wageProcess.WAG_Month.Month, GetNumberFromString(secondRow.GetCell(cellCount - 1).ToString()));


                    for (int i = (sheet.FirstRowNum + 5); i <= sheet.LastRowNum; i += 2)
                    {
                        ExcelRowViewModel excelRow = new();
                        IRow row = sheet.GetRow(i), rowExtra = sheet.GetRow(i + 1);

                        if (row == null) continue;
                        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;

                        EMP_Id = Convert.ToInt32(row.GetCell(1).ToString());
                        excelRow.EMP_Id = row.GetCell(1).ToString();
                        excelRow.EMP_Name = row.GetCell(3).ToString();
                        excelRow.Designation = row.GetCell(2).ToString();
                        TotEmp++;

                        int totalWeeklyOff = 0, totalPublicHoliday = 0, totalEarnLeave = 0, totalNightShift = 0;
                        double totalExtraHours = 0, totalPresence = 0, totalHalfdays = 0;

                        DateTime tmpDate = startDate;
                        for (int j = (row.FirstCellNum + 4); j <= row.LastCellNum - 1; j++)
                        {
                            Attendance attendance = new()
                            {
                                EMP_Id = EMP_Id,
                                CLI_Id = client.CLI_Id,
                                ATT_Date = DAL.Helpers.ProjectUtils.DateTimeToDate(tmpDate)
                            };

                            switch (row.GetCell(j).ToString().Replace(" ", ""))
                            {
                                case "P":
                                    attendance.ATT_IsPresent = true;
                                    totalPresence++;
                                    break;
                                case "PL":
                                    attendance.ATT_IsPresent = false;
                                    attendance.ATT_IsEarnLeave = true;
                                    totalEarnLeave++;
                                    break;
                                case "WO":
                                    attendance.ATT_IsPresent = false;
                                    attendance.ATT_IsWeeklyOff = true;
                                    totalWeeklyOff++;
                                    break;
                                case "W/O":
                                    attendance.ATT_IsPresent = false;
                                    attendance.ATT_IsWeeklyOff = true;
                                    totalWeeklyOff++;
                                    break;
                                case "PW":
                                    attendance.ATT_IsPresent = true;
                                    attendance.ATT_IsWeeklyOff = true;
                                    totalPresence++;
                                    totalWeeklyOff++;
                                    break;
                                case "CO":
                                    attendance.ATT_IsPresent = false;
                                    attendance.ATT_IsEarnLeave = true;
                                    totalEarnLeave++;
                                    break;
                                case "C/O":
                                    attendance.ATT_IsPresent = false;
                                    attendance.ATT_IsEarnLeave = true;
                                    totalEarnLeave++;
                                    break;
                                case "HO":
                                    attendance.ATT_IsPresent = false;
                                    attendance.ATT_IsPublicHoliday = true;
                                    totalPublicHoliday++;
                                    break;
                                case "PO":
                                    attendance.ATT_IsPresent = true;
                                    attendance.ATT_IsPublicHoliday = true;
                                    totalPresence++;
                                    totalPublicHoliday++;
                                    break;
                                case "NH":
                                    attendance.ATT_IsPresent = false;
                                    attendance.ATT_IsPublicHoliday = true;
                                    totalPublicHoliday++;
                                    break;
                                case "PN":
                                    attendance.ATT_IsPresent = true;
                                    attendance.ATT_IsPublicHoliday = true;
                                    totalPresence++;
                                    totalPublicHoliday++;
                                    break;
                                case "P/2":
                                    attendance.ATT_IsPresent = true;
                                    attendance.ATT_IsHalfday = true;
                                    totalPresence++;
                                    totalHalfdays++;
                                    break;
                                case "HF":
                                    attendance.ATT_IsPresent = true;
                                    attendance.ATT_IsHalfday = true;
                                    totalPresence++;
                                    totalHalfdays++;
                                    break;
                                case "A":
                                    attendance.ATT_IsPresent = false;
                                    break;
                                case "CP":
                                    attendance.ATT_IsPresent = true;
                                    attendance.ATT_NightShift = true;
                                    totalPresence++;
                                    totalNightShift++;
                                    break;
                                case "NC":
                                    attendance.ATT_IsPresent = true;
                                    attendance.ATT_NightShift = true;
                                    attendance.ATT_IsPublicHoliday = true;
                                    totalPresence++;
                                    totalNightShift++;
                                    totalPublicHoliday++;
                                    break;
                                case "CW":
                                    attendance.ATT_IsPresent = true;
                                    attendance.ATT_NightShift = true;
                                    attendance.ATT_IsWeeklyOff = true;
                                    totalPresence++;
                                    totalNightShift++;
                                    totalWeeklyOff++;
                                    break;
                                case "EL":
                                    attendance.ATT_IsPresent = false;
                                    attendance.ATT_IsEarnLeave = true;
                                    totalEarnLeave++;
                                    break;
                                case "E/L":
                                    attendance.ATT_IsPresent = false;
                                    attendance.ATT_IsEarnLeave = true;
                                    totalEarnLeave++;
                                    break;
                                case "PH":
                                    attendance.ATT_IsPresent = false;
                                    attendance.ATT_IsPublicHoliday = true;
                                    totalPublicHoliday++;
                                    break;
                            }

                            if (rowExtra.GetCell(j) != null)
                                if (!rowExtra.GetCell(j).ToString().Equals(""))
                                {
                                    if (double.TryParse(rowExtra.GetCell(j).ToString(), out double value))
                                    {
                                        attendance.ATT_ExtraHoursWorked = value;
                                        totalExtraHours += value;
                                    }
                                }

                            attandanceList.Add(attendance);
                            tmpDate = tmpDate.AddDays(1);
                        }
                        excelRow.totalPresenceDays = totalPresence + (totalHalfdays / 2) - totalHalfdays;
                        excelRow.totalWeeklyOff = totalWeeklyOff;
                        excelRow.totalHolidays = totalPublicHoliday;
                        excelRow.TotalEarnLeave = totalEarnLeave;
                        excelRow.TotalNightshift = totalNightShift;
                        excelRow.TotalExtraHours = totalExtraHours;
                        rows.Add(excelRow);
                        totalPublicHolidays += totalPublicHoliday;
                    }
                    excelViewModel.listAttendance = attandanceList;
                    excelViewModel.excelRows = rows;
                    excelViewModel.totalEmployees = TotEmp;
                    excelViewModel.startDate = startDate;
                    excelViewModel.endDate = endDate;
                    excelViewModel.totalPublicHolidays = totalPublicHolidays;
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
            }
            return excelViewModel;
        }

        [NonAction]
        private ExcelViewModel GetAttendance_BASIC_WithShifts(ISheet sheet, Wage_Process wageProcess, Client client)
        {
            ExcelViewModel excelViewModel = new ExcelViewModel();
            List<Attendance> attandanceList = new List<Attendance>();
            List<ExcelRowViewModel> rows = new List<ExcelRowViewModel>();
            try
            {
                IRow headerRow = sheet.GetRow(0);
                IRow secondRow = sheet.GetRow(4);
                int cellCount = secondRow.LastCellNum;
                int intStartDate = GetNumberFromString(secondRow.GetCell(4).ToString());
                int totalPublicHolidays = 0;

                int TotEmp = 0;

                DateTime startDate = DateTime.Now, endDate = DateTime.Now;
                if (intStartDate > 1)
                {
                    DateOnly lastMonth = wageProcess.WAG_Month.AddMonths(-1);
                    startDate = new DateTime(lastMonth.Year, lastMonth.Month, intStartDate);
                }
                else
                {
                    startDate = new DateTime(wageProcess.WAG_Month.Year, wageProcess.WAG_Month.Month, intStartDate);
                }
                endDate = new DateTime(wageProcess.WAG_Month.Year, wageProcess.WAG_Month.Month, GetNumberFromString(secondRow.GetCell(cellCount - 1).ToString()));
                for (int i = (sheet.FirstRowNum + 5); i <= sheet.LastRowNum; i += 2)
                {
                    ExcelRowViewModel excelRow = new ExcelRowViewModel();
                    IRow row = sheet.GetRow(i);
                    IRow rowExtra = sheet.GetRow(i + 1);

                    if (row == null) continue;
                    if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                    excelRow.EMP_Id = row.GetCell(1).ToString();
                    int EMP_Id = Convert.ToInt32(row.GetCell(1).ToString());
                    excelRow.EMP_Name = row.GetCell(3).ToString();
                    excelRow.Designation = row.GetCell(2).ToString();
                    TotEmp++;
                    int totalWeeklyOff = 0, totalPublicHoliday = 0, totalEarnLeave = 0, totalNightShift = 0;
                    Double totalExtraHours = 0, totalPresence = 0, totalHalfdays = 0;
                    DateTime tmpDate = startDate;
                    for (int j = (row.FirstCellNum + 4); j <= row.LastCellNum - 1; j++)
                    {
                        Attendance attendance = new Attendance();
                        attendance.EMP_Id = EMP_Id;
                        attendance.CLI_Id = client.CLI_Id;
                        attendance.ATT_Date = DAL.Helpers.ProjectUtils.DateTimeToDate(tmpDate);
                        tmpDate = tmpDate.AddDays(1);

                        switch (row.GetCell(j).ToString().Replace(" ", ""))
                        {
                            case ("G"):
                                attendance.ATT_IsPresent = true;
                                attendance.ATT_Shift = row.GetCell(j).ToString();
                                totalPresence++;
                                break;
                            case "I":
                                attendance.ATT_IsPresent = true;
                                attendance.ATT_Shift = row.GetCell(j).ToString();
                                totalPresence++;
                                break;
                            case "II":
                                attendance.ATT_IsPresent = true;
                                attendance.ATT_Shift = row.GetCell(j).ToString();
                                totalPresence++;
                                break;
                            case "III":
                                attendance.ATT_IsPresent = true;
                                attendance.ATT_Shift = row.GetCell(j).ToString();
                                totalPresence++;
                                break;
                            case "PL":
                                attendance.ATT_IsPresent = false;
                                attendance.ATT_IsEarnLeave = true;
                                totalEarnLeave++;
                                break;
                            case "WO":
                                attendance.ATT_IsPresent = false;
                                attendance.ATT_IsWeeklyOff = true;
                                totalWeeklyOff++;
                                break;
                            case "W/O":
                                attendance.ATT_IsPresent = false;
                                attendance.ATT_IsWeeklyOff = true;
                                totalWeeklyOff++;
                                break;
                            case "PW":
                                attendance.ATT_IsPresent = true;
                                attendance.ATT_IsWeeklyOff = true;
                                totalWeeklyOff++;
                                totalPresence++;
                                break;
                            case "CO":
                                attendance.ATT_IsPresent = false;
                                attendance.ATT_IsEarnLeave = true;
                                totalEarnLeave++;
                                break;
                            case "C/O":
                                attendance.ATT_IsPresent = false;
                                attendance.ATT_IsEarnLeave = true;
                                totalEarnLeave++;
                                break;
                            case "HO":
                                attendance.ATT_IsPresent = false;
                                attendance.ATT_IsPublicHoliday = true;
                                totalPublicHoliday++;
                                break;
                            case "PO":
                                attendance.ATT_IsPresent = true;
                                attendance.ATT_IsPublicHoliday = true;
                                totalPublicHoliday++;
                                totalPresence++;
                                break;
                            case "NH":
                                attendance.ATT_IsPresent = false;
                                attendance.ATT_IsPublicHoliday = true;
                                totalPublicHoliday++;
                                break;
                            case "PN":
                                attendance.ATT_IsPresent = true;
                                attendance.ATT_IsPublicHoliday = true;
                                totalPresence++;
                                totalPublicHoliday++;
                                break;
                            case ("G/2"):
                                attendance.ATT_IsPresent = true;
                                attendance.ATT_IsHalfday = true;
                                totalPresence++;
                                totalHalfdays++;
                                break;
                            case "I/2":
                                attendance.ATT_IsPresent = true;
                                attendance.ATT_IsHalfday = true;
                                attendance.ATT_Shift = row.GetCell(j).ToString();
                                totalPresence++;
                                totalHalfdays++;
                                break;
                            case "II/2":
                                attendance.ATT_IsPresent = true;
                                attendance.ATT_IsHalfday = true;
                                attendance.ATT_Shift = row.GetCell(j).ToString();
                                totalHalfdays++;
                                totalPresence++;
                                break;
                            case "III/2":
                                attendance.ATT_IsPresent = true;
                                attendance.ATT_IsHalfday = true;
                                attendance.ATT_Shift = row.GetCell(j).ToString();
                                totalHalfdays++;
                                totalPresence++;
                                break;
                            case "HF":
                                attendance.ATT_IsPresent = true;
                                attendance.ATT_IsHalfday = true;
                                attendance.ATT_Shift = row.GetCell(j).ToString();
                                totalHalfdays++;
                                totalPresence++;
                                break;
                            case "A":
                                attendance.ATT_IsPresent = false;
                                break;
                            case "CP":
                                attendance.ATT_IsPresent = true;
                                attendance.ATT_NightShift = true;
                                totalNightShift++;
                                totalPresence++;
                                break;
                            case "NC":
                                attendance.ATT_IsPresent = true;
                                attendance.ATT_IsPublicHoliday = true;
                                attendance.ATT_NightShift = true;
                                totalNightShift++;
                                totalPresence++;
                                totalPublicHoliday++;
                                break;
                            case "CW":
                                attendance.ATT_IsPresent = true;
                                attendance.ATT_NightShift = true;
                                attendance.ATT_IsWeeklyOff = true;
                                totalNightShift++;
                                totalPresence++;
                                totalWeeklyOff++;
                                break;
                            case "EL":
                                attendance.ATT_IsPresent = false;
                                attendance.ATT_IsEarnLeave = true;
                                totalEarnLeave++;
                                break;
                            case "E/L":
                                attendance.ATT_IsPresent = false;
                                attendance.ATT_IsEarnLeave = true;
                                totalEarnLeave++;
                                break;
                            case "PH":
                                attendance.ATT_IsPresent = false;
                                attendance.ATT_IsPublicHoliday = true;
                                totalPublicHoliday++;
                                break;
                        }
                        if (rowExtra.GetCell(j) != null)
                            if (!rowExtra.GetCell(j).ToString().Equals(""))
                            {
                                if (double.TryParse(rowExtra.GetCell(j).ToString(), out double value))
                                {
                                    attendance.ATT_ExtraHoursWorked = value;
                                    totalExtraHours += value;
                                }

                            }
                        attandanceList.Add(attendance);
                    }

                    excelRow.totalPresenceDays = totalPresence + (totalHalfdays / 2) - totalHalfdays;
                    excelRow.totalWeeklyOff = totalWeeklyOff;
                    excelRow.totalHolidays = totalPublicHoliday;
                    excelRow.TotalEarnLeave = totalEarnLeave;
                    excelRow.TotalNightshift = totalNightShift;
                    excelRow.TotalExtraHours = totalExtraHours;

                    rows.Add(excelRow);

                    totalPublicHolidays += totalPublicHoliday;
                }
                excelViewModel.listAttendance = attandanceList;
                excelViewModel.excelRows = rows;
                excelViewModel.totalEmployees = TotEmp;
                excelViewModel.startDate = startDate;
                excelViewModel.endDate = endDate;
                excelViewModel.totalPublicHolidays = totalPublicHolidays;

            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
            }
            return excelViewModel;
        }

        [NonAction]
        private ExcelViewModel GetAttendance_OneRow_WithoutShift(ISheet sheet, Wage_Process wageProcess, Client client)
        {
            ExcelViewModel excelViewModel = new ExcelViewModel();
            List<Attendance> attandanceList = new List<Attendance>();
            List<ExcelRowViewModel> rows = new List<ExcelRowViewModel>();
            //try
            //{
            IRow headerRow = sheet.GetRow(0);
            IRow secondRow = sheet.GetRow(4);
            int cellCount = secondRow.LastCellNum;
            int intStartDate = GetNumberFromString(secondRow.GetCell(4).ToString());
            int totalPublicHolidays = 0;

            for (int j = (secondRow.FirstCellNum + 4); j <= secondRow.LastCellNum - 1; j++)
            {
                if (secondRow.GetCell(j).ToString().Contains("PH"))
                    totalPublicHolidays++;
            }
            int TotEmp = 0;
            DateTime startDate = DateTime.Now, endDate = DateTime.Now;
            if (intStartDate > 1)
            {
                DateOnly lastMonth = wageProcess.WAG_Month.AddMonths(-1);
                startDate = new DateTime(lastMonth.Year, lastMonth.Month, intStartDate);
            }
            else
            {
                startDate = new DateTime(wageProcess.WAG_Month.Year, wageProcess.WAG_Month.Month, intStartDate);
            }
            endDate = new DateTime(wageProcess.WAG_Month.Year, wageProcess.WAG_Month.Month, GetNumberFromString(secondRow.GetCell(cellCount - 1).ToString()));

            IRow rowHeader = sheet.GetRow(1);
            for (int i = (sheet.FirstRowNum + 5); i <= sheet.LastRowNum; i++)
            {
                ExcelRowViewModel excelRow = new ExcelRowViewModel();
                IRow row = sheet.GetRow(i);
                if (row == null) continue;
                if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                excelRow.EMP_Id = row.GetCell(1).ToString();
                int EMP_Id = Convert.ToInt32(row.GetCell(1).ToString());
                excelRow.EMP_Name = row.GetCell(3).ToString();
                excelRow.Designation = row.GetCell(2).ToString();
                TotEmp++;
                int totalWeeklyOff = 0, totalPublicHoliday = 0, totalEarnLeave = 0, totalNightShift = 0;
                double totalHalfdays = 0, totalPresence = 0;
                DateTime tmpDate = startDate;

                for (int j = (row.FirstCellNum + 4); j <= row.LastCellNum - 1; j++)
                {
                    Attendance attendance = new Attendance();
                    attendance.EMP_Id = EMP_Id;
                    attendance.CLI_Id = client.CLI_Id;
                    attendance.ATT_Date = DAL.Helpers.ProjectUtils.DateTimeToDate(tmpDate);
                    tmpDate = tmpDate.AddDays(1);

                    if (row.GetCell(j).ToString().Contains("PH"))
                    {
                        attendance.ATT_IsPublicHoliday = true;
                        totalPublicHoliday++;

                        switch (row.GetCell(j).ToString())
                        {
                            case "P":
                                attendance.ATT_IsPresent = true;
                                totalPresence++;
                                break;
                            case "A":
                                attendance.ATT_IsPresent = false;
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        switch (row.GetCell(j).ToString().Replace(" ", ""))
                        {
                            case "P":
                                attendance.ATT_IsPresent = true;
                                totalPresence++;
                                break;
                            case "PL":
                                attendance.ATT_IsPresent = false;
                                attendance.ATT_IsEarnLeave = true;
                                totalEarnLeave++;
                                break;
                            case "WO":
                                attendance.ATT_IsPresent = false;
                                attendance.ATT_IsWeeklyOff = true;
                                totalWeeklyOff++;
                                break;
                            case "W/O":
                                attendance.ATT_IsPresent = false;
                                attendance.ATT_IsWeeklyOff = true;
                                totalWeeklyOff++;
                                break;
                            case "PW":
                                attendance.ATT_IsPresent = true;
                                attendance.ATT_IsWeeklyOff = true;
                                totalPresence++;
                                totalWeeklyOff++;
                                break;
                            case "CO":
                                attendance.ATT_IsPresent = false;
                                attendance.ATT_IsEarnLeave = true;
                                totalEarnLeave++;
                                break;
                            case "C/O":
                                attendance.ATT_IsPresent = false;
                                attendance.ATT_IsEarnLeave = true;
                                totalEarnLeave++;
                                break;
                            case "HO":
                                attendance.ATT_IsPresent = false;
                                attendance.ATT_IsPublicHoliday = true;
                                totalPublicHoliday++;
                                break;
                            case "PO":
                                attendance.ATT_IsPresent = true;
                                attendance.ATT_IsPublicHoliday = true;
                                totalPresence++;
                                totalPublicHoliday++;
                                break;
                            case "NH":
                                attendance.ATT_IsPresent = false;
                                attendance.ATT_IsPublicHoliday = true;
                                totalPublicHoliday++;
                                break;
                            case "PN":
                                attendance.ATT_IsPresent = true;
                                attendance.ATT_IsPublicHoliday = true;
                                totalPresence++;
                                totalPublicHoliday++;
                                break;
                            case "P/2":
                                attendance.ATT_IsPresent = true;
                                attendance.ATT_IsHalfday = true;
                                totalPresence++;
                                totalHalfdays++;
                                break;
                            case "HF":
                                attendance.ATT_IsPresent = true;
                                attendance.ATT_IsHalfday = true;
                                totalPresence++;
                                totalHalfdays++;
                                break;
                            case "A":
                                attendance.ATT_IsPresent = false;
                                break;
                            case "AB":
                                attendance.ATT_IsPresent = false;
                                break;
                            case "CP":
                                attendance.ATT_IsPresent = true;
                                attendance.ATT_NightShift = true;
                                totalPresence++;
                                totalNightShift++;
                                break;
                            case "NC":
                                attendance.ATT_IsPresent = true;
                                attendance.ATT_NightShift = true;
                                attendance.ATT_IsPublicHoliday = true;
                                totalPresence++;
                                totalNightShift++;
                                totalPublicHoliday++;
                                break;
                            case "CW":
                                attendance.ATT_IsPresent = true;
                                attendance.ATT_NightShift = true;
                                attendance.ATT_IsWeeklyOff = true;
                                totalPresence++;
                                totalNightShift++;
                                totalWeeklyOff++;
                                break;
                            case "EL":
                                attendance.ATT_IsPresent = false;
                                attendance.ATT_IsEarnLeave = true;
                                totalEarnLeave++;
                                break;
                            case "E/L":
                                attendance.ATT_IsPresent = false;
                                attendance.ATT_IsEarnLeave = true;
                                totalEarnLeave++;
                                break;
                                //case "PH":
                                //    attendance.ATT_IsPresent = false;
                                //    attendance.ATT_IsPublicHoliday = true;
                                //    totalPublicHoliday++;
                                //    break;
                        }
                    }
                    attandanceList.Add(attendance);
                }

                excelRow.totalPresenceDays = totalPresence + (totalHalfdays / 2) - totalHalfdays;
                excelRow.totalWeeklyOff = totalWeeklyOff;
                excelRow.totalHolidays = totalPublicHoliday;
                excelRow.TotalEarnLeave = totalEarnLeave;
                excelRow.TotalNightshift = totalNightShift;

                // totalPaybleDays = totalPresence + totalExtraDays + totalHOPresent + totalLeaves + totalHolidays + totalCOs + totExtraWorkingDays + (totalHalfdays / 2);
                // excelRow.totalPaybleDays = totalPaybleDays;

                rows.Add(excelRow);
            }
            excelViewModel.listAttendance = attandanceList;
            excelViewModel.excelRows = rows;
            excelViewModel.totalEmployees = TotEmp;
            excelViewModel.startDate = startDate;
            excelViewModel.endDate = endDate;
            excelViewModel.totalPublicHolidays = totalPublicHolidays;
            //}
            //catch (Exception ex)
            //{
            //    TempData["error"] = ex.Message;                
            //}

            return excelViewModel;
        }

        #endregion

        [HttpPost]
        public ActionResult ImportExcel(IFormCollection frm)
        {
            int WAG_Id = Convert.ToInt32(frm["WAG_Id"]);
            int CLI_Id = Convert.ToInt32(frm["CLI_Id"]);
            AttendanceManager attendanceManager = new(_context);
            AttendanceSummaryManager attSummaryManager = new(_context);
            if (!attSummaryManager.IsAlreadyUploaded(WAG_Id, CLI_Id) && !attendanceManager.IsAttendanceAlreadyUploaded(WAG_Id, CLI_Id))
            {
                try
                {
                    string strFilePath = frm["fileName"];
                    ClientsManager clientManager = new(_context, _configuration);
                    WageProcessManager wageManager = new(_context);
                    Wage_Process wageProcess = wageManager.GetWageProcessByWAG_Id(WAG_Id);
                    Client client = clientManager.GetClientById(CLI_Id);
                    ISheet sheet;
                    using (var stream = new FileStream(strFilePath, FileMode.Open))
                    {
                        string sFileExtension = Path.GetExtension(strFilePath).ToLower();
                        stream.Position = 0;
                        if (sFileExtension == ".xls")
                        {
                            HSSFWorkbook hssfwb = new(stream);  //THIS WILL READ THE EXCEL 97-2000 FORMATS  
                            sheet = hssfwb.GetSheetAt(0);       //GET FIRST SHEET FROM WORKBOOK  
                        }
                        else
                        {
                            XSSFWorkbook hssfwb = new(stream);  //THIS WILL READ 2007 EXCEL FORMAT  
                            sheet = hssfwb.GetSheetAt(0);       //GET FIRST SHEET FROM WORKBOOK   
                        }
                        switch (frm["Template"])
                        {
                            case "0":
                                //SaveAttendance_BASIC_WithoutShifts(sheet, wageProcess, client);
                                //SaveAttendanceSummary_BASIC_WithoutShifts(sheet, wageProcess, client);
                                SaveAttendanceAndSummaries_BASIC_TWOROW_WithoutShifts(sheet, wageProcess, client);
                                break;
                            case "1":
                                //SaveAttendance_BASIC_WithShifts(sheet, wageProcess, client);
                                //SaveAttendanceSummary_BASIC_WithShifts(sheet, wageProcess, client);
                                SaveAttendanceAndSummaries_BASIC_WithShifts(sheet, wageProcess, client);
                                break;
                            case "2":
                                //SaveAttendance_ONEROW_WithoutShift(sheet, wageProcess, client);
                                //SaveAttendanceSummary_ONEROW_WithoutShift(sheet, wageProcess, client);
                                SaveAttendanceAndSummaries_ONEROW_WithoutShifts(sheet, wageProcess, client);
                                break;
                        }
                    }
                }
                catch (Exception)
                {
                    TempData["err"] = "Try Again";
                }
            }
            return RedirectToAction("WageAttendanceList", new { WAG_Id = WAG_Id });
        }

        #region SAVE ATTENDANCE & SUMMARIES

        [NonAction]
        private void SaveAttendanceAndSummaries_BASIC_TWOROW_WithoutShifts(ISheet sheet, Wage_Process wageProcess, Client client)
        {
            AttendanceManager attManager = new(_context);
            AttendanceSummaryManager attSummaryManager = new(_context);
            DesignationManager designationManager = new(_context);
            SessionUtils sessionUtils = new(Request, Response);

            IRow headerRow = sheet.GetRow(0), secondRow = sheet.GetRow(4);
            int cellCount = secondRow.LastCellNum, intStartDate = GetNumberFromString(secondRow.GetCell(4).ToString());

            DateTime startDate = DateTime.Now, endDate = DateTime.Now;
            if (intStartDate > 1)
            {
                DateOnly lastMonth = wageProcess.WAG_Month.AddMonths(-1);
                startDate = new DateTime(lastMonth.Year, lastMonth.Month, intStartDate);
            }
            else
            {
                startDate = new DateTime(wageProcess.WAG_Month.Year, wageProcess.WAG_Month.Month, intStartDate);
            }
            endDate = new DateTime(wageProcess.WAG_Month.Year, wageProcess.WAG_Month.Month, GetNumberFromString(secondRow.GetCell(cellCount - 1).ToString()));

            List<Attendance_Summary> attSummaries = [];

            for (int i = (sheet.FirstRowNum + 5); i <= sheet.LastRowNum; i += 2)
            {
                List<Attendance> atts = [];
                IRow row = sheet.GetRow(i), rowExtra = sheet.GetRow(i + 1);

                if (row == null) continue;
                if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;

                int EMP_Id = Convert.ToInt16(row.GetCell(1).ToString());
                DateTime tmpDate = startDate;
                double ATS_PresentDays = 0, ATS_WeekOff = 0, ATS_PublicHolidays = 0, ATS_EarnLeaves = 0, ATS_NightShifts = 0, ATS_ExtraHours = 0, ATS_HalfDays = 0;


                for (int j = (row.FirstCellNum + 4); j <= row.LastCellNum - 1; j++)
                {
                    if (secondRow.GetCell(j) != null)
                    {
                        Attendance att = new()
                        {
                            EMP_Id = EMP_Id,
                            WAG_Id = wageProcess.WAG_Id,
                            CLI_Id = client.CLI_Id,
                            DES_Id = designationManager.getDesignationIdForAttandance(client.CLI_Id, EMP_Id, ProjectUtils.DateToDateTime(wageProcess.WAG_Month)),
                            ATT_Date = ProjectUtils.DateTimeToDate(tmpDate),
                            ATT_ImportedOn = DateTime.Now,
                            ADM_Id_ImportedBy = sessionUtils.GetLoggedAdminID()
                        };

                        switch (row.GetCell(j).ToString().Replace(" ", ""))
                        {
                            case "P":
                                att.ATT_IsPresent = true;
                                ATS_PresentDays++;
                                break;
                            case "PL":
                                att.ATT_IsPresent = false;
                                att.ATT_IsEarnLeave = true;
                                ATS_EarnLeaves++;
                                break;
                            case "WO":
                                att.ATT_IsPresent = false;
                                att.ATT_IsWeeklyOff = true;
                                ATS_WeekOff++;
                                break;
                            case "W/O":
                                att.ATT_IsPresent = false;
                                att.ATT_IsWeeklyOff = true;
                                ATS_WeekOff++;
                                break;
                            case "PW":
                                att.ATT_IsPresent = true;
                                att.ATT_IsWeeklyOff = true;
                                ATS_PresentDays++;
                                ATS_WeekOff++;
                                break;
                            case "CO":
                                att.ATT_IsPresent = false;
                                att.ATT_IsEarnLeave = true;
                                ATS_EarnLeaves++;
                                break;
                            case "C/O":
                                att.ATT_IsPresent = false;
                                att.ATT_IsEarnLeave = true;
                                ATS_EarnLeaves++;
                                break;
                            case "HO":
                                att.ATT_IsPresent = false;
                                att.ATT_IsPublicHoliday = true;
                                ATS_PublicHolidays++;
                                break;
                            case "PO":
                                att.ATT_IsPresent = true;
                                att.ATT_IsPublicHoliday = true;
                                ATS_PresentDays++;
                                ATS_PublicHolidays++;
                                break;
                            case "NH":
                                att.ATT_IsPresent = false;
                                att.ATT_IsPublicHoliday = true;
                                ATS_PublicHolidays++;
                                break;
                            case "PN":
                                att.ATT_IsPresent = true;
                                att.ATT_IsPublicHoliday = true;
                                ATS_PresentDays++;
                                ATS_PublicHolidays++;
                                break;
                            case "P/2":
                                att.ATT_IsPresent = true;
                                att.ATT_IsHalfday = true;
                                ATS_PresentDays++;
                                ATS_HalfDays++;
                                break;
                            case "HF":
                                att.ATT_IsPresent = true;
                                att.ATT_IsHalfday = true;
                                ATS_PresentDays++;
                                ATS_HalfDays++;
                                break;
                            case "A":
                                att.ATT_IsPresent = false;
                                break;
                            case "CP":
                                att.ATT_IsPresent = true;
                                att.ATT_NightShift = true;
                                ATS_PresentDays++;
                                ATS_NightShifts++;
                                break;
                            case "NC":
                                att.ATT_IsPresent = true;
                                att.ATT_NightShift = true;
                                att.ATT_IsPublicHoliday = true;
                                ATS_PresentDays++;
                                ATS_NightShifts++;
                                ATS_PublicHolidays++;
                                break;
                            case "CW":
                                att.ATT_IsPresent = true;
                                att.ATT_NightShift = true;
                                att.ATT_IsWeeklyOff = true;
                                ATS_PresentDays++;
                                ATS_NightShifts++;
                                ATS_WeekOff++;
                                break;
                            case "EL":
                                att.ATT_IsPresent = false;
                                att.ATT_IsEarnLeave = true;
                                ATS_EarnLeaves++;
                                break;
                            case "E/L":
                                att.ATT_IsPresent = false;
                                att.ATT_IsEarnLeave = true;
                                ATS_EarnLeaves++;
                                break;
                            case "PH":
                                att.ATT_IsPresent = false;
                                att.ATT_IsPublicHoliday = true;
                                ATS_PublicHolidays++;
                                break;
                        }

                        att.ATT_Orignal_Row1 = row.GetCell(j).ToString();

                        if (rowExtra.GetCell(j) != null)
                        {
                            if (!rowExtra.GetCell(j).ToString().Equals(""))
                            {
                                att.ATT_Orignal_Row2 = rowExtra.GetCell(j).ToString();
                                if (double.TryParse(rowExtra.GetCell(j).ToString(), out double value))
                                {
                                    att.ATT_ExtraHoursWorked = value;
                                    ATS_ExtraHours += value;
                                }
                            }
                        }
                        atts.Add(att);
                        tmpDate = tmpDate.AddDays(1);
                    }
                }

                attSummaries.Add(new()
                {
                    EMP_Id = EMP_Id,
                    WAG_Id = wageProcess.WAG_Id,
                    CLI_Id = client.CLI_Id,
                    DES_Id = designationManager.getDesignationIdForAttandance(client.CLI_Id, EMP_Id, ProjectUtils.DateToDateTime(wageProcess.WAG_Month)),
                    ATS_PresentDays = ATS_PresentDays,
                    ATS_HalfDays = ATS_HalfDays,
                    ATS_PublicHolidays = ATS_PublicHolidays,
                    ATS_WeekOff = ATS_WeekOff,
                    ATS_EarnLeaves = ATS_EarnLeaves,
                    ATS_NightShifts = ATS_NightShifts,
                    ATS_ExtraHours = ATS_ExtraHours,
                    ADM_Id_ImportedBy = sessionUtils.GetLoggedAdminID(),
                    ATS_ImportedOn = DateTime.Now,
                    ATS_TemplateReference = "TWOROW_WithoutShifts"
                });
                attManager.Save(atts);  //ADD DAY WISE ATTENDANCES PER EMPLOYEES
            }
            attSummaryManager.Save(attSummaries);   //ADD ATTENDANCE SUMMARIES ALL EMPLOYEES
        }

        [NonAction]
        private void SaveAttendanceAndSummaries_BASIC_WithShifts(ISheet sheet, Wage_Process wageProcess, Client client)
        {
            AttendanceManager attManager = new AttendanceManager(_context);
            DesignationManager designationManager = new DesignationManager(_context);
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            IRow headerRow = sheet.GetRow(0);
            IRow secondRow = sheet.GetRow(4);
            int cellCount = secondRow.LastCellNum;
            int intStartDate = GetNumberFromString(secondRow.GetCell(4).ToString());
            DateTime startDate = DateTime.Now, endDate = DateTime.Now;
            if (intStartDate > 1)
            {
                DateOnly lastMonth = wageProcess.WAG_Month.AddMonths(-1);
                startDate = new DateTime(lastMonth.Year, lastMonth.Month, intStartDate);
            }
            else
            {
                startDate = new DateTime(wageProcess.WAG_Month.Year, wageProcess.WAG_Month.Month, intStartDate);
            }
            endDate = new DateTime(wageProcess.WAG_Month.Year, wageProcess.WAG_Month.Month, GetNumberFromString(secondRow.GetCell(cellCount - 1).ToString()));
            for (int i = (sheet.FirstRowNum + 5); i <= sheet.LastRowNum; i += 2)
            {
                IRow row = sheet.GetRow(i);
                IRow rowExtra = sheet.GetRow(i + 1);
                if (row == null) continue;
                if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;

                int EMP_Id = Convert.ToInt16(row.GetCell(1).ToString());
                DateTime tmpDate = startDate;
                for (int j = (row.FirstCellNum + 4); j <= row.LastCellNum - 1; j++)
                {
                    Attendance att = new Attendance();
                    att.EMP_Id = EMP_Id;
                    att.WAG_Id = wageProcess.WAG_Id;
                    att.CLI_Id = client.CLI_Id;
                    att.DES_Id = designationManager.getDesignationIdForAttandance(client.CLI_Id, EMP_Id, ProjectUtils.DateToDateTime(wageProcess.WAG_Month));
                    att.ATT_Date = ProjectUtils.DateTimeToDate(tmpDate);

                    switch (row.GetCell(j).ToString().Replace(" ", ""))
                    {
                        case ("G"):
                            att.ATT_IsPresent = true;
                            att.ATT_Shift = row.GetCell(j).ToString();
                            break;
                        case "I":
                            att.ATT_IsPresent = true;
                            att.ATT_Shift = row.GetCell(j).ToString();
                            break;
                        case "II":
                            att.ATT_IsPresent = true;
                            att.ATT_Shift = row.GetCell(j).ToString();
                            break;
                        case "III":
                            att.ATT_IsPresent = true;
                            att.ATT_Shift = row.GetCell(j).ToString();
                            break;
                        case "PL":
                            att.ATT_IsPresent = false;
                            att.ATT_IsEarnLeave = true;
                            break;
                        case "WO":
                            att.ATT_IsPresent = false;
                            att.ATT_IsWeeklyOff = true;
                            break;
                        case "W/O":
                            att.ATT_IsPresent = false;
                            att.ATT_IsWeeklyOff = true;
                            break;
                        case "PW":
                            att.ATT_IsPresent = true;
                            att.ATT_IsWeeklyOff = true;
                            break;
                        case "CO":
                            att.ATT_IsPresent = false;
                            att.ATT_IsEarnLeave = true;
                            break;
                        case "C/O":
                            att.ATT_IsPresent = false;
                            att.ATT_IsEarnLeave = true;
                            break;
                        case "HO":
                            att.ATT_IsPresent = false;
                            att.ATT_IsPublicHoliday = true;
                            break;
                        case "PO":
                            att.ATT_IsPresent = true;
                            att.ATT_IsPublicHoliday = true;
                            break;
                        case "NH":
                            att.ATT_IsPresent = false;
                            att.ATT_IsPublicHoliday = true;
                            break;
                        case "PN":
                            att.ATT_IsPresent = true;
                            att.ATT_IsPublicHoliday = true;
                            break;
                        case ("G/2"):
                            att.ATT_IsPresent = true;
                            att.ATT_IsHalfday = true;
                            break;
                        case "I/2":
                            att.ATT_IsPresent = true;
                            att.ATT_IsHalfday = true;
                            att.ATT_Shift = row.GetCell(j).ToString();
                            break;
                        case "II/2":
                            att.ATT_IsPresent = true;
                            att.ATT_IsHalfday = true;
                            att.ATT_Shift = row.GetCell(j).ToString();
                            break;
                        case "III/2":
                            att.ATT_IsPresent = true;
                            att.ATT_IsHalfday = true;
                            att.ATT_Shift = row.GetCell(j).ToString();
                            break;
                        case "HF":
                            att.ATT_IsPresent = true;
                            att.ATT_IsHalfday = true;
                            att.ATT_Shift = row.GetCell(j).ToString();
                            break;
                        case "A":
                            att.ATT_IsPresent = false;
                            break;
                        case "CP":
                            att.ATT_IsPresent = true;
                            att.ATT_NightShift = true;
                            break;
                        case "NC":
                            att.ATT_IsPresent = true;
                            att.ATT_IsPublicHoliday = true;
                            att.ATT_NightShift = true;
                            break;
                        case "CW":
                            att.ATT_IsPresent = true;
                            att.ATT_NightShift = true;
                            att.ATT_IsWeeklyOff = true;
                            break;
                        case "EL":
                            att.ATT_IsPresent = false;
                            att.ATT_IsEarnLeave = true;
                            break;
                        case "E/L":
                            att.ATT_IsPresent = false;
                            att.ATT_IsEarnLeave = true;
                            break;
                        case "PH":
                            att.ATT_IsPresent = false;
                            att.ATT_IsPublicHoliday = true;
                            break;
                    }

                    att.ATT_Orignal_Row1 = row.GetCell(j).ToString();
                    if (rowExtra.GetCell(j) != null)
                        if (!rowExtra.GetCell(j).ToString().Equals(""))
                        {
                            att.ATT_Orignal_Row2 = rowExtra.GetCell(j).ToString();
                            if (double.TryParse(rowExtra.GetCell(j).ToString(), out double value))
                            {
                                att.ATT_ExtraHoursWorked = value;
                            }

                        }

                    att.ATT_ImportedOn = DateTime.Now;
                    att.ADM_Id_ImportedBy = sessionUtils.GetLoggedAdminID();
                    attManager.save(att);
                    tmpDate = tmpDate.AddDays(1);
                }
            }
        }

        [NonAction]
        private void SaveAttendanceAndSummaries_ONEROW_WithoutShifts(ISheet sheet, Wage_Process wageProcess, Client client)
        {
            AttendanceSummaryManager attSummaryManager = new(_context);
            AttendanceManager attManager = new(_context);
            DesignationManager designationManager = new(_context);
            SessionUtils sessionUtils = new(Request, Response);

            IRow headerRow = sheet.GetRow(0), secondRow = sheet.GetRow(4), rowHeader = sheet.GetRow(1);
            int cellCount = secondRow.LastCellNum, intStartDate = GetNumberFromString(secondRow.GetCell(4).ToString());

            DateTime startDate = DateTime.Now, endDate = DateTime.Now;
            if (intStartDate > 1)
            {
                DateOnly lastMonth = wageProcess.WAG_Month.AddMonths(-1);
                startDate = new DateTime(lastMonth.Year, lastMonth.Month, intStartDate);
            }
            else
            {
                startDate = new DateTime(wageProcess.WAG_Month.Year, wageProcess.WAG_Month.Month, intStartDate);
            }
            endDate = new DateTime(wageProcess.WAG_Month.Year, wageProcess.WAG_Month.Month, GetNumberFromString(secondRow.GetCell(cellCount - 1).ToString()));

            List<Attendance_Summary> attSummaries = [];
            try
            {
                for (int i = (sheet.FirstRowNum + 5); i <= sheet.LastRowNum; i++)
                {
                    List<Attendance> atts = [];
                    DateTime tmpDate = startDate;
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;
                    if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;

                    int EMP_Id = Convert.ToInt16(row.GetCell(1).ToString());

                    double ATS_PresentDays = 0, ATS_WeekOff = 0, ATS_PublicHolidays = 0, ATS_EarnLeaves = 0, ATS_NightShifts = 0, ATS_ExtraHours = 0, ATS_HalfDays = 0;

                    for (int j = (row.FirstCellNum + 4); j <= row.LastCellNum - 1; j++)
                    {
                        if (row.GetCell(j) != null)
                        {
                            Attendance att = new()
                            {
                                EMP_Id = EMP_Id,
                                WAG_Id = wageProcess.WAG_Id,
                                CLI_Id = client.CLI_Id,
                                DES_Id = designationManager.getDesignationIdForAttandance(client.CLI_Id, EMP_Id, ProjectUtils.DateToDateTime(wageProcess.WAG_Month)),
                                ATT_Date = ProjectUtils.DateTimeToDate(tmpDate),
                                ATT_Orignal_Row1 = row.GetCell(j).ToString(),
                                ATT_Orignal_Row2 = "",
                                ATT_ImportedOn = DateTime.Now,
                                ADM_Id_ImportedBy = sessionUtils.GetLoggedAdminID(),
                                //ATT_Shift = "",
                                //ATT_ExtraHoursWorked = 0.0,
                                //ATT_IsEarnLeave = false
                            };

                            if (row.GetCell(j).ToString().Contains("PH"))
                            {
                                att.ATT_IsPublicHoliday = true;
                                switch (row.GetCell(j).ToString())
                                {
                                    case "P":
                                        att.ATT_IsPresent = true;
                                        ATS_PresentDays++;
                                        break;
                                    case "A":
                                        att.ATT_IsPresent = false;
                                        break;
                                    default:
                                        break;
                                }
                            }
                            else
                            {
                                switch (row.GetCell(j).ToString().Replace(" ", ""))
                                {
                                    case "P":
                                        att.ATT_IsPresent = true;
                                        ATS_PresentDays++;
                                        break;
                                    case "PL":
                                        att.ATT_IsPresent = false;
                                        att.ATT_IsEarnLeave = true;
                                        ATS_EarnLeaves++;
                                        break;
                                    case "WO":
                                        att.ATT_IsPresent = false;
                                        att.ATT_IsWeeklyOff = true;
                                        ATS_WeekOff++;
                                        break;
                                    case "W/O":
                                        att.ATT_IsPresent = false;
                                        att.ATT_IsWeeklyOff = true;
                                        ATS_WeekOff++;
                                        break;
                                    case "PW":
                                        att.ATT_IsPresent = true;
                                        att.ATT_IsWeeklyOff = true;
                                        ATS_PresentDays++;
                                        ATS_WeekOff++;
                                        break;
                                    case "CO":
                                        att.ATT_IsPresent = false;
                                        att.ATT_IsEarnLeave = true;
                                        ATS_EarnLeaves++;
                                        break;
                                    case "C/O":
                                        att.ATT_IsPresent = false;
                                        att.ATT_IsEarnLeave = true;
                                        ATS_EarnLeaves++;
                                        break;
                                    case "HO":
                                        att.ATT_IsPresent = false;
                                        att.ATT_IsPublicHoliday = true;
                                        ATS_PublicHolidays++;
                                        break;
                                    case "PO":
                                        att.ATT_IsPresent = true;
                                        att.ATT_IsPublicHoliday = true;
                                        ATS_PresentDays++;
                                        ATS_PublicHolidays++;
                                        break;
                                    case "NH":
                                        att.ATT_IsPresent = false;
                                        att.ATT_IsPublicHoliday = true;
                                        ATS_PublicHolidays++;
                                        break;
                                    case "PN":
                                        att.ATT_IsPresent = true;
                                        att.ATT_IsPublicHoliday = true;
                                        ATS_PresentDays++;
                                        ATS_PublicHolidays++;
                                        break;
                                    case "P/2":
                                        att.ATT_IsPresent = true;
                                        att.ATT_IsHalfday = true;
                                        ATS_PresentDays++;
                                        ATS_HalfDays++;
                                        break;
                                    case "HF":
                                        att.ATT_IsPresent = true;
                                        att.ATT_IsHalfday = true;
                                        ATS_PresentDays++;
                                        ATS_HalfDays++;
                                        break;
                                    case "A":
                                        att.ATT_IsPresent = false;
                                        break;
                                    case "CP":
                                        att.ATT_IsPresent = true;
                                        att.ATT_NightShift = true;
                                        ATS_PresentDays++;
                                        ATS_NightShifts++;
                                        break;
                                    case "NC":
                                        att.ATT_IsPresent = true;
                                        att.ATT_NightShift = true;
                                        att.ATT_IsPublicHoliday = true;
                                        ATS_PresentDays++;
                                        ATS_NightShifts++;
                                        ATS_PublicHolidays++;
                                        break;
                                    case "CW":
                                        att.ATT_IsPresent = true;
                                        att.ATT_NightShift = true;
                                        att.ATT_IsWeeklyOff = true;
                                        ATS_PresentDays++;
                                        ATS_NightShifts++;
                                        ATS_WeekOff++;
                                        break;
                                    case "EL":
                                        att.ATT_IsPresent = false;
                                        att.ATT_IsEarnLeave = true;
                                        ATS_EarnLeaves++;
                                        break;
                                    case "E/L":
                                        att.ATT_IsPresent = false;
                                        att.ATT_IsEarnLeave = true;
                                        ATS_EarnLeaves++;
                                        break;
                                    case "PH":
                                        att.ATT_IsPresent = false;
                                        att.ATT_IsPublicHoliday = true;
                                        ATS_PublicHolidays++;
                                        break;
                                }
                            }


                            atts.Add(att);
                            tmpDate = tmpDate.AddDays(1);
                        }
                    }

                    attSummaries.Add(new()
                    {
                        EMP_Id = EMP_Id,
                        WAG_Id = wageProcess.WAG_Id,
                        CLI_Id = client.CLI_Id,
                        DES_Id = designationManager.getDesignationIdForAttandance(client.CLI_Id, EMP_Id, ProjectUtils.DateToDateTime(wageProcess.WAG_Month)),
                        ATS_PresentDays = ATS_PresentDays,
                        ATS_HalfDays = ATS_HalfDays,
                        ATS_PublicHolidays = ATS_PublicHolidays,
                        ATS_WeekOff = ATS_WeekOff,
                        ATS_EarnLeaves = ATS_EarnLeaves,
                        ATS_NightShifts = ATS_NightShifts,
                        ATS_ExtraHours = ATS_ExtraHours,
                        ADM_Id_ImportedBy = sessionUtils.GetLoggedAdminID(),
                        ATS_ImportedOn = DateTime.Now,
                        ATS_TemplateReference = "ONEROW_WithoutShift"
                    });
                    attManager.Save(atts);  //ADD DAY WISE ATTENDANCES PER EMPLOYEES
                }
                attSummaryManager.Save(attSummaries);   //ADD ATTENDANCE SUMMARIES ALL EMPLOYEES
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion

        public ActionResult DeleteWageClientAttendanceList(int WAG_Id, int CLI_Id)
        {
            AttendanceManager attManager = new(_context);
            AttendanceSummaryManager AttSummaryManager = new(_context);

            attManager.deleteAllAttendanceofWageClient(WAG_Id, CLI_Id);
            AttSummaryManager.DeleteAllAttendanceOfWageClient(WAG_Id, CLI_Id);

            return RedirectToAction("WageAttendanceList", new { WAG_Id = WAG_Id });
        }
    }
    public class _EmpID
    {
        private int id;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }
    }
}