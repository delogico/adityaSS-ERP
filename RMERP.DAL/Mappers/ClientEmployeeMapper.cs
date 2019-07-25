using System;
using System.Collections.Generic;
using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;
using RMERP.DAL.ManagerClasses;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace RMERP.DAL.Mappers
{
    public class ClientEmployeeMapper
    {        
        public static ClientEmployeeVM mapMe(Clients_Employees employee, RMERPContext _context = null)
        {
            ClientsManager clientsManager = new ClientsManager(_context);

            ClientEmployeeVM clientEmployeeVM = new ClientEmployeeVM();
            clientEmployeeVM.CLE_Id = employee.CLE_Id;
            clientEmployeeVM.CLE_RegisteredOn = employee.CLE_RegisteredOn;
            clientEmployeeVM.CLI_Id = employee.CLI_Id;
            clientEmployeeVM.EMP_Id = employee.EMP_Id;
            clientEmployeeVM.DES_Id = employee.DES_Id;
            clientEmployeeVM.ADM_Id_RegisteredBy = employee.ADM_Id_RegisteredBy;
            clientEmployeeVM.CLE_UnassignedOn = employee.CLE_UnassignedOn;
            clientEmployeeVM.ADM_Id_UnassignedBy = employee.ADM_Id_UnassignedBy;
            if (employee.DES_ != null)
                clientEmployeeVM.DES_Title = employee.DES_.DES_Title;
            if(employee.EMP_ != null)
                clientEmployeeVM.employee = EmployeesMapper.MapMe(employee.EMP_);
            if (_context != null)
            {
                clientEmployeeVM.IsEmployeeWagedForClient = clientsManager.IsEmployeeWagedForClient(employee.EMP_Id, employee.CLI_Id, employee.DES_Id);
            }           
            return clientEmployeeVM;
        }

        public static List<ClientEmployeeVM> mapEmployees(List<Clients_Employees> employees, RMERPContext _context=null)
        {
            List<ClientEmployeeVM> lst = new List<ClientEmployeeVM>();
            foreach (Clients_Employees employee in employees)
                lst.Add(mapMe(employee, _context));
            return lst;
        }
    }
}
