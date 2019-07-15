using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RMERP.DAL.ManagerClasses;
using RMERP.DAL.Models;
using RMERP.DAL.Mappers;
using RMERP.DAL.ViewModel;
using Microsoft.Extensions.Configuration;
using RMERP.DAL.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using static RMERP.DAL.Helpers.ProjectUtils;
using System.Text;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using RMERP.Helpers;
using SmartBreadcrumbs.Attributes;

namespace RMERP.Controllers
{
    [Authorize]
    public class AttendanceController : Controller
    {
        private readonly RMERPContext _context;
        public IConfiguration _configuration;
        private IHostingEnvironment _hostingEnvironment;
        public AttendanceController(RMERPContext context, IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }
        [Breadcrumb("Wage Attendance", FromAction = "Index", FromController = typeof(WageProcessController))]
        public ActionResult WageAttendanceList(int WAG_Id)
        {
            if (WAG_Id <= 0)
            {
                WAG_Id = WagId;
            }
            ClientsManager clientsManager = new ClientsManager(_context, _configuration);
            AttendanceManager attManager = new AttendanceManager(_context);
            WageProcessManager wageManager = new WageProcessManager(_context);
            Wage_Process wage = wageManager.getWageProcessById(WAG_Id);
            List<Clients> clients = clientsManager.GetActiveClientOfMonthByFirmId(wage.WAG_Month, wage.FRM_Id);
            WageProcessClientAttendancePageVM pageVM = new WageProcessClientAttendancePageVM();
            pageVM.wageProcess = WageProcessMapper.mapMe(wage);
            pageVM.lstClient = WageProcessMapper.mapClientToAttendanceWages(clients, wage, attManager.getAttendance_Wage(WAG_Id));
            return View(pageVM);
        }

        public ActionResult DeleteWageClientAttendanceList(int WAG_Id, int CLI_Id)
        {
            AttendanceManager attManager = new AttendanceManager(_context);
            attManager.deleteAllAttendanceofWageClient(WAG_Id, CLI_Id);
            return RedirectToAction("WageAttendanceList", new { WAG_Id = WAG_Id });
        }

        [Breadcrumb("Upload Attendance Excel", FromAction = "WageAttendanceList")]
        public ActionResult UploadExcel(int WAG_Id, int CLI_Id)
        {
            WAG_Id = (WAG_Id <= 0 ? WagId : WAG_Id);
            WagId = WAG_Id;
            CLI_Id = (CLI_Id <= 0 ? CliId : CLI_Id);
            ClientsManager clientManager = new ClientsManager(_context, _configuration);
            WageProcessManager wageManager = new WageProcessManager(_context);
            Wage_Process wageProcess = wageManager.getWageProcessById(WAG_Id);
            UploadExcelViewModel upvm = new UploadExcelViewModel();
            upvm.wageProcessVM = WageProcessMapper.mapMe(wageProcess);
            upvm.client = clientManager.GetClientById(CLI_Id);
            ViewBag.templateList = Enum.GetValues(typeof(ATTENDANCE_EXCEL_TEMPLATE));
            return View(upvm);
        }

