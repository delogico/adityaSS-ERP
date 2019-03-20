using System;
using System.Collections.Generic;
using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace RMERP.DAL.Mappers
{
    public class ClientEmployeeMapper
    {
        public static ClientEmployeeVM mapMe(Clients_Employees employee)
        {
            ClientEmployeeVM clientEmployeeVM = new ClientEmployeeVM();
            clientEmployeeVM.CLE_Id = employee.CLE_Id;
            clientEmployeeVM.CLE_RegisteredOn = employee.CLE_RegisteredOn;
            clientEmployeeVM.CLI_Id = employee.CLI_Id;
            clientEmployeeVM.EMP_Id = employee.EMP_Id;
            clientEmployeeVM.DES_Id = employee.DES_Id;
            clientEmployeeVM.ADM_Id_RegisteredBy = employee.ADM_Id_RegisteredBy;
            if(employee.DES_ != null)
                clientEmployeeVM.DES_Title = employee.DES_.DES_Title;
            if(employee.EMP_ != null)
                clientEmployeeVM.employee = EmployeesMapper.MapMe(employee.EMP_);
            return clientEmployeeVM;
        }

        public static List<ClientEmployeeVM> mapEmployees(List<Clients_Employees> employees)
        {
            List<ClientEmployeeVM> lst = new List<ClientEmployeeVM>();
            foreach (Clients_Employees employee in employees)
                lst.Add(mapMe(employee));
            return lst;
        }
    }
}
