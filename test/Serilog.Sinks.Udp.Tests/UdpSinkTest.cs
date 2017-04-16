using System.Net;

namespace Serilog
{
    public class UdpSinkTest : SinkFixture
    {
        public UdpSinkTest()
        {
            Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo
                .Udp(IPAddress.Loopback, 7071)
                .CreateLogger();
        }
    }
}
