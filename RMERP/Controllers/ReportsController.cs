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
using NPOI.HSSF.UserModel;
using System.Drawing;

namespace RMERP.Controllers
{
    public class ReportsController : Controller
    {
        private readonly RMERPContext _context;
        private IHostingEnvironment _hostingEnvironment;
        public ReportsController(RMERPContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }
        public IActionResult PFReport(int WAG_Id)
        {
            ReportsManager manager = new ReportsManager(_context);
            return View(manager.PFReports(WAG_Id));
        }

        public FileResult PFReportExcel(int WAG_Id)
        {
            ReportsManager manager = new ReportsManager(_context);
            WageProcessManager wageProcess = new WageProcessManager(_context);
            DateTime wageMonth = wageProcess.getWageProcessById(WAG_Id).WAG_Month;
            string WAG_Month = wageMonth.ToString("MMMM") + "_" + wageMonth.ToString("yyyy");

            string newPath = ProjectUtils.GetTempFolderPath(_hostingEnvironment.WebRootPath);
            string fileName = "Template_" + DateTime.Now.ToString("ddMMyyyyHHmm") + "_" + WAG_Month + "_PFReport.xlsx";
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
                font.FontHeightInPoints = ((short)24);
                font.FontName = ("Cambria");
                ICellStyle styleHeader = workbook.CreateCellStyle();
                styleHeader.FillBackgroundColor = HSSFColor.BlueGrey.Index;
                styleHeader.SetFont(font);

                // Grey25Percent background
                ICellStyle style = workbook.CreateCellStyle();
                style.FillForegroundColor = IndexedColors.Blue.Index;
                style.FillPattern = FillPattern.SolidForeground;
                style.FillBackgroundColor = IndexedColors.Blue.Index;

                // Style the cell with borders all around.
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
                fontcell.Color = (IndexedColors.White.Index);
                fontcell.IsBold = true;
                style.SetFont(fontcell);

                IRow row = excelSheet.CreateRow(0);
                ICell CellHeader = row.CreateCell(0);
                CellHeader.SetCellValue("PF SALEX TAX " + WAG_Month);
                CellHeader.CellStyle = styleHeader;
                CellUtil.SetAlignment(CellHeader, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 10));


                row = excelSheet.CreateRow(1);
                //row.HeightInPoints = ((5 * excelSheet.DefaultRowHeightInPoints));                
                //excelSheet.AutoSizeColumn(1);
                ICell cell0 = row.CreateCell(0);
                cell0.SetCellValue("UAN");
                cell0.CellStyle = style;
                ICell cell1 = row.CreateCell(1);
                cell1.SetCellValue("MEMBER NAME");
                cell1.CellStyle = style;
                ICell cell2 = row.CreateCell(2);
                cell2.SetCellValue("GROSS WAGES");
                cell2.CellStyle = style;
                ICell cell3 = row.CreateCell(3);
                cell3.SetCellValue("EPF WAGES");
                cell3.CellStyle = style;
                ICell cell4 = row.CreateCell(4);
                cell4.SetCellValue("EPS WAGES");
                cell4.CellStyle = style;
                ICell cell5 = row.CreateCell(5);
                cell5.SetCellValue("EDLI WAGES");
                cell5.CellStyle = style;
                ICell cell6 = row.CreateCell(6);
                cell6.SetCellValue("EPF CONTRI REMITTED");
                cell6.CellStyle = style;
                ICell cell7 = row.CreateCell(7);
                cell7.SetCellValue("EPS CONTRI REMITTED");
                cell7.CellStyle = style;
                ICell cell8 = row.CreateCell(8);
                cell8.SetCellValue("EPF_EPS_DIFF_REMITTED");
                cell8.CellStyle = style;
                ICell cell9 = row.CreateCell(9);
                cell9.SetCellValue("NCP DAYS");
                cell9.CellStyle = style;
                ICell cell10 = row.CreateCell(10);
                cell10.SetCellValue("REFUND OF ADVANCES");
                cell10.CellStyle = style;

