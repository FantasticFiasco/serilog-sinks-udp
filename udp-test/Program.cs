using System;
using System.Net;
using Serilog;
using Serilog.Sinks.Udp.TextFormatters;

namespace udp_test
{
    class Program
    {
        static void Main(string[] args)
        {
            Serilog.ILogger log = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext()
                .Enrich.WithThreadId()
                .WriteTo.Udp(IPAddress.Loopback, 8090, formatter: new Log4jTextFormatter())
                .CreateLogger()
                .ForContext<Program>();

            log.Verbose("Hello {level}", "verbose");
            log.Debug("Hello {level}", "debug");
            log.Information("Hello {level}", "info");
            log.Warning("Hello {level}", "warn");
            log.Error("Hello {level}", "error");
            log.Fatal("Hello {level}", "fatal");

            log.Error(new DivideByZeroException(), "Some error");

            Console.ReadKey();
        }
    }
}
