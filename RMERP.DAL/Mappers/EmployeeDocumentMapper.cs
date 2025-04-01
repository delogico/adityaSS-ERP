using System;
using System.Collections.Generic;
using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace RMERP.DAL.Mappers
{
    public class EmployeeDocumentMapper
    {
        public static EmployeeDocumentsVM mapMe(Employee_Document document)
        {
            EmployeeDocumentsVM documentVM = new EmployeeDocumentsVM();
            documentVM.EMD_Id = document.EMD_Id;
            documentVM.DOT_Id = document.DOT_Id;
            documentVM.EMP_Id = document.EMP_Id;
            documentVM.EMD_Name = document.EMD_Name;
            documentVM.EMD_UploadedOn = document.EMD_UploadedOn;
            return documentVM;
        }
        public static Employee_Document mapMeModel(EmployeeDocumentsVM documentVM)
        {
            Employee_Document document = new Employee_Document();
            document.EMD_Id = documentVM.EMD_Id;
            document.DOT_Id = documentVM.DOT_Id;
            document.EMP_Id = documentVM.EMP_Id;
            document.EMD_Name = documentVM.EMD_Name;
            document.EMD_UploadedOn = documentVM.EMD_UploadedOn;
            return document;
        }

        public static List<EmployeeDocumentsVM> mapEmployeeDocuments(List<Employee_Document> documents)
        {
            List<EmployeeDocumentsVM> lst = new List<EmployeeDocumentsVM>();
            foreach (Employee_Document document in documents)
            {
                lst.Add(mapMe(document));
            }
            return lst;
        }
    }
}
