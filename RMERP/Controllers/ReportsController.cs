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
                CellHeader.SetCellValue("PF SALEX TAX "+ WAG_Month);
                CellHeader.CellStyle = styleHeader;
                CellUtil.SetAlignment(CellHeader, workbook, (short)HorizontalAlignment.Center);
                excelSheet.AddMergedRegion(new CellRangeAddress(0, 0, 0,10));
         

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
                    Byte[] title = new UTF8Encoding(true).GetBytes(item.EMP_UAN_Number + "#~#"+ item.EMP_FullName+ "#~#"+ item.GrossWages +"#~#" + item.EPFWages + "#~#" + item.EPSWages+ "#~#"+ item.EDLIWages);
                    fs.Write(title, 0, title.Length);
                    byte[] br = new UTF8Encoding(true).GetBytes(item.EPF_CONTRI_REMITTED + "#~#" + item.EPS_CONTRI_REMITTED + "#~#" + item.EPF_EPS_DIFF_REMITTED + "#~#" );                       
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
                    cell_5.SetCellValue(Convert.ToDateTime(item.LastWorkingDay)+"");
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


    }
}