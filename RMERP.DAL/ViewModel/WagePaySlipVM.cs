using System;
using System.Collections.Generic;
using System.Text;
using RMERP.DAL.Models;
using static RMERP.DAL.Helpers.ProjectUtils;

namespace RMERP.DAL.ViewModel
{
    public class WPS_ClientsVM
    {
        public int FRM_Id { get; set; }
        public string FRM_Title { get; set; }
        public int WAG_Id { get; set; }
        public DateOnly WAG_Month { get; set; }
        public List<WPS_ClientsStatusVM> WageClients { get; set; } = [];
    }

    public class WPS_ClientsStatusVM
    {
        public int CLI_Id { get; set; }
        public string CLI_Name { get; set; }
        public bool Is_WageRegisterSaved { get; set; }
        public int TotalEmployees { get; set; }
        public int TotalGenerated { get; set; }
    }

    public class WPS_MasterVM
    {
        public int FRM_Id { get; set; }
        public string FRM_Name { get; set; }
        public int WAG_Id { get; set; }
        public string WAG_Month { get; set; }
        public int CLI_Id { get; set; }
        public string CLI_Name { get; set; }
        public int AllSalaryslipsGenerated { get; set; } = (int)SalaryslipsGenerated.NotGenerated;
        public List<EMP_PaySlipVM> EmployeesPaySlips { get; set; }
    }

    public class EMP_PaySlipVM
    {
        public int WPS_Id { get; set; }
        public int EMP_Id { get; set; }
        public string EMP_FirstName { get; set; }
        public string EMP_MiddleName { get; set; }
        public string EMP_SurName { get; set; }
        public string EMP_FullName
        {
            get
            {
                return string.Concat(EMP_FirstName + " " + EMP_MiddleName + " " + EMP_SurName);
            }
        }
        public bool IsPaySlipGenerated { get; set; }
        public DateTime? WPS_GeneratedOn { get; set; }
    }


    public class WagePaySlipMasterVM
    {
        public int WAG_Id { get; set; }
        public List<ClientWiseEmp> ClientWiseEmps { get; set; }
        public string WAG_Month { get; set; }
        public int FRM_Id { get; set; }
        public string FRM_Name { get; set; }
    }

    public class ClientWiseEmp
    {
        public int CLI_Id { get; set; }
        public string CLI_Name { get; set; }
        public int AllSalaryslipsGenerated { get; set; } = (int)SalaryslipsGenerated.NotGenerated;
        public List<EmpPaySlipVM> EmpPaySlipVMs { get; set; }
    }
    public class EmpPaySlipVM
    {
        public int CLI_Id { get; set; }
        public int WPS_Id { get; set; }
        public int EMP_Id { get; set; }
        public string EMP_FirstName { get; set; }
        public string EMP_MiddleName { get; set; }
        public string EMP_SurName { get; set; }
        public string EMP_FullName
        {
            get
            {
                return string.Concat(EMP_FirstName + " " + EMP_MiddleName + " " + EMP_SurName);
            }
        }
        public bool IsPaySlipGenerated { get; set; }
        public DateTime? WPS_GeneratedOn { get; set; }

    }
}
