# serilog-sinks-udp

[![Build status](https://ci.appveyor.com/api/projects/status/p7gx5eltx8u0op7d?svg=true)](https://ci.appveyor.com/project/FantasticFiasco/serilog-sinks-udp)

A Serilog sink that sends UDP packages over the network. Published to [NuGet](https://www.nuget.org/packages/serilog.sinks.udp).

### Usage

```c#
<pre>
    Serilog.ILogger log = new LoggerConfiguration()
        .MinimumLevel.Verbose()
        .WriteTo.Udp(IPAddress.Loopback, 7071)
        .CreateLogger();
</pre>
```