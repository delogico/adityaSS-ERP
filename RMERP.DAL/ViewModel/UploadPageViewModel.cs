using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMERP.DAL.ViewModel
{
    public class UploadPageViewModel
    {
        public string WageMonth { get; set; }
        public string ClientName { get; set; }
        public int WageId { get; set; }
        public int ClientId { get; set; }
        public IFormFile TemplateFile { get; set; }
        public string TemplateName { get; set; }
    }
}
