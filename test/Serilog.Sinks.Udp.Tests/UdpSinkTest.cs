using System.Net;

namespace Serilog
{
    public class UdpSinkTest : SinkFixture
    {
        public UdpSinkTest()
        {
            RemoteAddress = IPAddress.Loopback;
            RemotePort = 7071;

            Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo
                .Udp(RemoteAddress, RemotePort)
                .CreateLogger();
        }
    }
}
