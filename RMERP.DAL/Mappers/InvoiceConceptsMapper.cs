using System;
using System.Collections.Generic;
using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace RMERP.DAL.Mappers
{
    public class InvoiceConceptsMapper
    {
        public static Invoice_ConceptsVM mapMe(Invoice_Concepts Invoice_Concept)
        {
            Invoice_ConceptsVM conceptsVM = new Invoice_ConceptsVM();
            conceptsVM.INC_Id = Invoice_Concept.INC_Id;
            conceptsVM.INV_Id = Invoice_Concept.INV_Id;
            conceptsVM.INC_Serial_Number = Invoice_Concept.INC_Serial_Number;
            conceptsVM.INC_Description = Invoice_Concept.INC_Description;
            conceptsVM.INC_Total = Invoice_Concept.INC_Total;
            if (Invoice_Concept.INV_ != null)
                conceptsVM.INV_ = Invoice_Concept.INV_;
            return conceptsVM;
        }
        public static List<Invoice_ConceptsVM> mapMe(List<Invoice_Concepts> Invoice_Concepts)
        {
            List<Invoice_ConceptsVM> conceptsVMs = new List<Invoice_ConceptsVM>();
            foreach(Invoice_Concepts Invoice_Concept in Invoice_Concepts)
            {
                conceptsVMs.Add(mapMe(Invoice_Concept));
            }
            return conceptsVMs;
        }
        public static Invoice_Concepts mapMeModel(Invoice_ConceptsVM Invoice_ConceptVM)
        {
            Invoice_Concepts concept = new Invoice_Concepts();
            concept.INC_Id = Invoice_ConceptVM.INC_Id;
            concept.INV_Id = Invoice_ConceptVM.INV_Id;
            concept.INC_Serial_Number = Invoice_ConceptVM.INC_Serial_Number;
            concept.INC_Description = Invoice_ConceptVM.INC_Description.Replace("?", "");
            concept.INC_Total = Invoice_ConceptVM.INC_Total;
            if (Invoice_ConceptVM.INV_ != null)
                concept.INV_ = Invoice_ConceptVM.INV_;
            return concept;
        }
    }
}
