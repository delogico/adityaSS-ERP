using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;
using RMERP.DAL.ManagerClasses;
using System.IO;
using NPOI.SS.UserModel;
using RMERP.DAL.Helpers;
using Microsoft.AspNetCore.Hosting;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.Util;
using System.Text;
using RMERP.DAL.Mappers;
using RMERP.Helpers;
using Rotativa.AspNetCore;
using Microsoft.Extensions.Configuration;
using Rotativa.AspNetCore.Options;

namespace RMERP.Controllers
{
    public class ReportsController : Controller
    {
        public IConfiguration _configuration;
        private readonly RMERPContext _context;
        private IHostingEnvironment _hostingEnvironment;
        public ReportsController(RMERPContext context, IHostingEnvironment hostingEnvironment, IConfiguration configuration)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
        }

        public IActionResult PFReport(int WAG_Id)
        {
            ReportsManager manager = new ReportsManager(_context);
            return View(manager.PFReports(WAG_Id));
        }
        public async Task<FileResult> PayTaxExcel(int WAG_Id)
        {
            ReportsManager manager = new ReportsManager(_context);
            WageProcessManager wageProcess = new WageProcessManager(_context);
            Wage_Process wage_Process = wageProcess.getWageProcessById(WAG_Id);
            DateTime wageMonth = wage_Process.WAG_Month;
            string WAG_Month = wageMonth.ToString("MMMM") + "-" + wageMonth.ToString("yyyy");

            string newPath = ProjectUtils.GetTempFolderPath(_hostingEnvironment.WebRootPath);
            string fileName = "Pay Tax_" + DateTime.Now.ToString("ddMMyyyyHHmm") + "_" + WAG_Month + ".xlsx";
            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, fileName);
            FileInfo file = new FileInfo(Path.Combine(newPath, fileName));
            var memory = new MemoryStream();
            using (var fs = new FileStream(Path.Combine(newPath, fileName), FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook;
                workbook = new XSSFWorkbook();
                ISheet excelSheet = workbook.CreateSheet("Template");
                IFont font = workbook.CreateFont();
                font.IsBold = true;
                font.FontHeightInPoints = ((short)40);
                font.FontName = ("Cambria");
                font.Underline = FontUnderlineType.Single;
                font.Color = IndexedColors.Brown.Index;

                ICellStyle styleHeader = workbook.CreateCellStyle();
                styleHeader.FillBackgroundColor = HSSFColor.Aqua.Index;
                styleHeader.SetFont(font);

                ICellStyle styleAmount = workbook.CreateCellStyle();
                styleAmount.FillBackgroundColor = IndexedColors.Yellow.Index;
                styleAmount.FillPattern = FillPattern.SolidForeground;
                styleAmount.FillForegroundColor = IndexedColors.Yellow.Index;
                styleAmount.BorderBottom = (BorderStyle.Thin);
                styleAmount.BottomBorderColor = (IndexedColors.Black.Index);
                styleAmount.BorderLeft = (BorderStyle.Thin);
                styleAmount.LeftBorderColor = (IndexedColors.Black.Index);
                styleAmount.BorderRight = (BorderStyle.Thin);
                styleAmount.RightBorderColor = (IndexedColors.Black.Index);
                styleAmount.BorderTop = (BorderStyle.Thin);
                styleAmount.TopBorderColor = (IndexedColors.Black.Index);

                // Style the cell with font color white
                IFont fontcell = workbook.CreateFont();
                fontcell.IsBold = true;

                IFont fontClient = workbook.CreateFont();
                fontClient.IsBold = true;
                fontClient.FontHeightInPoints = ((short)15);

                IFont fontcellSub = workbook.CreateFont();
                fontcellSub.IsBold = true;
                fontcellSub.FontHeightInPoints = ((short)18);

                // Grey25Percent background
                ICellStyle style = workbook.CreateCellStyle();
                ICellStyle styleTotal = workbook.CreateCellStyle();
                ICellStyle styleClient = workbook.CreateCellStyle();
                ICellStyle styleSub = workbook.CreateCellStyle();
                ICellStyle styleBorder = workbook.CreateCellStyle();

                styleTotal.FillForegroundColor = IndexedColors.BrightGreen.Index;
                styleTotal.FillPattern = FillPattern.SolidForeground;
                styleTotal.FillBackgroundColor = IndexedColors.BrightGreen.Index;
                styleTotal.SetFont(fontClient);

                styleClient.SetFont(fontClient);
                styleSub.SetFont(fontcellSub);
                //#77bf2a

                // Style the cell with borders all around.
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

                //Border
                styleBorder.VerticalAlignment = VerticalAlignment.Center;
                styleBorder.BorderBottom = (BorderStyle.Thin);
                styleBorder.BottomBorderColor = (IndexedColors.Black.Index);
                styleBorder.BorderLeft = (BorderStyle.Thin);
                styleBorder.LeftBorderColor = (IndexedColors.Black.Index);
                styleBorder.BorderRight = (BorderStyle.Thin);
                styleBorder.RightBorderColor = (IndexedColors.Black.Index);
                styleBorder.BorderTop = (BorderStyle.Thin);
                styleBorder.TopBorderColor = (IndexedColors.Black.Index);

                IRow row = excelSheet.CreateRow(0);
                ICell CellHeader = row.CreateCell(0);
                CellHeader.SetCellValue("Reliable");
                CellHeader.CellStyle = styleHeader;
                CellUtil.SetAlignment(CellHeader, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 9));

                IRow rowSub = excelSheet.CreateRow(1);
                ICell CellSub = rowSub.CreateCell(0);
                CellSub.SetCellValue(wage_Process.FRM_.FRM_Name.Replace("Reliable","").ToUpper());
                CellSub.CellStyle = styleSub;
                CellUtil.SetAlignment(CellSub, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 9));

