using Microsoft.Extensions.Configuration;
using Serilog.Formatting.Display;
using Serilog.Sinks.Udp.Private;

namespace Serilog
{
    public class SettingsFileUsingTextFormatterTest : SinkFixture
    {
        public SettingsFileUsingTextFormatterTest()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings_text_formatter.json")
                .Build();

            RemoteAddress = configuration["Serilog:WriteTo:0:Args:remoteAddress"].ToIPAddress();
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
