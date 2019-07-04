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
                            int DesCount = 0;
                            foreach (var dd in distinceDesignations)
                            {
                                List<WageRegisterVM> wageRegisters = item.wageRegisterVMs.Where(r => r.designation.DES_Id.Equals(dd.DES_Id)).ToList();
                                ClientRequirementVM clientRequirement = wageRegisters.Select(m => m.clientRequirementVM).Where(m => m.CLI_Id.Equals(item.client.CLI_Id) && m.DES_Id.Equals(dd.DES_Id)).First();

                                IRow row = excelSheet.CreateRow(DesCount);
                                ICell CellHeader = row.CreateCell(0);
                                CellHeader.SetCellValue(dd.DES_Title);
                                CellUtil.SetAlignment(CellHeader, workbook, (short)HorizontalAlignment.Center);

                                row = excelSheet.CreateRow(DesCount+1);
                                row.HeightInPoints = (float)(3.2 * excelSheet.DefaultRowHeightInPoints);
                                ICell cell0 = row.CreateCell(0);
                                cell0.SetCellValue("ID");
                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("Name");
                                ICell cell2 = row.CreateCell(2);
                                cell2.SetCellValue("M/F");
                                ICell cell3 = row.CreateCell(3);
                                cell3.SetCellValue("Date Of Joining");
                                ICell cell4 = row.CreateCell(4);
                                cell4.SetCellValue("Total Working Days");
                                ICell cell5 = row.CreateCell(5);
                                cell5.SetCellValue("Total Payble Days");
                                ICell cell6 = row.CreateCell(6);
                                cell6.SetCellValue("Basic");
                                ICell cell7 = row.CreateCell(7);
                                cell7.SetCellValue("DA");
                                ICell cell8 = row.CreateCell(8);
                                cell8.SetCellValue("HRA");
                                int cell = 8;                               
                                foreach (var all in wageRegisters[0].allowanceVMs)
                                {
                                    ICell cellAll = row.CreateCell(cell+1);
                                    cellAll.SetCellValue(all.allowanceVM.ALL_Alias);
                                    cell++;
                                }
                                ICell cell10 = row.CreateCell(cell + 1);
                                cell10.SetCellValue("OT Amount");
                                ICell cell11 = row.CreateCell(cell + 2);
                                cell11.SetCellValue("Gross Total");
                                ICell cell12 = row.CreateCell(cell + 3);
                                cell12.SetCellValue("PF");
                                ICell cell13 = row.CreateCell(cell + 4);
                                cell13.SetCellValue("ESIC");

                                int cellNext = cell + 4;
                                if (clientRequirement.CRI_ProfessionalTax == true)
                                {
                                    cellNext= cellNext+1;
                                    ICell cellTax = row.CreateCell(cellNext);
                                    cellTax.SetCellValue("Proffesional Tax");
                                }
                                if (clientRequirement.CRI_RevenueDeduction == true)
                                {
                                    cellNext = cellNext + 1;
                                    ICell cellRevenue = row.CreateCell(cellNext);
                                    cellRevenue.SetCellValue("Revenue Deduction");
                                }
                                if (clientRequirement.CRI_CanteenFacility == true)
                                {
                                    cellNext = cellNext + 1;
                                    ICell cellCanteen = row.CreateCell(cellNext);
                                    cellCanteen.SetCellValue("Canteen Facility");
                                }
                                int cellNext1 = cellNext + 1;
                                ICell cell14 = row.CreateCell(cellNext1);
                                cell14.SetCellValue("Advance Installment");
                                ICell cell15 = row.CreateCell(cellNext1+1);
                                cell15.SetCellValue("Deduct Total");
                                ICell cell16 = row.CreateCell(cellNext1+2);
                                cell16.SetCellValue("Final Amount");
                                DesCount= DesCount+2;                                
                                foreach (var employee in wageRegisters)
                                {
                                    row = excelSheet.CreateRow(DesCount);
                                    ICell cellEmp0 = row.CreateCell(0);
                                    cellEmp0.SetCellValue(employee.employeeVM.EMP_Id.ToString("D5"));
                                    ICell cellEmp1 = row.CreateCell(1);
                                    cellEmp1.SetCellValue(employee.employeeVM.EMP_FirstName+" "+ employee.employeeVM.EMP_MiddleName +" "+employee.employeeVM.EMP_SurName);
                                    ICell cellEmp2 = row.CreateCell(2);
                                    cellEmp2.SetCellValue(employee.employeeVM.EMP_Gender == true ? "M" : "F");
                                    ICell cellEmp3 = row.CreateCell(3);
                                    cellEmp3.SetCellValue(DateHelper.getDateWithFormat(employee.employeeVM.EMP_DateOfJoining));
                                    ICell cellEmp4 = row.CreateCell(4);
                                    cellEmp4.SetCellValue(employee.WAR_TotalWorkingDays);
                                    ICell cellEmp5 = row.CreateCell(5);
                                    cellEmp5.SetCellValue(employee.WAR_TotalPaybleDays);
                                    ICell cellEmp6 = row.CreateCell(6);
                                    cellEmp6.SetCellValue(employee.WAR_Basic_Calculated.ToString());
                                    ICell cellEmp7 = row.CreateCell(7);
                                    cellEmp7.SetCellValue(employee.WAR_DA_Calculated.ToString());
                                    ICell cellEmp8 = row.CreateCell(8);
                                    cellEmp8.SetCellValue(employee.WAR_HRA_Calculated.ToString());
                                    int cellAllowance = 8;
                                    foreach (var all in employee.allowanceVMs)
                                    {
                                        ICell cellEmpAll = row.CreateCell(cellAllowance+1);
                                        cellEmpAll.SetCellValue(all.WAA_Amount_Calculated.ToString());
                                        cellAllowance++;
                                    }
                                    int cellNxt = cellAllowance+1;
                                    ICell cellEmp10 = row.CreateCell(cellNxt);
                                    cellEmp10.SetCellValue(employee.WAR_OverTime_Calculated.ToString());
                                    ICell cellEmp11 = row.CreateCell(cellNxt + 1);
                                    cellEmp11.SetCellValue(employee.WAR_GrossTotal.ToString());
                                    ICell cellEmp12 = row.CreateCell(cellNxt + 2);
                                    cellEmp12.SetCellValue(employee.WAR_PF_Calculated.ToString());
                                    ICell cellEmp13 = row.CreateCell(cellNxt + 3);
                                    cellEmp13.SetCellValue(employee.WAR_ESIC_Calculated.ToString());

                                    int cellNxt1 = cellNxt + 3;
                                    if (clientRequirement.CRI_ProfessionalTax == true)
                                    {
                                        cellNxt1 = cellNxt1 + 1;
                                        ICell cellEmpTax = row.CreateCell(cellNxt1);
                                        cellEmpTax.SetCellValue(employee.WAR_ProffesionalTax_Calculated.ToString());
                                    }
                                    if (clientRequirement.CRI_RevenueDeduction == true)
                                    {
                                        cellNxt1 = cellNxt1 + 1;
                                        ICell cellRevenue = row.CreateCell(cellNxt1);
                                        cellRevenue.SetCellValue(employee.WAR_RevenueDeduction_Calculated);
                                    }
                                    if (clientRequirement.CRI_CanteenFacility == true)
                                    {
                                        cellNxt1 = cellNxt1 + 1;
                                        ICell cellCanteen = row.CreateCell(cellNxt1);
                                        cellCanteen.SetCellValue(employee.WAR_CanteenFacility_Calculation);
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
                                    cellEmp14.SetCellValue(employee.WAR_Advance_Amount.ToString());
                                    ICell cellEmp15 = row.CreateCell(cellNext2 + 1);
                                    cellEmp15.SetCellValue(DeductTotal.ToString());
                                    ICell cellEmp16 = row.CreateCell(cellNext2 + 2);
                                    cellEmp16.SetCellValue(employee.WAR_FinalTotal.ToString());

                                    DesCount++;
                                }
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