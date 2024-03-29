<!-- omit in toc -->
# Serilog.Sinks.Udp - A Serilog sink sending UDP packages over the network

[![CI/CD](https://github.com/FantasticFiasco/serilog-sinks-udp/actions/workflows/ci-cd.yml/badge.svg)](https://github.com/FantasticFiasco/serilog-sinks-udp/actions/workflows/ci-cd.yml)
[![NuGet Version](http://img.shields.io/nuget/v/Serilog.Sinks.Udp.svg?style=flat)](https://www.nuget.org/packages/Serilog.Sinks.Udp/)
[![SemVer compatible](https://img.shields.io/badge/%E2%9C%85-SemVer%20compatible-blue)](https://semver.org/)
[![NuGet](https://img.shields.io/nuget/dt/Serilog.Sinks.Udp.svg)](https://www.nuget.org/packages/Serilog.Sinks.Udp/)
[![Documentation](https://img.shields.io/badge/docs-wiki-yellow.svg)](https://github.com/serilog/serilog/wiki)
[![Join the chat at https://gitter.im/serilog/serilog](https://img.shields.io/gitter/room/serilog/serilog.svg)](https://gitter.im/serilog/serilog)
[![Help](https://img.shields.io/badge/stackoverflow-serilog-orange.svg)](http://stackoverflow.com/questions/tagged/serilog)

__Package__ - [Serilog.Sinks.Udp](https://www.nuget.org/packages/serilog.sinks.udp)
| __Platforms__ - .NET Framework 4.6.1, .NET Standard 1.3/2.0/2.1

<!-- omit in toc -->
## Table of contents

- [Super simple to use](#super-simple-to-use)
- [Typical use case](#typical-use-case)
- [Event formatters](#event-formatters)
  - [`Log4jTextFormatter`](#log4jtextformatter)
  - [`Log4netTextFormatter`](#log4nettextformatter)
- [Sample applications](#sample-applications)
- [Install via NuGet](#install-via-nuget)
- [Donations](#donations)
- [Credit](#credit)

---

## Super simple to use

In the following example, the sink will send UDP packages on the network to localhost on port 7071.

```csharp
Serilog.ILogger log = new LoggerConfiguration()
  .MinimumLevel.Verbose()
  .WriteTo.Udp("localhost", 7071, AddressFamily.InterNetwork)
  .CreateLogger();
```

Used in conjunction with [Serilog.Settings.Configuration](https://github.com/serilog/serilog-settings-configuration) the sink can be configured in `appsettings.json`.

```json
{
  "Serilog": {
    "MinimumLevel": "Verbose",
    "WriteTo": [
      {
        "Name": "Udp",
        "Args": {
          "remoteAddress": "localhost",
          "remotePort": 7071,
          "family": "InterNetwork"
        }
      }
    ]
  }
}
```

Used in conjunction with [Serilog.Settings.AppSettings](https://github.com/serilog/serilog-settings-appsettings) the sink can be configured in XML `<appSettings>`.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <appSettings>
    <add key="serilog:minimum-level" value="Verbose" />
    <add key="serilog:using:Udp" value="Serilog.Sinks.Udp" />
    <add key="serilog:write-to:Udp" />
    <add key="serilog:write-to:Udp.remoteAddress" value="localhost" />
    <add key="serilog:write-to:Udp.remotePort" value="7071" />
    <add key="serilog:write-to:Udp.family" value="InterNetwork" />
  </appSettings>
</configuration>
```

## Typical use case

Producing log events is only half the story. Unless you are consuming them in a matter that benefits you in development or production, there is really no need to produce them in the first place.

In development I've been sending UDP packages to the loopback address, and used [Log2Console](https://github.com/Statyk7/log2console) to visualize them. It supports UDP receivers, and allows me to filter and search according to my needs.

Taking it to the next level is when you as a team agree on sending the log events to a multicast address, making them accessible to all team members. This can be beneficial for Quality Assurance who wishes to monitor log events from all instances of your running application.

## Event formatters

The event formatter is an output template on steroids. It has the responsibility of turning a single log event into a textual representation. It can serialize the log event into JSON, XML or anything else that matches the expectations of the receiver.

The sink comes pre-loaded with two XML based event formatters. One is matching the log4j schema expected by Log2Console and the other is matching the log4net schema expected by [Log4View](http://www.log4view.com).

### `Log4jTextFormatter`

The log event is formatted according to the log4j XML schema expected by Log2Console.

```xml
<log4j:event logger="Some.Serilog.Context" timestamp="1184286222308" level="ERROR" thread="1">
  <log4j:message>Something failed</log4j:message>
  <log4j:throwable>An exception describing the failure<log4j:throwable>
</log4j:event>
```

### `Log4netTextFormatter`

The log event is formatted according to the log4net XML schema expected by Log4View.

```xml
<log4net:event logger="Some.Serilog.Context" timestamp="2017-09-01T22:00:00.000+02:00" level="DEBUG" thread="1" username="MACHINE\username" domain="dotnet">
  <log4net:locationInfo class="Some.Serilog.Context" method="System.String Get(Int32)"/>
  <log4net:properties>
    <log4net:data name="log4net:HostName" value="MACHINE"/>
  </log4net:properties>
  <log4net:message>Something went wrong.</log4net:message>
  <log4net:throwable>System.ArgumentOutOfRangeException: Specified argument was out of the range of valid values.</log4net:throwable>
</log4net:event>
```

## Sample applications

The following sample applications demonstrate the usage of this sink in various contexts:

- [Serilog.Sinks.Udp - Sample in .NET Core](https://github.com/FantasticFiasco/serilog-sinks-udp-sample-dotnet-core) - Sample application producing log events in .NET Core
- [Serilog.Sinks.Udp - Sample in .NET Framework](https://github.com/FantasticFiasco/serilog-sinks-udp-sample-dotnet-framework) - Sample application producing log events in .NET Framework

## Install via NuGet

If you want to include the UDP sink in your project, you can [install it directly from NuGet](https://www.nuget.org/packages/Serilog.Sinks.UDP/).

To install the sink, run the following command in the Package Manager Console:

```
PM> Install-Package Serilog.Sinks.Udp
```

## Donations

If this project has helped you to stay productive and save money, you can buy me a cup of coffee :)

[![PayPal Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.me/FantasticFiasco)

## Credit

Thank you [JetBrains](https://www.jetbrains.com/) for your important initiative to support the open source community with free licenses to your products.

![JetBrains](./doc/resources/jetbrains.png)
