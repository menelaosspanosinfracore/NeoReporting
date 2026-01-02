using System;
using Microsoft.Extensions.Configuration;
using ReportGenerator.Api.Config;

namespace NeoReporting.Config
{
    public static class AppConfig
    {
        private static IConfigurationRoot _configuration;

        static AppConfig()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            _configuration = builder.Build();
        }

        public static ReportSettings ReportSettings =>
            _configuration.GetSection("ReportSettings").Get<ReportSettings>();
    }
}
