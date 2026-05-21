using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static RMERP.DAL.Helpers.ProjectUtils;

namespace RMERP.Controllers
{
    [Authorize]
    public class WageRegisterController : Controller
    {
        private IWebHostEnvironment _hostingEnvironment;
        private readonly RMERPContext _context;
        public WageRegisterController(RMERPContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        public ActionResult WageProcessClients(int WAG_Id, int FRM_Id)
        {
            try
            {
                SessionUtils sessionUtils = new(Request, Response);
                ClientsManager clientsManager = new(_context);
                WageProcessManager wageProcessManager = new(_context);
                WageRegisterManager wageRegisterManager = new(_context);
                if (WAG_Id > 0)
                {
                    Wage_Process wageProcess = wageProcessManager.GetWageProcessByWAG_Id(WAG_Id, true, true, false, true);
                    List<Client> clients = clientsManager.GetActiveClientOfMonthByFRM_Id1(new DateTime(wageProcess.WAG_Month.Year, wageProcess.WAG_Month.Month, wageProcess.WAG_Month.Day), FRM_Id);
                    WageProcessClientsVM ret = new() { FRM_Id = FRM_Id, FRM_Title = wageProcess.FRM.FRM_ShortName, WAG_Id = WAG_Id, WAG_Month = wageProcess.WAG_Month,WAG_Status=wageProcess.WAG_Status };
                    if (clients != null && clients.Count > 0)
                    {
                        ret.WageClients.AddRange(clients.Select(s => new WP_ClientsStatusVM
                        {
                            CLI_Id = s.CLI_Id,
                            CLI_Name = s.CLI_Name,
                            Is_WageRegisterSaved = s.Wage_Process_Clients.Where(wpc => wpc.WAG_Id.Equals(WAG_Id)).Any(),
                            Is_AttendanceDone = s.Attendance_Summaries.Where(asa => asa.WAG_Id.Equals(WAG_Id)).Any(),
                        }).ToList());
                    }
                    return View(ret);
                }
            }
            catch (Exception) { throw; }
            return null;
        }

        public ActionResult WageProcessClientRegister(int WAG_Id, int FRM_Id, int CLI_Id)
        {
            try
            {
                SessionUtils sessionUtils = new(Request, Response);
                WageRegisterManager wageRegisterManager = new(_context);
                if (WAG_Id > 0 && FRM_Id > 0 && CLI_Id > 0)
                {
                    ClientWageRegisterVM Table = wageRegisterManager.GenerateWageClientRegisterTable(WAG_Id, sessionUtils.GetLoggedAdminID(), FRM_Id, CLI_Id);
                    return View(Table);
                }
            }
            catch (Exception ex) { throw; }
            return null;
        }

        [HttpPost]
        public ActionResult NewSaveWageRegister(int WAG_Id, string item_CLI_Id)
        {
            WageRegisterManager wageRegisterManager = new(_context);
            SessionUtils sessionUtils = new(Request, Response);
            Wage_Process wageProcess = new();
            if (WAG_Id > 0)
            {
                WageProcessManager wageManager = new(_context);
                wageProcess = wageManager.GetWageProcessByWAG_Id(WAG_Id, true, true, true, true);
                List<Wage_Register> wage_Registers = wageRegisterManager.Get_WageRegisterCalculated(wageProcess, Convert.ToInt32(item_CLI_Id), sessionUtils.GetLoggedAdminID());
                wageRegisterManager.SaveWageRegister(wage_Registers, WAG_Id, item_CLI_Id, sessionUtils.GetLoggedAdminID());
            }
            return RedirectToAction("WageProcessClientRegister", new { WAG_Id = WAG_Id, FRM_Id = wageProcess.FRM_Id, CLI_Id = item_CLI_Id });
        }

        [HttpPost]
        public ActionResult NewResetWageRegister(int WAG_Id, string item_CLI_Id, int FRM_Id)
        {
            WageRegisterManager wageRegisterManager = new(_context);
            string res = wageRegisterManager.ResetWageRegister(WAG_Id, item_CLI_Id);
            if (!string.IsNullOrWhiteSpace(res))
                TempData["message"] = "Wage Register is not saved!";
            return RedirectToAction("WageProcessClientRegister", new { WAG_Id = WAG_Id, FRM_Id = FRM_Id, CLI_Id = item_CLI_Id });
        }

        public async Task<FileResult> WageRegisterNewExcel(int WAG_Id, int FRM_Id)
        {
            WageProcessManager wageProcess = new(_context);
            AllowanceManager allManager = new(_context);
            ClientsManager cliManager = new(_context);
            AttendanceManager attManager = new(_context);
            Wage_Process wage_Process = wageProcess.GetWageProcess_WAG_Id(WAG_Id);
            string WAG_Month = wage_Process.WAG_Month.ToString("MMMM").ToUpper() + "-" + wage_Process.WAG_Month.ToString("yyyy");
            DateTime WAG_MonthDT = new DateTime(wage_Process.WAG_Month.Year, wage_Process.WAG_Month.Month, wage_Process.WAG_Month.Day);
            string newPath = GetTempFolderPath(_hostingEnvironment.WebRootPath);
            string fileName = "New_Wage_Register_" + DateTime.Now.ToString("ddMMyyyyHHmm") + "_" + WAG_Month + ".xlsx";
            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, fileName);
            FileInfo file = new(Path.Combine(newPath, fileName));
            var memory = new MemoryStream();
            using (var fs = new FileStream(Path.Combine(newPath, fileName), FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook = new XSSFWorkbook();
                ISheet excelSheet = null;
                foreach (var client in wage_Process.Wage_Process_Clients.OrderBy(c => c.CLI.CLI_RegisteredOn))
                {
                    string sheetName = client.CLI_Id.ToString() + "-" + WorkbookUtil.CreateSafeSheetName((client.CLI.CLI_Name.ToString().Length > 20 ? client.CLI.CLI_Name.ToString().Substring(0, 20) : client.CLI.CLI_Name.ToString()));
                    excelSheet = workbook.CreateSheet(sheetName);

                    DateTime[] arr = DateHelper.GetStartEndDatePeriodForAttendance(client.CLI, cliManager.GetAttendanceParameterByMonth(client.CLI.CLI_Id, WAG_MonthDT), WAG_MonthDT);
                    List<AllowanceVM> lstMasterOtherAllowances = AllowanceVM.MapMes(allManager.GetAllowanceList().OrderBy(m => m.ALL_Id).ToList());
                    string[] lstMasterCustomeAllowancesAlias = new string[10];
                    decimal[] lstMasterCustomeAllowancesAmount = new decimal[10];

                    #region style of excel
                    ICellStyle style = workbook.CreateCellStyle();
                    ICellStyle styleDesignation = workbook.CreateCellStyle();
                    ICellStyle styleTotal = workbook.CreateCellStyle();

                    ICellStyle styleGrey25 = workbook.CreateCellStyle();
                    ICellStyle styleYellowBg = workbook.CreateCellStyle();
                    ICellStyle styleExtra = workbook.CreateCellStyle();

                    IFont fontcell = workbook.CreateFont();
                    fontcell.FontHeight = 11;

                    IFont font = workbook.CreateFont();
                    font.IsBold = true;
                    font.FontHeightInPoints = ((short)14);
                    font.FontName = ("Cambria");

                    styleGrey25.WrapText = true;
                    styleGrey25.VerticalAlignment = VerticalAlignment.Center;
                    styleGrey25.BorderBottom = (BorderStyle.Thin);
                    styleGrey25.BottomBorderColor = (IndexedColors.Black.Index);
                    styleGrey25.BorderLeft = (BorderStyle.Thin);
                    styleGrey25.LeftBorderColor = (IndexedColors.Black.Index);
                    styleGrey25.BorderRight = (BorderStyle.Thin);
                    styleGrey25.RightBorderColor = (IndexedColors.Black.Index);
                    styleGrey25.BorderTop = (BorderStyle.Thin);
                    styleGrey25.TopBorderColor = (IndexedColors.Black.Index);
                    styleGrey25.SetFont(fontcell);

                    styleYellowBg.WrapText = true;
                    styleYellowBg.VerticalAlignment = VerticalAlignment.Center;
                    styleYellowBg.BorderBottom = (BorderStyle.Thin);
                    styleYellowBg.BottomBorderColor = (IndexedColors.Black.Index);
                    styleYellowBg.BorderLeft = (BorderStyle.Thin);
                    styleYellowBg.LeftBorderColor = (IndexedColors.Black.Index);
                    styleYellowBg.BorderRight = (BorderStyle.Thin);
                    styleYellowBg.RightBorderColor = (IndexedColors.Black.Index);
                    styleYellowBg.BorderTop = (BorderStyle.Thin);
                    styleYellowBg.TopBorderColor = (IndexedColors.Black.Index);
                    styleYellowBg.SetFont(fontcell);
                    styleYellowBg.FillForegroundColor = IndexedColors.Yellow.Index;
                    styleYellowBg.FillPattern = FillPattern.SolidForeground;
                    styleYellowBg.FillBackgroundColor = HSSFColor.Yellow.Index;

                    styleExtra.WrapText = true;
                    styleExtra.VerticalAlignment = VerticalAlignment.Center;
                    styleExtra.SetFont(fontcell);

                    styleDesignation.SetFont(font);

                    IFont fontTotal = workbook.CreateFont();
                    fontTotal.IsBold = true;
                    fontTotal.FontHeightInPoints = ((short)10);
                    styleTotal.WrapText = true;
                    styleTotal.VerticalAlignment = VerticalAlignment.Center;
                    styleTotal.BorderBottom = (BorderStyle.Thin);
                    styleTotal.BottomBorderColor = (IndexedColors.Black.Index);
                    styleTotal.BorderLeft = (BorderStyle.Thin);
                    styleTotal.LeftBorderColor = (IndexedColors.Black.Index);
                    styleTotal.BorderRight = (BorderStyle.Thin);
                    styleTotal.RightBorderColor = (IndexedColors.Black.Index);
                    styleTotal.BorderTop = (BorderStyle.Thin);
                    styleTotal.TopBorderColor = (IndexedColors.Black.Index);
                    styleTotal.SetFont(fontTotal);

                    #endregion

                    int DesCount = 0;

                    #region Title structure in excel
                    IRow rowClientBlank = excelSheet.CreateRow(DesCount);
                    ICell CellClientBlank = rowClientBlank.CreateCell(0);
                    CellClientBlank.SetCellValue("");

                    IRow rowSubHeading = excelSheet.CreateRow(DesCount + 1);
                    ICell CellSubHeading = rowSubHeading.CreateCell(0);
                    CellSubHeading.SetCellValue("Name of Establishment :" + client.CLI.CLI_Name.ToString());
                    CellSubHeading.CellStyle = styleDesignation;
                    excelSheet.AddMergedRegion(new CellRangeAddress(DesCount + 1, DesCount + 1, 0, 6));

                    ICell CellSubHeadExtra = rowSubHeading.CreateCell(52);
                    CellSubHeadExtra.SetCellValue("The Minumum Wages Rules,1963");
                    CellSubHeadExtra.CellStyle = styleExtra;
                    excelSheet.AddMergedRegion(new CellRangeAddress(DesCount + 1, DesCount + 1, 52, 55));


                    IRow rowClient = excelSheet.CreateRow(DesCount + 2);
                    ICell CellClient = rowClient.CreateCell(0);
                    CellClient.SetCellValue("Name of Employer / Contractor:" + wage_Process.FRM?.FRM_Name);
                    CellClient.CellStyle = styleDesignation;
                    excelSheet.AddMergedRegion(new CellRangeAddress(DesCount + 2, DesCount + 2, 0, 7));

                    ICell CellSubHeadExtra2 = rowClient.CreateCell(52);
                    CellSubHeadExtra2.SetCellValue("Form II");
                    CellSubHeadExtra2.CellStyle = styleExtra;
                    excelSheet.AddMergedRegion(new CellRangeAddress(DesCount + 2, DesCount + 2, 52, 55));

                    IRow rowClient2 = excelSheet.CreateRow(DesCount + 3);
                    ICell CellClient2 = rowClient2.CreateCell(0);
                    CellClient2.SetCellValue("For the Month   :" + wage_Process.WAG_Month.ToString("MMM").ToUpper());
                    CellClient2.CellStyle = styleExtra;
                    excelSheet.AddMergedRegion(new CellRangeAddress(DesCount + 3, DesCount + 3, 0, 2));

                    ICell CellClient22 = rowClient2.CreateCell(5);
                    CellClient22.SetCellValue("YEAR : " + wage_Process.WAG_Month.ToString("yyyy"));
                    CellClient22.CellStyle = styleExtra;

                    IRow rowClient3 = excelSheet.CreateRow(DesCount + 4);
                    ICell CellClient3 = rowClient3.CreateCell(0);
                    CellClient3.SetCellValue("");
                    #endregion

                    DesCount = DesCount + 5;
                    bool IsCRI_OutStation_Allowance = false, IsCRI_Attendance_Allowance = false, IsCRI_Nightshift_Allowance = false, IsCRI_Performance_Allowance = false;
                    decimal Final_TotalOutStation = 0M, Final_TotalAttendance = 0M, Final_TotalNightshift = 0M, Final_TotalPerformance = 0M, Final_TotalLWF = 0M;
                    decimal Final_TotalEarnedBasic = 0M, Final_TotalEarnedHRA = 0M, Final_TotalGrossWagePayble = 0M, Final_TotalAdvance = 0M, Final_TotalProfTax = 0M, Final_TotalPF = 0M, Final_TotalESIC = 0M, Final_TotalOtherDeduction = 0M, Final_TotalNetWagesPaid = 0M;

                    var wgs = wage_Process.Wage_Registers.Where(wg => wg.CLI_Id.Equals(client.CLI_Id));
                    var CRI_Ids = wgs.GroupBy(gg => gg.CRI_Id).Select(cri => cri.Key).ToList();
                    int totalAttCols = 0;
                    foreach (int CRI_Id in CRI_Ids)
                    {
                        Client_Requirement cri = cliManager.GetClientRequirementsById(CRI_Id);

                        #region Headings
                        IRow row = excelSheet.CreateRow(DesCount);
                        ICell cell0 = row.CreateCell(0);
                        cell0.SetCellValue("Sr.No");
                        excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount + 1, 0, 0));
                        cell0.CellStyle = styleYellowBg;

                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("Employee Code");
                        excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount + 1, 1, 1));
                        cell1.CellStyle = styleYellowBg;

                        ICell cell2 = row.CreateCell(2);
                        cell2.SetCellValue("Employee Name");
                        excelSheet.SetColumnWidth(2, (int)((25 + 0.72) * 256));
                        excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount + 1, 2, 2));
                        cell2.CellStyle = styleYellowBg;

