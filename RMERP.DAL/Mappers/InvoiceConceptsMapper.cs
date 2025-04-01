using System;
using System.Collections.Generic;
using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Web;

namespace RMERP.DAL.Mappers
{
    public class InvoiceConceptsMapper
    {
        public static Invoice_ConceptsVM mapMe(Invoice_Concept Invoice_Concept)
        {
            Invoice_ConceptsVM conceptsVM = new Invoice_ConceptsVM();
            conceptsVM.INC_Id = Invoice_Concept.INC_Id;
            conceptsVM.INV_Id = Invoice_Concept.INV_Id;
            conceptsVM.INC_Serial_Number = Invoice_Concept.INC_Serial_Number;
            conceptsVM.INC_Description = Invoice_Concept.INC_Description.Replace("???????", "");
            conceptsVM.INC_Total = Invoice_Concept.INC_Total;
            if (Invoice_Concept.INV != null)
                conceptsVM.INV_ = Invoice_Concept.INV;
            return conceptsVM;
        }
        public static List<Invoice_ConceptsVM> mapMe(List<Invoice_Concept> Invoice_Concepts)
        {
            List<Invoice_ConceptsVM> conceptsVMs = new List<Invoice_ConceptsVM>();
            foreach(Invoice_Concept Invoice_Concept in Invoice_Concepts)
            {
                conceptsVMs.Add(mapMe(Invoice_Concept));
            }
            return conceptsVMs;
        }
        public static Invoice_Concept mapMeModel(Invoice_ConceptsVM Invoice_ConceptVM)
        {
            Invoice_Concept concept = new Invoice_Concept();
            concept.INC_Id = Invoice_ConceptVM.INC_Id;
            concept.INV_Id = Invoice_ConceptVM.INV_Id;
            concept.INC_Serial_Number = Invoice_ConceptVM.INC_Serial_Number;
            concept.INC_Description = Invoice_ConceptVM.INC_Description;
            concept.INC_Total = Invoice_ConceptVM.INC_Total;
            if (Invoice_ConceptVM.INV_ != null)
                concept.INV = Invoice_ConceptVM.INV_;
            return concept;
        }
    }
}
