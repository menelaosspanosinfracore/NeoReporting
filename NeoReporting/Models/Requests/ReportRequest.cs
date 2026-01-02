using System.Collections.Generic;

namespace ReportGenerator.Api.Models.Requests
{
    public class ReportRequest
    {
        public string ReportName { get; set; }
        public string ExportFormat { get; set; }   // PDF, XLSX
        public Dictionary<string, object> Parameters { get; set; }
        public List<string> GroupBy { get; set; }
    }
}
