using System.Net;

namespace Serilog
{
    public class CodeConfigurationTest : SinkFixture
    {
        public CodeConfigurationTest()
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
