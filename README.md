# UDP sink for Serilog

[![Build status](https://ci.appveyor.com/api/projects/status/p7gx5eltx8u0op7d?svg=true)](https://ci.appveyor.com/project/FantasticFiasco/serilog-sinks-udp)

A [Serilog](http://serilog.net/) sink that sends UDP packages over the network.

### Usage

```c#
Serilog.ILogger log = new LoggerConfiguration()
  .MinimumLevel.Verbose()
  .WriteTo.Udp(IPAddress.Loopback, 7071)
  .CreateLogger();
```

### Install via NuGet

If you want to include the UDP sink in your project, you can [install it directly from NuGet](https://www.nuget.org/packages/Serilog.Sinks.UDP/).

To install the sink, run the following command in the Package Manager Console:

```
PM> Install-Package Serilog.Sinks.UDP
```