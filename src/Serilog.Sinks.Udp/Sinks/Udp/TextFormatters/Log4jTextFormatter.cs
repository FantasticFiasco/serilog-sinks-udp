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

using Serilog.Events;
using Serilog.Formatting;
using System;
using System.IO;
using System.Xml;
using System.Reflection;

namespace Serilog.Sinks.Udp.TextFormatters
{
    /// <summary>
    /// Text formatter serializing log events into log4j complient XML.
    /// </summary>
    public class Log4jTextFormatter : ITextFormatter
    {
        private static readonly string SourceContextPropertyName = "SourceContext";
        private static readonly string ThreadIdPropertyName = "ThreadId";

        /// <summary>
        /// Format the log event into the output.
        /// </summary>
        /// <param name="logEvent">The event to format.</param>
        /// <param name="output">The output.</param>
        public void Format(LogEvent logEvent, TextWriter output)
        {
            output.Write("<log4j:event");

            WriteLogger(logEvent, output);
            WriteTimestamp(logEvent, output);
            WriteLevel(logEvent, output);
            WriteThread(logEvent, output);

            output.Write(">");

            WriteMessage(logEvent, output);
            WriteException(logEvent, output);

            output.Write("</log4j:event>");
        }

        private static void WriteLogger(LogEvent logEvent, TextWriter output)
        {
            if (logEvent.Properties.TryGetValue(SourceContextPropertyName, out LogEventPropertyValue sourceContext))
            {
                output.Write($" logger=\"{((ScalarValue)sourceContext).Value}\"");
            }
        }

        private static void WriteTimestamp(LogEvent logEvent, TextWriter output)
        {
            var milliseconds = logEvent.Timestamp.ToUnixTimeMilliseconds();
            output.Write($" timestamp=\"{XmlConvert.ToString(milliseconds)}\"");
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
                output.Write(" thread=\"");
                threadId.Render(output);
                output.Write("\"");
            }
        }

        private static void WriteMessage(LogEvent logEvent, TextWriter output)
        {
            output.Write("<log4j:message>");
            logEvent.RenderMessage(output);
            output.Write("</log4j:message>");
        }

        private static void WriteException(LogEvent logEvent, TextWriter output)
        {
            if (logEvent.Exception == null)
            {
                return;
            }

            output.Write($"<log4j:throwable>{logEvent.Exception.ToString()}</log4j:throwable>");
        }
    }
}
