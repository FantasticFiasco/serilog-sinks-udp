// Copyright 2015-2017 Serilog Contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.ComponentModel;
using System.Net;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;
using Serilog.Sinks.Udp.Private;
using Serilog.Core;

namespace Serilog
{
    /// <summary>
    /// Adds the WriteTo.Udp() extension method to <see cref="LoggerConfiguration"/>.
    /// </summary>
    public static class LoggerSinkConfigurationExtensions
    {
        private const string DefaultOutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}";

        /// <summary>
        /// Extension method providing JSON support via
        /// <a href="https://github.com/serilog/serilog-settings-configuration">Serilog.Settings.Configuration</a>.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static LoggerConfiguration Udp(
            this LoggerSinkConfiguration sinkConfiguration,
            string remoteAddress,
            int remotePort,
            int localPort = 0,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            LoggingLevelSwitch levelSwitch = null,
            string outputTemplate = DefaultOutputTemplate,
            IFormatProvider formatProvider = null)
        {
            return Udp(
                sinkConfiguration,
                IPAddress.Parse(remoteAddress),
                remotePort,
                localPort,
                restrictedToMinimumLevel,
                levelSwitch,
                outputTemplate,
                formatProvider);
        }

        /// <summary>
        /// Adds a sink that sends log events as UDP packages over the network.
        /// </summary>
        /// <param name="sinkConfiguration">Logger sink configuration.</param>
        /// <param name="remoteAddress">
        /// The <see cref="IPAddress"/> of the remote host or multicast group to which the UDP
        /// client should sent the logging event.
        /// </param>
        /// <param name="remotePort">
        /// The TCP port of the remote host or multicast group to which the UDP client should sent
        /// the logging event.
        /// </param>
        /// <param name="localPort">
        /// The TCP port from which the UDP client will communicate. The default is 0 and will
        /// cause the UDP client not to bind to a local port.
        /// </param>
        /// <param name="restrictedToMinimumLevel">
        /// The minimum level for events passed through the sink. The default is
        /// <see cref="LevelAlias.Minimum"/>.
        /// </param>
        /// <param name="levelSwitch">
        /// A switch allowing the pass-through minimum level to be changed at runtime.
        /// </param>
        /// <param name="outputTemplate">
        /// A message template describing the format used to write to the sink. The default is
        /// "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}".
        /// </param>
        /// <param name="formatProvider">
        /// Supplies culture-specific formatting information, or null.
        /// </param>
        /// <returns>Logger configuration, allowing configuration to continue.</returns>
        public static LoggerConfiguration Udp(
            this LoggerSinkConfiguration sinkConfiguration,
            IPAddress remoteAddress,
            int remotePort,
            int localPort = 0,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            LoggingLevelSwitch levelSwitch = null,
            string outputTemplate = DefaultOutputTemplate,
            IFormatProvider formatProvider = null)
        {
            if (sinkConfiguration == null)
                throw new ArgumentNullException(nameof(sinkConfiguration));
            if (outputTemplate == null)
                throw new ArgumentNullException(nameof(outputTemplate));

            var formatter = new MessageTemplateTextFormatter(outputTemplate, formatProvider);
            return Udp(
                sinkConfiguration,
                remoteAddress,
                remotePort,
                formatter,
                localPort,
                restrictedToMinimumLevel,
                levelSwitch);
        }

        /// <summary>
        /// Extension method providing JSON support via
        /// <a href="https://github.com/serilog/serilog-settings-configuration">Serilog.Settings.Configuration</a>.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static LoggerConfiguration Udp(
            this LoggerSinkConfiguration sinkConfiguration,
            string remoteAddress,
            int remotePort,
            ITextFormatter formatter,
            int localPort = 0,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            LoggingLevelSwitch levelSwitch = null)
        {
            return Udp(
                sinkConfiguration,
                IPAddress.Parse(remoteAddress),
                remotePort,
                formatter,
                localPort,
                restrictedToMinimumLevel,
                levelSwitch);
        }

        /// <summary>
        /// Adds a sink that sends log events as UDP packages over the network.
        /// </summary>
        /// <param name="sinkConfiguration">Logger sink configuration.</param>
        /// <param name="remoteAddress">
        /// The <see cref="IPAddress"/> of the remote host or multicast group to which the UDP
        /// client should sent the logging event.
        /// </param>
        /// <param name="remotePort">
        /// The TCP port of the remote host or multicast group to which the UDP client should sent
        /// the logging event.
        /// </param>
        /// <param name="formatter">
        /// Controls the rendering of log events into text, for example to log JSON. To control
        /// plain text formatting, use the overload that accepts an output template.
        /// </param>
        /// <param name="localPort">
        /// The TCP port from which the UDP client will communicate. The default is 0 and will
        /// cause the UDP client not to bind to a local port.
        /// </param>
        /// <param name="restrictedToMinimumLevel">
        /// The minimum level for events passed through the sink. The default is
        /// <see cref="LevelAlias.Minimum"/>.
        /// </param>
        /// <param name="levelSwitch">
        /// A switch allowing the pass-through minimum level to be changed at runtime.
        /// </param>
        /// <returns>Logger configuration, allowing configuration to continue.</returns>
        public static LoggerConfiguration Udp(
            this LoggerSinkConfiguration sinkConfiguration,
            IPAddress remoteAddress,
            int remotePort,
            ITextFormatter formatter,
            int localPort = 0,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            LoggingLevelSwitch levelSwitch = null)
        {
            if (sinkConfiguration == null)
                throw new ArgumentNullException(nameof(sinkConfiguration));

            var client = UdpClientFactory.Create(localPort, remoteAddress);
            var sink = new UdpSink(client, remoteAddress, remotePort, formatter);
            return sinkConfiguration.Sink(sink, restrictedToMinimumLevel, levelSwitch);
        }
    }
}