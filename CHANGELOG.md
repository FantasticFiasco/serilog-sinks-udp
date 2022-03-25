# Change Log

All notable changes to this project will be documented in this file.

This project adheres to [Semantic Versioning](http://semver.org/) and is following [Keep a CHANGELOG](http://keepachangelog.com/).

## Unreleased

### :skull: Removed

- [BREAKING CHANGE] Support for .NET Framework 4.5 due to [deprecation as of January 12, 2016](https://docs.microsoft.com/en-us/lifecycle/products/microsoft-net-framework)

## [7.1.0] - 2020-10-19

### :zap: Added

- Support for .NET Standard 2.1 (contributed by [@augustoproiete](https://github.com/augustoproiete))

## [7.0.1] - 2020-08-14

### :syringe: Fixed

- Configure [SourceLink](https://github.com/dotnet/sourcelink) to embed untracked sources
- Configure [SourceLink](https://github.com/dotnet/sourcelink) to use deterministic builds when running on AppVeyor

## [7.0.0] - 2020-03-09

### :syringe: Fixed

- [#73](https://github.com/FantasticFiasco/serilog-sinks-udp/pull/73) [BREAKING CHANGE] Leading and trailing white-space characters are no longer removed from the payload before being sent over the network, thus respecting output templates that use those characters. (discovered by [@tagcode](https://github.com/tagcode))

## [6.0.0] - 2019-07-08

### :syringe: Fixed

- [#42](https://github.com/FantasticFiasco/serilog-sinks-udp/pull/42) Revert to support IPv4 on networks without IPv6 (contribution by [brettdavis-bmw](https://github.com/brettdavis-bmw))
- [#45](https://github.com/FantasticFiasco/serilog-sinks-udp/issues/45) Correctly XML escape exception message serialized by `Log4jTextFormatter`
- [#51](https://github.com/FantasticFiasco/serilog-sinks-udp/issues/51) Correctly XML escape all properties serialized by `Log4jTextFormatter` and `Log4netTextFormatter`

### :dizzy: Changed

- [BREAKING CHANGE] The API for creating a sink has changed. The sink was originally intended for IPv4 addresses, but has along the way gotten support for host names and IPv6 addresses. These changes calls for a small redesign of the API, benefiting the majority of the consumers using application settings instead of code configuration. The following chapters will describe your migration path.

#### Migrate sink created by code

Lets assume you have configured the sink using code, and your code looks something like this.

```csharp
Serilog.ILogger log = new LoggerConfiguration()
  .MinimumLevel.Verbose()
  .WriteTo.Udp(IPAddress.Loopback, 7071)
  .CreateLogger();
```

The `Udp` extension is no longer accepting `IPAddress` as first argument, and you are now required to specify the address family your remote address is targeting. Lets change the code above to conform to the new API.

```csharp
Serilog.ILogger log = new LoggerConfiguration()
  .MinimumLevel.Verbose()
  .WriteTo.Udp("localhost", 7071, AddressFamily.InterNetwork)
  .CreateLogger();
```

#### Migrate sink created by application settings

Lets assume you have configured the sink using application settings, and your configuration looks something like this.

```json
{
  "Serilog": {
    "MinimumLevel": "Verbose",
    "WriteTo": [
      {
        "Name": "Udp",
        "Args": {
          "remoteAddress": "localhost",
          "remotePort": 7071
        }
      }
    ]
  }
}
```

You are now required to specify the address family your remote address is targeting. Lets change the configuration above to conform to the new API.

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

## [5.0.1] - 2019-01-07

### :zap: Added

- Support for .NET Framework 4.6.1 due to recommendations from the [cross-platform targeting guidelines](https://docs.microsoft.com/en-us/dotnet/standard/library-guidance/cross-platform-targeting#multi-targeting)
- Support for .NET Standard 2.0 due to recommendations from the [cross-platform targeting guidelines](https://docs.microsoft.com/en-us/dotnet/standard/library-guidance/cross-platform-targeting#net-standard)

## [5.0.0] - 2018-09-13

### :syringe: Fixed

- [#29](https://github.com/FantasticFiasco/serilog-sinks-udp/issues/29) Fix remote hostname implementation bug (contribution by [Nisheeth Barthwal](https://github.com/nbaztec))
- [#31](https://github.com/FantasticFiasco/serilog-sinks-udp/issues/30) Enable IPv6 dual mode operation for IPv4 mapped addresses (contribution by [Nisheeth Barthwal](https://github.com/nbaztec))

#### Breaking change

Please note that this release contains breaking changes. UDP packages are now sent using a socket in [dual mode](https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.socket.dualmode?view=netstandard-1.3), which tunnels IPv4 traffic over IPv6. For more information regarding dual mode, please see the [ASP.NET blog](https://blogs.msdn.microsoft.com/webdev/2013/01/08/dual-mode-sockets-never-create-an-ipv4-socket-again/). For questions regarding operating systems and IPv6 support, please see the [comparison of IPv6 support in operating systems](https://en.wikipedia.org/wiki/Comparison_of_IPv6_support_in_operating_systems).

## [4.1.0] - 2018-06-06

### :zap: Added

- [#26](https://github.com/FantasticFiasco/serilog-sinks-udp/issues/26) Ability to specify a hostname instead of an IP address when configuring sink using [Serilog.Settings.Configuration](https://github.com/serilog/serilog-settings-configuration) (contribution by [Marek Stachura](https://github.com/marekstachura))

## [4.0.1] - 2018-01-29

### :syringe: Fixed

- [#21](https://github.com/FantasticFiasco/serilog-sinks-udp/issues/21) Invalid XML generated by `Log4jTextFormatter` and `Log4netTextFormatter`

### :skull: Removed

- Support for .NET Standard 2.0 since the sink also has support for .NET Standard 1.3

## [4.0.0] - 2017-11-09

### :zap: Added

- [BREAKING CHANGE] Option to specify a logging level switch (contribution by [Gian Maria](https://github.com/alkampfergit))

## [3.3.0] - 2017-09-13

### :zap: Added

- Text formatter compliant with log4net XML schema, thus compatible with [Log4View](http://www.log4view.com) (contribution by [Johan van Rhyn](https://github.com/jvanrhyn))

## [3.2.0] - 2017-08-26

### :zap: Added

- Text formatter compliant with log4j XML schema, thus compatible with [Log2Console](https://github.com/Statyk7/log2console)

## [3.1.0] - 2017-08-20

### :zap: Added

- Support for .NET Core 2.0

## [3.0.0] - 2017-05-02

### :zap: Added

- Full parity between code and JSON configuration of sink by introducing [Serilog.Settings.Configuration](https://github.com/serilog/serilog-settings-configuration) support for extension method `Udp(IPAddress remoteAddress, int remotePort, ITextFormatter formatter, int localPort, LogEventLevel restrictedToMinimumLevel)`.

### :dizzy: Changed

- [BREAKING CHANGE] Renamed argument `remoteAddressAsString` to `remoteAddress`. JSON configuration using [Serilog.Settings.Configuration](https://github.com/serilog/serilog-settings-configuration) will have to be updated accordingly.

## [2.3.1] - 2017-04-24

### :syringe: Fixed

- Package project URL

## [2.3.0] - 2017-04-14

### :zap: Added

- [#7](https://github.com/FantasticFiasco/serilog-sinks-udp/issues/7) - Support for .NET Standard 1.3 (contribution by [Scott Martin](https://github.com/scottamartin))

## [2.2.0] - 2017-03-16

### :zap: Added

- Support for [Serilog.Settings.Configuration](https://github.com/serilog/serilog-settings-configuration) (contribution by [Jon](https://github.com/bonejon))

## [2.1.0] - 2016-11-01

### :zap: Added

- Support for specifying a log event formatter

## [2.0.0] - 2016-10-31

### :zap: Added

- [BREAKING CHANGE] Support for Serilog 2.0.0
- Support for .NET Standard 1.5

## [1.1.0] - 2016-01-19

### :zap: Added

- Support for Serilog v1.5.14.

## [1.0.1] - 2015-03-30

### :syringe: Fixed

- URL to NuGet package icon.

## [1.0.0] - 2015-03-26

Initial version.