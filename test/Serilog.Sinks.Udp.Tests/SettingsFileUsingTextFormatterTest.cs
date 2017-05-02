using System.Net;
using Microsoft.Extensions.Configuration;
using Serilog.Formatting.Display;

namespace Serilog
{
    public class SettingsFileUsingTextFormatterTest : SinkFixture
    {
        public SettingsFileUsingTextFormatterTest()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("text_formatter_appsettings.json")
                .Build();

            RemoteAddress = IPAddress.Parse(configuration["Serilog:WriteTo:0:Args:remoteAddress"]);
            RemotePort = int.Parse(configuration["Serilog:WriteTo:0:Args:remotePort"]);

            Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }

        public class TextFormatter : MessageTemplateTextFormatter
        {
            public TextFormatter()
                : base("{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}", null)
            {
            }
        }
    }
}
