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
        //[OutputCache(Duration = 60, VaryByParam = "WAG_Id,FRM_Id")]
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
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            Wage_Register wageRegister = wageRegisterManager.GetWage_RegisterByID(editWageRegisterVM.wageRegisterVM.WAR_Id);           
            editWageRegisterVM.wageRegisterVM.ADM_LastModifiedBy = sessionUtils.GetLoggedAdminID();
            wageRegister = WageRegisterMapper.mapMeEdit(editWageRegisterVM.wageRegisterVM, wageRegister);
           
            string res = wageRegisterManager.UpdateWageRegister(wageRegister);
            if (res != string.Empty)
            {
                TempData["message"] = "Wage Register is not updated!";
            }
            return RedirectToAction("WageRegister", new { WAG_Id = editWageRegisterVM.wageRegisterVM.WAG_Id, FRM_Id = editWageRegisterVM.FRM_Id });
        }


        public async Task<FileResult> WageRegisterExcel_1(int WAG_Id, int FRM_Id)
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
            Wage_Process wage_Process = wageProcess.getWageProcessById(WAG_Id);
            DateTime wageMonth = wage_Process.WAG_Month;
            string WAG_Month = wageMonth.ToString("MMMM").ToUpper() + "-" + wageMonth.ToString("yyyy");

            string newPath = GetTempFolderPath(_hostingEnvironment.WebRootPath);
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
                            CellHeading.SetCellValue(wage_Process.FRM_.FRM_Name.ToUpper());
                            excelSheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 28));
                            CellHeading.CellStyle = styleHeading;
                            CellUtil.SetAlignment(CellHeading, workbook, (short)HorizontalAlignment.Center);

                            IRow rowAdd1 = excelSheet.CreateRow(DesCount + 1);
                            ICell CellAdd1 = rowAdd1.CreateCell(0);
                            CellAdd1.SetCellValue(wage_Process.FRM_.FRM_Address1.ToUpper()+","+ wage_Process.FRM_.FRM_Address2.ToUpper()+",");
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
                            CellClient.SetCellValue(client.client.CLI_Name.ToString());
                            CellClient.CellStyle = styleDesignation;
                            CellUtil.SetAlignment(CellClient, workbook, (short)HorizontalAlignment.Center);
                            excelSheet.AddMergedRegion(new CellRangeAddress(DesCount + 3, DesCount + 3, 0, 28));
                            #endregion

                            DesCount = DesCount + 4;

                            double Final_TotalPaybleDays = 0, Final_TotalOTHrs=0;
                            decimal Final_TotalBasic = 0M, Final_TotalDA = 0M, Final_TotalHRA = 0M, Final_TotalOT = 0M, Final_TotalGrossTotal = 0M, Final_TotalPF = 0M, Final_TotalESIC = 0M;
                            decimal Final_TotalProfTax = 0m, Final_TotalRevenue = 0M, Final_TotalCanteenFacility = 0M, Final_TotalAdvance = 0M, Final_TotalDeduct = 0M, Final_TotalFinal = 0M;
                            decimal Final_TotalOutStation = 0M, Final_TotalAttendance = 0M, Final_TotalNightshift = 0M, Final_TotalPerformance = 0M, Final_TotalLWF = 0M;
                            decimal Final_TotalAllowance_1 = 0M, Final_TotalAllowance_2 = 0M, Final_TotalAllowance_3 = 0M, Final_TotalAllowance_4 = 0M, Final_TotalAllowance_5 = 0M;
                            bool IsCRI_OutStation_Allowance=false, IsCRI_Attendance_Allowance = false, IsCRI_Nightshift_Allowance = false, IsCRI_Performance_Allowance = false, IsCRI_OT_Calculate_Payableday = false;
                           // bool IsCRI_Allowance_1 = false, IsCRI_Allowance_2 = false, IsCRI_Allowance_3 = false, IsCRI_Allowance_4 = false, IsCRI_Allowance_5 = false;
                            bool IsCRI_ProfessionalTax = false, IsCRI_RevenueDeduction = false, IsCRI_CanteenFacility = false, IsDES_Include_LWF = false;
                            List<WageAllowancesTotalVM> WageAllowancesTotalVMs = new List<WageAllowancesTotalVM>();
                            //added on 26th oct 2020                           
                            IEnumerable<ClientRequirementVM> clientReqs = item.wageRegisterVMs.Select(m => m.clientRequirementVM);
                            string[] all1 = clientReqs.Select(m => m.CRI_Allowance_Name_1).Distinct().ToArray();
                            string[] all2 = clientReqs.Select(m => m.CRI_Allowance_Name_2).Distinct().ToArray();
                            string[] all3 = clientReqs.Select(m => m.CRI_Allowance_Name_3).Distinct().ToArray();
                            string[] all4 = clientReqs.Select(m => m.CRI_Allowance_Name_4).Distinct().ToArray();
                            string[] all5 = clientReqs.Select(m => m.CRI_Allowance_Name_5).Distinct().ToArray();

                            List<string> allCustomAallownces = new List<string>();
                            allCustomAallownces.AddRange(all1.ToList());
                            allCustomAallownces.AddRange(all2.ToList());
                            allCustomAallownces.AddRange(all3.ToList());
                            allCustomAallownces.AddRange(all4.ToList());
                            allCustomAallownces.AddRange(all5.ToList());
                            
                            //end
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
                                excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount, 0, 28));
                                CellHeader.CellStyle = styleTotal;

                                row = excelSheet.CreateRow(DesCount + 1);
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
                                List<string> allowance = new List<string>();
                                int i = 0;
                                foreach (var all in wageRegisters[0].allowanceVMs.OrderBy(m => m.allowanceVM.ALL_Id))
                                {
                                    arrAllowancesTotal[i] = 0;
                                    allowance.Add(all.allowanceVM.ALL_Alias);
                                    ICell cellAll = row.CreateCell(cell + 1);
                                    cellAll.SetCellValue(all.allowanceVM.ALL_Alias);
                                    excelSheet.SetColumnWidth(cell + 1, (int)((25 + 0.72) * 140));
                                    cellAll.CellStyle = styleGrey40;
                                    cell++; i++;
                                }
                                //custom allowance
                                foreach(string  allownce in allCustomAallownces.Distinct().Where(m => m != null))
                                {
                                    cell = cell + 1;
                                    ICell celOutAllow = row.CreateCell(cell);
                                    celOutAllow.SetCellValue(allownce);
                                    celOutAllow.CellStyle = styleGrey40;
                                }
                                //if (clientRequirement.CRI_Allowance_1 == true)
                                //{
                                //    cell = cell + 1;
                                //    ICell celOutAllow = row.CreateCell(cell);
                                //    celOutAllow.SetCellValue(clientRequirement.CRI_Allowance_Name_1);
                                //    celOutAllow.CellStyle = styleGrey40;
                                //}
                                //if (clientRequirement.CRI_Allowance_2 == true)
                                //{
                                //    cell = cell + 1;
                                //    ICell celOutAllow = row.CreateCell(cell);
                                //    celOutAllow.SetCellValue(clientRequirement.CRI_Allowance_Name_2);
                                //    celOutAllow.CellStyle = styleGrey40;
                                //}
                                //if (clientRequirement.CRI_Allowance_3 == true)
                                //{
                                //    cell = cell + 1;
                                //    ICell celOutAllow = row.CreateCell(cell);
                                //    celOutAllow.SetCellValue(clientRequirement.CRI_Allowance_Name_3);
                                //    celOutAllow.CellStyle = styleGrey40;
                                //}
                                //if (clientRequirement.CRI_Allowance_4 == true)
                                //{
                                //    cell = cell + 1;
                                //    ICell celOutAllow = row.CreateCell(cell);
                                //    celOutAllow.SetCellValue(clientRequirement.CRI_Allowance_Name_4);
                                //    celOutAllow.CellStyle = styleGrey40;
                                //}
                                //if (clientRequirement.CRI_Allowance_5 == true)
                                //{
                                //    cell = cell + 1;
                                //    ICell celOutAllow = row.CreateCell(cell);
                                //    celOutAllow.SetCellValue(clientRequirement.CRI_Allowance_Name_5);
                                //    celOutAllow.CellStyle = styleGrey40;
                                //}
                                //end custom allowance
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
                                    ICell cellotHrs = row.CreateCell(cell);
                                    cellotHrs.SetCellValue("OT Hrs");
                                    cellotHrs.CellStyle = styleGrey40;

                                    cell = cell + 1;
                                    ICell cell13 = row.CreateCell(cell);
                                    cell13.SetCellValue("OT Amount");
                                    cell13.CellStyle = styleGrey40;                                  
                                }
                                int count = cell + 1;
                                ICell cell14 = row.CreateCell(cell+1);
                                cell14.SetCellValue("Gross Total");
                                cell14.CellStyle = styleGrey40;
                                ICell cell15 = row.CreateCell(cell + 2);
                                cell15.SetCellValue("PF");
                                cell15.CellStyle = styleGrey50;
                                ICell cell16 = row.CreateCell(cell + 3);
                                cell16.SetCellValue("ESIC");
                                cell16.CellStyle = styleGrey50;

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
                                ICell cell17 = row.CreateCell(cellNext1);
                                cell17.SetCellValue("Advance Installment");
                                excelSheet.SetColumnWidth(cellNext1, (int)((25 + 0.72) * 140));
                                cell17.CellStyle = styleGrey50;

                                int cellLWFNext = cellNext1 + 1;
                                if (!dd.DES_Exclude_LWF)
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

                                DesCount = DesCount + 2;
                                double TotalPaybleDays = 0, TotalOTHrs=0;
                                decimal TotalBasic = 0M, TotalDA = 0M, TotalHRA = 0M, TotalOT = 0M, TotalGrossTotal = 0M, TotalPF = 0M, TotalESIC = 0M, TotalProfTax = 0m, TotalRevenue = 0M, TotalCanteenFacility = 0M, TotalAdvance = 0M, TotalDeduct = 0M, TotalFinal = 0M;
                                decimal TotalOutStation = 0M, TotalAttendance = 0M, TotalNightshift = 0M, TotalPerformance = 0M, TotalLWF=0M;
                                decimal TotalAllowance1 = -1M, TotalAllowance2 = -1M, TotalAllowance3 = -1M, TotalAllowance4 = -1M, TotalAllowance5 = -1M;
                                int srno = 0;
                                foreach (var employee in wageRegisters)
                                {
                                    srno = srno + 1;
                                    #region Employee Data

                                    row = excelSheet.CreateRow(DesCount);
                                    ICell cellEmp0 = row.CreateCell(0);
                                    cellEmp0.SetCellValue(srno);
                                    cellEmp0.CellStyle = styleGrey25;
                                                                        
                                    ICell cellEmp1 = row.CreateCell(1);
                                    cellEmp1.SetCellValue(employee.employeeVM.EMP_Id.ToString("D5"));
                                    cellEmp1.CellStyle = styleGrey25;

                                    ICell cellEmp2 = row.CreateCell(2);
                                    cellEmp2.SetCellValue(employee.employeeVM.EMP_ESIC_Number);
                                    cellEmp2.CellStyle = styleGrey25;

                                    ICell cellEmp3 = row.CreateCell(3);
                                    cellEmp3.SetCellValue(employee.employeeVM.EMP_UAN_Number);
                                    cellEmp3.CellStyle = styleGrey25;

                                    ICell cellEmp4 = row.CreateCell(4);
                                    cellEmp4.SetCellValue(employee.employeeVM.EMP_FirstName + " " + employee.employeeVM.EMP_MiddleName + " " + employee.employeeVM.EMP_SurName);
                                    cellEmp4.CellStyle = styleGrey25;

                                    ICell cellEmp5 = row.CreateCell(5);
                                    cellEmp5.SetCellValue(employee.employeeVM.EMP_Designation);
                                    cellEmp5.CellStyle = styleGrey25;

                                    ICell cellEmp6 = row.CreateCell(6);
                                    cellEmp6.SetCellValue(Convert.ToBoolean(employee.employeeVM.EMP_Gender) == true ? "M" : "F");
                                    cellEmp6.CellStyle = styleGrey25;

                                    ICell cellEmp7 = row.CreateCell(7);
                                    cellEmp7.SetCellValue(DateHelper.getDateWithFormat(employee.employeeVM.EMP_DateOfJoining));
                                    cellEmp7.CellStyle = styleGrey25;

                                    ICell cellEmp8 = row.CreateCell(8);
                                    cellEmp8.SetCellValue(employee.WAR_TotalWorkingDays);
                                    cellEmp8.CellStyle = styleGrey25;

                                    ICell cellEmp9 = row.CreateCell(9);
                                    cellEmp9.SetCellValue(employee.WAR_TotalPaybleDays);
                                    cellEmp9.CellStyle = styleGrey25;
                                    TotalPaybleDays = TotalPaybleDays + employee.WAR_TotalPaybleDays;

                                    ICell cellEmp10 = row.CreateCell(10);
                                    cellEmp10.SetCellValue(Math.Round(employee.WAR_Basic_Calculated, MidpointRounding.AwayFromZero).ToString());
                                    cellEmp10.CellStyle = styleGrey40;
                                    TotalBasic = TotalBasic + Math.Round(employee.WAR_Basic_Calculated, MidpointRounding.AwayFromZero);

                                    ICell cellEmp11 = row.CreateCell(11);
                                    cellEmp11.SetCellValue(Math.Round(employee.WAR_DA_Calculated, MidpointRounding.AwayFromZero).ToString());
                                    cellEmp11.CellStyle = styleGrey40;
                                    TotalDA = TotalDA + Math.Round(employee.WAR_DA_Calculated, MidpointRounding.AwayFromZero);

                                    ICell cellEmp12 = row.CreateCell(12);
                                    cellEmp12.SetCellValue(Math.Round(employee.WAR_HRA_Calculated, MidpointRounding.AwayFromZero).ToString());
                                    TotalHRA = TotalHRA + Math.Round(employee.WAR_HRA_Calculated, MidpointRounding.AwayFromZero);
                                    cellEmp12.CellStyle = styleGrey40;

                                    int cellAllowance = 12;
                                    int j = 0;

                                    foreach (var all in employee.allowanceVMs.OrderBy(m => m.allowanceVM.ALL_Id))
                                    {
                                        arrAllowancesTotal[j] = arrAllowancesTotal[j] + all.WAA_Amount_Calculated;                                        
                                        ICell cellEmpAll = row.CreateCell(cellAllowance + 1);
                                        cellEmpAll.SetCellValue(Math.Round(all.WAA_Amount_Calculated, MidpointRounding.AwayFromZero).ToString());
                                        cellEmpAll.CellStyle = styleGrey40;
                                        cellAllowance++;
                                        j++;
                                    }
                                    int cellNxt = cellAllowance + 1;

                                 

                                    //custom allowance value
                                    foreach (string allownce in allCustomAallownces.Distinct().Where(m => m != null))
                                    {
                                        ICell cellCustom = row.CreateCell(cellNxt);
                                        decimal WAR_Allowance_Calculated = 0M;

                                        if (employee.clientRequirementVM.CRI_Allowance_Name_1 == allownce)
                                        {
                                            WAR_Allowance_Calculated = Math.Round(employee.WAR_Allowance_Calculated_1.Value, MidpointRounding.AwayFromZero);
                                            cellCustom.SetCellValue(WAR_Allowance_Calculated.ToString());
                                            TotalAllowance1 = TotalAllowance1 + WAR_Allowance_Calculated;
                                        }
                                        else
                                        if (employee.clientRequirementVM.CRI_Allowance_Name_2 == allownce)
                                        {
                                            WAR_Allowance_Calculated = Math.Round(employee.WAR_Allowance_Calculated_2.Value, MidpointRounding.AwayFromZero);
                                            cellCustom.SetCellValue(WAR_Allowance_Calculated.ToString());
                                            TotalAllowance2 = TotalAllowance2 + WAR_Allowance_Calculated;
                                        }
                                        else
                                        if (employee.clientRequirementVM.CRI_Allowance_Name_3 == allownce)
                                        {
                                            WAR_Allowance_Calculated = Math.Round(employee.WAR_Allowance_Calculated_3.Value, MidpointRounding.AwayFromZero);
                                            cellCustom.SetCellValue(WAR_Allowance_Calculated.ToString());
                                            TotalAllowance3 = TotalAllowance3 + WAR_Allowance_Calculated;
                                        }
                                        else
                                        if (employee.clientRequirementVM.CRI_Allowance_Name_4 == allownce)
                                        {
                                            WAR_Allowance_Calculated = Math.Round(employee.WAR_Allowance_Calculated_4.Value, MidpointRounding.AwayFromZero);
                                            cellCustom.SetCellValue(WAR_Allowance_Calculated.ToString());
                                            TotalAllowance4 = TotalAllowance4 + WAR_Allowance_Calculated;
                                        }
                                        else
                                        if (employee.clientRequirementVM.CRI_Allowance_Name_5 == allownce)
                                        {
                                            WAR_Allowance_Calculated = Math.Round(employee.WAR_Allowance_Calculated_5.Value, MidpointRounding.AwayFromZero);
                                            cellCustom.SetCellValue(WAR_Allowance_Calculated.ToString());
                                            TotalAllowance5 = TotalAllowance5 + WAR_Allowance_Calculated;
                                        }
                                        else
                                        {
                                            cellCustom.SetCellValue(0);
                                        }
                                        cellCustom.CellStyle = styleGrey40;
                                        cellNxt = cellNxt + 1;
                                    }


                                    //********************************************
                                    //if (clientRequirement.CRI_Allowance_1 == true)
                                    //{
                                    //    ICell cellOutstation = row.CreateCell(cellNxt);
                                    //    decimal WAR_Allowance_Calculated_1 = 0M;
                                    //    if (employee.WAR_Allowance_Calculated_1 != null)
                                    //    {
                                    //        WAR_Allowance_Calculated_1 = Math.Round(employee.WAR_Allowance_Calculated_1.Value, MidpointRounding.AwayFromZero);
                                    //    }
                                    //    cellOutstation.SetCellValue(WAR_Allowance_Calculated_1.ToString());
                                    //    TotalAllowance1 = TotalAllowance1 + WAR_Allowance_Calculated_1;
                                    //    cellOutstation.CellStyle = styleGrey40;
                                    //    cellNxt = cellNxt + 1;
                                    //}
                                    //if (clientRequirement.CRI_Allowance_2 == true)
                                    //{
                                    //    ICell cellOutstation = row.CreateCell(cellNxt);
                                    //    decimal WAR_Allowance_Calculated_2 = 0M;
                                    //    if (employee.WAR_Allowance_Calculated_2 != null)
                                    //    {
                                    //        WAR_Allowance_Calculated_2 = Math.Round(employee.WAR_Allowance_Calculated_2.Value, MidpointRounding.AwayFromZero);
                                    //    }
                                    //    cellOutstation.SetCellValue(WAR_Allowance_Calculated_2.ToString());
                                    //    TotalAllowance2 = TotalAllowance2 + WAR_Allowance_Calculated_2;
                                    //    cellOutstation.CellStyle = styleGrey40;
                                    //    cellNxt = cellNxt + 1;
                                    //}
                                    //if (clientRequirement.CRI_Allowance_3 == true)
                                    //{
                                    //    ICell cellOutstation = row.CreateCell(cellNxt);
                                    //    decimal WAR_Allowance_Calculated_3 = 0M;
                                    //    if (employee.WAR_Allowance_Calculated_3 != null)
                                    //    {
                                    //        WAR_Allowance_Calculated_3 = Math.Round(employee.WAR_Allowance_Calculated_3.Value, MidpointRounding.AwayFromZero);
                                    //    }
                                    //    cellOutstation.SetCellValue(WAR_Allowance_Calculated_3.ToString());
                                    //    TotalAllowance3 = TotalAllowance3 + WAR_Allowance_Calculated_3;
                                    //    cellOutstation.CellStyle = styleGrey40;
                                    //    cellNxt = cellNxt + 1;
                                    //}
                                    //if (clientRequirement.CRI_Allowance_4 == true)
                                    //{
                                    //    ICell cellOutstation = row.CreateCell(cellNxt);
                                    //    decimal WAR_Allowance_Calculated_4 = 0M;
                                    //    if (employee.WAR_Allowance_Calculated_4 != null)
                                    //    {
                                    //        WAR_Allowance_Calculated_4 = Math.Round(employee.WAR_Allowance_Calculated_4.Value, MidpointRounding.AwayFromZero);
                                    //    }
                                    //    cellOutstation.SetCellValue(WAR_Allowance_Calculated_4.ToString());
                                    //    TotalAllowance4 = TotalAllowance4 + WAR_Allowance_Calculated_4;
                                    //    cellOutstation.CellStyle = styleGrey40;
                                    //    cellNxt = cellNxt + 1;
                                    //}
                                    //if (clientRequirement.CRI_Allowance_5 == true)
                                    //{
                                    //    ICell cellOutstation = row.CreateCell(cellNxt);
                                    //    decimal WAR_Allowance_Calculated_5 = 0M;
                                    //    if (employee.WAR_Allowance_Calculated_5 != null)
                                    //    {
                                    //        WAR_Allowance_Calculated_5 = Math.Round(employee.WAR_Allowance_Calculated_5.Value, MidpointRounding.AwayFromZero);
                                    //    }
                                    //    cellOutstation.SetCellValue(WAR_Allowance_Calculated_5.ToString());
                                    //    TotalAllowance5 = TotalAllowance5 + WAR_Allowance_Calculated_5;
                                    //    cellOutstation.CellStyle = styleGrey40;
                                    //    cellNxt = cellNxt + 1;
                                    //}
                                    //end cutsom allowance

                                    #region New allowances 29th july
                                    if (clientRequirement.CRI_OutStation_Allowance == true)
                                    {
                                        ICell cellOutstation = row.CreateCell(cellNxt);
                                        decimal WAR_OutStation_Allowance_Calculated = 0M;
                                        if (employee.WAR_OutStation_Allowance_Calculated != null)
                                        {
                                            WAR_OutStation_Allowance_Calculated = Math.Round(employee.WAR_OutStation_Allowance_Calculated.Value, MidpointRounding.AwayFromZero);
                                        }                                        
                                        cellOutstation.SetCellValue(WAR_OutStation_Allowance_Calculated.ToString());
                                        TotalOutStation = TotalOutStation + WAR_OutStation_Allowance_Calculated;
                                        cellOutstation.CellStyle = styleGrey40;
                                        cellNxt = cellNxt + 1;
                                    }
                                    if (clientRequirement.CRI_Attendance_Allowance == true)
                                    {
                                        ICell cellAttendance= row.CreateCell(cellNxt);
                                        decimal WAR_Attendance_Allowance_Calculated = 0M;
                                        if (employee.WAR_Attendance_Allowance_Calculated != null)
                                        {
                                            WAR_Attendance_Allowance_Calculated = Math.Round(employee.WAR_Attendance_Allowance_Calculated.Value, MidpointRounding.AwayFromZero);
                                        }
                                        cellAttendance.SetCellValue(WAR_Attendance_Allowance_Calculated.ToString());
                                        TotalAttendance = TotalAttendance + WAR_Attendance_Allowance_Calculated;
                                        cellAttendance.CellStyle = styleGrey40;
                                        cellNxt = cellNxt + 1;
                                    }
                                    if (clientRequirement.CRI_Nightshift_Allowance == true)
                                    {
                                        ICell cellNightshift = row.CreateCell(cellNxt);
                                        decimal WAR_Nightshift_Allowance_Calculated = 0M;
                                        if (employee.WAR_Nightshift_Allowance_Calculated != null)
                                        {
                                            WAR_Nightshift_Allowance_Calculated = Math.Round(employee.WAR_Nightshift_Allowance_Calculated.Value, MidpointRounding.AwayFromZero);
                                        }
                                        cellNightshift.SetCellValue(WAR_Nightshift_Allowance_Calculated.ToString());
                                        TotalNightshift = TotalNightshift + WAR_Nightshift_Allowance_Calculated;
                                        cellNightshift.CellStyle = styleGrey40;
                                        cellNxt = cellNxt + 1;
                                    }
                                    if (clientRequirement.CRI_Performance_Allowance == true)
                                    {
                                        ICell cellPerformance = row.CreateCell(cellNxt);
                                        decimal WAR_Performance_Allowance_Calculated = 0M;
                                        if (employee.WAR_Performance_Allowance_Calculated != null)
                                        {
                                            WAR_Performance_Allowance_Calculated = Math.Round(employee.WAR_Performance_Allowance_Calculated.Value, MidpointRounding.AwayFromZero);
                                        }
                                        cellPerformance.SetCellValue(WAR_Performance_Allowance_Calculated.ToString());
                                        TotalPerformance = TotalPerformance + WAR_Performance_Allowance_Calculated;
                                        cellPerformance.CellStyle = styleGrey40;
                                        cellNxt = cellNxt + 1;
                                    }
                                    if (!clientRequirement.CRI_OT_Calculate_Payableday)
                                    {
                                        ICell cellEmpotHrs = row.CreateCell(cellNxt);
                                        cellEmpotHrs.SetCellValue(employee.WAR_ExtraWorkingHours);
                                        TotalOTHrs = TotalOTHrs + employee.WAR_ExtraWorkingHours;
                                        cellEmpotHrs.CellStyle = styleGrey40;
                                        cellNxt = cellNxt + 1;

                                        ICell cellEmp13 = row.CreateCell(cellNxt);
                                        cellEmp13.SetCellValue(Math.Round(employee.WAR_OverTime_Calculated, MidpointRounding.AwayFromZero).ToString());
                                        TotalOT = TotalOT + Math.Round(employee.WAR_OverTime_Calculated, MidpointRounding.AwayFromZero);
                                        cellEmp13.CellStyle = styleGrey40;
                                        cellNxt = cellNxt + 1;
                                    }
                                    #endregion                                    
                                    ICell cellEmp14 = row.CreateCell(cellNxt);
                                    cellEmp14.SetCellValue(Math.Round(employee.WAR_GrossTotal, MidpointRounding.AwayFromZero).ToString());
                                    TotalGrossTotal = TotalGrossTotal + Math.Round(employee.WAR_GrossTotal, MidpointRounding.AwayFromZero);
                                    cellEmp14.CellStyle = styleGrey40;
                                    ICell cellEmp15 = row.CreateCell(cellNxt + 1);
                                    cellEmp15.SetCellValue(Math.Round(employee.WAR_PF_Calculated, MidpointRounding.AwayFromZero).ToString());
                                    TotalPF = TotalPF + Math.Round(employee.WAR_PF_Calculated, MidpointRounding.AwayFromZero);
                                    cellEmp15.CellStyle = styleGrey50;
                                    ICell cellEmp16 = row.CreateCell(cellNxt + 2);
                                    cellEmp16.SetCellValue(Math.Round(employee.WAR_ESIC_Calculated, MidpointRounding.AwayFromZero).ToString());
                                    TotalESIC = TotalESIC + Math.Round(employee.WAR_ESIC_Calculated, MidpointRounding.AwayFromZero);
                                    cellEmp16.CellStyle = styleGrey50;
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

                                    ICell cellEmp17 = row.CreateCell(cellNxt1+1);
                                    cellEmp17.SetCellValue(Math.Round(employee.WAR_Advance_Amount, MidpointRounding.AwayFromZero).ToString());
                                    TotalAdvance = TotalAdvance + Math.Round(employee.WAR_Advance_Amount, MidpointRounding.AwayFromZero);
                                    cellEmp17.CellStyle = styleGrey50;

                                    int cellNext2 = cellNxt1 + 2;
                                    //TotalLWF                                    
                                    if (!dd.DES_Exclude_LWF)
                                    {
                                        ICell cell_LWF = row.CreateCell(cellNext2);
                                        cell_LWF.SetCellValue(Convert.ToString(Math.Round((employee.WAR_LWF_Deduction_Employee!=null?employee.WAR_LWF_Deduction_Employee.Value:0), MidpointRounding.AwayFromZero).ToString()));
                                        excelSheet.SetColumnWidth(cellNext2, (int)((25 + 0.72) * 140));
                                        cell_LWF.CellStyle = styleGrey50;
                                        TotalLWF = TotalLWF + Convert.ToDecimal(employee.WAR_LWF_Deduction_Employee);
                                        cellNext2 = cellNext2 + 1;
                                    }

                                    #region Total Deduction
                                    decimal DeductTotal = Math.Round(employee.WAR_PF_Calculated, MidpointRounding.AwayFromZero) + Math.Round(employee.WAR_ESIC_Calculated, MidpointRounding.AwayFromZero) + Math.Round(Convert.ToDecimal(employee.WAR_ProffesionalTax_Calculated), MidpointRounding.AwayFromZero)
                                        + Math.Round(employee.WAR_Advance_Amount, MidpointRounding.AwayFromZero) + Math.Round((employee.WAR_LWF_Deduction_Employee != null ? employee.WAR_LWF_Deduction_Employee.Value : 0), MidpointRounding.AwayFromZero);
                                    if (employee.WAR_RevenueDeduction_Calculated != "-")
                                    {
                                        DeductTotal += Math.Round(Convert.ToDecimal(employee.WAR_RevenueDeduction_Calculated), MidpointRounding.AwayFromZero);
                                    }
                                    if (employee.WAR_CanteenFacility_Calculation != "-")
                                    {
                                        DeductTotal += Math.Round(Convert.ToDecimal(employee.WAR_CanteenFacility_Calculation), MidpointRounding.AwayFromZero);
                                    }
                                    #endregion
                                   
                                    ICell cellEmp18 = row.CreateCell(cellNext2);
                                    cellEmp18.SetCellValue(DeductTotal.ToString());
                                    cellEmp18.CellStyle = styleGrey50;
                                    TotalDeduct = TotalDeduct + DeductTotal;
                                    ICell cellEmp19 = row.CreateCell(cellNext2 + 1);
                                    cellEmp19.SetCellValue(Math.Round(employee.WAR_FinalTotal, MidpointRounding.AwayFromZero).ToString());
                                    TotalFinal = TotalFinal + Math.Round(employee.WAR_FinalTotal, MidpointRounding.AwayFromZero);
                                    cellEmp19.CellStyle = styleGrey80;
                                    ICell cellEmp20 = row.CreateCell(cellNext2 + 2);
                                    cellEmp20.SetCellValue("");                                    
                                    cellEmp20.CellStyle = styleGrey80;
                                    #endregion
                                    DesCount++;
                                }

                                #region Total line in Excel
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
                           
                                int totalCount = 8;
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
                                                              
                                foreach (var all in wageRegisters[0].allowanceVMs.OrderBy(m => m.allowanceVM.ALL_Id))
                                {
                                    WageAllowancesTotalVM wageTotalVM = new WageAllowancesTotalVM();
                                    wageTotalVM.ALL_Id = all.allowanceVM.ALL_Id;
                                    wageTotalVM.Parameter = all.allowanceVM.ALL_Alias;
                                    wageTotalVM.Value = arrAllowancesTotal[k];
                                    WageAllowancesTotalVMs.Add(wageTotalVM);
                                    ICell cellEmpAll = row.CreateCell(cellAllow + 1);
                                    cellEmpAll.SetCellValue(Math.Round(arrAllowancesTotal[k], MidpointRounding.AwayFromZero).ToString());
                                    cellEmpAll.CellStyle = styleGrey40;
                                    cellAllow++;
                                    k++;
                                }
                                
                                totalCount = cellAllow;

                                //custom allow
                                foreach (string allownce in allCustomAallownces.Distinct().Where(m => m != null))
                                {
                                    totalCount = totalCount + 1;
                                    ICell cellTotAllowance_1 = row.CreateCell(totalCount);
                                    cellTotAllowance_1.CellStyle = styleGrey40;
                                    cellTotAllowance_1.SetCellValue("");
                                }
                                //if (clientRequirement.CRI_Allowance_1 == true)
                                //{
                                //    IsCRI_Allowance_1 = true;
                                //    totalCount = totalCount + 1;
                                //    ICell cellTotAllowance_1 = row.CreateCell(totalCount);
                                //    cellTotAllowance_1.CellStyle = styleGrey40;
                                //    cellTotAllowance_1.SetCellValue(Math.Round(TotalAllowance1, MidpointRounding.AwayFromZero).ToString());
                                //}
                                //if (clientRequirement.CRI_Allowance_2 == true)
                                //{
                                //    IsCRI_Allowance_2 = true;
                                //    totalCount = totalCount + 1;
                                //    ICell cellTotAllowance_2 = row.CreateCell(totalCount);
                                //    cellTotAllowance_2.CellStyle = styleGrey40;
                                //    cellTotAllowance_2.SetCellValue(Math.Round(TotalAllowance2, MidpointRounding.AwayFromZero).ToString());
                                //}
                                //if (clientRequirement.CRI_Allowance_3 == true)
                                //{
                                //    IsCRI_Allowance_3 = true;
                                //    totalCount = totalCount + 1;
                                //    ICell cellTotAllowance_3 = row.CreateCell(totalCount);
                                //    cellTotAllowance_3.CellStyle = styleGrey40;
                                //    cellTotAllowance_3.SetCellValue(Math.Round(TotalAllowance3, MidpointRounding.AwayFromZero).ToString());
                                //}
                                //if (clientRequirement.CRI_Allowance_4 == true)
                                //{
                                //    IsCRI_Allowance_4 = true;
                                //    totalCount = totalCount + 1;
                                //    ICell cellTotAllowance_4 = row.CreateCell(totalCount);
                                //    cellTotAllowance_4.CellStyle = styleGrey40;
                                //    cellTotAllowance_4.SetCellValue(Math.Round(TotalAllowance4, MidpointRounding.AwayFromZero).ToString());
                                //}
                                //if (clientRequirement.CRI_Allowance_5 == true)
                                //{
                                //    IsCRI_Allowance_1 = true;
                                //    totalCount = totalCount + 1;
                                //    ICell cellTotAllowance_5 = row.CreateCell(totalCount);
                                //    cellTotAllowance_5.CellStyle = styleGrey40;
                                //    cellTotAllowance_5.SetCellValue(Math.Round(TotalAllowance5, MidpointRounding.AwayFromZero).ToString());
                                //}
                                //end custom allow

                                #region New total allowances 29th july
                                if (clientRequirement.CRI_OutStation_Allowance == true)
                                {
                                    IsCRI_OutStation_Allowance = true;
                                    totalCount = totalCount + 1;
                                    ICell cellTotOutstation = row.CreateCell(totalCount);
                                    cellTotOutstation.CellStyle = styleGrey40;
                                    cellTotOutstation.SetCellValue(Math.Round(TotalOutStation, MidpointRounding.AwayFromZero).ToString());
                                }
                                if (clientRequirement.CRI_Attendance_Allowance == true)
                                {
                                    IsCRI_Attendance_Allowance = true;
                                    totalCount = totalCount + 1;
                                    ICell cellTotAttendance = row.CreateCell(totalCount);
                                    cellTotAttendance.CellStyle = styleGrey40;
                                    cellTotAttendance.SetCellValue(Math.Round(TotalAttendance, MidpointRounding.AwayFromZero).ToString());
                                }
                                if (clientRequirement.CRI_Nightshift_Allowance == true)
                                {
                                    IsCRI_Nightshift_Allowance = true;
                                    totalCount = totalCount + 1;
                                    ICell cellTotNightshift = row.CreateCell(totalCount);
                                    cellTotNightshift.CellStyle = styleGrey40;
                                    cellTotNightshift.SetCellValue(Math.Round(TotalNightshift, MidpointRounding.AwayFromZero).ToString());
                                }
                                if (clientRequirement.CRI_Performance_Allowance == true)
                                {
                                    IsCRI_Performance_Allowance = true;
                                    totalCount = totalCount + 1;
                                    ICell cellTotPerformance = row.CreateCell(totalCount);
                                    cellTotPerformance.CellStyle = styleGrey40;
                                    cellTotPerformance.SetCellValue(Math.Round(TotalPerformance, MidpointRounding.AwayFromZero).ToString());
                                }
                                if (!clientRequirement.CRI_OT_Calculate_Payableday)
                                {                                   
                                    totalCount = totalCount + 1;
                                    ICell cellTotOtHrs = row.CreateCell(totalCount);
                                    cellTotOtHrs.CellStyle = styleGrey40;
                                    cellTotOtHrs.SetCellValue(TotalOTHrs);

                                    IsCRI_OT_Calculate_Payableday = true;
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
                                    IsCRI_ProfessionalTax = true;
                                    totalCount1 = totalCount1 + 1;
                                    ICell cellTot9 = row.CreateCell(totalCount1);
                                    cellTot9.SetCellValue(Math.Round(TotalProfTax, MidpointRounding.AwayFromZero).ToString());
                                    cellTot9.CellStyle = styleGrey50;
                                }
                                if (clientRequirement.CRI_RevenueDeduction)
                                {
                                    IsCRI_RevenueDeduction = true;
                                    totalCount1 = totalCount1 + 1;
                                    ICell cellTot10 = row.CreateCell(totalCount1);
                                    cellTot10.SetCellValue(Math.Round(TotalRevenue, MidpointRounding.AwayFromZero).ToString());
                                    cellTot10.CellStyle = styleGrey50;
                                }
                                if (clientRequirement.CRI_CanteenFacility)
                                {
                                    IsCRI_CanteenFacility = true;
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
                                    IsDES_Include_LWF = true;
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

                                ICell cellTot15 = row.CreateCell(TotCount + 2);
                                cellTot15.SetCellValue("");
                                cellTot15.CellStyle = styleGrey80;
                                #endregion

                                DesCount = DesCount + 1;

                                Final_TotalPaybleDays = Final_TotalPaybleDays + TotalPaybleDays;
                                Final_TotalBasic = Final_TotalBasic + TotalBasic;
                                Final_TotalDA = Final_TotalDA + TotalDA;
                                Final_TotalHRA = Final_TotalHRA + TotalHRA;
                                Final_TotalOTHrs = Final_TotalOTHrs + TotalOTHrs;
                                Final_TotalOT = Final_TotalOT + TotalOT;
                                Final_TotalGrossTotal = Final_TotalGrossTotal + TotalGrossTotal;
                                Final_TotalPF = Final_TotalPF + TotalPF;
                                Final_TotalESIC = Final_TotalESIC + TotalESIC;
                                Final_TotalProfTax = Final_TotalProfTax + TotalProfTax;
                                Final_TotalRevenue = Final_TotalRevenue + TotalRevenue;
                                Final_TotalCanteenFacility = Final_TotalCanteenFacility + TotalCanteenFacility;
                                Final_TotalAdvance = Final_TotalAdvance + TotalAdvance;
                                Final_TotalDeduct = Final_TotalDeduct + TotalDeduct;
                                Final_TotalFinal = Final_TotalFinal + TotalFinal;
                                Final_TotalOutStation = Final_TotalOutStation + TotalOutStation;
                                Final_TotalAttendance = Final_TotalAttendance + TotalAttendance;
                                Final_TotalNightshift = Final_TotalNightshift + TotalNightshift;
                                Final_TotalPerformance = Final_TotalPerformance + TotalPerformance;
                                Final_TotalAllowance_1 = Final_TotalAllowance_1 + TotalAllowance1;
                                Final_TotalAllowance_2 = Final_TotalAllowance_2 + TotalAllowance2;
                                Final_TotalAllowance_3 = Final_TotalAllowance_3 + TotalAllowance3;
                                Final_TotalAllowance_4 = Final_TotalAllowance_4 + TotalAllowance4;
                                Final_TotalAllowance_5 = Final_TotalAllowance_5 + TotalAllowance5;
                                Final_TotalLWF = Final_TotalLWF + TotalLWF;
                            }

                            DesCount = DesCount + 1;
                            IRow frow = excelSheet.CreateRow(DesCount);
                            IRow frowSecond = excelSheet.CreateRow(DesCount+1);
                            excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount+1, 0, 8));                            
                                                       
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

                            int fk = 0;
                            int fcellAllow = ftotalCount + 4;
                            foreach (var all in WageAllowancesTotalVMs.GroupBy(m=> new { m.ALL_Id ,m.Parameter}).Select(g=>new { Total= g.Sum(m=>m.Value), ALL_Title=g.Key.Parameter }))
                            {
                                ICell fcellEmpAll = frow.CreateCell(fcellAllow + 1);
                                fcellEmpAll.SetCellValue(all.ALL_Title);
                                fcellEmpAll.CellStyle = styleGrey40;
                                ICell fcellEmpAlls = frowSecond.CreateCell(fcellAllow + 1);
                                fcellEmpAlls.SetCellValue(Math.Round(all.Total, MidpointRounding.AwayFromZero).ToString());
                                fcellEmpAlls.CellStyle = styleGrey40;
                                fcellAllow++;
                                fk++;
                            }
                            ftotalCount = fcellAllow;

                            //custom
                            foreach (string allownce in allCustomAallownces.Distinct().Where(m => m != null))
                            {
                                ftotalCount = ftotalCount + 1;
                                ICell cellTot_Allowance_1 = frow.CreateCell(ftotalCount);
                                cellTot_Allowance_1.CellStyle = styleGrey40;
                                cellTot_Allowance_1.SetCellValue("");
                                ICell cellTotAllowance_1 = frowSecond.CreateCell(ftotalCount);
                                cellTotAllowance_1.CellStyle = styleGrey40;
                                cellTotAllowance_1.SetCellValue("");
                            }
                            //if (IsCRI_Allowance_1)
                            //{
                            //    ftotalCount = ftotalCount + 1;
                            //    ICell cellTot_Allowance_1 = frow.CreateCell(ftotalCount);
                            //    cellTot_Allowance_1.CellStyle = styleGrey40;
                            //    cellTot_Allowance_1.SetCellValue("Total Allowance 1");
                            //    ICell cellTotAllowance_1 = frowSecond.CreateCell(ftotalCount);
                            //    cellTotAllowance_1.CellStyle = styleGrey40;
                            //    cellTotAllowance_1.SetCellValue(Math.Round(Final_TotalAllowance_1, MidpointRounding.AwayFromZero).ToString());
                            //}
                            //if (IsCRI_Allowance_2)
                            //{
                            //    ftotalCount = ftotalCount + 1;
                            //    ICell cellTot_Allowance_2 = frow.CreateCell(ftotalCount);
                            //    cellTot_Allowance_2.CellStyle = styleGrey40;
                            //    cellTot_Allowance_2.SetCellValue("Total Allowance 2");
                            //    ICell cellTotAllowance_2 = frowSecond.CreateCell(ftotalCount);
                            //    cellTotAllowance_2.CellStyle = styleGrey40;
                            //    cellTotAllowance_2.SetCellValue(Math.Round(Final_TotalAllowance_2, MidpointRounding.AwayFromZero).ToString());
                            //}
                            //if (IsCRI_Allowance_3)
                            //{
                            //    ftotalCount = ftotalCount + 1;
                            //    ICell cellTot_Allowance_3 = frow.CreateCell(ftotalCount);
                            //    cellTot_Allowance_3.CellStyle = styleGrey40;
                            //    cellTot_Allowance_3.SetCellValue("Total Allowance 3");
                            //    ICell cellTotAllowance_3 = frowSecond.CreateCell(ftotalCount);
                            //    cellTotAllowance_3.CellStyle = styleGrey40;
                            //    cellTotAllowance_3.SetCellValue(Math.Round(Final_TotalAllowance_3, MidpointRounding.AwayFromZero).ToString());
                            //}
                            //if (IsCRI_Allowance_4)
                            //{
                            //    ftotalCount = ftotalCount + 1;
                            //    ICell cellTot_Allowance_4 = frow.CreateCell(ftotalCount);
                            //    cellTot_Allowance_4.CellStyle = styleGrey40;
                            //    cellTot_Allowance_4.SetCellValue("Total Allowance 4");
                            //    ICell cellTotAllowance_4 = frowSecond.CreateCell(ftotalCount);
                            //    cellTotAllowance_4.CellStyle = styleGrey40;
                            //    cellTotAllowance_4.SetCellValue(Math.Round(Final_TotalAllowance_4, MidpointRounding.AwayFromZero).ToString());
                            //}
                            //if (IsCRI_Allowance_5)
                            //{
                            //    ftotalCount = ftotalCount + 1;
                            //    ICell cellTot_Allowance_5 = frow.CreateCell(ftotalCount);
                            //    cellTot_Allowance_5.CellStyle = styleGrey40;
                            //    cellTot_Allowance_5.SetCellValue("Total Allowance 5");
                            //    ICell cellTotAllowance_5 = frowSecond.CreateCell(ftotalCount);
                            //    cellTotAllowance_5.CellStyle = styleGrey40;
                            //    cellTotAllowance_5.SetCellValue(Math.Round(Final_TotalAllowance_5, MidpointRounding.AwayFromZero).ToString());
                            //}
                            //end custom
                            #region New grand total allowances
                            if (IsCRI_OutStation_Allowance)
                            {
                                ftotalCount = ftotalCount + 1;
                                ICell cellTotOutstation = frow.CreateCell(ftotalCount);
                                cellTotOutstation.CellStyle = styleGrey40;
                                cellTotOutstation.SetCellValue("Total OutStation");
                                ICell cellTotOutstations = frowSecond.CreateCell(ftotalCount);
                                cellTotOutstations.CellStyle = styleGrey40;
                                cellTotOutstations.SetCellValue(Math.Round(Final_TotalOutStation, MidpointRounding.AwayFromZero).ToString());
                            }
                            if (IsCRI_Attendance_Allowance)
                            {
                                ftotalCount = ftotalCount + 1;
                                ICell cellTotAttendance = frow.CreateCell(ftotalCount);
                                cellTotAttendance.CellStyle = styleGrey40;
                                cellTotAttendance.SetCellValue("Total Attendance");
                                ICell cellTotAttendances = frowSecond.CreateCell(ftotalCount);
                                cellTotAttendances.CellStyle = styleGrey40;
                                cellTotAttendances.SetCellValue(Math.Round(Final_TotalAttendance, MidpointRounding.AwayFromZero).ToString());
                            }
                            if (IsCRI_Nightshift_Allowance)
                            {
                                ftotalCount = ftotalCount + 1;
                                ICell cellTotNightshift = frow.CreateCell(ftotalCount);
                                cellTotNightshift.CellStyle = styleGrey40;
                                cellTotNightshift.SetCellValue("Total Nightshift");
                                ICell cellTotNightshifts = frowSecond.CreateCell(ftotalCount);
                                cellTotNightshifts.CellStyle = styleGrey40;
                                cellTotNightshifts.SetCellValue(Math.Round(Final_TotalNightshift, MidpointRounding.AwayFromZero).ToString());
                            }
                            if (IsCRI_Performance_Allowance)
                            {
                                ftotalCount = ftotalCount + 1;
                                ICell cellTotPerformance = frow.CreateCell(ftotalCount);
                                cellTotPerformance.CellStyle = styleGrey40;
                                cellTotPerformance.SetCellValue("Total Performance");
                                ICell cellTotPerformances = frowSecond.CreateCell(ftotalCount);
                                cellTotPerformances.CellStyle = styleGrey40;
                                cellTotPerformances.SetCellValue(Math.Round(Final_TotalPerformance, MidpointRounding.AwayFromZero).ToString());
                            }
                            if (IsCRI_OT_Calculate_Payableday)
                            {
                                ftotalCount = ftotalCount + 1;
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
                            }
                            #endregion

                            ICell fcellTotGross = frow.CreateCell(ftotalCount + 1);
                            fcellTotGross.SetCellValue("Total Gross");
                            fcellTotGross.CellStyle = styleGrey40;
                            ICell fcellTotGrosss = frowSecond.CreateCell(ftotalCount + 1);
                            fcellTotGrosss.SetCellValue(Math.Round(Final_TotalGrossTotal, MidpointRounding.AwayFromZero).ToString());
                            fcellTotGrosss.CellStyle = styleGrey40;

                            ICell fcellTotPF = frow.CreateCell(ftotalCount + 2);
                            fcellTotPF.SetCellValue("Total PF");
                            fcellTotPF.CellStyle = styleGrey50;
                            ICell fcellTotPFs = frowSecond.CreateCell(ftotalCount + 2);
                            fcellTotPFs.SetCellValue(Math.Round(Final_TotalPF, MidpointRounding.AwayFromZero).ToString());
                            fcellTotPFs.CellStyle = styleGrey50;

                            ICell fcellTotEsic = frow.CreateCell(ftotalCount + 3);
                            fcellTotEsic.SetCellValue("Total ESIC");
                            fcellTotEsic.CellStyle = styleGrey50;
                            ICell fcellTotEsics = frowSecond.CreateCell(ftotalCount + 3);
                            fcellTotEsics.SetCellValue(Math.Round(Final_TotalESIC, MidpointRounding.AwayFromZero).ToString());
                            fcellTotEsics.CellStyle = styleGrey50;

                            int ftotalCount1 = ftotalCount + 3;
                            if (IsCRI_ProfessionalTax)
                            {
                                ftotalCount1 = ftotalCount1 + 1;
                                ICell cellTot9 = frow.CreateCell(ftotalCount1);
                                cellTot9.SetCellValue("Total Professional Tax");
                                cellTot9.CellStyle = styleGrey50;
                                ICell cellTot9s = frowSecond.CreateCell(ftotalCount1);
                                cellTot9s.SetCellValue(Math.Round(Final_TotalProfTax, MidpointRounding.AwayFromZero).ToString());
                                cellTot9s.CellStyle = styleGrey50;
                            }
                            if (IsCRI_RevenueDeduction)
                            {
                                ftotalCount1 = ftotalCount1 + 1;
                                ICell cellTot10 = frow.CreateCell(ftotalCount1);
                                cellTot10.SetCellValue("Total Revenue Deduction");
                                cellTot10.CellStyle = styleGrey50;
                                ICell cellTot10s = frowSecond.CreateCell(ftotalCount1);
                                cellTot10s.SetCellValue(Math.Round(Final_TotalRevenue, MidpointRounding.AwayFromZero).ToString());
                                cellTot10s.CellStyle = styleGrey50;
                            }
                            if (IsCRI_CanteenFacility)
                            {
                                ftotalCount1 = ftotalCount1 + 1;
                                ICell cellTot11 = frow.CreateCell(ftotalCount1);
                                cellTot11.SetCellValue("Total Canteen Facility");
                                cellTot11.CellStyle = styleGrey50;
                                ICell cellTot11s = frowSecond.CreateCell(ftotalCount1);
                                cellTot11s.SetCellValue(Math.Round(Final_TotalCanteenFacility, MidpointRounding.AwayFromZero).ToString());
                                cellTot11s.CellStyle = styleGrey50;
                            }

                            ICell cellTotAdvance = frow.CreateCell(ftotalCount1 + 1);
                            cellTotAdvance.SetCellValue("Total Advance Installment");
                            cellTotAdvance.CellStyle = styleGrey50;
                            ICell cellTotAdvances = frowSecond.CreateCell(ftotalCount1 + 1);
                            cellTotAdvances.SetCellValue(Math.Round(Final_TotalAdvance, MidpointRounding.AwayFromZero).ToString());
                            cellTotAdvances.CellStyle = styleGrey50;

                            int fTotCount = ftotalCount1 + 2;
                            if (IsDES_Include_LWF)
                            {
                                ICell cellTotLWF = frow.CreateCell(fTotCount);
                                cellTotLWF.SetCellValue("Total MLWF Deduction");
                                cellTotLWF.CellStyle = styleGrey50;
                                ICell cellTotLWFs = frowSecond.CreateCell(fTotCount);
                                cellTotLWFs.SetCellValue(Math.Round(Final_TotalLWF, MidpointRounding.AwayFromZero).ToString());
                                cellTotLWFs.CellStyle = styleGrey50;
                                fTotCount = fTotCount + 1;
                            }

                            ICell fcellTotDeduct = frow.CreateCell(fTotCount);
                            fcellTotDeduct.SetCellValue("Deduction");
                            fcellTotDeduct.CellStyle = styleGrey50;
                            ICell fcellTotDeducts = frowSecond.CreateCell(fTotCount);
                            fcellTotDeducts.SetCellValue(Math.Round(Final_TotalDeduct, MidpointRounding.AwayFromZero).ToString());
                            fcellTotDeducts.CellStyle = styleGrey50;

                            ICell fcellTotFinal = frow.CreateCell(fTotCount + 1);
                            fcellTotFinal.SetCellValue("Grand Total");
                            fcellTotFinal.CellStyle = styleGrey80;
                            ICell fcellTotFinals = frowSecond.CreateCell(fTotCount + 1);
                            fcellTotFinals.SetCellValue(Math.Round(Final_TotalFinal, MidpointRounding.AwayFromZero).ToString());
                            fcellTotFinals.CellStyle = styleGrey80;
                                   
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
            Wage_Process wage_Process = wageProcess.getWageProcessById(WAG_Id);
            DateTime wageMonth = wage_Process.WAG_Month;
            string WAG_Month = wageMonth.ToString("MMMM").ToUpper() + "-" + wageMonth.ToString("yyyy");

            string newPath = GetTempFolderPath(_hostingEnvironment.WebRootPath);
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
                            var distinceDesignations = item.wageRegisterVMs.Select(q => new { q.designation.DES_Id, q.designation.DES_Title, q.designation.DES_Exclude_LWF }).Distinct();

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
                            CellHeading.SetCellValue(wage_Process.FRM_.FRM_Name.ToUpper());
                            excelSheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 28));
                            CellHeading.CellStyle = styleHeading;
                            CellUtil.SetAlignment(CellHeading, workbook, (short)HorizontalAlignment.Center);

                            IRow rowAdd1 = excelSheet.CreateRow(DesCount + 1);
                            ICell CellAdd1 = rowAdd1.CreateCell(0);
                            CellAdd1.SetCellValue(wage_Process.FRM_.FRM_Address1.ToUpper() + "," + wage_Process.FRM_.FRM_Address2.ToUpper() + ",");
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
                            CellClient.SetCellValue(client.client.CLI_Name.ToString());
                            CellClient.CellStyle = styleDesignation;
                            CellUtil.SetAlignment(CellClient, workbook, (short)HorizontalAlignment.Center);
                            excelSheet.AddMergedRegion(new CellRangeAddress(DesCount + 3, DesCount + 3, 0, 28));
                            #endregion

                            DesCount = DesCount + 4;

                            double Final_TotalPaybleDays = 0, Final_TotalOTHrs = 0;
                            decimal Final_TotalBasic = 0M, Final_TotalDA = 0M, Final_TotalHRA = 0M, Final_TotalOT = 0M, Final_TotalGrossTotal = 0M, Final_TotalPF = 0M, Final_TotalESIC = 0M;
                            decimal Final_TotalProfTax = 0m, Final_TotalRevenue = 0M, Final_TotalCanteenFacility = 0M, Final_TotalAdvance = 0M, Final_TotalDeduct = 0M, Final_TotalFinal = 0M;
                            decimal Final_TotalOutStation = 0M, Final_TotalAttendance = 0M, Final_TotalNightshift = 0M, Final_TotalPerformance = 0M, Final_TotalLWF = 0M;
                            decimal Final_TotalAllowance_1 = 0M, Final_TotalAllowance_2 = 0M, Final_TotalAllowance_3 = 0M, Final_TotalAllowance_4 = 0M, Final_TotalAllowance_5 = 0M;
                            bool IsCRI_OutStation_Allowance = false, IsCRI_Attendance_Allowance = false, IsCRI_Nightshift_Allowance = false, IsCRI_Performance_Allowance = false, IsCRI_OT_Calculate_Payableday = false;
                            bool IsCRI_Allowance_1 = false, IsCRI_Allowance_2 = false, IsCRI_Allowance_3 = false, IsCRI_Allowance_4 = false, IsCRI_Allowance_5 = false;
                            bool IsCRI_ProfessionalTax = false, IsCRI_RevenueDeduction = false, IsCRI_CanteenFacility = false, IsDES_Include_LWF = false;
                            List<WageAllowancesTotalVM> WageAllowancesTotalVMs = new List<WageAllowancesTotalVM>();
                            //added on 26th oct 2020
                            List<CustomAllowanceVM> allowanceVMs = new List<CustomAllowanceVM>();
                            IEnumerable<ClientRequirementVM> clientReqs = item.wageRegisterVMs.Select(m => m.clientRequirementVM);
                            string[] all1 = clientReqs.Select(m => m.CRI_Allowance_Name_1).Distinct().ToArray();
                            string[] all2 = clientReqs.Select(m => m.CRI_Allowance_Name_2).Distinct().ToArray();
                            string[] all3 = clientReqs.Select(m => m.CRI_Allowance_Name_3).Distinct().ToArray();
                            string[] all4 = clientReqs.Select(m => m.CRI_Allowance_Name_4).Distinct().ToArray();
                            string[] all5 = clientReqs.Select(m => m.CRI_Allowance_Name_5).Distinct().ToArray();

                            
                            List<string> allCustomAallownces = new List<string>();
                            allCustomAallownces.AddRange(all1.ToList());
                            allCustomAallownces.AddRange(all2.ToList());
                            allCustomAallownces.AddRange(all3.ToList());
                            allCustomAallownces.AddRange(all4.ToList());
                            allCustomAallownces.AddRange(all5.ToList());

                            foreach(var all in allCustomAallownces.Distinct().Where(m => m != null))
                            {
                                allowanceVMs.Add(new CustomAllowanceVM() { DES_Id = 0, C_ALL_Title = all, C_ALL_Value = 0 });
                            }

                            //end
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
                                excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount, 0, 28));
                                CellHeader.CellStyle = styleTotal;

                                row = excelSheet.CreateRow(DesCount + 1);
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
                                List<string> allowance = new List<string>();
                                int i = 0;
                                foreach (var all in wageRegisters[0].allowanceVMs.OrderBy(m => m.allowanceVM.ALL_Id))
                                {
                                    arrAllowancesTotal[i] = 0;
                                    allowance.Add(all.allowanceVM.ALL_Alias);
                                    ICell cellAll = row.CreateCell(cell + 1);
                                    cellAll.SetCellValue(all.allowanceVM.ALL_Alias);
                                    excelSheet.SetColumnWidth(cell + 1, (int)((25 + 0.72) * 140));
                                    cellAll.CellStyle = styleGrey40;
                                    cell++; i++;
                                }
                                //custom allowance
                                foreach (CustomAllowanceVM allownce in allowanceVMs)
                                {
                                    cell = cell + 1;
                                    ICell celOutAllow = row.CreateCell(cell);
                                    celOutAllow.SetCellValue(allownce.C_ALL_Title);
                                    celOutAllow.CellStyle = styleGrey40;
                                }
                                //if (clientRequirement.CRI_Allowance_1 == true)
                                //{
                                //    cell = cell + 1;
                                //    ICell celOutAllow = row.CreateCell(cell);
                                //    celOutAllow.SetCellValue(clientRequirement.CRI_Allowance_Name_1);
                                //    celOutAllow.CellStyle = styleGrey40;
                                //}
                                //if (clientRequirement.CRI_Allowance_2 == true)
                                //{
                                //    cell = cell + 1;
                                //    ICell celOutAllow = row.CreateCell(cell);
                                //    celOutAllow.SetCellValue(clientRequirement.CRI_Allowance_Name_2);
                                //    celOutAllow.CellStyle = styleGrey40;
                                //}
                                //if (clientRequirement.CRI_Allowance_3 == true)
                                //{
                                //    cell = cell + 1;
                                //    ICell celOutAllow = row.CreateCell(cell);
                                //    celOutAllow.SetCellValue(clientRequirement.CRI_Allowance_Name_3);
                                //    celOutAllow.CellStyle = styleGrey40;
                                //}
                                //if (clientRequirement.CRI_Allowance_4 == true)
                                //{
                                //    cell = cell + 1;
                                //    ICell celOutAllow = row.CreateCell(cell);
                                //    celOutAllow.SetCellValue(clientRequirement.CRI_Allowance_Name_4);
                                //    celOutAllow.CellStyle = styleGrey40;
                                //}
                                //if (clientRequirement.CRI_Allowance_5 == true)
                                //{
                                //    cell = cell + 1;
                                //    ICell celOutAllow = row.CreateCell(cell);
                                //    celOutAllow.SetCellValue(clientRequirement.CRI_Allowance_Name_5);
                                //    celOutAllow.CellStyle = styleGrey40;
                                //}
                                //end custom allowance
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
                                if (!clientRequirement.CRI_OT_Calculate_Payableday)
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
                                ICell cell17 = row.CreateCell(cellNext1);
                                cell17.SetCellValue("Advance Installment");
                                excelSheet.SetColumnWidth(cellNext1, (int)((25 + 0.72) * 140));
                                cell17.CellStyle = styleGrey50;

                                int cellLWFNext = cellNext1 + 1;
                                if (!dd.DES_Exclude_LWF)
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

                                DesCount = DesCount + 2;
                                double TotalPaybleDays = 0, TotalOTHrs = 0;
                                decimal TotalBasic = 0M, TotalDA = 0M, TotalHRA = 0M, TotalOT = 0M, TotalGrossTotal = 0M, TotalPF = 0M, TotalESIC = 0M, TotalProfTax = 0m, TotalRevenue = 0M, TotalCanteenFacility = 0M, TotalAdvance = 0M, TotalDeduct = 0M, TotalFinal = 0M;
                                decimal TotalOutStation = 0M, TotalAttendance = 0M, TotalNightshift = 0M, TotalPerformance = 0M, TotalLWF = 0M;
                                decimal TotalAllowance1 = -1M, TotalAllowance2 = -1M, TotalAllowance3 = -1M, TotalAllowance4 = -1M, TotalAllowance5 = -1M;
                                int srno = 0;
                                foreach (var employee in wageRegisters)
                                {
                                    srno = srno + 1;
                                    #region Employee Data

                                    row = excelSheet.CreateRow(DesCount);
                                    ICell cellEmp0 = row.CreateCell(0);
                                    cellEmp0.SetCellValue(srno);
                                    cellEmp0.CellStyle = styleGrey25;

                                    ICell cellEmp1 = row.CreateCell(1);
                                    cellEmp1.SetCellValue(employee.employeeVM.EMP_Id.ToString("D5"));
                                    cellEmp1.CellStyle = styleGrey25;

                                    ICell cellEmp2 = row.CreateCell(2);
                                    cellEmp2.SetCellValue(employee.employeeVM.EMP_ESIC_Number);
                                    cellEmp2.CellStyle = styleGrey25;

                                    ICell cellEmp3 = row.CreateCell(3);
                                    cellEmp3.SetCellValue(employee.employeeVM.EMP_UAN_Number);
                                    cellEmp3.CellStyle = styleGrey25;

                                    ICell cellEmp4 = row.CreateCell(4);
                                    cellEmp4.SetCellValue(employee.employeeVM.EMP_FirstName + " " + employee.employeeVM.EMP_MiddleName + " " + employee.employeeVM.EMP_SurName);
                                    cellEmp4.CellStyle = styleGrey25;

                                    ICell cellEmp5 = row.CreateCell(5);
                                    cellEmp5.SetCellValue(employee.employeeVM.EMP_Designation);
                                    cellEmp5.CellStyle = styleGrey25;

                                    ICell cellEmp6 = row.CreateCell(6);
                                    cellEmp6.SetCellValue(Convert.ToBoolean(employee.employeeVM.EMP_Gender) == true ? "M" : "F");
                                    cellEmp6.CellStyle = styleGrey25;

                                    ICell cellEmp7 = row.CreateCell(7);
                                    cellEmp7.SetCellValue(DateHelper.getDateWithFormat(employee.employeeVM.EMP_DateOfJoining));
                                    cellEmp7.CellStyle = styleGrey25;

                                    ICell cellEmp8 = row.CreateCell(8);
                                    cellEmp8.SetCellValue(employee.WAR_TotalWorkingDays);
                                    cellEmp8.CellStyle = styleGrey25;

                                    ICell cellEmp9 = row.CreateCell(9);
                                    cellEmp9.SetCellValue(employee.WAR_TotalPaybleDays);
                                    cellEmp9.CellStyle = styleGrey25;
                                    TotalPaybleDays = TotalPaybleDays + employee.WAR_TotalPaybleDays;

                                    ICell cellEmp10 = row.CreateCell(10);
                                    cellEmp10.SetCellValue(Math.Round(employee.WAR_Basic_Calculated, MidpointRounding.AwayFromZero).ToString());
                                    cellEmp10.CellStyle = styleGrey40;
                                    TotalBasic = TotalBasic + Math.Round(employee.WAR_Basic_Calculated, MidpointRounding.AwayFromZero);

                                    ICell cellEmp11 = row.CreateCell(11);
                                    cellEmp11.SetCellValue(Math.Round(employee.WAR_DA_Calculated, MidpointRounding.AwayFromZero).ToString());
                                    cellEmp11.CellStyle = styleGrey40;
                                    TotalDA = TotalDA + Math.Round(employee.WAR_DA_Calculated, MidpointRounding.AwayFromZero);

                                    ICell cellEmp12 = row.CreateCell(12);
                                    cellEmp12.SetCellValue(Math.Round(employee.WAR_HRA_Calculated, MidpointRounding.AwayFromZero).ToString());
                                    TotalHRA = TotalHRA + Math.Round(employee.WAR_HRA_Calculated, MidpointRounding.AwayFromZero);
                                    cellEmp12.CellStyle = styleGrey40;

                                    int cellAllowance = 12;
                                    int j = 0;

                                    foreach (var all in employee.allowanceVMs.OrderBy(m => m.allowanceVM.ALL_Id))
                                    {
                                        arrAllowancesTotal[j] = arrAllowancesTotal[j] + all.WAA_Amount_Calculated;
                                        ICell cellEmpAll = row.CreateCell(cellAllowance + 1);
                                        cellEmpAll.SetCellValue(Math.Round(all.WAA_Amount_Calculated, MidpointRounding.AwayFromZero).ToString());
                                        cellEmpAll.CellStyle = styleGrey40;
                                        cellAllowance++;
                                        j++;
                                    }
                                    int cellNxt = cellAllowance + 1;



                                    //custom allowance value                                    
                                    foreach (CustomAllowanceVM allownce in allowanceVMs)
                                    {
                                        allownce.DES_Id = dd.DES_Id;
                                        ICell cellCustom = row.CreateCell(cellNxt);
                                        decimal WAR_Allowance_Calculated = 0M;

                                        if (employee.clientRequirementVM.CRI_Allowance_Name_1 == allownce.C_ALL_Title)
                                        {
                                            WAR_Allowance_Calculated = Math.Round(employee.WAR_Allowance_Calculated_1!=null? employee.WAR_Allowance_Calculated_1.Value:0, MidpointRounding.AwayFromZero);
                                            cellCustom.SetCellValue(WAR_Allowance_Calculated.ToString());
                                           // TotalAllowance1 = TotalAllowance1 + WAR_Allowance_Calculated;
                                            allownce.C_ALL_Value = allownce.C_ALL_Value + WAR_Allowance_Calculated;
                                        }
                                        else
                                        if (employee.clientRequirementVM.CRI_Allowance_Name_2 == allownce.C_ALL_Title)
                                        {
                                            WAR_Allowance_Calculated = Math.Round(employee.WAR_Allowance_Calculated_2!=null?employee.WAR_Allowance_Calculated_2.Value:0, MidpointRounding.AwayFromZero);
                                            cellCustom.SetCellValue(WAR_Allowance_Calculated.ToString());
                                            // TotalAllowance2 = TotalAllowance2 + WAR_Allowance_Calculated;
                                            allownce.C_ALL_Value = allownce.C_ALL_Value + WAR_Allowance_Calculated;
                                        }
                                        else
                                        if (employee.clientRequirementVM.CRI_Allowance_Name_3 == allownce.C_ALL_Title)
                                        {
                                            WAR_Allowance_Calculated = Math.Round(employee.WAR_Allowance_Calculated_3!=null?employee.WAR_Allowance_Calculated_3.Value:0, MidpointRounding.AwayFromZero);
                                            cellCustom.SetCellValue(WAR_Allowance_Calculated.ToString());
                                            //TotalAllowance3 = TotalAllowance3 + WAR_Allowance_Calculated;
                                            allownce.C_ALL_Value = allownce.C_ALL_Value + WAR_Allowance_Calculated;
                                        }
                                        else
                                        if (employee.clientRequirementVM.CRI_Allowance_Name_4 == allownce.C_ALL_Title)
                                        {
                                            WAR_Allowance_Calculated = Math.Round(employee.WAR_Allowance_Calculated_4!=null?employee.WAR_Allowance_Calculated_4.Value:0, MidpointRounding.AwayFromZero);
                                            cellCustom.SetCellValue(WAR_Allowance_Calculated.ToString());
                                            //TotalAllowance4 = TotalAllowance4 + WAR_Allowance_Calculated;
                                            allownce.C_ALL_Value = allownce.C_ALL_Value + WAR_Allowance_Calculated;
                                        }
                                        else
                                        if (employee.clientRequirementVM.CRI_Allowance_Name_5 == allownce.C_ALL_Title)
                                        {
                                            WAR_Allowance_Calculated = Math.Round(employee.WAR_Allowance_Calculated_5!=null?employee.WAR_Allowance_Calculated_5.Value:0, MidpointRounding.AwayFromZero);
                                            cellCustom.SetCellValue(WAR_Allowance_Calculated.ToString());
                                            //TotalAllowance5 = TotalAllowance5 + WAR_Allowance_Calculated;
                                            allownce.C_ALL_Value = allownce.C_ALL_Value + WAR_Allowance_Calculated;
                                        }
                                        else
                                        {
                                            cellCustom.SetCellValue(0);
                                        }
                                        cellCustom.CellStyle = styleGrey40;
                                        cellNxt = cellNxt + 1;
                                    }


                                    //********************************************
                                    //if (clientRequirement.CRI_Allowance_1 == true)
                                    //{
                                    //    ICell cellOutstation = row.CreateCell(cellNxt);
                                    //    decimal WAR_Allowance_Calculated_1 = 0M;
                                    //    if (employee.WAR_Allowance_Calculated_1 != null)
                                    //    {
                                    //        WAR_Allowance_Calculated_1 = Math.Round(employee.WAR_Allowance_Calculated_1.Value, MidpointRounding.AwayFromZero);
                                    //    }
                                    //    cellOutstation.SetCellValue(WAR_Allowance_Calculated_1.ToString());
                                    //    TotalAllowance1 = TotalAllowance1 + WAR_Allowance_Calculated_1;
                                    //    cellOutstation.CellStyle = styleGrey40;
                                    //    cellNxt = cellNxt + 1;
                                    //}
                                    //if (clientRequirement.CRI_Allowance_2 == true)
                                    //{
                                    //    ICell cellOutstation = row.CreateCell(cellNxt);
                                    //    decimal WAR_Allowance_Calculated_2 = 0M;
                                    //    if (employee.WAR_Allowance_Calculated_2 != null)
                                    //    {
                                    //        WAR_Allowance_Calculated_2 = Math.Round(employee.WAR_Allowance_Calculated_2.Value, MidpointRounding.AwayFromZero);
                                    //    }
                                    //    cellOutstation.SetCellValue(WAR_Allowance_Calculated_2.ToString());
                                    //    TotalAllowance2 = TotalAllowance2 + WAR_Allowance_Calculated_2;
                                    //    cellOutstation.CellStyle = styleGrey40;
                                    //    cellNxt = cellNxt + 1;
                                    //}
                                    //if (clientRequirement.CRI_Allowance_3 == true)
                                    //{
                                    //    ICell cellOutstation = row.CreateCell(cellNxt);
                                    //    decimal WAR_Allowance_Calculated_3 = 0M;
                                    //    if (employee.WAR_Allowance_Calculated_3 != null)
                                    //    {
                                    //        WAR_Allowance_Calculated_3 = Math.Round(employee.WAR_Allowance_Calculated_3.Value, MidpointRounding.AwayFromZero);
                                    //    }
                                    //    cellOutstation.SetCellValue(WAR_Allowance_Calculated_3.ToString());
                                    //    TotalAllowance3 = TotalAllowance3 + WAR_Allowance_Calculated_3;
                                    //    cellOutstation.CellStyle = styleGrey40;
                                    //    cellNxt = cellNxt + 1;
                                    //}
                                    //if (clientRequirement.CRI_Allowance_4 == true)
                                    //{
                                    //    ICell cellOutstation = row.CreateCell(cellNxt);
                                    //    decimal WAR_Allowance_Calculated_4 = 0M;
                                    //    if (employee.WAR_Allowance_Calculated_4 != null)
                                    //    {
                                    //        WAR_Allowance_Calculated_4 = Math.Round(employee.WAR_Allowance_Calculated_4.Value, MidpointRounding.AwayFromZero);
                                    //    }
                                    //    cellOutstation.SetCellValue(WAR_Allowance_Calculated_4.ToString());
                                    //    TotalAllowance4 = TotalAllowance4 + WAR_Allowance_Calculated_4;
                                    //    cellOutstation.CellStyle = styleGrey40;
                                    //    cellNxt = cellNxt + 1;
                                    //}
                                    //if (clientRequirement.CRI_Allowance_5 == true)
                                    //{
                                    //    ICell cellOutstation = row.CreateCell(cellNxt);
                                    //    decimal WAR_Allowance_Calculated_5 = 0M;
                                    //    if (employee.WAR_Allowance_Calculated_5 != null)
                                    //    {
                                    //        WAR_Allowance_Calculated_5 = Math.Round(employee.WAR_Allowance_Calculated_5.Value, MidpointRounding.AwayFromZero);
                                    //    }
                                    //    cellOutstation.SetCellValue(WAR_Allowance_Calculated_5.ToString());
                                    //    TotalAllowance5 = TotalAllowance5 + WAR_Allowance_Calculated_5;
                                    //    cellOutstation.CellStyle = styleGrey40;
                                    //    cellNxt = cellNxt + 1;
                                    //}
                                    //end cutsom allowance

                                    #region New allowances 29th july
                                    if (clientRequirement.CRI_OutStation_Allowance == true)
                                    {
                                        ICell cellOutstation = row.CreateCell(cellNxt);
                                        decimal WAR_OutStation_Allowance_Calculated = 0M;
                                        if (employee.WAR_OutStation_Allowance_Calculated != null)
                                        {
                                            WAR_OutStation_Allowance_Calculated = Math.Round(employee.WAR_OutStation_Allowance_Calculated.Value, MidpointRounding.AwayFromZero);
                                        }
                                        cellOutstation.SetCellValue(WAR_OutStation_Allowance_Calculated.ToString());
                                        TotalOutStation = TotalOutStation + WAR_OutStation_Allowance_Calculated;
                                        cellOutstation.CellStyle = styleGrey40;
                                        cellNxt = cellNxt + 1;
                                    }
                                    if (clientRequirement.CRI_Attendance_Allowance == true)
                                    {
                                        ICell cellAttendance = row.CreateCell(cellNxt);
                                        decimal WAR_Attendance_Allowance_Calculated = 0M;
                                        if (employee.WAR_Attendance_Allowance_Calculated != null)
                                        {
                                            WAR_Attendance_Allowance_Calculated = Math.Round(employee.WAR_Attendance_Allowance_Calculated.Value, MidpointRounding.AwayFromZero);
                                        }
                                        cellAttendance.SetCellValue(WAR_Attendance_Allowance_Calculated.ToString());
                                        TotalAttendance = TotalAttendance + WAR_Attendance_Allowance_Calculated;
                                        cellAttendance.CellStyle = styleGrey40;
                                        cellNxt = cellNxt + 1;
                                    }
                                    if (clientRequirement.CRI_Nightshift_Allowance == true)
                                    {
                                        ICell cellNightshift = row.CreateCell(cellNxt);
                                        decimal WAR_Nightshift_Allowance_Calculated = 0M;
                                        if (employee.WAR_Nightshift_Allowance_Calculated != null)
                                        {
                                            WAR_Nightshift_Allowance_Calculated = Math.Round(employee.WAR_Nightshift_Allowance_Calculated.Value, MidpointRounding.AwayFromZero);
                                        }
                                        cellNightshift.SetCellValue(WAR_Nightshift_Allowance_Calculated.ToString());
                                        TotalNightshift = TotalNightshift + WAR_Nightshift_Allowance_Calculated;
                                        cellNightshift.CellStyle = styleGrey40;
                                        cellNxt = cellNxt + 1;
                                    }
                                    if (clientRequirement.CRI_Performance_Allowance == true)
                                    {
                                        ICell cellPerformance = row.CreateCell(cellNxt);
                                        decimal WAR_Performance_Allowance_Calculated = 0M;
                                        if (employee.WAR_Performance_Allowance_Calculated != null)
                                        {
                                            WAR_Performance_Allowance_Calculated = Math.Round(employee.WAR_Performance_Allowance_Calculated.Value, MidpointRounding.AwayFromZero);
                                        }
                                        cellPerformance.SetCellValue(WAR_Performance_Allowance_Calculated.ToString());
                                        TotalPerformance = TotalPerformance + WAR_Performance_Allowance_Calculated;
                                        cellPerformance.CellStyle = styleGrey40;
                                        cellNxt = cellNxt + 1;
                                    }
                                    if (!clientRequirement.CRI_OT_Calculate_Payableday)
                                    {
                                        ICell cellEmpotHrs = row.CreateCell(cellNxt);
                                        cellEmpotHrs.SetCellValue(employee.WAR_ExtraWorkingHours);
                                        TotalOTHrs = TotalOTHrs + employee.WAR_ExtraWorkingHours;
                                        cellEmpotHrs.CellStyle = styleGrey40;
                                        cellNxt = cellNxt + 1;

                                        ICell cellEmp13 = row.CreateCell(cellNxt);
                                        cellEmp13.SetCellValue(Math.Round(employee.WAR_OverTime_Calculated, MidpointRounding.AwayFromZero).ToString());
                                        TotalOT = TotalOT + Math.Round(employee.WAR_OverTime_Calculated, MidpointRounding.AwayFromZero);
                                        cellEmp13.CellStyle = styleGrey40;
                                        cellNxt = cellNxt + 1;
                                    }
                                    #endregion                                    
                                    ICell cellEmp14 = row.CreateCell(cellNxt);
                                    cellEmp14.SetCellValue(Math.Round(employee.WAR_GrossTotal, MidpointRounding.AwayFromZero).ToString());
                                    TotalGrossTotal = TotalGrossTotal + Math.Round(employee.WAR_GrossTotal, MidpointRounding.AwayFromZero);
                                    cellEmp14.CellStyle = styleGrey40;
                                    ICell cellEmp15 = row.CreateCell(cellNxt + 1);
                                    cellEmp15.SetCellValue(Math.Round(employee.WAR_PF_Calculated, MidpointRounding.AwayFromZero).ToString());
                                    TotalPF = TotalPF + Math.Round(employee.WAR_PF_Calculated, MidpointRounding.AwayFromZero);
                                    cellEmp15.CellStyle = styleGrey50;
                                    ICell cellEmp16 = row.CreateCell(cellNxt + 2);
                                    cellEmp16.SetCellValue(Math.Round(employee.WAR_ESIC_Calculated, MidpointRounding.AwayFromZero).ToString());
                                    TotalESIC = TotalESIC + Math.Round(employee.WAR_ESIC_Calculated, MidpointRounding.AwayFromZero);
                                    cellEmp16.CellStyle = styleGrey50;
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

                                    ICell cellEmp17 = row.CreateCell(cellNxt1 + 1);
                                    cellEmp17.SetCellValue(Math.Round(employee.WAR_Advance_Amount, MidpointRounding.AwayFromZero).ToString());
                                    TotalAdvance = TotalAdvance + Math.Round(employee.WAR_Advance_Amount, MidpointRounding.AwayFromZero);
                                    cellEmp17.CellStyle = styleGrey50;

                                    int cellNext2 = cellNxt1 + 2;
                                    //TotalLWF                                    
                                    if (!dd.DES_Exclude_LWF)
                                    {
                                        ICell cell_LWF = row.CreateCell(cellNext2);
                                        cell_LWF.SetCellValue(Convert.ToString(Math.Round((employee.WAR_LWF_Deduction_Employee != null ? employee.WAR_LWF_Deduction_Employee.Value : 0), MidpointRounding.AwayFromZero).ToString()));
                                        excelSheet.SetColumnWidth(cellNext2, (int)((25 + 0.72) * 140));
                                        cell_LWF.CellStyle = styleGrey50;
                                        TotalLWF = TotalLWF + Convert.ToDecimal(employee.WAR_LWF_Deduction_Employee);
                                        cellNext2 = cellNext2 + 1;
                                    }

                                    #region Total Deduction
                                    decimal DeductTotal = Math.Round(employee.WAR_PF_Calculated, MidpointRounding.AwayFromZero) + Math.Round(employee.WAR_ESIC_Calculated, MidpointRounding.AwayFromZero) + Math.Round(Convert.ToDecimal(employee.WAR_ProffesionalTax_Calculated), MidpointRounding.AwayFromZero)
                                        + Math.Round(employee.WAR_Advance_Amount, MidpointRounding.AwayFromZero) + Math.Round((employee.WAR_LWF_Deduction_Employee != null ? employee.WAR_LWF_Deduction_Employee.Value : 0), MidpointRounding.AwayFromZero);
                                    if (employee.WAR_RevenueDeduction_Calculated != "-")
                                    {
                                        DeductTotal += Math.Round(Convert.ToDecimal(employee.WAR_RevenueDeduction_Calculated), MidpointRounding.AwayFromZero);
                                    }
                                    if (employee.WAR_CanteenFacility_Calculation != "-")
                                    {
                                        DeductTotal += Math.Round(Convert.ToDecimal(employee.WAR_CanteenFacility_Calculation), MidpointRounding.AwayFromZero);
                                    }
                                    #endregion

                                    ICell cellEmp18 = row.CreateCell(cellNext2);
                                    cellEmp18.SetCellValue(DeductTotal.ToString());
                                    cellEmp18.CellStyle = styleGrey50;
                                    TotalDeduct = TotalDeduct + DeductTotal;
                                    ICell cellEmp19 = row.CreateCell(cellNext2 + 1);
                                    cellEmp19.SetCellValue(Math.Round(employee.WAR_FinalTotal, MidpointRounding.AwayFromZero).ToString());
                                    TotalFinal = TotalFinal + Math.Round(employee.WAR_FinalTotal, MidpointRounding.AwayFromZero);
                                    cellEmp19.CellStyle = styleGrey80;
                                    ICell cellEmp20 = row.CreateCell(cellNext2 + 2);
                                    cellEmp20.SetCellValue("");
                                    cellEmp20.CellStyle = styleGrey80;
                                    #endregion
                                    DesCount++;
                                }

                                #region Total line in Excel
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

                                int totalCount = 8;
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

                                foreach (var all in wageRegisters[0].allowanceVMs.OrderBy(m => m.allowanceVM.ALL_Id))
                                {
                                    WageAllowancesTotalVM wageTotalVM = new WageAllowancesTotalVM();
                                    wageTotalVM.ALL_Id = all.allowanceVM.ALL_Id;
                                    wageTotalVM.Parameter = all.allowanceVM.ALL_Alias;
                                    wageTotalVM.Value = arrAllowancesTotal[k];
                                    WageAllowancesTotalVMs.Add(wageTotalVM);
                                    ICell cellEmpAll = row.CreateCell(cellAllow + 1);
                                    cellEmpAll.SetCellValue(Math.Round(arrAllowancesTotal[k], MidpointRounding.AwayFromZero).ToString());
                                    cellEmpAll.CellStyle = styleGrey40;
                                    cellAllow++;
                                    k++;
                                }

                                totalCount = cellAllow;

                                //custom allow
                                foreach (CustomAllowanceVM allownce in allowanceVMs)
                                {
                                    totalCount = totalCount + 1;
                                    ICell cellTotAllowance_1 = row.CreateCell(totalCount);
                                    cellTotAllowance_1.CellStyle = styleGrey40;
                                    cellTotAllowance_1.SetCellValue(Math.Round(allownce.C_ALL_Value, MidpointRounding.AwayFromZero).ToString());
                                }

                                //if (clientRequirement.CRI_Allowance_1 == true)
                                //{
                                //    IsCRI_Allowance_1 = true;
                                //    totalCount = totalCount + 1;
                                //    ICell cellTotAllowance_1 = row.CreateCell(totalCount);
                                //    cellTotAllowance_1.CellStyle = styleGrey40;
                                //    cellTotAllowance_1.SetCellValue(Math.Round(TotalAllowance1, MidpointRounding.AwayFromZero).ToString());
                                //}
                                //if (clientRequirement.CRI_Allowance_2 == true)
                                //{
                                //    IsCRI_Allowance_2 = true;
                                //    totalCount = totalCount + 1;
                                //    ICell cellTotAllowance_2 = row.CreateCell(totalCount);
                                //    cellTotAllowance_2.CellStyle = styleGrey40;
                                //    cellTotAllowance_2.SetCellValue(Math.Round(TotalAllowance2, MidpointRounding.AwayFromZero).ToString());
                                //}
                                //if (clientRequirement.CRI_Allowance_3 == true)
                                //{
                                //    IsCRI_Allowance_3 = true;
                                //    totalCount = totalCount + 1;
                                //    ICell cellTotAllowance_3 = row.CreateCell(totalCount);
                                //    cellTotAllowance_3.CellStyle = styleGrey40;
                                //    cellTotAllowance_3.SetCellValue(Math.Round(TotalAllowance3, MidpointRounding.AwayFromZero).ToString());
                                //}
                                //if (clientRequirement.CRI_Allowance_4 == true)
                                //{
                                //    IsCRI_Allowance_4 = true;
                                //    totalCount = totalCount + 1;
                                //    ICell cellTotAllowance_4 = row.CreateCell(totalCount);
                                //    cellTotAllowance_4.CellStyle = styleGrey40;
                                //    cellTotAllowance_4.SetCellValue(Math.Round(TotalAllowance4, MidpointRounding.AwayFromZero).ToString());
                                //}
                                //if (clientRequirement.CRI_Allowance_5 == true)
                                //{
                                //    IsCRI_Allowance_1 = true;
                                //    totalCount = totalCount + 1;
                                //    ICell cellTotAllowance_5 = row.CreateCell(totalCount);
                                //    cellTotAllowance_5.CellStyle = styleGrey40;
                                //    cellTotAllowance_5.SetCellValue(Math.Round(TotalAllowance5, MidpointRounding.AwayFromZero).ToString());
                                //}
                                //end custom allow

                                #region New total allowances 29th july
                                if (clientRequirement.CRI_OutStation_Allowance == true)
                                {
                                    IsCRI_OutStation_Allowance = true;
                                    totalCount = totalCount + 1;
                                    ICell cellTotOutstation = row.CreateCell(totalCount);
                                    cellTotOutstation.CellStyle = styleGrey40;
                                    cellTotOutstation.SetCellValue(Math.Round(TotalOutStation, MidpointRounding.AwayFromZero).ToString());
                                }
                                if (clientRequirement.CRI_Attendance_Allowance == true)
                                {
                                    IsCRI_Attendance_Allowance = true;
                                    totalCount = totalCount + 1;
                                    ICell cellTotAttendance = row.CreateCell(totalCount);
                                    cellTotAttendance.CellStyle = styleGrey40;
                                    cellTotAttendance.SetCellValue(Math.Round(TotalAttendance, MidpointRounding.AwayFromZero).ToString());
                                }
                                if (clientRequirement.CRI_Nightshift_Allowance == true)
                                {
                                    IsCRI_Nightshift_Allowance = true;
                                    totalCount = totalCount + 1;
                                    ICell cellTotNightshift = row.CreateCell(totalCount);
                                    cellTotNightshift.CellStyle = styleGrey40;
                                    cellTotNightshift.SetCellValue(Math.Round(TotalNightshift, MidpointRounding.AwayFromZero).ToString());
                                }
                                if (clientRequirement.CRI_Performance_Allowance == true)
                                {
                                    IsCRI_Performance_Allowance = true;
                                    totalCount = totalCount + 1;
                                    ICell cellTotPerformance = row.CreateCell(totalCount);
                                    cellTotPerformance.CellStyle = styleGrey40;
                                    cellTotPerformance.SetCellValue(Math.Round(TotalPerformance, MidpointRounding.AwayFromZero).ToString());
                                }
                                if (!clientRequirement.CRI_OT_Calculate_Payableday)
                                {
                                    totalCount = totalCount + 1;
                                    ICell cellTotOtHrs = row.CreateCell(totalCount);
                                    cellTotOtHrs.CellStyle = styleGrey40;
                                    cellTotOtHrs.SetCellValue(TotalOTHrs);

                                    IsCRI_OT_Calculate_Payableday = true;
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
                                    IsCRI_ProfessionalTax = true;
                                    totalCount1 = totalCount1 + 1;
                                    ICell cellTot9 = row.CreateCell(totalCount1);
                                    cellTot9.SetCellValue(Math.Round(TotalProfTax, MidpointRounding.AwayFromZero).ToString());
                                    cellTot9.CellStyle = styleGrey50;
                                }
                                if (clientRequirement.CRI_RevenueDeduction)
                                {
                                    IsCRI_RevenueDeduction = true;
                                    totalCount1 = totalCount1 + 1;
                                    ICell cellTot10 = row.CreateCell(totalCount1);
                                    cellTot10.SetCellValue(Math.Round(TotalRevenue, MidpointRounding.AwayFromZero).ToString());
                                    cellTot10.CellStyle = styleGrey50;
                                }
                                if (clientRequirement.CRI_CanteenFacility)
                                {
                                    IsCRI_CanteenFacility = true;
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
                                    IsDES_Include_LWF = true;
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

                                ICell cellTot15 = row.CreateCell(TotCount + 2);
                                cellTot15.SetCellValue("");
                                cellTot15.CellStyle = styleGrey80;
                                #endregion

                                DesCount = DesCount + 1;

                                Final_TotalPaybleDays = Final_TotalPaybleDays + TotalPaybleDays;
                                Final_TotalBasic = Final_TotalBasic + TotalBasic;
                                Final_TotalDA = Final_TotalDA + TotalDA;
                                Final_TotalHRA = Final_TotalHRA + TotalHRA;
                                Final_TotalOTHrs = Final_TotalOTHrs + TotalOTHrs;
                                Final_TotalOT = Final_TotalOT + TotalOT;
                                Final_TotalGrossTotal = Final_TotalGrossTotal + TotalGrossTotal;
                                Final_TotalPF = Final_TotalPF + TotalPF;
                                Final_TotalESIC = Final_TotalESIC + TotalESIC;
                                Final_TotalProfTax = Final_TotalProfTax + TotalProfTax;
                                Final_TotalRevenue = Final_TotalRevenue + TotalRevenue;
                                Final_TotalCanteenFacility = Final_TotalCanteenFacility + TotalCanteenFacility;
                                Final_TotalAdvance = Final_TotalAdvance + TotalAdvance;
                                Final_TotalDeduct = Final_TotalDeduct + TotalDeduct;
                                Final_TotalFinal = Final_TotalFinal + TotalFinal;
                                Final_TotalOutStation = Final_TotalOutStation + TotalOutStation;
                                Final_TotalAttendance = Final_TotalAttendance + TotalAttendance;
                                Final_TotalNightshift = Final_TotalNightshift + TotalNightshift;
                                Final_TotalPerformance = Final_TotalPerformance + TotalPerformance;
                                Final_TotalAllowance_1 = Final_TotalAllowance_1 + TotalAllowance1;
                                Final_TotalAllowance_2 = Final_TotalAllowance_2 + TotalAllowance2;
                                Final_TotalAllowance_3 = Final_TotalAllowance_3 + TotalAllowance3;
                                Final_TotalAllowance_4 = Final_TotalAllowance_4 + TotalAllowance4;
                                Final_TotalAllowance_5 = Final_TotalAllowance_5 + TotalAllowance5;
                                Final_TotalLWF = Final_TotalLWF + TotalLWF;
                            }

                            DesCount = DesCount + 1;
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

                            int fk = 0;
                            int fcellAllow = ftotalCount + 4;
                            foreach (var all in WageAllowancesTotalVMs.GroupBy(m => new { m.ALL_Id, m.Parameter }).Select(g => new { Total = g.Sum(m => m.Value), ALL_Title = g.Key.Parameter }))
                            {
                                ICell fcellEmpAll = frow.CreateCell(fcellAllow + 1);
                                fcellEmpAll.SetCellValue(all.ALL_Title);
                                fcellEmpAll.CellStyle = styleGrey40;
                                ICell fcellEmpAlls = frowSecond.CreateCell(fcellAllow + 1);
                                fcellEmpAlls.SetCellValue(Math.Round(all.Total, MidpointRounding.AwayFromZero).ToString());
                                fcellEmpAlls.CellStyle = styleGrey40;
                                fcellAllow++;
                                fk++;
                            }
                            ftotalCount = fcellAllow;

                            //custom
                            //foreach (string allownce in allCustomAallownces.Distinct().Where(m => m != null))
                            //{
                            //    ftotalCount = ftotalCount + 1;
                            //    ICell cellTot_Allowance_1 = frow.CreateCell(ftotalCount);
                            //    cellTot_Allowance_1.CellStyle = styleGrey40;
                            //    cellTot_Allowance_1.SetCellValue("");
                            //    ICell cellTotAllowance_1 = frowSecond.CreateCell(ftotalCount);
                            //    cellTotAllowance_1.CellStyle = styleGrey40;
                            //    cellTotAllowance_1.SetCellValue("");
                            //}
                            foreach (CustomAllowanceVM allownce in allowanceVMs)
                            {
                                ftotalCount = ftotalCount + 1;
                                ICell cellTot_Allowance_1 = frow.CreateCell(ftotalCount);
                                cellTot_Allowance_1.CellStyle = styleGrey40;
                                cellTot_Allowance_1.SetCellValue(allownce.C_ALL_Title);
                                ICell cellTotAllowance_1 = frowSecond.CreateCell(ftotalCount);
                                cellTotAllowance_1.CellStyle = styleGrey40;
                                cellTotAllowance_1.SetCellValue(allownce.C_ALL_Value.ToString());
                            }
                            //if (IsCRI_Allowance_1)
                            //{
                            //    ftotalCount = ftotalCount + 1;
                            //    ICell cellTot_Allowance_1 = frow.CreateCell(ftotalCount);
                            //    cellTot_Allowance_1.CellStyle = styleGrey40;
                            //    cellTot_Allowance_1.SetCellValue("Total Allowance 1");
                            //    ICell cellTotAllowance_1 = frowSecond.CreateCell(ftotalCount);
                            //    cellTotAllowance_1.CellStyle = styleGrey40;
                            //    cellTotAllowance_1.SetCellValue(Math.Round(Final_TotalAllowance_1, MidpointRounding.AwayFromZero).ToString());
                            //}
                            //if (IsCRI_Allowance_2)
                            //{
                            //    ftotalCount = ftotalCount + 1;
                            //    ICell cellTot_Allowance_2 = frow.CreateCell(ftotalCount);
                            //    cellTot_Allowance_2.CellStyle = styleGrey40;
                            //    cellTot_Allowance_2.SetCellValue("Total Allowance 2");
                            //    ICell cellTotAllowance_2 = frowSecond.CreateCell(ftotalCount);
                            //    cellTotAllowance_2.CellStyle = styleGrey40;
                            //    cellTotAllowance_2.SetCellValue(Math.Round(Final_TotalAllowance_2, MidpointRounding.AwayFromZero).ToString());
                            //}
                            //if (IsCRI_Allowance_3)
                            //{
                            //    ftotalCount = ftotalCount + 1;
                            //    ICell cellTot_Allowance_3 = frow.CreateCell(ftotalCount);
                            //    cellTot_Allowance_3.CellStyle = styleGrey40;
                            //    cellTot_Allowance_3.SetCellValue("Total Allowance 3");
                            //    ICell cellTotAllowance_3 = frowSecond.CreateCell(ftotalCount);
                            //    cellTotAllowance_3.CellStyle = styleGrey40;
                            //    cellTotAllowance_3.SetCellValue(Math.Round(Final_TotalAllowance_3, MidpointRounding.AwayFromZero).ToString());
                            //}
                            //if (IsCRI_Allowance_4)
                            //{
                            //    ftotalCount = ftotalCount + 1;
                            //    ICell cellTot_Allowance_4 = frow.CreateCell(ftotalCount);
                            //    cellTot_Allowance_4.CellStyle = styleGrey40;
                            //    cellTot_Allowance_4.SetCellValue("Total Allowance 4");
                            //    ICell cellTotAllowance_4 = frowSecond.CreateCell(ftotalCount);
                            //    cellTotAllowance_4.CellStyle = styleGrey40;
                            //    cellTotAllowance_4.SetCellValue(Math.Round(Final_TotalAllowance_4, MidpointRounding.AwayFromZero).ToString());
                            //}
                            //if (IsCRI_Allowance_5)
                            //{
                            //    ftotalCount = ftotalCount + 1;
                            //    ICell cellTot_Allowance_5 = frow.CreateCell(ftotalCount);
                            //    cellTot_Allowance_5.CellStyle = styleGrey40;
                            //    cellTot_Allowance_5.SetCellValue("Total Allowance 5");
                            //    ICell cellTotAllowance_5 = frowSecond.CreateCell(ftotalCount);
                            //    cellTotAllowance_5.CellStyle = styleGrey40;
                            //    cellTotAllowance_5.SetCellValue(Math.Round(Final_TotalAllowance_5, MidpointRounding.AwayFromZero).ToString());
                            //}
                            //end custom
                            #region New grand total allowances
                            if (IsCRI_OutStation_Allowance)
                            {
                                ftotalCount = ftotalCount + 1;
                                ICell cellTotOutstation = frow.CreateCell(ftotalCount);
                                cellTotOutstation.CellStyle = styleGrey40;
                                cellTotOutstation.SetCellValue("Total OutStation");
                                ICell cellTotOutstations = frowSecond.CreateCell(ftotalCount);
                                cellTotOutstations.CellStyle = styleGrey40;
                                cellTotOutstations.SetCellValue(Math.Round(Final_TotalOutStation, MidpointRounding.AwayFromZero).ToString());
                            }
                            if (IsCRI_Attendance_Allowance)
                            {
                                ftotalCount = ftotalCount + 1;
                                ICell cellTotAttendance = frow.CreateCell(ftotalCount);
                                cellTotAttendance.CellStyle = styleGrey40;
                                cellTotAttendance.SetCellValue("Total Attendance");
                                ICell cellTotAttendances = frowSecond.CreateCell(ftotalCount);
                                cellTotAttendances.CellStyle = styleGrey40;
                                cellTotAttendances.SetCellValue(Math.Round(Final_TotalAttendance, MidpointRounding.AwayFromZero).ToString());
                            }
                            if (IsCRI_Nightshift_Allowance)
                            {
                                ftotalCount = ftotalCount + 1;
                                ICell cellTotNightshift = frow.CreateCell(ftotalCount);
                                cellTotNightshift.CellStyle = styleGrey40;
                                cellTotNightshift.SetCellValue("Total Nightshift");
                                ICell cellTotNightshifts = frowSecond.CreateCell(ftotalCount);
                                cellTotNightshifts.CellStyle = styleGrey40;
                                cellTotNightshifts.SetCellValue(Math.Round(Final_TotalNightshift, MidpointRounding.AwayFromZero).ToString());
                            }
                            if (IsCRI_Performance_Allowance)
                            {
                                ftotalCount = ftotalCount + 1;
                                ICell cellTotPerformance = frow.CreateCell(ftotalCount);
                                cellTotPerformance.CellStyle = styleGrey40;
                                cellTotPerformance.SetCellValue("Total Performance");
                                ICell cellTotPerformances = frowSecond.CreateCell(ftotalCount);
                                cellTotPerformances.CellStyle = styleGrey40;
                                cellTotPerformances.SetCellValue(Math.Round(Final_TotalPerformance, MidpointRounding.AwayFromZero).ToString());
                            }
                            if (IsCRI_OT_Calculate_Payableday)
                            {
                                ftotalCount = ftotalCount + 1;
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
                            }
                            #endregion

                            ICell fcellTotGross = frow.CreateCell(ftotalCount + 1);
                            fcellTotGross.SetCellValue("Total Gross");
                            fcellTotGross.CellStyle = styleGrey40;
                            ICell fcellTotGrosss = frowSecond.CreateCell(ftotalCount + 1);
                            fcellTotGrosss.SetCellValue(Math.Round(Final_TotalGrossTotal, MidpointRounding.AwayFromZero).ToString());
                            fcellTotGrosss.CellStyle = styleGrey40;

                            ICell fcellTotPF = frow.CreateCell(ftotalCount + 2);
                            fcellTotPF.SetCellValue("Total PF");
                            fcellTotPF.CellStyle = styleGrey50;
                            ICell fcellTotPFs = frowSecond.CreateCell(ftotalCount + 2);
                            fcellTotPFs.SetCellValue(Math.Round(Final_TotalPF, MidpointRounding.AwayFromZero).ToString());
                            fcellTotPFs.CellStyle = styleGrey50;

                            ICell fcellTotEsic = frow.CreateCell(ftotalCount + 3);
                            fcellTotEsic.SetCellValue("Total ESIC");
                            fcellTotEsic.CellStyle = styleGrey50;
                            ICell fcellTotEsics = frowSecond.CreateCell(ftotalCount + 3);
                            fcellTotEsics.SetCellValue(Math.Round(Final_TotalESIC, MidpointRounding.AwayFromZero).ToString());
                            fcellTotEsics.CellStyle = styleGrey50;

                            int ftotalCount1 = ftotalCount + 3;
                            if (IsCRI_ProfessionalTax)
                            {
                                ftotalCount1 = ftotalCount1 + 1;
                                ICell cellTot9 = frow.CreateCell(ftotalCount1);
                                cellTot9.SetCellValue("Total Professional Tax");
                                cellTot9.CellStyle = styleGrey50;
                                ICell cellTot9s = frowSecond.CreateCell(ftotalCount1);
                                cellTot9s.SetCellValue(Math.Round(Final_TotalProfTax, MidpointRounding.AwayFromZero).ToString());
                                cellTot9s.CellStyle = styleGrey50;
                            }
                            if (IsCRI_RevenueDeduction)
                            {
                                ftotalCount1 = ftotalCount1 + 1;
                                ICell cellTot10 = frow.CreateCell(ftotalCount1);
                                cellTot10.SetCellValue("Total Revenue Deduction");
                                cellTot10.CellStyle = styleGrey50;
                                ICell cellTot10s = frowSecond.CreateCell(ftotalCount1);
                                cellTot10s.SetCellValue(Math.Round(Final_TotalRevenue, MidpointRounding.AwayFromZero).ToString());
                                cellTot10s.CellStyle = styleGrey50;
                            }
                            if (IsCRI_CanteenFacility)
                            {
                                ftotalCount1 = ftotalCount1 + 1;
                                ICell cellTot11 = frow.CreateCell(ftotalCount1);
                                cellTot11.SetCellValue("Total Canteen Facility");
                                cellTot11.CellStyle = styleGrey50;
                                ICell cellTot11s = frowSecond.CreateCell(ftotalCount1);
                                cellTot11s.SetCellValue(Math.Round(Final_TotalCanteenFacility, MidpointRounding.AwayFromZero).ToString());
                                cellTot11s.CellStyle = styleGrey50;
                            }

                            ICell cellTotAdvance = frow.CreateCell(ftotalCount1 + 1);
                            cellTotAdvance.SetCellValue("Total Advance Installment");
                            cellTotAdvance.CellStyle = styleGrey50;
                            ICell cellTotAdvances = frowSecond.CreateCell(ftotalCount1 + 1);
                            cellTotAdvances.SetCellValue(Math.Round(Final_TotalAdvance, MidpointRounding.AwayFromZero).ToString());
                            cellTotAdvances.CellStyle = styleGrey50;

                            int fTotCount = ftotalCount1 + 2;
                            if (IsDES_Include_LWF)
                            {
                                ICell cellTotLWF = frow.CreateCell(fTotCount);
                                cellTotLWF.SetCellValue("Total MLWF Deduction");
                                cellTotLWF.CellStyle = styleGrey50;
                                ICell cellTotLWFs = frowSecond.CreateCell(fTotCount);
                                cellTotLWFs.SetCellValue(Math.Round(Final_TotalLWF, MidpointRounding.AwayFromZero).ToString());
                                cellTotLWFs.CellStyle = styleGrey50;
                                fTotCount = fTotCount + 1;
                            }

                            ICell fcellTotDeduct = frow.CreateCell(fTotCount);
                            fcellTotDeduct.SetCellValue("Deduction");
                            fcellTotDeduct.CellStyle = styleGrey50;
                            ICell fcellTotDeducts = frowSecond.CreateCell(fTotCount);
                            fcellTotDeducts.SetCellValue(Math.Round(Final_TotalDeduct, MidpointRounding.AwayFromZero).ToString());
                            fcellTotDeducts.CellStyle = styleGrey50;

                            ICell fcellTotFinal = frow.CreateCell(fTotCount + 1);
                            fcellTotFinal.SetCellValue("Grand Total");
                            fcellTotFinal.CellStyle = styleGrey80;
                            ICell fcellTotFinals = frowSecond.CreateCell(fTotCount + 1);
                            fcellTotFinals.SetCellValue(Math.Round(Final_TotalFinal, MidpointRounding.AwayFromZero).ToString());
                            fcellTotFinals.CellStyle = styleGrey80;

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
        [HttpPost]
        public JsonResult Calculate_PF_ESIC([FromBody]CalculationEditVM CalculationEditVM)
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
                                CalculationEditVM.WAR_Allowance_Calculated_1, CalculationEditVM.WAR_Allowance_Calculated_2, CalculationEditVM.WAR_Allowance_Calculated_3, CalculationEditVM.WAR_Allowance_Calculated_4, CalculationEditVM.WAR_Allowance_Calculated_5), MidpointRounding.AwayFromZero);

                decimal ESICsum = Math.Round(GetAmountBasedOnFormula_Edit(
                               CalculationEditVM.CRI_ESIC_Formula, CalculationEditVM.WAR_Basic_Calculated, CalculationEditVM.CRI_DA_Calculated, CalculationEditVM.CRI_HRA_Calculated,
                               CalculationEditVM.CalculatedAllowanceVM, CalculationEditVM.totalWorkingDays, CalculationEditVM.totalPaybleDays, CalculationEditVM.WAR_OverTime_Calculated,
                               CalculationEditVM.WAR_Outstation_Allowance_Calculated, CalculationEditVM.WAR_Attendance_Allowance_Calculated, CalculationEditVM.WAR_Nightshift_Allowance_Calculated,
                               CalculationEditVM.WAR_Performance_Allowance_Calculated,
                               CalculationEditVM.WAR_Allowance_Calculated_1, CalculationEditVM.WAR_Allowance_Calculated_2, CalculationEditVM.WAR_Allowance_Calculated_3, CalculationEditVM.WAR_Allowance_Calculated_4, CalculationEditVM.WAR_Allowance_Calculated_5), MidpointRounding.AwayFromZero);

                calculated.WAR_PF_Calculated = Math.Round(Decimal.Multiply(PFsum, CalculationEditVM.CRI_PF_Percentage) / 100, MidpointRounding.AwayFromZero);
                calculated.WAR_ESIC_Calculated = Math.Ceiling(Decimal.Multiply(ESICsum, CalculationEditVM.CRI_ESIC_Percentage) / 100);

            }

            return Json(calculated);
        }

         [HttpPost]
        public JsonResult Calculate_OT1([FromBody]CalculationEditVM CalculationEditVM)
        {
            WageRegisterManager registerManager = new WageRegisterManager(_context);
            decimal Calculated_OT = 0M;
            if (CalculationEditVM != null)
            {
                Wage_Register wageRegister = registerManager.GetWageRegister(CalculationEditVM.WAR_Id);
                double OvertimeInDay = CalculationEditVM.ExtraWorkingHours / Convert.ToDouble(wageRegister.CLI_.CLI_WorkingHours_In_Day);
                if (OvertimeInDay > 0)
                {
                    if (!wageRegister.CRI_.CRI_OT_Calculate_Payableday)
                    {
                        if (wageRegister.CRI_.CRI_OT_Fixed_PerHour > 0)
                        {
                            Calculated_OT = Convert.ToDecimal(CalculationEditVM.ExtraWorkingHours) * wageRegister.CRI_.CRI_OT_Fixed_PerHour.Value;
                        }
                        else if (wageRegister.CRI_.CRI_OT_Formula != null)
                        {
                            CalculationEditVM.CRI_OT_Formula = wageRegister.WAR_OverTime_Formula;
                            decimal OTsum = Math.Round(GetAmountBasedOnFormula_Edit(
                                            CalculationEditVM.CRI_OT_Formula, CalculationEditVM.WAR_Basic_Calculated, CalculationEditVM.CRI_DA_Calculated, CalculationEditVM.CRI_HRA_Calculated,
                                            CalculationEditVM.CalculatedAllowanceVM, CalculationEditVM.totalWorkingDays, CalculationEditVM.totalPaybleDays,
                                            CalculationEditVM.WAR_OverTime_Calculated, CalculationEditVM.WAR_Outstation_Allowance_Calculated, CalculationEditVM.WAR_Attendance_Allowance_Calculated,
                                            CalculationEditVM.WAR_Nightshift_Allowance_Calculated, CalculationEditVM.WAR_Performance_Allowance_Calculated,
                                             CalculationEditVM.WAR_Allowance_Calculated_1, CalculationEditVM.WAR_Allowance_Calculated_2, CalculationEditVM.WAR_Allowance_Calculated_3, CalculationEditVM.WAR_Allowance_Calculated_4, CalculationEditVM.WAR_Allowance_Calculated_5), MidpointRounding.AwayFromZero);

                            Calculated_OT = Math.Round(Convert.ToDecimal(((Convert.ToDouble(OTsum) / CalculationEditVM.totalPaybleDays) * OvertimeInDay) * wageRegister.CRI_.CRI_OT_MultipleTimes), MidpointRounding.AwayFromZero);
                        }
                    }
                }
   
            }

            return Json(Calculated_OT);
        }

        public bool IsWageSaved(int EMP_id, int CLI_id, int WAG_Id)
        {
            return _context.Wage_Register.Where(m => m.WAG_Id.Equals(WAG_Id) && m.CLI_Id.Equals(CLI_id) && m.EMP_Id.Equals(EMP_id)).Count() > 0;
            
        }
    }
    
}