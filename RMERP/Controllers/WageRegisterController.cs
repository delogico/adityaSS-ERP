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
        [Breadcrumb("Wage Register", FromAction = "Index", FromController = typeof(WageProcessController))]
        public ActionResult WageRegister(int WAG_Id)
        {
            WAG_Id = (WAG_Id <= 0 ? WagId : WAG_Id);
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            WageRegisterManager wageRegisterManager = new WageRegisterManager(_context);
            if (WAG_Id > 0)
            {
                List<ClientWageRegisterVM> lst = wageRegisterManager.GenerateWageRegisterTable(WAG_Id, sessionUtils.GetLoggedAdminID());
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
            if (WAG_Id > 0)
            {
                WageProcessManager wageManager = new WageProcessManager(_context);
                Wage_Process wageProcess = wageManager.getWageProcessById(WAG_Id);
                List<Wage_Register> wage_Registers = WageRegisterMapper.mapWageRegisters(wageRegisterManager.GetWageRegisterCalculated(wageProcess, Convert.ToInt32(item_CLI_Id), sessionUtils.GetLoggedAdminID()));
                wageRegisterManager.SaveWageRegister(wage_Registers, WAG_Id, item_CLI_Id, sessionUtils.GetLoggedAdminID());
            }
            return RedirectToAction("WageRegister", new { WAG_Id = WAG_Id });
        }
        [HttpPost]
        public ActionResult ResetWageRegister(int WAG_Id, string item_CLI_Id)
        {
            WageRegisterManager wageRegisterManager = new WageRegisterManager(_context);
            string res = wageRegisterManager.ResetWageRegister(WAG_Id, item_CLI_Id);
            if (res != string.Empty)
            {
                TempData["message"] = "Wage Register is not saved!";
            }
            return RedirectToAction("WageRegister", new { WAG_Id = WAG_Id });
        }
        [Breadcrumb("Edit WageRegister", FromAction = "WageRegister")]
        public ActionResult EditWageRegister(int WAR_Id = -1)
        {
            EditWageRegisterVM editWageRegisterVM = new EditWageRegisterVM();
            WageRegisterManager wageRegisterManager = new WageRegisterManager(_context);
            editWageRegisterVM.wageRegisterVM = WageRegisterMapper.mapMe(wageRegisterManager.GetWage_RegisterByID(WAR_Id));
            WagId = editWageRegisterVM.wageRegisterVM.WAG_Id;

            WageProcessManager wageProcessManager = new WageProcessManager(_context);
            DateTime WAG_Month = wageProcessManager.getWageProcessById(WagId).WAG_Month;
            ViewBag.Wag_Month = WAG_Month.ToString("MMMM") + "-" + WAG_Month.ToString("yyyy");

            editWageRegisterVM.wage_Register_Allowances = wageRegisterManager.GetWage_Register_Allowances(WAR_Id);
            return View(editWageRegisterVM);
        }
        [HttpPost]
        public ActionResult EditWageRegister(WageRegisterVM wageRegisterVM)
        {
            WageRegisterManager wageRegisterManager = new WageRegisterManager(_context);
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            wageRegisterVM.ADM_LastModifiedBy = sessionUtils.GetLoggedAdminID();
            string res = wageRegisterManager.UpdateWageRegister(WageRegisterMapper.mapMe(wageRegisterVM));
            if (res != string.Empty)
            {
                TempData["message"] = "Wage Register is not updated!";
            }
            return RedirectToAction("WageRegister", new { WAG_Id = wageRegisterVM.WAG_Id });
        }


        public async Task<FileResult> WageRegisterExcel(int WAG_Id)
        {
            SessionUtils sessionUtils = new SessionUtils(Request, Response);
            WageRegisterManager wageRegisterManager = new WageRegisterManager(_context);
            List<ClientWageRegisterVM> List = new List<ClientWageRegisterVM>();
            if (WAG_Id > 0)
            {
                List = wageRegisterManager.GenerateWageRegisterTable(WAG_Id, sessionUtils.GetLoggedAdminID());
            }

            ReportsManager manager = new ReportsManager(_context);
            WageProcessManager wageProcess = new WageProcessManager(_context);
            DateTime wageMonth = wageProcess.getWageProcessById(WAG_Id).WAG_Month;
            string WAG_Month = wageMonth.ToString("MMMM") + "-" + wageMonth.ToString("yyyy");

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
                            var distinceDesignations = item.wageRegisterVMs.Select(q => new { q.designation.DES_Id, q.designation.DES_Title }).Distinct();                           
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

                            IRow rowAdd1 = excelSheet.CreateRow(DesCount+1);
                            ICell CellAdd1 = rowAdd1.CreateCell(0);
                            CellAdd1.SetCellValue("G-9, MALTI TOWER, TARABAI PARK, KOLHAPUR");
                            CellUtil.SetAlignment(CellAdd1, workbook, (short)HorizontalAlignment.Center);
                            excelSheet.AddMergedRegion(new CellRangeAddress(DesCount+1, DesCount+1, 0, 25));

                            IRow rowSubHeading = excelSheet.CreateRow(DesCount+2);
                            ICell CellSubHeading = rowSubHeading.CreateCell(0);
                            CellSubHeading.SetCellValue("PAYSHEET FOR THE MONTH OF "+ WAG_Month);
                            CellSubHeading.CellStyle = styleDesignation;
                            CellUtil.SetAlignment(CellSubHeading, workbook, (short)HorizontalAlignment.Center);
                            excelSheet.AddMergedRegion(new CellRangeAddress(DesCount+2, DesCount+2, 0, 25));

                            IRow rowClient = excelSheet.CreateRow(DesCount+3);
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
                                excelSheet.AddMergedRegion(new CellRangeAddress(DesCount, DesCount,0,25));                                
                                CellHeader.CellStyle = styleTotal;

                                row = excelSheet.CreateRow(DesCount+1);
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
                                    ICell cellAll = row.CreateCell(cell+1);
                                    cellAll.SetCellValue(all.allowanceVM.ALL_Alias);
                                    excelSheet.SetColumnWidth(cell + 1, (int)((25 + 0.72) * 140));
                                    cellAll.CellStyle = styleGrey40;
                                    cell++;i++;
                                }
                                ICell cell10 = row.CreateCell(cell + 1);
                                cell10.SetCellValue("OT Amount");
                                cell10.CellStyle = styleGrey40;
                                ICell cell11 = row.CreateCell(cell + 2);
                                cell11.SetCellValue("Gross Total");
                                cell11.CellStyle = styleGrey40;
                                ICell cell12 = row.CreateCell(cell + 3);
                                cell12.SetCellValue("PF");
                                cell12.CellStyle = styleGrey50;
                                ICell cell13 = row.CreateCell(cell + 4);
                                cell13.SetCellValue("ESIC");
                                cell13.CellStyle = styleGrey50;

                                int cellNext = cell + 4;
                                if (clientRequirement.CRI_ProfessionalTax == true)
                                {
                                    cellNext= cellNext+1;
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
                                ICell cell15 = row.CreateCell(cellNext1+1);
                                cell15.SetCellValue("Deduct Total");
                                cell15.CellStyle = styleGrey50;
                                ICell cell16 = row.CreateCell(cellNext1+2);
                                cell16.SetCellValue("Final Amount");                                
                                cell16.CellStyle = styleGrey80;
                                #endregion

                                DesCount = DesCount + 2;
                                double TotalPaybleDays = 0;
                                decimal TotalBasic = 0M, TotalDA = 0M, TotalHRA = 0M,TotalOT=0M,TotalGrossTotal=0M,TotalPF=0M,TotalESIC=0M,TotalProfTax=0m,TotalRevenue=0M,TotalCanteenFacility=0M,TotalAdvance=0M,TotalDeduct=0M,TotalFinal=0M;
                                
                                  List<AllowanceData> ListAllowances = new List<AllowanceData>();
                               
                                foreach (var employee in wageRegisters)
                                {                                   
                                    #region Employee Data
                                    row = excelSheet.CreateRow(DesCount);
                                    ICell cellEmp0 = row.CreateCell(0);
                                    cellEmp0.SetCellValue(employee.employeeVM.EMP_Id.ToString("D5"));
                                    cellEmp0.CellStyle = styleGrey25;
                                    ICell cellEmp1 = row.CreateCell(1);
                                    cellEmp1.SetCellValue(employee.employeeVM.EMP_FirstName+" "+ employee.employeeVM.EMP_MiddleName +" "+employee.employeeVM.EMP_SurName);
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
                                    cellEmp6.SetCellValue(Math.Round(employee.WAR_Basic_Calculated).ToString());
                                    cellEmp6.CellStyle = styleGrey40;
                                    TotalBasic = TotalBasic + Math.Round(employee.WAR_Basic_Calculated);
                                    ICell cellEmp7 = row.CreateCell(7);
                                    cellEmp7.SetCellValue(Math.Round(employee.WAR_DA_Calculated).ToString());
                                    cellEmp7.CellStyle = styleGrey40;
                                    TotalDA = TotalDA + Math.Round(employee.WAR_DA_Calculated);
                                    ICell cellEmp8 = row.CreateCell(8);
                                    cellEmp8.SetCellValue(Math.Round(employee.WAR_HRA_Calculated).ToString());
                                    TotalHRA = TotalHRA + Math.Round(employee.WAR_HRA_Calculated);
                                    cellEmp8.CellStyle = styleGrey40;
                                    int cellAllowance = 8;
                                    int j = 0;
                                    foreach (var all in employee.allowanceVMs)
                                    {
                                        arrAllowancesTotal[j] = arrAllowancesTotal[j] + all.WAA_Amount_Calculated;
                                        AllowanceData allowanceData = new AllowanceData();
                                        allowanceData.AllowanceId = all.allowanceVM.ALL_Id;
                                        allowanceData.AllowanceName = all.allowanceVM.ALL_Alias;
                                        allowanceData.TotalAllowance = all.WAA_Amount_Calculated;
                                        ListAllowances.Add(allowanceData);
                                        ICell cellEmpAll = row.CreateCell(cellAllowance+1);
                                        cellEmpAll.SetCellValue(Math.Round(all.WAA_Amount_Calculated).ToString());
                                        cellEmpAll.CellStyle = styleGrey40;
                                        cellAllowance++;
                                        j++;
                                    }
                                    int cellNxt = cellAllowance+1;
                                    ICell cellEmp10 = row.CreateCell(cellNxt);
                                    cellEmp10.SetCellValue(Math.Round(employee.WAR_OverTime_Calculated).ToString());
                                    TotalOT = TotalOT + Math.Round(employee.WAR_OverTime_Calculated);
                                    cellEmp10.CellStyle = styleGrey40;
                                    ICell cellEmp11 = row.CreateCell(cellNxt + 1);
                                    cellEmp11.SetCellValue(Math.Round(employee.WAR_GrossTotal).ToString());
                                    TotalGrossTotal = TotalGrossTotal + Math.Round(employee.WAR_GrossTotal);
                                    cellEmp11.CellStyle = styleGrey40;
                                    ICell cellEmp12 = row.CreateCell(cellNxt + 2);
                                    cellEmp12.SetCellValue(Math.Round(employee.WAR_PF_Calculated).ToString());
                                    TotalPF = TotalPF + Math.Round(employee.WAR_PF_Calculated);
                                    cellEmp12.CellStyle = styleGrey50;
                                    ICell cellEmp13 = row.CreateCell(cellNxt + 3);
                                    cellEmp13.SetCellValue(Math.Round(employee.WAR_ESIC_Calculated).ToString());
                                    TotalESIC = TotalESIC + Math.Round(employee.WAR_ESIC_Calculated);
                                    cellEmp13.CellStyle = styleGrey50;
                                    int cellNxt1 = cellNxt + 3;
                                    if (clientRequirement.CRI_ProfessionalTax == true)
                                    {
                                        cellNxt1 = cellNxt1 + 1;
                                        ICell cellEmpTax = row.CreateCell(cellNxt1);
                                        cellEmpTax.SetCellValue(employee.WAR_ProffesionalTax_Calculated.ToString());
                                        TotalProfTax = TotalProfTax + Convert.ToDecimal(employee.WAR_ProffesionalTax_Calculated);
                                        cellEmpTax.CellStyle = styleGrey50;
                                    }
                                    if (clientRequirement.CRI_RevenueDeduction == true)
                                    {
                                        cellNxt1 = cellNxt1 + 1;
                                        ICell cellRevenue = row.CreateCell(cellNxt1);
                                        cellRevenue.SetCellValue(employee.WAR_RevenueDeduction_Calculated);
                                        if (employee.WAR_RevenueDeduction_Calculated!="-")
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
                                    int cellNext2 = cellNxt1 + 1;

                                    #region Total Deduction
                                    decimal DeductTotal = @Math.Round(employee.WAR_PF_Calculated) + @Math.Round(employee.WAR_ESIC_Calculated) + @Math.Round(Convert.ToDecimal(employee.WAR_ProffesionalTax_Calculated))
                                        + @Math.Round(employee.WAR_Advance_Amount);
                                    if (employee.WAR_RevenueDeduction_Calculated != "-")
                                    {
                                        DeductTotal += @Math.Round(Convert.ToDecimal(employee.WAR_RevenueDeduction_Calculated));
                                    }
                                    if (employee.WAR_CanteenFacility_Calculation != "-")
                                    {
                                        DeductTotal += @Math.Round(Convert.ToDecimal(employee.WAR_CanteenFacility_Calculation));
                                    }
                                    #endregion

                                    ICell cellEmp14 = row.CreateCell(cellNext2);
                                    cellEmp14.SetCellValue(Math.Round(employee.WAR_Advance_Amount).ToString());
                                    TotalAdvance = TotalAdvance + Math.Round(employee.WAR_Advance_Amount);
                                    cellEmp14.CellStyle = styleGrey50;
                                    ICell cellEmp15 = row.CreateCell(cellNext2 + 1);
                                    cellEmp15.SetCellValue(DeductTotal.ToString());
                                    cellEmp15.CellStyle = styleGrey50;
                                    TotalDeduct = TotalDeduct + DeductTotal;
                                    ICell cellEmp16 = row.CreateCell(cellNext2 + 2);
                                    cellEmp16.SetCellValue(Math.Round(employee.WAR_FinalTotal).ToString());
                                    TotalFinal = TotalFinal + Math.Round(employee.WAR_FinalTotal);
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
                                ICell cellTot1 = row.CreateCell(totalCount+1);
                                cellTot1.SetCellValue(TotalPaybleDays.ToString());
                                cellTot1.CellStyle = styleGrey25;
                                ICell cellTot2 = row.CreateCell(totalCount+2);
                                cellTot2.SetCellValue(Math.Round(TotalBasic).ToString());
                                cellTot2.CellStyle = styleGrey40;
                                ICell cellTot3 = row.CreateCell(totalCount+3);
                                cellTot3.SetCellValue(Math.Round(TotalDA).ToString());
                                cellTot3.CellStyle = styleGrey40;
                                ICell cellTot4 = row.CreateCell(totalCount+4);
                                cellTot4.SetCellValue(Math.Round(TotalHRA).ToString());
                                cellTot4.CellStyle = styleGrey40;

                                // decimal TotalAllowance = ListAllowances.Sum();                               
                                int k = 0;
                                int cellAllow = totalCount + 4;
                                foreach (var all in wageRegisters[0].allowanceVMs)
                                {                                   
                                    ICell cellEmpAll = row.CreateCell(cellAllow + 1);
                                    cellEmpAll.SetCellValue(Math.Round(arrAllowancesTotal[k]).ToString());
                                    cellEmpAll.CellStyle = styleGrey40;
                                    cellAllow++;
                                    k++;
                                }

                                totalCount = cellAllow;
                                ICell cellTot5 = row.CreateCell(totalCount + 1);
                                cellTot5.CellStyle = styleGrey40;
                                cellTot5.SetCellValue(Math.Round(TotalOT).ToString());
                                ICell cellTot6 = row.CreateCell(totalCount + 2);
                                cellTot6.SetCellValue(Math.Round(TotalGrossTotal).ToString());
                                cellTot6.CellStyle = styleGrey40;
                                ICell cellTot7 = row.CreateCell(totalCount + 3);
                                cellTot7.SetCellValue(Math.Round(TotalPF).ToString());
                                cellTot7.CellStyle = styleGrey50;
                                ICell cellTot8 = row.CreateCell(totalCount + 4);
                                cellTot8.SetCellValue(Math.Round(TotalESIC).ToString());
                                cellTot8.CellStyle = styleGrey50;

                                int totalCount1 = totalCount+4;
                                if (clientRequirement.CRI_ProfessionalTax == true)
                                {
                                    totalCount1 = totalCount1 + 1;
                                    ICell cellTot9 = row.CreateCell(totalCount1);
                                    cellTot9.SetCellValue(Math.Round(TotalProfTax).ToString());
                                    cellTot9.CellStyle = styleGrey50;
                                }
                                if (clientRequirement.CRI_RevenueDeduction == true)
                                {
                                    totalCount1 = totalCount1 + 1;
                                    ICell cellTot10 = row.CreateCell(totalCount1);
                                    cellTot10.SetCellValue(Math.Round(TotalRevenue).ToString());
                                    cellTot10.CellStyle = styleGrey50;
                                }
                                if (clientRequirement.CRI_CanteenFacility == true)
                                {
                                    totalCount1 = totalCount1 + 1;
                                    ICell cellTot11 = row.CreateCell(totalCount1);
                                    cellTot11.SetCellValue(Math.Round(TotalCanteenFacility).ToString());
                                    cellTot11.CellStyle = styleGrey50;
                                }
                                    
                                ICell cellTot12 = row.CreateCell(totalCount1+1);
                                cellTot12.SetCellValue(Math.Round(TotalAdvance).ToString());
                                cellTot12.CellStyle = styleGrey50;
                                ICell cellTot13 = row.CreateCell(totalCount1+2);
                                cellTot13.SetCellValue(Math.Round(TotalDeduct).ToString());
                                cellTot13.CellStyle = styleGrey50;
                                ICell cellTot14 = row.CreateCell(totalCount1+3);
                                cellTot14.SetCellValue(Math.Round(TotalFinal).ToString());
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
        private class AllowanceData
        {
            public int AllowanceId { get; set; }
            public string AllowanceName { get; set; }
            public decimal TotalAllowance { get; set; }
        }
    }
}