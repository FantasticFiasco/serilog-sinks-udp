# Serilog.Sinks.Udp

[![Build status](https://ci.appveyor.com/api/projects/status/p7gx5eltx8u0op7d/branch/master?svg=true)](https://ci.appveyor.com/project/FantasticFiasco/serilog-sinks-udp) [![NuGet Version](http://img.shields.io/nuget/v/Serilog.Sinks.Udp.svg?style=flat)](https://www.nuget.org/packages/Serilog.Sinks.Udp/) [![Documentation](https://img.shields.io/badge/docs-wiki-yellow.svg)](https://github.com/serilog/serilog/wiki) [![Join the chat at https://gitter.im/serilog/serilog](https://img.shields.io/gitter/room/serilog/serilog.svg)](https://gitter.im/serilog/serilog) [![Help](https://img.shields.io/badge/stackoverflow-serilog-orange.svg)](http://stackoverflow.com/questions/tagged/serilog)

A [Serilog](http://serilog.net/) sink that sends UDP packages over the network.

**Package** - [Serilog.Sinks.Udp](https://www.nuget.org/packages/serilog.sinks.udp)
| **Platforms** - .NET 4.5, .NETStandard 1.5

### Getting started

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
PM> Install-Package Serilog.Sinks.Udp
```