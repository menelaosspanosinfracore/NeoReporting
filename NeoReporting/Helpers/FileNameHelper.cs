namespace ReportGenerator.Api.Helpers
{
    public static class FileNameHelper
    {
        public static string GetReportFileName(string baseName) => $"{baseName}_{System.Guid.NewGuid()}";
    }
}
