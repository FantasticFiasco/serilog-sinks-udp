// Copyright 2015-2018 Serilog Contributors
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

namespace Serilog.Sinks.Udp.TextFormatters
{
    /// <summary>
    /// Text formatter serializing log events into log4net complient XML.
    /// </summary>
    public class Log4netTextFormatter : ITextFormatter
    {
        private static readonly string SourceContextPropertyName = "SourceContext";
        private static readonly string ThreadIdPropertyName = "ThreadId";
        private static readonly string MachineNamePropertyName = "MachineName";
        private static readonly string UserNamePropertyName = "EnvironmentUserName";
        private static readonly string MethodPropertyName = "Method";
        private static readonly string ProcessNamePropertyName = "ProcessName";

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

        private static void WriteProcessName(LogEvent logEvent, TextWriter output)
        {
            if (logEvent.Properties.TryGetValue(ProcessNamePropertyName, out LogEventPropertyValue processName))
            {
                output.Write($" domain=\"{((ScalarValue)processName).Value}\"");
            }
        }

        private static void WriteLocationInfoClass(LogEvent logEvent, TextWriter output)
        {
            if (logEvent.Properties.TryGetValue(SourceContextPropertyName, out LogEventPropertyValue sourceContext))
            {
                output.Write($" class=\"{((ScalarValue)sourceContext).Value}\"");
            }
        }

        private static void WriteLocationInfoMethod(LogEvent logEvent, TextWriter output)
        {
            if (logEvent.Properties.TryGetValue(MethodPropertyName, out LogEventPropertyValue methodName))
            {
                output.Write($" method=\"{((ScalarValue)methodName).Value}\"");
            }
        }

        private static void WriteLogger(LogEvent logEvent, TextWriter output)
        {
            if (logEvent.Properties.TryGetValue(SourceContextPropertyName, out LogEventPropertyValue sourceContext))
            {
                output.Write($" logger=\"{((ScalarValue)sourceContext).Value}\"");
            }
        }

        private static void WriteUserName(LogEvent logEvent, TextWriter output)
        {
            if (logEvent.Properties.TryGetValue(UserNamePropertyName, out LogEventPropertyValue userName))
            {
                output.Write($" username=\"{((ScalarValue)userName).Value}\"");
            }
        }

        private static void WriteHostName(LogEvent logEvent, TextWriter output)
        {
            if (logEvent.Properties.TryGetValue(MachineNamePropertyName, out LogEventPropertyValue machineName))
            {
                output.Write($" <log4net:data name=\"log4net:HostName\" value=\"{((ScalarValue)machineName).Value}\"></log4net:data>");
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

        private static void WriteThread(LogEvent logEvent, TextWriter output)
        {
            if (logEvent.Properties.TryGetValue(ThreadIdPropertyName, out LogEventPropertyValue threadId))
            {
                output.Write($" thread=\"{((ScalarValue)threadId).Value}\"");
            }
        }

        private static void WriteMessage(LogEvent logEvent, TextWriter output)
        {
            output.Write("<log4net:message>");
            logEvent.RenderMessage(output);
            output.Write("</log4net:message>");
        }

        private static void WriteException(LogEvent logEvent, TextWriter output)
        {
            if (logEvent.Exception == null)
            {
                return;
            }

            output.Write($"<log4net:throwable>{logEvent.Exception}</log4net:throwable>");
        }
    }
}