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

        public ActionResult WageAttendanceList(int WAG_Id)
        {
            ClientsManager clientsManager = new ClientsManager(_context, _configuration);
            AttendanceManager attManager = new AttendanceManager(_context);
            WageProcessManager wageManager = new WageProcessManager(_context);
            Wage_Process wage = wageManager.getWageProcessById(WAG_Id);
            List<Clients> clients = clientsManager.GetActiveClientofaMonth(wage.WAG_Month);
            WageProcessClientAttendancePageVM pageVM = new WageProcessClientAttendancePageVM();
            pageVM.wageProcess = WageProcessMapper.mapMe(wage);
            pageVM.lstClient = WageProcessMapper.mapClientToAttendanceWages(clients, wage, attManager.getAttendance_Wage(WAG_Id));
            return View(pageVM);
        }

        public ActionResult DeleteWageClientAttendanceList(int WAG_Id, int CLI_Id)
        {
            AttendanceManager attManager = new AttendanceManager(_context);
            attManager.deleteAllAttendanceofWageClient(WAG_Id,CLI_Id);
            return RedirectToAction("WageAttendanceList", new { WAG_Id = WAG_Id });
        }

        public ActionResult UploadExcel(int WAG_Id, int CLI_Id)
        {
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
        public ActionResult VerifyTemplate(UploadExcelViewModel uvm)
        {
            WageProcessManager wageManager = new WageProcessManager(_context);
            AttendanceManager attManager = new AttendanceManager(_context);
            EmployeeManager empManager = new EmployeeManager(_context);
            ClientsManager clientsManager = new ClientsManager(_context, _configuration); 
            Wage_Process wageProcess = wageManager.getWageProcessById(uvm.wageProcessVM.WAG_Id);
            IFormFile file = uvm.ExcelFile;
            ExcelViewModel excelViewModel = new ExcelViewModel();
            List<ExcelRowViewModel> rows = new List<ExcelRowViewModel>();
            int TotEmp = 0;
            #region 
            List<Employees> empListExtraInExcel = new List<Employees>();
            List<Employees> empListExtraInDb = new List<Employees>();
            List<Attendance> attandanceList=new List<Attendance>();
            #endregion
            string newPath = ProjectUtils.GetTempFolderPath(_hostingEnvironment.WebRootPath);
            StringBuilder sb = new StringBuilder();
            if (file != null)
            {
                if (file.Length > 0)
                {
                    string sFileExtension = Path.GetExtension(file.FileName).ToLower();
                    ISheet sheet;
                    string fullPath = Path.Combine(newPath, ProjectUtils.GetTempFileName()+sFileExtension);
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
                        IRow headerRow = sheet.GetRow(0);
                        IRow secondRow = sheet.GetRow(1);
                        int cellCount = secondRow.LastCellNum;
                        int intStartDate = Convert.ToInt16(secondRow.GetCell(4).ToString());
                        int totalPublicHolidays = 0;
                        DateTime startDate = DateTime.Now, endDate = DateTime.Now;
                        if (intStartDate > 1) {
                            DateTime lastMonth = wageProcess.WAG_Month.AddMonths(-1);
                            startDate = new DateTime(lastMonth.Year, lastMonth.Month, intStartDate);
                        }
                        else
                        {
                            startDate = new DateTime(wageProcess.WAG_Month.Year, wageProcess.WAG_Month.Month, intStartDate);
                        }
                        endDate = new DateTime(wageProcess.WAG_Month.Year, wageProcess.WAG_Month.Month, Convert.ToInt16(secondRow.GetCell(cellCount-5).ToString()));

                        #region
                        DateTime DbstartDate = DateTime.Now, DbendDate = DateTime.Now;
                        Clients client = clientsManager.GetClientById(uvm.client.CLI_Id);
                        if (client.CLI_Att_MonthReal == true)
                        {
                            DbstartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                            DbendDate = startDate.AddMonths(1).AddDays(-1);
                        }
                        else if (client.CLI_Att_MonthReal == false)
                        {
                            DbstartDate = new DateTime(DateTime.Now.AddMonths(-1).Year, DateTime.Now.AddMonths(-1).Month, client.CLI_Att_Month_Start.Value);
                            DbendDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, client.CLI_Att_Month_End.Value); ;
                        }
                        if(startDate.Date!=DbstartDate.Date || endDate.Date != DbendDate.Date)
                        {
                            excelViewModel.datePeriod = false;
                        }
                        #endregion
                        for (int j = (secondRow.FirstCellNum + 4); j <= secondRow.LastCellNum - 4; j++)
                        {
                            if (secondRow.GetCell(j).ToString().Contains("PH"))
                                totalPublicHolidays++;
                        }

                        #region 
                        List<Clients_Employees> assignEmployeeList = attManager.assignEmployeeList(uvm.client.CLI_Id);
                        List<_EmpID> _empID = new List<_EmpID>();
                        #endregion
                       
                        for (int i = (sheet.FirstRowNum + 2); i <= sheet.LastRowNum; i+=2)
                        {
                            _EmpID EmpID = new _EmpID();
                                               
                            ExcelRowViewModel excelRow = new ExcelRowViewModel();
                            IRow row = sheet.GetRow(i);
                            IRow rowExtra = sheet.GetRow(i + 1);

                            if (row == null) continue;
                            if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                            excelRow.EMP_Id = row.GetCell(1).ToString();
                           
                            #region created by rinku on 15 march - 19 For error display 
                            int EMP_Id = Convert.ToInt32(row.GetCell(1).ToString());
                           
                            Clients_Employees emp = assignEmployeeList.Where(m => m.EMP_Id.Equals(EMP_Id)).FirstOrDefault();
                            
                            if (emp==null)
                            {
                                empListExtraInExcel.Add(empManager.GetEmployeesById(EMP_Id));
                            }
                            else
                            {
                                EmpID.Id = EMP_Id;
                                _empID.Add(EmpID);
                            }
                          
                            #endregion
                            excelRow.EMP_Name = row.GetCell(2).ToString();
                            excelRow.Designation = row.GetCell(3).ToString();
                            TotEmp++;
                            int totalPresence = 0;
                            Double totalExtraHours = 0;
                            DateTime tmpDate = startDate;
                            for (int j = (row.FirstCellNum + 4); j <= row.LastCellNum-4; j++)
                            {
                                Attendance attendance = new Attendance();
                                attendance.EMP_Id = EMP_Id;
                                attendance.CLI_Id = uvm.client.CLI_Id;
                                attendance.ATT_Date = tmpDate;
                                tmpDate = tmpDate.AddDays(1);                               
                                if (row.GetCell(j).ToString().Equals("P"))
                                {
                                    attendance.ATT_IsPresent = true;
                                    totalPresence++;
                                }
                                if (rowExtra.GetCell(j) != null)
                                    if (!rowExtra.GetCell(j).ToString().Equals("")) { 
                                        attendance.ATT_ExtraHoursWorked = Convert.ToDouble(rowExtra.GetCell(j).ToString());
                                        totalExtraHours += Convert.ToDouble(rowExtra.GetCell(j).ToString());
                                    }
                                attandanceList.Add(attendance);
                            }
                            excelRow.TotalPresenceDays = totalPresence;
                            excelRow.TotalExtraHours = totalExtraHours;
                            rows.Add(excelRow);
                        }
                        #region
                        HashSet<int> diffids = new HashSet<int>(_empID.Select(s => s.Id));
                        var EmployeeIdList = assignEmployeeList.Where(m => !diffids.Contains(m.EMP_Id)).ToList();
                        foreach(var EMP_Id in EmployeeIdList)
                        {
                            empListExtraInDb.Add(empManager.GetEmployeesById(Convert.ToInt32(EMP_Id.EMP_Id)));
                        }
                        #endregion
                        excelViewModel.excelRows = rows;
                        excelViewModel.totalEmployees = TotEmp;
                        excelViewModel.startDate = startDate;
                        excelViewModel.endDate = endDate;
                        excelViewModel.fileName = fullPath;
                        excelViewModel.totalPublicHolidays = totalPublicHolidays;
                        excelViewModel.WAG_Id = uvm.wageProcessVM.WAG_Id;
                        excelViewModel.CLI_Id = uvm.client.CLI_Id;                        
                    }
                }
            }
            #region
            excelViewModel.empListExtraInExcel = empListExtraInExcel;
            excelViewModel.EmpListExtraInDb = empListExtraInDb;
            if (empListExtraInExcel.Count >0 || empListExtraInDb.Count >0 || excelViewModel.datePeriod == false)
            {
                excelViewModel.btnExportToDatabase = false;
            }
            excelViewModel.listAttendance = attandanceList;
            #endregion
            return View(excelViewModel);
        }

        [HttpPost]
        public ActionResult ImportExcel(IFormCollection frm)
        {
            int WAG_Id = Convert.ToInt32(frm["WAG_Id"]);
            int CLI_Id = Convert.ToInt32(frm["CLI_Id"]);
            string strFilePath = frm["fileName"];
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            AttendanceManager attManager = new AttendanceManager(_context);
            ClientsManager clientManager = new ClientsManager(_context,_configuration);
            WageProcessManager wageManager = new WageProcessManager(_context);
            Wage_Process wageProcess = wageManager.getWageProcessById(WAG_Id);
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
                endDate = new DateTime(wageProcess.WAG_Month.Year, wageProcess.WAG_Month.Month, Convert.ToInt16(secondRow.GetCell(cellCount - 5).ToString()));

                //for (int j = (secondRow.FirstCellNum + 4); j <= secondRow.LastCellNum - 4; j++)
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

                    int EMP_Id = Convert.ToInt16(row.GetCell(1).ToString());
                    DateTime tmpDate = startDate;
                    for (int j = (row.FirstCellNum + 4); j <= row.LastCellNum - 5; j++)
                    {
                        Attendance att = new Attendance();
                        att.EMP_Id = EMP_Id;
                        att.WAG_Id = WAG_Id;
                        att.CLI_Id = CLI_Id;
                        att.CRI_Id = clientManager.getClientRequirementId(CLI_Id, EMP_Id);
                        att.ATT_Date = tmpDate;
                        if (row.GetCell(j).ToString().Equals("P"))
                            att.ATT_IsPresent = true;
                        else if (row.GetCell(j).ToString().Equals("A"))
                            att.ATT_IsPresent = false;
                        att.ATT_IsPaidHoliday = secondRow.GetCell(j).ToString().Contains("PH");
                        if (rowExtra.GetCell(j) != null)
                            if (!rowExtra.GetCell(j).ToString().Equals(""))
                                att.ATT_ExtraHoursWorked = Convert.ToDouble(rowExtra.GetCell(j).ToString());
                        att.ATT_IsEarnLeave = false;
                        att.ATT_IsWeeklyOff = false;
                        att.ATT_Shift = "";
                        att.ATT_ImportedOn = DateTime.Now;
                        att.ADM_Id_ImportedBy = sessionUtils.GetLoggedAdminID();
                        attManager.save(att);
                        tmpDate = tmpDate.AddDays(1);
                    }
                }
            }
            return RedirectToAction("WageAttendanceList", new { WAG_Id = WAG_Id});
        }

        public ActionResult AttendanceRegister(int WAG_Id)
        {
            AttendanceRegisterVM avm = new AttendanceRegisterVM();
            ClientsManager clientsManager = new ClientsManager(_context, _configuration);
            WageProcessManager wageManager = new WageProcessManager(_context);
            Wage_Process wage = wageManager.getWageProcessById(WAG_Id);
            List<Clients> lstCli = clientsManager.GetActiveClientForAttandanceReg(wage.WAG_Month);
            avm.listClients = lstCli;           
            return View(avm);
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