using Microsoft.Extensions.Configuration;
using Serilog.Core;

namespace Serilog
{
    public class TextFormatterGivenAppSettingsAndIPv4Should : SinkFixture
    {
        public TextFormatterGivenAppSettingsAndIPv4Should()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings_text_formatter_ipv4.json")
                .Build();

            RemoteAddress = configuration["Serilog:WriteTo:0:Args:remoteAddress"];
            RemotePort = int.Parse(configuration["Serilog:WriteTo:0:Args:remotePort"]);

            Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }

        protected override string RemoteAddress{ get; }

        protected override int RemotePort { get; }

        protected override Logger Logger { get; }
    }
}