                        ICell cell3 = row.CreateCell(3);
                        cell3.SetCellValue("Age");
                        excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount + 1, 3, 3));
                        cell3.CellStyle = styleYellowBg;

                        ICell cell4 = row.CreateCell(4);
                        cell4.SetCellValue("Gender");
                        excelSheet.SetColumnWidth(2, (int)((4 + 0.72) * 256));
                        cell4.CellStyle = styleYellowBg;
                        excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount + 1, 4, 4));

                        ICell cell5 = row.CreateCell(5);
                        excelSheet.SetColumnWidth(5, (int)((13 + 0.72) * 256));
                        cell5.SetCellValue("Date of Entry");
                        cell5.CellStyle = styleYellowBg;
                        excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount + 1, 5, 5));

                        ICell cell6 = row.CreateCell(6);
                        cell6.SetCellValue("Designation");
                        excelSheet.SetColumnWidth(6, (int)((13 + 0.72) * 256));
                        cell6.CellStyle = styleYellowBg;
                        excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount + 1, 6, 6));

                        ICell cell7 = row.CreateCell(7);
                        cell7.SetCellValue("Working Hours");
                        cell7.CellStyle = styleYellowBg;
                        ICell cell327 = row.CreateCell(8);
                        cell327.CellStyle = styleYellowBg;
                        excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount, 7, 8));

                        ICell cell8 = row.CreateCell(9);
                        cell8.SetCellValue("Interval for Rest");
                        cell8.CellStyle = styleYellowBg;
                        ICell cell328 = row.CreateCell(10);
                        cell328.CellStyle = styleYellowBg;
                        excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount, 9, 10));

                        IRow row2 = excelSheet.CreateRow(DesCount + 1);
                        ICell cell27 = row2.CreateCell(7);
                        cell27.SetCellValue("From");
                        cell27.CellStyle = styleYellowBg;
                        ICell cell28 = row2.CreateCell(8);
                        cell28.SetCellValue("To");
                        cell28.CellStyle = styleYellowBg;
                        ICell cell29 = row2.CreateCell(9);
                        cell29.SetCellValue("From");
                        cell29.CellStyle = styleYellowBg;
                        ICell cell220 = row2.CreateCell(10);
                        cell220.SetCellValue("To");
                        cell220.CellStyle = styleYellowBg;

                        ICell cell221 = row.CreateCell(11);
                        cell221.SetCellValue("No of Hours Worked");
                        cell221.CellStyle = styleYellowBg;
                        CellUtil.SetAlignment(cell221, workbook, (short)HorizontalAlignment.Center);

                        int totalday = Convert.ToInt32((arr[1] - arr[0]).TotalDays);
                        ICell cell329;
                        for (int j = 12; j <= totalday + 12; j++)
                        {
                            cell329 = row.CreateCell(j);
                            cell329.CellStyle = styleYellowBg;
                        }
                        excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount, 11, totalday + 11));
                        int cellCount = 11;

                        if (arr.Length >= 2)
                        {
                            int end = DateTime.DaysInMonth(WAG_MonthDT.Year, WAG_MonthDT.Month);
                            ICell cellday;
                            for (var day = arr[0].Date; day <= arr[1].Date; day = day.AddDays(1))
                            {

                                cellday = row2.CreateCell(cellCount);
                                cellday.SetCellValue(day.Day);
                                excelSheet.SetColumnWidth(cellCount, (int)((3 + 0.72) * 256));
                                cellday.CellStyle = styleYellowBg;
                                cellCount++;
                            }
                        }
                        ICell cell31 = row.CreateCell(cellCount);
                        cell31.SetCellValue("Total Days worked");
                        cell31.CellStyle = styleYellowBg;
                        excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount + 1, cellCount, cellCount));

                        cellCount = cellCount + 1;
                        ICell cell32 = row.CreateCell(cellCount);
                        cell32.SetCellValue("Weekly Off");
                        cell32.CellStyle = styleYellowBg;
                        excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount + 1, cellCount, cellCount));

                        cellCount = cellCount + 1;
                        ICell cell33 = row.CreateCell(cellCount);
                        cell33.SetCellValue("Paid Holiday");
                        cell33.CellStyle = styleYellowBg;
                        excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount + 1, cellCount, cellCount));

                        cellCount = cellCount + 1;
                        ICell cell34 = row.CreateCell(cellCount);
                        cell34.SetCellValue("Leave");
                        cell34.CellStyle = styleYellowBg;
                        excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount + 1, cellCount, cellCount));

                        cellCount = cellCount + 1;
                        ICell cell35 = row.CreateCell(cellCount);
                        cell35.SetCellValue("Extra Day");
                        cell35.CellStyle = styleYellowBg;
                        excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount + 1, cellCount, cellCount));

                        cellCount = cellCount + 1;
                        ICell cell36 = row.CreateCell(cellCount);
                        cell36.SetCellValue("Comp Off");
                        cell36.CellStyle = styleYellowBg;
                        excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount + 1, cellCount, cellCount));

                        cellCount = cellCount + 1;
                        ICell cell37 = row.CreateCell(cellCount);
                        cell37.SetCellValue("Adjustment");
                        cell37.CellStyle = styleYellowBg;
                        excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount + 1, cellCount, cellCount));

                        cellCount = cellCount + 1;
                        ICell cell38 = row.CreateCell(cellCount);
                        cell38.SetCellValue("Absent");
                        cell38.CellStyle = styleYellowBg;
                        excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount + 1, cellCount, cellCount));

                        cellCount = cellCount + 1;
                        ICell cell39 = row.CreateCell(cellCount);
                        cell39.SetCellValue("Total Payable Days");
                        cell39.CellStyle = styleYellowBg;
                        excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount + 1, cellCount, cellCount));

                        cellCount = cellCount + 1;
                        ICell cell40 = row.CreateCell(cellCount);
                        cell40.SetCellValue("Earned Basic");
                        cell40.CellStyle = styleGrey25;
                        excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount + 1, cellCount, cellCount));

                        cellCount = cellCount + 1;
                        ICell cell41 = row.CreateCell(cellCount);
                        cell41.SetCellValue("Earned HRA");
                        cell41.CellStyle = styleGrey25;
                        excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount + 1, cellCount, cellCount));

                        cellCount = cellCount + 1;
                        ICell cell42 = row.CreateCell(cellCount);
                        cell42.SetCellValue("Earned CCA");
                        cell42.CellStyle = styleGrey25;
                        excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount + 1, cellCount, cellCount));

                        int cell = cellCount;

                        #region CLIENTS REQUIREMENTS ALLOWANCE

                        foreach (var all in cri.Client_Requirement_Allowances.OrderBy(m => m.ALL_Id))
                        {
                            ICell cellAll = row.CreateCell(cell + 1);
                            cellAll.SetCellValue(all.ALL.ALL_Alias);
                            excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount + 1, cell + 1, cell + 1));
                            excelSheet.SetColumnWidth(cell + 1, (int)((25 + 0.72) * 140));
                            cellAll.CellStyle = styleGrey25;
                            cell++;
                        }

                        #endregion

                        #region CUSTOM ALLOWANCE

                        if (cri.CRI_Allowance_1)
                        {
                            cell = cell + 1;
                            ICell celOutAllow = row.CreateCell(cell);
                            celOutAllow.SetCellValue(cri.CRI_Allowance_Name_1);
                            excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount + 1, cell, cell));
                            celOutAllow.CellStyle = styleGrey25;
                            lstMasterCustomeAllowancesAlias[0] = cri.CRI_Allowance_Name_1;
                        }
                        if (cri.CRI_Allowance_2)
                        {
                            cell = cell + 1;
                            ICell celOutAllow = row.CreateCell(cell);
                            celOutAllow.SetCellValue(cri.CRI_Allowance_Name_2);
                            excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount + 1, cell, cell));
                            celOutAllow.CellStyle = styleGrey25;
                            lstMasterCustomeAllowancesAlias[1] = cri.CRI_Allowance_Name_2;
                        }
                        if (cri.CRI_Allowance_3)
                        {
                            cell = cell + 1;
                            ICell celOutAllow = row.CreateCell(cell);
                            celOutAllow.SetCellValue(cri.CRI_Allowance_Name_3);
                            excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount + 1, cell, cell));
                            celOutAllow.CellStyle = styleGrey25;
                            lstMasterCustomeAllowancesAlias[2] = cri.CRI_Allowance_Name_3;
                        }
                        if (cri.CRI_Allowance_4)
                        {
                            cell = cell + 1;
                            ICell celOutAllow = row.CreateCell(cell);
                            celOutAllow.SetCellValue(cri.CRI_Allowance_Name_4);
                            excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount + 1, cell, cell));
                            celOutAllow.CellStyle = styleGrey25;
                            lstMasterCustomeAllowancesAlias[3] = cri.CRI_Allowance_Name_4;
                        }
                        if (cri.CRI_Allowance_5)
                        {
                            cell = cell + 1;
                            ICell celOutAllow = row.CreateCell(cell);
                            celOutAllow.SetCellValue(cri.CRI_Allowance_Name_5);
                            excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount + 1, cell, cell));
                            celOutAllow.CellStyle = styleGrey25;
                            lstMasterCustomeAllowancesAlias[4] = cri.CRI_Allowance_Name_5;
                        }
                        if (cri.CRI_Allowance_6)
                        {
                            cell = cell + 1;
                            ICell celOutAllow = row.CreateCell(cell);
                            celOutAllow.SetCellValue(cri.CRI_Allowance_Name_6);
                            excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount + 1, cell, cell));
                            celOutAllow.CellStyle = styleGrey25;
                            lstMasterCustomeAllowancesAlias[5] = cri.CRI_Allowance_Name_6;
                        }
                        if (cri.CRI_Allowance_7)
                        {
                            cell = cell + 1;
                            ICell celOutAllow = row.CreateCell(cell);
                            celOutAllow.SetCellValue(cri.CRI_Allowance_Name_7);
                            excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount + 1, cell, cell));
                            celOutAllow.CellStyle = styleGrey25;
                            lstMasterCustomeAllowancesAlias[6] = cri.CRI_Allowance_Name_7;
                        }
                        if (cri.CRI_Allowance_8)
                        {
                            cell = cell + 1;
                            ICell celOutAllow = row.CreateCell(cell);
                            celOutAllow.SetCellValue(cri.CRI_Allowance_Name_8);
                            excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount + 1, cell, cell));
                            celOutAllow.CellStyle = styleGrey25;
                            lstMasterCustomeAllowancesAlias[7] = cri.CRI_Allowance_Name_8;
                        }
                        if (cri.CRI_Allowance_9)
                        {
                            cell = cell + 1;
                            ICell celOutAllow = row.CreateCell(cell);
                            celOutAllow.SetCellValue(cri.CRI_Allowance_Name_9);
                            excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount + 1, cell, cell));
                            celOutAllow.CellStyle = styleGrey25;
                            lstMasterCustomeAllowancesAlias[8] = cri.CRI_Allowance_Name_9;
                        }
                        if (cri.CRI_Allowance_10)
                        {
                            cell = cell + 1;
                            ICell celOutAllow = row.CreateCell(cell);
                            celOutAllow.SetCellValue(cri.CRI_Allowance_Name_10);
                            excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount + 1, cell, cell));
                            celOutAllow.CellStyle = styleGrey25;
                            lstMasterCustomeAllowancesAlias[9] = cri.CRI_Allowance_Name_10;
                        }

                        #endregion

                        #region STATIC ALLOWANCES

                        if (cri.CRI_OutStation_Allowance == true)
                        {
                            cell = cell + 1;
                            ICell celOutAllow = row.CreateCell(cell);
                            celOutAllow.SetCellValue("Outstation Allowance");
                            excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount + 1, cell, cell));
                            celOutAllow.CellStyle = styleGrey25;
                        }
                        if (cri.CRI_Attendance_Allowance == true)
                        {
                            cell = cell + 1;
                            ICell cellAttAllow = row.CreateCell(cell);
                            cellAttAllow.SetCellValue("Attendance Allowance");
                            excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount + 1, cell, cell));
                            cellAttAllow.CellStyle = styleGrey25;
                        }
                        if (cri.CRI_Nightshift_Allowance == true)
                        {
                            cell = cell + 1;
                            ICell cellNightAllow = row.CreateCell(cell);
                            cellNightAllow.SetCellValue("Night Allowance");
                            excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount + 1, cell, cell));
                            cellNightAllow.CellStyle = styleGrey25;
                        }
                        if (cri.CRI_Performance_Allowance == true)
                        {
                            cell = cell + 1;
                            ICell cellPerformanceAllow = row.CreateCell(cell);
                            cellPerformanceAllow.SetCellValue("Performance Allowance");
                            excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount + 1, cell, cell));
                            cellPerformanceAllow.CellStyle = styleGrey25;
                        }

                        #endregion


                        cell = cell + 1;
                        ICell cell50 = row.CreateCell(cell);
                        cell50.SetCellValue("Arrs.");
                        excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount + 1, cell, cell));
                        cell50.CellStyle = styleGrey25;

                        cell = cell + 1;
                        ICell cell51 = row.CreateCell(cell);
                        cell51.SetCellValue("Gross Wages Payable");
                        excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount + 1, cell, cell));
                        cell51.CellStyle = styleGrey25;

                        cell = cell + 1;
                        ICell cell52 = row.CreateCell(cell);
                        cell52.SetCellValue("Deductions");
                        CellUtil.SetAlignment(cell52, workbook, (short)HorizontalAlignment.Center);
                        ICell cellx51 = row.CreateCell(cell + 1);
                        cellx51.CellStyle = styleGrey25;
                        ICell cellx52 = row.CreateCell(cell + 2);
                        cellx52.CellStyle = styleGrey25;
                        ICell cellx53 = row.CreateCell(cell + 3);
                        cellx53.CellStyle = styleGrey25;
                        ICell cellx54 = row.CreateCell(cell + 4);
                        cellx54.CellStyle = styleGrey25;
                        ICell cellx55 = row.CreateCell(cell + 5);
                        cellx55.CellStyle = styleGrey25;
                        ICell cellx56 = row.CreateCell(cell + 6);
                        cellx56.CellStyle = styleGrey25;
                        excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount, cell, cell + 6));
                        cell52.CellStyle = styleGrey25;

                        ICell cell53 = row2.CreateCell(cell);
                        cell53.SetCellValue("Advance");
                        cell53.CellStyle = styleGrey25;

                        cell = cell + 1;
                        ICell cell55 = row2.CreateCell(cell);
                        cell55.SetCellValue("Profit/Income Tax/PT");
                        cell55.CellStyle = styleGrey25;
                        excelSheet.SetColumnWidth(cell, (int)((13 + 0.72) * 256));

                        cell = cell + 1;
                        ICell cell561 = row2.CreateCell(cell);
                        cell561.SetCellValue("PF");
                        cell561.CellStyle = styleGrey25;

                        cell = cell + 1;
                        ICell cell562 = row2.CreateCell(cell);
                        cell562.SetCellValue("ESIC");
                        cell562.CellStyle = styleGrey25;

                        cell = cell + 1;
                        ICell cell56 = row2.CreateCell(cell);
                        cell56.SetCellValue("MLWF");
                        cell56.CellStyle = styleGrey25;

                        cell = cell + 1;
                        ICell cell57 = row2.CreateCell(cell);
                        cell57.SetCellValue("Other Ded.");
                        cell57.CellStyle = styleGrey25;

                        cell = cell + 1;
                        ICell cell58 = row2.CreateCell(cell);
                        cell58.SetCellValue("Tot.Ded");
                        cell58.CellStyle = styleGrey25;

                        cell = cell + 1;
                        ICell cell60 = row.CreateCell(cell);
                        cell60.SetCellValue("Net Wages Paid");
                        cell60.CellStyle = styleGrey25;
                        excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount + 1, cell, cell));

                        cell = cell + 1;
                        ICell cell61 = row.CreateCell(cell);
                        cell61.SetCellValue("Leave with Wages");
                        cell61.CellStyle = styleYellowBg;

                        ICell cellx61;
                        for (int ii = 1; ii <= 4; ii++)
                        {
                            cellx61 = row.CreateCell(cell + ii);
                            cellx61.CellStyle = styleGrey25;
                        }
                        excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount, cell, cell + 4));

                        ICell cell62 = row2.CreateCell(cell);
                        cell62.SetCellValue("Previous Balance");
                        cell62.CellStyle = styleYellowBg;

                        cell = cell + 1;
                        ICell cell63 = row2.CreateCell(cell);
                        cell63.SetCellValue("Earned During Month");
                        cell63.CellStyle = styleYellowBg;
                        excelSheet.SetColumnWidth(cell, (int)((14 + 0.72) * 256));

                        cell = cell + 1;
                        ICell cell64 = row2.CreateCell(cell);
                        cell64.SetCellValue("Further Enjoyed");
                        cell64.CellStyle = styleYellowBg;
                        excelSheet.SetColumnWidth(cell, (int)((14 + 0.72) * 256));

                        cell = cell + 1;
                        ICell cell65 = row2.CreateCell(cell);
                        cell65.SetCellValue("Refused");
                        cell65.CellStyle = styleYellowBg;

                        cell = cell + 1;
                        ICell cell66 = row2.CreateCell(cell);
                        cell66.SetCellValue("Balance at the end of month");
                        cell66.CellStyle = styleYellowBg;
                        excelSheet.SetColumnWidth(cell, (int)((14 + 0.72) * 256));

                        cell = cell + 1;
                        ICell cell67 = row.CreateCell(cell);
                        cell67.SetCellValue("Date of Payment of wages");
                        cell67.CellStyle = styleGrey25;
                        excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount + 1, cell, cell));

                        cell = cell + 1;
                        ICell cell68 = row.CreateCell(cell);
                        cell68.SetCellValue("Signature of Thumb Impression of the Employee");
                        cell68.CellStyle = styleGrey25;
                        excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount + 1, cell, cell));
                        excelSheet.SetColumnWidth(cell, (int)((20 + 0.72) * 256));

                        #endregion

                        DesCount = DesCount + 2;

                        var wage_registers = wgs.Where(wg => wg.CRI_Id.Equals(cri.CRI_Id)).ToList();
                        decimal TotalEarnedBasic = 0M, TotalEarnedHRA = 0M, TotalGrossWagePayble = 0M, TotalAdvance = 0M, TotalProfTax = 0M, TotalPF = 0M, TotalESIC = 0M, TotalOtherDeduction = 0M, TotalNetWagesPaid = 0M;
                        decimal TotalOutStation = 0M, TotalAttendance = 0M, TotalNightshift = 0M, TotalPerformance = 0M, TotalLWF = 0M;
                        decimal TotalAllowance1 = 0M, TotalAllowance2 = 0M, TotalAllowance3 = 0M, TotalAllowance4 = 0M, TotalAllowance5 = 0M, TotalAllowance6 = 0M, TotalAllowance7 = 0M, TotalAllowance8 = 0M, TotalAllowance9 = 0M, TotalAllowance10 = 0M;
                        int srno = 0;
                        decimal[] otherAllowances = new decimal[cri.Client_Requirement_Allowances.Count()];

                        foreach (Wage_Register wage_register in wage_registers)
                        {
                            srno = srno + 1;

                            row = excelSheet.CreateRow(DesCount);
                            ICell cellEmp0 = row.CreateCell(0);
                            cellEmp0.SetCellValue(srno);
                            cellEmp0.CellStyle = styleGrey25;

                            ICell cellEmp1 = row.CreateCell(1);
                            cellEmp1.SetCellValue(wage_register.EMP.EMP_Id.ToString("D5"));
                            cellEmp1.CellStyle = styleGrey25;

                            ICell cellEmp2 = row.CreateCell(2);
                            cellEmp2.SetCellValue(wage_register.EMP.EMP_FirstName + " " + wage_register.EMP.EMP_MiddleName + " " + wage_register.EMP.EMP_SurName);
                            cellEmp2.CellStyle = styleGrey25;
                            excelSheet.SetColumnWidth(2, (int)((25 + 0.72) * 256));

                            ICell cellEmp3 = row.CreateCell(3);
                            cellEmp3.SetCellValue((int)((DateTime.Now.Subtract(new DateTime(wage_register.EMP.EMP_DOB.Year, wage_register.EMP.EMP_DOB.Month, wage_register.EMP.EMP_DOB.Day)).TotalDays) / 365));
                            cellEmp3.CellStyle = styleGrey25;

                            ICell cellEmp4 = row.CreateCell(4);
                            cellEmp4.SetCellValue(Convert.ToBoolean(wage_register.EMP.EMP_Gender) == true ? "M" : "F");
                            cellEmp4.CellStyle = styleGrey25;

                            ICell cellEmp5 = row.CreateCell(5);
                            cellEmp5.SetCellValue(wage_register.EMP.EMP_DateOfJoining.ToString("dd-MMM-yyyy"));
                            cellEmp5.CellStyle = styleGrey25;

                            ICell cellEmp6 = row.CreateCell(6);
                            cellEmp6.SetCellValue(wage_register.EMP.EMP_Designation);
                            cellEmp6.CellStyle = styleGrey25;

                            ICell cellEmp7 = row.CreateCell(7);
                            cellEmp7.SetCellValue("10");
                            cellEmp7.CellStyle = styleGrey25;

                            ICell cellEmp8 = row.CreateCell(8);
                            cellEmp8.SetCellValue("7");
                            cellEmp8.CellStyle = styleGrey25;

                            ICell cellEmp9 = row.CreateCell(9);
                            cellEmp9.SetCellValue("1");
                            cellEmp9.CellStyle = styleGrey25;

                            ICell cellEmp10 = row.CreateCell(10);
                            cellEmp10.SetCellValue("2");
                            cellEmp10.CellStyle = styleGrey25;

                            int cellEmpCount = 11;

                            IEnumerable<Attendance> att = attManager.GetAttendance_WageClient_Employee(WAG_Id, client.CLI_Id, wage_register.EMP_Id);

                            if (arr.Length >= 2)
                            {
                                Attendance attendance;
                                ICell cellday;
                                for (DateTime day = arr[0].Date; day <= arr[1].Date; day = day.AddDays(1))
                                {
                                    attendance = att.Where(m => m.ATT_Date.Equals(ProjectUtils.DateTimeToDate(day))).FirstOrDefault();
                                    cellday = row.CreateCell(cellEmpCount);
                                    if (attendance == null)
                                    {
                                        cellday.SetCellValue("");
                                    }
                                    else
                                    {
                                        if (string.IsNullOrEmpty(attendance.ATT_Orignal_Row2))
                                            cellday.SetCellValue(attendance.ATT_Orignal_Row1);
                                        else
                                            cellday.SetCellValue(attendance.ATT_Orignal_Row1 + "/" + attendance.ATT_Orignal_Row2);
                                    }
                                    excelSheet.SetColumnWidth(cellEmpCount, (int)((3 + 0.72) * 256));
                                    cellday.CellStyle = styleGrey25;
                                    cellEmpCount++;
                                }
                            }

                            totalAttCols = cellEmpCount - 11;
                            ICell cellEmp22 = row.CreateCell(cellEmpCount);
                            int halfDay = att.Where(m => m.ATT_IsHalfday).Count();
                            var presence = att.Where(m => m.ATT_IsPresent).Count() + (halfDay / 2) - halfDay;
                            cellEmp22.SetCellValue(presence);
                            cellEmp22.CellStyle = styleGrey25;

                            cellEmpCount = cellEmpCount + 1;
                            ICell cellEmp23 = row.CreateCell(cellEmpCount); //weekly off
                            cellEmp23.SetCellValue(att.Where(m => m.ATT_IsWeeklyOff && !m.ATT_IsPresent).Count());
                            cellEmp23.CellStyle = styleGrey25;

                            cellEmpCount = cellEmpCount + 1;
                            ICell cellEmp24 = row.CreateCell(cellEmpCount); //paid holiday
                            cellEmp24.SetCellValue(att.Where(m => !m.ATT_IsPresent && m.ATT_IsPublicHoliday).Count());
                            cellEmp24.CellStyle = styleGrey25;

                            cellEmpCount = cellEmpCount + 1;
                            ICell cellEmp244 = row.CreateCell(cellEmpCount); //Leave
                            cellEmp244.SetCellValue(att.Where(m => m.ATT_IsEarnLeave).Count());
                            cellEmp244.CellStyle = styleGrey25;

                            cellEmpCount = cellEmpCount + 1;
                            ICell cellEmp25 = row.CreateCell(cellEmpCount); //Extra Day
                            cellEmp25.SetCellValue("-");
                            cellEmp25.CellStyle = styleGrey25;

                            cellEmpCount = cellEmpCount + 1;
                            ICell cellEmp26 = row.CreateCell(cellEmpCount); //Comp Off
                            cellEmp26.SetCellValue(att.Where(m => m.ATT_Orignal_Row1 == "CO" || m.ATT_Orignal_Row1 == "C/O").Count());
                            cellEmp26.CellStyle = styleGrey25;

                            cellEmpCount = cellEmpCount + 1;
                            ICell cellEmp27 = row.CreateCell(cellEmpCount); //Adjustment
                            cellEmp27.SetCellValue("-");
                            cellEmp27.CellStyle = styleGrey25;

                            cellEmpCount = cellEmpCount + 1;
                            ICell cellEmp28 = row.CreateCell(cellEmpCount); //Absent
                            cellEmp28.SetCellValue(att.Where(m => !m.ATT_IsPresent).Count());
                            cellEmp28.CellStyle = styleGrey25;

                            cellEmpCount = cellEmpCount + 1;
                            ICell cellEmp29 = row.CreateCell(cellEmpCount); //Total Payable Days
                            cellEmp29.SetCellValue(wage_register.WAR_TotalPaybleDays);
                            cellEmp29.CellStyle = styleGrey25;

                            cellEmpCount = cellEmpCount + 1;
                            ICell cellEmp30 = row.CreateCell(cellEmpCount); //Earned Basic
                            cellEmp30.SetCellValue(Math.Round(wage_register.WAR_Basic_Calculated + wage_register.WAR_DA_Calculated, MidpointRounding.AwayFromZero).ToString());
                            TotalEarnedBasic = TotalEarnedBasic + wage_register.WAR_Basic_Calculated + wage_register.WAR_DA_Calculated;
                            cellEmp30.CellStyle = styleGrey25;

                            cellEmpCount = cellEmpCount + 1;
                            ICell cellEmp31 = row.CreateCell(cellEmpCount); //Earned HRA
                            TotalEarnedHRA = TotalEarnedHRA + wage_register.WAR_HRA_Calculated;
                            cellEmp31.SetCellValue(Math.Round(wage_register.WAR_HRA_Calculated, MidpointRounding.AwayFromZero).ToString());
                            cellEmp31.CellStyle = styleGrey25;

                            cellEmpCount = cellEmpCount + 1;
                            ICell cellEmp32 = row.CreateCell(cellEmpCount); //Earned CCA
                            cellEmp32.SetCellValue("-");
                            cellEmp32.CellStyle = styleGrey25;

                            int cellAllowance = cellEmpCount;

                            #region CLIENTS REQUIREMENT ALLOWANCES

                            List<Wage_Register_Allowance> alls = allManager.GetEmployeeAllowances_WAR_Id(wage_register.WAR_Id);
                            int flg = 0;
                            foreach (var all in cri.Client_Requirement_Allowances.OrderBy(m => m.ALL_Id))
                            {
                                cell++;
                                Wage_Register_Allowance wrl = alls.Where(w => w.CRA_Id.Equals(all.CRA_Id)).FirstOrDefault();
                                AllowanceVM obj = lstMasterOtherAllowances.Where(a => a.ALL_Id.Equals(all.ALL_Id)).FirstOrDefault();
                                if (obj != null)
                                {
                                    obj.total = obj.total + wrl.WAA_Amount_Calculated;
                                    int index = lstMasterOtherAllowances.FindIndex(s => s.ALL_Id.Equals(all.ALL_Id));
                                    if (index != -1)
                                        lstMasterOtherAllowances[index] = obj;
                                }

                                otherAllowances[flg] = otherAllowances[flg] + wrl.WAA_Amount_Calculated;
                                ICell cellAll = row.CreateCell(cell);
                                cellAll.SetCellValue(wrl != null ? Math.Round(wrl.WAA_Amount_Calculated, MidpointRounding.AwayFromZero).ToString() : "0");
                                cellAll.CellStyle = styleGrey25;
                                flg++;
                            }

                            #endregion

                            int cellNxt = cellAllowance + 1;

                            #region CUSTOM ALLOWANCES

                            if (cri.CRI_Allowance_1)
                            {
                                ICell celOutAllow = row.CreateCell(cellNxt);
                                decimal criAll = wage_register.WAR_Allowance_Calculated_1.HasValue ? Math.Round(wage_register.WAR_Allowance_Calculated_1.Value, MidpointRounding.AwayFromZero) : 0M;
                                celOutAllow.SetCellValue(criAll.ToString());
                                TotalAllowance1 += criAll;
                                celOutAllow.CellStyle = styleGrey25;
                                cellNxt++;
                            }
                            if (cri.CRI_Allowance_2)
                            {
                                ICell celOutAllow = row.CreateCell(cellNxt);
                                celOutAllow.CellStyle = styleGrey25;
                                decimal criAll = wage_register.WAR_Allowance_Calculated_2.HasValue ? Math.Round(wage_register.WAR_Allowance_Calculated_2.Value, MidpointRounding.AwayFromZero) : 0M;
                                celOutAllow.SetCellValue(criAll.ToString());
                                TotalAllowance2 += criAll;
                                cellNxt++;
                            }
                            if (cri.CRI_Allowance_3)
                            {
                                ICell celOutAllow = row.CreateCell(cellNxt);
                                celOutAllow.CellStyle = styleGrey25;
                                decimal criAll = wage_register.WAR_Allowance_Calculated_3.HasValue ? Math.Round(wage_register.WAR_Allowance_Calculated_3.Value, MidpointRounding.AwayFromZero) : 0M;
                                celOutAllow.SetCellValue(criAll.ToString());
                                TotalAllowance3 += criAll;
                                cellNxt++;
                            }
                            if (cri.CRI_Allowance_4)
                            {
                                ICell celOutAllow = row.CreateCell(cellNxt);
                                celOutAllow.CellStyle = styleGrey25;
                                decimal criAll = wage_register.WAR_Allowance_Calculated_4.HasValue ? Math.Round(wage_register.WAR_Allowance_Calculated_4.Value, MidpointRounding.AwayFromZero) : 0M;
                                celOutAllow.SetCellValue(criAll.ToString());
                                TotalAllowance4 += criAll;
                                cellNxt++;
                            }
                            if (cri.CRI_Allowance_5)
                            {
                                ICell celOutAllow = row.CreateCell(cellNxt);
                                celOutAllow.CellStyle = styleGrey25;
                                decimal criAll = wage_register.WAR_Allowance_Calculated_5.HasValue ? Math.Round(wage_register.WAR_Allowance_Calculated_5.Value, MidpointRounding.AwayFromZero) : 0M;
                                celOutAllow.SetCellValue(criAll.ToString());
                                TotalAllowance5 += criAll;
                                cellNxt++;
                            }
                            if (cri.CRI_Allowance_6)
                            {
                                ICell celOutAllow = row.CreateCell(cellNxt);
                                celOutAllow.CellStyle = styleGrey25;
                                decimal criAll = wage_register.WAR_Allowance_Calculated_6.HasValue ? Math.Round(wage_register.WAR_Allowance_Calculated_6.Value, MidpointRounding.AwayFromZero) : 0M;
                                celOutAllow.SetCellValue(criAll.ToString());
                                TotalAllowance6 += criAll;
                                cellNxt++;
                            }
                            if (cri.CRI_Allowance_7)
                            {
                                ICell celOutAllow = row.CreateCell(cellNxt);
                                celOutAllow.CellStyle = styleGrey25;
                                decimal criAll = wage_register.WAR_Allowance_Calculated_7.HasValue ? Math.Round(wage_register.WAR_Allowance_Calculated_7.Value, MidpointRounding.AwayFromZero) : 0M;
                                celOutAllow.SetCellValue(criAll.ToString());
                                TotalAllowance7 += criAll;
                                cellNxt++;
                            }
                            if (cri.CRI_Allowance_8)
                            {
                                ICell celOutAllow = row.CreateCell(cellNxt);
                                celOutAllow.CellStyle = styleGrey25;
                                decimal criAll = wage_register.WAR_Allowance_Calculated_8.HasValue ? Math.Round(wage_register.WAR_Allowance_Calculated_8.Value, MidpointRounding.AwayFromZero) : 0M;
                                celOutAllow.SetCellValue(criAll.ToString());
                                TotalAllowance8 += criAll;
                                cellNxt++;
                            }
                            if (cri.CRI_Allowance_9)
                            {
                                ICell celOutAllow = row.CreateCell(cellNxt);
                                celOutAllow.CellStyle = styleGrey25;
                                decimal criAll = wage_register.WAR_Allowance_Calculated_9.HasValue ? Math.Round(wage_register.WAR_Allowance_Calculated_9.Value, MidpointRounding.AwayFromZero) : 0M;
                                celOutAllow.SetCellValue(criAll.ToString());
                                TotalAllowance9 += criAll;
                                cellNxt++;
                            }
                            if (cri.CRI_Allowance_10)
                            {
                                ICell celOutAllow = row.CreateCell(cellNxt);
                                celOutAllow.CellStyle = styleGrey25;
                                decimal criAll = wage_register.WAR_Allowance_Calculated_10.HasValue ? Math.Round(wage_register.WAR_Allowance_Calculated_10.Value, MidpointRounding.AwayFromZero) : 0M;
                                celOutAllow.SetCellValue(criAll.ToString());
                                TotalAllowance10 += criAll;
                                cellNxt++;
                            }

                            #endregion

                            #region STATIC ALLOWANCES

                            if (cri.CRI_OutStation_Allowance == true)
                            {
                                ICell cellOutstation = row.CreateCell(cellNxt);
                                decimal WAR_OutStation_Allowance_Calculated = wage_register.WAR_OutStation_Allowance_Calculated != null ? Math.Round(wage_register.WAR_OutStation_Allowance_Calculated.Value, MidpointRounding.AwayFromZero) : 0M;
                                cellOutstation.SetCellValue(WAR_OutStation_Allowance_Calculated.ToString());
                                TotalOutStation += WAR_OutStation_Allowance_Calculated;
                                cellOutstation.CellStyle = styleGrey25;
                                cellNxt = cellNxt + 1;
                            }
                            if (cri.CRI_Attendance_Allowance == true)
                            {
                                ICell cellAttendance = row.CreateCell(cellNxt);
                                decimal WAR_Attendance_Allowance_Calculated = wage_register.WAR_Attendance_Allowance_Calculated != null ? Math.Round(wage_register.WAR_Attendance_Allowance_Calculated.Value, MidpointRounding.AwayFromZero) : 0M;
                                cellAttendance.SetCellValue(WAR_Attendance_Allowance_Calculated.ToString());
                                TotalAttendance += WAR_Attendance_Allowance_Calculated;
                                cellAttendance.CellStyle = styleGrey25;
                                cellNxt = cellNxt + 1;
                            }
                            if (cri.CRI_Nightshift_Allowance == true)
                            {
                                ICell cellNightshift = row.CreateCell(cellNxt);
                                decimal WAR_Nightshift_Allowance_Calculated = wage_register.WAR_Nightshift_Allowance_Calculated != null ? Math.Round(wage_register.WAR_Nightshift_Allowance_Calculated.Value, MidpointRounding.AwayFromZero) : 0M;
                                cellNightshift.SetCellValue(WAR_Nightshift_Allowance_Calculated.ToString());
                                TotalNightshift += WAR_Nightshift_Allowance_Calculated;
                                cellNightshift.CellStyle = styleGrey25;
                                cellNxt = cellNxt + 1;
                            }
                            if (cri.CRI_Performance_Allowance == true)
                            {
                                ICell cellPerformance = row.CreateCell(cellNxt);
                                decimal WAR_Performance_Allowance_Calculated = wage_register.WAR_Performance_Allowance_Calculated != null ? Math.Round(wage_register.WAR_Performance_Allowance_Calculated.Value, MidpointRounding.AwayFromZero) : 0M;
                                cellPerformance.SetCellValue(WAR_Performance_Allowance_Calculated.ToString());
                                TotalPerformance += WAR_Performance_Allowance_Calculated;
                                cellPerformance.CellStyle = styleGrey25;
                                cellNxt = cellNxt + 1;
                            }

                            #endregion

                            TotalGrossWagePayble = TotalGrossWagePayble + Math.Round(wage_register.WAR_GrossTotal, MidpointRounding.AwayFromZero);
                            decimal DeductTotal = Math.Round(wage_register.WAR_PF_Calculated, MidpointRounding.AwayFromZero) + Math.Round(wage_register.WAR_ESIC_Calculated, MidpointRounding.AwayFromZero) + Math.Round(Convert.ToDecimal(wage_register.WAR_ProffesionalTax_Calculated), MidpointRounding.AwayFromZero)
                                + Math.Round(wage_register.WAR_Advance_Amount ?? 0, MidpointRounding.AwayFromZero) + Math.Round((wage_register.WAR_LWF_Deduction_Employee != null ? wage_register.WAR_LWF_Deduction_Employee.Value : 0), MidpointRounding.AwayFromZero);

                            ICell cellEmp14 = row.CreateCell(cellNxt);
                            cellEmp14.SetCellValue("-"); //Arrs.									
                            cellEmp14.CellStyle = styleGrey25;

                            ICell cellEmp15 = row.CreateCell(cellNxt + 1);
                            cellEmp15.SetCellValue(Math.Round(wage_register.WAR_GrossTotal, MidpointRounding.AwayFromZero).ToString()); //Gross Wages Payable
                            cellEmp15.CellStyle = styleGrey25;

                            ICell cellEmp16 = row.CreateCell(cellNxt + 2);
                            cellEmp16.SetCellValue(Math.Round(wage_register.WAR_Advance_Amount ?? 0, MidpointRounding.AwayFromZero).ToString()); //Advance
                            cellEmp16.CellStyle = styleGrey25;
                            TotalAdvance = TotalAdvance + Math.Round(wage_register.WAR_Advance_Amount ?? 0, MidpointRounding.AwayFromZero);

                            int cellNxt1 = cellNxt + 2;

                            cellNxt1 = cellNxt1 + 1;
                            ICell cellEmp181 = row.CreateCell(cellNxt1);
                            cellEmp181.SetCellValue(Convert.ToString(wage_register.WAR_ProffesionalTax_Calculated)); //Profit/Income Tax/PT	
                            TotalProfTax = TotalProfTax + Convert.ToDecimal(wage_register.WAR_ProffesionalTax_Calculated);
                            cellEmp181.CellStyle = styleGrey25;

                            cellNxt1 = cellNxt1 + 1;
                            ICell cellEmp182 = row.CreateCell(cellNxt1);
                            cellEmp182.SetCellValue(Math.Round(wage_register.WAR_PF_Calculated, MidpointRounding.AwayFromZero).ToString()); //PF								
                            TotalPF = TotalPF + wage_register.WAR_PF_Calculated;
                            cellEmp182.CellStyle = styleGrey25;

                            cellNxt1 = cellNxt1 + 1;
                            ICell cellEmp183 = row.CreateCell(cellNxt1);
                            cellEmp183.SetCellValue(Math.Round(wage_register.WAR_ESIC_Calculated, MidpointRounding.AwayFromZero).ToString()); //ESIC								
                            TotalESIC = TotalESIC + wage_register.WAR_ESIC_Calculated;
                            cellEmp183.CellStyle = styleGrey25;

                            cellNxt1 = cellNxt1 + 1;
                            ICell cellEmp19 = row.CreateCell(cellNxt1);
                            decimal mlwf = wage_register.WAR_LWF_Deduction_Employee != null ? wage_register.WAR_LWF_Deduction_Employee.Value : 0;
                            cellEmp19.SetCellValue(Math.Round(mlwf, MidpointRounding.AwayFromZero).ToString()); //MLWF							
                            cellEmp19.CellStyle = styleGrey25;
                            TotalLWF = TotalLWF + Convert.ToDecimal(wage_register.WAR_LWF_Deduction_Employee);

                            #region Total Deduction
                            if (wage_register.WAR_RevenueDeduction_Calculated != "-")
                            {
                                DeductTotal += Math.Round(Convert.ToDecimal(wage_register.WAR_RevenueDeduction_Calculated), MidpointRounding.AwayFromZero);
                            }
                            if (wage_register.WAR_CanteenFacility_Calculation != "-")
                            {
                                DeductTotal += Math.Round(Convert.ToDecimal(wage_register.WAR_CanteenFacility_Calculation), MidpointRounding.AwayFromZero);
                            }
                            #endregion

                            cellNxt1 = cellNxt1 + 1;
                            ICell cellEmp20 = row.CreateCell(cellNxt1);
                            cellEmp20.SetCellValue(DeductTotal.ToString()); //Other Ded.							
                            cellEmp20.CellStyle = styleGrey25;
                            TotalOtherDeduction += DeductTotal;

                            cellNxt1 = cellNxt1 + 1;
                            ICell cellEmp21 = row.CreateCell(cellNxt1);
                            cellEmp21.SetCellValue(DeductTotal.ToString()); //Tot.Ded.							
                            cellEmp21.CellStyle = styleGrey25;

                            cellNxt1 = cellNxt1 + 1;
                            //Net Wages Paid
                            ICell cellEmp222 = row.CreateCell(cellNxt1);
                            cellEmp222.SetCellValue(Math.Round(Convert.ToDecimal(wage_register.WAR_FinalTotal), MidpointRounding.AwayFromZero).ToString());
                            TotalNetWagesPaid += (wage_register.WAR_FinalTotal ?? 0);
                            cellEmp222.CellStyle = styleGrey25;

                            cellNxt1 = cellNxt1 + 1;
                            ICell cellEmp223 = row.CreateCell(cellNxt1);
                            cellEmp223.SetCellValue("0"); //Previous Balance							
                            cellEmp223.CellStyle = styleGrey25;

                            cellNxt1 = cellNxt1 + 1;
                            ICell cellEmp224 = row.CreateCell(cellNxt1);
                            cellEmp224.SetCellValue("0"); //Earned During Month						
                            cellEmp224.CellStyle = styleGrey25;

                            cellNxt1 = cellNxt1 + 1;
                            ICell cellEmp225 = row.CreateCell(cellNxt1);
                            cellEmp225.SetCellValue("0"); //Further Enjoyed					
                            cellEmp225.CellStyle = styleGrey25;

                            cellNxt1 = cellNxt1 + 1;
                            ICell cellEmp226 = row.CreateCell(cellNxt1);
                            cellEmp226.SetCellValue("0"); //Refused							
                            cellEmp226.CellStyle = styleGrey25;

                            cellNxt1 = cellNxt1 + 1;
                            ICell cellEmp227 = row.CreateCell(cellNxt1);
                            cellEmp227.SetCellValue("0"); //Balance at the end of month							
                            cellEmp227.CellStyle = styleGrey25;

                            cellNxt1 = cellNxt1 + 1;
                            ICell cellEmp228 = row.CreateCell(cellNxt1);
                            cellEmp228.SetCellValue(new DateTime(wage_Process.WAG_Month.Year, wage_Process.WAG_Month.Month, 1).AddMonths(1).AddDays(-1).ToString("dd-MMM-yyyy")); //Date of Payment of wages (Last date of month)							
                            cellEmp228.CellStyle = styleGrey25;

                            cellNxt1 = cellNxt1 + 1;
                            ICell cellEmp229 = row.CreateCell(cellNxt1);
                            cellEmp229.SetCellValue(""); //Signature of Thumb Impression of the Employee							
                            cellEmp229.CellStyle = styleGrey25;

                            DesCount++;
                        }

                        #region TOTAL LINE IN EXCEL
                        int totalCount = totalAttCols + 9 + 10;
                        row = excelSheet.CreateRow(DesCount);
                        excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount, 0, totalCount));
                        ICell cellTot;
                        for (int j = 0; j < totalCount; j++)
                        {
                            cellTot = row.CreateCell(j);
                            cellTot.SetCellValue("TOTAL");
                            cellTot.CellStyle = styleTotal;
                            CellUtil.SetAlignment(cellTot, workbook, (short)HorizontalAlignment.Center);
                        }

                        ICell cellTot2 = row.CreateCell(totalCount + 1);
                        cellTot2.SetCellValue(Math.Round(TotalEarnedBasic, MidpointRounding.AwayFromZero).ToString());
                        cellTot2.CellStyle = styleTotal;
                        ICell cellTot3 = row.CreateCell(totalCount + 2);
                        cellTot3.SetCellValue(Math.Round(TotalEarnedHRA, MidpointRounding.AwayFromZero).ToString());
                        cellTot3.CellStyle = styleTotal;
                        ICell cellTot4 = row.CreateCell(totalCount + 3);
                        cellTot4.SetCellValue("-");
                        cellTot4.CellStyle = styleTotal;

                        int cellAllow = totalCount + 3;

                        #region CLIENTS REQUIREMENT ALLOWANCES TOTALS

                        foreach (decimal d in otherAllowances)
                        {
                            ICell cellEmpAll = row.CreateCell(cellAllow + 1);
                            cellEmpAll.SetCellValue(Math.Round(d, MidpointRounding.AwayFromZero).ToString());
                            cellEmpAll.CellStyle = styleTotal;
                            cellAllow++;
                        }

                        #endregion

                        totalCount = cellAllow;

                        #region CUSTOM ALLOWANCES TOTALS

                        if (cri.CRI_Allowance_1)
                        {
                            totalCount++;
                            ICell cellTotAllowance_1 = row.CreateCell(totalCount);
                            cellTotAllowance_1.CellStyle = styleTotal;
                            cellTotAllowance_1.SetCellValue(Math.Round(TotalAllowance1, MidpointRounding.AwayFromZero).ToString());
                            lstMasterCustomeAllowancesAmount[0] += TotalAllowance1;
                        }
                        if (cri.CRI_Allowance_2)
                        {
                            totalCount++;
                            ICell cellTotAllowance_2 = row.CreateCell(totalCount);
                            cellTotAllowance_2.CellStyle = styleTotal;
                            cellTotAllowance_2.SetCellValue(Math.Round(TotalAllowance2, MidpointRounding.AwayFromZero).ToString());
                            lstMasterCustomeAllowancesAmount[1] += TotalAllowance2;
                        }
                        if (cri.CRI_Allowance_3)
                        {
                            totalCount++;
                            ICell cellTotAllowance_3 = row.CreateCell(totalCount);
                            cellTotAllowance_3.CellStyle = styleTotal;
                            cellTotAllowance_3.SetCellValue(Math.Round(TotalAllowance3, MidpointRounding.AwayFromZero).ToString());
                            lstMasterCustomeAllowancesAmount[2] += TotalAllowance3;
                        }
                        if (cri.CRI_Allowance_4)
                        {
                            totalCount++;
                            ICell cellTotAllowance_4 = row.CreateCell(totalCount);
                            cellTotAllowance_4.CellStyle = styleTotal;
                            cellTotAllowance_4.SetCellValue(Math.Round(TotalAllowance4, MidpointRounding.AwayFromZero).ToString());
                            lstMasterCustomeAllowancesAmount[3] += TotalAllowance4;
                        }
                        if (cri.CRI_Allowance_5)
                        {
                            //IsCRI_Allowance_1 = true;
                            totalCount++;
                            ICell cellTotAllowance_5 = row.CreateCell(totalCount);
                            cellTotAllowance_5.CellStyle = styleTotal;
                            cellTotAllowance_5.SetCellValue(Math.Round(TotalAllowance5, MidpointRounding.AwayFromZero).ToString());
                            lstMasterCustomeAllowancesAmount[4] += TotalAllowance5;
                        }
                        if (cri.CRI_Allowance_6)
                        {
                            totalCount++;
                            ICell cellTotAllowance_6 = row.CreateCell(totalCount);
                            cellTotAllowance_6.CellStyle = styleTotal;
                            cellTotAllowance_6.SetCellValue(Math.Round(TotalAllowance6, MidpointRounding.AwayFromZero).ToString());
                            lstMasterCustomeAllowancesAmount[5] += TotalAllowance6;
                        }
                        if (cri.CRI_Allowance_7)
                        {
                            totalCount++;
                            ICell cellTotAllowance_7 = row.CreateCell(totalCount);
                            cellTotAllowance_7.CellStyle = styleTotal;
                            cellTotAllowance_7.SetCellValue(Math.Round(TotalAllowance7, MidpointRounding.AwayFromZero).ToString());
                            lstMasterCustomeAllowancesAmount[6] += TotalAllowance7;
                        }
                        if (cri.CRI_Allowance_8)
                        {
                            totalCount++;
                            ICell cellTotAllowance_8 = row.CreateCell(totalCount);
                            cellTotAllowance_8.CellStyle = styleTotal;
                            cellTotAllowance_8.SetCellValue(Math.Round(TotalAllowance8, MidpointRounding.AwayFromZero).ToString());
                            lstMasterCustomeAllowancesAmount[7] += TotalAllowance8;
                        }
                        if (cri.CRI_Allowance_9)
                        {
                            totalCount++;
                            ICell cellTotAllowance_9 = row.CreateCell(totalCount);
                            cellTotAllowance_9.CellStyle = styleTotal;
                            cellTotAllowance_9.SetCellValue(Math.Round(TotalAllowance9, MidpointRounding.AwayFromZero).ToString());
                            lstMasterCustomeAllowancesAmount[8] += TotalAllowance9;
                        }
                        if (cri.CRI_Allowance_10)
                        {
                            totalCount++;
                            ICell cellTotAllowance_10 = row.CreateCell(totalCount);
                            cellTotAllowance_10.CellStyle = styleTotal;
                            cellTotAllowance_10.SetCellValue(Math.Round(TotalAllowance10, MidpointRounding.AwayFromZero).ToString());
                            lstMasterCustomeAllowancesAmount[9] += TotalAllowance10;
                        }

                        #endregion

                        #region STATIC ALLOWANCES TOTALS

                        if (cri.CRI_OutStation_Allowance == true)
                        {
                            IsCRI_OutStation_Allowance = true;
                            totalCount = totalCount + 1;
                            ICell cellTotOutstation = row.CreateCell(totalCount);
                            cellTotOutstation.CellStyle = styleTotal;
                            cellTotOutstation.SetCellValue(Math.Round(TotalOutStation, MidpointRounding.AwayFromZero).ToString());
                        }
                        if (cri.CRI_Attendance_Allowance == true)
                        {
                            IsCRI_Attendance_Allowance = true;
                            totalCount = totalCount + 1;
                            ICell cellTotAttendance = row.CreateCell(totalCount);
                            cellTotAttendance.CellStyle = styleTotal;
                            cellTotAttendance.SetCellValue(Math.Round(TotalAttendance, MidpointRounding.AwayFromZero).ToString());
                        }
                        if (cri.CRI_Nightshift_Allowance == true)
                        {
                            IsCRI_Nightshift_Allowance = true;
                            totalCount = totalCount + 1;
                            ICell cellTotNightshift = row.CreateCell(totalCount);
                            cellTotNightshift.CellStyle = styleTotal;
                            cellTotNightshift.SetCellValue(Math.Round(TotalNightshift, MidpointRounding.AwayFromZero).ToString());
                        }
                        if (cri.CRI_Performance_Allowance == true)
                        {
                            IsCRI_Performance_Allowance = true;
                            totalCount = totalCount + 1;
                            ICell cellTotPerformance = row.CreateCell(totalCount);
                            cellTotPerformance.CellStyle = styleTotal;
                            cellTotPerformance.SetCellValue(Math.Round(TotalPerformance, MidpointRounding.AwayFromZero).ToString());
                        }

                        #endregion

                        ICell cellTot6 = row.CreateCell(totalCount + 1);
                        cellTot6.SetCellValue("-");
                        cellTot6.CellStyle = styleTotal;
                        ICell cellTot7 = row.CreateCell(totalCount + 2);
                        cellTot7.SetCellValue(Math.Round(TotalGrossWagePayble, MidpointRounding.AwayFromZero).ToString());
                        cellTot7.CellStyle = styleTotal;
                        ICell cellTot8 = row.CreateCell(totalCount + 3);
                        cellTot8.SetCellValue(Math.Round(TotalAdvance, MidpointRounding.AwayFromZero).ToString());
                        cellTot8.CellStyle = styleTotal;
                        ICell cellTot81 = row.CreateCell(totalCount + 4);
                        cellTot81.SetCellValue(Math.Round(TotalProfTax, MidpointRounding.AwayFromZero).ToString());
                        cellTot81.CellStyle = styleTotal;
                        ICell cellTot82 = row.CreateCell(totalCount + 5);
                        cellTot82.SetCellValue(Math.Round(TotalPF, MidpointRounding.AwayFromZero).ToString());
                        cellTot82.CellStyle = styleTotal;
                        ICell cellTot83 = row.CreateCell(totalCount + 6);
                        cellTot83.SetCellValue(Math.Round(TotalESIC, MidpointRounding.AwayFromZero).ToString());
                        cellTot83.CellStyle = styleTotal;
                        ICell cellTot84 = row.CreateCell(totalCount + 7);
                        cellTot84.SetCellValue(Math.Round(TotalLWF, MidpointRounding.AwayFromZero).ToString());
                        cellTot84.CellStyle = styleTotal;
                        ICell cellTot85 = row.CreateCell(totalCount + 8);
                        cellTot85.SetCellValue(Math.Round(TotalOtherDeduction, MidpointRounding.AwayFromZero).ToString());
                        cellTot85.CellStyle = styleTotal;
                        ICell cellTot851 = row.CreateCell(totalCount + 9);
                        cellTot851.SetCellValue(Math.Round(TotalOtherDeduction, MidpointRounding.AwayFromZero).ToString());
                        cellTot851.CellStyle = styleTotal;
                        ICell cellTot86 = row.CreateCell(totalCount + 10);
                        cellTot86.SetCellValue(Math.Round(TotalNetWagesPaid, MidpointRounding.AwayFromZero).ToString());
                        cellTot86.CellStyle = styleTotal;

                        #endregion

                        DesCount = DesCount + 1;

                        Final_TotalEarnedBasic = Final_TotalEarnedBasic + TotalEarnedBasic;
                        Final_TotalEarnedHRA = Final_TotalEarnedHRA + TotalEarnedHRA;
                        Final_TotalGrossWagePayble = Final_TotalGrossWagePayble + TotalGrossWagePayble;
                        Final_TotalAdvance = Final_TotalAdvance + TotalAdvance;
                        Final_TotalProfTax = Final_TotalProfTax + TotalProfTax;
                        Final_TotalPF = Final_TotalPF + TotalPF;
                        Final_TotalESIC = Final_TotalESIC + TotalESIC;
                        Final_TotalLWF = Final_TotalLWF + TotalLWF;
                        Final_TotalOtherDeduction = Final_TotalOtherDeduction + TotalOtherDeduction;
                        Final_TotalNetWagesPaid = Final_TotalNetWagesPaid + TotalNetWagesPaid;
                        Final_TotalOutStation = Final_TotalOutStation + TotalOutStation;
                        Final_TotalAttendance = Final_TotalAttendance + TotalAttendance;
                        Final_TotalNightshift = Final_TotalNightshift + TotalNightshift;
                        Final_TotalPerformance = Final_TotalPerformance + TotalPerformance;
                    }

                    DesCount = DesCount + 1;

                    #region GRAND TOTALS

                    IRow frow = excelSheet.CreateRow(DesCount);
                    IRow frowSecond = excelSheet.CreateRow(DesCount + 1);
                    int ftotalCount = 10 + totalAttCols + 9;
                    excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount + 1, 0, ftotalCount));

                    ICell fcellTotsSecond;
                    ICell fcellTot;
                    for (int j = 0; j < ftotalCount; j++)
                    {
                        fcellTot = frow.CreateCell(j);
                        fcellTot.SetCellValue("");
                        fcellTot.CellStyle = styleTotal;
                        CellUtil.SetAlignment(fcellTot, workbook, (short)HorizontalAlignment.Center);

                        fcellTotsSecond = frowSecond.CreateCell(j);
                        fcellTotsSecond.SetCellValue("GRAND TOTAL");
                        fcellTotsSecond.CellStyle = styleTotal;
                        CellUtil.SetAlignment(fcellTotsSecond, workbook, (short)HorizontalAlignment.Center);
                    }
                    ftotalCount = ftotalCount + 1;


                    ICell fcellTot1 = frow.CreateCell(ftotalCount);
                    fcellTot1.SetCellValue("Total Earned Basic");
                    fcellTot1.CellStyle = styleTotal;
                    ICell fcellTot1s = frowSecond.CreateCell(ftotalCount);
                    fcellTot1s.SetCellValue(Final_TotalEarnedBasic.ToString());
                    fcellTot1s.CellStyle = styleTotal;
                    ftotalCount = ftotalCount + 1;


                    ICell fcellTot2 = frow.CreateCell(ftotalCount);
                    fcellTot2.SetCellValue("Total Earned HRA");
                    fcellTot2.CellStyle = styleTotal;
                    ICell fcellTot2s = frowSecond.CreateCell(ftotalCount);
                    fcellTot2s.SetCellValue(Math.Round(Final_TotalEarnedHRA, MidpointRounding.AwayFromZero).ToString());
                    fcellTot2s.CellStyle = styleTotal;
                    ftotalCount = ftotalCount + 1;

                    ICell fcellTot3 = frow.CreateCell(ftotalCount);
                    fcellTot3.SetCellValue("Total Earned CCA");
                    fcellTot3.CellStyle = styleTotal;
                    ICell fcellTot3s = frowSecond.CreateCell(ftotalCount);
                    fcellTot3s.SetCellValue("0");
                    fcellTot3s.CellStyle = styleTotal;
                    ftotalCount = ftotalCount + 1;

                    #region CLIENTS REQUIREMNETS ALLOWANCES | GRAND TOTALS

                    foreach (var t in lstMasterOtherAllowances.Where(dd => dd.total > 0))
                    {
                        ICell fcellTotOA = frow.CreateCell(ftotalCount);
                        fcellTotOA.SetCellValue(t.ALL_Alias);
                        ICellStyle tt = styleTotal;
                        tt.FillForegroundColor = IndexedColors.DarkGreen.Index;
                        fcellTotOA.CellStyle = tt;

                        ICell fcellTotOAs = frowSecond.CreateCell(ftotalCount);
                        fcellTotOAs.SetCellValue(Math.Round(t.total, MidpointRounding.AwayFromZero).ToString());
                        fcellTotOAs.CellStyle = styleTotal;
                        ftotalCount = ftotalCount + 1;
                    }

                    #endregion

                    #region CUSTOM ALLOWANCES | GRAND TOTALS

                    for (int i = 0; i < 10; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(lstMasterCustomeAllowancesAlias[i]))
                        {
                            ICell fcellTotCA = frow.CreateCell(ftotalCount);
                            fcellTotCA.SetCellValue(lstMasterCustomeAllowancesAlias[i]);
                            ICellStyle tt = styleTotal;
                            tt.FillForegroundColor = IndexedColors.DarkBlue.Index;
                            fcellTotCA.CellStyle = tt;

                            ICell fcellTotOAs = frowSecond.CreateCell(ftotalCount);
                            fcellTotOAs.SetCellValue(lstMasterCustomeAllowancesAmount[i] > 0 ? Math.Round(lstMasterCustomeAllowancesAmount[i], MidpointRounding.AwayFromZero).ToString() : "0");
                            fcellTotOAs.CellStyle = styleTotal;
                            ftotalCount = ftotalCount + 1;
                        }
                    }

                    #endregion

                    #region STATIC ALLOWANCES | GRAND TOTALS

                    if (IsCRI_OutStation_Allowance)
                    {
                        ICell cellTotOutstation = frow.CreateCell(ftotalCount);
                        cellTotOutstation.CellStyle = styleTotal;
                        cellTotOutstation.SetCellValue("Total OutStation");
                        ICell cellTotOutstations = frowSecond.CreateCell(ftotalCount);
                        cellTotOutstations.CellStyle = styleTotal;
                        cellTotOutstations.SetCellValue(Math.Round(Final_TotalOutStation, MidpointRounding.AwayFromZero).ToString());
                        ftotalCount = ftotalCount + 1;
                    }
                    if (IsCRI_Attendance_Allowance)
                    {
                        ICell cellTotAttendance = frow.CreateCell(ftotalCount);
                        cellTotAttendance.CellStyle = styleTotal;
                        cellTotAttendance.SetCellValue("Total Attendance");
                        ICell cellTotAttendances = frowSecond.CreateCell(ftotalCount);
                        cellTotAttendances.CellStyle = styleTotal;
                        cellTotAttendances.SetCellValue(Math.Round(Final_TotalAttendance, MidpointRounding.AwayFromZero).ToString());
                        ftotalCount = ftotalCount + 1;
                    }
                    if (IsCRI_Nightshift_Allowance)
                    {
                        ICell cellTotNightshift = frow.CreateCell(ftotalCount);
                        cellTotNightshift.CellStyle = styleTotal;
                        cellTotNightshift.SetCellValue("Total Nightshift");
                        ICell cellTotNightshifts = frowSecond.CreateCell(ftotalCount);
                        cellTotNightshifts.CellStyle = styleTotal;
                        cellTotNightshifts.SetCellValue(Math.Round(Final_TotalNightshift, MidpointRounding.AwayFromZero).ToString());
                        ftotalCount = ftotalCount + 1;
                    }
                    if (IsCRI_Performance_Allowance)
                    {
                        ICell cellTotPerformance = frow.CreateCell(ftotalCount);
                        cellTotPerformance.CellStyle = styleTotal;
                        cellTotPerformance.SetCellValue("Total Performance");
                        ICell cellTotPerformances = frowSecond.CreateCell(ftotalCount);
                        cellTotPerformances.CellStyle = styleTotal;
                        cellTotPerformances.SetCellValue(Math.Round(Final_TotalPerformance, MidpointRounding.AwayFromZero).ToString());
                        ftotalCount = ftotalCount + 1;
                    }

                    #endregion

                    ICell fcellTotGross = frow.CreateCell(ftotalCount);
                    fcellTotGross.SetCellValue("Total Arrs");
                    fcellTotGross.CellStyle = styleTotal;
                    ICell fcellTotGrosss = frowSecond.CreateCell(ftotalCount);
                    fcellTotGrosss.SetCellValue("0");
                    fcellTotGrosss.CellStyle = styleTotal;
                    ftotalCount = ftotalCount + 1;

                    ICell fcellTotgwp = frow.CreateCell(ftotalCount);
                    fcellTotgwp.SetCellValue("Total Gross Wages Payable");
                    fcellTotgwp.CellStyle = styleTotal;
                    ICell fcellTotgwps = frowSecond.CreateCell(ftotalCount);
                    fcellTotgwps.SetCellValue(Math.Round(Final_TotalGrossWagePayble, MidpointRounding.AwayFromZero).ToString());
                    fcellTotgwps.CellStyle = styleTotal;
                    ftotalCount = ftotalCount + 1;

                    ICell fcellTotAdvance = frow.CreateCell(ftotalCount);
                    fcellTotAdvance.SetCellValue("Total Advance");
                    fcellTotAdvance.CellStyle = styleTotal;
                    ICell fcellTotAdvances = frowSecond.CreateCell(ftotalCount);
                    fcellTotAdvances.SetCellValue(Math.Round(Final_TotalAdvance, MidpointRounding.AwayFromZero).ToString());
                    fcellTotAdvances.CellStyle = styleTotal;
                    ftotalCount = ftotalCount + 1;

                    ICell fcellTotProff = frow.CreateCell(ftotalCount);
                    fcellTotProff.SetCellValue("Total Profit/Income Tax/PT");
                    fcellTotProff.CellStyle = styleTotal;
                    ICell fcellTotProffs = frowSecond.CreateCell(ftotalCount);
                    fcellTotProffs.SetCellValue(Math.Round(Final_TotalProfTax, MidpointRounding.AwayFromZero).ToString());
                    fcellTotProffs.CellStyle = styleTotal;
                    ftotalCount = ftotalCount + 1;

                    ICell fcellTotPF = frow.CreateCell(ftotalCount);
                    fcellTotPF.SetCellValue("Total PF");
                    fcellTotPF.CellStyle = styleTotal;
                    ICell fcellTotPFs = frowSecond.CreateCell(ftotalCount);
                    fcellTotPFs.SetCellValue(Math.Round(Final_TotalPF, MidpointRounding.AwayFromZero).ToString());
                    fcellTotPFs.CellStyle = styleTotal;
                    ftotalCount = ftotalCount + 1;

                    ICell fcellTotESIC = frow.CreateCell(ftotalCount);
                    fcellTotESIC.SetCellValue("Total ESIC");
                    fcellTotESIC.CellStyle = styleTotal;
                    ICell fcellTotESICs = frowSecond.CreateCell(ftotalCount);
                    fcellTotESICs.SetCellValue(Math.Round(Final_TotalESIC, MidpointRounding.AwayFromZero).ToString());
                    fcellTotESICs.CellStyle = styleTotal;
                    ftotalCount = ftotalCount + 1;

                    ICell fcellTotMLWF = frow.CreateCell(ftotalCount);
                    fcellTotMLWF.SetCellValue("Total MLWF");
                    fcellTotMLWF.CellStyle = styleTotal;
                    ICell fcellTotMLWFs = frowSecond.CreateCell(ftotalCount);
                    fcellTotMLWFs.SetCellValue(Math.Round(Final_TotalLWF, MidpointRounding.AwayFromZero).ToString());
                    fcellTotMLWFs.CellStyle = styleTotal;
                    ftotalCount = ftotalCount + 1;

                    ICell fcellTotOtherDed = frow.CreateCell(ftotalCount);
                    fcellTotOtherDed.SetCellValue("Total Other Deduction");
                    fcellTotOtherDed.CellStyle = styleTotal;
                    ICell fcellTotOtherDeds = frowSecond.CreateCell(ftotalCount);
                    fcellTotOtherDeds.SetCellValue(Math.Round(Final_TotalOtherDeduction, MidpointRounding.AwayFromZero).ToString());
                    fcellTotOtherDeds.CellStyle = styleTotal;
                    ftotalCount = ftotalCount + 1;

                    ICell fcellTotTotDed = frow.CreateCell(ftotalCount);
                    fcellTotTotDed.SetCellValue("Total Deduction");
                    fcellTotTotDed.CellStyle = styleTotal;
                    ICell fcellTotTotDeds = frowSecond.CreateCell(ftotalCount);
                    fcellTotTotDeds.SetCellValue(Math.Round(Final_TotalOtherDeduction, MidpointRounding.AwayFromZero).ToString());
                    fcellTotTotDeds.CellStyle = styleTotal;
                    ftotalCount = ftotalCount + 1;

                    ICell fcellTotNetWagesPaid = frow.CreateCell(ftotalCount);
                    fcellTotNetWagesPaid.SetCellValue("Total Net Wages Paid");
                    fcellTotNetWagesPaid.CellStyle = styleTotal;
                    ICell fcellTotNetWagesPaids = frowSecond.CreateCell(ftotalCount);
                    fcellTotNetWagesPaids.SetCellValue(Math.Round(Final_TotalNetWagesPaid, MidpointRounding.AwayFromZero).ToString());
                    fcellTotNetWagesPaids.CellStyle = styleTotal;
                    
                    #endregion

                }
                workbook.Write(fs);
            }
            using (var stream = new FileStream(Path.Combine(newPath, fileName), FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            new FileInfo(Path.Combine(newPath, fileName)).Delete();
            return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        public async Task<FileResult> WageRegisterExcel(int WAG_Id, int FRM_Id)
        {
            WageProcessManager wageProcess = new(_context);
            AllowanceManager allManager = new(_context);
            ClientsManager cliManager = new(_context);
            Wage_Process wage_Process = wageProcess.GetWageProcess_WAG_Id(WAG_Id);
            string WAG_Month = wage_Process.WAG_Month.ToString("MMMM").ToUpper() + "-" + wage_Process.WAG_Month.ToString("yyyy");

            string newPath = GetTempFolderPath(_hostingEnvironment.WebRootPath);
            string fileName = "Wage_Register_" + DateTime.Now.ToString("ddMMyyyyHHmm") + "_" + WAG_Month + ".xlsx";
            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, fileName);
            FileInfo file = new(Path.Combine(newPath, fileName));
            var memory = new MemoryStream();
            using (var fs = new FileStream(Path.Combine(newPath, fileName), FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook = new XSSFWorkbook();
                ISheet excelSheet = null;
                foreach (var client in wage_Process.Wage_Process_Clients.OrderBy(c => c.CLI.CLI_RegisteredOn))
                {
                    string sheetName = client.CLI_Id.ToString() + "-" + WorkbookUtil.CreateSafeSheetName((client.CLI.CLI_Name.ToString().Length > 20 ? client.CLI.CLI_Name.ToString().Substring(0, 20) : client.CLI.CLI_Name.ToString()));
                    excelSheet = workbook.CreateSheet(sheetName);

                    List<AllowanceVM> lstMasterOtherAllowances = AllowanceVM.MapMes(allManager.GetAllowanceList().OrderBy(m => m.ALL_Id).ToList());
                    string[] lstMasterCustomeAllowancesAlias = new string[10];
                    decimal[] lstMasterCustomeAllowancesAmount = new decimal[10];

                    #region STYLE OF EXCEL
                    ICellStyle style = workbook.CreateCellStyle();
                    ICellStyle styleDesignation = workbook.CreateCellStyle();
                    ICellStyle styleHeading = workbook.CreateCellStyle();
                    ICellStyle styleTotal = workbook.CreateCellStyle();

                    ICellStyle styleGrey25 = workbook.CreateCellStyle();
                    ICellStyle styleGrey40 = workbook.CreateCellStyle();
                    ICellStyle styleGrey50 = workbook.CreateCellStyle();
                    ICellStyle styleGrey80 = workbook.CreateCellStyle();

                    IFont fontcell = workbook.CreateFont();
                    //fontcell.IsBold = true;

                    IFont font = workbook.CreateFont();
                    font.IsBold = true;
                    font.FontHeightInPoints = ((short)14);
                    font.FontName = ("Cambria");

                    IFont fontTotal = workbook.CreateFont();
                    fontTotal.IsBold = true;
                    fontTotal.FontHeightInPoints = ((short)10);
                    fontTotal.FontName = ("Cambria");

                    IFont fontHeading = workbook.CreateFont();
                    fontHeading.IsBold = true;
                    fontHeading.FontHeightInPoints = ((short)24);
                    fontHeading.FontName = ("Cambria");

                    styleGrey25.WrapText = true;
                    styleGrey25.VerticalAlignment = VerticalAlignment.Center;
                    styleGrey25.BorderBottom = (BorderStyle.Thin);
                    styleGrey25.BottomBorderColor = (IndexedColors.Black.Index);
                    styleGrey25.BorderLeft = (BorderStyle.Thin);
                    styleGrey25.LeftBorderColor = (IndexedColors.Black.Index);
                    styleGrey25.BorderRight = (BorderStyle.Thin);
                    styleGrey25.RightBorderColor = (IndexedColors.Black.Index);
                    styleGrey25.BorderTop = (BorderStyle.Thin);
                    styleGrey25.TopBorderColor = (IndexedColors.Black.Index);
                    styleGrey25.FillForegroundColor = IndexedColors.White.Index;
                    styleGrey25.FillPattern = FillPattern.SolidForeground;
                    styleGrey25.FillBackgroundColor = HSSFColor.White.Index;


                    styleGrey40.WrapText = true;
                    styleGrey40.VerticalAlignment = VerticalAlignment.Center;
                    styleGrey40.BorderBottom = (BorderStyle.Thin);
                    styleGrey40.BottomBorderColor = (IndexedColors.Black.Index);
                    styleGrey40.BorderLeft = (BorderStyle.Thin);
                    styleGrey40.LeftBorderColor = (IndexedColors.Black.Index);
                    styleGrey40.BorderRight = (BorderStyle.Thin);
                    styleGrey40.RightBorderColor = (IndexedColors.Black.Index);
                    styleGrey40.BorderTop = (BorderStyle.Thin);
                    styleGrey40.TopBorderColor = (IndexedColors.Black.Index);
                    styleGrey40.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                    styleGrey40.FillPattern = FillPattern.SolidForeground;
                    styleGrey40.FillBackgroundColor = HSSFColor.Grey25Percent.Index;

                    styleGrey50.WrapText = true;
                    styleGrey50.VerticalAlignment = VerticalAlignment.Center;
                    styleGrey50.BorderBottom = (BorderStyle.Thin);
                    styleGrey50.BottomBorderColor = (IndexedColors.Black.Index);
                    styleGrey50.BorderLeft = (BorderStyle.Thin);
                    styleGrey50.LeftBorderColor = (IndexedColors.Black.Index);
                    styleGrey50.BorderRight = (BorderStyle.Thin);
                    styleGrey50.RightBorderColor = (IndexedColors.Black.Index);
                    styleGrey50.BorderTop = (BorderStyle.Thin);
                    styleGrey50.TopBorderColor = (IndexedColors.Black.Index);
                    styleGrey50.FillForegroundColor = IndexedColors.Grey40Percent.Index;
                    styleGrey50.FillPattern = FillPattern.SolidForeground;
                    styleGrey50.FillBackgroundColor = HSSFColor.Grey40Percent.Index;

                    styleGrey80.WrapText = true;
                    styleGrey80.VerticalAlignment = VerticalAlignment.Center;
                    styleGrey80.BorderBottom = (BorderStyle.Thin);
                    styleGrey80.BottomBorderColor = (IndexedColors.Black.Index);
                    styleGrey80.BorderLeft = (BorderStyle.Thin);
                    styleGrey80.LeftBorderColor = (IndexedColors.Black.Index);
                    styleGrey80.BorderRight = (BorderStyle.Thin);
                    styleGrey80.RightBorderColor = (IndexedColors.Black.Index);
                    styleGrey80.BorderTop = (BorderStyle.Thin);
                    styleGrey80.TopBorderColor = (IndexedColors.Black.Index);
                    styleGrey80.FillForegroundColor = IndexedColors.Grey50Percent.Index;
                    styleGrey80.FillPattern = FillPattern.SolidForeground;
                    styleGrey80.FillBackgroundColor = HSSFColor.Grey50Percent.Index;

                    style.WrapText = true;
                    style.VerticalAlignment = VerticalAlignment.Center;
                    style.BorderBottom = (BorderStyle.Thin);
                    style.BottomBorderColor = (IndexedColors.Black.Index);
                    style.BorderLeft = (BorderStyle.Thin);
                    style.LeftBorderColor = (IndexedColors.Black.Index);
                    style.BorderRight = (BorderStyle.Thin);
                    style.RightBorderColor = (IndexedColors.Black.Index);
                    style.BorderTop = (BorderStyle.Thin);
                    style.TopBorderColor = (IndexedColors.Black.Index);
                    style.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                    style.FillPattern = FillPattern.SolidForeground;
                    style.FillBackgroundColor = HSSFColor.Grey25Percent.Index;


                    style.SetFont(fontcell);

                    styleDesignation.FillBackgroundColor = HSSFColor.BlueGrey.Index;
                    styleDesignation.SetFont(font);

                    styleHeading.WrapText = true;
                    styleHeading.VerticalAlignment = VerticalAlignment.Center;
                    styleHeading.SetFont(fontHeading);

                    styleTotal.SetFont(fontTotal);

                    #endregion

                    int DesCount = 0;

                    #region TITLE STRUCTURE IN EXCEL
                    IRow rowHeading = excelSheet.CreateRow(DesCount);
                    rowHeading.HeightInPoints = (float)(2.8 * excelSheet.DefaultRowHeightInPoints);
                    ICell CellHeading = rowHeading.CreateCell(0);
                    CellHeading.SetCellValue(wage_Process.FRM.FRM_Name.ToUpper());
                    excelSheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 28));
                    CellHeading.CellStyle = styleHeading;
                    CellUtil.SetAlignment(CellHeading, workbook, (short)HorizontalAlignment.Center);

                    IRow rowAdd1 = excelSheet.CreateRow(DesCount + 1);
                    ICell CellAdd1 = rowAdd1.CreateCell(0);
                    CellAdd1.SetCellValue(wage_Process.FRM.FRM_Address1.ToUpper() + "," + wage_Process.FRM.FRM_Address2.ToUpper() + ",");
                    CellUtil.SetAlignment(CellAdd1, workbook, (short)HorizontalAlignment.Center);
                    excelSheet.AddMergedRegion(new CellRangeAddress(DesCount + 1, DesCount + 1, 0, 28));

                    IRow rowSubHeading = excelSheet.CreateRow(DesCount + 2);
                    ICell CellSubHeading = rowSubHeading.CreateCell(0);
                    CellSubHeading.SetCellValue("PAYSHEET FOR THE MONTH OF " + WAG_Month);
                    CellSubHeading.CellStyle = styleDesignation;
                    CellUtil.SetAlignment(CellSubHeading, workbook, (short)HorizontalAlignment.Center);
                    excelSheet.AddMergedRegion(new CellRangeAddress(DesCount + 2, DesCount + 2, 0, 28));

                    IRow rowClient = excelSheet.CreateRow(DesCount + 3);
                    ICell CellClient = rowClient.CreateCell(0);
                    CellClient.SetCellValue(client.CLI.CLI_Name.ToString());
                    CellClient.CellStyle = styleDesignation;
                    CellUtil.SetAlignment(CellClient, workbook, (short)HorizontalAlignment.Center);
                    excelSheet.AddMergedRegion(new CellRangeAddress(DesCount + 3, DesCount + 3, 0, 28));
                    #endregion

                    DesCount += 4;

                    var wgs = wage_Process.Wage_Registers.Where(wg => wg.CLI_Id.Equals(client.CLI_Id));
                    var CRI_Ids = wgs.GroupBy(gg => gg.CRI_Id).Select(cri => cri.Key).ToList();

                    double Final_TotalPaybleDays = 0, Final_TotalOTHrs = 0;
                    decimal Final_TotalBasic = 0M, Final_TotalDA = 0M, Final_TotalHRA = 0M, Final_TotalOT = 0M, Final_TotalGrossTotal = 0M, Final_TotalPF = 0M, Final_TotalESIC = 0M;
                    decimal Final_TotalProfTax = 0m, Final_TotalRevenue = 0M, Final_TotalCanteenFacility = 0M, Final_TotalAdvance = 0M, Final_TotalDeduct = 0M, Final_TotalFinal = 0M;
                    decimal Final_TotalOutStation = 0M, Final_TotalAttendance = 0M, Final_TotalNightshift = 0M, Final_TotalPerformance = 0M, Final_TotalLWF = 0M;
                    bool IsCRI_OT_Calculate_Payableday = false;

                    foreach (int CRI_Id in CRI_Ids)
                    {
                        Client_Requirement cri = cliManager.GetClientRequirementsById(CRI_Id);

                        ////////////////////////////  DESIGNATION ROW ///////////////////////////////////
                        IRow row = excelSheet.CreateRow(DesCount);
                        ICell CellHeader = row.CreateCell(0);
                        CellHeader.SetCellValue(cri.DES.DES_Title);
                        CellUtil.SetAlignment(CellHeader, workbook, (short)HorizontalAlignment.Center);
                        excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount, 0, 28));
                        CellHeader.CellStyle = styleTotal;
                        DesCount = DesCount + 1;

                        ////////////////////////////  HEADER ROW ///////////////////////////////////

                        #region "Header Line"
                        row = excelSheet.CreateRow(DesCount);
                        row.HeightInPoints = (float)(3.2 * excelSheet.DefaultRowHeightInPoints);

                        ICell cell0 = row.CreateCell(0);
                        cell0.SetCellValue("Sr.No");
                        cell0.CellStyle = styleGrey25;

                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("ID");
                        cell1.CellStyle = styleGrey25;

                        ICell cell2 = row.CreateCell(2);
                        cell2.SetCellValue("ESIC No");
                        cell2.CellStyle = styleGrey25;

                        ICell cell3 = row.CreateCell(3);
                        cell3.SetCellValue("UAN NO");
                        cell3.CellStyle = styleGrey25;

                        ICell cell4 = row.CreateCell(4);
                        cell4.SetCellValue("Name");
                        excelSheet.SetColumnWidth(4, (int)((25 + 0.72) * 256));
                        cell4.CellStyle = styleGrey25;

                        ICell cell5 = row.CreateCell(5);
                        excelSheet.SetColumnWidth(5, (int)((15 + 0.72) * 256));
                        cell5.SetCellValue("DESIGNATION");
                        cell5.CellStyle = styleGrey25;

                        ICell cell6 = row.CreateCell(6);
                        cell6.SetCellValue("M/F");
                        cell6.CellStyle = styleGrey25;

                        ICell cell7 = row.CreateCell(7);
                        cell7.SetCellValue("Date Of Joining");
                        cell7.CellStyle = styleGrey25;

                        ICell cell8 = row.CreateCell(8);
                        cell8.SetCellValue("Total Working Days");
                        cell8.CellStyle = styleGrey25;

                        ICell cell9 = row.CreateCell(9);
                        cell9.SetCellValue("Total Payble Days");
                        cell9.CellStyle = styleGrey25;

                        ICell cell10 = row.CreateCell(10);
                        cell10.SetCellValue("Basic");
                        cell10.CellStyle = styleGrey40;

                        ICell cell11 = row.CreateCell(11);
                        cell11.SetCellValue("DA");
                        cell11.CellStyle = styleGrey40;

                        ICell cell12 = row.CreateCell(12);
                        cell12.SetCellValue("HRA");
                        cell12.CellStyle = styleGrey40;

                        int cell = 12;

                        ////////////////////////////  CLIENTS REQUIREMENTS ALLOWANCE ///////////////////////////////////
                        ///
                        #region CLIENTS REQUIREMENTS ALLOWANCE

                        foreach (var all in cri.Client_Requirement_Allowances.OrderBy(m => m.ALL_Id))
                        {
                            ICell cellAll = row.CreateCell(cell + 1);
                            cellAll.SetCellValue(all.ALL.ALL_Alias);
                            excelSheet.SetColumnWidth(cell + 1, (int)((25 + 0.72) * 140));
                            cellAll.CellStyle = styleGrey40;
                            cell++;
                        }

                        #endregion

                        #region CUSTOM ALLOWANCE

                        if (cri.CRI_Allowance_1)
                        {
                            cell = cell + 1;
                            ICell celOutAllow = row.CreateCell(cell);
                            celOutAllow.SetCellValue(cri.CRI_Allowance_Name_1);
                            celOutAllow.CellStyle = styleGrey40;
                            lstMasterCustomeAllowancesAlias[0] = cri.CRI_Allowance_Name_1;
                        }
                        if (cri.CRI_Allowance_2)
                        {
                            cell = cell + 1;
                            ICell celOutAllow = row.CreateCell(cell);
                            celOutAllow.SetCellValue(cri.CRI_Allowance_Name_2);
                            celOutAllow.CellStyle = styleGrey40;
                            lstMasterCustomeAllowancesAlias[1] = cri.CRI_Allowance_Name_2;
                        }
                        if (cri.CRI_Allowance_3)
                        {
                            cell = cell + 1;
                            ICell celOutAllow = row.CreateCell(cell);
                            celOutAllow.SetCellValue(cri.CRI_Allowance_Name_3);
                            celOutAllow.CellStyle = styleGrey40;
                            lstMasterCustomeAllowancesAlias[2] = cri.CRI_Allowance_Name_3;
                        }
                        if (cri.CRI_Allowance_4)
                        {
                            cell = cell + 1;
                            ICell celOutAllow = row.CreateCell(cell);
                            celOutAllow.SetCellValue(cri.CRI_Allowance_Name_4);
                            celOutAllow.CellStyle = styleGrey40;
                            lstMasterCustomeAllowancesAlias[3] = cri.CRI_Allowance_Name_4;
                        }
                        if (cri.CRI_Allowance_5)
                        {
                            cell = cell + 1;
                            ICell celOutAllow = row.CreateCell(cell);
                            celOutAllow.SetCellValue(cri.CRI_Allowance_Name_5);
                            celOutAllow.CellStyle = styleGrey40;
                            lstMasterCustomeAllowancesAlias[4] = cri.CRI_Allowance_Name_5;
                        }
                        if (cri.CRI_Allowance_6)
                        {
                            cell = cell + 1;
                            ICell celOutAllow = row.CreateCell(cell);
                            celOutAllow.SetCellValue(cri.CRI_Allowance_Name_6);
                            celOutAllow.CellStyle = styleGrey40;
                            lstMasterCustomeAllowancesAlias[5] = cri.CRI_Allowance_Name_6;
                        }
                        if (cri.CRI_Allowance_7)
                        {
                            cell = cell + 1;
                            ICell celOutAllow = row.CreateCell(cell);
                            celOutAllow.SetCellValue(cri.CRI_Allowance_Name_7);
                            celOutAllow.CellStyle = styleGrey40;
                            lstMasterCustomeAllowancesAlias[6] = cri.CRI_Allowance_Name_7;
                        }
                        if (cri.CRI_Allowance_8)
                        {
                            cell = cell + 1;
                            ICell celOutAllow = row.CreateCell(cell);
                            celOutAllow.SetCellValue(cri.CRI_Allowance_Name_8);
                            celOutAllow.CellStyle = styleGrey40;
                            lstMasterCustomeAllowancesAlias[7] = cri.CRI_Allowance_Name_8;
                        }
                        if (cri.CRI_Allowance_9)
                        {
                            cell = cell + 1;
                            ICell celOutAllow = row.CreateCell(cell);
                            celOutAllow.SetCellValue(cri.CRI_Allowance_Name_9);
                            celOutAllow.CellStyle = styleGrey40;
                            lstMasterCustomeAllowancesAlias[8] = cri.CRI_Allowance_Name_9;
                        }
                        if (cri.CRI_Allowance_10)
                        {
                            cell = cell + 1;
                            ICell celOutAllow = row.CreateCell(cell);
                            celOutAllow.SetCellValue(cri.CRI_Allowance_Name_10);
                            celOutAllow.CellStyle = styleGrey40;
                            lstMasterCustomeAllowancesAlias[9] = cri.CRI_Allowance_Name_10;
                        }

                        #endregion

                        #region STATIC ALLOWANCE

                        if (cri.CRI_OutStation_Allowance)
                        {
                            cell = cell + 1;
                            ICell celOutAllow = row.CreateCell(cell);
                            celOutAllow.SetCellValue("Outstation Allowance");
                            celOutAllow.CellStyle = styleGrey40;
                        }
                        if (cri.CRI_Attendance_Allowance)
                        {
                            cell = cell + 1;
                            ICell cellAttAllow = row.CreateCell(cell);
                            cellAttAllow.SetCellValue("Attendance Allowance");
                            cellAttAllow.CellStyle = styleGrey40;
                        }
                        if (cri.CRI_Nightshift_Allowance)
                        {
                            cell = cell + 1;
                            ICell cellNightAllow = row.CreateCell(cell);
                            cellNightAllow.SetCellValue("Night Allowance");
                            cellNightAllow.CellStyle = styleGrey40;
                        }
                        if (cri.CRI_Performance_Allowance)
                        {
                            cell = cell + 1;
                            ICell cellPerformanceAllow = row.CreateCell(cell);
                            cellPerformanceAllow.SetCellValue("Performance Allowance");
                            cellPerformanceAllow.CellStyle = styleGrey40;
                        }

                        #endregion

                        if (!cri.CRI_OT_Calculate_Payableday)
                        {
                            cell = cell + 1;
                            ICell cellotHrs = row.CreateCell(cell);
                            cellotHrs.SetCellValue("OT Hrs");
                            cellotHrs.CellStyle = styleGrey40;

                            cell = cell + 1;
                            ICell cell13 = row.CreateCell(cell);
                            cell13.SetCellValue("OT Amount");
                            cell13.CellStyle = styleGrey40;
                        }
                        int count = cell + 1;
                        ICell cell14 = row.CreateCell(cell + 1);
                        cell14.SetCellValue("Gross Total");
                        cell14.CellStyle = styleGrey40;
                        ICell cell15 = row.CreateCell(cell + 2);
                        cell15.SetCellValue("PF");
                        cell15.CellStyle = styleGrey50;
                        ICell cell16 = row.CreateCell(cell + 3);
                        cell16.SetCellValue("ESIC");
                        cell16.CellStyle = styleGrey50;

                        int cellNext = cell + 3;
                        if (cri.CRI_ProfessionalTax == true)
                        {
                            cellNext = cellNext + 1;
                            ICell cellTax = row.CreateCell(cellNext);
                            cellTax.SetCellValue("Proffesional Tax");
                            excelSheet.SetColumnWidth(cellNext, (int)((25 + 0.72) * 140));
                            cellTax.CellStyle = styleGrey50;
                        }
                        if (cri.CRI_RevenueDeduction == true)
                        {
                            cellNext = cellNext + 1;
                            ICell cellRevenue = row.CreateCell(cellNext);
                            cellRevenue.SetCellValue("Revenue Deduction");
                            excelSheet.SetColumnWidth(cellNext, (int)((25 + 0.72) * 140));
                            cellRevenue.CellStyle = styleGrey50;
                        }
                        if (cri.CRI_CanteenFacility == true)
                        {
                            cellNext = cellNext + 1;
                            ICell cellCanteen = row.CreateCell(cellNext);
                            cellCanteen.SetCellValue("Canteen Facility");
                            excelSheet.SetColumnWidth(cellNext, (int)((25 + 0.72) * 140));
                            cellCanteen.CellStyle = styleGrey50;
                        }
                        int cellNext1 = cellNext + 1;
                        ICell cell17 = row.CreateCell(cellNext1);
                        cell17.SetCellValue("Advance Installment");
                        excelSheet.SetColumnWidth(cellNext1, (int)((25 + 0.72) * 140));
                        cell17.CellStyle = styleGrey50;

                        int cellLWFNext = cellNext1 + 1;
                        if (!cri.DES.DES_Exclude_LWF)
                        {
                            ICell cell_LWF = row.CreateCell(cellLWFNext);
                            cell_LWF.SetCellValue("MLWF Deduction");
                            excelSheet.SetColumnWidth(cellLWFNext, (int)((25 + 0.72) * 140));
                            cell_LWF.CellStyle = styleGrey50;
                            cellLWFNext = cellLWFNext + 1;
                        }

                        ICell cell18 = row.CreateCell(cellLWFNext);
                        cell18.SetCellValue("Deduct Total");
                        cell18.CellStyle = styleGrey50;
                        ICell cell19 = row.CreateCell(cellLWFNext + 1);
                        cell19.SetCellValue("Final Amount");
                        cell19.CellStyle = styleGrey80;

                        ICell cell20 = row.CreateCell(cellLWFNext + 2);
                        cell20.SetCellValue("Signaure");
                        excelSheet.SetColumnWidth(cellLWFNext + 2, (int)((20 + 0.72) * 256));
                        cell20.CellStyle = styleGrey80;
                        #endregion

                        DesCount++;
                        var wage_registers = wgs.Where(wg => wg.CRI_Id.Equals(cri.CRI_Id)).ToList();
                        double TotalPaybleDays = 0, TotalOTHrs = 0;
                        decimal TotalBasic = 0M, TotalDA = 0M, TotalHRA = 0M, TotalOT = 0M, TotalGrossTotal = 0M, TotalPF = 0M, TotalESIC = 0M, TotalProfTax = 0m, TotalRevenue = 0M, TotalCanteenFacility = 0M, TotalAdvance = 0M, TotalDeduct = 0M, TotalFinal = 0M;
                        decimal TotalOutStation = 0M, TotalAttendance = 0M, TotalNightshift = 0M, TotalPerformance = 0M, TotalLWF = 0M;
                        decimal TotalAllowance1 = 0M, TotalAllowance2 = 0M, TotalAllowance3 = 0M, TotalAllowance4 = 0M, TotalAllowance5 = 0M, TotalAllowance6 = 0M, TotalAllowance7 = 0M, TotalAllowance8 = 0M, TotalAllowance9 = 0M, TotalAllowance10 = 0M;
                        int srno = 0;
                        decimal[] otherAllowances = new decimal[cri.Client_Requirement_Allowances.Count()];
                        foreach (Wage_Register wage_register in wage_registers)
                        {
                            srno = srno + 1;

                            #region EMPLOYEE PERSONAL DATA
                            row = excelSheet.CreateRow(DesCount);
                            ICell cellEmp0 = row.CreateCell(0);
                            cellEmp0.SetCellValue(srno);
                            cellEmp0.CellStyle = styleGrey25;

                            ICell cellEmp1 = row.CreateCell(1);
                            cellEmp1.SetCellValue(wage_register.EMP_Id.ToString("D5"));
                            cellEmp1.CellStyle = styleGrey25;

                            ICell cellEmp2 = row.CreateCell(2);
                            cellEmp2.SetCellValue(wage_register.EMP.EMP_ESIC_Number);
                            cellEmp2.CellStyle = styleGrey25;

                            ICell cellEmp3 = row.CreateCell(3);
                            cellEmp3.SetCellValue(wage_register.EMP.EMP_UAN_Number);
                            cellEmp3.CellStyle = styleGrey25;

                            ICell cellEmp4 = row.CreateCell(4);
                            cellEmp4.SetCellValue(wage_register.EMP.EMP_FirstName + " " + wage_register.EMP.EMP_MiddleName + " " + wage_register.EMP.EMP_SurName);
                            cellEmp4.CellStyle = styleGrey25;

                            ICell cellEmp5 = row.CreateCell(5);
                            cellEmp5.SetCellValue(wage_register.EMP.EMP_Designation);
                            cellEmp5.CellStyle = styleGrey25;

                            ICell cellEmp6 = row.CreateCell(6);
                            cellEmp6.SetCellValue(Convert.ToBoolean(wage_register.EMP.EMP_Gender) == true ? "M" : "F");
                            cellEmp6.CellStyle = styleGrey25;

                            ICell cellEmp7 = row.CreateCell(7);
                            cellEmp7.SetCellValue(wage_register.EMP.EMP_DateOfJoining.ToString("dd-MMM-yyyy"));
                            cellEmp7.CellStyle = styleGrey25;

                            ICell cellEmp8 = row.CreateCell(8);
                            cellEmp8.SetCellValue(wage_register.WAR_TotalWorkingDays);
                            cellEmp8.CellStyle = styleGrey25;
                            #endregion

                            ICell cellEmp9 = row.CreateCell(9);
                            cellEmp9.SetCellValue(wage_register.WAR_TotalPaybleDays);
                            cellEmp9.CellStyle = styleGrey25;
                            TotalPaybleDays = TotalPaybleDays + wage_register.WAR_TotalPaybleDays;

                            ICell cellEmp10 = row.CreateCell(10);
                            cellEmp10.SetCellValue(Math.Round(wage_register.WAR_Basic_Calculated, MidpointRounding.AwayFromZero).ToString());
                            cellEmp10.CellStyle = styleGrey40;
                            TotalBasic = TotalBasic + Math.Round(wage_register.WAR_Basic_Calculated, MidpointRounding.AwayFromZero);

                            ICell cellEmp11 = row.CreateCell(11);
                            cellEmp11.SetCellValue(Math.Round(wage_register.WAR_DA_Calculated, MidpointRounding.AwayFromZero).ToString());
                            cellEmp11.CellStyle = styleGrey40;
                            TotalDA = TotalDA + Math.Round(wage_register.WAR_DA_Calculated, MidpointRounding.AwayFromZero);

                            ICell cellEmp12 = row.CreateCell(12);
                            cellEmp12.SetCellValue(Math.Round(wage_register.WAR_HRA_Calculated, MidpointRounding.AwayFromZero).ToString());
                            TotalHRA = TotalHRA + Math.Round(wage_register.WAR_HRA_Calculated, MidpointRounding.AwayFromZero);
                            cellEmp12.CellStyle = styleGrey40;

                            List<Wage_Register_Allowance> alls = allManager.GetEmployeeAllowances_WAR_Id(wage_register.WAR_Id);
                            cell = 12;

                            int flg = 0;
                            foreach (var all in cri.Client_Requirement_Allowances.OrderBy(m => m.ALL_Id))
                            {
                                cell++;
                                Wage_Register_Allowance wrl = alls.Where(w => w.CRA_Id.Equals(all.CRA_Id)).FirstOrDefault();
                                AllowanceVM obj = lstMasterOtherAllowances.Where(a => a.ALL_Id.Equals(all.ALL_Id)).FirstOrDefault();
                                if (obj != null)
                                {
                                    obj.total = obj.total + wrl.WAA_Amount_Calculated;
                                    int index = lstMasterOtherAllowances.FindIndex(s => s.ALL_Id.Equals(all.ALL_Id));
                                    if (index != -1)
                                        lstMasterOtherAllowances[index] = obj;
                                }

                                otherAllowances[flg] = otherAllowances[flg] + wrl.WAA_Amount_Calculated;
                                ICell cellAll = row.CreateCell(cell);
                                cellAll.SetCellValue(wrl != null ? Math.Round(wrl.WAA_Amount_Calculated, MidpointRounding.AwayFromZero).ToString() : "0");
                                excelSheet.SetColumnWidth(cell, (int)((25 + 0.72) * 140));
                                cellAll.CellStyle = styleGrey40;
                                flg++;
                            }

                            int cellNxt = cell + 1;

                            if (cri.CRI_Allowance_1)
                            {
                                ICell celOutAllow = row.CreateCell(cellNxt);
                                decimal criAll = wage_register.WAR_Allowance_Calculated_1.HasValue ? Math.Round(wage_register.WAR_Allowance_Calculated_1.Value, MidpointRounding.AwayFromZero) : 0M;
                                celOutAllow.SetCellValue(criAll.ToString());
                                TotalAllowance1 += criAll;

                                //lstMasterCustomeAllowancesAmount[0] += TotalAllowance1;
                                celOutAllow.CellStyle = styleGrey40;
                                cellNxt++;
                            }
                            if (cri.CRI_Allowance_2)
                            {
                                ICell celOutAllow = row.CreateCell(cellNxt);
                                celOutAllow.CellStyle = styleGrey40;
                                decimal criAll = wage_register.WAR_Allowance_Calculated_2.HasValue ? Math.Round(wage_register.WAR_Allowance_Calculated_2.Value, MidpointRounding.AwayFromZero) : 0M;
                                celOutAllow.SetCellValue(criAll.ToString());
                                TotalAllowance2 += criAll;
                                //lstMasterCustomeAllowancesAmount[1] += TotalAllowance2;
                                cellNxt++;
                            }
                            if (cri.CRI_Allowance_3)
                            {
                                ICell celOutAllow = row.CreateCell(cellNxt);
                                celOutAllow.CellStyle = styleGrey40;
                                decimal criAll = wage_register.WAR_Allowance_Calculated_3.HasValue ? Math.Round(wage_register.WAR_Allowance_Calculated_3.Value, MidpointRounding.AwayFromZero) : 0M;
                                celOutAllow.SetCellValue(criAll.ToString());
                                TotalAllowance3 += criAll;
                                //lstMasterCustomeAllowancesAmount[2] += TotalAllowance3;
                                cellNxt++;
                            }
                            if (cri.CRI_Allowance_4)
                            {
                                ICell celOutAllow = row.CreateCell(cellNxt);
                                celOutAllow.CellStyle = styleGrey40;
                                decimal criAll = wage_register.WAR_Allowance_Calculated_4.HasValue ? Math.Round(wage_register.WAR_Allowance_Calculated_4.Value, MidpointRounding.AwayFromZero) : 0M;
                                celOutAllow.SetCellValue(criAll.ToString());
                                TotalAllowance4 += criAll;
                                //lstMasterCustomeAllowancesAmount[3] += TotalAllowance4;
                                cellNxt++;
                            }
                            if (cri.CRI_Allowance_5)
                            {
                                ICell celOutAllow = row.CreateCell(cellNxt);
                                celOutAllow.CellStyle = styleGrey40;
                                decimal criAll = wage_register.WAR_Allowance_Calculated_5.HasValue ? Math.Round(wage_register.WAR_Allowance_Calculated_5.Value, MidpointRounding.AwayFromZero) : 0M;
                                celOutAllow.SetCellValue(criAll.ToString());
                                TotalAllowance5 += criAll;
                                //lstMasterCustomeAllowancesAmount[4] += TotalAllowance5;
                                cellNxt++;
                            }
                            if (cri.CRI_Allowance_6)
                            {
                                ICell celOutAllow = row.CreateCell(cellNxt);
                                celOutAllow.CellStyle = styleGrey40;
                                decimal criAll = wage_register.WAR_Allowance_Calculated_6.HasValue ? Math.Round(wage_register.WAR_Allowance_Calculated_6.Value, MidpointRounding.AwayFromZero) : 0M;
                                celOutAllow.SetCellValue(criAll.ToString());
                                TotalAllowance6 += criAll;
                                //lstMasterCustomeAllowancesAmount[5] += TotalAllowance6;
                                cellNxt++;
                            }
                            if (cri.CRI_Allowance_7)
                            {
                                ICell celOutAllow = row.CreateCell(cellNxt);
                                celOutAllow.CellStyle = styleGrey40;
                                decimal criAll = wage_register.WAR_Allowance_Calculated_7.HasValue ? Math.Round(wage_register.WAR_Allowance_Calculated_7.Value, MidpointRounding.AwayFromZero) : 0M;
                                celOutAllow.SetCellValue(criAll.ToString());
                                TotalAllowance7 += criAll;
                                // lstMasterCustomeAllowancesAmount[6] += TotalAllowance7;
                                cellNxt++;
                            }
                            if (cri.CRI_Allowance_8)
                            {
                                ICell celOutAllow = row.CreateCell(cellNxt);
                                celOutAllow.CellStyle = styleGrey40;
                                decimal criAll = wage_register.WAR_Allowance_Calculated_8.HasValue ? Math.Round(wage_register.WAR_Allowance_Calculated_8.Value, MidpointRounding.AwayFromZero) : 0M;
                                celOutAllow.SetCellValue(criAll.ToString());
                                TotalAllowance8 += criAll;
                                //lstMasterCustomeAllowancesAmount[8] += TotalAllowance9;
                                cellNxt++;
                            }
                            if (cri.CRI_Allowance_9)
                            {
                                ICell celOutAllow = row.CreateCell(cellNxt);
                                celOutAllow.CellStyle = styleGrey40;
                                decimal criAll = wage_register.WAR_Allowance_Calculated_9.HasValue ? Math.Round(wage_register.WAR_Allowance_Calculated_9.Value, MidpointRounding.AwayFromZero) : 0M;
                                celOutAllow.SetCellValue(criAll.ToString());
                                TotalAllowance9 += criAll;
                                //lstMasterCustomeAllowancesAmount[8] += TotalAllowance9;
                                cellNxt++;
                            }
                            if (cri.CRI_Allowance_10)
                            {
                                ICell celOutAllow = row.CreateCell(cellNxt);
                                celOutAllow.CellStyle = styleGrey40;
                                decimal criAll = wage_register.WAR_Allowance_Calculated_10.HasValue ? Math.Round(wage_register.WAR_Allowance_Calculated_10.Value, MidpointRounding.AwayFromZero) : 0M;
                                celOutAllow.SetCellValue(criAll.ToString());
                                TotalAllowance10 += criAll;
                                //lstMasterCustomeAllowancesAmount[9] += TotalAllowance10;
                                cellNxt++;
                            }

                            #region More static Field Allowances
                            if (cri.CRI_OutStation_Allowance == true)
                            {
                                ICell cellOutstation = row.CreateCell(cellNxt);
                                decimal WAR_OutStation_Allowance_Calculated = wage_register.WAR_OutStation_Allowance_Calculated != null ? Math.Round(wage_register.WAR_OutStation_Allowance_Calculated.Value, MidpointRounding.AwayFromZero) : 0M;
                                cellOutstation.SetCellValue(WAR_OutStation_Allowance_Calculated.ToString());
                                TotalOutStation += WAR_OutStation_Allowance_Calculated;
                                cellOutstation.CellStyle = styleGrey40;
                                cellNxt++;
                            }
                            if (cri.CRI_Attendance_Allowance == true)
                            {
                                ICell cellAttendance = row.CreateCell(cellNxt);
                                decimal WAR_Attendance_Allowance_Calculated = wage_register.WAR_Attendance_Allowance_Calculated != null ? Math.Round(wage_register.WAR_Attendance_Allowance_Calculated.Value, MidpointRounding.AwayFromZero) : 0M;
                                cellAttendance.SetCellValue(WAR_Attendance_Allowance_Calculated.ToString());
                                TotalAttendance += WAR_Attendance_Allowance_Calculated;
                                cellAttendance.CellStyle = styleGrey40;
                                cellNxt++;
                            }
                            if (cri.CRI_Nightshift_Allowance == true)
                            {
                                ICell cellNightshift = row.CreateCell(cellNxt);
                                decimal WAR_Nightshift_Allowance_Calculated = wage_register.WAR_Nightshift_Allowance_Calculated != null ? Math.Round(wage_register.WAR_Nightshift_Allowance_Calculated.Value, MidpointRounding.AwayFromZero) : 0M;

                                cellNightshift.SetCellValue(WAR_Nightshift_Allowance_Calculated.ToString());
                                TotalNightshift += WAR_Nightshift_Allowance_Calculated;
                                cellNightshift.CellStyle = styleGrey40;
                                cellNxt++;
                            }
                            if (cri.CRI_Performance_Allowance == true)
                            {
                                ICell cellPerformance = row.CreateCell(cellNxt);
                                decimal WAR_Performance_Allowance_Calculated = wage_register.WAR_Performance_Allowance_Calculated != null ? Math.Round(wage_register.WAR_Performance_Allowance_Calculated.Value, MidpointRounding.AwayFromZero) : 0M;
                                cellPerformance.SetCellValue(WAR_Performance_Allowance_Calculated.ToString());
                                TotalPerformance += WAR_Performance_Allowance_Calculated;
                                cellPerformance.CellStyle = styleGrey40;
                                cellNxt++;
                            }
                            if (!cri.CRI_OT_Calculate_Payableday)
                            {
                                ICell cellEmpotHrs = row.CreateCell(cellNxt);
                                cellEmpotHrs.SetCellValue(wage_register.WAR_ExtraWorkingHours);
                                TotalOTHrs += wage_register.WAR_ExtraWorkingHours;
                                cellEmpotHrs.CellStyle = styleGrey40;
                                cellNxt++;

                                ICell cellEmp13 = row.CreateCell(cellNxt);
                                cellEmp13.SetCellValue(Math.Round(wage_register.WAR_OverTime_Calculated, MidpointRounding.AwayFromZero).ToString());
                                TotalOT += Math.Round(wage_register.WAR_OverTime_Calculated, MidpointRounding.AwayFromZero);
                                cellEmp13.CellStyle = styleGrey40;
                                cellNxt++;
                            }
                            #endregion

                            ICell cellEmp14 = row.CreateCell(cellNxt);
                            cellEmp14.SetCellValue(Math.Round(wage_register.WAR_GrossTotal, MidpointRounding.AwayFromZero).ToString());
                            TotalGrossTotal = TotalGrossTotal + Math.Round(wage_register.WAR_GrossTotal, MidpointRounding.AwayFromZero);
                            cellEmp14.CellStyle = styleGrey40;
                            ICell cellEmp15 = row.CreateCell(cellNxt + 1);
                            cellEmp15.SetCellValue(Math.Round(wage_register.WAR_PF_Calculated, MidpointRounding.AwayFromZero).ToString());
                            TotalPF = TotalPF + Math.Round(wage_register.WAR_PF_Calculated, MidpointRounding.AwayFromZero);
                            cellEmp15.CellStyle = styleGrey50;
                            ICell cellEmp16 = row.CreateCell(cellNxt + 2);
                            cellEmp16.SetCellValue(Math.Round(wage_register.WAR_ESIC_Calculated, MidpointRounding.AwayFromZero).ToString());
                            TotalESIC = TotalESIC + Math.Round(wage_register.WAR_ESIC_Calculated, MidpointRounding.AwayFromZero);
                            cellEmp16.CellStyle = styleGrey50;
                            int cellNxt1 = cellNxt + 2;
                            if (cri.CRI_ProfessionalTax)
                            {
                                cellNxt1 = cellNxt1 + 1;
                                ICell cellEmpTax = row.CreateCell(cellNxt1);
                                cellEmpTax.SetCellValue(Convert.ToString(wage_register.WAR_ProffesionalTax_Calculated));
                                TotalProfTax = TotalProfTax + Convert.ToDecimal(wage_register.WAR_ProffesionalTax_Calculated);
                                cellEmpTax.CellStyle = styleGrey50;
                            }
                            if (cri.CRI_RevenueDeduction)
                            {
                                cellNxt1 = cellNxt1 + 1;
                                ICell cellRevenue = row.CreateCell(cellNxt1);
                                cellRevenue.SetCellValue(wage_register.WAR_RevenueDeduction_Calculated);
                                if (wage_register.WAR_RevenueDeduction_Calculated != "-")
                                    TotalRevenue = TotalRevenue + Convert.ToDecimal(wage_register.WAR_RevenueDeduction_Calculated);
                                cellRevenue.CellStyle = styleGrey50;
                            }
                            if (cri.CRI_CanteenFacility)
                            {
                                cellNxt1 = cellNxt1 + 1;
                                ICell cellCanteen = row.CreateCell(cellNxt1);
                                cellCanteen.SetCellValue(wage_register.WAR_CanteenFacility_Calculation);
                                if (wage_register.WAR_CanteenFacility_Calculation != "-")
                                    TotalCanteenFacility = TotalCanteenFacility + Convert.ToDecimal(wage_register.WAR_CanteenFacility_Calculation);
                                cellCanteen.CellStyle = styleGrey50;
                            }

                            ICell cellEmp17 = row.CreateCell(cellNxt1 + 1);
                            decimal WAR_Advance_Amount = wage_register.WAR_Advance_Amount.HasValue ? Math.Round(wage_register.WAR_Advance_Amount.Value, MidpointRounding.AwayFromZero) : 0M;
                            cellEmp17.SetCellValue(WAR_Advance_Amount.ToString());
                            TotalAdvance += WAR_Advance_Amount;
                            cellEmp17.CellStyle = styleGrey50;

                            int cellNext2 = cellNxt1 + 2;
                            //TotalLWF                                    
                            if (!cri.DES.DES_Exclude_LWF)
                            {
                                ICell cell_LWF = row.CreateCell(cellNext2);
                                cell_LWF.SetCellValue(Convert.ToString(Math.Round((wage_register.WAR_LWF_Deduction_Employee != null ? wage_register.WAR_LWF_Deduction_Employee.Value : 0), MidpointRounding.AwayFromZero).ToString()));
                                excelSheet.SetColumnWidth(cellNext2, (int)((25 + 0.72) * 140));
                                cell_LWF.CellStyle = styleGrey50;
                                TotalLWF = TotalLWF + Convert.ToDecimal(wage_register.WAR_LWF_Deduction_Employee);
                                cellNext2 = cellNext2 + 1;
                            }

                            #region Total Deduction
                            decimal DeductTotal = Math.Round(wage_register.WAR_PF_Calculated, MidpointRounding.AwayFromZero) + Math.Round(wage_register.WAR_ESIC_Calculated, MidpointRounding.AwayFromZero) + Math.Round(Convert.ToDecimal(wage_register.WAR_ProffesionalTax_Calculated), MidpointRounding.AwayFromZero)
                                + Math.Round(wage_register.WAR_Advance_Amount.Value, MidpointRounding.AwayFromZero) + Math.Round((wage_register.WAR_LWF_Deduction_Employee != null ? wage_register.WAR_LWF_Deduction_Employee.Value : 0), MidpointRounding.AwayFromZero);
                            if (wage_register.WAR_RevenueDeduction_Calculated != "-")
                            {
                                DeductTotal += Math.Round(Convert.ToDecimal(wage_register.WAR_RevenueDeduction_Calculated), MidpointRounding.AwayFromZero);
                            }
                            if (wage_register.WAR_CanteenFacility_Calculation != "-")
                            {
                                DeductTotal += Math.Round(Convert.ToDecimal(wage_register.WAR_CanteenFacility_Calculation), MidpointRounding.AwayFromZero);
                            }
                            #endregion

                            ICell cellEmp18 = row.CreateCell(cellNext2);
                            cellEmp18.SetCellValue(DeductTotal.ToString());
                            cellEmp18.CellStyle = styleGrey50;
                            TotalDeduct = TotalDeduct + DeductTotal;
                            ICell cellEmp19 = row.CreateCell(cellNext2 + 1);
                            cellEmp19.SetCellValue(Math.Round(wage_register.WAR_FinalTotal.Value, MidpointRounding.AwayFromZero).ToString());
                            TotalFinal = TotalFinal + Math.Round(wage_register.WAR_FinalTotal.Value, MidpointRounding.AwayFromZero);
                            cellEmp19.CellStyle = styleGrey80;
                            ICell cellEmp20 = row.CreateCell(cellNext2 + 2);
                            cellEmp20.SetCellValue("");
                            cellEmp20.CellStyle = styleGrey80;

                            DesCount++;
                        }
                        row = excelSheet.CreateRow(DesCount);
                        excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount, 0, 8));
                        ICell cellTot = row.CreateCell(0);
                        cellTot.CellStyle = styleGrey25;
                        ICell cellTot_1 = row.CreateCell(1);
                        cellTot_1.CellStyle = styleGrey25;
                        ICell cellTot_2 = row.CreateCell(2);
                        cellTot_2.CellStyle = styleGrey25;
                        ICell cellTot_3 = row.CreateCell(3);
                        cellTot_3.CellStyle = styleGrey25;
                        ICell cellTot_4 = row.CreateCell(4);
                        cellTot_4.CellStyle = styleGrey25;
                        ICell cellTot_5 = row.CreateCell(5);
                        cellTot_5.CellStyle = styleGrey25;
                        ICell cellTot_6 = row.CreateCell(6);
                        cellTot_6.CellStyle = styleGrey25;
                        ICell cellTot_7 = row.CreateCell(7);
                        cellTot_7.CellStyle = styleGrey25;
                        ICell cellTot_8 = row.CreateCell(8);
                        cellTot_8.CellStyle = styleGrey25;
                        CellUtil.SetAlignment(cellTot, workbook, (short)HorizontalAlignment.Center);

                        int totalsCell_Index = 8;
                        ICell cellTot1 = row.CreateCell(totalsCell_Index + 1);
                        cellTot1.SetCellValue(TotalPaybleDays.ToString());
                        cellTot1.CellStyle = styleGrey25;
                        ICell cellTot2 = row.CreateCell(totalsCell_Index + 2);
                        cellTot2.SetCellValue(Math.Round(TotalBasic, MidpointRounding.AwayFromZero).ToString());
                        cellTot2.CellStyle = styleGrey40;
                        ICell cellTot3 = row.CreateCell(totalsCell_Index + 3);
                        cellTot3.SetCellValue(Math.Round(TotalDA, MidpointRounding.AwayFromZero).ToString());
                        cellTot3.CellStyle = styleGrey40;
                        ICell cellTot4 = row.CreateCell(totalsCell_Index + 4);
                        cellTot4.SetCellValue(Math.Round(TotalHRA, MidpointRounding.AwayFromZero).ToString());
                        cellTot4.CellStyle = styleGrey40;

                        int cellAllow = totalsCell_Index + 4;

                        foreach (decimal d in otherAllowances)
                        {
                            //WageAllowancesTotalVM wageTotalVM = new WageAllowancesTotalVM();
                            //wageTotalVM.ALL_Id = all.allowanceVM.ALL_Id;
                            //wageTotalVM.Parameter = all.allowanceVM.ALL_Alias;
                            //wageTotalVM.Value = arrAllowancesTotal[k];
                            //WageAllowancesTotalVMs.Add(wageTotalVM);
                            ICell cellEmpAll = row.CreateCell(cellAllow + 1);
                            cellEmpAll.SetCellValue(Math.Round(d, MidpointRounding.AwayFromZero).ToString());
                            cellEmpAll.CellStyle = styleGrey40;
                            cellAllow++;
                            //k++;
                        }

                        totalsCell_Index = cellAllow;

                        #region CUSTOM Allowance

                        if (cri.CRI_Allowance_1)
                        {
                            //IsCRI_Allowance_1 = true;
                            totalsCell_Index++;
                            ICell cellTotAllowance_1 = row.CreateCell(totalsCell_Index);
                            cellTotAllowance_1.CellStyle = styleGrey40;
                            cellTotAllowance_1.SetCellValue(Math.Round(TotalAllowance1, MidpointRounding.AwayFromZero).ToString());
                            lstMasterCustomeAllowancesAmount[0] += TotalAllowance1;
                        }
                        if (cri.CRI_Allowance_2)
                        {
                            //IsCRI_Allowance_1 = true;
                            totalsCell_Index++;
                            ICell cellTotAllowance_2 = row.CreateCell(totalsCell_Index);
                            cellTotAllowance_2.CellStyle = styleGrey40;
                            cellTotAllowance_2.SetCellValue(Math.Round(TotalAllowance2, MidpointRounding.AwayFromZero).ToString());
                            lstMasterCustomeAllowancesAmount[1] += TotalAllowance2;
                        }
                        if (cri.CRI_Allowance_3)
                        {
                            //IsCRI_Allowance_1 = true;
                            totalsCell_Index++;
                            ICell cellTotAllowance_3 = row.CreateCell(totalsCell_Index);
                            cellTotAllowance_3.CellStyle = styleGrey40;
                            cellTotAllowance_3.SetCellValue(Math.Round(TotalAllowance3, MidpointRounding.AwayFromZero).ToString());
                            lstMasterCustomeAllowancesAmount[2] += TotalAllowance3;
                        }
                        if (cri.CRI_Allowance_4)
                        {
                            //IsCRI_Allowance_1 = true;
                            totalsCell_Index++;
                            ICell cellTotAllowance_4 = row.CreateCell(totalsCell_Index);
                            cellTotAllowance_4.CellStyle = styleGrey40;
                            cellTotAllowance_4.SetCellValue(Math.Round(TotalAllowance4, MidpointRounding.AwayFromZero).ToString());
                            lstMasterCustomeAllowancesAmount[3] += TotalAllowance4;
                        }
                        if (cri.CRI_Allowance_5)
                        {
                            //IsCRI_Allowance_1 = true;
                            totalsCell_Index++;
                            ICell cellTotAllowance_5 = row.CreateCell(totalsCell_Index);
                            cellTotAllowance_5.CellStyle = styleGrey40;
                            cellTotAllowance_5.SetCellValue(Math.Round(TotalAllowance5, MidpointRounding.AwayFromZero).ToString());
                            lstMasterCustomeAllowancesAmount[4] += TotalAllowance5;
                        }
                        if (cri.CRI_Allowance_6)
                        {
                            //IsCRI_Allowance_1 = true;
                            totalsCell_Index++;
                            ICell cellTotAllowance_6 = row.CreateCell(totalsCell_Index);
                            cellTotAllowance_6.CellStyle = styleGrey40;
                            cellTotAllowance_6.SetCellValue(Math.Round(TotalAllowance6, MidpointRounding.AwayFromZero).ToString());
                            lstMasterCustomeAllowancesAmount[5] += TotalAllowance6;
                        }
                        if (cri.CRI_Allowance_7)
                        {
                            //IsCRI_Allowance_1 = true;
                            totalsCell_Index++;
                            ICell cellTotAllowance_7 = row.CreateCell(totalsCell_Index);
                            cellTotAllowance_7.CellStyle = styleGrey40;
                            cellTotAllowance_7.SetCellValue(Math.Round(TotalAllowance7, MidpointRounding.AwayFromZero).ToString());
                            lstMasterCustomeAllowancesAmount[6] += TotalAllowance7;
                        }
                        if (cri.CRI_Allowance_8)
                        {
                            //IsCRI_Allowance_1 = true;
                            totalsCell_Index++;
                            ICell cellTotAllowance_8 = row.CreateCell(totalsCell_Index);
                            cellTotAllowance_8.CellStyle = styleGrey40;
                            cellTotAllowance_8.SetCellValue(Math.Round(TotalAllowance8, MidpointRounding.AwayFromZero).ToString());
                            lstMasterCustomeAllowancesAmount[7] += TotalAllowance8;
                        }
                        if (cri.CRI_Allowance_9)
                        {
                            //IsCRI_Allowance_1 = true;
                            totalsCell_Index++;
                            ICell cellTotAllowance_9 = row.CreateCell(totalsCell_Index);
                            cellTotAllowance_9.CellStyle = styleGrey40;
                            cellTotAllowance_9.SetCellValue(Math.Round(TotalAllowance9, MidpointRounding.AwayFromZero).ToString());
                            lstMasterCustomeAllowancesAmount[8] += TotalAllowance9;
                        }
                        if (cri.CRI_Allowance_10)
                        {
                            //IsCRI_Allowance_1 = true;
                            totalsCell_Index++;
                            ICell cellTotAllowance_10 = row.CreateCell(totalsCell_Index);
                            cellTotAllowance_10.CellStyle = styleGrey40;
                            cellTotAllowance_10.SetCellValue(Math.Round(TotalAllowance10, MidpointRounding.AwayFromZero).ToString());
                            lstMasterCustomeAllowancesAmount[9] += TotalAllowance10;
                        }

                        #endregion

                        #region STATIC ALLOWANCE

                        if (cri.CRI_OutStation_Allowance == true)
                        {
                            //IsCRI_OutStation_Allowance = true;
                            totalsCell_Index = totalsCell_Index + 1;
                            ICell cellTotOutstation = row.CreateCell(totalsCell_Index);
                            cellTotOutstation.CellStyle = styleGrey40;
                            cellTotOutstation.SetCellValue(Math.Round(TotalOutStation, MidpointRounding.AwayFromZero).ToString());
                        }
                        if (cri.CRI_Attendance_Allowance == true)
                        {
                            //IsCRI_Attendance_Allowance = true;
                            totalsCell_Index = totalsCell_Index + 1;
                            ICell cellTotAttendance = row.CreateCell(totalsCell_Index);
                            cellTotAttendance.CellStyle = styleGrey40;
                            cellTotAttendance.SetCellValue(Math.Round(TotalAttendance, MidpointRounding.AwayFromZero).ToString());
                        }
                        if (cri.CRI_Nightshift_Allowance == true)
                        {
                            //IsCRI_Nightshift_Allowance = true;
                            totalsCell_Index = totalsCell_Index + 1;
                            ICell cellTotNightshift = row.CreateCell(totalsCell_Index);
                            cellTotNightshift.CellStyle = styleGrey40;
                            cellTotNightshift.SetCellValue(Math.Round(TotalNightshift, MidpointRounding.AwayFromZero).ToString());
                        }
                        if (cri.CRI_Performance_Allowance == true)
                        {
                            //IsCRI_Performance_Allowance = true;
                            totalsCell_Index = totalsCell_Index + 1;
                            ICell cellTotPerformance = row.CreateCell(totalsCell_Index);
                            cellTotPerformance.CellStyle = styleGrey40;
                            cellTotPerformance.SetCellValue(Math.Round(TotalPerformance, MidpointRounding.AwayFromZero).ToString());
                        }

                        #endregion

                        if (!cri.CRI_OT_Calculate_Payableday)
                        {
                            totalsCell_Index = totalsCell_Index + 1;
                            ICell cellTotOtHrs = row.CreateCell(totalsCell_Index);
                            cellTotOtHrs.CellStyle = styleGrey40;
                            cellTotOtHrs.SetCellValue(TotalOTHrs);

                            IsCRI_OT_Calculate_Payableday = true;
                            totalsCell_Index = totalsCell_Index + 1;
                            ICell cellTot5 = row.CreateCell(totalsCell_Index);
                            cellTot5.CellStyle = styleGrey40;
                            cellTot5.SetCellValue(Math.Round(TotalOT, MidpointRounding.AwayFromZero).ToString());
                        }

                        ICell cellTot6 = row.CreateCell(totalsCell_Index + 1);
                        cellTot6.SetCellValue(Math.Round(TotalGrossTotal, MidpointRounding.AwayFromZero).ToString());
                        cellTot6.CellStyle = styleGrey40;
                        ICell cellTot7 = row.CreateCell(totalsCell_Index + 2);
                        cellTot7.SetCellValue(Math.Round(TotalPF, MidpointRounding.AwayFromZero).ToString());
                        cellTot7.CellStyle = styleGrey50;
                        ICell cellTot8 = row.CreateCell(totalsCell_Index + 3);
                        cellTot8.SetCellValue(Math.Round(TotalESIC, MidpointRounding.AwayFromZero).ToString());
                        cellTot8.CellStyle = styleGrey50;

                        totalsCell_Index += 3;
                        if (cri.CRI_ProfessionalTax)
                        {
                            //IsCRI_ProfessionalTax = true;
                            totalsCell_Index++;
                            ICell cellTot9 = row.CreateCell(totalsCell_Index);
                            cellTot9.SetCellValue(Math.Round(TotalProfTax, MidpointRounding.AwayFromZero).ToString());
                            cellTot9.CellStyle = styleGrey50;
                        }
                        if (cri.CRI_RevenueDeduction)
                        {
                            //IsCRI_RevenueDeduction = true;
                            totalsCell_Index++;
                            ICell cellTot10 = row.CreateCell(totalsCell_Index);
                            cellTot10.SetCellValue(Math.Round(TotalRevenue, MidpointRounding.AwayFromZero).ToString());
                            cellTot10.CellStyle = styleGrey50;
                        }
                        if (cri.CRI_CanteenFacility)
                        {
                            //IsCRI_CanteenFacility = true;
                            totalsCell_Index++;
                            ICell cellTot11 = row.CreateCell(totalsCell_Index);
                            cellTot11.SetCellValue(Math.Round(TotalCanteenFacility, MidpointRounding.AwayFromZero).ToString());
                            cellTot11.CellStyle = styleGrey50;
                        }

                        ICell cellTot12 = row.CreateCell(totalsCell_Index + 1);
                        cellTot12.SetCellValue(Math.Round(TotalAdvance, MidpointRounding.AwayFromZero).ToString());
                        cellTot12.CellStyle = styleGrey50;

                        totalsCell_Index += 2;
                        if (!cri.DES.DES_Exclude_LWF)
                        {
                            //IsDES_Include_LWF = true;
                            ICell cellTotLWF = row.CreateCell(totalsCell_Index);
                            cellTotLWF.SetCellValue(Math.Round(TotalLWF, MidpointRounding.AwayFromZero).ToString());
                            cellTotLWF.CellStyle = styleGrey50;
                            totalsCell_Index++;
                        }

                        ICell cellTot13 = row.CreateCell(totalsCell_Index);
                        cellTot13.SetCellValue(Math.Round(TotalDeduct, MidpointRounding.AwayFromZero).ToString());
                        cellTot13.CellStyle = styleGrey50;
                        ICell cellTot14 = row.CreateCell(totalsCell_Index + 1);
                        cellTot14.SetCellValue(Math.Round(TotalFinal, MidpointRounding.AwayFromZero).ToString());
                        cellTot14.CellStyle = styleGrey80;

                        ICell cellTot15 = row.CreateCell(totalsCell_Index + 2);
                        cellTot15.SetCellValue("");
                        cellTot15.CellStyle = styleGrey80;

                        Final_TotalPaybleDays += TotalPaybleDays;
                        Final_TotalBasic += TotalBasic;
                        Final_TotalDA += TotalDA;
                        Final_TotalHRA += TotalHRA;
                        Final_TotalOTHrs += TotalOTHrs;
                        Final_TotalOT += TotalOT;
                        Final_TotalGrossTotal += TotalGrossTotal;
                        Final_TotalPF += TotalPF;
                        Final_TotalESIC += TotalESIC;
                        Final_TotalProfTax += TotalProfTax;
                        Final_TotalRevenue += TotalRevenue;
                        Final_TotalCanteenFacility += TotalCanteenFacility;
                        Final_TotalAdvance += TotalAdvance;
                        Final_TotalDeduct += TotalDeduct;
                        Final_TotalFinal += TotalFinal;
                        Final_TotalOutStation += TotalOutStation;
                        Final_TotalAttendance += TotalAttendance;
                        Final_TotalNightshift += TotalNightshift;
                        Final_TotalPerformance += TotalPerformance;
                        Final_TotalLWF += TotalLWF;

                        DesCount++;
                    }
                    DesCount++;

                    IRow frow = excelSheet.CreateRow(DesCount);
                    IRow frowSecond = excelSheet.CreateRow(DesCount + 1);
                    excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount + 1, 0, 8));

                    ICell fcellTot = frow.CreateCell(0);
                    fcellTot.SetCellValue("GRAND TOTAL");
                    fcellTot.CellStyle = styleGrey25;
                    ICell fcellTot_1 = frow.CreateCell(1);
                    fcellTot_1.CellStyle = styleGrey25;
                    ICell fcellTot_2 = frow.CreateCell(2);
                    fcellTot_2.CellStyle = styleGrey25;
                    ICell fcellTot_3 = frow.CreateCell(3);
                    fcellTot_3.CellStyle = styleGrey25;
                    ICell fcellTot_4 = frow.CreateCell(4);
                    fcellTot_4.CellStyle = styleGrey25;
                    ICell fcellTot_5 = frow.CreateCell(5);
                    fcellTot_5.CellStyle = styleGrey25;
                    ICell fcellTot_6 = frow.CreateCell(6);
                    fcellTot_6.CellStyle = styleGrey25;
                    ICell fcellTot_7 = frow.CreateCell(7);
                    fcellTot_7.CellStyle = styleGrey25;
                    ICell fcellTot_8 = frow.CreateCell(8);
                    fcellTot_8.CellStyle = styleGrey25;
                    CellUtil.SetAlignment(fcellTot, workbook, (short)HorizontalAlignment.Center);
                    ICell fcellTots = frowSecond.CreateCell(0);
                    fcellTots.CellStyle = styleGrey25;
                    ICell fcellTot_1s = frowSecond.CreateCell(1);
                    fcellTot_1s.CellStyle = styleGrey25;
                    ICell fcellTot_2s = frowSecond.CreateCell(2);
                    fcellTot_2s.CellStyle = styleGrey25;
                    ICell fcellTot_3s = frowSecond.CreateCell(3);
                    fcellTot_3s.CellStyle = styleGrey25;
                    ICell fcellTot_4s = frowSecond.CreateCell(4);
                    fcellTot_4s.CellStyle = styleGrey25;
                    ICell fcellTot_5s = frowSecond.CreateCell(5);
                    fcellTot_5s.CellStyle = styleGrey25;
                    ICell fcellTot_6s = frowSecond.CreateCell(6);
                    fcellTot_6s.CellStyle = styleGrey25;
                    ICell fcellTot_7s = frowSecond.CreateCell(7);
                    fcellTot_7s.CellStyle = styleGrey25;
                    ICell fcellTot_8s = frowSecond.CreateCell(8);
                    fcellTot_8s.CellStyle = styleGrey25;

                    int ftotalCount = 8;
                    ICell fcellTot1 = frow.CreateCell(ftotalCount + 1);
                    fcellTot1.SetCellValue("Total Payable Days");
                    fcellTot1.CellStyle = styleGrey25;
                    ICell fcellTot1s = frowSecond.CreateCell(ftotalCount + 1);
                    fcellTot1s.SetCellValue(Final_TotalPaybleDays.ToString());
                    fcellTot1s.CellStyle = styleGrey25;

                    ICell fcellTot2 = frow.CreateCell(ftotalCount + 2);
                    fcellTot2.SetCellValue("Total Basic");
                    fcellTot2.CellStyle = styleGrey40;
                    ICell fcellTot2s = frowSecond.CreateCell(ftotalCount + 2);
                    fcellTot2s.SetCellValue(Math.Round(Final_TotalBasic, MidpointRounding.AwayFromZero).ToString());
                    fcellTot2s.CellStyle = styleGrey40;

                    ICell fcellTot3 = frow.CreateCell(ftotalCount + 3);
                    fcellTot3.SetCellValue("Total DA");
                    fcellTot3.CellStyle = styleGrey40;
                    ICell fcellTot3s = frowSecond.CreateCell(ftotalCount + 3);
                    fcellTot3s.SetCellValue(Math.Round(Final_TotalDA, MidpointRounding.AwayFromZero).ToString());
                    fcellTot3s.CellStyle = styleGrey40;

                    ICell fcellTot4 = frow.CreateCell(ftotalCount + 4);
                    fcellTot4.SetCellValue("Total HRA");
                    fcellTot4.CellStyle = styleGrey40;
                    ICell fcellTot4s = frowSecond.CreateCell(ftotalCount + 4);
                    fcellTot4s.SetCellValue(Math.Round(Final_TotalHRA, MidpointRounding.AwayFromZero).ToString());
                    fcellTot4s.CellStyle = styleGrey40;

                    ftotalCount = ftotalCount + 5;

                    foreach (var t in lstMasterOtherAllowances.Where(dd => dd.total > 0))
                    {
                        ICell fcellTotOA = frow.CreateCell(ftotalCount);
                        fcellTotOA.SetCellValue(t.ALL_Alias);
                        fcellTotOA.CellStyle = styleGrey40;
                        ICell fcellTotOAs = frowSecond.CreateCell(ftotalCount);
                        fcellTotOAs.SetCellValue(Math.Round(t.total, MidpointRounding.AwayFromZero).ToString());
                        fcellTotOAs.CellStyle = styleGrey40;
                        ftotalCount = ftotalCount + 1;
                    }

                    for (int i = 0; i < 10; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(lstMasterCustomeAllowancesAlias[i]))
                        {
                            ICell fcellTotCA = frow.CreateCell(ftotalCount);
                            fcellTotCA.SetCellValue(lstMasterCustomeAllowancesAlias[i]);
                            fcellTotCA.CellStyle = styleGrey40;
                            ICell fcellTotOAs = frowSecond.CreateCell(ftotalCount);
                            fcellTotOAs.SetCellValue(lstMasterCustomeAllowancesAmount[i] > 0 ? Math.Round(lstMasterCustomeAllowancesAmount[i], MidpointRounding.AwayFromZero).ToString() : "0");
                            fcellTotOAs.CellStyle = styleGrey40;
                            ftotalCount = ftotalCount + 1;
                        }
                    }

                    #region STATIC ALLOWANCES

                    if (Final_TotalOutStation > 0)
                    {
                        ICell fcellTotCA = frow.CreateCell(ftotalCount);
                        fcellTotCA.SetCellValue("Outstation Allowance");
                        fcellTotCA.CellStyle = styleGrey40;
                        ICell fcellTotOAs = frowSecond.CreateCell(ftotalCount);
                        fcellTotOAs.SetCellValue(Math.Round(Final_TotalOutStation, MidpointRounding.AwayFromZero).ToString());
                        fcellTotOAs.CellStyle = styleGrey40;
                        ftotalCount = ftotalCount + 1;
                    }
                    if (Final_TotalAttendance > 0)
                    {
                        ICell fcellTotCA = frow.CreateCell(ftotalCount);
                        fcellTotCA.SetCellValue("Attendance Allowance");
                        fcellTotCA.CellStyle = styleGrey40;
                        ICell fcellTotOAs = frowSecond.CreateCell(ftotalCount);
                        fcellTotOAs.SetCellValue(Math.Round(Final_TotalAttendance, MidpointRounding.AwayFromZero).ToString());
                        fcellTotOAs.CellStyle = styleGrey40;
                        ftotalCount = ftotalCount + 1;
                    }
                    if (Final_TotalNightshift > 0)
                    {
                        ICell fcellTotCA = frow.CreateCell(ftotalCount);
                        fcellTotCA.SetCellValue("Night Allowance");
                        fcellTotCA.CellStyle = styleGrey40;
                        ICell fcellTotOAs = frowSecond.CreateCell(ftotalCount);
                        fcellTotOAs.SetCellValue(Math.Round(Final_TotalNightshift, MidpointRounding.AwayFromZero).ToString());
                        fcellTotOAs.CellStyle = styleGrey40;
                        ftotalCount = ftotalCount + 1;
                    }
                    if (Final_TotalPerformance > 0)
                    {
                        ICell fcellTotCA = frow.CreateCell(ftotalCount);
                        fcellTotCA.SetCellValue("Performance Allowance");
                        fcellTotCA.CellStyle = styleGrey40;
                        ICell fcellTotOAs = frowSecond.CreateCell(ftotalCount);
                        fcellTotOAs.SetCellValue(Math.Round(Final_TotalPerformance, MidpointRounding.AwayFromZero).ToString());
                        fcellTotOAs.CellStyle = styleGrey40;
                        ftotalCount = ftotalCount + 1;
                    }

                    if (Final_TotalOTHrs > 0)
                    {
                        ICell cellTotOtHr = frow.CreateCell(ftotalCount);
                        cellTotOtHr.CellStyle = styleGrey40;
                        cellTotOtHr.SetCellValue("Total OT Hrs");
                        ICell cellTotOtHrs = frowSecond.CreateCell(ftotalCount);
                        cellTotOtHrs.CellStyle = styleGrey40;
                        cellTotOtHrs.SetCellValue(Final_TotalOTHrs);
                        ftotalCount = ftotalCount + 1;

                        ICell cellTot5 = frow.CreateCell(ftotalCount);
                        cellTot5.CellStyle = styleGrey40;
                        cellTot5.SetCellValue("Total OT");
                        ICell cellTot5s = frowSecond.CreateCell(ftotalCount);
                        cellTot5s.CellStyle = styleGrey40;
                        cellTot5s.SetCellValue(Math.Round(Final_TotalOT, MidpointRounding.AwayFromZero).ToString());
                        ftotalCount = ftotalCount + 1;

                    }

                    #endregion

                    ICell fcellTotGross = frow.CreateCell(ftotalCount);
                    fcellTotGross.SetCellValue("Total Gross");
                    fcellTotGross.CellStyle = styleGrey40;
                    ICell fcellTotGrosss = frowSecond.CreateCell(ftotalCount);
                    fcellTotGrosss.SetCellValue(Math.Round(Final_TotalGrossTotal, MidpointRounding.AwayFromZero).ToString());
                    fcellTotGrosss.CellStyle = styleGrey40;
                    ftotalCount = ftotalCount + 1;

                    ICell fcellTotPF = frow.CreateCell(ftotalCount);
                    fcellTotPF.SetCellValue("Total PF");
                    fcellTotPF.CellStyle = styleGrey50;
                    ICell fcellTotPFs = frowSecond.CreateCell(ftotalCount);
                    fcellTotPFs.SetCellValue(Math.Round(Final_TotalPF, MidpointRounding.AwayFromZero).ToString());
                    fcellTotPFs.CellStyle = styleGrey50;
                    ftotalCount = ftotalCount + 1;

                    ICell fcellTotEsic = frow.CreateCell(ftotalCount);
                    fcellTotEsic.SetCellValue("Total ESIC");
                    fcellTotEsic.CellStyle = styleGrey50;
                    ICell fcellTotEsics = frowSecond.CreateCell(ftotalCount);
                    fcellTotEsics.SetCellValue(Math.Round(Final_TotalESIC, MidpointRounding.AwayFromZero).ToString());
                    fcellTotEsics.CellStyle = styleGrey50;
                    ftotalCount = ftotalCount + 1;

                    if (Final_TotalProfTax > 0)
                    {
                        ICell cellTot9 = frow.CreateCell(ftotalCount);
                        cellTot9.SetCellValue("Total Professional Tax");
                        cellTot9.CellStyle = styleGrey50;
                        ICell cellTot9s = frowSecond.CreateCell(ftotalCount);
                        cellTot9s.SetCellValue(Math.Round(Final_TotalProfTax, MidpointRounding.AwayFromZero).ToString());
                        cellTot9s.CellStyle = styleGrey50;
                        ftotalCount = ftotalCount + 1;
                    }
                    if (Final_TotalRevenue > 0)
                    {
                        ICell cellTot10 = frow.CreateCell(ftotalCount);
                        cellTot10.SetCellValue("Total Revenue Deduction");
                        cellTot10.CellStyle = styleGrey50;
                        ICell cellTot10s = frowSecond.CreateCell(ftotalCount);
                        cellTot10s.SetCellValue(Math.Round(Final_TotalRevenue, MidpointRounding.AwayFromZero).ToString());
                        cellTot10s.CellStyle = styleGrey50;
                        ftotalCount = ftotalCount + 1;
                    }
                    if (Final_TotalCanteenFacility > 0)
                    {
                        ICell cellTot11 = frow.CreateCell(ftotalCount);
                        cellTot11.SetCellValue("Total Canteen Facility");
                        cellTot11.CellStyle = styleGrey50;
                        ICell cellTot11s = frowSecond.CreateCell(ftotalCount);
                        cellTot11s.SetCellValue(Math.Round(Final_TotalCanteenFacility, MidpointRounding.AwayFromZero).ToString());
                        cellTot11s.CellStyle = styleGrey50;
                        ftotalCount = ftotalCount + 1;
                    }

                    ICell cellTotAdvance = frow.CreateCell(ftotalCount);
                    cellTotAdvance.SetCellValue("Total Advance Installment");
                    cellTotAdvance.CellStyle = styleGrey50;
                    ICell cellTotAdvances = frowSecond.CreateCell(ftotalCount);
                    cellTotAdvances.SetCellValue(Math.Round(Final_TotalAdvance, MidpointRounding.AwayFromZero).ToString());
                    cellTotAdvances.CellStyle = styleGrey50;
                    ftotalCount = ftotalCount + 1;

                    if (Final_TotalLWF > 0)
                    {
                        ICell cellTotLWF = frow.CreateCell(ftotalCount);
                        cellTotLWF.SetCellValue("Total MLWF Deduction");
                        cellTotLWF.CellStyle = styleGrey50;
                        ICell cellTotLWFs = frowSecond.CreateCell(ftotalCount);
                        cellTotLWFs.SetCellValue(Math.Round(Final_TotalLWF, MidpointRounding.AwayFromZero).ToString());
                        cellTotLWFs.CellStyle = styleGrey50;
                        ftotalCount = ftotalCount + 1;
                    }

                    ICell fcellTotDeduct = frow.CreateCell(ftotalCount);
                    fcellTotDeduct.SetCellValue("Deduction");
                    fcellTotDeduct.CellStyle = styleGrey50;
                    ICell fcellTotDeducts = frowSecond.CreateCell(ftotalCount);
                    fcellTotDeducts.SetCellValue(Math.Round(Final_TotalDeduct, MidpointRounding.AwayFromZero).ToString());
                    fcellTotDeducts.CellStyle = styleGrey50;
                    ftotalCount = ftotalCount + 1;

                    ICell fcellTotFinal = frow.CreateCell(ftotalCount);
                    fcellTotFinal.SetCellValue("Grand Total");
                    fcellTotFinal.CellStyle = styleGrey80;
                    ICell fcellTotFinals = frowSecond.CreateCell(ftotalCount);
                    fcellTotFinals.SetCellValue(Math.Round(Final_TotalFinal, MidpointRounding.AwayFromZero).ToString());
                    fcellTotFinals.CellStyle = styleGrey80;
                    ftotalCount = ftotalCount + 1;
                }
                workbook.Write(fs);
            }


            using (var stream = new FileStream(Path.Combine(newPath, fileName), FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            new FileInfo(Path.Combine(newPath, fileName)).Delete();
            return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);

        }

        public ActionResult EditWageRegister(int WAR_Id = -1, int FRM_Id = -1)
        {
            //EditWageRegisterVM editWageRegisterVM = new EditWageRegisterVM();
            //WageRegisterManager wageRegisterManager = new WageRegisterManager(_context);
            //editWageRegisterVM.wageRegisterVM = WageRegisterMapper.mapMe(wageRegisterManager.GetWage_RegisterByID(WAR_Id));
            //int WAG_Id = editWageRegisterVM.wageRegisterVM.WAG_Id;

            //WageProcessManager wageProcessManager = new WageProcessManager(_context);
            //DateOnly WAG_Month = wageProcessManager.getWageProcessById(WAG_Id).WAG_Month;
            //ViewBag.Wag_Month = WAG_Month.ToString("MMMM") + "-" + WAG_Month.ToString("yyyy");

            //editWageRegisterVM.wage_Register_Allowances = wageRegisterManager.GetWage_Register_Allowances(WAR_Id);
            //editWageRegisterVM.FRM_Id = FRM_Id;
            //return View(editWageRegisterVM);

            EditWageRegisterVM editWageRegisterVM = new();
            WageRegisterManager wageRegisterManager = new(_context);
            editWageRegisterVM.wageRegisterVM = WageRegisterMapper.mapMe(wageRegisterManager.GetWage_RegisterByID(WAR_Id));
            int WAG_Id = editWageRegisterVM.wageRegisterVM.WAG_Id;

            WageProcessManager wageProcessManager = new WageProcessManager(_context);
            DateOnly WAG_Month = wageProcessManager.GetWageProcessByWAG_Id(WAG_Id).WAG_Month;
            ViewBag.Wag_Month = WAG_Month.ToString("MMMM") + "-" + WAG_Month.ToString("yyyy");

            editWageRegisterVM.wage_Register_Allowances = wageRegisterManager.GetWage_Register_Allowances(WAR_Id);
            editWageRegisterVM.FRM_Id = FRM_Id;
            return View(editWageRegisterVM);
        }

        [HttpPost]
        public ActionResult EditWageRegister(EditWageRegisterVM editWageRegisterVM)
        {
            WageRegisterManager wageRegisterManager = new(_context);
            SessionUtils sessionUtils = new(Request, Response);
            Wage_Register wageRegister = wageRegisterManager.GetWage_RegisterByID(editWageRegisterVM.wageRegisterVM.WAR_Id);
            editWageRegisterVM.wageRegisterVM.ADM_LastModifiedBy = sessionUtils.GetLoggedAdminID();
            wageRegister = WageRegisterMapper.mapMeEdit(editWageRegisterVM.wageRegisterVM, wageRegister);

            string res = wageRegisterManager.UpdateWageRegister(wageRegister);
            if (res != string.Empty)
            {
                TempData["message"] = "Wage Register is not updated!";
            }
            //return RedirectToAction("WageRegister", new { WAG_Id = editWageRegisterVM.wageRegisterVM.WAG_Id, FRM_Id = editWageRegisterVM.FRM_Id });
            return RedirectToAction("WageProcessClientRegister", "WageRegister", new { WAG_Id = editWageRegisterVM.wageRegisterVM.WAG_Id, FRM_Id = editWageRegisterVM.FRM_Id, CLI_Id = editWageRegisterVM.wageRegisterVM.CLI_Id });
        }

        [HttpPost]
        public JsonResult Calculate_PF_ESIC([FromBody] CalculationEditVM CalculationEditVM)
        {
            WageRegisterManager registerManager = new WageRegisterManager(_context);
            CalculatedEditVM calculated = new CalculatedEditVM();
            if (CalculationEditVM != null)
            {
                Wage_Register wageRegister = registerManager.GetWageRegister(CalculationEditVM.WAR_Id);
                CalculationEditVM.CRI_PF_Formula = wageRegister.WAR_PF_Formula;
                CalculationEditVM.CRI_ESIC_Formula = wageRegister.WAR_ESIC_Formula;
                decimal PFsum = Math.Round(GetAmountBasedOnFormula_Edit(
                                CalculationEditVM.CRI_PF_Formula, CalculationEditVM.WAR_Basic_Calculated, CalculationEditVM.CRI_DA_Calculated, CalculationEditVM.CRI_HRA_Calculated,
                                CalculationEditVM.CalculatedAllowanceVM, CalculationEditVM.totalWorkingDays, CalculationEditVM.totalPaybleDays,
                                CalculationEditVM.WAR_OverTime_Calculated, CalculationEditVM.WAR_Outstation_Allowance_Calculated, CalculationEditVM.WAR_Attendance_Allowance_Calculated,
                                CalculationEditVM.WAR_Nightshift_Allowance_Calculated, CalculationEditVM.WAR_Performance_Allowance_Calculated,
                                CalculationEditVM.WAR_Allowance_Calculated_1, CalculationEditVM.WAR_Allowance_Calculated_2, CalculationEditVM.WAR_Allowance_Calculated_3, CalculationEditVM.WAR_Allowance_Calculated_4, CalculationEditVM.WAR_Allowance_Calculated_5, CalculationEditVM.WAR_Allowance_Calculated_6, CalculationEditVM.WAR_Allowance_Calculated_7, CalculationEditVM.WAR_Allowance_Calculated_9, CalculationEditVM.WAR_Allowance_Calculated_9, CalculationEditVM.WAR_Allowance_Calculated_10), MidpointRounding.AwayFromZero);

                decimal ESICsum = Math.Round(GetAmountBasedOnFormula_Edit(
                               CalculationEditVM.CRI_ESIC_Formula, CalculationEditVM.WAR_Basic_Calculated, CalculationEditVM.CRI_DA_Calculated, CalculationEditVM.CRI_HRA_Calculated,
                               CalculationEditVM.CalculatedAllowanceVM, CalculationEditVM.totalWorkingDays, CalculationEditVM.totalPaybleDays, CalculationEditVM.WAR_OverTime_Calculated,
                               CalculationEditVM.WAR_Outstation_Allowance_Calculated, CalculationEditVM.WAR_Attendance_Allowance_Calculated, CalculationEditVM.WAR_Nightshift_Allowance_Calculated,
                               CalculationEditVM.WAR_Performance_Allowance_Calculated, CalculationEditVM.WAR_Allowance_Calculated_1, CalculationEditVM.WAR_Allowance_Calculated_2, CalculationEditVM.WAR_Allowance_Calculated_3, CalculationEditVM.WAR_Allowance_Calculated_4, CalculationEditVM.WAR_Allowance_Calculated_5, CalculationEditVM.WAR_Allowance_Calculated_6, CalculationEditVM.WAR_Allowance_Calculated_7, CalculationEditVM.WAR_Allowance_Calculated_9, CalculationEditVM.WAR_Allowance_Calculated_9, CalculationEditVM.WAR_Allowance_Calculated_10), MidpointRounding.AwayFromZero);

                calculated.WAR_PF_Calculated = Math.Round(Decimal.Multiply(PFsum, CalculationEditVM.CRI_PF_Percentage) / 100, MidpointRounding.AwayFromZero);
                calculated.WAR_ESIC_Calculated = Math.Ceiling(Decimal.Multiply(ESICsum, CalculationEditVM.CRI_ESIC_Percentage) / 100);

            }

            return Json(calculated);
        }

        [HttpPost]
        public JsonResult Calculate_OT1([FromBody] CalculationEditVM CalculationEditVM)
        {
            WageRegisterManager registerManager = new WageRegisterManager(_context);
            decimal Calculated_OT = 0M;
            if (CalculationEditVM != null)
            {
                Wage_Register wageRegister = registerManager.GetWageRegister(CalculationEditVM.WAR_Id);
                double OvertimeInDay = CalculationEditVM.ExtraWorkingHours / Convert.ToDouble(wageRegister.CLI.CLI_WorkingHours_In_Day);
                if (OvertimeInDay > 0)
                {
                    if (!wageRegister.CRI.CRI_OT_Calculate_Payableday)
                    {
                        if (wageRegister.CRI.CRI_OT_Fixed_PerHour > 0)
                        {
                            Calculated_OT = Convert.ToDecimal(CalculationEditVM.ExtraWorkingHours) * wageRegister.CRI.CRI_OT_Fixed_PerHour.Value;
                        }
                        else if (wageRegister.CRI.CRI_OT_Formula != null)
                        {
                            CalculationEditVM.CRI_OT_Formula = wageRegister.WAR_OverTime_Formula;
                            decimal OTsum = Math.Round(GetAmountBasedOnFormula_Edit(
                                            CalculationEditVM.CRI_OT_Formula, CalculationEditVM.WAR_Basic_Calculated, CalculationEditVM.CRI_DA_Calculated, CalculationEditVM.CRI_HRA_Calculated,
                                            CalculationEditVM.CalculatedAllowanceVM, CalculationEditVM.totalWorkingDays, CalculationEditVM.totalPaybleDays,
                                            CalculationEditVM.WAR_OverTime_Calculated, CalculationEditVM.WAR_Outstation_Allowance_Calculated, CalculationEditVM.WAR_Attendance_Allowance_Calculated,
                                            CalculationEditVM.WAR_Nightshift_Allowance_Calculated, CalculationEditVM.WAR_Performance_Allowance_Calculated,
                                             CalculationEditVM.WAR_Allowance_Calculated_1, CalculationEditVM.WAR_Allowance_Calculated_2, CalculationEditVM.WAR_Allowance_Calculated_3, CalculationEditVM.WAR_Allowance_Calculated_4, CalculationEditVM.WAR_Allowance_Calculated_5, CalculationEditVM.WAR_Allowance_Calculated_6, CalculationEditVM.WAR_Allowance_Calculated_7, CalculationEditVM.WAR_Allowance_Calculated_9, CalculationEditVM.WAR_Allowance_Calculated_9, CalculationEditVM.WAR_Allowance_Calculated_10), MidpointRounding.AwayFromZero);

                            Calculated_OT = Math.Round(Convert.ToDecimal(((Convert.ToDouble(OTsum) / CalculationEditVM.totalPaybleDays) * OvertimeInDay) * wageRegister.CRI.CRI_OT_MultipleTimes), MidpointRounding.AwayFromZero);
                        }
                    }
                }

            }

            return Json(Calculated_OT);
        }

        public bool IsWageSaved(int EMP_id, int CLI_id, int WAG_Id)
        {
            return _context.Wage_Registers.Where(m => m.WAG_Id.Equals(WAG_Id) && m.CLI_Id.Equals(CLI_id) && m.EMP_Id.Equals(EMP_id)).Count() > 0;

        }
    }

    public class WageAllowancesTotalVM
    {
        public int ALL_Id { get; set; }
        public string Parameter { get; set; }
        public decimal Value { get; set; }
    }

    public class CustomAllowanceVM
    {
        public int DES_Id { get; set; }
        public string C_ALL_Title { get; set; }
        public decimal C_ALL_Value { get; set; }
    }
}