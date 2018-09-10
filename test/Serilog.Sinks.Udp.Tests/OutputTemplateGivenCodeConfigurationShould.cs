using Serilog.Core;
using Serilog.Support;

namespace Serilog
{
    public class OutputTemplateGivenCodeConfigurationShould : SinkFixture
    {
        private const string OutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message} - {Exception}";

        public OutputTemplateGivenCodeConfigurationShould()
        {
            var remoteAddress = "localhost";
            var remotePort = 7071;

            RemoteAddress = remoteAddress;
            RemotePort = remotePort;

            Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo
                .Udp(remoteAddress, remotePort, outputTemplate: OutputTemplate, formatProvider: new FormatProvider())
                .CreateLogger();
        }

        protected override string RemoteAddress { get; }

        protected override int RemotePort { get; }

        protected override Logger Logger { get; }
    }
}
