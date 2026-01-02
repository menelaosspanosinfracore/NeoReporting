using System;
using System.IO;
using System.Linq;
using System.Web.Http.Results;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using NeoReporting.Config;
using NeoReporting.Logging;
using ReportGenerator.Api.Models.Requests;
using ReportGenerator.Api.Models.Responses;
using ReportGenerator.Api.Services.Interfaces;

namespace ReportGenerator.Api.Services.Crystal
{
    public class CrystalReportService : IReportService
    {
        private readonly ILoggerService _logger;
        private readonly string _templatePath;
        private readonly string _outputPath;

        public CrystalReportService()
        {
            _logger = new NLogLogger();
            var settings = AppConfig.ReportSettings;

            if (settings == null)
                throw new InvalidOperationException("ReportSettings not configured");

            _templatePath = settings.TemplatePath;
            _outputPath = settings.OutputPath;
        }

        public ReportResult GenerateReport(ReportRequest request)
        {
            _logger.Info($"Generating report '{request.ReportName}'");

            ReportDocument report = new ReportDocument();
            ApplyOracleLogon(report, "dbUser", "dbPassword", "TNS_ALIAS");

            try
            {
                string rptFile = Path.Combine(_templatePath, request.ReportName + ".rpt");

                if (!File.Exists(rptFile))
                {
                    _logger.Error($"Report template not found: {rptFile}");
                    throw new FileNotFoundException("Report template not found", rptFile);
                }

                report.Load(rptFile);
                _logger.Debug("Report loaded successfully");

                // report.SetDatabaseLogon("dbUser", "dbPassword");

                //foreach (Table table in report.Database.Tables)
                //{
                //    var logonInfo = table.LogOnInfo;
                //    logonInfo.ConnectionInfo.UserID = "dbUser";
                //    logonInfo.ConnectionInfo.Password = "dbPassword";
                //    table.ApplyLogOnInfo(logonInfo);
                //}



                ApplyParameters(report, request);
                ApplyGrouping(report, request);

                var exportType = ResolveExportType(request.ExportFormat);
                var result = ExportReport(report, request.ReportName, exportType);
                _logger.Info($"Report generated: {result.FilePath}");


                return ExportReport(report, request.ReportName, exportType);
            }
            catch (Exception ex)
            {
                _logger.Error("Report generation failed", ex);
                throw;
            }
            finally
            {
                report.Close();
                report.Dispose();
                _logger.Debug("Crystal report disposed");
            }
        }

        private void ApplyOracleLogon(
                ReportDocument report,
                string user,
                string password,
                string tnsName)
                    {
                        foreach (Table table in report.Database.Tables)
                        {
                            var logonInfo = table.LogOnInfo;

                            logonInfo.ConnectionInfo.ServerName = tnsName; // TNS alias
                            logonInfo.ConnectionInfo.UserID = user;
                            logonInfo.ConnectionInfo.Password = password;
                            logonInfo.ConnectionInfo.DatabaseName = "";

                            table.ApplyLogOnInfo(logonInfo);
                        }
        }

        private void ApplyParameters(ReportDocument report, ReportRequest request)
        {
            if (request.Parameters == null) return;

            foreach (var param in request.Parameters)
                report.SetParameterValue(param.Key, param.Value);
        }

        private void ApplyGrouping(ReportDocument report, ReportRequest request)
        {
            if (request.GroupBy == null || !request.GroupBy.Any()) return;

            foreach (Section section in report.ReportDefinition.Sections)
                if (section.Name.StartsWith("GroupHeader"))
                    section.SectionFormat.EnableSuppress = true;

            foreach (string group in request.GroupBy)
            {
                var target = report.ReportDefinition.Sections
                    .Cast<Section>()
                    .FirstOrDefault(s => s.Name.Contains(group));

                if (target != null)
                    target.SectionFormat.EnableSuppress = false;
            }
        }

        private ExportFormatType ResolveExportType(string format)
        {
            switch (format?.ToUpper())
            {
                case "PDF": return ExportFormatType.PortableDocFormat;
                case "XLSX": return ExportFormatType.ExcelWorkbook;
                default: throw new NotSupportedException("Unsupported export format");
            }
        }

        private ReportResult ExportReport(
            ReportDocument report,
            string reportName,
            ExportFormatType exportType)
        {
            string fileId = Guid.NewGuid().ToString("N");
            string extension = exportType == ExportFormatType.PortableDocFormat ? "pdf" : "xlsx";

            string year = DateTime.UtcNow.Year.ToString();
            string month = DateTime.UtcNow.Month.ToString("D2");

            string folder = Path.Combine(_outputPath, year, month);
            Directory.CreateDirectory(folder);

            string fileName = $"{reportName}_{fileId}.{extension}";
            string fullPath = Path.Combine(folder, fileName);

            report.ExportToDisk(exportType, fullPath);

            return new ReportResult
            {
                FileId = fileId,
                FileName = fileName,
                FilePath = fullPath
            };
        }
    }
}
