using System.Globalization;
using Microsoft.Extensions.Configuration;
using Serilog.Sinks.Udp.Private;

namespace Serilog
{
    public class SettingsFileUsingOutputTemplateTest : SinkFixture
    {
        public SettingsFileUsingOutputTemplateTest()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings_output_template.json")
                .Build();

            RemoteAddress = configuration["Serilog:WriteTo:0:Args:remoteAddress"].ToIPAddress();
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