        [HttpPost]
        [Breadcrumb("Verify Attendance", FromAction = "UploadExcel")]
        public ActionResult VerifyTemplate(UploadExcelViewModel uvm)
        {

            WageProcessManager wageManager = new WageProcessManager(_context);
            AttendanceManager attManager = new AttendanceManager(_context);
            EmployeeManager empManager = new EmployeeManager(_context);
            ClientsManager clientsManager = new ClientsManager(_context, _configuration);
            Wage_Process wageProcess = wageManager.getWageProcessById(uvm.wageProcessVM.WAG_Id);
            IFormFile file = uvm.ExcelFile;
            ExcelViewModel excelViewModel = new ExcelViewModel();
            List<Employees> empListExtraInExcel = new List<Employees>();
            List<Employees> empListExtraInDb = new List<Employees>();
            List<Attendance> attandanceList = new List<Attendance>();
            string newPath = ProjectUtils.GetTempFolderPath(_hostingEnvironment.WebRootPath);
            StringBuilder sb = new StringBuilder();
            if (file != null)
            {
                if (file.Length > 0)
                {
                    Clients client = clientsManager.GetClientById(uvm.client.CLI_Id);
                    CliId = uvm.client.CLI_Id;
                    string sFileExtension = Path.GetExtension(file.FileName).ToLower();
                    ISheet sheet;
                    string fullPath = Path.Combine(newPath, ProjectUtils.GetTempFileName() + sFileExtension);
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
                    excelViewModel.WAG_Id = uvm.wageProcessVM.WAG_Id;
                    excelViewModel.CLI_Id = uvm.client.CLI_Id;

                    /***************** CHECK DATETIME MATCH ***************************/
                    DateTime[] arr = DateHelper.getStartEndDatePeriodForAttendance(client, wageProcess.WAG_Month);
                    if (excelViewModel.startDate.Date != arr[0].Date || excelViewModel.endDate.Date != arr[1].Date)
                    {
                        excelViewModel.datePeriod = false;
                    }
                    /***************** CHECK DATETIME MATCH ***************************/
                    /***************** CHECK EXTRA EMPLOYEES IN EXCEL ***************************/
                    List<_EmpID> _empID = new List<_EmpID>();
                    //List<Clients_Employees> assignEmployeeList = attManager.assignEmployeeList(client.CLI_Id);
                    IEnumerable<Clients_Employees> assignEmployeeList = clientsManager.listActiveClientsEmployees(client.CLI_Id, wageProcess.WAG_Month);
                    foreach (ExcelRowViewModel row in excelViewModel.excelRows)
                    {
                        Clients_Employees emp = assignEmployeeList.Where(m => m.EMP_Id.Equals(Convert.ToInt32(row.EMP_Id))).FirstOrDefault();
                        if (emp == null)
                        {
                            Employees empExtra = new Employees();
                            empExtra.EMP_Id = Convert.ToInt32(row.EMP_Id);
                            empExtra.EMP_FirstName = row.EMP_Name;
                            empListExtraInExcel.Add(empExtra);
                        }
                        else
                        {
                            _EmpID EmpID = new _EmpID();
                            EmpID.Id = emp.EMP_Id;
                            _empID.Add(EmpID);
                        }
                    }
                    /***************** CHECK EXTRA EMPLOYEES IN EXCEL ***************************/
                    /***************** CHECK EMPLOYEES REMAINING EXCEL ***************************/
                    HashSet<int> diffids = new HashSet<int>(_empID.Select(s => s.Id));
                    var EmployeeIdList = assignEmployeeList.Where(m => !diffids.Contains(m.EMP_Id)).ToList();
                    foreach (var EMP_Id in EmployeeIdList)
                    {
                        empListExtraInDb.Add(empManager.GetEmployeeById(Convert.ToInt32(EMP_Id.EMP_Id)));
                    }
                    excelViewModel.empListExtraInExcel = empListExtraInExcel;
                    excelViewModel.EmpListExtraInDb = empListExtraInDb;
                    if (empListExtraInExcel.Count > 0 || empListExtraInDb.Count > 0 || excelViewModel.datePeriod == false)
                    {
                        excelViewModel.btnExportToDatabase = false;
                    }
                    /***************** CHECK EMPLOYEES REMAINING EXCEL ***************************/
                }
            }
            return View(excelViewModel);
        }

        public ExcelViewModel GetAttendance_BASIC_WithoutShifts(ISheet sheet, Wage_Process wageProcess, Clients client)
        {
            ExcelViewModel excelViewModel = new ExcelViewModel();
            List<Attendance> attandanceList = new List<Attendance>();
            List<ExcelRowViewModel> rows = new List<ExcelRowViewModel>();
            IRow headerRow = sheet.GetRow(0);
            IRow secondRow = sheet.GetRow(1);
            int cellCount = secondRow.LastCellNum;
            int intStartDate = Convert.ToInt16(secondRow.GetCell(4).ToString());
            int TotEmp = 0;
            DateTime startDate = DateTime.Now, endDate = DateTime.Now;
            if (intStartDate > 1)
            {
                DateTime lastMonth = wageProcess.WAG_Month.AddMonths(-1);
                startDate = new DateTime(lastMonth.Year, lastMonth.Month, intStartDate);
            }
            else
            {
                startDate = new DateTime(wageProcess.WAG_Month.Year, wageProcess.WAG_Month.Month, intStartDate);
            }
            endDate = new DateTime(wageProcess.WAG_Month.Year, wageProcess.WAG_Month.Month, Convert.ToInt16(secondRow.GetCell(cellCount - 1).ToString()));
            int totalPublicHolidays = 0;
            //for (int j = (secondRow.FirstCellNum + 4); j <= secondRow.LastCellNum - 1; j++)
            //{
            //    if (secondRow.GetCell(j).ToString().Contains("PH"))
            //        totalPublicHolidays++;
            //}

            for (int i = (sheet.FirstRowNum + 2); i <= sheet.LastRowNum; i += 2)
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
                    attendance.ATT_Date = tmpDate;
                    tmpDate = tmpDate.AddDays(1);

                    switch (row.GetCell(j).ToString())
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
            return excelViewModel;
        }

