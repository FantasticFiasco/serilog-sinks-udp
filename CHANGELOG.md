# Change Log

All notable changes to this project will be documented in this file.

This project adheres to [Semantic Versioning](http://semver.org/) and is following [Keep a CHANGELOG](http://keepachangelog.com/).

## Unreleased

## [3.1.0] - 2017-08-20

### Added

- Support for .NET Core 2.0

## [3.0.0] - 2017-05-02

### Added

- Full parity between code and JSON configuration of sink by introducing [Serilog.Settings.Configuration](https://github.com/serilog/serilog-settings-configuration) support for extension method `Udp(IPAddress remoteAddress, int remotePort, ITextFormatter formatter, int localPort, LogEventLevel restrictedToMinimumLevel)`.

### Changed

- [BREAKING CHANGE] Renamed argument `remoteAddressAsString` to `remoteAddress`. JSON configuration using [Serilog.Settings.Configuration](https://github.com/serilog/serilog-settings-configuration) will have to be updated accordingly.

## [2.3.1] - 2017-04-24

### Fixed

- Package project URL

## [2.3.0] - 2017-04-14

### Added

- [#7](https://github.com/FantasticFiasco/serilog-sinks-udp/issues/7) - Support for .NET Standard 1.3 (contribution by [scottamartin](https://github.com/scottamartin))

## [2.2.0] - 2017-03-16

### Added

- Support for [Serilog.Settings.Configuration](https://github.com/serilog/serilog-settings-configuration) (contribution by [bonejon](https://github.com/bonejon))

## [2.1.0] - 2016-11-01

### Added

- Support for specifying a log event formatter

## [2.0.0] - 2016-10-31

### Added

- [BREAKING CHANGE] Support for Serilog 2.0.0
- Support for .NET Standard 1.5

## [1.1.0] - 2016-01-19

### Added

- Support for Serilog v1.5.14.

## [1.0.1] - 2015-03-30

### Fixed

- URL to NuGet package icon.

## [1.0.0] - 2015-03-26

Initial version.