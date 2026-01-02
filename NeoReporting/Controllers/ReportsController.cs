using System.Web.Http;
using NeoReporting.Logging;
using ReportGenerator.Api.Models.Requests;
using ReportGenerator.Api.Services.Crystal;

namespace ReportGenerator.Api.Controllers
{
    [RoutePrefix("api/reports")]
    public class ReportsController : ApiController
    {
        private readonly CrystalReportService _service;
        private readonly ILoggerService _logger = new NLogLogger();
        public ReportsController()
        {
            _service = new CrystalReportService();
        }

        [HttpPost]
        [Route("generate")]
        public IHttpActionResult Generate(ReportRequest request)
        {
            _logger.Info("Generate report API called");

            if (request == null)
            {
                return BadRequest("Invalid request");
                _logger.Info("Invalid request");
            }

            var result = _service.GenerateReport(request);
            return Ok(result);
        }



        [HttpGet]
        [Route("test")]
        public IHttpActionResult Test()
        {

            _logger.Info("Generate Test report API called");

            var request = new ReportRequest
            {
                ReportName = "test",
                ExportFormat = "PDF"
            };

            return Ok(_service.GenerateReport(request));
        }
    }
}
