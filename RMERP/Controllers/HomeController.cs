using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Office.Interop.Excel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using RMERP.DAL.Models;
using Excel = Microsoft.Office.Interop.Excel;

using DataTable = System.Data.DataTable;
using NPOI.SS.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using RMERP.DAL.ViewModel;
//using Microsoft.Office.Interop.Excel;

namespace RMERP.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private RMERPContext _context;
        private IWebHostEnvironment _hostingEnvironment;
        public HomeController(RMERPContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            var temp = _context.AdminUsers.ToList();
            ViewData["Message"] = "Your application description page.";
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            ErrorVM error = new();
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();
            if (exceptionFeature != null)
            {
                error.ErrorMessage = exceptionFeature.Error.Message;
                error.InnerException = exceptionFeature.Error.InnerException?.Message;
            }
            return View(error);
        }
        public ActionResult ExcelDownload()
        {
            return View();
        }
        public ActionResult CreateDocument1()
        {
            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            string sFileName = @"demo.xlsx";
            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            var memory = new MemoryStream();
            using (var fs = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook;
                workbook = new XSSFWorkbook();
                ISheet excelSheet = workbook.CreateSheet("Demo");
                IRow row = excelSheet.CreateRow(0);

                row.CreateCell(0).SetCellValue("ID");
                row.CreateCell(1).SetCellValue("Name");
                row.CreateCell(2).SetCellValue("Age");

                CellRangeAddress region = new CellRangeAddress(0, 5, 0, 5);
                excelSheet.AddMergedRegion(region);



                row = excelSheet.CreateRow(1);
                ICell cell = row.CreateCell(1);
                cell.SetCellValue(new XSSFRichTextString("This is a test of merging"));

                excelSheet.AddMergedRegion(new CellRangeAddress(1, 1, 1, 2));
                ICell cell2 = row.CreateCell(3);
                cell2.SetCellValue(new XSSFRichTextString("ok This is a test of merging"));

                excelSheet.AddMergedRegion(new CellRangeAddress(1, 1, 3, 5));

                workbook.Write(fs);
            }
            using (var stream = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open))
            {
                stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
        }

        public ActionResult CreateDocument()
        {
            string csv = string.Empty;
            csv += " Name,";
            csv += " xxxName";
            csv += "\r\n";
            csv += "r,";
            csv += "xxr";
            csv += "\r\n";
            byte[] bytes = Encoding.ASCII.GetBytes(csv);

            Response.Headers["Content-Disposition"] = new ContentDispositionHeaderValue("attachment")
            {
                FileName = "demofile.xls"
            }.ToString();

            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "demofile.xls");

        }

        //public ActionResult ExportToExcel()
        //{
        //    string reportPath = "your file path for excel";
        //    string reportName = "YourReport.xlsb";



        //    Application excelApp =new Excel.Application();

        //    //Create an Excel workbook instance 
        //    Microsoft.Office.Interop.Excel.Workbook excelWorkBook =
        //    excelApp.Workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);

        //    Worksheet excelWorkSheet = excelWorkBook.ActiveSheet;
        //    excelWorkSheet.Name = Convert.ToString(table.TableName);
        //    excelWorkSheet.Columns.AutoFit();

        //    for (int i = 1; i < table.Columns.Count + 1; i++)
        //    {
        //        excelWorkSheet.Cells[1, i] = table.Columns[i - 1].ColumnName;
        //    }

        //    for (int j = 0; j < table.Rows.Count; j++)
        //    {
        //        for (int k = 0; k < table.Columns.Count; k++)
        //        {
        //            excelWorkSheet.Cells[j + 2, k + 1] = table.Rows[j].ItemArray[k].ToString();
        //        }
        //    }

        //    //-- check file directory is present or not/if note create new
        //    bool exists = System.IO.Directory.Exists(reportPath);
        //    if (!exists)
        //    {
        //        System.IO.Directory.CreateDirectory(reportPath);
        //    }

        //    excelWorkBook.SaveAs(reportPath + reportName,
        //    XlFileFormat.xlExcel12, Type.Missing, Type.Missing,
        //    Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive,
        //    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
        //    excelWorkBook.Close();
        //    excelApp.Quit();

        //    return File(reportPath + reportName, "application/vnd.ms-excel", reportName);
        //}
        public static string filePath = @"c:\ExcelSample\Rinkup.xlsx";
        public ActionResult downloadExcel()
        {
            Excel.Application xlApp = new Excel.Application();

            if (xlApp == null)
            {
                //Console.WriteLine("Excel is not installed in the system...");
                return Content("Excel is not installed in the system...");
            }

            object misValue = System.Reflection.Missing.Value;

            Excel.Workbook xlWorkBook = xlApp.Workbooks.Add(misValue);
            Excel.Worksheet xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            xlWorkSheet.Cells[1, 1] = "ID";
            xlWorkSheet.Cells[1, 2] = "Name";
            xlWorkSheet.Cells[2, 1] = "1001";
            xlWorkSheet.Cells[2, 2] = "Ramakrishna";
            xlWorkSheet.Cells[3, 1] = "1002";
            xlWorkSheet.Cells[3, 2] = "Praveenkumar";
            //xlWorkBook.Sheets.Add(xlWorkSheet);
            xlWorkBook.SaveAs(filePath, Excel.XlFileFormat.xlOpenXMLWorkbook, misValue, misValue, misValue, misValue,
                Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);

            xlWorkBook.Close();
            xlApp.Quit();

            Marshal.ReleaseComObject(xlWorkSheet);
            Marshal.ReleaseComObject(xlWorkBook);
            Marshal.ReleaseComObject(xlApp);

            return Content("Done");
        }
        public static System.Data.DataTable ConvertCSVtoDataTable(string strFilePath)
        {
            DataTable dt = new DataTable();
            using (StreamReader sr = new StreamReader(strFilePath))
            {
                string[] headers = sr.ReadLine().Split(',');
                foreach (string header in headers)
                {
                    dt.Columns.Add(header);
                }

                while (!sr.EndOfStream)
                {
                    string[] rows = sr.ReadLine().Split(',');
                    if (rows.Length > 1)
                    {
                        DataRow dr = dt.NewRow();
                        for (int i = 0; i < headers.Length; i++)
                        {
                            dr[i] = rows[i].Trim();
                        }
                        dt.Rows.Add(dr);
                    }
                }

            }


            return dt;
        }

        public ActionResult ExportToExcel1()
        {

            // Load Excel application
            Application excel = new Application();

            // Create empty workbook
            excel.Workbooks.Add();

            // Create Worksheet from active sheet
            _Worksheet workSheet = (Excel.Worksheet)excel.ActiveSheet;

            // I created Application and Worksheet objects before try/catch,
            // so that i can close them in finnaly block.
            // It's IMPORTANT to release these COM objects!!
            try
            {
                // ------------------------------------------------
                // Creation of header cells
                // ------------------------------------------------
                workSheet.Cells[1, "A"] = "Name";
                workSheet.Cells[1, "B"] = "Color";
                workSheet.Cells[1, "C"] = "Maximum speed";

                // ------------------------------------------------
                // Populate sheet with some real data from "cars" list
                // ------------------------------------------------
                int row = 2; // start row (in row 1 are header cells)
                             //foreach (Car car in cars)
                             //{
                workSheet.Cells[row, "A"] = "abc";
                workSheet.Cells[row, "B"] = "red";
                workSheet.Cells[row, "C"] = string.Format("{0} km/h", 50);

                // row++;
                // }

                // Apply some predefined styles for data to look nicely :)
                workSheet.Range["A1"].AutoFormat(XlRangeAutoFormat.xlRangeAutoFormatClassic1);

                // Define filename
                string fileName = string.Format(filePath, Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));

                // Save this data as a file
                workSheet.SaveAs(fileName);

                // Display SUCCESS message
                //   MessageBox.Show(string.Format("The file '{0}' is saved successfully!", fileName));
                return Content("done");
            }
            catch (Exception exception)
            {
                return Content(exception.Message);
            }
            finally
            {
                //// Quit Excel application
                //excel.Quit();

                //// Release COM objects (very important!)
                //if (excel != null)
                //    System.Runtime.InteropServices.Marshal.ReleaseComObject(excel);

                //if (workSheet != null)
                //    System.Runtime.InteropServices.Marshal.ReleaseComObject(workSheet);

                //// Empty variables
                //excel = null;
                //workSheet = null;

                //// Force garbage collector cleaning
                //GC.Collect();
            }
        }
        public ActionResult excelD()
        {
            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();

            if (xlApp == null)
            {
                return Content("");
            }


            Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
            Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;

            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            xlWorkSheet.Cells[1, 1] = "ID";
            xlWorkSheet.Cells[1, 2] = "Name";
            xlWorkSheet.Cells[2, 1] = "1";
            xlWorkSheet.Cells[2, 2] = "One";
            xlWorkSheet.Cells[3, 1] = "2";
            xlWorkSheet.Cells[3, 2] = "Two";

            //Here saving the file in xlsx
            xlWorkBook.SaveAs(filePath, Microsoft.Office.Interop.Excel.XlFileFormat.xlOpenXMLWorkbook, misValue,
            misValue, misValue, misValue, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);


            xlWorkBook.Close(true, misValue, misValue);
            xlApp.Quit();

            Marshal.ReleaseComObject(xlWorkSheet);
            Marshal.ReleaseComObject(xlWorkBook);
            Marshal.ReleaseComObject(xlApp);
            return Content("");
        }
    }
}

namespace RMERP.DAL.ViewModel
{
    public class ErrorVM
    {
        public string ErrorMessage { get; set; }
        public string InnerException { get; set; }
    }
}