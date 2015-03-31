# serilog-sinks-udp

A Serilog sink that sends UDP packages over the network. Published to [NuGet](https://www.nuget.org/packages/serilog.sinks.udp).

### Usage

<pre>
    Serilog.ILogger log = new LoggerConfiguration()
        .MinimumLevel.Verbose()
        .WriteTo.Udp(IPAddress.Loopback, 7071)
        .CreateLogger();
</pre>

### Versions

#### 1.0.1
Updated NuGet package icon to reflect that the package is a Serilog sink on not Serilog itself

#### 1.0.0
Initial version