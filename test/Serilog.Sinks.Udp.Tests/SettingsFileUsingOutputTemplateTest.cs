using System.Globalization;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace Serilog
{
    public class SettingsFileUsingOutputTemplateTest : SinkFixture
    {
        public SettingsFileUsingOutputTemplateTest()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("output_template_appsettings.json")
                .Build();

            RemoteAddress = IPAddress.Parse(configuration["Serilog:WriteTo:0:Args:remoteAddress"]);
            RemotePort = int.Parse(configuration["Serilog:WriteTo:0:Args:remotePort"]);

            Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }

        public class FormatProvider : CultureInfo
        {
            public FormatProvider()
                : base("en-US")
            {
            }
        }
    }
}
