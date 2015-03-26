# serilog-sinks-udp

A sink for Serilog that sends UDP packages over the network. Published to [NuGet](https://www.nuget.org/packages/serilog.sinks.udp).

### Usage

<pre>
    Serilog.ILogger log = new LoggerConfiguration()
        .MinimumLevel.Verbose()
        .WriteTo.Udp(IPAddress.Loopback, 7071)
        .CreateLogger();
</pre>