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
using RMERP.DAL.Mappers;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using RMERP.DAL.App_Code;
using SmartBreadcrumbs.Attributes;
using static RMERP.DAL.Helpers.ProjectUtils;

namespace RMERP.Controllers
{
    [Authorize]
    public class WageProcessController : Controller
    {
        private readonly RMERPContext _context;
        public IConfiguration _configuration;
        WageProcessManager wpm;
        private IHostingEnvironment _hostingEnvironment;
       
        public WageProcessController(RMERPContext context, IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _configuration = configuration;
            wpm = new WageProcessManager(context);
            _hostingEnvironment = hostingEnvironment;
        }


        [Breadcrumb("Choose Firm")]
        public IActionResult ChooseFirm()
        {
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            if (sessionUtils.GetLoggedFirmID().HasValue)
            {
                return RedirectToAction("Index", new { FRM_Id = sessionUtils.GetLoggedFirmID().Value });
            }
            else
            {
                FirmsManager frmManager = new FirmsManager(_context);
                return View(frmManager.getFirmList());
            }
        }
        

        [Breadcrumb("Wage Process")]
        public IActionResult Index(int FRM_Id)
        {
            if (FRM_Id <= 0)
            {
                FRM_Id = Frm_Id;
            }
            else
            {
                Frm_Id = FRM_Id;
            }
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            DateTime nextMonth = wpm.nextWageMonth(sessionUtils.GetLoggedAdminID(),FRM_Id);
            WageProcessManager wageProcessManager = new WageProcessManager(_context);
            ViewBag.month = nextMonth.ToString("MMMM", CultureInfo.CreateSpecificCulture("IN"));
            ViewBag.FRM_Id = FRM_Id;
            IEnumerable<Wage_Process> wage_Processes = wpm.getWageProcessList(FRM_Id);
            return View(WageProcessMapper.mapMeVMs(wage_Processes, FRM_Id, _context, _configuration));
         }
        public IActionResult CreateNextMonthWage(int FRM_Id)
        {
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            string rse = wpm.CreateNextMonthWage(sessionUtils.GetLoggedAdminID(), FRM_Id);
            return RedirectToAction("Index", new { FRM_Id = FRM_Id});
        }
        public IActionResult DeleteWageProcess(int WagId)
        {
            WageProcessManager wpm = new WageProcessManager(_context);
            int FRM_Id = wpm.getWageProcessById(WagId).FRM_Id;
            string res = wpm.DeleteWageProcess(WagId);
            if (res != string.Empty)
            {
                TempData["message"] = "Wage Process can not Deleted";
            }
            return RedirectToAction("Index", new { FRM_Id = FRM_Id });
        }
        [HttpGet]
        public IActionResult nextWageMonth(int FRM_Id)
        {
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            DateTime nextMonth = wpm.nextWageMonth(sessionUtils.GetLoggedAdminID(), FRM_Id);
            return Content(nextMonth.ToString("MMMM", CultureInfo.CreateSpecificCulture("IN")));
        }
       // [Breadcrumb("WageProcess List")]
        //public ActionResult WageProcessList()
        //{
        //    SessionUtils sessionUtils = new SessionUtils(Request, Response);

        //    var model = wpm.getWageProcessList();
        //    return PartialView("_WageProcessList", model);
        //}
        
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

        [Breadcrumb("Edit Attendance Record", FromAction = "UploadExcel", FromController = typeof(AttendanceController))]
        public ActionResult EditAttendanceRecord(int attID)
        {
            AttendanceListViewModel alvm = new AttendanceListViewModel();
            alvm.attendanceVM = new AttendanceVM();
           
            Attendance att = new Attendance();
            if (attID > 0)
            {
                
                att = wpm.GetAttendanceById(attID);
                CliId = att.CLI_Id;
                alvm.attendanceVM.ATT_Id = attID;
                alvm.attendanceVM.DES_Id = att.DES_Id;
                alvm.attendanceVM.CLI_Id = att.CLI_Id;
                alvm.attendanceVM.EMP_Id = att.EMP_Id;
                alvm.attendanceVM.WAG_Id = att.WAG_Id;
                alvm.attendanceVM.ATT_ImportedOn = att.ATT_ImportedOn;
                alvm.attendanceVM.ATT_IsEarnLeave = att.ATT_IsEarnLeave;
                alvm.attendanceVM.ATT_IsPublicHoliday = att.ATT_IsPublicHoliday;
                alvm.attendanceVM.ATT_IsPresent = att.ATT_IsPresent;
                alvm.attendanceVM.ATT_IsWeeklyOff = att.ATT_IsWeeklyOff;
                alvm.attendanceVM.ATT_Shift = att.ATT_Shift;
                alvm.attendanceVM.ATT_Date = att.ATT_Date;        
                alvm.attendanceVM.ADM_Id_ImportedBy = att.ADM_Id_ImportedBy;
                alvm.attendanceVM.ATT_ExtraHoursWorked = att.ATT_ExtraHoursWorked;
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
                CliId = atta.CLI_Id;
                atta.ATT_Id = alvm.attendanceVM.ATT_Id;
                atta.ATT_ImportedOn = alvm.attendanceVM.ATT_ImportedOn;
                atta.ATT_IsEarnLeave = alvm.attendanceVM.ATT_IsEarnLeave;
                atta.ATT_IsPublicHoliday = alvm.attendanceVM.ATT_IsPublicHoliday;
                atta.ATT_IsPresent = alvm.attendanceVM.ATT_IsPresent;
                atta.ATT_IsWeeklyOff = alvm.attendanceVM.ATT_IsWeeklyOff;
                atta.ATT_Date = alvm.attendanceVM.ATT_Date;
                atta.ADM_Id_ImportedBy = alvm.attendanceVM.ADM_Id_ImportedBy;
                atta.ATT_Shift = alvm.attendanceVM.ATT_Shift;
                atta.CLI_Id = alvm.attendanceVM.CLI_Id;
                atta.EMP_Id = alvm.attendanceVM.EMP_Id;
                atta.DES_Id = alvm.attendanceVM.DES_Id;
                atta.WAG_Id = alvm.attendanceVM.WAG_Id;
                atta.ATT_ExtraHoursWorked = alvm.attendanceVM.ATT_ExtraHoursWorked;

                res = wpm.UpdateAttendance(atta);
            }
            

            return RedirectToAction("ViewAttendance", "Attendance", new { WAG_Id = alvm.attendanceVM.WAG_Id, CLI_Id = alvm.attendanceVM.CLI_Id });
        }
        public ActionResult WageRegisterStatus(int WAG_Id)
        {
            string res = string.Empty;
            ClientsManager clientsManager = new ClientsManager(_context, _configuration);
            Wage_Process wage=wpm.getWageProcessById(WAG_Id);
            res = wpm.WageRegisterStatus(wage);            
            if (res != "")
            {
                TempData["message"] = "Wage Process status is not changed!";                
            }
            return RedirectToAction("Index", new { FRM_Id = wage.FRM_Id });
        }

    }

}