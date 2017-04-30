# Serilog.Sinks.Udp

[![Build status](https://ci.appveyor.com/api/projects/status/p7gx5eltx8u0op7d/branch/master?svg=true)](https://ci.appveyor.com/project/FantasticFiasco/serilog-sinks-udp) [![NuGet Version](http://img.shields.io/nuget/v/Serilog.Sinks.Udp.svg?style=flat)](https://www.nuget.org/packages/Serilog.Sinks.Udp/) [![Documentation](https://img.shields.io/badge/docs-wiki-yellow.svg)](https://github.com/serilog/serilog/wiki) [![Join the chat at https://gitter.im/serilog/serilog](https://img.shields.io/gitter/room/serilog/serilog.svg)](https://gitter.im/serilog/serilog) [![Help](https://img.shields.io/badge/stackoverflow-serilog-orange.svg)](http://stackoverflow.com/questions/tagged/serilog)

A [Serilog](http://serilog.net/) sink that sends UDP packages over the network.

**Package** - [Serilog.Sinks.Udp](https://www.nuget.org/packages/serilog.sinks.udp)
| **Platforms** - .NET 4.5, .NETStandard 1.3

### Getting started

In the example shown, the sink will send UDP packages on the network to localhost on port 7071.

```csharp
Serilog.ILogger log = new LoggerConfiguration()
  .MinimumLevel.Verbose()
  .WriteTo.Udp(IPAddress.Loopback, 7071)
  .CreateLogger();
```

Used in conjunction with [Serilog.Settings.Configuration](https://github.com/serilog/serilog-settings-configuration) the same sink can be configured in the following way:
```json
{
  "Serilog": {
    "MinimumLevel": "Verbose",
    "WriteTo": [
      {
        "Name": "Udp",
        "Args": {
          "remoteAddress": "127.0.0.1",
          "remotePort": "7071"
        } 
      }
    ]
  }
}
```

### Typical use case

Producing log events is only half the story. Unless you are consuming them in a matter that benefits you in development or production, there is really no need to produce them in the first place.

In development I've been sending UDP packages to the loopback address, and use [Log2Console](https://log2console.codeplex.com/) to visualize them. It supports UDP receivers, and allows me to filter and search according to my needs.

Taking it to the next level is when you as a team agree on sending the log events to a multicast address, making them accessible to all team members. This can be beneficial for Quality Assurance who wishes to monitor log events from all instances of your running application.

### Install via NuGet

If you want to include the UDP sink in your project, you can [install it directly from NuGet](https://www.nuget.org/packages/Serilog.Sinks.UDP/).

To install the sink, run the following command in the Package Manager Console:

```
PM> Install-Package Serilog.Sinks.Udp
```

### Credit

Thank you [JetBrains](https://www.jetbrains.com/) for your important initiative to support the open source community with free licenses to your products.
