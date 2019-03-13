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

        public ActionResult UploadExcel(int WAG_Id, int CLI_Id)
        {
            ClientsManager clientManager = new ClientsManager(_context, _configuration);
            WageProcessManager wageManager = new WageProcessManager(_context);
            WageProcess wageProcess = wageManager.getWageProcessById(WAG_Id);
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
            WageProcess wageProcess = wageManager.getWageProcessById(uvm.wageProcessVM.WagId);
            IFormFile file = uvm.ExcelFile;
            ExcelViewModel excelViewModel = new ExcelViewModel();
            List<ExcelRowViewModel> rows = new List<ExcelRowViewModel>();
            int TotEmp = 0;
            string newPath = ProjectUtils.GetTempFolderPath(_hostingEnvironment.WebRootPath);
            StringBuilder sb = new StringBuilder();
            if (file != null)
            {
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
                        IRow headerRow = sheet.GetRow(0);
                        IRow secondRow = sheet.GetRow(1);
                        int cellCount = secondRow.LastCellNum;
                        int intStartDate = Convert.ToInt16(secondRow.GetCell(4).ToString());
                        int totalPublicHolidays = 0;
                        DateTime startDate = DateTime.Now, endDate = DateTime.Now;
                        if (intStartDate > 1) {
                            DateTime lastMonth = wageProcess.WagMonth.AddMonths(-1);
                            startDate = new DateTime(lastMonth.Year, lastMonth.Month, intStartDate);
                        }
                        else
                        {
                            startDate = new DateTime(wageProcess.WagMonth.Year, wageProcess.WagMonth.Month, intStartDate);
                        }
                        endDate = new DateTime(wageProcess.WagMonth.Year, wageProcess.WagMonth.Month, Convert.ToInt16(secondRow.GetCell(cellCount-5).ToString()));

                        for (int j = (secondRow.FirstCellNum + 4); j <= secondRow.LastCellNum - 4; j++)
                        {
                            if (secondRow.GetCell(j).ToString().Contains("PH"))
                                totalPublicHolidays++;
                        }

                        for (int i = (sheet.FirstRowNum + 2); i <= sheet.LastRowNum; i+=2)
                        {
                            ExcelRowViewModel excelRow = new ExcelRowViewModel();
                            IRow row = sheet.GetRow(i);
                            IRow rowExtra = sheet.GetRow(i + 1);

                            if (row == null) continue;
                            if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                            excelRow.EMP_Id = row.GetCell(1).ToString();
                            excelRow.EMP_Name = row.GetCell(2).ToString();
                            excelRow.Designation = row.GetCell(3).ToString();
                            TotEmp++;
                            int totalPresence = 0;
                            Double totalExtraHours = 0;
                            for (int j = (row.FirstCellNum + 4); j <= row.LastCellNum-4; j++)
                            {
                                if (row.GetCell(j).ToString().Equals("P"))
                                    totalPresence++;
                                if(rowExtra.GetCell(j) != null)
                                    if (!rowExtra.GetCell(j).ToString().Equals(""))
                                        totalExtraHours += Convert.ToDouble(rowExtra.GetCell(j).ToString());
                            }
                            excelRow.TotalPresenceDays = totalPresence;
                            excelRow.TotalExtraHours = totalExtraHours;
                            rows.Add(excelRow);
                        }
                        excelViewModel.excelRows = rows;
                        excelViewModel.totalEmployees = TotEmp;
                        excelViewModel.startDate = startDate;
                        excelViewModel.endDate = endDate;
                        excelViewModel.totalPublicHolidays = totalPublicHolidays;
                    }
                }
            }
           
            return View(excelViewModel);
        }
    }
}