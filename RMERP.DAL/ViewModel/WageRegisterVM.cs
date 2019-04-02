using System;
using System.Collections.Generic;
using System.Text;
using RMERP.DAL.Models;

namespace RMERP.DAL.ViewModel
{
    public class WageRegisterVM
    {
        public List<Clients> listClients { get; set; }
        public List<Attendance> listAttendance { get; set; }
        public IEnumerable<Designations> listDesignations { get; set; }
        public IEnumerable<Employees> listEmployee { get; set; }
        public List<Client_Requirements> client_Requirements { get; set; }
        public DateTime WAG_Month { get; set; }
        public List<Allowances> allowances { get; set; }
        public int WAG_Id { get; set; }
        public List<Wage_Process_Clients> listWageProcessClients  { get; set; }
        public string WAG_Full_Month()
        {
            return WAG_Month.ToString("MMMM") + "-" + WAG_Month.ToString("yyyy");
        }

    }
    public class WageRegisterViewModel
    {
        public List<WageRegister> wageRegisters { get; set; }
        public Wage_Process_Clients wage_Process_Clients { get; set; }
        //public List<Clients_Employees> clientsEmployees { get; set; }
        public DateTime WAG_Month { get; set; }
        public int WAG_Id { get; set; }
        public int TotalDays { get; set; }
        public string WAG_Full_Month()
        {
            return WAG_Month.ToString("MMMM") + "-" + WAG_Month.ToString("yyyy");
        }

    }

    public class WageRegister
    {
        public int WAR_Id { get; set; }
        public int WAG_Id { get; set; }
        public int CLI_Id { get; set; }
        public int EMP_Id { get; set; }
        public int CRI_Id { get; set; }
        public double WAR_TotalPaybleDays { get; set; }
        public double WAR_TotalWorkingDays { get; set; }
        public double WAR_ExtraWorkingHours { get; set; }
        public decimal WAR_Basic { get; set; }
        public decimal WAR_DA { get; set; }
        public decimal WAR_HRA { get; set; }
        public decimal WAR_GrossTotal { get; set; }
        public DateTime WAR_LastModifiedOn { get; set; }
        public int ADM_LastModifiedBy { get; set; }

        public decimal OTAmt { get; set; }
        public decimal PFAmt { get; set; }
        public decimal EsicAmt { get; set; }
        public decimal finalAmt { get; set; }
        public decimal payDays { get; set; }

        public string DES_Title { get; set; }
        public int DES_Id { get; set; }

        public IEnumerable<int> designationID { get; set; }
        public List<Allowances> allowances { get; set; }
        //public Clients_Employees Clients_Employee { get; set; }
        public List<Clients_Employees> clientsEmployees { get; set; }


        public Clients CLI_ { get; set; }
        public Client_Requirements CRI_ { get; set; }
        public Employees EMP_ { get; set; }
        public Wage_Process WAG_ { get; set; }

    }
    #region
    public class _WageRegisterViewModel
    {
        public List<_ClientsVM> ClientList { get; set; }
        public List<Clients> Clients { get; set; }
        public DateTime WAG_Month { get; set; }
        public int WAG_Id { get; set; }
        public int TotalDays { get; set; }
        public string WAG_Full_Month()
        {
            return WAG_Month.ToString("MMMM") + "-" + WAG_Month.ToString("yyyy");
        }

    }
    public class _ClientsVM
    {
        public int CLI_Id { get; set; }
        public string CLI_Name { get; set; }
        public List<_DesignationsVM> DesignationsList { get; set; }
        public Wage_Process_Clients wage_Process_Client { get; set; }
    }

    public class _DesignationsVM
    {
        public int DES_Id { get; set; }
        public string DES_Title { get; set; }
        public List<_EmployeeVM> EmployeeList { get; set; }
        public List<Clients_Employees> clients_Employees { get; set; }
    }

    public class _EmployeeVM
    {
        public Employees employee { get; set; }
        public Client_Requirements client_Requirements { get; set; }
        public List<Attendance> attendances { get; set; }
        public List<Allowances> allowancess { get; set; }
        public int  TotalDays { get; set; }
        public int WorkingDays { get; set; }
        public double ExtraWorkingHrs { get; set; }
        public decimal OTAmt { get; set; }
        public decimal PFAmt { get; set; }
        public decimal EsicAmt { get; set; }
        public decimal Basic { get; set; }
        public decimal DA { get; set; }
        public decimal HRA { get; set; }
        public decimal GrossTotal { get; set; }
        public decimal finalAmt { get; set; }
        
    }
    #endregion


}
