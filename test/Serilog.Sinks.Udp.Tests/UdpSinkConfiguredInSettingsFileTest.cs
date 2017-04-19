using System.Net;
using Microsoft.Extensions.Configuration;

namespace Serilog
{
    public class UdpSinkConfiguredInSettingsFileTest : SinkFixture
    {
        public UdpSinkConfiguredInSettingsFileTest()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            RemoteAddress = IPAddress.Parse(configuration["Serilog:WriteTo:0:Args:remoteAddressAsString"]);
            RemotePort = int.Parse(configuration["Serilog:WriteTo:0:Args:remotePort"]);

            Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }
    }
}