                IRow rowAdd1 = excelSheet.CreateRow(2);
                ICell CellAdd1 = rowAdd1.CreateCell(0);
                CellAdd1.SetCellValue(wage_Process.FRM_.FRM_Address1+","+ wage_Process.FRM_.FRM_Address2+",");
                CellUtil.SetAlignment(CellAdd1, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 9));

                IRow rowAdd2 = excelSheet.CreateRow(3);
                ICell CellAdd2 = rowAdd2.CreateCell(0);
                CellAdd2.SetCellValue("Ph.- 0231-2666389. Mobile : 9922967130. E-mail : "+ wage_Process.FRM_.FRM_Email);
                CellUtil.SetAlignment(CellAdd2, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(3, 3, 0, 9));

                IRow rowSubHeading = excelSheet.CreateRow(4);
                ICell CellSubHeading = rowSubHeading.CreateCell(0);
                CellSubHeading.SetCellValue("DETAILS OF PAY TAX " + WAG_Month);
                CellSubHeading.CellStyle = styleClient;
                CellUtil.SetAlignment(CellSubHeading, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(4, 4, 0, 9));

                row = excelSheet.CreateRow(5);
                ICell cellPTRC = row.CreateCell(0);
                cellPTRC.SetCellValue("PTRC No - 27565016092");
                excelSheet.AddMergedRegion(new CellRangeAddress(5, 5, 0, 9));

                row = excelSheet.CreateRow(6);
                row.HeightInPoints = (float)(3.2 * excelSheet.DefaultRowHeightInPoints);
                ICell cell0 = row.CreateCell(0);
                cell0.SetCellValue("SR. NO.");
                cell0.CellStyle = style;
                ICell cell1 = row.CreateCell(1);
                cell1.SetCellValue("UNIT");
                excelSheet.SetColumnWidth(1, (int)((25 + 0.72) * 256));//A
                cell1.CellStyle = style;
                ICell cell2 = row.CreateCell(2);
                cell2.SetCellValue("Upto 7500");
                cell2.CellStyle = style;
                ICell cell3 = row.CreateCell(3);
                cell3.SetCellValue("Upto 7500 Ladies");
                cell3.CellStyle = style;
                ICell cell4 = row.CreateCell(4);
                cell4.SetCellValue("Upto 10000");
                cell4.CellStyle = style;
                ICell cell5 = row.CreateCell(5);
                cell5.SetCellValue("Upto 10000 Ladies");
                cell5.CellStyle = style;
                ICell cell6 = row.CreateCell(6);
                cell6.SetCellValue("Above 10000");
                cell6.CellStyle = style;
                ICell cell7 = row.CreateCell(7);
                cell7.SetCellValue("Above 10000 Ladies");
                cell7.CellStyle = style;
                ICell cell8 = row.CreateCell(8);
                cell8.SetCellValue("STREGNTH");
                cell8.CellStyle = style;
                ICell cell9 = row.CreateCell(9);
                cell9.SetCellValue("AMOUNT");
                cell9.CellStyle = style;

                List<PayTaxReportVM> PayTaxReports = manager.PayTaxReports(WAG_Id);
                int rowCount = 7, count = 1;
                int UpTo7500 = 0, UpTo7500Ladies = 0, UpTo10000 = 0, UpTo10000Ladies = 0, Above10000 = 0, Above10000Ladies = 0, Strength = 0;
                decimal Amount = 0M;
                foreach (var item in PayTaxReports)
                {
                    row = excelSheet.CreateRow(rowCount);
                    row.HeightInPoints = (float)(1.5 * excelSheet.DefaultRowHeightInPoints);
                    row.CreateCell(0).SetCellValue(count);
                    row.CreateCell(1).SetCellValue(item.CLI_Name);
                    row.CreateCell(2).SetCellValue(item.UpTo7500);
                    row.CreateCell(3).SetCellValue(item.UpTo7500Ladies);
                    row.CreateCell(4).SetCellValue(item.UpTo10000);
                    row.CreateCell(5).SetCellValue(item.UpTo10000Ladies);
                    row.CreateCell(6).SetCellValue(item.Above10000);
                    row.CreateCell(7).SetCellValue(item.Above10000Ladies);
                    row.CreateCell(8).SetCellValue(item.STREGNTH());
                    row.CreateCell(9).SetCellValue(Convert.ToDouble(item.AMOUNT()));
                    UpTo7500 = UpTo7500 + item.UpTo7500;
                    UpTo7500Ladies = UpTo7500Ladies + item.UpTo7500Ladies;
                    UpTo10000 = UpTo10000 + item.UpTo10000;
                    UpTo10000Ladies = UpTo10000Ladies + item.UpTo10000Ladies;
                    Above10000 = Above10000 + item.Above10000;
                    Above10000Ladies = Above10000Ladies + item.Above10000Ladies;
                    Strength = Strength + item.STREGNTH();
                    Amount = Amount + item.AMOUNT();
                    rowCount++;
                    count++;
                }
                row = excelSheet.CreateRow(rowCount);

                ICell cellTotal = row.CreateCell(0);
                cellTotal.SetCellValue("Total");
                cellTotal.CellStyle = style;
                ICell cellEmpty = row.CreateCell(1);
                cellEmpty.CellStyle = style;
                excelSheet.AddMergedRegion(new CellRangeAddress(rowCount, rowCount, 0, 1));

                ICell cellTotal2 = row.CreateCell(2);
                cellTotal2.SetCellValue(UpTo7500);
                cellTotal2.CellStyle = style;
                ICell cellTotal3 = row.CreateCell(3);
                cellTotal3.SetCellValue(UpTo7500Ladies);
                cellTotal3.CellStyle = style;
                ICell cellTotal4 = row.CreateCell(4);
                cellTotal4.SetCellValue(UpTo10000);
                cellTotal4.CellStyle = style;
                ICell cellTotal5 = row.CreateCell(5);
                cellTotal5.SetCellValue(UpTo10000Ladies);
                cellTotal5.CellStyle = style;
                ICell cellTotal6 = row.CreateCell(6);
                cellTotal6.SetCellValue(Above10000);
                cellTotal6.CellStyle = style;
                ICell cellTotal7 = row.CreateCell(7);
                cellTotal7.SetCellValue(Above10000Ladies);
                cellTotal7.CellStyle = style;
                ICell cellTotal8 = row.CreateCell(8);
                cellTotal8.SetCellValue(Strength);
                cellTotal8.CellStyle = style;
                ICell cellTotal9 = row.CreateCell(9);
                cellTotal9.SetCellValue(Convert.ToDouble(Amount));
                cellTotal9.CellStyle = style;

                row = excelSheet.CreateRow(rowCount + 2);
                ICell cellSummary = row.CreateCell(1);
                cellSummary.SetCellValue("GROSS SALARY");
                cellSummary.CellStyle = style;
                ICell cellEmp = row.CreateCell(2);
                cellEmp.SetCellValue("NO OF EMP.");
                cellEmp.CellStyle = style;
                ICell cellRate = row.CreateCell(3);
                cellRate.SetCellValue("RATE");
                cellRate.CellStyle = style;
                ICell cellTtl = row.CreateCell(4);
                cellTtl.SetCellValue("TOTAL");
                cellTtl.CellStyle = style;

                row = excelSheet.CreateRow(rowCount + 3);
                ICell cellSummary1 = row.CreateCell(1);
                cellSummary1.SetCellValue("UP TO 7500");
                cellSummary1.CellStyle = styleBorder;
                CellUtil.SetAlignment(cellSummary1, workbook, (short)HorizontalAlignment.Center);
                ICell cellEmp1 = row.CreateCell(2);
                cellEmp1.SetCellValue(UpTo7500);
                cellEmp1.CellStyle = styleBorder;
                ICell cellRate1 = row.CreateCell(3);
                cellRate1.SetCellValue(0);
                cellRate1.CellStyle = styleBorder;
                ICell cellTtl1 = row.CreateCell(4);
                cellTtl1.SetCellValue(0);
                cellTtl1.CellStyle = styleBorder;

                row = excelSheet.CreateRow(rowCount + 4);
                ICell cellSummary2 = row.CreateCell(1);
                cellSummary2.SetCellValue("UP TO 7500 Ladies");
                cellSummary2.CellStyle = style;
                CellUtil.SetAlignment(cellSummary2, workbook, (short)HorizontalAlignment.Center);
                ICell cellEmp2 = row.CreateCell(2);
                cellEmp2.SetCellValue(UpTo7500Ladies);
                cellEmp2.CellStyle = styleBorder;
                ICell cellRate2 = row.CreateCell(3);
                cellRate2.SetCellValue(0);
                cellRate2.CellStyle = styleBorder;
                ICell cellTtl2 = row.CreateCell(4);
                cellTtl2.SetCellValue(0);
                cellTtl2.CellStyle = styleBorder;

                row = excelSheet.CreateRow(rowCount + 5);
                ICell cellSummary3 = row.CreateCell(1);
                cellSummary3.SetCellValue("7500 TO 10000");
                cellSummary3.CellStyle = styleBorder;
                CellUtil.SetAlignment(cellSummary3, workbook, (short)HorizontalAlignment.Center);
                ICell cellEmp3 = row.CreateCell(2);
                cellEmp3.SetCellValue(UpTo10000);
                cellEmp3.CellStyle = styleBorder;
                ICell cellRate3 = row.CreateCell(3);
                cellRate3.SetCellValue(175);
                cellRate3.CellStyle = styleBorder;
                ICell cellTtl3 = row.CreateCell(4);
                cellTtl3.SetCellValue((175 * UpTo10000));
                cellTtl3.CellStyle = styleBorder;

                row = excelSheet.CreateRow(rowCount + 6);
                ICell cellSummary4 = row.CreateCell(1);
                cellSummary4.SetCellValue("UP TO 10000 Ladies");
                cellSummary4.CellStyle = style;
                CellUtil.SetAlignment(cellSummary4, workbook, (short)HorizontalAlignment.Center);
                ICell cellEmp4 = row.CreateCell(2);
                cellEmp4.SetCellValue(UpTo10000Ladies);
                cellEmp4.CellStyle = styleBorder;
                ICell cellRate4 = row.CreateCell(3);
                cellRate4.SetCellValue(0);
                cellRate4.CellStyle = styleBorder;
                ICell cellTtl4 = row.CreateCell(4);
                cellTtl4.SetCellValue(0);
                cellTtl4.CellStyle = styleBorder;

                row = excelSheet.CreateRow(rowCount + 7);
                ICell cellSummary5 = row.CreateCell(1);
                cellSummary5.SetCellValue("ABOVE 10000");
                cellSummary5.CellStyle = styleBorder;
                CellUtil.SetAlignment(cellSummary5, workbook, (short)HorizontalAlignment.Center);
                ICell cellEmp5 = row.CreateCell(2);
                cellEmp5.SetCellValue(Above10000);
                cellEmp5.CellStyle = styleBorder;
                ICell cellRate5 = row.CreateCell(3);
                cellRate5.SetCellValue(200);
                cellRate5.CellStyle = styleBorder;
                ICell cellTtl5 = row.CreateCell(4);
                cellTtl5.SetCellValue((200 * Above10000));
                cellTtl5.CellStyle = styleBorder;

                row = excelSheet.CreateRow(rowCount + 8);
                ICell cellSummary6 = row.CreateCell(1);
                cellSummary6.SetCellValue("ABOVE 10000 Ladies");
                cellSummary6.CellStyle = style;
                CellUtil.SetAlignment(cellSummary6, workbook, (short)HorizontalAlignment.Center);
                ICell cellEmp6 = row.CreateCell(2);
                cellEmp6.SetCellValue(Above10000Ladies);
                cellEmp6.CellStyle = styleBorder;
                ICell cellRate6 = row.CreateCell(3);
                cellRate6.SetCellValue(200);
                cellRate6.CellStyle = styleBorder;
                ICell cellTtl6 = row.CreateCell(4);
                cellTtl6.SetCellValue((200 * Above10000Ladies));
                cellTtl6.CellStyle = styleBorder;

                row = excelSheet.CreateRow(rowCount + 9);
                ICell cellSummaryFinal = row.CreateCell(1);
                cellSummaryFinal.SetCellValue("Total");
                cellSummaryFinal.CellStyle = style;
                CellUtil.SetAlignment(cellSummaryFinal, workbook, (short)HorizontalAlignment.Center);
                ICell cellEmpFinal = row.CreateCell(2);
                cellEmpFinal.SetCellValue(Strength);
                cellEmpFinal.CellStyle = style;
                ICell cellRateFinal = row.CreateCell(3);
                cellRateFinal.SetCellValue("");
                cellRateFinal.CellStyle = style;
                ICell cellTtlFinal = row.CreateCell(4);
                cellTtlFinal.SetCellValue(Convert.ToDouble(Amount));
                cellTtlFinal.CellStyle = style;

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

        #region ADVANCE Report

        public async Task<FileResult> IDBI_To_IDBI_AdvanceReportExcel(int WAG_Id, int FRM_Id)
        {
            ReportsManager manager = new ReportsManager(_context);
            WageProcessManager wageProcess = new WageProcessManager(_context);
            Wage_Process wage_Process = wageProcess.getWageProcessById(WAG_Id);
            DateTime wageMonth = wage_Process.WAG_Month;
            string WAG_Month = wageMonth.ToString("MMMM") + "-" + wageMonth.ToString("yyyy");

            string newPath = ProjectUtils.GetTempFolderPath(_hostingEnvironment.WebRootPath);
            string fileName = "IDBI_To_IDBI_BankReport_" + DateTime.Now.ToString("ddMMyyyyHHmm") + "_" + WAG_Month + ".xlsx";
            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, fileName);
            FileInfo file = new FileInfo(Path.Combine(newPath, fileName));
            var memory = new MemoryStream();
            using (var fs = new FileStream(Path.Combine(newPath, fileName), FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook;
                workbook = new XSSFWorkbook();
                ISheet excelSheet = workbook.CreateSheet("Template");
                IFont font = workbook.CreateFont();
                font.IsBold = true;
                font.FontHeightInPoints = ((short)40);
                font.FontName = ("Cambria");
                font.Underline = FontUnderlineType.Single;
                font.Color = IndexedColors.Brown.Index;

                ICellStyle styleHeader = workbook.CreateCellStyle();
                styleHeader.FillBackgroundColor = HSSFColor.Aqua.Index;
                styleHeader.SetFont(font);

                ICellStyle styleAmount = workbook.CreateCellStyle();
                styleAmount.FillBackgroundColor = IndexedColors.Yellow.Index;
                styleAmount.FillPattern = FillPattern.SolidForeground;
                styleAmount.FillForegroundColor = IndexedColors.Yellow.Index;
                styleAmount.BorderBottom = (BorderStyle.Thin);
                styleAmount.BottomBorderColor = (IndexedColors.Black.Index);
                styleAmount.BorderLeft = (BorderStyle.Thin);
                styleAmount.LeftBorderColor = (IndexedColors.Black.Index);
                styleAmount.BorderRight = (BorderStyle.Thin);
                styleAmount.RightBorderColor = (IndexedColors.Black.Index);
                styleAmount.BorderTop = (BorderStyle.Thin);
                styleAmount.TopBorderColor = (IndexedColors.Black.Index);

                // Style the cell with font color white
                IFont fontcell = workbook.CreateFont();
                fontcell.IsBold = true;

                IFont fontClient = workbook.CreateFont();
                fontClient.IsBold = true;
                fontClient.FontHeightInPoints = ((short)15);

                IFont fontcellSub = workbook.CreateFont();
                fontcellSub.IsBold = true;
                fontcellSub.FontHeightInPoints = ((short)18);

                // Grey25Percent background
                ICellStyle style = workbook.CreateCellStyle();
                ICellStyle styleTotal = workbook.CreateCellStyle();
                ICellStyle styleClient = workbook.CreateCellStyle();
                ICellStyle styleSub = workbook.CreateCellStyle();

                styleTotal.FillForegroundColor = IndexedColors.BrightGreen.Index;
                styleTotal.FillPattern = FillPattern.SolidForeground;
                styleTotal.FillBackgroundColor = IndexedColors.BrightGreen.Index;
                styleTotal.SetFont(fontcell);

                styleClient.SetFont(fontcell);
                styleSub.SetFont(fontcellSub);
                //#77bf2a

                // Style the cell with borders all around.
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
                style.SetFont(fontcell);

                IRow row = excelSheet.CreateRow(0);
                ICell CellHeader = row.CreateCell(0);
                CellHeader.SetCellValue("Reliable");
                CellHeader.CellStyle = styleHeader;
                CellUtil.SetAlignment(CellHeader, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 6));

                IRow rowSub = excelSheet.CreateRow(1);
                ICell CellSub = rowSub.CreateCell(0);
                CellSub.SetCellValue(wage_Process.FRM_.FRM_Name.Replace("Reliable", "").ToUpper());
                CellSub.CellStyle = styleSub;
                CellUtil.SetAlignment(CellSub, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 6));

                IRow rowAdd1 = excelSheet.CreateRow(2);
                ICell CellAdd1 = rowAdd1.CreateCell(0);
                CellAdd1.SetCellValue(wage_Process.FRM_.FRM_Address1 + "," + wage_Process.FRM_.FRM_Address2 + ",");
                CellUtil.SetAlignment(CellAdd1, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 6));

                IRow rowAdd2 = excelSheet.CreateRow(3);
                ICell CellAdd2 = rowAdd2.CreateCell(0);
                CellAdd2.SetCellValue("Ph.- 0231-2666389. Mobile : 9922967130. E-mail : "+ wage_Process.FRM_.FRM_Email);
                CellUtil.SetAlignment(CellAdd2, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(3, 3, 0, 6));

                IRow rowSubHeading = excelSheet.CreateRow(4);
                ICell CellSubHeading = rowSubHeading.CreateCell(0);
                CellSubHeading.SetCellValue("SALARY FOR THE MONTH OF " + WAG_Month);
                CellSubHeading.CellStyle = styleClient;
                CellUtil.SetAlignment(CellSubHeading, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(4, 4, 0, 6));


                row = excelSheet.CreateRow(5);
                row.HeightInPoints = (float)(3.2 * excelSheet.DefaultRowHeightInPoints);
                ICell cell0 = row.CreateCell(0);
                cell0.SetCellValue("EMP NAME");
                excelSheet.SetColumnWidth(0, (int)((25 + 0.72) * 256));//A
                cell0.CellStyle = style;
                ICell cell1 = row.CreateCell(1);
                cell1.SetCellValue("ACCOUNT \r\n NUMBER");
                excelSheet.SetColumnWidth(1, (int)((22 + 0.72) * 256));//A
                cell1.CellStyle = style;
                ICell cell2 = row.CreateCell(2);
                cell2.SetCellValue("CURRENCY \r\n CODE");
                excelSheet.SetColumnWidth(2, (int)((15 + 0.72) * 256));//A
                cell2.CellStyle = style;
                ICell cell3 = row.CreateCell(3);
                cell3.SetCellValue("SERVICE \r\n OUTLET");
                cell3.CellStyle = style;
                ICell cell4 = row.CreateCell(4);
                cell4.SetCellValue("PART \r\n TRAN \r\n TYPE");
                cell4.CellStyle = style;
                ICell cell5 = row.CreateCell(5);
                cell5.SetCellValue("TRANSACTION \r\n AMOUNT");
                excelSheet.SetColumnWidth(5, (int)((15 + 0.72) * 256));//A
                cell5.CellStyle = style;
                ICell cell6 = row.CreateCell(6);
                cell6.SetCellValue("TRANSACTION \r\n PARTICULARS");
                excelSheet.SetColumnWidth(6, (int)((22 + 0.72) * 256));//A
                cell6.CellStyle = style;

                List<BankReportVM> BankReportVMs = manager.IDBI_TO_IDBI_AdvanceReports(WAG_Id, FRM_Id);
                int rowCount = 6;
                decimal TRANSACTION_AMOUNT = 0M;
                foreach (var item in BankReportVMs)
                {
                    row = excelSheet.CreateRow(rowCount);
                    row.CreateCell(0).SetCellValue(item.EMP_NAME);
                    row.CreateCell(1).SetCellValue(item.EMP_ACCOUNT_NUMBER);
                    row.CreateCell(2).SetCellValue(item.EMP_CURRENCY_CODE);
                    row.CreateCell(3).SetCellValue(item.EMP_SERVICE_OUTLET);
                    row.CreateCell(4).SetCellValue(item.EMP_PART_TRAN_TYPE);
                    row.CreateCell(5).SetCellValue(Convert.ToString(item.EMP_TRANSACTION_AMOUNT));
                    row.CreateCell(6).SetCellValue(item.EMP_TRANSACTION_PARTICULARS);
                    TRANSACTION_AMOUNT = TRANSACTION_AMOUNT + item.EMP_TRANSACTION_AMOUNT;
                    rowCount++;
                }
                row = excelSheet.CreateRow(rowCount);
                ICell CellTotal = row.CreateCell(2);
                CellTotal.SetCellValue("GRAND TOTAL");
                CellTotal.CellStyle = styleClient;
                CellUtil.SetAlignment(CellTotal, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(rowCount, rowCount, 2, 4));
                ICell cellTotalAmt = row.CreateCell(5);
                cellTotalAmt.SetCellValue(Convert.ToString(TRANSACTION_AMOUNT));
                cellTotalAmt.CellStyle = styleTotal;
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

        public async Task<FileResult> IDBI_To_Other_AdvanceReportExcel(int WAG_Id, int FRM_Id)
        {
            ReportsManager manager = new ReportsManager(_context);
            WageProcessManager wageProcess = new WageProcessManager(_context);
            Wage_Process wage_Process = wageProcess.getWageProcessById(WAG_Id);
            DateTime wageMonth = wage_Process.WAG_Month;
            string WAG_Month = wageMonth.ToString("MMMM") + "-" + wageMonth.ToString("yyyy");

            string newPath = ProjectUtils.GetTempFolderPath(_hostingEnvironment.WebRootPath);
            string fileName = "IDBI_To_Other_BankReport_" + DateTime.Now.ToString("ddMMyyyyHHmm") + "_" + WAG_Month + ".xlsx";
            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, fileName);
            FileInfo file = new FileInfo(Path.Combine(newPath, fileName));
            var memory = new MemoryStream();
            using (var fs = new FileStream(Path.Combine(newPath, fileName), FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook;
                workbook = new XSSFWorkbook();
                ISheet excelSheet = workbook.CreateSheet("Template");
                IFont font = workbook.CreateFont();
                font.IsBold = true;
                font.FontHeightInPoints = ((short)40);
                font.FontName = ("Cambria");
                font.Underline = FontUnderlineType.Single;
                font.Color = IndexedColors.Brown.Index;

                ICellStyle styleHeader = workbook.CreateCellStyle();
                styleHeader.FillBackgroundColor = HSSFColor.Aqua.Index;
                styleHeader.SetFont(font);

                // Style the cell with font color white
                IFont fontcell = workbook.CreateFont();
                fontcell.IsBold = true;

                IFont fontClient = workbook.CreateFont();
                fontClient.IsBold = true;
                fontClient.FontHeightInPoints = ((short)15);

                IFont fontcellSub = workbook.CreateFont();
                fontcellSub.IsBold = true;
                fontcellSub.FontHeightInPoints = ((short)18);

                // Grey25Percent background
                ICellStyle style = workbook.CreateCellStyle();
                ICellStyle styleTotal = workbook.CreateCellStyle();
                ICellStyle styleClient = workbook.CreateCellStyle();
                ICellStyle styleSub = workbook.CreateCellStyle();

                styleTotal.WrapText = true;
                styleTotal.SetFont(fontcell);

                styleClient.SetFont(fontcell);
                styleSub.SetFont(fontcellSub);

                // Style the cell with borders all around.
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
                style.SetFont(fontcell);

                IRow row = excelSheet.CreateRow(0);
                ICell CellHeader = row.CreateCell(0);
                CellHeader.SetCellValue("Reliable");
                CellHeader.CellStyle = styleHeader;
                CellUtil.SetAlignment(CellHeader, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 9));

                IRow rowSub = excelSheet.CreateRow(1);
                ICell CellSub = rowSub.CreateCell(0);
                CellSub.SetCellValue(wage_Process.FRM_.FRM_Name.Replace("Reliable", "").ToUpper());
                CellSub.CellStyle = styleSub;
                CellUtil.SetAlignment(CellSub, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 9));

                IRow rowAdd1 = excelSheet.CreateRow(2);
                ICell CellAdd1 = rowAdd1.CreateCell(0);
                CellAdd1.SetCellValue(wage_Process.FRM_.FRM_Address1 + "," + wage_Process.FRM_.FRM_Address2 + ",");
                CellUtil.SetAlignment(CellAdd1, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 9));

                IRow rowAdd2 = excelSheet.CreateRow(3);
                ICell CellAdd2 = rowAdd2.CreateCell(0);
                CellAdd2.SetCellValue("Ph.- 0231-2666389. Mobile : 9922967130. E-mail : "+ wage_Process.FRM_.FRM_Email);
                CellUtil.SetAlignment(CellAdd2, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(3, 3, 0, 9));

                IRow rowSubHeading = excelSheet.CreateRow(4);
                ICell CellSubHeading = rowSubHeading.CreateCell(0);
                CellSubHeading.SetCellValue("SALARY FOR THE MONTH OF " + WAG_Month);
                CellSubHeading.CellStyle = styleClient;
                CellUtil.SetAlignment(CellSubHeading, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(4, 4, 0, 9));

                row = excelSheet.CreateRow(5);
                ICell cell_1 = row.CreateCell(0);
                cell_1.SetCellValue("1");
                cell_1.CellStyle = style;
                CellUtil.SetAlignment(cell_1, workbook, (short)HorizontalAlignment.Center);
                excelSheet.SetColumnWidth(0, (int)((10 + 0.72) * 256));
                ICell cell_2 = row.CreateCell(1);
                cell_2.SetCellValue("2");
                cell_2.CellStyle = style;
                CellUtil.SetAlignment(cell_2, workbook, (short)HorizontalAlignment.Center);
                excelSheet.SetColumnWidth(1, (int)((15 + 0.72) * 256));
                ICell cell_3 = row.CreateCell(2);
                cell_3.SetCellValue("3");
                cell_3.CellStyle = style;
                CellUtil.SetAlignment(cell_3, workbook, (short)HorizontalAlignment.Center);
                excelSheet.SetColumnWidth(2, (int)((12 + 0.72) * 256));
                ICell cell_4 = row.CreateCell(3);
                cell_4.SetCellValue("4");
                cell_4.CellStyle = style;
                CellUtil.SetAlignment(cell_4, workbook, (short)HorizontalAlignment.Center);
                excelSheet.SetColumnWidth(3, (int)((15 + 0.72) * 256));
                ICell cell_5 = row.CreateCell(4);
                cell_5.SetCellValue("5");
                cell_5.CellStyle = style;
                CellUtil.SetAlignment(cell_5, workbook, (short)HorizontalAlignment.Center);
                ICell cell_6 = row.CreateCell(5);
                cell_6.SetCellValue("6");
                cell_6.CellStyle = style;
                CellUtil.SetAlignment(cell_6, workbook, (short)HorizontalAlignment.Center);
                excelSheet.SetColumnWidth(5, (int)((25 + 0.72) * 256));
                ICell cell_7 = row.CreateCell(6);
                cell_7.SetCellValue("7");
                cell_7.CellStyle = style;
                CellUtil.SetAlignment(cell_7, workbook, (short)HorizontalAlignment.Center);
                excelSheet.SetColumnWidth(6, (int)((12 + 0.72) * 256));
                ICell cell_8 = row.CreateCell(7);
                cell_8.SetCellValue("8");
                cell_8.CellStyle = style;
                CellUtil.SetAlignment(cell_8, workbook, (short)HorizontalAlignment.Center);
                excelSheet.SetColumnWidth(7, (int)((12 + 0.72) * 256));
                ICell cell_9 = row.CreateCell(8);
                cell_9.SetCellValue("9");
                cell_9.CellStyle = style;
                CellUtil.SetAlignment(cell_9, workbook, (short)HorizontalAlignment.Center);
                excelSheet.SetColumnWidth(8, (int)((12 + 0.72) * 256));
                ICell cell_10 = row.CreateCell(9);
                cell_10.SetCellValue("");
                cell_10.CellStyle = style;
                CellUtil.SetAlignment(cell_10, workbook, (short)HorizontalAlignment.Center);
                excelSheet.SetColumnWidth(8, (int)((12 + 0.72) * 256));

                row = excelSheet.CreateRow(6);
                row.HeightInPoints = (float)(3.2 * excelSheet.DefaultRowHeightInPoints);
                ICell cell0 = row.CreateCell(0);
                cell0.SetCellValue("AMOUNT");
                cell0.CellStyle = style;
                ICell cell1 = row.CreateCell(1);
                cell1.SetCellValue("ACCOUNT \r\n SENDERS \r\n NUMBER");
                cell1.CellStyle = style;
                ICell cell2 = row.CreateCell(2);
                cell2.SetCellValue("CODE \r\n IFSC");
                cell2.CellStyle = style;
                ICell cell3 = row.CreateCell(3);
                cell3.SetCellValue("ACCOUNT \r\n RECEIVERS \r\n NUMBER");
                cell3.CellStyle = style;
                ICell cell4 = row.CreateCell(4);
                cell4.SetCellValue("TYPE \r\n A/C");
                cell4.CellStyle = style;
                ICell cell5 = row.CreateCell(5);
                cell5.SetCellValue("NAME \r\n BENIFECIARY");
                cell5.CellStyle = style;
                ICell cell6 = row.CreateCell(6);
                cell6.SetCellValue("ADDRESS");
                cell6.CellStyle = style;
                ICell cell7 = row.CreateCell(7);
                cell7.SetCellValue("MESSAGE");
                cell7.CellStyle = style;
                ICell cell8 = row.CreateCell(8);
                cell8.SetCellValue("ORIGINETOR");
                cell8.CellStyle = style;
                ICell cell9 = row.CreateCell(9);
                cell9.SetCellValue("");
                cell9.CellStyle = style;

                List<BankReportVM> BankReportVMs = manager.IDBI_TO_Other_AdvanceReports(WAG_Id, FRM_Id);
                int rowCount = 7;
                decimal TRANSACTION_AMOUNT = 0M;
                foreach (var item in BankReportVMs)
                {
                    row = excelSheet.CreateRow(rowCount);
                    row.HeightInPoints = (float)(1.2 * excelSheet.DefaultRowHeightInPoints);
                    row.CreateCell(0).SetCellValue(Convert.ToString(item.EMP_TRANSACTION_AMOUNT));
                    row.CreateCell(1).SetCellValue(item.ACCOUNT_SENDER_NUMBER);
                    row.CreateCell(2).SetCellValue(item.ACCOUNT_IFSC_CODE);
                    row.CreateCell(3).SetCellValue(item.ACCOUNT_RECEIVERS_NUMBER);
                    row.CreateCell(4).SetCellValue(item.ACCOUNT_TYPE);
                    row.CreateCell(5).SetCellValue(item.EMP_NAME);
                    row.CreateCell(6).SetCellValue(item.EMP_ADDRESS);
                    row.CreateCell(7).SetCellValue(item.MESSAGE);
                    row.CreateCell(8).SetCellValue(item.ORIGINETOR);
                    row.CreateCell(9).SetCellValue(item.COMPANY_NAME);
                    TRANSACTION_AMOUNT = TRANSACTION_AMOUNT + item.EMP_TRANSACTION_AMOUNT;
                    rowCount++;
                }
                row = excelSheet.CreateRow(rowCount);
                row.HeightInPoints = (float)(1.2 * excelSheet.DefaultRowHeightInPoints);
                ICell cellTotalAmt = row.CreateCell(0);
                cellTotalAmt.SetCellValue(Convert.ToString(TRANSACTION_AMOUNT));
                cellTotalAmt.CellStyle = styleTotal;
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

        public async Task<FileResult> CHEQUE_CASH_AdvanceReportExcel(int WAG_Id, int FRM_Id)
        {
            ReportsManager manager = new ReportsManager(_context);
            WageProcessManager wageProcess = new WageProcessManager(_context);
            Wage_Process wage_Process = wageProcess.getWageProcessById(WAG_Id);
            DateTime wageMonth = wage_Process.WAG_Month;
            string WAG_Month = wageMonth.ToString("MMMM") + "-" + wageMonth.ToString("yyyy");

            string newPath = ProjectUtils.GetTempFolderPath(_hostingEnvironment.WebRootPath);
            string fileName = "CHEQUE_CASH_AdvanceReport_" + DateTime.Now.ToString("ddMMyyyyHHmm") + "_" + WAG_Month + ".xlsx";
            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, fileName);
            FileInfo file = new FileInfo(Path.Combine(newPath, fileName));
            var memory = new MemoryStream();
            using (var fs = new FileStream(Path.Combine(newPath, fileName), FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook;
                workbook = new XSSFWorkbook();
                ISheet excelSheet = workbook.CreateSheet("Template");

                // Style the cell with font color white
                IFont fontcell = workbook.CreateFont();
                fontcell.IsBold = true;

                IFont fontcellSub = workbook.CreateFont();
                fontcellSub.IsBold = true;
                fontcellSub.FontHeightInPoints = ((short)18);

                // Grey25Percent background
                ICellStyle style = workbook.CreateCellStyle();
                ICellStyle styleTotal = workbook.CreateCellStyle();
                ICellStyle styleClient = workbook.CreateCellStyle();
                ICellStyle styleSub = workbook.CreateCellStyle();

                styleTotal.WrapText = true;
                styleTotal.VerticalAlignment = VerticalAlignment.Center;
                styleTotal.SetFont(fontcell);
                styleClient.SetFont(fontcell);
                styleSub.SetFont(fontcellSub);
                //#77bf2a

                // Style the cell with borders all around.
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
                style.SetFont(fontcell);

                IRow row = excelSheet.CreateRow(0);

                ICell CellSub = row.CreateCell(0);
                CellSub.SetCellValue(wage_Process.FRM_.FRM_Name.ToUpper());
                CellSub.CellStyle = styleSub;
                CellUtil.SetAlignment(CellSub, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 5));

                IRow rowAdd1 = excelSheet.CreateRow(1);
                ICell CellAdd1 = rowAdd1.CreateCell(0);
                CellAdd1.SetCellValue(wage_Process.FRM_.FRM_Address1.ToUpper()+","+wage_Process.FRM_.FRM_Address2.ToUpper());
                CellUtil.SetAlignment(CellAdd1, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 5));

                IRow rowSubHeading = excelSheet.CreateRow(2);
                ICell CellSubHeading = rowSubHeading.CreateCell(0);
                CellSubHeading.SetCellValue("CHEQUE/CASH ADVANCE FOR THE MONTH OF " + WAG_Month);
                CellSubHeading.CellStyle = styleClient;
                CellUtil.SetAlignment(CellSubHeading, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 5));

                row = excelSheet.CreateRow(3);
                row.HeightInPoints = (float)(3.2 * excelSheet.DefaultRowHeightInPoints);
                ICell cell0 = row.CreateCell(0);
                cell0.SetCellValue("SR.NO");
                cell0.CellStyle = style;
                ICell cell1 = row.CreateCell(1);
                cell1.SetCellValue("NAME");
                excelSheet.SetColumnWidth(1, (int)((32 + 0.72) * 256));
                cell1.CellStyle = style;
                ICell cell2 = row.CreateCell(2);
                cell2.SetCellValue("LOCATION");
                excelSheet.SetColumnWidth(2, (int)((15 + 0.72) * 256));
                cell2.CellStyle = style;
                ICell cell3 = row.CreateCell(3);
                cell3.SetCellValue("SALARY");
                excelSheet.SetColumnWidth(3, (int)((15 + 0.72) * 256));
                cell3.CellStyle = style;
                ICell cell4 = row.CreateCell(4);
                cell4.SetCellValue("CHEQUE/CASH");
                excelSheet.SetColumnWidth(4, (int)((15 + 0.72) * 256));
                cell4.CellStyle = style;
                ICell cell5 = row.CreateCell(5);
                cell5.SetCellValue("SIGN");
                excelSheet.SetColumnWidth(5, (int)((15 + 0.72) * 256));
                cell5.CellStyle = style;

                List<BankReportVM> BankReportVMs = manager.ChequeCash_AdvancesReports(WAG_Id, FRM_Id);
                int rowCount = 4, srNo = 1;
                decimal TRANSACTION_AMOUNT = 0M;
                foreach (var item in BankReportVMs)
                {
                    row = excelSheet.CreateRow(rowCount);
                    row.HeightInPoints = (float)(1.2 * excelSheet.DefaultRowHeightInPoints);
                    row.CreateCell(0).SetCellValue(Convert.ToString(srNo));
                    row.CreateCell(1).SetCellValue(item.EMP_NAME);
                    row.CreateCell(2).SetCellValue(item.EMP_ADDRESS);
                    row.CreateCell(3).SetCellValue(Convert.ToString(item.EMP_TRANSACTION_AMOUNT));
                    row.CreateCell(4).SetCellValue("");
                    row.CreateCell(5).SetCellValue("");
                    TRANSACTION_AMOUNT = TRANSACTION_AMOUNT + item.EMP_TRANSACTION_AMOUNT;
                    rowCount++;
                    srNo++;
                }
                row = excelSheet.CreateRow(rowCount);
                row.HeightInPoints = (float)(1.3 * excelSheet.DefaultRowHeightInPoints);
                ICell cellTotal = row.CreateCell(0);
                cellTotal.SetCellValue("GRAND TOTAL");
                cellTotal.CellStyle = styleTotal;
                CellUtil.SetAlignment(cellTotal, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(rowCount, rowCount, 0, 2));

                ICell cellTotalAmt = row.CreateCell(3);
                cellTotalAmt.SetCellValue(Convert.ToString(TRANSACTION_AMOUNT));
                cellTotalAmt.CellStyle = styleTotal;
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

        #endregion

        #region PF Reports

        public ActionResult ClientsSelectionForPF(int WAG_Id, int FRM_Id)
        {            
            WageRegisterManager wageRegisterManager = new WageRegisterManager(_context);
            WageProcessManager wageProcessManager = new WageProcessManager(_context);
            ClientSelectionVM clientSelectionVM = new ClientSelectionVM();
            List<Clients> clients = wageRegisterManager.GetWageRegisters(WAG_Id).Where(m => m.WAG_.FRM_Id.Equals(FRM_Id)).Select(m => m.CLI_).Distinct().ToList();
            clientSelectionVM.selectionVMs = ClientSelectionMapper.mapMe(clients, WAG_Id);
            clientSelectionVM.Report = ((int)ProjectUtils.PF_REPORT_TYPE.Client_Wise_PF_Details_Excel).ToString();
            clientSelectionVM.FRM_Id = FRM_Id;
            ClientsManager clientsManager = new ClientsManager(_context);
            clientSelectionVM.TotalActiveClients =clientsManager.GetActiveClientOfMonthByFirmId(wageProcessManager.getWageProcessById(WAG_Id).WAG_Month, FRM_Id).Count();
            return View(clientSelectionVM);
        }

        public async Task<FileResult> PF_Reports(ClientSelectionVM clientSelectionVM)
        {
            string report = clientSelectionVM.Report;

            WageProcessManager wageProcess = new WageProcessManager(_context);
            
            Wage_Process wage_Process = wageProcess.getWageProcessById(clientSelectionVM.selectionVMs[0].WAG_Id);
            DateTime wageMonth = wage_Process.WAG_Month;

            string WAG_Month = wageMonth.ToString("MMMM") + "-" + wageMonth.ToString("yyyy");

            string newPath = ProjectUtils.GetTempFolderPath(_hostingEnvironment.WebRootPath);
            var memory = new MemoryStream();

            string fileName = "";
            FileInfo file;
            try
            {
                switch (clientSelectionVM.Report)
                {
                    case "0":
                        fileName = "Client_Wise_PF_Report_" + DateTime.Now.ToString("ddMMyyyyHHmm") + "_" + WAG_Month + ".xlsx";
                        file = new FileInfo(Path.Combine(newPath, fileName));
                        Client_Wise_PF_Details_Excel(wage_Process.FRM_,clientSelectionVM.selectionVMs.ToList(), clientSelectionVM.selectionVMs[0].WAG_Id, newPath, fileName, WAG_Month);
                        break;
                    case "1":
                        fileName = "PF_Employees_Pending_For_Registration_Report_" + DateTime.Now.ToString("ddMMyyyyHHmm") + "_" + WAG_Month + ".xlsx";
                        file = new FileInfo(Path.Combine(newPath, fileName));
                        Employees_Pending_For_Registration_Excel(wage_Process.FRM_,clientSelectionVM.selectionVMs.ToList(), clientSelectionVM.selectionVMs[0].WAG_Id, newPath, fileName, WAG_Month);
                        break;
                    case "2":
                        fileName = "Employee_PF_" + DateTime.Now.ToString("ddMMyyyyHHmm") + "_" + WAG_Month + ".xlsx";
                        file = new FileInfo(Path.Combine(newPath, fileName));
                        Employee_PF_Excel(wage_Process.FRM_,clientSelectionVM.selectionVMs.ToList(), clientSelectionVM.selectionVMs[0].WAG_Id, newPath, fileName, WAG_Month);
                        break;
                    case "3":
                        fileName = "Employee_PF_Above58_" + DateTime.Now.ToString("ddMMyyyyHHmm") + "_" + WAG_Month + ".xlsx";
                        file = new FileInfo(Path.Combine(newPath, fileName));
                        Client_Wise_PF_Above58_Excel(wage_Process.FRM_,clientSelectionVM.selectionVMs.ToList(), clientSelectionVM.selectionVMs[0].WAG_Id, newPath, fileName, WAG_Month);
                        break;
                    case "4":
                        fileName = "Employee_PF_" + DateTime.Now.ToString("ddMMyyyyHHmm") + "_" + WAG_Month + ".txt";
                        file = new FileInfo(Path.Combine(newPath, fileName));
                        Employee_PF_Text(wage_Process.FRM_,clientSelectionVM.selectionVMs.ToList(), clientSelectionVM.selectionVMs[0].WAG_Id, newPath, fileName, WAG_Month);
                        break;
                    default: break;
                }

                using (var stream = new FileStream(Path.Combine(newPath, fileName), FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                new FileInfo(Path.Combine(newPath, fileName)).Delete();

            }
            catch (Exception)
            {
                TempData["message"] = "Try Again";
            }
            if (clientSelectionVM.Report == ((int)ProjectUtils.PF_REPORT_TYPE.PF_Report_Text).ToString())
            {
                return File(memory, "application/rtf", fileName);
            }
            else
            {
                return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }

        public void Client_Wise_PF_Details_Excel(Firms firm, List<SelectionVM> selectionVMs, int WAG_Id, string newPath, string fileName, string WAG_Month)
        {
            bool IsSelected = selectionVMs.Any(m => m.IsSelect.Equals(false));
            ReportsManager manager = new ReportsManager(_context);
            using (var fs = new FileStream(Path.Combine(newPath, fileName), FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook;
                workbook = new XSSFWorkbook();
                #region single client
                ISheet excelSheet = workbook.CreateSheet("PF CON");

                IFont fontcell = workbook.CreateFont();
                fontcell.IsBold = true;

                IFont fontcellSub = workbook.CreateFont();
                fontcellSub.IsBold = true;
                fontcellSub.FontHeightInPoints = ((short)18);

                ICellStyle style = workbook.CreateCellStyle();
                ICellStyle styleTotal = workbook.CreateCellStyle();
                ICellStyle styleClient = workbook.CreateCellStyle();
                ICellStyle styleSub = workbook.CreateCellStyle();

                styleTotal.SetFont(fontcell);
                styleClient.SetFont(fontcell);
                styleSub.SetFont(fontcellSub);

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

                IRow row = excelSheet.CreateRow(0);
                ICell CellSub = row.CreateCell(0);
                CellSub.SetCellValue(firm.FRM_Name.ToUpper());
                CellSub.CellStyle = styleSub;
                CellUtil.SetAlignment(CellSub, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 6));

                IRow rowAdd1 = excelSheet.CreateRow(1);
                ICell CellAdd1 = rowAdd1.CreateCell(0);
                CellAdd1.SetCellValue(firm.FRM_Address1.ToUpper()+","+ firm.FRM_Address2.ToUpper());
                CellUtil.SetAlignment(CellAdd1, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 6));

                IRow rowSubHeading = excelSheet.CreateRow(2);
                ICell CellSubHeading = rowSubHeading.CreateCell(0);
                CellSubHeading.SetCellValue("LIST OF PF CONTRIBUTION FOR THE MONTH OF " + WAG_Month);
                CellSubHeading.CellStyle = styleClient;
                CellUtil.SetAlignment(CellSubHeading, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 6));

                row = excelSheet.CreateRow(3);
                row.HeightInPoints = (float)(3.2 * excelSheet.DefaultRowHeightInPoints);
                ICell cell0 = row.CreateCell(0);
                cell0.SetCellValue("NAME OF COMPANY");
                excelSheet.SetColumnWidth(0, (int)((35 + 0.72) * 256));
                cell0.CellStyle = style;
                ICell cell1 = row.CreateCell(1);
                cell1.SetCellValue("STRENGTH");
                excelSheet.SetColumnWidth(1, (int)((22 + 0.72) * 256));
                cell1.CellStyle = style;
                ICell cell2 = row.CreateCell(2);
                cell2.SetCellValue("PF APPLICABLE \r\n  SALARY / \r\n  BASIC+DA");
                excelSheet.SetColumnWidth(2, (int)((15 + 0.72) * 256));
                cell2.CellStyle = style;
                ICell cell3 = row.CreateCell(3);
                cell3.SetCellValue("EMPLOYEE CONT.");
                excelSheet.SetColumnWidth(3, (int)((15 + 0.72) * 256));
                cell3.CellStyle = style;
                ICell cell4 = row.CreateCell(4);
                cell4.SetCellValue("EMPLOYER CONT.");
                excelSheet.SetColumnWidth(4, (int)((15 + 0.72) * 256));
                cell4.CellStyle = style;
                ICell cell5 = row.CreateCell(5);
                cell5.SetCellValue("TOTAL CONT.");
                excelSheet.SetColumnWidth(5, (int)((15 + 0.72) * 256));
                cell5.CellStyle = style;
                ICell cell6 = row.CreateCell(6);
                cell6.SetCellValue("REMARKS");
                excelSheet.SetColumnWidth(6, (int)((15 + 0.72) * 256));
                cell6.CellStyle = style;

                List<PFClientReportVM> reportVM = manager.Client_Wise_PF_Details_Excel(WAG_Id, selectionVMs.Where(m => m.IsSelect.Equals(true)).ToList(), IsSelected);
                int rowCount = 4, srNo = 1;
                decimal TOT_STRENGTH = 0M, TOT_APPLICABLE_SALARY = 0M, TOT_EMPLOYEE_CONT = 0M, TOT_EMPLOYER_CONT = 0M, TOT_CONT = 0M;
                foreach (var item in reportVM)
                {
                    row = excelSheet.CreateRow(rowCount);
                    row.HeightInPoints = (float)(1.2 * excelSheet.DefaultRowHeightInPoints);
                    row.CreateCell(0).SetCellValue(item.COMPANY_NAME);
                    row.CreateCell(1).SetCellValue(Convert.ToString(item.STRENGTH));
                    row.CreateCell(2).SetCellValue(Convert.ToString(item.PF_APPLICABLE_SALARY));
                    row.CreateCell(3).SetCellValue(Convert.ToString(item.EMPLOYEE_CONTRIBUTION));
                    row.CreateCell(4).SetCellValue(Convert.ToString(item.EMPLOYER_CONTRIBUTION));
                    row.CreateCell(5).SetCellValue(Convert.ToString(item.TOTAL_CONTRIBUTION));
                    row.CreateCell(6).SetCellValue(item.REMARKS);

                    TOT_STRENGTH = TOT_STRENGTH + item.STRENGTH;
                    TOT_APPLICABLE_SALARY = TOT_APPLICABLE_SALARY + item.PF_APPLICABLE_SALARY;
                    TOT_EMPLOYEE_CONT = TOT_EMPLOYEE_CONT + item.EMPLOYEE_CONTRIBUTION;
                    TOT_EMPLOYER_CONT = TOT_EMPLOYER_CONT + item.EMPLOYER_CONTRIBUTION;
                    TOT_CONT = TOT_CONT + item.TOTAL_CONTRIBUTION;
                    rowCount++;
                    srNo++;
                }
                row = excelSheet.CreateRow(rowCount);
                row.HeightInPoints = (float)(1.3 * excelSheet.DefaultRowHeightInPoints);
                ICell cellTotal = row.CreateCell(0);
                cellTotal.SetCellValue("TOTAL");
                cellTotal.CellStyle = styleTotal;
                CellUtil.SetAlignment(cellTotal, workbook, (short)HorizontalAlignment.Center);

                ICell cellTOT_STRENGTH = row.CreateCell(1);
                cellTOT_STRENGTH.SetCellValue(Convert.ToString(TOT_STRENGTH));
                cellTOT_STRENGTH.CellStyle = styleTotal;

                ICell cellTOT_APPLICABLE_SALARY = row.CreateCell(2);
                cellTOT_APPLICABLE_SALARY.SetCellValue(Convert.ToString(TOT_APPLICABLE_SALARY));
                cellTOT_APPLICABLE_SALARY.CellStyle = styleTotal;

                ICell cellTOT_EMPLOYEE_CONT = row.CreateCell(3);
                cellTOT_EMPLOYEE_CONT.SetCellValue(Convert.ToString(TOT_EMPLOYEE_CONT));
                cellTOT_EMPLOYEE_CONT.CellStyle = styleTotal;

                ICell cellTOT_EMPLOYER_CONT = row.CreateCell(4);
                cellTOT_EMPLOYER_CONT.SetCellValue(Convert.ToString(TOT_EMPLOYER_CONT));
                cellTOT_EMPLOYER_CONT.CellStyle = styleTotal;

                ICell cellTOT_CONT = row.CreateCell(5);
                cellTOT_CONT.SetCellValue(Convert.ToString(TOT_CONT));
                cellTOT_CONT.CellStyle = styleTotal;
                #endregion
                workbook.Write(fs);
            }
        }

        public void Employees_Pending_For_Registration_Excel(Firms firm, List<SelectionVM> selectionVMs, int WAG_Id, string newPath, string fileName, string WAG_Month)
        {
            bool IsSelected = selectionVMs.Any(m => m.IsSelect.Equals(false));
            ReportsManager manager = new ReportsManager(_context);
            using (var fs = new FileStream(Path.Combine(newPath, fileName), FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook;
                workbook = new XSSFWorkbook();
                ISheet excelSheet = workbook.CreateSheet("P.F. CON");
                #region single client
                IFont fontcell = workbook.CreateFont();
                fontcell.IsBold = true;
                IFont fontcellSub = workbook.CreateFont();
                fontcellSub.IsBold = true;
                fontcellSub.FontHeightInPoints = ((short)18);

                ICellStyle style = workbook.CreateCellStyle();
                ICellStyle styleClient = workbook.CreateCellStyle();
                ICellStyle styleSub = workbook.CreateCellStyle();


                styleClient.SetFont(fontcell);
                styleSub.SetFont(fontcellSub);

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

                IRow row = excelSheet.CreateRow(0);
                ICell CellSub = row.CreateCell(0);
                CellSub.SetCellValue(firm.FRM_Name.ToUpper());
                CellSub.CellStyle = styleSub;
                CellUtil.SetAlignment(CellSub, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 4));

                IRow rowAdd1 = excelSheet.CreateRow(1);
                ICell CellAdd1 = rowAdd1.CreateCell(0);
                CellAdd1.SetCellValue(firm.FRM_Address1+","+ firm.FRM_Address2);
                CellUtil.SetAlignment(CellAdd1, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 4));

                IRow rowSubHeading = excelSheet.CreateRow(2);
                ICell CellSubHeading = rowSubHeading.CreateCell(0);
                CellSubHeading.SetCellValue("LIST OF P.F. EMPLOYEES PENDING FOR REGISTRATION THE MONTH OF " + WAG_Month);
                CellSubHeading.CellStyle = styleClient;
                CellUtil.SetAlignment(CellSubHeading, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 4));

                row = excelSheet.CreateRow(3);
                row.HeightInPoints = (float)(3.2 * excelSheet.DefaultRowHeightInPoints);
                ICell cell0 = row.CreateCell(0);
                cell0.SetCellValue("SR.NO");
                cell0.CellStyle = style;
                ICell cell1 = row.CreateCell(1);
                cell1.SetCellValue("NAME OF COMPANY");
                excelSheet.SetColumnWidth(1, (int)((35 + 0.72) * 256));
                cell1.CellStyle = style;
                ICell cell2 = row.CreateCell(2);
                cell2.SetCellValue("NAME OF EMPLOYEE");
                excelSheet.SetColumnWidth(2, (int)((35 + 0.72) * 256));
                cell2.CellStyle = style;
                ICell cell3 = row.CreateCell(3);
                cell3.SetCellValue("PENDING \r\nREGISTRAION \r\nSINCE");
                excelSheet.SetColumnWidth(3, (int)((30 + 0.72) * 256));
                cell3.CellStyle = style;
                ICell cell4 = row.CreateCell(4);
                cell4.SetCellValue("REMARK FOR \r\nPENING UAN \r\nREGISTRATION");
                excelSheet.SetColumnWidth(4, (int)((30 + 0.72) * 256));
                cell4.CellStyle = style;

                List<PFClientReportVM> reportVM = manager.Employees_Pending_For_Registration(WAG_Id, selectionVMs.Where(m => m.IsSelect.Equals(true)).ToList(), IsSelected);
                int rowCount = 4, srNo = 1;
                foreach (var item in reportVM)
                {
                    row = excelSheet.CreateRow(rowCount);
                    row.HeightInPoints = (float)(1.4 * excelSheet.DefaultRowHeightInPoints);
                    row.CreateCell(0).SetCellValue(Convert.ToString(srNo));
                    row.CreateCell(1).SetCellValue(Convert.ToString(item.COMPANY_NAME));
                    row.CreateCell(2).SetCellValue(Convert.ToString(item.EMP_FullName));
                    row.CreateCell(3).SetCellValue(Convert.ToString(item.PENDING_REGISTRAION_SINCE));
                    row.CreateCell(4).SetCellValue(item.REMARKS);
                    rowCount++;
                    srNo++;
                }
                #endregion

                workbook.Write(fs);
            }

        }

        public void Employee_PF_Excel(Firms firm, List<SelectionVM> selectionVMs, int WAG_Id, string newPath, string fileName, string WAG_Month)
        {
            bool IsSelected = selectionVMs.Any(m => m.IsSelect.Equals(false));
            ReportsManager manager = new ReportsManager(_context);
            using (var fs = new FileStream(Path.Combine(newPath, fileName), FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook;
                workbook = new XSSFWorkbook();

                ISheet excelSheet = workbook.CreateSheet("P.F Chalan");

                #region single client
                IFont fontcell = workbook.CreateFont();
                fontcell.IsBold = true;
                IFont fontcellSub = workbook.CreateFont();
                fontcellSub.IsBold = true;
                fontcellSub.FontHeightInPoints = ((short)18);

                ICellStyle style = workbook.CreateCellStyle();
                ICellStyle styleClient = workbook.CreateCellStyle();
                ICellStyle styleSub = workbook.CreateCellStyle();

                styleClient.SetFont(fontcell);
                styleSub.SetFont(fontcellSub);

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

                IRow row = excelSheet.CreateRow(0);
                row.HeightInPoints = (float)(3.2 * excelSheet.DefaultRowHeightInPoints);
                ICell cell0 = row.CreateCell(0);
                cell0.SetCellValue("UAN.NO");
                excelSheet.SetColumnWidth(0, (int)((25 + 0.72) * 256));
                cell0.CellStyle = style;
                ICell cell1 = row.CreateCell(1);
                cell1.SetCellValue("NAME OF EMPLOYEE");
                excelSheet.SetColumnWidth(1, (int)((35 + 0.72) * 256));
                cell1.CellStyle = style;
                ICell cell2 = row.CreateCell(2);
                cell2.SetCellValue("PF APLICABLE SALARY");
                excelSheet.SetColumnWidth(2, (int)((15 + 0.72) * 256));
                cell2.CellStyle = style;
                ICell cell3 = row.CreateCell(3);
                cell3.SetCellValue("PF APLICABLE SALARY");
                excelSheet.SetColumnWidth(3, (int)((15 + 0.72) * 256));
                cell3.CellStyle = style;
                ICell cell4 = row.CreateCell(4);
                cell4.SetCellValue("PF APLICABLE SALARY");
                excelSheet.SetColumnWidth(4, (int)((15 + 0.72) * 256));
                cell4.CellStyle = style;
                ICell cell5 = row.CreateCell(5);
                cell5.SetCellValue("PF APLICABLE SALARY");
                excelSheet.SetColumnWidth(5, (int)((15 + 0.72) * 256));
                cell5.CellStyle = style;
                ICell cell6 = row.CreateCell(6);
                cell6.SetCellValue("EPF CONTRIBUTION");
                excelSheet.SetColumnWidth(6, (int)((15 + 0.72) * 256));
                cell6.CellStyle = style;
                ICell cell7 = row.CreateCell(7);
                cell7.SetCellValue("EPS CONTRIBUTION");
                excelSheet.SetColumnWidth(7, (int)((15 + 0.72) * 256));
                cell7.CellStyle = style;
                ICell cell8 = row.CreateCell(8);
                cell8.SetCellValue("EPF-EPS CONTRIBUTION");
                excelSheet.SetColumnWidth(8, (int)((15 + 0.72) * 256));
                cell8.CellStyle = style;
                ICell cell9 = row.CreateCell(9);
                cell9.SetCellValue("NCP-1");
                cell9.CellStyle = style;
                ICell cell10 = row.CreateCell(10);
                cell10.SetCellValue("NCP-2");
                cell10.CellStyle = style;

                List<PFClientReportVM> reportVM = manager.Employees_PF_Excel(WAG_Id, selectionVMs.Where(m => m.IsSelect.Equals(true)).ToList(), IsSelected);
                int rowCount = 1, srNo = 1;
                decimal tot_PF_APPLICABLE_SALARY = 0M, tot_EPF_CONTRIBUTION = 0M, tot_EPS_CONTRIBUTION = 0M, tot_DIFF_EPF_EPS = 0M, tot_NCP1 = 0M, tot_NCP2 = 0M;

                var result = reportVM
                           .GroupBy(x => new { x.EMP_Id, x.UAN_Number, x.EMP_FullName })
                           .Select(g => new
                           {
                               UAN_Number = g.Key.UAN_Number,
                               EMP_FullName = g.Key.EMP_FullName,
                               PF_APPLICABLE_SALARY = g.Sum(x => x.PF_APPLICABLE_SALARY),
                               EPF_CONTRIBUTION = g.Sum(x => x.EPF_CONTRIBUTION),
                               EPS_CONTRIBUTION = g.Sum(x => x.EPS_CONTRIBUTION),                               
                               NCP1 = g.Sum(x => x.NCP1),
                               NCP2 = g.Sum(x => x.NCP2)
                           });

                foreach (var item in result)
                {
                    decimal PF_APPLICABLE_SALARY = Math.Round(item.PF_APPLICABLE_SALARY, MidpointRounding.AwayFromZero);
                    decimal EPF_CONTRIBUTION = Math.Round(item.EPF_CONTRIBUTION, MidpointRounding.AwayFromZero);
                    decimal EPS_CONTRIBUTION = Math.Round(item.EPS_CONTRIBUTION, MidpointRounding.AwayFromZero);
                    decimal DIFF_EPF_EPS = Math.Round(EPF_CONTRIBUTION- EPS_CONTRIBUTION);
                    row = excelSheet.CreateRow(rowCount);
                    row.HeightInPoints = (float)(1.4 * excelSheet.DefaultRowHeightInPoints);
                    row.CreateCell(0).SetCellValue(Convert.ToString(item.UAN_Number));
                    row.CreateCell(1).SetCellValue(Convert.ToString(item.EMP_FullName));
                    row.CreateCell(2).SetCellValue(Convert.ToString(PF_APPLICABLE_SALARY));
                    row.CreateCell(3).SetCellValue(Convert.ToString(PF_APPLICABLE_SALARY));
                    row.CreateCell(4).SetCellValue(Convert.ToString(PF_APPLICABLE_SALARY));
                    row.CreateCell(5).SetCellValue(Convert.ToString(PF_APPLICABLE_SALARY));
                    row.CreateCell(6).SetCellValue(Convert.ToString(EPF_CONTRIBUTION));
                    row.CreateCell(7).SetCellValue(Convert.ToString(EPS_CONTRIBUTION));
                    row.CreateCell(8).SetCellValue(Convert.ToString(DIFF_EPF_EPS));
                    row.CreateCell(9).SetCellValue(Convert.ToString(item.NCP1));
                    row.CreateCell(10).SetCellValue(Convert.ToString(item.NCP2));

                    tot_PF_APPLICABLE_SALARY = tot_PF_APPLICABLE_SALARY + PF_APPLICABLE_SALARY;
                    tot_EPF_CONTRIBUTION = tot_EPF_CONTRIBUTION + EPF_CONTRIBUTION;
                    tot_EPS_CONTRIBUTION = tot_EPS_CONTRIBUTION + EPS_CONTRIBUTION;
                    tot_DIFF_EPF_EPS = tot_DIFF_EPF_EPS + DIFF_EPF_EPS;
                    tot_NCP1 = tot_NCP1 + item.NCP1;
                    tot_NCP2 = tot_NCP2 + item.NCP2;

                    rowCount++;
                    srNo++;
                }
                row = excelSheet.CreateRow(rowCount);
                row.HeightInPoints = (float)(1.3 * excelSheet.DefaultRowHeightInPoints);

                ICell cell_Total = row.CreateCell(0);
                cell_Total.SetCellValue(Convert.ToString("TOTAL"));
                cell_Total.CellStyle = styleClient;
                CellUtil.SetAlignment(cell_Total, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(rowCount, rowCount, 0, 1));

                ICell cellSALARY1 = row.CreateCell(2);
                cellSALARY1.SetCellValue(Convert.ToString(tot_PF_APPLICABLE_SALARY));
                cellSALARY1.CellStyle = styleClient;
                CellUtil.SetAlignment(cellSALARY1, workbook, (short)HorizontalAlignment.Center);

                ICell cellSALARY2 = row.CreateCell(3);
                cellSALARY2.SetCellValue(Convert.ToString(tot_PF_APPLICABLE_SALARY));
                cellSALARY2.CellStyle = styleClient;

                ICell cellSALARY3 = row.CreateCell(4);
                cellSALARY3.SetCellValue(Convert.ToString(tot_PF_APPLICABLE_SALARY));
                cellSALARY3.CellStyle = styleClient;

                ICell cellSALARY4 = row.CreateCell(5);
                cellSALARY4.SetCellValue(Convert.ToString(tot_PF_APPLICABLE_SALARY));
                cellSALARY4.CellStyle = styleClient;

                ICell cell_EPF = row.CreateCell(6);
                cell_EPF.SetCellValue(Convert.ToString(tot_EPF_CONTRIBUTION));
                cell_EPF.CellStyle = styleClient;

                ICell cell_EPS = row.CreateCell(7);
                cell_EPS.SetCellValue(Convert.ToString(tot_EPS_CONTRIBUTION));
                cell_EPS.CellStyle = styleClient;

                ICell cell_EPF_EPS = row.CreateCell(8);
                cell_EPF_EPS.SetCellValue(Convert.ToString(tot_DIFF_EPF_EPS));
                cell_EPF_EPS.CellStyle = styleClient;

                ICell cell_NCP1 = row.CreateCell(9);
                cell_NCP1.SetCellValue(Convert.ToString(tot_NCP1));
                cell_NCP1.CellStyle = styleClient;

                ICell cell_NCP2 = row.CreateCell(10);
                cell_NCP2.SetCellValue(Convert.ToString(tot_NCP2));
                cell_NCP2.CellStyle = styleClient;
                #endregion

                workbook.Write(fs);
            }
        }

        public void Client_Wise_PF_Above58_Excel(Firms firm, List<SelectionVM> selectionVMs, int WAG_Id, string newPath, string fileName, string WAG_Month)
        {
            bool IsSelected = selectionVMs.Any(m => m.IsSelect.Equals(false));
            ReportsManager manager = new ReportsManager(_context);
            using (var fs = new FileStream(Path.Combine(newPath, fileName), FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook;
                workbook = new XSSFWorkbook();
                #region single client
                ISheet excelSheet = workbook.CreateSheet("P.F. CON");

                IFont fontcell = workbook.CreateFont();
                fontcell.IsBold = true;

                IFont fontcellSub = workbook.CreateFont();
                fontcellSub.IsBold = true;
                fontcellSub.FontHeightInPoints = ((short)18);

                ICellStyle style = workbook.CreateCellStyle();
                ICellStyle styleTotal = workbook.CreateCellStyle();
                ICellStyle styleClient = workbook.CreateCellStyle();
                ICellStyle styleSub = workbook.CreateCellStyle();

                styleTotal.SetFont(fontcell);
                styleClient.SetFont(fontcell);
                styleSub.SetFont(fontcellSub);

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

                IRow row = excelSheet.CreateRow(0);
                ICell CellSub = row.CreateCell(0);
                CellSub.SetCellValue(firm.FRM_Name.ToUpper());
                CellSub.CellStyle = styleSub;
                CellUtil.SetAlignment(CellSub, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 6));

                IRow rowAdd1 = excelSheet.CreateRow(1);
                ICell CellAdd1 = rowAdd1.CreateCell(0);
                CellAdd1.SetCellValue(firm.FRM_Address1.ToUpper()+","+ firm.FRM_Address2.ToUpper());
                CellUtil.SetAlignment(CellAdd1, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 6));

                IRow rowSubHeading = excelSheet.CreateRow(2);
                ICell CellSubHeading = rowSubHeading.CreateCell(0);
                CellSubHeading.SetCellValue("LIST OF EMPLOYEES ABOVE 58 AGE P.F. CONTRIBUTION OF EMPLOYEES FOR THE MONTH OF " + WAG_Month);
                CellSubHeading.CellStyle = styleClient;
                CellUtil.SetAlignment(CellSubHeading, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 6));

                row = excelSheet.CreateRow(3);
                row.HeightInPoints = (float)(3.2 * excelSheet.DefaultRowHeightInPoints);
                ICell cell0 = row.CreateCell(0);
                cell0.SetCellValue("NAME OF COMPANY");
                excelSheet.SetColumnWidth(0, (int)((35 + 0.72) * 256));
                cell0.CellStyle = style;
                ICell cell1 = row.CreateCell(1);
                cell1.SetCellValue("NAME OF EMPLOYEE");
                excelSheet.SetColumnWidth(1, (int)((35 + 0.72) * 256));
                cell1.CellStyle = style;
                ICell cell2 = row.CreateCell(2);
                cell2.SetCellValue("STRENGTH");
                excelSheet.SetColumnWidth(2, (int)((15 + 0.72) * 256));
                cell2.CellStyle = style;
                ICell cell3 = row.CreateCell(3);
                cell3.SetCellValue("PF APPLICABLE \r\n  SALARY / \r\n  BASIC+DA");
                excelSheet.SetColumnWidth(3, (int)((15 + 0.72) * 256));
                cell3.CellStyle = style;
                ICell cell4 = row.CreateCell(4);
                cell4.SetCellValue("EMPLOYEE CONT.");
                excelSheet.SetColumnWidth(4, (int)((15 + 0.72) * 256));
                cell4.CellStyle = style;
                ICell cell5 = row.CreateCell(5);
                cell5.SetCellValue("EMPLOYER CONT.");
                excelSheet.SetColumnWidth(5, (int)((15 + 0.72) * 256));
                cell5.CellStyle = style;
                ICell cell6 = row.CreateCell(6);
                cell6.SetCellValue("TOTAL CONT.");
                excelSheet.SetColumnWidth(6, (int)((15 + 0.72) * 256));
                cell6.CellStyle = style;
                ICell cell7 = row.CreateCell(7);
                cell7.SetCellValue("REMARKS");
                excelSheet.SetColumnWidth(7, (int)((30 + 0.72) * 256));
                cell7.CellStyle = style;
                List<PFClientReportVM> reportVM = manager.Client_Wise_PF_Above58_Excel(WAG_Id, selectionVMs.Where(m => m.IsSelect.Equals(true)).ToList(), IsSelected);
                int rowCount = 4, srNo = 1;
                decimal TOT_STRENGTH = 0M, TOT_APPLICABLE_SALARY = 0M, TOT_EMPLOYEE_CONT = 0M, TOT_EMPLOYER_CONT = 0M, TOT_CONT = 0M;
                foreach (var item in reportVM)
                {
                    row = excelSheet.CreateRow(rowCount);
                    row.HeightInPoints = (float)(1.2 * excelSheet.DefaultRowHeightInPoints);
                    row.CreateCell(0).SetCellValue(item.COMPANY_NAME);
                    row.CreateCell(1).SetCellValue(item.EMP_FullName);
                    row.CreateCell(2).SetCellValue(Convert.ToString(item.STRENGTH));
                    row.CreateCell(3).SetCellValue(Convert.ToString(item.PF_APPLICABLE_SALARY));
                    row.CreateCell(4).SetCellValue(Convert.ToString(item.EMPLOYEE_CONTRIBUTION));
                    row.CreateCell(5).SetCellValue(Convert.ToString(item.EMPLOYER_CONTRIBUTION));
                    row.CreateCell(6).SetCellValue(Convert.ToString(item.TOTAL_CONTRIBUTION));
                    row.CreateCell(7).SetCellValue(item.REMARKS);

                    TOT_STRENGTH = TOT_STRENGTH + item.STRENGTH;
                    TOT_APPLICABLE_SALARY = TOT_APPLICABLE_SALARY + item.PF_APPLICABLE_SALARY;
                    TOT_EMPLOYEE_CONT = TOT_EMPLOYEE_CONT + item.EMPLOYEE_CONTRIBUTION;
                    TOT_EMPLOYER_CONT = TOT_EMPLOYER_CONT + item.EMPLOYER_CONTRIBUTION;
                    TOT_CONT = TOT_CONT + item.TOTAL_CONTRIBUTION;
                    rowCount++;
                    srNo++;
                }
                row = excelSheet.CreateRow(rowCount);
                row.HeightInPoints = (float)(1.3 * excelSheet.DefaultRowHeightInPoints);
                ICell cellTotal = row.CreateCell(0);
                cellTotal.SetCellValue("TOTAL");
                cellTotal.CellStyle = styleTotal;
                CellUtil.SetAlignment(cellTotal, workbook, (short)HorizontalAlignment.Center);

                ICell cellTOT_STRENGTH = row.CreateCell(2);
                cellTOT_STRENGTH.SetCellValue(Convert.ToString(TOT_STRENGTH));
                cellTOT_STRENGTH.CellStyle = styleTotal;

                ICell cellTOT_APPLICABLE_SALARY = row.CreateCell(3);
                cellTOT_APPLICABLE_SALARY.SetCellValue(Convert.ToString(TOT_APPLICABLE_SALARY));
                cellTOT_APPLICABLE_SALARY.CellStyle = styleTotal;

                ICell cellTOT_EMPLOYEE_CONT = row.CreateCell(4);
                cellTOT_EMPLOYEE_CONT.SetCellValue(Convert.ToString(TOT_EMPLOYEE_CONT));
                cellTOT_EMPLOYEE_CONT.CellStyle = styleTotal;

                ICell cellTOT_EMPLOYER_CONT = row.CreateCell(5);
                cellTOT_EMPLOYER_CONT.SetCellValue(Convert.ToString(TOT_EMPLOYER_CONT));
                cellTOT_EMPLOYER_CONT.CellStyle = styleTotal;

                ICell cellTOT_CONT = row.CreateCell(6);
                cellTOT_CONT.SetCellValue(Convert.ToString(TOT_CONT));
                cellTOT_CONT.CellStyle = styleTotal;
                #endregion
                workbook.Write(fs);
            }
        }

        public void Employee_PF_Text(Firms firm,List<SelectionVM> selectionVMs, int WAG_Id, string newPath, string fileName, string WAG_Month)
        {
            bool IsSelected = selectionVMs.Any(m => m.IsSelect.Equals(false));
            ReportsManager manager = new ReportsManager(_context);
            FileInfo file = new FileInfo(Path.Combine(newPath, fileName));
            var memory = new MemoryStream();
            using (var fs = new FileStream(Path.Combine(newPath, fileName), FileMode.Create, FileAccess.Write))
            {
                List<PFClientReportVM> reportVM = manager.Employees_PF_Excel(WAG_Id, selectionVMs.Where(m => m.IsSelect.Equals(true)).ToList(), IsSelected);
                int rowCount = 2;
                var result = reportVM
                          .GroupBy(x => new { x.EMP_Id, x.UAN_Number, x.EMP_FullName })
                          .Select(g => new
                          {
                              UAN_Number = g.Key.UAN_Number,
                              EMP_FullName = g.Key.EMP_FullName,
                              PF_APPLICABLE_SALARY = g.Sum(x => x.PF_APPLICABLE_SALARY),
                              EPF_CONTRIBUTION = g.Sum(x => x.EPF_CONTRIBUTION),
                              EPS_CONTRIBUTION = g.Sum(x => x.EPS_CONTRIBUTION),                              
                              NCP1 = g.Sum(x => x.NCP1),
                              NCP2 = g.Sum(x => x.NCP2)
                          });
                foreach (var item in result)
                {
                    decimal PF_APPLICABLE_SALARY = Math.Round(item.PF_APPLICABLE_SALARY, MidpointRounding.AwayFromZero);
                    decimal EPF_CONTRIBUTION = Math.Round(item.EPF_CONTRIBUTION, MidpointRounding.AwayFromZero);
                    decimal EPS_CONTRIBUTION = Math.Round(item.EPS_CONTRIBUTION, MidpointRounding.AwayFromZero);
                    decimal DIFF_EPF_EPS = Math.Round(EPF_CONTRIBUTION- EPS_CONTRIBUTION);

                    Byte[] title = new UTF8Encoding(true).GetBytes(item.UAN_Number + "#~#" + item.EMP_FullName + "#~#" + PF_APPLICABLE_SALARY + "#~#" + PF_APPLICABLE_SALARY + "#~#" + PF_APPLICABLE_SALARY + "#~#" + PF_APPLICABLE_SALARY + "#~#");
                    fs.Write(title, 0, title.Length);
                    byte[] br = new UTF8Encoding(true).GetBytes(EPF_CONTRIBUTION + "#~#" + EPS_CONTRIBUTION + "#~#" + DIFF_EPF_EPS + "#~#");
                    fs.Write(br, 0, br.Length);
                    byte[] author = new UTF8Encoding(true).GetBytes(item.NCP1 + "#~#" + item.NCP1);
                    fs.Write(author, 0, author.Length);
                    byte[] br2 = new UTF8Encoding(true).GetBytes("\r\n");
                    fs.Write(br2, 0, br2.Length);
                    rowCount++;
                }
            }

        }

        #endregion

        #region Bank reports

        public ActionResult ClientsSelectionForBank(int WAG_Id, int FRM_Id)
        {
            WageRegisterManager wageRegisterManager = new WageRegisterManager(_context);
            WageProcessManager wageProcessManager = new WageProcessManager(_context);
            ClientSelectionVM clientSelectionVM = new ClientSelectionVM();
            List<Clients> clients = wageRegisterManager.GetWageRegisters(WAG_Id).Where(m => m.WAG_.FRM_Id.Equals(FRM_Id)).Select(m => m.CLI_).Distinct().ToList();
            clientSelectionVM.selectionVMs = ClientSelectionMapper.mapMe(clients, WAG_Id);
            clientSelectionVM.Report = ((int)ProjectUtils.PF_REPORT_TYPE.Client_Wise_PF_Details_Excel).ToString();
            clientSelectionVM.FRM_Id = FRM_Id;
            ClientsManager clientsManager = new ClientsManager(_context);
            clientSelectionVM.TotalActiveClients = clientsManager.GetActiveClientOfMonthByFirmId(wageProcessManager.getWageProcessById(WAG_Id).WAG_Month, FRM_Id).Count();
            return View(clientSelectionVM);
        }

        public async Task<FileResult> Bank_Reports(ClientSelectionVM clientSelectionVM)
        {
            string report = clientSelectionVM.Report;

            WageProcessManager wageProcess = new WageProcessManager(_context);           
            Wage_Process wage_Process = wageProcess.getWageProcessById(clientSelectionVM.selectionVMs[0].WAG_Id);
            DateTime wageMonth = wage_Process.WAG_Month;
            string WAG_Month = wageMonth.ToString("MMMM") + "-" + wageMonth.ToString("yyyy");

            string newPath = ProjectUtils.GetTempFolderPath(_hostingEnvironment.WebRootPath);
            var memory = new MemoryStream();

            string fileName = "";
            FileInfo file;
            try
            {
                switch (clientSelectionVM.Report)
                {
                    case "0": //Company_Wise_Transfer_Report
                        fileName = "NEFT_BankReport_" + DateTime.Now.ToString("ddMMyyyyHHmm") + "_" + WAG_Month + ".xlsx";
                        file = new FileInfo(Path.Combine(newPath, fileName));
                        NEFT_BankReportExcel(wage_Process.FRM_, clientSelectionVM.selectionVMs.ToList(), clientSelectionVM.selectionVMs[0].WAG_Id, newPath, fileName, WAG_Month);
                        break;
                    case "1": //IDBI_Bank_To_IDBI_Bank_Report
                        fileName = "IDBI_To_IDBI_BankReport_" + DateTime.Now.ToString("ddMMyyyyHHmm") + "_" + WAG_Month + ".xlsx";
                        file = new FileInfo(Path.Combine(newPath, fileName));
                        IDBI_To_IDBI_BankReportExcel(wage_Process.FRM_, clientSelectionVM.selectionVMs.ToList(), clientSelectionVM.selectionVMs[0].WAG_Id, newPath, fileName, WAG_Month);
                        break;
                    case "2": //IDBI_Bank_To_Others_Report
                        fileName = "IDBI_To_Other_BankReport_" + DateTime.Now.ToString("ddMMyyyyHHmm") + "_" + WAG_Month + ".xlsx";
                        file = new FileInfo(Path.Combine(newPath, fileName));
                        IDBI_To_Other_BankReportExcel(wage_Process.FRM_, clientSelectionVM.selectionVMs.ToList(), clientSelectionVM.selectionVMs[0].WAG_Id, newPath, fileName, WAG_Month);
                        break;
                    case "3": //CHEQUE_CASH_Report
                        fileName = "CHEQUE_CASH_BankReport_" + DateTime.Now.ToString("ddMMyyyyHHmm") + "_" + WAG_Month + ".xlsx";
                        file = new FileInfo(Path.Combine(newPath, fileName));
                        CHEQUE_CASH_BankReportExcel(wage_Process.FRM_, clientSelectionVM.selectionVMs.ToList(), clientSelectionVM.selectionVMs[0].WAG_Id, newPath, fileName, WAG_Month);
                        break;
                    default: break;
                }

                using (var stream = new FileStream(Path.Combine(newPath, fileName), FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                new FileInfo(Path.Combine(newPath, fileName)).Delete();

            }
            catch (Exception)
            {
                TempData["message"] = "Try Again";
            }
            if (clientSelectionVM.Report == ((int)ProjectUtils.PF_REPORT_TYPE.PF_Report_Text).ToString())
            {
                return File(memory, "application/rtf", fileName);
            }
            else
            {
                return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }

        public void NEFT_BankReportExcel(Firms firm, List<SelectionVM> selectionVMs, int WAG_Id, string newPath, string fileName, string WAG_Month)
        {
            bool IsSelected = selectionVMs.Any(m => m.IsSelect.Equals(false));
            ReportsManager manager = new ReportsManager(_context);
            using (var fs = new FileStream(Path.Combine(newPath, fileName), FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook;
                workbook = new XSSFWorkbook();
                ISheet excelSheet = workbook.CreateSheet("Template");
                IFont font = workbook.CreateFont();
                font.IsBold = true;
                font.FontHeightInPoints = ((short)40);
                font.FontName = ("Cambria");
                font.Underline = FontUnderlineType.Single;
                font.Color = IndexedColors.Brown.Index;

                ICellStyle styleHeader = workbook.CreateCellStyle();
                styleHeader.FillBackgroundColor = HSSFColor.Aqua.Index;
                styleHeader.SetFont(font);

                ICellStyle styleAmount = workbook.CreateCellStyle();
                styleAmount.FillBackgroundColor = IndexedColors.Yellow.Index;
                styleAmount.FillPattern = FillPattern.SolidForeground;
                styleAmount.FillForegroundColor = IndexedColors.Yellow.Index;
                styleAmount.BorderBottom = (BorderStyle.Thin);
                styleAmount.BottomBorderColor = (IndexedColors.Black.Index);
                styleAmount.BorderLeft = (BorderStyle.Thin);
                styleAmount.LeftBorderColor = (IndexedColors.Black.Index);
                styleAmount.BorderRight = (BorderStyle.Thin);
                styleAmount.RightBorderColor = (IndexedColors.Black.Index);
                styleAmount.BorderTop = (BorderStyle.Thin);
                styleAmount.TopBorderColor = (IndexedColors.Black.Index);

                // Style the cell with font color white
                IFont fontcell = workbook.CreateFont();
                fontcell.IsBold = true;

                IFont fontClient = workbook.CreateFont();
                fontClient.IsBold = true;
                fontClient.FontHeightInPoints = ((short)15);

                IFont fontcellSub = workbook.CreateFont();
                fontcellSub.IsBold = true;
                fontcellSub.FontHeightInPoints = ((short)18);

                // Grey25Percent background
                ICellStyle style = workbook.CreateCellStyle();
                ICellStyle styleTotal = workbook.CreateCellStyle();
                ICellStyle styleClient = workbook.CreateCellStyle();
                ICellStyle styleSub = workbook.CreateCellStyle();

                styleTotal.FillForegroundColor = IndexedColors.BrightGreen.Index;
                styleTotal.FillPattern = FillPattern.SolidForeground;
                styleTotal.FillBackgroundColor = IndexedColors.BrightGreen.Index;
                styleTotal.SetFont(fontClient);

                styleClient.SetFont(fontClient);
                styleSub.SetFont(fontcellSub);
                //#77bf2a

                // Style the cell with borders all around.
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
                style.SetFont(fontcell);

                IRow row = excelSheet.CreateRow(0);
                ICell CellHeader = row.CreateCell(0);
                CellHeader.SetCellValue("Reliable");
                CellHeader.CellStyle = styleHeader;
                CellUtil.SetAlignment(CellHeader, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 6));

                IRow rowSub = excelSheet.CreateRow(1);
                ICell CellSub = rowSub.CreateCell(0);
                CellSub.SetCellValue(firm.FRM_Name.Replace("Reliable", "").ToUpper());
                CellSub.CellStyle = styleSub;
                CellUtil.SetAlignment(CellSub, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 6));

                IRow rowAdd1 = excelSheet.CreateRow(2);
                ICell CellAdd1 = rowAdd1.CreateCell(0);
                CellAdd1.SetCellValue(firm.FRM_Address1.ToUpper()+","+ firm.FRM_Address2.ToUpper()+",");
                CellUtil.SetAlignment(CellAdd1, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 6));

                IRow rowAdd2 = excelSheet.CreateRow(3);
                ICell CellAdd2 = rowAdd2.CreateCell(0);
                CellAdd2.SetCellValue("Ph.- 0231-2666389. Mobile : 9922967130. E-mail : "+firm.FRM_Email);
                CellUtil.SetAlignment(CellAdd2, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(3, 3, 0, 6));

                IRow rowSubHeading = excelSheet.CreateRow(4);
                ICell CellSubHeading = rowSubHeading.CreateCell(0);
                CellSubHeading.SetCellValue("SALARY FOR THE MONTH OF " + WAG_Month);
                CellSubHeading.CellStyle = styleClient;
                CellUtil.SetAlignment(CellSubHeading, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(4, 4, 0, 6));


                row = excelSheet.CreateRow(5);
                row.HeightInPoints = (float)(3.2 * excelSheet.DefaultRowHeightInPoints);
                ICell cell0 = row.CreateCell(0);
                cell0.SetCellValue("EMP NAME");
                excelSheet.SetColumnWidth(0, (int)((25 + 0.72) * 256));//A
                cell0.CellStyle = style;
                ICell cell1 = row.CreateCell(1);
                cell1.SetCellValue("ACCOUNT \r\n NUMBER");
                excelSheet.SetColumnWidth(1, (int)((22 + 0.72) * 256));//A
                cell1.CellStyle = style;
                ICell cell2 = row.CreateCell(2);
                cell2.SetCellValue("CURRENCY \r\n CODE");
                excelSheet.SetColumnWidth(2, (int)((15 + 0.72) * 256));//A
                cell2.CellStyle = style;
                ICell cell3 = row.CreateCell(3);
                cell3.SetCellValue("SERVICE \r\n OUTLET");
                cell3.CellStyle = style;
                ICell cell4 = row.CreateCell(4);
                cell4.SetCellValue("PART \r\n TRAN \r\n TYPE");
                cell4.CellStyle = style;
                ICell cell5 = row.CreateCell(5);
                cell5.SetCellValue("TRANSACTION \r\n AMOUNT");
                excelSheet.SetColumnWidth(5, (int)((15 + 0.72) * 256));//A
                cell5.CellStyle = style;
                ICell cell6 = row.CreateCell(6);
                cell6.SetCellValue("TRANSACTION \r\n PARTICULARS");
                excelSheet.SetColumnWidth(6, (int)((22 + 0.72) * 256));//A
                cell6.CellStyle = style;

                List<NEFT_BankReportVM> NEFT_BankReports = manager.NEFT_BankReports(WAG_Id, selectionVMs.Where(m => m.IsSelect.Equals(true)).ToList(), IsSelected);
                int rowCount = 6;
                foreach (var item in NEFT_BankReports.Select(a => new { a.CLI_Id, a.CLI_Name }).Distinct())
                {
                    row = excelSheet.CreateRow(rowCount);
                    row.HeightInPoints = (float)(1.5 * excelSheet.DefaultRowHeightInPoints);
                    ICell cellClient = row.CreateCell(0);
                    cellClient.SetCellValue(item.CLI_Name);
                    cellClient.CellStyle = styleClient;
                    excelSheet.AddMergedRegion(new CellRangeAddress(rowCount, rowCount, 0, 6));

                    rowCount++;

                    NEFT_BankReportVM NEFTBankRpt = NEFT_BankReports.Where(m => m.CLI_Id.Equals(item.CLI_Id)).FirstOrDefault();
                    List<NEFTBank_EMP_ReportVM> rpts = new List<NEFTBank_EMP_ReportVM>();
                    rpts = NEFTBankRpt.NEFTBank_EMP_ReportVMs.ToList();
                    decimal TRANSACTION_AMOUNT = 0M;
                    foreach (var emp in rpts)
                    {
                        row = excelSheet.CreateRow(rowCount);
                        row.CreateCell(0).SetCellValue(emp.EMP_FullName);
                        row.CreateCell(1).SetCellValue(emp.EMP_Account_Number);
                        row.CreateCell(2).SetCellValue(emp.CURRENCY_CODE);
                        row.CreateCell(3).SetCellValue(emp.SERVICE_OUTLET);
                        row.CreateCell(4).SetCellValue(emp.PART_TRAN_TYPE);
                        row.CreateCell(5).SetCellValue(Convert.ToString(emp.TRANSACTION_AMOUNT));
                        row.CreateCell(6).SetCellValue(emp.TRANSACTION_PARTICULARS);
                        TRANSACTION_AMOUNT = TRANSACTION_AMOUNT + emp.TRANSACTION_AMOUNT;
                        rowCount++;
                    }
                    row = excelSheet.CreateRow(rowCount);
                    ICell CellTotal = row.CreateCell(2);
                    CellTotal.SetCellValue("TOTAL");
                    CellTotal.CellStyle = styleClient;
                    CellUtil.SetAlignment(CellTotal, workbook, (short)HorizontalAlignment.Center);
                    excelSheet.AddMergedRegion(new CellRangeAddress(rowCount, rowCount, 2, 4));
                    ICell cellTotalAmt = row.CreateCell(5);
                    cellTotalAmt.SetCellValue(Convert.ToString(TRANSACTION_AMOUNT));
                    cellTotalAmt.CellStyle = styleTotal;
                    rowCount++;
                }
                workbook.Write(fs);
            }

        }

        public void IDBI_To_IDBI_BankReportExcel(Firms firm, List<SelectionVM> selectionVMs, int WAG_Id, string newPath, string fileName, string WAG_Month)
        {
            bool IsSelected = selectionVMs.Any(m => m.IsSelect.Equals(false));
            ReportsManager manager = new ReportsManager(_context);
            using (var fs = new FileStream(Path.Combine(newPath, fileName), FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook;
                workbook = new XSSFWorkbook();
                ISheet excelSheet = workbook.CreateSheet("Template");
                IFont font = workbook.CreateFont();
                font.IsBold = true;
                font.FontHeightInPoints = ((short)40);
                font.FontName = ("Cambria");
                font.Underline = FontUnderlineType.Single;
                font.Color = IndexedColors.Brown.Index;

                ICellStyle styleHeader = workbook.CreateCellStyle();
                styleHeader.FillBackgroundColor = HSSFColor.Aqua.Index;
                styleHeader.SetFont(font);

                ICellStyle styleAmount = workbook.CreateCellStyle();
                styleAmount.FillBackgroundColor = IndexedColors.Yellow.Index;
                styleAmount.FillPattern = FillPattern.SolidForeground;
                styleAmount.FillForegroundColor = IndexedColors.Yellow.Index;
                styleAmount.BorderBottom = (BorderStyle.Thin);
                styleAmount.BottomBorderColor = (IndexedColors.Black.Index);
                styleAmount.BorderLeft = (BorderStyle.Thin);
                styleAmount.LeftBorderColor = (IndexedColors.Black.Index);
                styleAmount.BorderRight = (BorderStyle.Thin);
                styleAmount.RightBorderColor = (IndexedColors.Black.Index);
                styleAmount.BorderTop = (BorderStyle.Thin);
                styleAmount.TopBorderColor = (IndexedColors.Black.Index);

                // Style the cell with font color white
                IFont fontcell = workbook.CreateFont();
                fontcell.IsBold = true;

                IFont fontClient = workbook.CreateFont();
                fontClient.IsBold = true;
                fontClient.FontHeightInPoints = ((short)15);

                IFont fontcellSub = workbook.CreateFont();
                fontcellSub.IsBold = true;
                fontcellSub.FontHeightInPoints = ((short)18);

                // Grey25Percent background
                ICellStyle style = workbook.CreateCellStyle();
                ICellStyle styleTotal = workbook.CreateCellStyle();
                ICellStyle styleClient = workbook.CreateCellStyle();
                ICellStyle styleSub = workbook.CreateCellStyle();

                styleTotal.FillForegroundColor = IndexedColors.BrightGreen.Index;
                styleTotal.FillPattern = FillPattern.SolidForeground;
                styleTotal.FillBackgroundColor = IndexedColors.BrightGreen.Index;
                styleTotal.SetFont(fontcell);

                styleClient.SetFont(fontcell);
                styleSub.SetFont(fontcellSub);
                //#77bf2a

                // Style the cell with borders all around.
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
                style.SetFont(fontcell);

                IRow row = excelSheet.CreateRow(0);
                ICell CellHeader = row.CreateCell(0);
                CellHeader.SetCellValue("Reliable");
                CellHeader.CellStyle = styleHeader;
                CellUtil.SetAlignment(CellHeader, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 6));

                IRow rowSub = excelSheet.CreateRow(1);
                ICell CellSub = rowSub.CreateCell(0);
                CellSub.SetCellValue(firm.FRM_Name.Replace("Reliable", "").ToUpper());
                CellSub.CellStyle = styleSub;
                CellUtil.SetAlignment(CellSub, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 6));

                IRow rowAdd1 = excelSheet.CreateRow(2);
                ICell CellAdd1 = rowAdd1.CreateCell(0);
                CellAdd1.SetCellValue(firm.FRM_Address1.ToUpper()+","+ firm.FRM_Address2.ToUpper()+",");
                CellUtil.SetAlignment(CellAdd1, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 6));

                IRow rowAdd2 = excelSheet.CreateRow(3);
                ICell CellAdd2 = rowAdd2.CreateCell(0);
                CellAdd2.SetCellValue("Ph.- 0231-2666389. Mobile : 9922967130. E-mail : "+ firm.FRM_Email);
                CellUtil.SetAlignment(CellAdd2, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(3, 3, 0, 6));

                IRow rowSubHeading = excelSheet.CreateRow(4);
                ICell CellSubHeading = rowSubHeading.CreateCell(0);
                CellSubHeading.SetCellValue("SALARY FOR THE MONTH OF " + WAG_Month);
                CellSubHeading.CellStyle = styleClient;
                CellUtil.SetAlignment(CellSubHeading, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(4, 4, 0, 6));


                row = excelSheet.CreateRow(5);
                row.HeightInPoints = (float)(3.2 * excelSheet.DefaultRowHeightInPoints);
                ICell cell0 = row.CreateCell(0);
                cell0.SetCellValue("EMP NAME");
                excelSheet.SetColumnWidth(0, (int)((25 + 0.72) * 256));//A
                cell0.CellStyle = style;
                ICell cell1 = row.CreateCell(1);
                cell1.SetCellValue("ACCOUNT \r\n NUMBER");
                excelSheet.SetColumnWidth(1, (int)((22 + 0.72) * 256));//A
                cell1.CellStyle = style;
                ICell cell2 = row.CreateCell(2);
                cell2.SetCellValue("CURRENCY \r\n CODE");
                excelSheet.SetColumnWidth(2, (int)((15 + 0.72) * 256));//A
                cell2.CellStyle = style;
                ICell cell3 = row.CreateCell(3);
                cell3.SetCellValue("SERVICE \r\n OUTLET");
                cell3.CellStyle = style;
                ICell cell4 = row.CreateCell(4);
                cell4.SetCellValue("PART \r\n TRAN \r\n TYPE");
                cell4.CellStyle = style;
                ICell cell5 = row.CreateCell(5);
                cell5.SetCellValue("TRANSACTION \r\n AMOUNT");
                excelSheet.SetColumnWidth(5, (int)((15 + 0.72) * 256));//A
                cell5.CellStyle = style;
                ICell cell6 = row.CreateCell(6);
                cell6.SetCellValue("TRANSACTION \r\n PARTICULARS");
                excelSheet.SetColumnWidth(6, (int)((22 + 0.72) * 256));//A
                cell6.CellStyle = style;

                List<BankReportVM> BankReportVMs = manager.IDBI_TO_IDBI_BankReports(WAG_Id, selectionVMs.Where(m => m.IsSelect.Equals(true)).ToList(), IsSelected);
                int rowCount = 6;
                decimal TRANSACTION_AMOUNT = 0M;
                foreach (var item in BankReportVMs)
                {
                    row = excelSheet.CreateRow(rowCount);
                    row.CreateCell(0).SetCellValue(item.EMP_NAME);
                    row.CreateCell(1).SetCellValue(item.EMP_ACCOUNT_NUMBER);
                    row.CreateCell(2).SetCellValue(item.EMP_CURRENCY_CODE);
                    row.CreateCell(3).SetCellValue(item.EMP_SERVICE_OUTLET);
                    row.CreateCell(4).SetCellValue(item.EMP_PART_TRAN_TYPE);
                    row.CreateCell(5).SetCellValue(Convert.ToString(item.EMP_TRANSACTION_AMOUNT));
                    row.CreateCell(6).SetCellValue(item.EMP_TRANSACTION_PARTICULARS);
                    TRANSACTION_AMOUNT = TRANSACTION_AMOUNT + item.EMP_TRANSACTION_AMOUNT;
                    rowCount++;
                }
                row = excelSheet.CreateRow(rowCount);
                ICell CellTotal = row.CreateCell(2);
                CellTotal.SetCellValue("TOTAL");
                CellTotal.CellStyle = styleClient;
                CellUtil.SetAlignment(CellTotal, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(rowCount, rowCount, 2, 4));
                ICell cellTotalAmt = row.CreateCell(5);
                cellTotalAmt.SetCellValue(Convert.ToString(TRANSACTION_AMOUNT));
                cellTotalAmt.CellStyle = styleTotal;
                workbook.Write(fs);
            }
        }

        public void IDBI_To_Other_BankReportExcel(Firms firm, List<SelectionVM> selectionVMs, int WAG_Id, string newPath, string fileName, string WAG_Month)
        {
            bool IsSelected = selectionVMs.Any(m => m.IsSelect.Equals(false));
            ReportsManager manager = new ReportsManager(_context);
            using (var fs = new FileStream(Path.Combine(newPath, fileName), FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook;
                workbook = new XSSFWorkbook();
                ISheet excelSheet = workbook.CreateSheet("Template");
                IFont font = workbook.CreateFont();
                font.IsBold = true;
                font.FontHeightInPoints = ((short)40);
                font.FontName = ("Cambria");
                font.Underline = FontUnderlineType.Single;
                font.Color = IndexedColors.Brown.Index;

                ICellStyle styleHeader = workbook.CreateCellStyle();
                styleHeader.FillBackgroundColor = HSSFColor.Aqua.Index;
                styleHeader.SetFont(font);


                // Style the cell with font color white
                IFont fontcell = workbook.CreateFont();
                fontcell.IsBold = true;

                IFont fontClient = workbook.CreateFont();
                fontClient.IsBold = true;
                fontClient.FontHeightInPoints = ((short)15);

                IFont fontcellSub = workbook.CreateFont();
                fontcellSub.IsBold = true;
                fontcellSub.FontHeightInPoints = ((short)18);

                // Grey25Percent background
                ICellStyle style = workbook.CreateCellStyle();
                ICellStyle styleTotal = workbook.CreateCellStyle();
                ICellStyle styleClient = workbook.CreateCellStyle();
                ICellStyle styleSub = workbook.CreateCellStyle();

                styleTotal.WrapText = true;
                styleTotal.SetFont(fontcell);

                styleClient.SetFont(fontcell);
                styleSub.SetFont(fontcellSub);
                //#77bf2a

                // Style the cell with borders all around.
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
                style.SetFont(fontcell);

                IRow row = excelSheet.CreateRow(0);
                ICell CellHeader = row.CreateCell(0);
                CellHeader.SetCellValue("Reliable");
                CellHeader.CellStyle = styleHeader;
                CellUtil.SetAlignment(CellHeader, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 8));

                IRow rowSub = excelSheet.CreateRow(1);
                ICell CellSub = rowSub.CreateCell(0);
                CellSub.SetCellValue(firm.FRM_Name.Replace("Reliable", "").ToUpper());
                CellSub.CellStyle = styleSub;
                CellUtil.SetAlignment(CellSub, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 8));

                IRow rowAdd1 = excelSheet.CreateRow(2);
                ICell CellAdd1 = rowAdd1.CreateCell(0);
                CellAdd1.SetCellValue(firm.FRM_Address1.ToUpper()+","+firm.FRM_Address2.ToUpper()+",");
                CellUtil.SetAlignment(CellAdd1, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 8));

                IRow rowAdd2 = excelSheet.CreateRow(3);
                ICell CellAdd2 = rowAdd2.CreateCell(0);
                CellAdd2.SetCellValue("Ph.- 0231-2666389. Mobile : 9922967130. E-mail : "+firm.FRM_Email);
                CellUtil.SetAlignment(CellAdd2, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(3, 3, 0, 8));

                IRow rowSubHeading = excelSheet.CreateRow(4);
                ICell CellSubHeading = rowSubHeading.CreateCell(0);
                CellSubHeading.SetCellValue("SALARY FOR THE MONTH OF " + WAG_Month);
                CellSubHeading.CellStyle = styleClient;
                CellUtil.SetAlignment(CellSubHeading, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(4, 4, 0, 8));

                row = excelSheet.CreateRow(5);
                ICell cell_1 = row.CreateCell(0);
                cell_1.SetCellValue("1");
                cell_1.CellStyle = style;
                CellUtil.SetAlignment(cell_1, workbook, (short)HorizontalAlignment.Center);
                excelSheet.SetColumnWidth(0, (int)((10 + 0.72) * 256));
                ICell cell_2 = row.CreateCell(1);
                cell_2.SetCellValue("2");
                cell_2.CellStyle = style;
                CellUtil.SetAlignment(cell_2, workbook, (short)HorizontalAlignment.Center);
                excelSheet.SetColumnWidth(1, (int)((15 + 0.72) * 256));
                ICell cell_3 = row.CreateCell(2);
                cell_3.SetCellValue("3");
                cell_3.CellStyle = style;
                CellUtil.SetAlignment(cell_3, workbook, (short)HorizontalAlignment.Center);
                excelSheet.SetColumnWidth(2, (int)((14 + 0.72) * 256));
                ICell cell_4 = row.CreateCell(3);
                cell_4.SetCellValue("4");
                cell_4.CellStyle = style;
                CellUtil.SetAlignment(cell_4, workbook, (short)HorizontalAlignment.Center);
                excelSheet.SetColumnWidth(3, (int)((17 + 0.72) * 256));
                ICell cell_5 = row.CreateCell(4);
                cell_5.SetCellValue("5");
                cell_5.CellStyle = style;
                CellUtil.SetAlignment(cell_5, workbook, (short)HorizontalAlignment.Center);
                ICell cell_6 = row.CreateCell(5);
                cell_6.SetCellValue("6");
                cell_6.CellStyle = style;
                CellUtil.SetAlignment(cell_6, workbook, (short)HorizontalAlignment.Center);
                excelSheet.SetColumnWidth(5, (int)((35 + 0.72) * 256));
                ICell cell_7 = row.CreateCell(6);
                cell_7.SetCellValue("7");
                cell_7.CellStyle = style;
                CellUtil.SetAlignment(cell_7, workbook, (short)HorizontalAlignment.Center);
                excelSheet.SetColumnWidth(6, (int)((12 + 0.72) * 256));
                ICell cell_8 = row.CreateCell(7);
                cell_8.SetCellValue("8");
                cell_8.CellStyle = style;
                CellUtil.SetAlignment(cell_8, workbook, (short)HorizontalAlignment.Center);
                excelSheet.SetColumnWidth(7, (int)((12 + 0.72) * 256));
                ICell cell_9 = row.CreateCell(8);
                cell_9.SetCellValue("9");
                cell_9.CellStyle = style;
                CellUtil.SetAlignment(cell_9, workbook, (short)HorizontalAlignment.Center);
                excelSheet.SetColumnWidth(8, (int)((12 + 0.72) * 256));

                row = excelSheet.CreateRow(6);
                row.HeightInPoints = (float)(3.2 * excelSheet.DefaultRowHeightInPoints);
                ICell cell0 = row.CreateCell(0);
                cell0.SetCellValue("AMOUNT");
                cell0.CellStyle = style;
                ICell cell1 = row.CreateCell(1);
                cell1.SetCellValue("ACCOUNT \r\n SENDERS \r\n NUMBER");
                cell1.CellStyle = style;
                ICell cell2 = row.CreateCell(2);
                cell2.SetCellValue("CODE \r\n IFSC");
                cell2.CellStyle = style;
                ICell cell3 = row.CreateCell(3);
                cell3.SetCellValue("ACCOUNT \r\n RECEIVERS \r\n NUMBER");
                cell3.CellStyle = style;
                ICell cell4 = row.CreateCell(4);
                cell4.SetCellValue("TYPE \r\n A/C");
                cell4.CellStyle = style;
                ICell cell5 = row.CreateCell(5);
                cell5.SetCellValue("NAME \r\n BENIFECIARY");
                cell5.CellStyle = style;
                ICell cell6 = row.CreateCell(6);
                cell6.SetCellValue("ADDRESS");
                cell6.CellStyle = style;
                ICell cell7 = row.CreateCell(7);
                cell7.SetCellValue("MESSAGE");
                cell7.CellStyle = style;
                ICell cell8 = row.CreateCell(8);
                cell8.SetCellValue("ORIGINETOR");
                cell8.CellStyle = style;

                List<BankReportVM> BankReportVMs = manager.IDBI_TO_Other_BankReports(WAG_Id, selectionVMs.Where(m => m.IsSelect.Equals(true)).ToList(), IsSelected);
                int rowCount = 7;
                decimal TRANSACTION_AMOUNT = 0M;
                foreach (var item in BankReportVMs)
                {
                    row = excelSheet.CreateRow(rowCount);
                    row.HeightInPoints = (float)(1.2 * excelSheet.DefaultRowHeightInPoints);
                    row.CreateCell(0).SetCellValue(Convert.ToString(item.EMP_TRANSACTION_AMOUNT));
                    row.CreateCell(1).SetCellValue(item.ACCOUNT_SENDER_NUMBER);
                    row.CreateCell(2).SetCellValue(item.ACCOUNT_IFSC_CODE);
                    row.CreateCell(3).SetCellValue(item.ACCOUNT_RECEIVERS_NUMBER);
                    row.CreateCell(4).SetCellValue(item.ACCOUNT_TYPE);
                    row.CreateCell(5).SetCellValue(item.EMP_NAME);
                    row.CreateCell(6).SetCellValue(item.EMP_ADDRESS);
                    row.CreateCell(7).SetCellValue(item.MESSAGE);
                    row.CreateCell(8).SetCellValue(item.ORIGINETOR);
                    TRANSACTION_AMOUNT = TRANSACTION_AMOUNT + item.EMP_TRANSACTION_AMOUNT;
                    rowCount++;
                }
                row = excelSheet.CreateRow(rowCount);
                row.HeightInPoints = (float)(1.2 * excelSheet.DefaultRowHeightInPoints);
                ICell cellTotalAmt = row.CreateCell(0);
                cellTotalAmt.SetCellValue(Convert.ToString(TRANSACTION_AMOUNT));
                cellTotalAmt.CellStyle = styleTotal;
                workbook.Write(fs);
            }
        }

        public void CHEQUE_CASH_BankReportExcel(Firms firm, List<SelectionVM> selectionVMs, int WAG_Id, string newPath, string fileName, string WAG_Month)
        {
            bool IsSelected = selectionVMs.Any(m => m.IsSelect.Equals(false));
            ReportsManager manager = new ReportsManager(_context);
            using (var fs = new FileStream(Path.Combine(newPath, fileName), FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook;
                workbook = new XSSFWorkbook();
                ISheet excelSheet = workbook.CreateSheet("Template");
                IFont font = workbook.CreateFont();
                font.IsBold = true;
                font.FontHeightInPoints = ((short)40);
                font.FontName = ("Cambria");
                font.Underline = FontUnderlineType.Single;
                font.Color = IndexedColors.Brown.Index;

                ICellStyle styleHeader = workbook.CreateCellStyle();
                styleHeader.FillBackgroundColor = HSSFColor.Aqua.Index;
                styleHeader.SetFont(font);

                ICellStyle styleAmount = workbook.CreateCellStyle();
                styleAmount.FillBackgroundColor = IndexedColors.Yellow.Index;
                styleAmount.FillPattern = FillPattern.SolidForeground;
                styleAmount.FillForegroundColor = IndexedColors.Yellow.Index;
                styleAmount.BorderBottom = (BorderStyle.Thin);
                styleAmount.BottomBorderColor = (IndexedColors.Black.Index);
                styleAmount.BorderLeft = (BorderStyle.Thin);
                styleAmount.LeftBorderColor = (IndexedColors.Black.Index);
                styleAmount.BorderRight = (BorderStyle.Thin);
                styleAmount.RightBorderColor = (IndexedColors.Black.Index);
                styleAmount.BorderTop = (BorderStyle.Thin);
                styleAmount.TopBorderColor = (IndexedColors.Black.Index);

                // Style the cell with font color white
                IFont fontcell = workbook.CreateFont();
                fontcell.IsBold = true;

                IFont fontClient = workbook.CreateFont();
                fontClient.IsBold = true;
                fontClient.FontHeightInPoints = ((short)15);

                IFont fontcellSub = workbook.CreateFont();
                fontcellSub.IsBold = true;
                fontcellSub.FontHeightInPoints = ((short)18);

                // Grey25Percent background
                ICellStyle style = workbook.CreateCellStyle();
                ICellStyle styleTotal = workbook.CreateCellStyle();
                ICellStyle styleClient = workbook.CreateCellStyle();
                ICellStyle styleSub = workbook.CreateCellStyle();

                styleTotal.WrapText = true;
                styleTotal.VerticalAlignment = VerticalAlignment.Center;
                styleTotal.SetFont(fontcell);
                styleClient.SetFont(fontcell);
                styleSub.SetFont(fontcellSub);
                //#77bf2a

                // Style the cell with borders all around.
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
                style.SetFont(fontcell);

                IRow row = excelSheet.CreateRow(0);

                // IRow rowSub = excelSheet.CreateRow(0);
                ICell CellSub = row.CreateCell(0);
                CellSub.SetCellValue(firm.FRM_Name.ToUpper());
                CellSub.CellStyle = styleSub;
                CellUtil.SetAlignment(CellSub, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 5));

                IRow rowAdd1 = excelSheet.CreateRow(1);
                ICell CellAdd1 = rowAdd1.CreateCell(0);
                CellAdd1.SetCellValue(firm.FRM_Address1.ToUpper()+","+ firm.FRM_Address2.ToUpper()+",");
                CellUtil.SetAlignment(CellAdd1, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 5));

                //IRow rowAdd2 = excelSheet.CreateRow(3);
                //ICell CellAdd2 = rowAdd2.CreateCell(0);
                //CellAdd2.SetCellValue("Ph.- 0231-2666389. Mobile : 9922967130. E-mail : reliable.manpower@yahoo.com");
                //CellUtil.SetAlignment(CellAdd2, workbook, (short)HorizontalAlignment.Center);
                //excelSheet.AddMergedRegion(new CellRangeAddress(3, 3, 0, 5));

                IRow rowSubHeading = excelSheet.CreateRow(2);
                ICell CellSubHeading = rowSubHeading.CreateCell(0);
                CellSubHeading.SetCellValue("CHEQUE/CASH PAYMENT FOR THE MONTH OF " + WAG_Month);
                CellSubHeading.CellStyle = styleClient;
                CellUtil.SetAlignment(CellSubHeading, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 5));

                row = excelSheet.CreateRow(3);
                row.HeightInPoints = (float)(3.2 * excelSheet.DefaultRowHeightInPoints);
                ICell cell0 = row.CreateCell(0);
                cell0.SetCellValue("SR.NO");
                cell0.CellStyle = style;
                ICell cell1 = row.CreateCell(1);
                cell1.SetCellValue("NAME");
                excelSheet.SetColumnWidth(1, (int)((35 + 0.72) * 256));
                cell1.CellStyle = style;
                ICell cell2 = row.CreateCell(2);
                cell2.SetCellValue("LOCATION");
                excelSheet.SetColumnWidth(2, (int)((22 + 0.72) * 256));
                cell2.CellStyle = style;
                ICell cell3 = row.CreateCell(3);
                cell3.SetCellValue("SALARY");
                excelSheet.SetColumnWidth(3, (int)((15 + 0.72) * 256));
                cell3.CellStyle = style;
                ICell cell4 = row.CreateCell(4);
                cell4.SetCellValue("CHEQUE/CASH");
                excelSheet.SetColumnWidth(4, (int)((15 + 0.72) * 256));
                cell4.CellStyle = style;
                ICell cell5 = row.CreateCell(5);
                cell5.SetCellValue("SIGN");
                excelSheet.SetColumnWidth(5, (int)((15 + 0.72) * 256));
                cell5.CellStyle = style;

                List<BankReportVM> BankReportVMs = manager.ChequeCash_BankReports(WAG_Id, selectionVMs.Where(m => m.IsSelect.Equals(true)).ToList(), IsSelected);
                int rowCount = 4, srNo = 1;
                decimal TRANSACTION_AMOUNT = 0M;
                foreach (var item in BankReportVMs)
                {
                    row = excelSheet.CreateRow(rowCount);
                    row.HeightInPoints = (float)(1.2 * excelSheet.DefaultRowHeightInPoints);
                    row.CreateCell(0).SetCellValue(Convert.ToString(srNo));
                    row.CreateCell(1).SetCellValue(item.EMP_NAME);
                    row.CreateCell(2).SetCellValue(item.EMP_ADDRESS);
                    row.CreateCell(3).SetCellValue(Convert.ToString(item.EMP_TRANSACTION_AMOUNT));
                    row.CreateCell(4).SetCellValue("");
                    row.CreateCell(5).SetCellValue("");
                    TRANSACTION_AMOUNT = TRANSACTION_AMOUNT + item.EMP_TRANSACTION_AMOUNT;
                    rowCount++;
                    srNo++;
                }
                row = excelSheet.CreateRow(rowCount);
                row.HeightInPoints = (float)(1.3 * excelSheet.DefaultRowHeightInPoints);
                ICell cellTotal = row.CreateCell(0);
                cellTotal.SetCellValue("GRAND TOTAL");
                cellTotal.CellStyle = styleTotal;
                CellUtil.SetAlignment(cellTotal, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(rowCount, rowCount, 0, 2));

                ICell cellTotalAmt = row.CreateCell(3);
                cellTotalAmt.SetCellValue(Convert.ToString(TRANSACTION_AMOUNT));
                cellTotalAmt.CellStyle = styleTotal;
                workbook.Write(fs);
            }

        }

        #endregion

        #region ESIC Reports

        public ActionResult ClientsSelectionForESIC(int WAG_Id, int FRM_Id)
        {
            WageProcessManager wageProcessManager = new WageProcessManager(_context);
            WageRegisterManager wageRegisterManager = new WageRegisterManager(_context);
            ClientSelectionVM clientSelectionVM = new ClientSelectionVM();
            List<Clients> clients = wageRegisterManager.GetWageRegisters(WAG_Id).Where(m => m.WAG_.FRM_Id.Equals(FRM_Id)).Select(m => m.CLI_).Distinct().ToList();
            clientSelectionVM.selectionVMs = ClientSelectionMapper.mapMe(clients, WAG_Id);
            clientSelectionVM.Report = ((int)ProjectUtils.ESIC_REPORT_TYPE.Client_Wise_ESIC_Excel).ToString();
            clientSelectionVM.FRM_Id = FRM_Id;
            ClientsManager clientsManager = new ClientsManager(_context);
            clientSelectionVM.TotalActiveClients = clientsManager.GetActiveClientOfMonthByFirmId(wageProcessManager.getWageProcessById(WAG_Id).WAG_Month, FRM_Id).Count();
            return View(clientSelectionVM);
        }

        public async Task<FileResult> ESIC_Reports(ClientSelectionVM clientSelectionVM)
        {
            string report = clientSelectionVM.Report;

            WageProcessManager wageProcess = new WageProcessManager(_context);
            Wage_Process wage_Process = wageProcess.getWageProcessById(clientSelectionVM.selectionVMs[0].WAG_Id);
            DateTime wageMonth = wage_Process.WAG_Month;
            string WAG_Month = wageMonth.ToString("MMMM") + "-" + wageMonth.ToString("yyyy");

            string newPath = ProjectUtils.GetTempFolderPath(_hostingEnvironment.WebRootPath);
            var memory = new MemoryStream();

            string fileName = "";
            FileInfo file;
            try
            {
                switch (clientSelectionVM.Report)
                {
                    case "0": //Client_Wise_ESIC_Excel
                        fileName = "Client_Wise_ESIC_" + DateTime.Now.ToString("ddMMyyyyHHmm") + "_" + WAG_Month + ".xlsx";
                        file = new FileInfo(Path.Combine(newPath, fileName));
                        Client_Wise_ESIC_Excel(wage_Process.FRM_, clientSelectionVM.selectionVMs.ToList(), clientSelectionVM.selectionVMs[0].WAG_Id, newPath, fileName, WAG_Month);
                        break;
                    case "1": //Employee_Wise_Esic_Details_Excel
                        fileName = "Employee_Wise_ESIC_" + DateTime.Now.ToString("ddMMyyyyHHmm") + "_" + WAG_Month + ".xlsx";
                        file = new FileInfo(Path.Combine(newPath, fileName));
                        Employee_Wise_Esic_Details_Excel(wage_Process.FRM_, clientSelectionVM.selectionVMs.ToList(), clientSelectionVM.selectionVMs[0].WAG_Id, newPath, fileName, WAG_Month);
                        break;
                    case "2": //ESIC_Employees_Pending_For_Registration_Excel
                        fileName = "EISC_Employees_Pending_For_Registration_Report_" + DateTime.Now.ToString("ddMMyyyyHHmm") + "_" + WAG_Month + ".xlsx";
                        file = new FileInfo(Path.Combine(newPath, fileName));
                        ESIC_Employees_Pending_For_Registration_Excel(wage_Process.FRM_, clientSelectionVM.selectionVMs.ToList(), clientSelectionVM.selectionVMs[0].WAG_Id, newPath, fileName, WAG_Month);
                        break;
                    default: break;
                }

                using (var stream = new FileStream(Path.Combine(newPath, fileName), FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                new FileInfo(Path.Combine(newPath, fileName)).Delete();

            }
            catch (Exception)
            {
                TempData["message"] = "Try Again";
            }
            if (clientSelectionVM.Report == ((int)ProjectUtils.PF_REPORT_TYPE.PF_Report_Text).ToString())
            {
                return File(memory, "application/rtf", fileName);
            }
            else
            {
                return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }

        public void Client_Wise_ESIC_Excel(Firms firm,List<SelectionVM> selectionVMs, int WAG_Id, string newPath, string fileName, string WAG_Month)
        {
            bool IsSelected = selectionVMs.Any(m => m.IsSelect.Equals(false));
            ReportsManager manager = new ReportsManager(_context);
            using (var fs = new FileStream(Path.Combine(newPath, fileName), FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook;
                workbook = new XSSFWorkbook();
                ISheet excelSheet = workbook.CreateSheet("Template");
                IFont font = workbook.CreateFont();
                font.IsBold = true;
                font.FontHeightInPoints = ((short)22);
                font.FontName = ("Cambria");
                IFont fontcell = workbook.CreateFont();
                fontcell.IsBold = true;

                IFont fontcellSub = workbook.CreateFont();
                fontcellSub.IsBold = true;
                fontcellSub.FontHeightInPoints = ((short)18);

                ICellStyle styleHeader = workbook.CreateCellStyle();
                styleHeader.FillBackgroundColor = HSSFColor.Grey25Percent.Index;
                styleHeader.SetFont(font);

                ICellStyle styleAmount = workbook.CreateCellStyle();
                styleAmount.FillBackgroundColor = IndexedColors.Grey25Percent.Index;
                styleAmount.FillPattern = FillPattern.SolidForeground;
                styleAmount.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                styleAmount.BorderBottom = (BorderStyle.Thin);
                styleAmount.BottomBorderColor = (IndexedColors.Black.Index);
                styleAmount.BorderLeft = (BorderStyle.Thin);
                styleAmount.LeftBorderColor = (IndexedColors.Black.Index);
                styleAmount.BorderRight = (BorderStyle.Thin);
                styleAmount.RightBorderColor = (IndexedColors.Black.Index);
                styleAmount.BorderTop = (BorderStyle.Thin);
                styleAmount.TopBorderColor = (IndexedColors.Black.Index);

                // Grey25Percent background
                ICellStyle style = workbook.CreateCellStyle();
                ICellStyle styleClient = workbook.CreateCellStyle();
                ICellStyle styleSub = workbook.CreateCellStyle();

                styleClient.SetFont(fontcell);
                styleSub.SetFont(fontcellSub);

                style.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                style.FillPattern = FillPattern.SolidForeground;
                style.FillBackgroundColor = IndexedColors.Grey25Percent.Index;


                // Style the cell with borders all around.
                style.WrapText = true;
                style.BorderBottom = (BorderStyle.Thin);
                style.BottomBorderColor = (IndexedColors.Black.Index);
                style.BorderLeft = (BorderStyle.Thin);
                style.LeftBorderColor = (IndexedColors.Black.Index);
                style.BorderRight = (BorderStyle.Thin);
                style.RightBorderColor = (IndexedColors.Black.Index);
                style.BorderTop = (BorderStyle.Thin);
                style.TopBorderColor = (IndexedColors.Black.Index);

                // Style the cell with font color white                
                style.SetFont(fontcell);

                IRow row = excelSheet.CreateRow(0);
                ICell CellSub = row.CreateCell(0);
                CellSub.SetCellValue(firm.FRM_Name.ToUpper());
                CellSub.CellStyle = styleSub;
                CellUtil.SetAlignment(CellSub, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 6));

                IRow rowAdd1 = excelSheet.CreateRow(1);
                ICell CellAdd1 = rowAdd1.CreateCell(0);
                CellAdd1.SetCellValue(firm.FRM_Address1.ToUpper()+","+ firm.FRM_Address2.ToUpper()+",");
                CellUtil.SetAlignment(CellAdd1, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 6));

                IRow rowSubHeading = excelSheet.CreateRow(2);
                ICell CellSubHeading = rowSubHeading.CreateCell(0);
                CellSubHeading.SetCellValue("LIST OF ESIC CONTRIBUTION FOR THE MONTH OF " + WAG_Month);
                CellSubHeading.CellStyle = styleClient;
                CellUtil.SetAlignment(CellSubHeading, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 6));

                row = excelSheet.CreateRow(3);
                ICell cell0 = row.CreateCell(0);
                cell0.SetCellValue("SR. NO.");
                cell0.CellStyle = style;
                ICell cell1 = row.CreateCell(1);
                cell1.SetCellValue("NAME OF COMPANY");
                excelSheet.SetColumnWidth(1, (int)((30 + 0.72) * 256));
                cell1.CellStyle = style;
                ICell cell2 = row.CreateCell(2);
                cell2.SetCellValue("NO. OF \r\n EMPLOYEE");
                excelSheet.SetColumnWidth(2, (int)((15 + 0.72) * 256));
                cell2.CellStyle = style;
                ICell cell3 = row.CreateCell(3);
                cell3.SetCellValue("TOTAL \r\n WAGES");
                excelSheet.SetColumnWidth(3, (int)((15 + 0.72) * 256));
                cell3.CellStyle = style;
                ICell cell4 = row.CreateCell(4);
                cell4.SetCellValue("EMPLOYEES \r\n CONTRIBUTION");
                excelSheet.SetColumnWidth(4, (int)((15 + 0.72) * 256));
                cell4.CellStyle = style;
                ICell cell5 = row.CreateCell(5);
                cell5.SetCellValue("EMPLOYERS \r\n CONTRIBUTION");
                excelSheet.SetColumnWidth(5, (int)((15 + 0.72) * 256));
                cell5.CellStyle = style;
                ICell cell6 = row.CreateCell(6);
                cell6.SetCellValue("TOTAL \r\n CONTRIBUTION");
                cell6.CellStyle = style;
                excelSheet.SetColumnWidth(6, (int)((15 + 0.72) * 256));
                List<ESICReportVM> ESICReportVMS = manager.Client_Wise_ESIC(WAG_Id, selectionVMs.Where(m => m.IsSelect.Equals(true)).ToList(), IsSelected);
                int rowCount = 4, SrNo = 0, NO_OF_EMPLOYEE = 0;
                decimal TOTAL_WAGES = 0M, EMPLOYEES_CONTRIBUTION = 0M, EMPLOYERS_CONTRIBUTION = 0M, TOTAL_CONTRIBUTION = 0M;
                foreach (var item in ESICReportVMS)
                {
                    SrNo++;
                    row = excelSheet.CreateRow(rowCount);
                    row.HeightInPoints = (float)(1.5 * excelSheet.DefaultRowHeightInPoints);
                    row.CreateCell(0).SetCellValue(SrNo);
                    row.CreateCell(1).SetCellValue(item.NAME_OF_COMPANY);
                    row.CreateCell(2).SetCellValue(Convert.ToString(item.NO_OF_EMPLOYEE));
                    row.CreateCell(3).SetCellValue(Convert.ToString(item.TOTAL_WAGES));
                    row.CreateCell(4).SetCellValue(Convert.ToString(item.EMPLOYEES_CONTRIBUTION));
                    row.CreateCell(5).SetCellValue(Convert.ToString(item.EMPLOYERS_CONTRIBUTION));
                    row.CreateCell(6).SetCellValue(Convert.ToString(item.TOTAL_CONTRIBUTION));
                    rowCount++;
                    NO_OF_EMPLOYEE = NO_OF_EMPLOYEE + item.NO_OF_EMPLOYEE;
                    TOTAL_WAGES = TOTAL_WAGES + item.TOTAL_WAGES;
                    EMPLOYEES_CONTRIBUTION = EMPLOYEES_CONTRIBUTION + item.EMPLOYEES_CONTRIBUTION;
                    EMPLOYERS_CONTRIBUTION = EMPLOYERS_CONTRIBUTION + item.EMPLOYERS_CONTRIBUTION;
                    TOTAL_CONTRIBUTION = TOTAL_CONTRIBUTION + item.TOTAL_CONTRIBUTION;
                }

                row = excelSheet.CreateRow(rowCount);
                ICell cellTot0 = row.CreateCell(0);
                cellTot0.SetCellValue("TOTAL");
                excelSheet.AddMergedRegion(new CellRangeAddress(rowCount, rowCount, 0, 1));
                cellTot0.CellStyle = style;

                ICell cellTot1 = row.CreateCell(2);
                cellTot1.SetCellValue(NO_OF_EMPLOYEE);
                cellTot1.CellStyle = style;

                ICell cellTot2 = row.CreateCell(3);
                cellTot2.SetCellValue(Convert.ToString(TOTAL_WAGES));
                cellTot2.CellStyle = style;

                ICell cellTot3 = row.CreateCell(4);
                cellTot3.SetCellValue(Convert.ToString(EMPLOYEES_CONTRIBUTION));
                cellTot3.CellStyle = style;

                ICell cellTot4 = row.CreateCell(5);
                cellTot4.SetCellValue(Convert.ToString(EMPLOYERS_CONTRIBUTION));
                cellTot4.CellStyle = style;

                ICell cellTot5 = row.CreateCell(6);
                cellTot5.SetCellValue(Convert.ToString(TOTAL_CONTRIBUTION));
                cellTot5.CellStyle = style;

                workbook.Write(fs);
            }
        }

        public void Employee_Wise_Esic_Details_Excel(Firms firm, List<SelectionVM> selectionVMs, int WAG_Id, string newPath, string fileName, string WAG_Month)
        {
            bool IsSelected = selectionVMs.Any(m => m.IsSelect.Equals(false));
            ReportsManager manager = new ReportsManager(_context);
            using (var fs = new FileStream(Path.Combine(newPath, fileName), FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook;
                workbook = new XSSFWorkbook();
                ISheet excelSheet = workbook.CreateSheet("Template");
                IFont font = workbook.CreateFont();
                font.IsBold = true;
                font.FontHeightInPoints = ((short)22);
                font.FontName = ("Cambria");
                ICellStyle styleHeader = workbook.CreateCellStyle();
                styleHeader.FillBackgroundColor = HSSFColor.Aqua.Index;
                styleHeader.SetFont(font);

                IFont fontClient = workbook.CreateFont();
                fontClient.IsBold = true;
                fontClient.FontHeightInPoints = ((short)15);
                ICellStyle styleClient = workbook.CreateCellStyle();
                styleClient.SetFont(fontClient);

                ICellStyle styleAmount = workbook.CreateCellStyle();
                styleAmount.FillBackgroundColor = IndexedColors.Yellow.Index;
                styleAmount.FillPattern = FillPattern.SolidForeground;
                styleAmount.FillForegroundColor = IndexedColors.Yellow.Index;
                styleAmount.BorderBottom = (BorderStyle.Thin);
                styleAmount.BottomBorderColor = (IndexedColors.Black.Index);
                styleAmount.BorderLeft = (BorderStyle.Thin);
                styleAmount.LeftBorderColor = (IndexedColors.Black.Index);
                styleAmount.BorderRight = (BorderStyle.Thin);
                styleAmount.RightBorderColor = (IndexedColors.Black.Index);
                styleAmount.BorderTop = (BorderStyle.Thin);
                styleAmount.TopBorderColor = (IndexedColors.Black.Index);

                // Grey25Percent background
                ICellStyle style = workbook.CreateCellStyle();
                style.FillForegroundColor = IndexedColors.Aqua.Index;
                style.FillPattern = FillPattern.SolidForeground;
                style.FillBackgroundColor = IndexedColors.Aqua.Index;


                // Style the cell with borders all around.
                style.WrapText = true;
                style.BorderBottom = (BorderStyle.Thin);
                style.BottomBorderColor = (IndexedColors.Black.Index);
                style.BorderLeft = (BorderStyle.Thin);
                style.LeftBorderColor = (IndexedColors.Black.Index);
                style.BorderRight = (BorderStyle.Thin);
                style.RightBorderColor = (IndexedColors.Black.Index);
                style.BorderTop = (BorderStyle.Thin);
                style.TopBorderColor = (IndexedColors.Black.Index);

                // Style the cell with font color white
                IFont fontcell = workbook.CreateFont();
                fontcell.IsBold = true;
                style.SetFont(fontcell);

                IRow row = excelSheet.CreateRow(0);

                //row = excelSheet.CreateRow(1);
                ICell cell0 = row.CreateCell(0);
                cell0.SetCellValue("IP Number");
                excelSheet.SetColumnWidth(0, (int)((20 + 0.72) * 256));
                cell0.CellStyle = style;
                ICell cell1 = row.CreateCell(1);
                cell1.SetCellValue("IP Name");
                excelSheet.SetColumnWidth(1, (int)((30 + 0.72) * 256));
                cell1.CellStyle = style;
                ICell cell2 = row.CreateCell(2);
                cell2.SetCellValue("No of Days for which wages paid/payable during the month");
                cell2.CellStyle = style;
                excelSheet.SetColumnWidth(2, (int)((22 + 0.72) * 256));
                ICell cell3 = row.CreateCell(3);
                cell3.SetCellValue("Total Monthly Wages");
                cell3.CellStyle = style;
                ICell cell4 = row.CreateCell(4);
                cell4.SetCellValue("Reason Code For Zero Working Days");
                cell4.CellStyle = style;
                ICell cell5 = row.CreateCell(5);
                cell5.SetCellValue("Last Working Day");
                cell5.CellStyle = style;
                excelSheet.SetColumnWidth(5, (int)((22 + 0.72) * 256));

                List<ESICReportEmpWiseVM> ESICReportEmpWiseVMs = manager.ESICReportEmpWise(WAG_Id, firm.FRM_Id, selectionVMs.Where(m => m.IsSelect.Equals(true)).ToList(), IsSelected);
                int rowCount = 1;
                foreach (var item in ESICReportEmpWiseVMs)
                {
                    row = excelSheet.CreateRow(rowCount);
                    row.HeightInPoints = (float)(1.5 * excelSheet.DefaultRowHeightInPoints);
                    ICell cell_client = row.CreateCell(0);
                    cell_client.SetCellValue(item.NAME_OF_COMPANY);
                    cell_client.CellStyle = styleClient;
                    excelSheet.AddMergedRegion(new CellRangeAddress(rowCount, rowCount, 0, 5));
                    foreach (var emp in item.ESICReportVMs)
                    {
                        rowCount++;
                        row = excelSheet.CreateRow(rowCount);
                        row.CreateCell(0).SetCellValue(emp.IP_Number);
                        row.CreateCell(1).SetCellValue(emp.IP_Name);
                        row.CreateCell(2).SetCellValue(emp.PayableDays);

                        ICell cell_3 = row.CreateCell(3);
                        cell_3.SetCellValue(Convert.ToDouble(emp.TotalMonthlyWages));
                        cell_3.CellStyle = styleAmount;

                        ICell cell_5 = row.CreateCell(5);

                        row.CreateCell(4).SetCellValue(emp.ReasonCode);
                        cell_5.SetCellValue(emp.LastWorkingDay);
                    }
                    rowCount++;

                }                
                workbook.Write(fs);
            }
        }

        public void ESIC_Employees_Pending_For_Registration_Excel(Firms firm, List<SelectionVM> selectionVMs, int WAG_Id, string newPath, string fileName, string WAG_Month)
        {
            bool IsSelected = selectionVMs.Any(m => m.IsSelect.Equals(false));
            ReportsManager manager = new ReportsManager(_context);
            using (var fs = new FileStream(Path.Combine(newPath, fileName), FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook;
                workbook = new XSSFWorkbook();
                ISheet excelSheet = workbook.CreateSheet("ESIC. CON");
                #region single client
                IFont fontcell = workbook.CreateFont();
                fontcell.IsBold = true;
                IFont fontcellSub = workbook.CreateFont();
                fontcellSub.IsBold = true;
                fontcellSub.FontHeightInPoints = ((short)18);

                ICellStyle style = workbook.CreateCellStyle();
                ICellStyle styleClient = workbook.CreateCellStyle();
                ICellStyle styleSub = workbook.CreateCellStyle();


                styleClient.SetFont(fontcell);
                styleSub.SetFont(fontcellSub);

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

                IRow row = excelSheet.CreateRow(0);
                ICell CellSub = row.CreateCell(0);
                CellSub.SetCellValue(firm.FRM_Name.ToUpper());
                CellSub.CellStyle = styleSub;
                CellUtil.SetAlignment(CellSub, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 4));

                IRow rowAdd1 = excelSheet.CreateRow(1);
                ICell CellAdd1 = rowAdd1.CreateCell(0);
                CellAdd1.SetCellValue(firm.FRM_Address1.ToUpper()+","+ firm.FRM_Address2.ToUpper()+",");
                CellUtil.SetAlignment(CellAdd1, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 4));

                IRow rowSubHeading = excelSheet.CreateRow(2);
                ICell CellSubHeading = rowSubHeading.CreateCell(0);
                CellSubHeading.SetCellValue("LIST OF ESIC EMPLOYEES PENDING FOR REGISTRATION THE MONTH OF " + WAG_Month);
                CellSubHeading.CellStyle = styleClient;
                CellUtil.SetAlignment(CellSubHeading, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 4));

                row = excelSheet.CreateRow(3);
                row.HeightInPoints = (float)(3.2 * excelSheet.DefaultRowHeightInPoints);
                ICell cell0 = row.CreateCell(0);
                cell0.SetCellValue("SR.NO");
                cell0.CellStyle = style;
                ICell cell1 = row.CreateCell(1);
                cell1.SetCellValue("NAME OF COMPANY");
                excelSheet.SetColumnWidth(1, (int)((35 + 0.72) * 256));
                cell1.CellStyle = style;
                ICell cell2 = row.CreateCell(2);
                cell2.SetCellValue("NAME OF EMPLOYEE");
                excelSheet.SetColumnWidth(2, (int)((35 + 0.72) * 256));
                cell2.CellStyle = style;
                ICell cell3 = row.CreateCell(3);
                cell3.SetCellValue("PENDING \r\nREGISTRAION \r\nSINCE");
                excelSheet.SetColumnWidth(3, (int)((30 + 0.72) * 256));
                cell3.CellStyle = style;
                ICell cell4 = row.CreateCell(4);
                cell4.SetCellValue("REMARK FOR \r\nPENING ESIC \r\nREGISTRATION");
                excelSheet.SetColumnWidth(4, (int)((30 + 0.72) * 256));
                cell4.CellStyle = style;

                List<ESICReportVM> reportVM = manager.ESIC_Employees_Pending_For_Registration(WAG_Id, selectionVMs.Where(m => m.IsSelect.Equals(true)).ToList(), IsSelected);
                int rowCount = 4, srNo = 1;
                foreach (var item in reportVM)
                {
                    row = excelSheet.CreateRow(rowCount);
                    row.HeightInPoints = (float)(1.4 * excelSheet.DefaultRowHeightInPoints);
                    row.CreateCell(0).SetCellValue(Convert.ToString(srNo));
                    row.CreateCell(1).SetCellValue(Convert.ToString(item.NAME_OF_COMPANY));
                    row.CreateCell(2).SetCellValue(Convert.ToString(item.IP_Name));
                    row.CreateCell(3).SetCellValue(Convert.ToString(item.PENDING_REGISTRAION_SINCE));
                    row.CreateCell(4).SetCellValue(item.REMARKS);
                    rowCount++;
                    srNo++;
                }
                #endregion

                workbook.Write(fs);
            }

        }

        #endregion

        #region Labour WelfareFund
        public async Task<FileResult> Labour_WelfareFund_Excel(int WAG_Id)
        {
            ReportsManager manager = new ReportsManager(_context);
            WageProcessManager wageProcess = new WageProcessManager(_context);
            Wage_Process wage_Process = wageProcess.getWageProcessById(WAG_Id);
            DateTime WAG_Month = wage_Process.WAG_Month;
            string WAGMonth = WAG_Month.ToString("MMMM") + "-" + WAG_Month.ToString("yyyy");

            string newPath = ProjectUtils.GetTempFolderPath(_hostingEnvironment.WebRootPath);
            string fileName = "MLWF CONTRIBUTION " + DateTime.Now.ToString("ddMMyyyyHHmm") + "_" + WAGMonth + ".xlsx";
            //URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, fileName);
            //FileInfo file = new FileInfo(Path.Combine(newPath, fileName));
            var memory = new MemoryStream();
            using (var fs = new FileStream(Path.Combine(newPath, fileName), FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook;
                workbook = new XSSFWorkbook();
                ISheet excelSheet = workbook.CreateSheet("Template");
                IFont font = workbook.CreateFont();
                font.IsBold = true;
                font.FontHeightInPoints = ((short)25);
                font.FontName = ("Cambria");
                font.Underline = FontUnderlineType.Single;

                ICellStyle styleHeader = workbook.CreateCellStyle();
                styleHeader.SetFont(font);

                // Style the cell with font color white
                IFont fontcell = workbook.CreateFont();
                fontcell.IsBold = true;
                ICellStyle style = workbook.CreateCellStyle();
                ICellStyle styleBold = workbook.CreateCellStyle();
                ICellStyle Defaultstyle = workbook.CreateCellStyle();
                style.WrapText = true;
                Defaultstyle.WrapText = true;
                styleBold.SetFont(fontcell);

                style.VerticalAlignment = VerticalAlignment.Center;
                styleBold.VerticalAlignment = VerticalAlignment.Center;
                //#77bf2a

                // Style the cell with borders all around.
                style.BorderBottom = (BorderStyle.Thin);
                style.BottomBorderColor = (IndexedColors.Black.Index);
                style.BorderLeft = (BorderStyle.Thin);
                style.LeftBorderColor = (IndexedColors.Black.Index);
                style.BorderRight = (BorderStyle.Thin);
                style.RightBorderColor = (IndexedColors.Black.Index);
                style.BorderTop = (BorderStyle.Thin);
                style.TopBorderColor = (IndexedColors.Black.Index);
                style.SetFont(fontcell);

                IRow row = excelSheet.CreateRow(0);
                ICell CellHeader = row.CreateCell(0);
                CellHeader.SetCellValue(wage_Process.FRM_.FRM_Name.ToUpper());
                CellHeader.CellStyle = styleHeader;
                CellUtil.SetAlignment(CellHeader, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 7));


                IRow rowAdd1 = excelSheet.CreateRow(1);
                ICell CellAdd1 = rowAdd1.CreateCell(0);
                CellAdd1.SetCellValue(wage_Process.FRM_.FRM_Address1.ToUpper()+","+ wage_Process.FRM_.FRM_Address2.ToUpper()+",");
                CellUtil.SetAlignment(CellAdd1, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 7));


                IRow rowSubHeading = excelSheet.CreateRow(2);
                ICell CellSubHeading = rowSubHeading.CreateCell(0);
                CellSubHeading.SetCellValue("DETAILS OF MLWF CONTRIBUTION FOR THE MONTH OF " + WAG_Month.ToString("MMM-yyyy").ToUpper());
                CellSubHeading.CellStyle = styleBold;
                CellUtil.SetAlignment(CellSubHeading, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 7));

                List<MLWF_ContributionVM> MLWF_ContributionReports = manager.MLWF_ContributionReports(WAG_Id);

                row = excelSheet.CreateRow(3);
                row.HeightInPoints = (float)(4 * excelSheet.DefaultRowHeightInPoints);
                ICell cell0 = row.CreateCell(0);
                cell0.SetCellValue("SR. NO.");
                cell0.CellStyle = style;
                excelSheet.AddMergedRegion(new CellRangeAddress(3, 4, 0, 0));
                ICell cell1 = row.CreateCell(1);
                cell1.SetCellValue("NAME OF COMPANY");
                excelSheet.SetColumnWidth(1, (int)((30 + 0.72) * 256));//A
                cell1.CellStyle = style;
                excelSheet.AddMergedRegion(new CellRangeAddress(3, 4, 1, 1));
                ICell cell2 = row.CreateCell(2);
                cell2.SetCellValue("NO. OF  \r\n EMPLOYEE  \r\n BELOW "+ MLWF_ContributionReports[0].MLWF_Employee_Base + "/-");
                excelSheet.SetColumnWidth(2, (int)((15 + 0.72) * 256));
                cell2.CellStyle = style;
                excelSheet.AddMergedRegion(new CellRangeAddress(3, 4, 2, 2));
                ICell cell3 = row.CreateCell(3);
                cell3.SetCellValue("NO. OF  \r\n EMPLOYEE \r\n ABOVE "+ MLWF_ContributionReports[0].MLWF_Employee_Base + "/-");
                excelSheet.SetColumnWidth(3, (int)((15 + 0.72) * 256));
                cell3.CellStyle = style;
                excelSheet.AddMergedRegion(new CellRangeAddress(3, 4, 3, 3));
                ICell cell4 = row.CreateCell(4);
                cell4.SetCellValue("EMPLOYEE \r\n CONTR. BELOW \r\n "+ MLWF_ContributionReports[0].MLWF_Employee_Base + "/-");
                excelSheet.SetColumnWidth(4, (int)((16 + 0.72) * 256));
                cell4.CellStyle = style;
                ICell cell5 = row.CreateCell(5);
                cell5.SetCellValue("EMPLOYEE \r\n CONTR. ABOVE \r\n "+ MLWF_ContributionReports[0].MLWF_Employee_Base + "/-");
                excelSheet.SetColumnWidth(5, (int)((16 + 0.72) * 256));
                cell5.CellStyle = style;
                ICell cell6 = row.CreateCell(6);
                cell6.SetCellValue("EMPLOYER \r\n CONTR. BELOW \r\n "+ MLWF_ContributionReports[0].MLWF_Employer_Base + "/-");
                excelSheet.SetColumnWidth(6, (int)((16 + 0.72) * 256));
                cell6.CellStyle = style;
                ICell cell7 = row.CreateCell(7);
                cell7.SetCellValue("EMPLOYER \r\n CONTR. ABOVE  \r\n "+ MLWF_ContributionReports[0].MLWF_Employer_Base + "/-");
                excelSheet.SetColumnWidth(7, (int)((16 + 0.72) * 256));
                cell7.CellStyle = style;

                row = excelSheet.CreateRow(4);

                ICell cell44 = row.CreateCell(4);
                cell44.SetCellValue("RS. "+ MLWF_ContributionReports[0].CRI_MLWF_Employee_LThen + "/-");
                cell44.CellStyle = style;
                CellUtil.SetAlignment(cell44, workbook, (short)HorizontalAlignment.Center);
                ICell cell55 = row.CreateCell(5);
                cell55.SetCellValue("RS. "+ MLWF_ContributionReports[0].CRI_MLWF_Employee_GThen + "/-");
                cell55.CellStyle = style;
                CellUtil.SetAlignment(cell55, workbook, (short)HorizontalAlignment.Center);
                ICell cell66 = row.CreateCell(6);
                cell66.SetCellValue("RS. "+ MLWF_ContributionReports[0].CRI_MLWF_Employer_LThen + "/-");
                cell66.CellStyle = style;
                CellUtil.SetAlignment(cell66, workbook, (short)HorizontalAlignment.Center);
                ICell cell77 = row.CreateCell(7);
                cell77.SetCellValue("RS. "+ MLWF_ContributionReports[0].CRI_MLWF_Employer_GThen + "/-");
                cell77.CellStyle = style;
                CellUtil.SetAlignment(cell77, workbook, (short)HorizontalAlignment.Center);

               
                int rowCount = 5;
                int srNo = 1;
                int EMP_BELOW_3K = 0, EMP_ABOVE_3K = 0;
                decimal EMP_CONTR_BELOW_3K = 0M, EMP_CONTR_ABOVE_3K = 0M, EMPLOYER_CONTR_BELOW_3K = 0M, EMPLOYER_CONTR_ABOVE_3K = 0M;
                foreach (var item in MLWF_ContributionReports)
                {
                    row = excelSheet.CreateRow(rowCount);
                    row.CreateCell(0).SetCellValue(srNo++);
                    row.CreateCell(1).SetCellValue(item.CLI_Name);

                    ICell celL_2 = row.CreateCell(2);
                    celL_2.SetCellValue(item.EMP_BELOW_3K);
                    celL_2.CellStyle = Defaultstyle;
                    CellUtil.SetAlignment(celL_2, workbook, (short)HorizontalAlignment.Center);

                    ICell celL_3 = row.CreateCell(3);
                    celL_3.SetCellValue(item.EMP_ABOVE_3K);
                    celL_3.CellStyle = Defaultstyle;
                    CellUtil.SetAlignment(celL_3, workbook, (short)HorizontalAlignment.Center);

                    ICell celL_4 = row.CreateCell(4);
                    celL_4.SetCellValue(Convert.ToString(item.EMP_DEDUCTION_BELOW));
                    celL_4.CellStyle = Defaultstyle;
                    CellUtil.SetAlignment(celL_4, workbook, (short)HorizontalAlignment.Center);

                    ICell celL_5 = row.CreateCell(5);
                    celL_5.SetCellValue(Convert.ToString(item.EMP_DEDUCTION_ABOVE));
                    celL_5.CellStyle = Defaultstyle;
                    CellUtil.SetAlignment(celL_5, workbook, (short)HorizontalAlignment.Center);

                    ICell celL_6 = row.CreateCell(6);
                    celL_6.SetCellValue(Convert.ToString(item.EMPLOYER_CONTR_BELOW));
                    celL_6.CellStyle = Defaultstyle;
                    CellUtil.SetAlignment(celL_6, workbook, (short)HorizontalAlignment.Center);

                    ICell celL_7 = row.CreateCell(7);
                    celL_7.SetCellValue(Convert.ToString(item.EMPLOYER_CONTR_ABOVE));
                    celL_7.CellStyle = Defaultstyle;
                    CellUtil.SetAlignment(celL_7, workbook, (short)HorizontalAlignment.Center);

                    EMP_BELOW_3K = EMP_BELOW_3K + item.EMP_BELOW_3K;
                    EMP_ABOVE_3K = EMP_ABOVE_3K + item.EMP_ABOVE_3K;

                    EMP_CONTR_BELOW_3K = EMP_CONTR_BELOW_3K + (item.EMP_DEDUCTION_BELOW);
                    EMP_CONTR_ABOVE_3K = EMP_CONTR_ABOVE_3K + (item.EMP_DEDUCTION_ABOVE);
                    EMPLOYER_CONTR_BELOW_3K = EMPLOYER_CONTR_BELOW_3K + (item.EMPLOYER_CONTR_BELOW);
                    EMPLOYER_CONTR_ABOVE_3K = EMPLOYER_CONTR_ABOVE_3K + (item.EMPLOYER_CONTR_ABOVE);


                    rowCount++;
                }

                row = excelSheet.CreateRow(rowCount);
                ICell cellTotal = row.CreateCell(0);
                cellTotal.SetCellValue("TOTAL");
                cellTotal.CellStyle = styleBold;
                CellUtil.SetAlignment(cellTotal, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(rowCount, rowCount + 2, 0, 1));

                ICell cellTotal2 = row.CreateCell(2);
                cellTotal2.SetCellValue(EMP_BELOW_3K);
                cellTotal2.CellStyle = styleBold;
                CellUtil.SetAlignment(cellTotal2, workbook, (short)HorizontalAlignment.Center);

                ICell cellTotal3 = row.CreateCell(3);
                cellTotal3.SetCellValue(EMP_ABOVE_3K);
                cellTotal3.CellStyle = styleBold;
                CellUtil.SetAlignment(cellTotal3, workbook, (short)HorizontalAlignment.Center);

                ICell cellTotal4 = row.CreateCell(4);
                cellTotal4.SetCellValue(Convert.ToString(EMP_CONTR_BELOW_3K));
                cellTotal4.CellStyle = styleBold;
                CellUtil.SetAlignment(cellTotal4, workbook, (short)HorizontalAlignment.Center);

                ICell cellTotal5 = row.CreateCell(5);
                cellTotal5.SetCellValue(Convert.ToString(EMP_CONTR_ABOVE_3K));
                cellTotal5.CellStyle = styleBold;
                CellUtil.SetAlignment(cellTotal5, workbook, (short)HorizontalAlignment.Center);

                ICell cellTotal6 = row.CreateCell(6);
                cellTotal6.SetCellValue(Convert.ToString(EMPLOYER_CONTR_BELOW_3K));
                cellTotal6.CellStyle = styleBold;
                CellUtil.SetAlignment(cellTotal6, workbook, (short)HorizontalAlignment.Center);

                ICell cellTotal7 = row.CreateCell(7);
                cellTotal7.SetCellValue(Convert.ToString(EMPLOYER_CONTR_ABOVE_3K));
                cellTotal7.CellStyle = styleBold;
                CellUtil.SetAlignment(cellTotal7, workbook, (short)HorizontalAlignment.Center);

                row = excelSheet.CreateRow(rowCount + 1);
                ICell cellTotalEmp = row.CreateCell(2);
                cellTotalEmp.SetCellValue(EMP_BELOW_3K + EMP_ABOVE_3K);
                cellTotalEmp.CellStyle = styleBold;
                CellUtil.SetAlignment(cellTotalEmp, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(rowCount + 1, rowCount + 2, 2, 3));

                ICell cellEmpCont = row.CreateCell(4);
                cellEmpCont.SetCellValue(Convert.ToString(EMP_CONTR_BELOW_3K + EMP_CONTR_ABOVE_3K));
                cellEmpCont.CellStyle = styleBold;
                CellUtil.SetAlignment(cellEmpCont, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(rowCount + 1, rowCount + 1, 4, 5));


                ICell cellEmployerCont = row.CreateCell(6);
                cellEmployerCont.SetCellValue(Convert.ToString(EMPLOYER_CONTR_BELOW_3K + EMPLOYER_CONTR_ABOVE_3K));
                cellEmployerCont.CellStyle = styleBold;
                CellUtil.SetAlignment(cellEmployerCont, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(rowCount + 1, rowCount + 1, 6, 7));

                row = excelSheet.CreateRow(rowCount + 2);
                ICell cellFinal = row.CreateCell(4);
                cellFinal.SetCellValue(Convert.ToString(EMP_CONTR_BELOW_3K + EMP_CONTR_ABOVE_3K + EMPLOYER_CONTR_BELOW_3K + EMPLOYER_CONTR_ABOVE_3K));
                cellFinal.CellStyle = styleBold;
                CellUtil.SetAlignment(cellFinal, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(rowCount + 2, rowCount + 2, 4, 7));

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
        #endregion

        #region PaySlip
        public ActionResult PaySlipGenerate(int WAG_Id)
        {
            WageRegisterManager wageRegisterManager = new WageRegisterManager(_context);
            WageProcessManager wageProcessManager = new WageProcessManager(_context);
            Wage_Process wage_Process = wageProcessManager.getWageProcessById(WAG_Id);
            WagePaySlipMasterVM paySlipMasterVM = new WagePaySlipMasterVM();
            paySlipMasterVM.WAG_Id = WAG_Id;
            paySlipMasterVM.FRM_Id = wage_Process.FRM_Id; ;
            paySlipMasterVM.FRM_Name = wage_Process.FRM_.FRM_Name;
            paySlipMasterVM.WAG_Month = wage_Process.WAG_Month.ToString("MMM-yyyy");

            List<Employees> emps = wageRegisterManager.GetEmployeesForWage(WAG_Id).ToList();
            paySlipMasterVM.EmpPaySlipVMs = EmployeePaySlipMapper.mapMe(emps, WAG_Id);
            return View(paySlipMasterVM);
        }

        [Obsolete]
        [HttpGet]
        public async Task<IActionResult> GeneratedPaySlip(int WAG_Id, int EMP_Id, int WPS_Id)
        {            
            try
            {                
                Wage_PaySlips paySlip = new Wage_PaySlips();
                EmployeePaySlipVM paySlipVM = new EmployeePaySlipVM();
                ReportsManager reportsManager = new ReportsManager(_context);

                paySlipVM = reportsManager.GeneratePaySlip(WAG_Id, EMP_Id);
                string PaySlipPath = _configuration.GetSection("DEFAULT_FOLDER_PATH").Value + _configuration.GetSection("RMERP_EMPLOYEE_PAYSLIP_PATH").Value;
                var FileName = "PaySlip_"+ paySlipVM.EMP_Id.ToString("D5") + "_" + (paySlipVM.EMP_FirstName + "_" + paySlipVM.EMP_MiddleName + "_" + paySlipVM.EMP_SurName) + "_" + DateTime.Now.ToString("ddMMyyyy") + ".pdf";     

                var root = PaySlipPath + "\\" + WAG_Id + "\\" + "Salary Slip";
                if (!Directory.Exists(root))
                {
                    Directory.CreateDirectory(root);
                }
                else
                {
                    string[] fileList = Directory.GetFiles(root, FileName);
                    if (fileList != null)
                    {
                        foreach (string s in fileList)
                        {
                            string fileName = Path.GetFileName(s);
                            System.IO.File.Delete(root + "/" + fileName);
                        }
                    }
                }
                var path = Path.Combine(root, FileName);
                path = Path.GetFullPath(path);
                var view = new ViewAsPdf(paySlipVM)
                {
                    FileName = FileName,
                    SaveOnServerPath = path,
                    PageSize = Rotativa.AspNetCore.Options.Size.A4
                };
                #region Add Payslip
                paySlip.WPS_Id = WPS_Id;
                paySlip.WAG_Id = WAG_Id;
                paySlip.EMP_Id = EMP_Id;
                paySlip.WPS_Status = (int)ProjectUtils.WagePaySlip.Generated;
                paySlip.WPS_GeneratedOn = ProjectUtils.DateNow();                               
                paySlip.WPS_FileName = FileName;
                Wage_PaySlips wage_PaySlip = reportsManager.AddWagePaySlip(paySlip);
                #endregion
                var byteArray = await view.BuildFile(ControllerContext);
                GetFile(byteArray);                               
                return Json(new { data="ok"});
            }
            catch (Exception)
            {
                return Json(new { data = "ko" });
            }
        }    
        public FileResult GetFile(byte[] byteArray)
        {
            return File(byteArray, "application/pdf");
        }
        public async Task<FileResult> DownloadPaySlip(int WPS_Id)
        {
            ReportsManager reportsManager = new ReportsManager(_context);
            Wage_PaySlips paySlip = reportsManager.GetPaySlip(WPS_Id);
            string DocumentPath = _configuration.GetSection("DEFAULT_FOLDER_PATH").Value + _configuration.GetSection("RMERP_EMPLOYEE_PAYSLIP_PATH").Value;
            DocumentPath += "\\" + paySlip.WAG_Id + "\\" + "Salary Slip" + "\\" + paySlip.WPS_FileName;

            var memory = new MemoryStream();
            using (var stream = new FileStream(DocumentPath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, ProjectUtils.GetContentType(DocumentPath), paySlip.WPS_FileName);
        }

        #endregion
    }

}



