using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RMERP.DAL.ManagerClasses;
using RMERP.DAL.Models;
using RMERP.DAL.Mappers;
using RMERP.Helpers;
using RMERP.DAL.ViewModel;
using SmartBreadcrumbs.Attributes;
using static RMERP.DAL.Helpers.ProjectUtils;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.Util;
using System.IO;
using NPOI.SS.UserModel;
using RMERP.DAL.Helpers;
using Microsoft.AspNetCore.Hosting;

namespace RMERP.Controllers
{
    [Authorize]
    public class WageRegisterController : Controller
    {
        private IHostingEnvironment _hostingEnvironment;
        private readonly RMERPContext _context;
        public WageRegisterController(RMERPContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        public ActionResult WageRegister(int WAG_Id, int FRM_Id)
        {
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            WageRegisterManager wageRegisterManager = new WageRegisterManager(_context);
            if (WAG_Id > 0)
            {
                List<ClientWageRegisterVM> lst = wageRegisterManager.GenerateWageRegisterTable(WAG_Id, sessionUtils.GetLoggedAdminID(), FRM_Id);
                return View(lst);
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        public ActionResult SaveWageRegister(int WAG_Id, string item_CLI_Id)
        {
            WageRegisterManager wageRegisterManager = new WageRegisterManager(_context);
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            Wage_Process wageProcess = new Wage_Process();
            if (WAG_Id > 0)
            {
                WageProcessManager wageManager = new WageProcessManager(_context);
                wageProcess = wageManager.getWageProcessById(WAG_Id);
                List<Wage_Register> wage_Registers = WageRegisterMapper.mapWageRegisters(wageRegisterManager.GetWageRegisterCalculated(wageProcess, Convert.ToInt32(item_CLI_Id), sessionUtils.GetLoggedAdminID()));
                wageRegisterManager.SaveWageRegister(wage_Registers, WAG_Id, item_CLI_Id, sessionUtils.GetLoggedAdminID());
            }
            return RedirectToAction("WageRegister", new { WAG_Id = WAG_Id, FRM_Id = wageProcess.FRM_Id });
        }

        [HttpPost]
        public ActionResult ResetWageRegister(int WAG_Id, string item_CLI_Id, int FRM_Id)
        {
            WageRegisterManager wageRegisterManager = new WageRegisterManager(_context);
            string res = wageRegisterManager.ResetWageRegister(WAG_Id, item_CLI_Id);
            if (res != string.Empty)
            {
                TempData["message"] = "Wage Register is not saved!";
            }
            return RedirectToAction("WageRegister", new { WAG_Id = WAG_Id, FRM_Id = FRM_Id });
        }

        public ActionResult EditWageRegister(int WAR_Id = -1, int FRM_Id = -1)
        {
            EditWageRegisterVM editWageRegisterVM = new EditWageRegisterVM();
            WageRegisterManager wageRegisterManager = new WageRegisterManager(_context);
            editWageRegisterVM.wageRegisterVM = WageRegisterMapper.mapMe(wageRegisterManager.GetWage_RegisterByID(WAR_Id));
            int WAG_Id = editWageRegisterVM.wageRegisterVM.WAG_Id;

            WageProcessManager wageProcessManager = new WageProcessManager(_context);
            DateTime WAG_Month = wageProcessManager.getWageProcessById(WAG_Id).WAG_Month;
            ViewBag.Wag_Month = WAG_Month.ToString("MMMM") + "-" + WAG_Month.ToString("yyyy");

            editWageRegisterVM.wage_Register_Allowances = wageRegisterManager.GetWage_Register_Allowances(WAR_Id);
            editWageRegisterVM.FRM_Id = FRM_Id;
            return View(editWageRegisterVM);
        }

        [HttpPost]
        public ActionResult EditWageRegister(EditWageRegisterVM editWageRegisterVM)
        {
            WageRegisterManager wageRegisterManager = new WageRegisterManager(_context);

            Wage_Register wageRegister = wageRegisterManager.GetWage_RegisterByID(editWageRegisterVM.wageRegisterVM.WAR_Id);

            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            editWageRegisterVM.wageRegisterVM.ADM_LastModifiedBy = sessionUtils.GetLoggedAdminID();
            string res = wageRegisterManager.UpdateWageRegister(WageRegisterMapper.mapMe(editWageRegisterVM.wageRegisterVM));
            if (res != string.Empty)
            {
                TempData["message"] = "Wage Register is not updated!";
            }
            return RedirectToAction("WageRegister", new { WAG_Id = editWageRegisterVM.wageRegisterVM.WAG_Id, FRM_Id = editWageRegisterVM.FRM_Id });
        }

        public async Task<FileResult> WageRegisterExcel(int WAG_Id, int FRM_Id)
        {
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            WageRegisterManager wageRegisterManager = new WageRegisterManager(_context);
            List<ClientWageRegisterVM> List = new List<ClientWageRegisterVM>();
            if (WAG_Id > 0)
            {
                List = wageRegisterManager.GenerateWageRegisterTable(WAG_Id, sessionUtils.GetLoggedAdminID(), FRM_Id);
            }

            ReportsManager manager = new ReportsManager(_context);
            WageProcessManager wageProcess = new WageProcessManager(_context);
            DateTime wageMonth = wageProcess.getWageProcessById(WAG_Id).WAG_Month;
            string WAG_Month = wageMonth.ToString("MMMM").ToUpper() + "-" + wageMonth.ToString("yyyy");

            string newPath = ProjectUtils.GetTempFolderPath(_hostingEnvironment.WebRootPath);
            string fileName = "Wage_Register_" + DateTime.Now.ToString("ddMMyyyyHHmm") + "_" + WAG_Month + ".xlsx";
            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, fileName);
            FileInfo file = new FileInfo(Path.Combine(newPath, fileName));
            var memory = new MemoryStream();
            using (var fs = new FileStream(Path.Combine(newPath, fileName), FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook;
                workbook = new XSSFWorkbook();
                ISheet excelSheet = null;
                foreach (var client in List.Select(m => new { m.client, m.wageProcessClientVM, m.wageProcessVM.Attendance }).Distinct())
                {
                    if (client.Attendance.Where(m => m.CLI_Id.Equals(client.client.CLI_Id)).Count() > 0)
                    {
                        excelSheet = workbook.CreateSheet(client.client.CLI_Name.ToString());
                        foreach (var item in List.Where(m => m.client.CLI_Id.Equals(client.client.CLI_Id)))
                        {
                            var distinceDesignations = item.wageRegisterVMs.Select(q => new { q.designation.DES_Id, q.designation.DES_Title,q.designation.DES_Exclude_LWF }).Distinct();

                            #region style of excel
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

                            #region Title structure in excel
                            IRow rowHeading = excelSheet.CreateRow(DesCount);
                            rowHeading.HeightInPoints = (float)(2.8 * excelSheet.DefaultRowHeightInPoints);
                            ICell CellHeading = rowHeading.CreateCell(0);
                            CellHeading.SetCellValue("RELIABLE SECURITY SERVICES");
                            excelSheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 25));
                            CellHeading.CellStyle = styleHeading;
                            CellUtil.SetAlignment(CellHeading, workbook, (short)HorizontalAlignment.Center);

                            IRow rowAdd1 = excelSheet.CreateRow(DesCount + 1);
                            ICell CellAdd1 = rowAdd1.CreateCell(0);
                            CellAdd1.SetCellValue("G-9, MALTI TOWER, TARABAI PARK, KOLHAPUR");
                            CellUtil.SetAlignment(CellAdd1, workbook, (short)HorizontalAlignment.Center);
                            excelSheet.AddMergedRegion(new CellRangeAddress(DesCount + 1, DesCount + 1, 0, 25));

                            IRow rowSubHeading = excelSheet.CreateRow(DesCount + 2);
                            ICell CellSubHeading = rowSubHeading.CreateCell(0);
                            CellSubHeading.SetCellValue("PAYSHEET FOR THE MONTH OF " + WAG_Month);
                            CellSubHeading.CellStyle = styleDesignation;
                            CellUtil.SetAlignment(CellSubHeading, workbook, (short)HorizontalAlignment.Center);
                            excelSheet.AddMergedRegion(new CellRangeAddress(DesCount + 2, DesCount + 2, 0, 25));

                            IRow rowClient = excelSheet.CreateRow(DesCount + 3);
                            ICell CellClient = rowClient.CreateCell(0);
                            CellClient.SetCellValue(client.client.CLI_Name.ToString());
                            CellClient.CellStyle = styleDesignation;
                            CellUtil.SetAlignment(CellClient, workbook, (short)HorizontalAlignment.Center);
                            excelSheet.AddMergedRegion(new CellRangeAddress(DesCount + 3, DesCount + 3, 0, 25));
                            #endregion

                            DesCount = DesCount + 4;
                            foreach (var dd in distinceDesignations)
                            {

                                int TotalAllowances = 0;

                                List<WageRegisterVM> wageRegisters = item.wageRegisterVMs.Where(r => r.designation.DES_Id.Equals(dd.DES_Id)).ToList();
                                ClientRequirementVM clientRequirement = wageRegisters.Select(m => m.clientRequirementVM).Where(m => m.CLI_Id.Equals(item.client.CLI_Id) && m.DES_Id.Equals(dd.DES_Id)).First();

                                TotalAllowances = wageRegisters[0].allowanceVMs.Count();
                                decimal[] arrAllowancesTotal = new decimal[TotalAllowances];
                                #region Headings
                                IRow row = excelSheet.CreateRow(DesCount);
                                ICell CellHeader = row.CreateCell(0);
                                CellHeader.SetCellValue(dd.DES_Title);
                                CellUtil.SetAlignment(CellHeader, workbook, (short)HorizontalAlignment.Center);
                                excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount, 0, 25));
                                CellHeader.CellStyle = styleTotal;

                                row = excelSheet.CreateRow(DesCount + 1);
                                row.HeightInPoints = (float)(3.2 * excelSheet.DefaultRowHeightInPoints);
                                ICell cell0 = row.CreateCell(0);
                                cell0.SetCellValue("ID");
                                cell0.CellStyle = styleGrey25;
                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("Name");
                                excelSheet.SetColumnWidth(1, (int)((25 + 0.72) * 256));
                                cell1.CellStyle = styleGrey25;
                                ICell cell2 = row.CreateCell(2);
                                cell2.SetCellValue("M/F");
                                cell2.CellStyle = styleGrey25;
                                ICell cell3 = row.CreateCell(3);
                                cell3.SetCellValue("Date Of Joining");
                                cell3.CellStyle = styleGrey25;
                                ICell cell4 = row.CreateCell(4);
                                cell4.SetCellValue("Total Working Days");
                                cell4.CellStyle = styleGrey25;
                                ICell cell5 = row.CreateCell(5);
                                cell5.SetCellValue("Total Payble Days");
                                cell5.CellStyle = styleGrey25;
                                ICell cell6 = row.CreateCell(6);
                                cell6.SetCellValue("Basic");
                                cell6.CellStyle = styleGrey40;
                                ICell cell7 = row.CreateCell(7);
                                cell7.SetCellValue("DA");
                                cell7.CellStyle = styleGrey40;
                                ICell cell8 = row.CreateCell(8);
                                cell8.SetCellValue("HRA");
                                cell8.CellStyle = styleGrey40;
                                int cell = 8;
                                List<string> allowance = new List<string>();
                                int i = 0;
                                foreach (var all in wageRegisters[0].allowanceVMs)
                                {
                                    arrAllowancesTotal[i] = 0;
                                    allowance.Add(all.allowanceVM.ALL_Alias);
                                    ICell cellAll = row.CreateCell(cell + 1);
                                    cellAll.SetCellValue(all.allowanceVM.ALL_Alias);
                                    excelSheet.SetColumnWidth(cell + 1, (int)((25 + 0.72) * 140));
                                    cellAll.CellStyle = styleGrey40;
                                    cell++; i++;
                                }              
                                if (clientRequirement.CRI_OutStation_Allowance == true)
                                {
                                    cell = cell + 1;
                                    ICell celOutAllow = row.CreateCell(cell);
                                    celOutAllow.SetCellValue("Outstation Allowance");
                                    celOutAllow.CellStyle = styleGrey40;                                   
                                }
                                if (clientRequirement.CRI_Attendance_Allowance == true)
                                {
                                    cell = cell + 1;
                                    ICell cellAttAllow = row.CreateCell(cell);
                                    cellAttAllow.SetCellValue("Attendance Allowance");
                                    cellAttAllow.CellStyle = styleGrey40;                                   
                                }
                                if (clientRequirement.CRI_Nightshift_Allowance == true)
                                {
                                    cell = cell + 1;
                                    ICell cellNightAllow = row.CreateCell(cell);
                                    cellNightAllow.SetCellValue("Night Allowance");
                                    cellNightAllow.CellStyle = styleGrey40;                                 
                                }
                                if (clientRequirement.CRI_Performance_Allowance == true)
                                {
                                    cell = cell + 1;
                                    ICell cellPerformanceAllow = row.CreateCell(cell);
                                    cellPerformanceAllow.SetCellValue("Performance Allowance");
                                    cellPerformanceAllow.CellStyle = styleGrey40;                                   
                                }
                                if(!clientRequirement.CRI_OT_Calculate_Payableday)
                                {
                                    cell = cell + 1;
                                    ICell cell10 = row.CreateCell(cell);
                                    cell10.SetCellValue("OT Amount");
                                    cell10.CellStyle = styleGrey40;                                  
                                }
                                int count = cell + 1;
                                ICell cell11 = row.CreateCell(cell+1);
                                cell11.SetCellValue("Gross Total");
                                cell11.CellStyle = styleGrey40;
                                ICell cell12 = row.CreateCell(cell + 2);
                                cell12.SetCellValue("PF");
                                cell12.CellStyle = styleGrey50;
                                ICell cell13 = row.CreateCell(cell + 3);
                                cell13.SetCellValue("ESIC");
                                cell13.CellStyle = styleGrey50;

                                int cellNext = cell + 3;
                                if (clientRequirement.CRI_ProfessionalTax == true)
                                {
                                    cellNext = cellNext + 1;
                                    ICell cellTax = row.CreateCell(cellNext);
                                    cellTax.SetCellValue("Proffesional Tax");
                                    excelSheet.SetColumnWidth(cellNext, (int)((25 + 0.72) * 140));
                                    cellTax.CellStyle = styleGrey50;
                                }
                                if (clientRequirement.CRI_RevenueDeduction == true)
                                {
                                    cellNext = cellNext + 1;
                                    ICell cellRevenue = row.CreateCell(cellNext);
                                    cellRevenue.SetCellValue("Revenue Deduction");
                                    excelSheet.SetColumnWidth(cellNext, (int)((25 + 0.72) * 140));
                                    cellRevenue.CellStyle = styleGrey50;
                                }
                                if (clientRequirement.CRI_CanteenFacility == true)
                                {
                                    cellNext = cellNext + 1;
                                    ICell cellCanteen = row.CreateCell(cellNext);
                                    cellCanteen.SetCellValue("Canteen Facility");
                                    excelSheet.SetColumnWidth(cellNext, (int)((25 + 0.72) * 140));
                                    cellCanteen.CellStyle = styleGrey50;
                                }
                                int cellNext1 = cellNext + 1;
                                ICell cell14 = row.CreateCell(cellNext1);
                                cell14.SetCellValue("Advance Installment");
                                excelSheet.SetColumnWidth(cellNext1, (int)((25 + 0.72) * 140));
                                cell14.CellStyle = styleGrey50;

                                int cellLWFNext = cellNext1 + 1;
                                if (!dd.DES_Exclude_LWF)
                                {                                   
                                    ICell cell_LWF = row.CreateCell(cellLWFNext);
                                    cell_LWF.SetCellValue("MLWF Deduction");
                                    excelSheet.SetColumnWidth(cellLWFNext, (int)((25 + 0.72) * 140));
                                    cell_LWF.CellStyle = styleGrey50;
                                    cellLWFNext = cellLWFNext + 1;
                                }                                

                                ICell cell15 = row.CreateCell(cellLWFNext);
                                cell15.SetCellValue("Deduct Total");
                                cell15.CellStyle = styleGrey50;
                                ICell cell16 = row.CreateCell(cellLWFNext + 1);
                                cell16.SetCellValue("Final Amount");
                                cell16.CellStyle = styleGrey80;
                                #endregion

                                DesCount = DesCount + 2;
                                double TotalPaybleDays = 0;
                                decimal TotalBasic = 0M, TotalDA = 0M, TotalHRA = 0M, TotalOT = 0M, TotalGrossTotal = 0M, TotalPF = 0M, TotalESIC = 0M, TotalProfTax = 0m, TotalRevenue = 0M, TotalCanteenFacility = 0M, TotalAdvance = 0M, TotalDeduct = 0M, TotalFinal = 0M;
                                decimal TotalOutStation = 0M, TotalAttendance = 0M, TotalNightshift = 0M, TotalPerformance = 0M, TotalLWF=0M;
                               
                                foreach (var employee in wageRegisters)
                                {
                                    #region Employee Data
                                    row = excelSheet.CreateRow(DesCount);
                                    ICell cellEmp0 = row.CreateCell(0);
                                    cellEmp0.SetCellValue(employee.employeeVM.EMP_Id.ToString("D5"));
                                    cellEmp0.CellStyle = styleGrey25;
                                    ICell cellEmp1 = row.CreateCell(1);
                                    cellEmp1.SetCellValue(employee.employeeVM.EMP_FirstName + " " + employee.employeeVM.EMP_MiddleName + " " + employee.employeeVM.EMP_SurName);
                                    cellEmp1.CellStyle = styleGrey25;
                                    ICell cellEmp2 = row.CreateCell(2);
                                    cellEmp2.SetCellValue(employee.employeeVM.EMP_Gender == true ? "M" : "F");
                                    cellEmp2.CellStyle = styleGrey25;
                                    ICell cellEmp3 = row.CreateCell(3);
                                    cellEmp3.SetCellValue(DateHelper.getDateWithFormat(employee.employeeVM.EMP_DateOfJoining));
                                    cellEmp3.CellStyle = styleGrey25;
                                    ICell cellEmp4 = row.CreateCell(4);
                                    cellEmp4.SetCellValue(employee.WAR_TotalWorkingDays);
                                    cellEmp4.CellStyle = styleGrey25;
                                    ICell cellEmp5 = row.CreateCell(5);
                                    cellEmp5.SetCellValue(employee.WAR_TotalPaybleDays);
                                    cellEmp5.CellStyle = styleGrey25;
                                    TotalPaybleDays = TotalPaybleDays + employee.WAR_TotalPaybleDays;
                                    ICell cellEmp6 = row.CreateCell(6);
                                    cellEmp6.SetCellValue(Math.Round(employee.WAR_Basic_Calculated, MidpointRounding.AwayFromZero).ToString());
                                    cellEmp6.CellStyle = styleGrey40;
                                    TotalBasic = TotalBasic + Math.Round(employee.WAR_Basic_Calculated, MidpointRounding.AwayFromZero);
                                    ICell cellEmp7 = row.CreateCell(7);
                                    cellEmp7.SetCellValue(Math.Round(employee.WAR_DA_Calculated, MidpointRounding.AwayFromZero).ToString());
                                    cellEmp7.CellStyle = styleGrey40;
                                    TotalDA = TotalDA + Math.Round(employee.WAR_DA_Calculated, MidpointRounding.AwayFromZero);
                                    ICell cellEmp8 = row.CreateCell(8);
                                    cellEmp8.SetCellValue(Math.Round(employee.WAR_HRA_Calculated, MidpointRounding.AwayFromZero).ToString());
                                    TotalHRA = TotalHRA + Math.Round(employee.WAR_HRA_Calculated, MidpointRounding.AwayFromZero);
                                    cellEmp8.CellStyle = styleGrey40;
                                    int cellAllowance = 8;
                                    int j = 0;
                                    foreach (var all in employee.allowanceVMs)
                                    {
                                        arrAllowancesTotal[j] = arrAllowancesTotal[j] + all.WAA_Amount_Calculated;                                        
                                        ICell cellEmpAll = row.CreateCell(cellAllowance + 1);
                                        cellEmpAll.SetCellValue(Math.Round(all.WAA_Amount_Calculated, MidpointRounding.AwayFromZero).ToString());
                                        cellEmpAll.CellStyle = styleGrey40;
                                        cellAllowance++;
                                        j++;
                                    }
                                    int cellNxt = cellAllowance + 1;

                                    #region New allowances 29th july
                                    if (clientRequirement.CRI_OutStation_Allowance == true)
                                    {
                                        ICell cellOutstation = row.CreateCell(cellNxt);
                                        cellOutstation.SetCellValue(Math.Round(employee.WAR_OutStation_Allowance_Calculated, MidpointRounding.AwayFromZero).ToString());
                                        TotalOutStation = TotalOutStation + Math.Round(employee.WAR_OutStation_Allowance_Calculated);
                                        cellOutstation.CellStyle = styleGrey40;
                                        cellNxt = cellNxt + 1;
                                    }
                                    if (clientRequirement.CRI_Attendance_Allowance == true)
                                    {
                                        ICell cellAttendance= row.CreateCell(cellNxt);
                                        cellAttendance.SetCellValue(Math.Round(employee.WAR_Attendance_Allowance_Calculated, MidpointRounding.AwayFromZero).ToString());
                                        TotalAttendance = TotalAttendance + Math.Round(employee.WAR_Attendance_Allowance_Calculated, MidpointRounding.AwayFromZero);
                                        cellAttendance.CellStyle = styleGrey40;
                                        cellNxt = cellNxt + 1;
                                    }
                                    if (clientRequirement.CRI_Nightshift_Allowance == true)
                                    {
                                        ICell cellNightshift = row.CreateCell(cellNxt);
                                        cellNightshift.SetCellValue(Math.Round(employee.WAR_Nightshift_Allowance_Calculated, MidpointRounding.AwayFromZero).ToString());
                                        TotalNightshift = TotalNightshift + Math.Round(employee.WAR_Nightshift_Allowance_Calculated, MidpointRounding.AwayFromZero);
                                        cellNightshift.CellStyle = styleGrey40;
                                        cellNxt = cellNxt + 1;
                                    }
                                    if (clientRequirement.CRI_Performance_Allowance == true)
                                    {
                                        ICell cellPerformance = row.CreateCell(cellNxt);
                                        cellPerformance.SetCellValue(Math.Round(employee.WAR_Performance_Allowance_Calculated, MidpointRounding.AwayFromZero).ToString());
                                        TotalPerformance = TotalPerformance + Math.Round(employee.WAR_Performance_Allowance_Calculated, MidpointRounding.AwayFromZero);
                                        cellPerformance.CellStyle = styleGrey40;
                                        cellNxt = cellNxt + 1;
                                    }
                                    if (!clientRequirement.CRI_OT_Calculate_Payableday)
                                    {
                                        ICell cellEmp10 = row.CreateCell(cellNxt);
                                        cellEmp10.SetCellValue(Math.Round(employee.WAR_OverTime_Calculated, MidpointRounding.AwayFromZero).ToString());
                                        TotalOT = TotalOT + Math.Round(employee.WAR_OverTime_Calculated, MidpointRounding.AwayFromZero);
                                        cellEmp10.CellStyle = styleGrey40;
                                        cellNxt = cellNxt + 1;
                                    }
                                    #endregion                                    
                                    ICell cellEmp11 = row.CreateCell(cellNxt);
                                    cellEmp11.SetCellValue(Math.Round(employee.WAR_GrossTotal, MidpointRounding.AwayFromZero).ToString());
                                    TotalGrossTotal = TotalGrossTotal + Math.Round(employee.WAR_GrossTotal, MidpointRounding.AwayFromZero);
                                    cellEmp11.CellStyle = styleGrey40;
                                    ICell cellEmp12 = row.CreateCell(cellNxt + 1);
                                    cellEmp12.SetCellValue(Math.Round(employee.WAR_PF_Calculated, MidpointRounding.AwayFromZero).ToString());
                                    TotalPF = TotalPF + Math.Round(employee.WAR_PF_Calculated, MidpointRounding.AwayFromZero);
                                    cellEmp12.CellStyle = styleGrey50;
                                    ICell cellEmp13 = row.CreateCell(cellNxt + 2);
                                    cellEmp13.SetCellValue(Math.Round(employee.WAR_ESIC_Calculated, MidpointRounding.AwayFromZero).ToString());
                                    TotalESIC = TotalESIC + Math.Round(employee.WAR_ESIC_Calculated, MidpointRounding.AwayFromZero);
                                    cellEmp13.CellStyle = styleGrey50;
                                    int cellNxt1 = cellNxt + 2;
                                    if (clientRequirement.CRI_ProfessionalTax == true)
                                    {                                        
                                        cellNxt1 = cellNxt1 + 1;
                                        ICell cellEmpTax = row.CreateCell(cellNxt1);
                                        cellEmpTax.SetCellValue(Convert.ToString(employee.WAR_ProffesionalTax_Calculated));
                                        TotalProfTax = TotalProfTax + Convert.ToDecimal(employee.WAR_ProffesionalTax_Calculated);
                                        cellEmpTax.CellStyle = styleGrey50;
                                    }
                                    if (clientRequirement.CRI_RevenueDeduction == true)
                                    {
                                        cellNxt1 = cellNxt1 + 1;
                                        ICell cellRevenue = row.CreateCell(cellNxt1);
                                        cellRevenue.SetCellValue(employee.WAR_RevenueDeduction_Calculated);
                                        if (employee.WAR_RevenueDeduction_Calculated != "-")
                                            TotalRevenue = TotalRevenue + Convert.ToDecimal(employee.WAR_RevenueDeduction_Calculated);
                                        cellRevenue.CellStyle = styleGrey50;
                                    }
                                    if (clientRequirement.CRI_CanteenFacility == true)
                                    {
                                        cellNxt1 = cellNxt1 + 1;
                                        ICell cellCanteen = row.CreateCell(cellNxt1);
                                        cellCanteen.SetCellValue(employee.WAR_CanteenFacility_Calculation);
                                        if (employee.WAR_CanteenFacility_Calculation != "-")
                                            TotalCanteenFacility = TotalCanteenFacility + Convert.ToDecimal(employee.WAR_CanteenFacility_Calculation);
                                        cellCanteen.CellStyle = styleGrey50;
                                    }                                   

                                    ICell cellEmp14 = row.CreateCell(cellNxt1+1);
                                    cellEmp14.SetCellValue(Math.Round(employee.WAR_Advance_Amount, MidpointRounding.AwayFromZero).ToString());
                                    TotalAdvance = TotalAdvance + Math.Round(employee.WAR_Advance_Amount, MidpointRounding.AwayFromZero);
                                    cellEmp14.CellStyle = styleGrey50;

                                    int cellNext2 = cellNxt1 + 2;
                                    //TotalLWF                                    
                                    if (!dd.DES_Exclude_LWF)
                                    {
                                        ICell cell_LWF = row.CreateCell(cellNext2);
                                        cell_LWF.SetCellValue(Convert.ToString(Math.Round(employee.WAR_LWF_Deduction_Calculated, MidpointRounding.AwayFromZero).ToString()));
                                        excelSheet.SetColumnWidth(cellNext2, (int)((25 + 0.72) * 140));
                                        cell_LWF.CellStyle = styleGrey50;
                                        TotalLWF = TotalLWF + Convert.ToDecimal(employee.WAR_LWF_Deduction_Calculated);
                                        cellNext2 = cellNext2 + 1;
                                    }

                                    #region Total Deduction
                                    decimal DeductTotal = Math.Round(employee.WAR_PF_Calculated, MidpointRounding.AwayFromZero) + Math.Round(employee.WAR_ESIC_Calculated, MidpointRounding.AwayFromZero) + Math.Round(Convert.ToDecimal(employee.WAR_ProffesionalTax_Calculated), MidpointRounding.AwayFromZero)
                                        + Math.Round(employee.WAR_Advance_Amount, MidpointRounding.AwayFromZero) + Math.Round(employee.WAR_LWF_Deduction_Calculated, MidpointRounding.AwayFromZero);
                                    if (employee.WAR_RevenueDeduction_Calculated != "-")
                                    {
                                        DeductTotal += Math.Round(Convert.ToDecimal(employee.WAR_RevenueDeduction_Calculated), MidpointRounding.AwayFromZero);
                                    }
                                    if (employee.WAR_CanteenFacility_Calculation != "-")
                                    {
                                        DeductTotal += Math.Round(Convert.ToDecimal(employee.WAR_CanteenFacility_Calculation), MidpointRounding.AwayFromZero);
                                    }
                                    #endregion
                                   
                                    ICell cellEmp15 = row.CreateCell(cellNext2);
                                    cellEmp15.SetCellValue(DeductTotal.ToString());
                                    cellEmp15.CellStyle = styleGrey50;
                                    TotalDeduct = TotalDeduct + DeductTotal;
                                    ICell cellEmp16 = row.CreateCell(cellNext2 + 1);
                                    cellEmp16.SetCellValue(Math.Round(employee.WAR_FinalTotal, MidpointRounding.AwayFromZero).ToString());
                                    TotalFinal = TotalFinal + Math.Round(employee.WAR_FinalTotal, MidpointRounding.AwayFromZero);
                                    cellEmp16.CellStyle = styleGrey80;
                                    #endregion
                                    DesCount++;
                                }

                                #region Total in Excel
                                row = excelSheet.CreateRow(DesCount);
                                excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount, 0, 4));
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
                                CellUtil.SetAlignment(cellTot, workbook, (short)HorizontalAlignment.Center);

                                int totalCount = 4;
                                ICell cellTot1 = row.CreateCell(totalCount + 1);
                                cellTot1.SetCellValue(TotalPaybleDays.ToString());
                                cellTot1.CellStyle = styleGrey25;
                                ICell cellTot2 = row.CreateCell(totalCount + 2);
                                cellTot2.SetCellValue(Math.Round(TotalBasic, MidpointRounding.AwayFromZero).ToString());
                                cellTot2.CellStyle = styleGrey40;
                                ICell cellTot3 = row.CreateCell(totalCount + 3);
                                cellTot3.SetCellValue(Math.Round(TotalDA, MidpointRounding.AwayFromZero).ToString());
                                cellTot3.CellStyle = styleGrey40;
                                ICell cellTot4 = row.CreateCell(totalCount + 4);
                                cellTot4.SetCellValue(Math.Round(TotalHRA, MidpointRounding.AwayFromZero).ToString());
                                cellTot4.CellStyle = styleGrey40;
                              
                                int k = 0;
                                int cellAllow = totalCount + 4;
                                foreach (var all in wageRegisters[0].allowanceVMs)
                                {
                                    ICell cellEmpAll = row.CreateCell(cellAllow + 1);
                                    cellEmpAll.SetCellValue(Math.Round(arrAllowancesTotal[k], MidpointRounding.AwayFromZero).ToString());
                                    cellEmpAll.CellStyle = styleGrey40;
                                    cellAllow++;
                                    k++;
                                }
                                totalCount = cellAllow;

                                #region New total allowances 29th july
                                if (clientRequirement.CRI_OutStation_Allowance == true)
                                {
                                    totalCount = totalCount + 1;
                                    ICell cellTotOutstation = row.CreateCell(totalCount);
                                    cellTotOutstation.CellStyle = styleGrey40;
                                    cellTotOutstation.SetCellValue(Math.Round(TotalOutStation, MidpointRounding.AwayFromZero).ToString());
                                }
                                if (clientRequirement.CRI_Attendance_Allowance == true)
                                {
                                    totalCount = totalCount + 1;
                                    ICell cellTotAttendance = row.CreateCell(totalCount);
                                    cellTotAttendance.CellStyle = styleGrey40;
                                    cellTotAttendance.SetCellValue(Math.Round(TotalAttendance, MidpointRounding.AwayFromZero).ToString());
                                }
                                if (clientRequirement.CRI_Nightshift_Allowance == true)
                                {
                                    totalCount = totalCount + 1;
                                    ICell cellTotNightshift = row.CreateCell(totalCount);
                                    cellTotNightshift.CellStyle = styleGrey40;
                                    cellTotNightshift.SetCellValue(Math.Round(TotalNightshift, MidpointRounding.AwayFromZero).ToString());
                                }
                                if (clientRequirement.CRI_Performance_Allowance == true)
                                {
                                    totalCount = totalCount + 1;
                                    ICell cellTotPerformance = row.CreateCell(totalCount);
                                    cellTotPerformance.CellStyle = styleGrey40;
                                    cellTotPerformance.SetCellValue(Math.Round(TotalPerformance, MidpointRounding.AwayFromZero).ToString());
                                }
                                if (!clientRequirement.CRI_OT_Calculate_Payableday)
                                {
                                    totalCount = totalCount + 1;
                                    ICell cellTot5 = row.CreateCell(totalCount);
                                    cellTot5.CellStyle = styleGrey40;
                                    cellTot5.SetCellValue(Math.Round(TotalOT, MidpointRounding.AwayFromZero).ToString());
                                }
                                #endregion
                                
                                ICell cellTot6 = row.CreateCell(totalCount + 1);
                                cellTot6.SetCellValue(Math.Round(TotalGrossTotal, MidpointRounding.AwayFromZero).ToString());
                                cellTot6.CellStyle = styleGrey40;
                                ICell cellTot7 = row.CreateCell(totalCount + 2);
                                cellTot7.SetCellValue(Math.Round(TotalPF, MidpointRounding.AwayFromZero).ToString());
                                cellTot7.CellStyle = styleGrey50;
                                ICell cellTot8 = row.CreateCell(totalCount + 3);
                                cellTot8.SetCellValue(Math.Round(TotalESIC, MidpointRounding.AwayFromZero).ToString());
                                cellTot8.CellStyle = styleGrey50;

                                int totalCount1 = totalCount + 3;
                                if (clientRequirement.CRI_ProfessionalTax)
                                {
                                    totalCount1 = totalCount1 + 1;
                                    ICell cellTot9 = row.CreateCell(totalCount1);
                                    cellTot9.SetCellValue(Math.Round(TotalProfTax, MidpointRounding.AwayFromZero).ToString());
                                    cellTot9.CellStyle = styleGrey50;
                                }
                                if (clientRequirement.CRI_RevenueDeduction)
                                {
                                    totalCount1 = totalCount1 + 1;
                                    ICell cellTot10 = row.CreateCell(totalCount1);
                                    cellTot10.SetCellValue(Math.Round(TotalRevenue, MidpointRounding.AwayFromZero).ToString());
                                    cellTot10.CellStyle = styleGrey50;
                                }
                                if (clientRequirement.CRI_CanteenFacility)
                                {
                                    totalCount1 = totalCount1 + 1;
                                    ICell cellTot11 = row.CreateCell(totalCount1);
                                    cellTot11.SetCellValue(Math.Round(TotalCanteenFacility, MidpointRounding.AwayFromZero).ToString());
                                    cellTot11.CellStyle = styleGrey50;
                                }

                                ICell cellTot12 = row.CreateCell(totalCount1 + 1);
                                cellTot12.SetCellValue(Math.Round(TotalAdvance, MidpointRounding.AwayFromZero).ToString());
                                cellTot12.CellStyle = styleGrey50;

                                int TotCount = totalCount1 + 2;
                                if (!dd.DES_Exclude_LWF)
                                {
                                    ICell cellTotLWF = row.CreateCell(TotCount);
                                    cellTotLWF.SetCellValue(Math.Round(TotalLWF, MidpointRounding.AwayFromZero).ToString());
                                    cellTotLWF.CellStyle = styleGrey50;
                                    TotCount = TotCount + 1;
                                }

                                ICell cellTot13 = row.CreateCell(TotCount);
                                cellTot13.SetCellValue(Math.Round(TotalDeduct, MidpointRounding.AwayFromZero).ToString());
                                cellTot13.CellStyle = styleGrey50;
                                ICell cellTot14 = row.CreateCell(TotCount + 1);
                                cellTot14.SetCellValue(Math.Round(TotalFinal, MidpointRounding.AwayFromZero).ToString());
                                cellTot14.CellStyle = styleGrey80;
                                #endregion

                                DesCount = DesCount + 1;
                            }

                        }
                    }

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

    }
}