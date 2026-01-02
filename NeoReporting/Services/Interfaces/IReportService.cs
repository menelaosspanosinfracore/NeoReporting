using ReportGenerator.Api.Models.Requests;
using ReportGenerator.Api.Models.Responses;

namespace ReportGenerator.Api.Services.Interfaces
{
    public interface IReportService
    {
        ReportResult GenerateReport(ReportRequest request);
    }
}
