﻿using Serilog.Core;
using Serilog.Sinks.Udp;
using Serilog.Support;

namespace Serilog
{
    public class OutputTemplateGivenCodeConfigurationAndIPv6Should : SinkFixture
    {
        private const string OutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message} - {Exception}";

        public OutputTemplateGivenCodeConfigurationAndIPv6Should()
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
                    internetProtocol: InternetProtocol.Version6,
                    outputTemplate: OutputTemplate,
                    formatProvider: new FormatProvider())
                .CreateLogger();
        }

        protected override string RemoteAddress { get; }

        protected override int RemotePort { get; }

        protected override Logger Logger { get; }
    }
}