        public ExcelViewModel GetAttendance_BASIC_WithShifts(ISheet sheet, Wage_Process wageProcess, Clients client)
        {
            ExcelViewModel excelViewModel = new ExcelViewModel();
            List<Attendance> attandanceList = new List<Attendance>();
            List<ExcelRowViewModel> rows = new List<ExcelRowViewModel>();
            IRow headerRow = sheet.GetRow(0);
            IRow secondRow = sheet.GetRow(1);
            int cellCount = secondRow.LastCellNum;
            int intStartDate = Convert.ToInt16(secondRow.GetCell(4).ToString());
            int totalPublicHolidays = 0;

            int TotEmp = 0;
            DateTime startDate = DateTime.Now, endDate = DateTime.Now;
            if (intStartDate > 1)
            {
                DateTime lastMonth = wageProcess.WAG_Month.AddMonths(-1);
                startDate = new DateTime(lastMonth.Year, lastMonth.Month, intStartDate);
            }
            else
            {
                startDate = new DateTime(wageProcess.WAG_Month.Year, wageProcess.WAG_Month.Month, intStartDate);
            }
            endDate = new DateTime(wageProcess.WAG_Month.Year, wageProcess.WAG_Month.Month, Convert.ToInt16(secondRow.GetCell(cellCount - 1).ToString()));
            for (int i = (sheet.FirstRowNum + 2); i <= sheet.LastRowNum; i += 2)
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
                    attendance.ATT_Date = tmpDate;
                    tmpDate = tmpDate.AddDays(1);
                    //if (row.GetCell(j).ToString().Equals("G") || row.GetCell(j).ToString().Equals("I") || row.GetCell(j).ToString().Equals("II") || row.GetCell(j).ToString().Equals("III"))
                    //{
                    //    attendance.ATT_IsPresent = true;
                    //    totalPresence++;
                    //    attendance.ATT_Shift = row.GetCell(j).ToString();
                    //}

                    switch (row.GetCell(j).ToString())
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

                // excelRow.totalPaybleDays= totalPresence;
                rows.Add(excelRow);

                totalPublicHolidays += totalPublicHoliday;
            }
            excelViewModel.listAttendance = attandanceList;
            excelViewModel.excelRows = rows;
            excelViewModel.totalEmployees = TotEmp;
            excelViewModel.startDate = startDate;
            excelViewModel.endDate = endDate;
            excelViewModel.totalPublicHolidays = totalPublicHolidays;

            return excelViewModel;
        }

        public ExcelViewModel GetAttendance_OneRow_WithoutShift(ISheet sheet, Wage_Process wageProcess, Clients client)
        {
            ExcelViewModel excelViewModel = new ExcelViewModel();
            List<Attendance> attandanceList = new List<Attendance>();
            List<ExcelRowViewModel> rows = new List<ExcelRowViewModel>();
            IRow headerRow = sheet.GetRow(0);
            IRow secondRow = sheet.GetRow(1);
            int cellCount = secondRow.LastCellNum;
            int intStartDate = Convert.ToInt16(secondRow.GetCell(4).ToString());
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
                DateTime lastMonth = wageProcess.WAG_Month.AddMonths(-1);
                startDate = new DateTime(lastMonth.Year, lastMonth.Month, intStartDate);
            }
            else
            {
                startDate = new DateTime(wageProcess.WAG_Month.Year, wageProcess.WAG_Month.Month, intStartDate);
            }
            endDate = new DateTime(wageProcess.WAG_Month.Year, wageProcess.WAG_Month.Month, Convert.ToInt16(secondRow.GetCell(cellCount - 1).ToString()));

            IRow rowHeader = sheet.GetRow(1);
            for (int i = (sheet.FirstRowNum + 2); i <= sheet.LastRowNum; i++)
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
                    attendance.ATT_Date = tmpDate;
                    tmpDate = tmpDate.AddDays(1);

