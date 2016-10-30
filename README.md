# Serilog.Sinks.UDP

[![Build status](https://ci.appveyor.com/api/projects/status/p7gx5eltx8u0op7d?svg=true)](https://ci.appveyor.com/project/FantasticFiasco/serilog-sinks-udp)

A [Serilog](http://serilog.net/) sink that sends UDP packages over the network.

**Package** - [Serilog.Sinks.UDP](https://www.nuget.org/packages/serilog.sinks.udp)
| **Platforms** - .NET 4.6, .NETStandard 1.5

In the example shown, the sink will send UDP packages on the network to localhost on port 7071.

```csharp
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