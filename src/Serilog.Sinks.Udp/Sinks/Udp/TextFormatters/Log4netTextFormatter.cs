// Copyright 2015-2023 Serilog Contributors
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

using Serilog.Events;
using Serilog.Formatting;
using System.IO;
using Serilog.Sinks.Udp.Private;

namespace Serilog.Sinks.Udp.TextFormatters
{
    /// <summary>
    /// Text formatter serializing log events into log4net compliant XML.
    /// </summary>
    public class Log4netTextFormatter : ITextFormatter
    {
        private static readonly string SourceContextPropertyName = "SourceContext";
        private static readonly string ThreadIdPropertyName = "ThreadId";
        private static readonly string UserNamePropertyName = "EnvironmentUserName";
        private static readonly string ProcessNamePropertyName = "ProcessName";
        private static readonly string MethodPropertyName = "Method";
        private static readonly string MachineNamePropertyName = "MachineName";

        private readonly XmlSerializer xmlSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Log4netTextFormatter"/> class.
        /// </summary>
        public Log4netTextFormatter()
        {
            xmlSerializer = new XmlSerializer();
        }

        /// <summary>
        /// Format the log event into the output.
        /// </summary>
        /// <param name="logEvent">The event to format.</param>
        /// <param name="output">The output.</param>
        public void Format(LogEvent logEvent, TextWriter output)
        {
            output.Write("<log4net:event xmlns:log4net=\"http://logging.apache.org/log4net/schemas/log4net-events-1.2/\"");

            WriteLogger(logEvent, output);
            WriteEventTime(logEvent, output);
            WriteLevel(logEvent, output);
            WriteThread(logEvent, output);
            WriteUserName(logEvent, output);
            WriteProcessName(logEvent, output);
            output.Write(">");

            output.Write("<log4net:locationInfo");
            WriteLocationInfoClass(logEvent, output);
            WriteLocationInfoMethod(logEvent, output);
            output.Write("/>");

            output.Write("<log4net:properties>");
            WriteHostName(logEvent, output);
            output.Write("</log4net:properties>");

            WriteMessage(logEvent, output);
            WriteException(logEvent, output);

            output.Write("</log4net:event>");
        }

        private void WriteLogger(LogEvent logEvent, TextWriter output)
        {
            if (logEvent.Properties.TryGetValue(SourceContextPropertyName, out var sourceContext))
            {
                var sourceContextValue = ((ScalarValue)sourceContext).Value.ToString();
                output.Write($" logger=\"{xmlSerializer.SerializeXmlValue(sourceContextValue, true)}\"");
            }
        }

        private static void WriteEventTime(LogEvent logEvent, TextWriter output)
        {
            var eventTime = logEvent.Timestamp;
            output.Write($" timestamp=\"{eventTime.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz")}\"");
        }

        private static void WriteLevel(LogEvent logEvent, TextWriter output)
        {
            string level;

            switch (logEvent.Level)
            {
                case LogEventLevel.Fatal:
                    level = "FATAL";
                    break;

                case LogEventLevel.Error:
                    level = "ERROR";
                    break;

                case LogEventLevel.Warning:
                    level = "WARN";
                    break;

                case LogEventLevel.Information:
                    level = "INFO";
                    break;

                case LogEventLevel.Debug:
                    level = "DEBUG";
                    break;

                default:
                    level = "TRACE";
                    break;
            }

            output.Write($" level=\"{level}\"");
        }

        private void WriteThread(LogEvent logEvent, TextWriter output)
        {
            if (logEvent.Properties.TryGetValue(ThreadIdPropertyName, out var threadId))
            {
                var threadIdValue = ((ScalarValue)threadId).Value.ToString();
                output.Write($" thread=\"{xmlSerializer.SerializeXmlValue(threadIdValue, true)}\"");
            }
        }

        private void WriteUserName(LogEvent logEvent, TextWriter output)
        {
            if (logEvent.Properties.TryGetValue(UserNamePropertyName, out var userName))
            {
                var userNameValue = ((ScalarValue)userName).Value.ToString();
                output.Write($" username=\"{xmlSerializer.SerializeXmlValue(userNameValue, true)}\"");
            }
        }

        private void WriteProcessName(LogEvent logEvent, TextWriter output)
        {
            if (logEvent.Properties.TryGetValue(ProcessNamePropertyName, out var processName))
            {
                var processNameValue = ((ScalarValue)processName).Value.ToString();
                output.Write($" domain=\"{xmlSerializer.SerializeXmlValue(processNameValue, true)}\"");
            }
        }

        private void WriteLocationInfoClass(LogEvent logEvent, TextWriter output)
        {
            if (logEvent.Properties.TryGetValue(SourceContextPropertyName, out var sourceContext))
            {
                var sourceContextValue = ((ScalarValue)sourceContext).Value.ToString();
                output.Write($" class=\"{xmlSerializer.SerializeXmlValue(sourceContextValue, true)}\"");
            }
        }

        private void WriteLocationInfoMethod(LogEvent logEvent, TextWriter output)
        {
            if (logEvent.Properties.TryGetValue(MethodPropertyName, out var methodName))
            {
                var methodNameValue = ((ScalarValue)methodName).Value.ToString();
                output.Write($" method=\"{xmlSerializer.SerializeXmlValue(methodNameValue, true)}\"");
            }
        }

        private void WriteHostName(LogEvent logEvent, TextWriter output)
        {
            if (logEvent.Properties.TryGetValue(MachineNamePropertyName, out var machineName))
            {
                var machineNameValue = ((ScalarValue)machineName).Value.ToString();
                output.Write($" <log4net:data name=\"log4net:HostName\" value=\"{xmlSerializer.SerializeXmlValue(machineNameValue, true)}\"></log4net:data>");
            }
        }

        private void WriteMessage(LogEvent logEvent, TextWriter output)
        {
            output.Write("<log4net:message>");
            xmlSerializer.SerializeXmlValue(output, logEvent.RenderMessage(), false);
            output.Write("</log4net:message>");
        }

        private void WriteException(LogEvent logEvent, TextWriter output)
        {
            if (logEvent.Exception == null)
            {
                return;
            }

            output.Write("<log4net:throwable>");
            xmlSerializer.SerializeXmlValue(output, logEvent.Exception.ToString(), false);
            output.Write("</log4net:throwable>");
        }
    }
}