                    if (rowHeader.GetCell(j).ToString().Contains("PH"))
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
                        switch (row.GetCell(j).ToString())
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
            return excelViewModel;
        }
        [HttpPost]
        public ActionResult ImportExcel(IFormCollection frm)
        {
            int WAG_Id = Convert.ToInt32(frm["WAG_Id"]);
            int CLI_Id = Convert.ToInt32(frm["CLI_Id"]);
            string strFilePath = frm["fileName"];
            ClientsManager clientManager = new ClientsManager(_context, _configuration);
            WageProcessManager wageManager = new WageProcessManager(_context);
            Wage_Process wageProcess = wageManager.getWageProcessById(WAG_Id);
            Clients client = clientManager.GetClientById(CLI_Id);
            StringBuilder sb = new StringBuilder();
            ISheet sheet;
            using (var stream = new FileStream(strFilePath, FileMode.Open))
            {
                string sFileExtension = Path.GetExtension(strFilePath).ToLower();
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
                switch (frm["Template"])
                {
                    case "0":
                        saveAttendance_BASIC_WithoutShifts(sheet, wageProcess, client);
                        break;
                    case "1":
                        saveAttendance_BASIC_WithShifts(sheet, wageProcess, client);
                        break;
                    case "2":
                        saveAttendance_ONEROW_WithoutShift(sheet, wageProcess, client);
                        break;
                }
                new FileInfo(strFilePath).Delete();
            }
            return RedirectToAction("WageAttendanceList", new { WAG_Id = WAG_Id });
        }

        public void saveAttendance_BASIC_WithoutShifts(ISheet sheet, Wage_Process wageProcess, Clients client)
        {
            AttendanceManager attManager = new AttendanceManager(_context);
            DesignationManager designationManager = new DesignationManager(_context);
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            IRow headerRow = sheet.GetRow(0);
            IRow secondRow = sheet.GetRow(1);
            int cellCount = secondRow.LastCellNum;
            int intStartDate = Convert.ToInt16(secondRow.GetCell(4).ToString());
            DateTime startDate = DateTime.Now, endDate = DateTime.Now;
            if (intStartDate > 1)
            {
                DateTime lastMonth = wageProcess.WAG_Month.AddMonths(-1);
                startDate = new DateTime(lastMonth.Year, lastMonth.Month, intStartDate);
            }
            else
            {
                startDate = new DateTime(wageProcess.WAG_Month.Year, wageProcess.WAG_Month.Month, intStartDate);
            }
            endDate = new DateTime(wageProcess.WAG_Month.Year, wageProcess.WAG_Month.Month, Convert.ToInt16(secondRow.GetCell(cellCount - 1).ToString()));
            for (int i = (sheet.FirstRowNum + 2); i <= sheet.LastRowNum; i += 2)
            {
                IRow row = sheet.GetRow(i);
                IRow rowExtra = sheet.GetRow(i + 1);
                if (row == null) continue;
                if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;

                int EMP_Id = Convert.ToInt16(row.GetCell(1).ToString());
                DateTime tmpDate = startDate;
                for (int j = (row.FirstCellNum + 4); j <= row.LastCellNum - 1; j++)
                {
                    if (secondRow.GetCell(j) != null)
                    {
                        Attendance att = new Attendance();
                        att.EMP_Id = EMP_Id;
                        att.WAG_Id = wageProcess.WAG_Id;
                        att.CLI_Id = client.CLI_Id;
                        att.DES_Id = designationManager.getDesignationIdForAttandance(client.CLI_Id, EMP_Id);
                        att.ATT_Date = tmpDate;
                        //if (row.GetCell(j).ToString().Equals("P"))
                        //    att.ATT_IsPresent = true;
                        //else if (row.GetCell(j).ToString().Equals("P/2"))
                        //    att.ATT_IsHalfday = true;
                        //else if (row.GetCell(j).ToString().Equals("A"))
                        //    att.ATT_IsPresent = false;
                        //else if (row.GetCell(j).ToString().Equals("W/O"))
                        //{
                        //    att.ATT_IsWeeklyOff = true;
                        //    att.ATT_IsPresent = false;
                        //}
                        //att.ATT_IsPublicHoliday = secondRow.GetCell(j).ToString().Contains("PH");
                        //// att.ATT_IsPaidLeave = secondRow.GetCell(j).ToString().Contains("PL");
                        //if (rowExtra.GetCell(j) != null)
                        //    if (!rowExtra.GetCell(j).ToString().Equals(""))
                        //        att.ATT_ExtraHoursWorked = Convert.ToDouble(rowExtra.GetCell(j).ToString());
                        //att.ATT_IsEarnLeave = false;
                        ////att.ATT_IsWeeklyOff = false;                     
                        switch (row.GetCell(j).ToString())
                        {
                            case "P":
                                att.ATT_IsPresent = true;
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
                            case "P/2":
                                att.ATT_IsPresent = true;
                                att.ATT_IsHalfday = true;
                                break;
                            case "HF":
                                att.ATT_IsPresent = true;
                                att.ATT_IsHalfday = true;
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
                                att.ATT_NightShift = true;
                                att.ATT_IsPublicHoliday = true;
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
        }

        public void saveAttendance_BASIC_WithShifts(ISheet sheet, Wage_Process wageProcess, Clients client)
        {
            AttendanceManager attManager = new AttendanceManager(_context);
            DesignationManager designationManager = new DesignationManager(_context);
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            IRow headerRow = sheet.GetRow(0);
            IRow secondRow = sheet.GetRow(1);
            int cellCount = secondRow.LastCellNum;
            int intStartDate = Convert.ToInt16(secondRow.GetCell(4).ToString());
            DateTime startDate = DateTime.Now, endDate = DateTime.Now;
            if (intStartDate > 1)
            {
                DateTime lastMonth = wageProcess.WAG_Month.AddMonths(-1);
                startDate = new DateTime(lastMonth.Year, lastMonth.Month, intStartDate);
            }
            else
            {
                startDate = new DateTime(wageProcess.WAG_Month.Year, wageProcess.WAG_Month.Month, intStartDate);
            }
            endDate = new DateTime(wageProcess.WAG_Month.Year, wageProcess.WAG_Month.Month, Convert.ToInt16(secondRow.GetCell(cellCount - 1).ToString()));
            for (int i = (sheet.FirstRowNum + 2); i <= sheet.LastRowNum; i += 2)
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
                    att.DES_Id = designationManager.getDesignationIdForAttandance(client.CLI_Id, EMP_Id);
                    att.ATT_Date = tmpDate;
                    //if (row.GetCell(j).ToString().Equals("A"))
                    //    att.ATT_IsPresent = false;
                    //else if (row.GetCell(j).ToString().Equals("W/O"))
                    //{
                    //    att.ATT_IsWeeklyOff = true;
                    //    att.ATT_IsPresent = false;
                    //}
                    //else if (row.GetCell(j).ToString().Equals("G") || row.GetCell(j).ToString().Equals("I") || row.GetCell(j).ToString().Equals("II") || row.GetCell(j).ToString().Equals("III"))
                    //{
                    //    att.ATT_IsPresent = true;
                    //    att.ATT_Shift = row.GetCell(j).ToString();
                    //}
                    //else if (row.GetCell(j).ToString().Equals("G/2") || row.GetCell(j).ToString().Equals("I/2") || row.GetCell(j).ToString().Equals("II/2") || row.GetCell(j).ToString().Equals("III/2"))
                    //{
                    //    att.ATT_IsHalfday = true;
                    //    att.ATT_Shift = row.GetCell(j).ToString().Split("/2")[0];
                    //}
                    //#region by rinku 11 july
                    //else if (row.GetCell(j).ToString().Equals("EL"))
                    //    att.ATT_IsEarnLeave = true;
                    //else if (row.GetCell(j).ToString().Equals("PH"))
                    //    att.ATT_IsPublicHoliday = true;
                    //else if (row.GetCell(j).ToString().Equals("PL"))
                    //    //  att.ATT_IsPaidLeave = true;
                    //    #endregion
                    //    if (rowExtra.GetCell(j) != null)
                    //        if (!rowExtra.GetCell(j).ToString().Equals(""))
                    //        {
                    //            att.ATT_ExtraHoursWorked = Convert.ToDouble(rowExtra.GetCell(j).ToString());
                    //        }

                    switch (row.GetCell(j).ToString())
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

        public void saveAttendance_ONEROW_WithoutShift(ISheet sheet, Wage_Process wageProcess, Clients client)
        {
            AttendanceManager attManager = new AttendanceManager(_context);
            DesignationManager designationManager = new DesignationManager(_context);
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            IRow headerRow = sheet.GetRow(0);
            IRow secondRow = sheet.GetRow(1);
            int cellCount = secondRow.LastCellNum;
            int intStartDate = Convert.ToInt16(secondRow.GetCell(4).ToString());
            DateTime startDate = DateTime.Now, endDate = DateTime.Now;
            if (intStartDate > 1)
            {
                DateTime lastMonth = wageProcess.WAG_Month.AddMonths(-1);
                startDate = new DateTime(lastMonth.Year, lastMonth.Month, intStartDate);
            }
            else
            {
                startDate = new DateTime(wageProcess.WAG_Month.Year, wageProcess.WAG_Month.Month, intStartDate);
            }
            endDate = new DateTime(wageProcess.WAG_Month.Year, wageProcess.WAG_Month.Month, Convert.ToInt16(secondRow.GetCell(cellCount - 1).ToString()));
            IRow rowHeader = sheet.GetRow(1);
            for (int i = (sheet.FirstRowNum + 2); i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                if (row == null) continue;
                if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;

                int EMP_Id = Convert.ToInt16(row.GetCell(1).ToString());
                DateTime tmpDate = startDate;
                for (int j = (row.FirstCellNum + 4); j <= row.LastCellNum - 1; j++)
                {
                    if (secondRow.GetCell(j) != null)
                    {
                        Attendance att = new Attendance();
                        att.EMP_Id = EMP_Id;
                        att.WAG_Id = wageProcess.WAG_Id;
                        att.CLI_Id = client.CLI_Id;
                        att.DES_Id = designationManager.getDesignationIdForAttandance(client.CLI_Id, EMP_Id);
                        att.ATT_Date = tmpDate;
                        //att.ATT_Shift = "";
                        //att.ATT_ExtraHoursWorked = 0.0;
                        //att.ATT_IsEarnLeave = false;
                        if (rowHeader.GetCell(j).ToString().Contains("PH"))
                        {
                            att.ATT_IsPublicHoliday = true;
                            switch (row.GetCell(j).ToString())
                            {
                                case "P":
                                    att.ATT_IsPresent = true;
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
                            switch (row.GetCell(j).ToString())
                            {
                                case "P":
                                    att.ATT_IsPresent = true;
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
                                case "P/2":
                                    att.ATT_IsPresent = true;
                                    att.ATT_IsHalfday = true;
                                    break;
                                case "HF":
                                    att.ATT_IsPresent = true;
                                    att.ATT_IsHalfday = true;
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
                                    att.ATT_NightShift = true;
                                    att.ATT_IsPublicHoliday = true;
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
                        }


                        att.ATT_Orignal_Row1 = row.GetCell(j).ToString();
                        att.ATT_Orignal_Row2 = "";
                        att.ATT_ImportedOn = DateTime.Now;
                        att.ADM_Id_ImportedBy = sessionUtils.GetLoggedAdminID();
                        attManager.save(att);
                        tmpDate = tmpDate.AddDays(1);
                    }
                }
            }
        }
        [HttpGet]
        [Breadcrumb("View Attendance", FromAction = "WageAttendanceList")]
        public ActionResult ViewAttendance(int WAG_Id, int CLI_Id)
        {
            WagId = WAG_Id;
            ClientsManager clientsManager = new ClientsManager(_context, _configuration);
            AttendanceManager attendanceManager = new AttendanceManager(_context);
            WageProcessManager wagManager = new WageProcessManager(_context);
            Wage_Process wage = wagManager.getWageProcessById(WAG_Id);
            ViewBag.WAG_Month = wage.WAG_Month.ToString("MMMM") + "-" + wage.WAG_Month.ToString("yyyy");
            Clients client = clientsManager.GetClientById(CLI_Id);
            ViewBag.ClientName = client.CLI_Name;
            ViewBag.CLI_Id = client.CLI_Id;
            ViewBag.CLI_Total_WorkingDays = client.CLI_Total_WorkingDays;
            DateTime[] arrDate = DateHelper.getStartEndDatePeriodForAttendance(client, wage.WAG_Month);
            ViewBag.startDate = arrDate[0];
            ViewBag.endDate = arrDate[1];
            List<AttendanceVM> list = AttendanceMapper.mapAttendances(attendanceManager.getAttendance_Wage_Client(WAG_Id, CLI_Id));
            return View(new Tuple<List<AttendanceVM>, Clients>(list, client));
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