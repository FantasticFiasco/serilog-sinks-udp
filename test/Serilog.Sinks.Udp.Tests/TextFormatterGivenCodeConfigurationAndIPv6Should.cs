using System.Net.Sockets;
using Serilog.Core;
using Serilog.Support.TextFormatters;

namespace Serilog
{
    public class TextFormatterGivenCodeConfigurationAndIPv6Should : SinkFixture
    {
        public TextFormatterGivenCodeConfigurationAndIPv6Should()
        {
            var remoteAddress = "localhost";
            var remotePort = 7071;

            RemoteAddress = remoteAddress;
            RemotePort = remotePort;

            Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo
                .Udp(
                    remoteAddress,
                    remotePort,
                    AddressFamily.InterNetworkV6,
                    new TextFormatter())
                .CreateLogger();
        }

        protected override string RemoteAddress { get; }

        protected override int RemotePort { get; }

        protected override Logger Logger { get; }
    }
}
