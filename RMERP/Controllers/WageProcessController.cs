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

namespace RMERP.Controllers
{
    [Authorize]
    public class WageProcessController : Controller
    {
        private readonly RMERPContext _context;
        public IConfiguration Configuration;
        WageProcessManager wpm;
        private IHostingEnvironment _hostingEnvironment;
       
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
      
        
        //[HttpGet]
        //public ActionResult EditAttendance(int empId)
        //{
        //    AttendanceListViewModel alvm = new AttendanceListViewModel();
        //    alvm.attendanceViewModel = new AttendanceViewModel();        

        //   // List<Attendance> list = wpm.GetAttendanceList(WagID, clientID, empId);          
        //    //alvm.attendancesList = list;
        //    //alvm.EmpID = empId;
        //    //alvm.EmpName = list.FirstOrDefault().EMP_.EMP_FirstName;
        //    //alvm.EmpDesignation = list.FirstOrDefault().EMP_.EMP_Designation;
        //    return View(alvm);
        //}
        //[HttpPost]
        //public ActionResult EditAttendance(AttendanceListViewModel alvm)
        //{
        //    string res = string.Empty;
        //    foreach(var item in alvm.attendancesList)
        //    {
        //        Attendance attendance = new Attendance();
        //        attendance.ATT_Id = item.ATT_Id;
        //        attendance.ATT_IsPresent = item.ATT_IsPresent;
        //        attendance.ATT_IsWeeklyOff = item.ATT_IsWeeklyOff;
        //        attendance.ATT_ExtraHoursWorked = item.ATT_ExtraHoursWorked;
        //        res=wpm.UpdateAttendance(attendance);
        //    }

        //    return RedirectToAction("ViewAttendance","Attendance", new { WAG_Id = WagID , CLI_Id = clientID });
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

        public ActionResult EditAttendanceRecord(int attID)
        {
            AttendanceListViewModel alvm = new AttendanceListViewModel();
            alvm.attendanceViewModel = new AttendanceViewModel();
           
            Attendance att = new Attendance();
            if (attID > 0)
            {
                att = wpm.GetAttendanceById(attID);
                alvm.attendanceViewModel.ATT_Id = attID;
                alvm.attendanceViewModel.DES_Id = att.DES_Id;
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
                atta.DES_Id = alvm.attendanceViewModel.DES_Id;
                atta.WAG_Id = alvm.attendanceViewModel.WAG_Id;
                atta.ATT_ExtraHoursWorked = alvm.attendanceViewModel.ATT_ExtraHoursWorked;

                res = wpm.UpdateAttendance(atta);
            }
            

            return RedirectToAction("ViewAttendance", "Attendance", new { WAG_Id = alvm.attendanceViewModel.WAG_Id, CLI_Id = alvm.attendanceViewModel.CLI_Id });
        }