                List<PFReportVM> PFReportVMs = manager.PFReports(WAG_Id);
                int rowCount = 2;
                foreach (var item in PFReportVMs)
                {
                    row = excelSheet.CreateRow(rowCount);
                    row.HeightInPoints = (float)(1.5 * excelSheet.DefaultRowHeightInPoints);
                    row.CreateCell(0).SetCellValue(item.EMP_UAN_Number);
                    row.CreateCell(1).SetCellValue(item.EMP_FullName);
                    row.CreateCell(2).SetCellValue(Convert.ToDouble(item.GrossWages));
                    row.CreateCell(3).SetCellValue(Convert.ToDouble(item.EPFWages));
                    row.CreateCell(4).SetCellValue(Convert.ToDouble(item.EPSWages));
                    row.CreateCell(5).SetCellValue(Convert.ToDouble(item.EDLIWages));
                    row.CreateCell(6).SetCellValue(Convert.ToDouble(item.EPF_CONTRI_REMITTED));
                    row.CreateCell(7).SetCellValue(Convert.ToDouble(item.EPS_CONTRI_REMITTED));
                    row.CreateCell(8).SetCellValue(Convert.ToDouble(item.EPF_EPS_DIFF_REMITTED));
                    row.CreateCell(9).SetCellValue(Convert.ToDouble(item.NCP_DAYS));
                    row.CreateCell(10).SetCellValue(Convert.ToDouble(item.REFUND_OF_ADVANCES));
                    rowCount++;
                }
                workbook.Write(fs);
            }
            using (var stream = new FileStream(Path.Combine(newPath, fileName), FileMode.Open))
            {
                stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            new FileInfo(Path.Combine(newPath, fileName)).Delete();
            return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        public async Task<FileResult> PFReportText(int WAG_Id)
        {
            ReportsManager manager = new ReportsManager(_context);
            WageProcessManager wageProcess = new WageProcessManager(_context);
            DateTime wageMonth = wageProcess.getWageProcessById(WAG_Id).WAG_Month;
            string WAG_Month = wageMonth.ToString("MMMM") + "_" + wageMonth.ToString("yyyy");

            string newPath = ProjectUtils.GetTempFolderPath(_hostingEnvironment.WebRootPath);
            string fileName = "PF_SALEX_TAX_" + DateTime.Now.ToString("ddMMyyyyHHmm") + "_" + WAG_Month + "_PFReport.txt";
            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, fileName);
            FileInfo file = new FileInfo(Path.Combine(newPath, fileName));
            var memory = new MemoryStream();
            using (var fs = new FileStream(Path.Combine(newPath, fileName), FileMode.Create, FileAccess.Write))
            {
                List<PFReportVM> PFReportVMs = manager.PFReports(WAG_Id);
                int rowCount = 2;
                foreach (var item in PFReportVMs)
                {
                    Byte[] title = new UTF8Encoding(true).GetBytes(item.EMP_UAN_Number + "#~#" + item.EMP_FullName + "#~#" + item.GrossWages + "#~#" + item.EPFWages + "#~#" + item.EPSWages + "#~#" + item.EDLIWages);
                    fs.Write(title, 0, title.Length);
                    byte[] br = new UTF8Encoding(true).GetBytes(item.EPF_CONTRI_REMITTED + "#~#" + item.EPS_CONTRI_REMITTED + "#~#" + item.EPF_EPS_DIFF_REMITTED + "#~#");
                    fs.Write(br, 0, br.Length);
                    byte[] author = new UTF8Encoding(true).GetBytes(item.NCP_DAYS + "#~#" + item.REFUND_OF_ADVANCES);
                    fs.Write(author, 0, author.Length);
                    byte[] br2 = new UTF8Encoding(true).GetBytes("\r\n");
                    fs.Write(br2, 0, br2.Length);
                    rowCount++;
                }
            }
            using (var stream = new FileStream(Path.Combine(newPath, fileName), FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            new FileInfo(Path.Combine(newPath, fileName)).Delete();
            return File(memory, "application/rtf", fileName);
        }

        public async Task<FileResult> ESICReportExcel(int WAG_Id)
        {
            ReportsManager manager = new ReportsManager(_context);
            WageProcessManager wageProcess = new WageProcessManager(_context);
            DateTime wageMonth = wageProcess.getWageProcessById(WAG_Id).WAG_Month;
            string WAG_Month = wageMonth.ToString("MMMM") + "_" + wageMonth.ToString("yyyy");

            string newPath = ProjectUtils.GetTempFolderPath(_hostingEnvironment.WebRootPath);
            string fileName = "Template_" + DateTime.Now.ToString("ddMMyyyyHHmm") + "_" + WAG_Month + "_ESICReport.xlsx";
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
                font.FontHeightInPoints = ((short)22);
                font.FontName = ("Cambria");
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

                // Grey25Percent background
                ICellStyle style = workbook.CreateCellStyle();
                style.FillForegroundColor = IndexedColors.Aqua.Index;
                style.FillPattern = FillPattern.SolidForeground;
                style.FillBackgroundColor = IndexedColors.Aqua.Index;


                // Style the cell with borders all around.
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
                ICell CellHeader = row.CreateCell(0);
                CellHeader.SetCellValue("ESIC SALEX TAX " + WAG_Month);
                CellHeader.CellStyle = styleHeader;
                CellUtil.SetAlignment(CellHeader, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 6));


                row = excelSheet.CreateRow(1);
                ICell cell0 = row.CreateCell(0);
                cell0.SetCellValue("IP Number");
                cell0.CellStyle = style;
                ICell cell1 = row.CreateCell(1);
                cell1.SetCellValue("IP Name");
                cell1.CellStyle = style;
                ICell cell2 = row.CreateCell(2);
                cell2.SetCellValue("No of Days for which wages paid/payable during the month");
                cell2.CellStyle = style;
                ICell cell3 = row.CreateCell(3);
                cell3.SetCellValue("Total Monthly Wages");
                cell3.CellStyle = style;
                ICell cell4 = row.CreateCell(4);
                cell4.SetCellValue("Reason Code For Zero Working Days");
                cell4.CellStyle = style;
                ICell cell5 = row.CreateCell(5);
                cell5.SetCellValue("Last Working Day");
                cell5.CellStyle = style;

                List<ESICReportVM> ESICReportVMS = manager.ESICReports(WAG_Id);
                int rowCount = 2;
                foreach (var item in ESICReportVMS)
                {
                    row = excelSheet.CreateRow(rowCount);
                    row.HeightInPoints = (float)(1.5 * excelSheet.DefaultRowHeightInPoints);
                    row.CreateCell(0).SetCellValue(item.IP_Number);
                    row.CreateCell(1).SetCellValue(item.IP_Name);
                    row.CreateCell(2).SetCellValue(item.PayableDays);

                    ICell cell_3 = row.CreateCell(3);
                    cell_3.SetCellValue(Convert.ToDouble(item.TotalMonthlyWages));
                    cell_3.CellStyle = styleAmount;

                    ICell cell_5 = row.CreateCell(5);
                    IDataFormat dataFormatCustom = workbook.CreateDataFormat();
                    cell_5.CellStyle.DataFormat = dataFormatCustom.GetFormat("yyyy-MM-dd");

                    row.CreateCell(4).SetCellValue(item.ReasonCode);
                    cell_5.SetCellValue(Convert.ToDateTime(item.LastWorkingDay) + "");
                    rowCount++;
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

        public async Task<FileResult> ESICReportText(int WAG_Id)
        {
            ReportsManager manager = new ReportsManager(_context);
            WageProcessManager wageProcess = new WageProcessManager(_context);
            DateTime wageMonth = wageProcess.getWageProcessById(WAG_Id).WAG_Month;
            string WAG_Month = wageMonth.ToString("MMMM") + "_" + wageMonth.ToString("yyyy");

            string newPath = ProjectUtils.GetTempFolderPath(_hostingEnvironment.WebRootPath);
            string fileName = "ESIC_SALEX_TAX_" + DateTime.Now.ToString("ddMMyyyyHHmm") + "_" + WAG_Month + "_ESICReport.txt";
            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, fileName);
            FileInfo file = new FileInfo(Path.Combine(newPath, fileName));
            var memory = new MemoryStream();
            using (var fs = new FileStream(Path.Combine(newPath, fileName), FileMode.Create, FileAccess.Write))
            {
                List<ESICReportVM> ESICReportVMs = manager.ESICReports(WAG_Id);
                int rowCount = 2;
                foreach (var item in ESICReportVMs)
                {
                    Byte[] title = new UTF8Encoding(true).GetBytes(item.IP_Number + "#~#" + item.IP_Name + "#~#" + item.PayableDays + "#~#" + item.TotalMonthlyWages + "#~#" + item.ReasonCode + "#~#" + item.LastWorkingDay);
                    fs.Write(title, 0, title.Length);
                    byte[] br2 = new UTF8Encoding(true).GetBytes("\r\n");
                    fs.Write(br2, 0, br2.Length);
                    rowCount++;
                }
            }
            using (var stream = new FileStream(Path.Combine(newPath, fileName), FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            new FileInfo(Path.Combine(newPath, fileName)).Delete();
            return File(memory, "application/rtf", fileName);
        }

        public async Task<FileResult> NEFT_BankReportExcel(int WAG_Id)
        {
            ReportsManager manager = new ReportsManager(_context);
            WageProcessManager wageProcess = new WageProcessManager(_context);
            DateTime wageMonth = wageProcess.getWageProcessById(WAG_Id).WAG_Month;
            string WAG_Month = wageMonth.ToString("MMMM") + "-" + wageMonth.ToString("yyyy");

            string newPath = ProjectUtils.GetTempFolderPath(_hostingEnvironment.WebRootPath);
            string fileName = "NEFT_BankReport_" + DateTime.Now.ToString("ddMMyyyyHHmm") + "_" + WAG_Month + ".xlsx";
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
                CellSub.SetCellValue("SECURITY SERVICES");
                CellSub.CellStyle = styleSub;
                CellUtil.SetAlignment(CellSub, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 6));

                IRow rowAdd1 = excelSheet.CreateRow(2);
                ICell CellAdd1 = rowAdd1.CreateCell(0);
                CellAdd1.SetCellValue("G-9, Malti Tower, ‘E’ Ward, Near Kiran Bungalow, Tarabai Park, Kolhapur – 416 003,");
                CellUtil.SetAlignment(CellAdd1, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 6));

                IRow rowAdd2 = excelSheet.CreateRow(3);
                ICell CellAdd2 = rowAdd2.CreateCell(0);
                CellAdd2.SetCellValue("Ph.- 0231-2666389. Mobile : 9922967130. E-mail : reliable.manpower@yahoo.com");
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

                List<NEFT_BankReportVM> NEFT_BankReports = manager.NEFT_BankReports(WAG_Id);
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
            using (var stream = new FileStream(Path.Combine(newPath, fileName), FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            new FileInfo(Path.Combine(newPath, fileName)).Delete();
            return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        public async Task<FileResult> MLWF_ContributionExcel(int WAG_Id)
        {
            ReportsManager manager = new ReportsManager(_context);
            WageProcessManager wageProcess = new WageProcessManager(_context);
            DateTime wageMonth = wageProcess.getWageProcessById(WAG_Id).WAG_Month;
            string WAG_Month = wageMonth.ToString("MMMM") + "-" + wageMonth.ToString("yyyy");

            string newPath = ProjectUtils.GetTempFolderPath(_hostingEnvironment.WebRootPath);
            string fileName = "MLWF CONTRIBUTION " + DateTime.Now.ToString("ddMMyyyyHHmm") + "_" + WAG_Month + ".xlsx";
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
                style.WrapText = true;
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
                CellHeader.SetCellValue("RELIABLE SECURITY SERVICES");
                CellHeader.CellStyle = styleHeader;
                CellUtil.SetAlignment(CellHeader, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 7));


                IRow rowAdd1 = excelSheet.CreateRow(1);
                ICell CellAdd1 = rowAdd1.CreateCell(0);
                CellAdd1.SetCellValue("G-9, MALTI TOWER, TARABAI PARK, KOLHAPUR");
                CellUtil.SetAlignment(CellAdd1, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 7));


                IRow rowSubHeading = excelSheet.CreateRow(2);
                ICell CellSubHeading = rowSubHeading.CreateCell(0);
                CellSubHeading.SetCellValue("DETAILS OF MLWF CONTRIBUTION FOR THE MONTH OF " + WAG_Month);
                CellSubHeading.CellStyle = styleBold;
                CellUtil.SetAlignment(CellSubHeading, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 7));


                row = excelSheet.CreateRow(3);
                row.HeightInPoints = (float)(4 * excelSheet.DefaultRowHeightInPoints);
                ICell cell0 = row.CreateCell(0);
                cell0.SetCellValue("SR. NO.");
                cell0.CellStyle = style;
                excelSheet.AddMergedRegion(new CellRangeAddress(3, 4, 0, 0));
                ICell cell1 = row.CreateCell(1);
                cell1.SetCellValue("NAME OF COMPANY");
                excelSheet.SetColumnWidth(1, (int)((25 + 0.72) * 256));//A
                cell1.CellStyle = style;
                excelSheet.AddMergedRegion(new CellRangeAddress(3, 4, 1, 1));
                ICell cell2 = row.CreateCell(2);
                cell2.SetCellValue("NO. OF EMP. \r\n BELOW 3000/-");
                cell2.CellStyle = style;
                excelSheet.AddMergedRegion(new CellRangeAddress(3, 4, 2, 2));
                ICell cell3 = row.CreateCell(3);
                cell3.SetCellValue("NO. OF EMP. \r\n ABOVE 3000/-");
                cell3.CellStyle = style;
                excelSheet.AddMergedRegion(new CellRangeAddress(3, 4, 3, 3));
                ICell cell4 = row.CreateCell(4);
                cell4.SetCellValue("EMP. CONTR.\r\n BELOW 3000/-");
                cell4.CellStyle = style;
                ICell cell5 = row.CreateCell(5);
                cell5.SetCellValue("EMP. CONTR. \r\n ABOVE 3000/-");
                cell5.CellStyle = style;
                ICell cell6 = row.CreateCell(6);
                cell6.SetCellValue("EMPLOYER CONTR. \r\n BELOW 3000/-");
                cell6.CellStyle = style;
                ICell cell7 = row.CreateCell(7);
                cell7.SetCellValue("EMPLOYER CONTR. \r\n ABOVE 3000/-");
                cell7.CellStyle = style;

                row = excelSheet.CreateRow(4);

                ICell cell44 = row.CreateCell(4);
                cell44.SetCellValue("RS. 6/-");
                CellUtil.SetAlignment(cell44, workbook, (short)HorizontalAlignment.Center);
                cell44.CellStyle = style;
                ICell cell55 = row.CreateCell(5);
                CellUtil.SetAlignment(cell55, workbook, (short)HorizontalAlignment.Center);
                cell55.SetCellValue("RS. 12/-");
                cell55.CellStyle = style;
                ICell cell66 = row.CreateCell(6);
                CellUtil.SetAlignment(cell66, workbook, (short)HorizontalAlignment.Center);
                cell66.SetCellValue("RS. 18/-");
                cell66.CellStyle = style;
                ICell cell77 = row.CreateCell(7);
                CellUtil.SetAlignment(cell77, workbook, (short)HorizontalAlignment.Center);
                cell77.SetCellValue("RS. 36/-");
                cell77.CellStyle = style;

                List<MLWF_ContributionVM> MLWF_ContributionReports = manager.MLWF_ContributionReports(WAG_Id);
                int rowCount = 5;
                int srNo = 1;
                int EMP_BELOW_3K = 0, EMP_ABOVE_3K = 0;
                decimal EMP_CONTR_BELOW_3K = 0M, EMP_CONTR_ABOVE_3K = 0M, EMPLOYER_CONTR_BELOW_3K = 0M, EMPLOYER_CONTR_ABOVE_3K = 0M;
                foreach (var item in MLWF_ContributionReports)
                {
                    row = excelSheet.CreateRow(rowCount);
                    row.CreateCell(0).SetCellValue(srNo++);
                    row.CreateCell(1).SetCellValue(item.CLI_Name);
                    row.CreateCell(2).SetCellValue(item.EMP_BELOW_3K);
                    row.CreateCell(3).SetCellValue(item.EMP_ABOVE_3K);
                    row.CreateCell(4).SetCellValue(Convert.ToString(item.EMP_CONTR_BELOW_3K()));
                    row.CreateCell(5).SetCellValue(Convert.ToString(item.EMP_CONTR_ABOVE_3K()));
                    row.CreateCell(6).SetCellValue(Convert.ToString(item.EMPLOYER_CONTR_BELOW_3K()));
                    row.CreateCell(7).SetCellValue(Convert.ToString(item.EMPLOYER_CONTR_ABOVE_3K()));

                    EMP_BELOW_3K = EMP_BELOW_3K + item.EMP_BELOW_3K;
                    EMP_ABOVE_3K = EMP_ABOVE_3K + item.EMP_ABOVE_3K;
                    EMP_CONTR_BELOW_3K = EMP_CONTR_BELOW_3K + (item.EMP_CONTR_BELOW_3K());
                    EMP_CONTR_ABOVE_3K = EMP_CONTR_ABOVE_3K + (item.EMP_CONTR_ABOVE_3K());
                    EMPLOYER_CONTR_BELOW_3K = EMPLOYER_CONTR_BELOW_3K + (item.EMPLOYER_CONTR_BELOW_3K());
                    EMPLOYER_CONTR_ABOVE_3K= EMPLOYER_CONTR_ABOVE_3K + (item.EMPLOYER_CONTR_ABOVE_3K());

                    rowCount++;
                }

                row = excelSheet.CreateRow(rowCount);
                ICell cellTotal = row.CreateCell(0);
                cellTotal.SetCellValue("TOTAL");
                cellTotal.CellStyle = styleBold;
                CellUtil.SetAlignment(cellTotal, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(rowCount, rowCount+2, 0, 1));

                ICell cellTotal2 = row.CreateCell(2);
                cellTotal2.SetCellValue(EMP_BELOW_3K);
                cellTotal2.CellStyle = styleBold;

                ICell cellTotal3 = row.CreateCell(3);
                cellTotal3.SetCellValue(EMP_ABOVE_3K);
                cellTotal3.CellStyle = styleBold;

                ICell cellTotal4 = row.CreateCell(4);
                cellTotal4.SetCellValue(Convert.ToString(EMP_CONTR_BELOW_3K));
                cellTotal4.CellStyle = styleBold;

                ICell cellTotal5 = row.CreateCell(5);
                cellTotal5.SetCellValue(Convert.ToString(EMP_CONTR_ABOVE_3K));
                cellTotal5.CellStyle = styleBold;

                ICell cellTotal6 = row.CreateCell(6);
                cellTotal6.SetCellValue(Convert.ToString(EMPLOYER_CONTR_BELOW_3K));
                cellTotal6.CellStyle = styleBold;

                ICell cellTotal7 = row.CreateCell(7);
                cellTotal7.SetCellValue(Convert.ToString(EMPLOYER_CONTR_ABOVE_3K));
                cellTotal7.CellStyle = styleBold;

                row = excelSheet.CreateRow(rowCount+1);
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


    }
}