        public ActionResult WageRegister(int WAG_Id=-1)
        {
            WageRegisterVM avm = new WageRegisterVM();           
            avm = GetWage_RegisterData(WAG_Id);
            avm.listWageProcessClients = wpm.GetWage_Process_Clients(WAG_Id);
            return View(avm);
            
        }
        [HttpPost]
        public ActionResult ResetWageRegister(int WAG_Id, string item_CLI_Id)
        {
            string res= wpm.ResetWageRegister(WAG_Id, item_CLI_Id);
            if (res != string.Empty)
            {
                TempData["message"] = "Wage Register is not saved!";
            }
            return RedirectToAction("WageRegister", new { WAG_Id = WAG_Id });

        }
        [HttpPost]
        public ActionResult SaveWageRegister(int WAG_Id, string item_CLI_Id)
        {
            WageRegisterVM avm = new WageRegisterVM();
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            avm = GetWage_RegisterData(WAG_Id);
           
            List<Clients> clients = avm.listClients.Where(m => m.CLI_Id.Equals(Convert.ToInt32(item_CLI_Id))).ToList();
           
            foreach(var item in clients)
            {
                List<Wage_Register> lst = new List<Wage_Register>();                
                List<Clients_Employees> clientsEmployees = item.Clients_Employees.ToList();
                IEnumerable<int> designationID = clientsEmployees.Select(m => m.DES_.DES_Id).Distinct();
                foreach (var desID in designationID)
                {                  
                    Client_Requirements cr = avm.client_Requirements.Where(m => m.DES_Id.Equals(desID) && m.CLI_Id.Equals(item.CLI_Id)).First();
                    List<Allowances> allowances = avm.allowances;
                    foreach(var employee in clientsEmployees.Where(m => m.DES_Id.Equals(desID)))
                    {
                        Wage_Register wr = new Wage_Register();
                        wr.WAG_Id = WAG_Id;
                        wr.CLI_Id = item.CLI_Id;
                        wr.EMP_Id = employee.EMP_Id;
                        wr.CRI_Id = cr.CRI_Id;

                        decimal basic, da, BasicDa, HRA;
                        int payDays;
                        int DaysInMonth = DateTime.DaysInMonth(avm.WAG_Month.Year, avm.WAG_Month.Month);
                        List<Attendance> attendances = employee.EMP_.Attendance.Where(m => m.CLI_Id.Equals(item.CLI_Id)).ToList();
                        payDays = attendances.Where(m => m.ATT_IsPresent).Count();
                        basic = (Decimal.Multiply(cr.CRI_Basic.Value, Convert.ToDecimal(payDays))) / DaysInMonth;
                        da = (Decimal.Multiply(cr.CRI_Basic.Value, Convert.ToDecimal(cr.CRI_DA))) / 100;
                        BasicDa = (Decimal.Add(cr.CRI_Basic.Value, Convert.ToDecimal(cr.CRI_DA)));
                        HRA = (cr.CRI_HRA_Fixed == null ? ((BasicDa * Convert.ToDecimal(cr.CRI_HRA_Percentage)) / 100) : ((Decimal.Multiply(cr.CRI_HRA_Fixed.Value, payDays)) / DaysInMonth));

                        wr.WAR_TotalPaybleDays = payDays;
                        wr.WAR_TotalWorkingDays = DaysInMonth;                       

                        decimal OTAmt, allAllowancesAmt, PFAmt, EsicAmt;
                        double OThrs = attendances.Sum(m => m.ATT_ExtraHoursWorked);
                        OTAmt = (((Convert.ToDecimal(cr.CRI_Basic.Value) / Convert.ToDecimal(DaysInMonth)) / 8) * Convert.ToDecimal(OThrs));
                        allAllowancesAmt = 0M;
                        PFAmt = Decimal.Multiply(BasicDa, Convert.ToDecimal(cr.CRI_PF_Percentage)) / 100;
                        EsicAmt = Decimal.Multiply(BasicDa, Convert.ToDecimal(cr.CRI_ESIC_Percentage)) / 100;

                        wr.WAR_ExtraWorkingHours = OThrs;
                        wr.WAR_Basic = Math.Round(basic,2);
                        wr.WAR_DA = Math.Round(da,2);
                        wr.WAR_HRA = Math.Round(HRA,2);                       

                        foreach (var all in cr.Client_Requirement_Allowances)
                        {
                            decimal amount = all.CRA_Amount;
                            decimal fullAmt = Decimal.Multiply(amount, Convert.ToDecimal(payDays)) / DaysInMonth;
                            if (all.CRA_DayswiseOrFull)
                            {
                                Math.Round(amount, 2);
                                allAllowancesAmt += amount;
                            }
                            else
                            {
                                Math.Round(fullAmt, 2);
                                allAllowancesAmt += fullAmt;
                             }

                        }
                        decimal GrossTot = basic + da + HRA + allAllowancesAmt + OTAmt;
                        decimal finalAmt = GrossTot - (PFAmt + EsicAmt);
                        wr.WAR_GrossTotal = Math.Round(GrossTot,2);
                        wr.ADM_LastModifiedBy = sessionUtils.GetLoggedAdminID();
                        wr.WAR_LastModifiedOn = ProjectUtils.DateNow();
                        lst.Add(wr);                        
                        
                    }
                }
                string res = wpm.SaveWageRegister(lst, WAG_Id, item_CLI_Id, sessionUtils.GetLoggedAdminID());
                if (res != string.Empty)
                {
                    TempData["message"] = "Wage Register is not saved!";
                }
                else
                {
                    TempData["message"] = "Wage Register is successfully saved!";
                }
            }           
            return RedirectToAction("WageRegister",new { WAG_Id = WAG_Id });
        }
        private WageRegisterVM GetWage_RegisterData(int WAG_Id)
        {
            WageRegisterVM avm = new WageRegisterVM();
            ClientsManager clientsManager = new ClientsManager(_context, Configuration);
            WageProcessManager wageManager = new WageProcessManager(_context);
            DesignationManager ds = new DesignationManager(_context);
            AttendanceManager attMgr = new AttendanceManager(_context);
            AllowanceManager allowanceManager = new AllowanceManager(_context);
            EmployeeManager em = new EmployeeManager(_context);
           
            Wage_Process wage = wageManager.getWageProcessById(WAG_Id);
            List<Clients> lstCli = clientsManager.GetActiveClientForAttandanceReg(wage.WAG_Month);
            avm.listAttendance = attMgr.getAttendance_Wage(WAG_Id);
            avm.client_Requirements = clientsManager.getClientRequirements();            
            avm.allowances = allowanceManager.GetAllowanceList();
            avm.listDesignations = ds.getDesignationsList();
            avm.listClients = lstCli;
            avm.listEmployee = em.GetEmployees();
            avm.WAG_Month = wage.WAG_Month;
            avm.WAG_Id = WAG_Id;
            return avm;
        }
    }
}