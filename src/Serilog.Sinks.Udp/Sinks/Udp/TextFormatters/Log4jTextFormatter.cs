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
using System.Xml;
using Serilog.Sinks.Udp.Private;

namespace Serilog.Sinks.Udp.TextFormatters;

/// <summary>
/// Text formatter serializing log events into log4j compliant XML.
/// </summary>
public class Log4jTextFormatter : ITextFormatter
{
    private static readonly string SourceContextPropertyName = "SourceContext";
    private static readonly string ThreadIdPropertyName = "ThreadId";

    private readonly XmlSerializer xmlSerializer;

    /// <summary>
    /// Initializes a new instance of the <see cref="Log4jTextFormatter"/> class.
    /// </summary>
    public Log4jTextFormatter()
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
        output.Write("<log4j:event xmlns:log4j=\"http://jakarta.apache.org/log4j/\"");

        WriteLogger(logEvent, output);
        WriteTimestamp(logEvent, output);
        WriteLevel(logEvent, output);
        WriteThread(logEvent, output);

        output.Write(">");

        WriteMessage(logEvent, output);
        WriteException(logEvent, output);

        output.Write("</log4j:event>");
    }

    private void WriteLogger(LogEvent logEvent, TextWriter output)
    {
        if (logEvent.Properties.TryGetValue(SourceContextPropertyName, out var sourceContext))
        {
            var sourceContextValue = ((ScalarValue)sourceContext).Value.ToString();
            output.Write($" logger=\"{xmlSerializer.SerializeXmlValue(sourceContextValue, true)}\"");
        }
    }

    private static void WriteTimestamp(LogEvent logEvent, TextWriter output)
    {
        // Milliseconds since 1970-01-01
        var milliseconds = logEvent.Timestamp.UtcDateTime.Ticks / 10000L - 62135596800000L;
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

    private void WriteThread(LogEvent logEvent, TextWriter output)
    {
        if (logEvent.Properties.TryGetValue(ThreadIdPropertyName, out var threadId))
        {
            var threadIdValue = ((ScalarValue)threadId).Value.ToString();
            output.Write($" thread=\"{xmlSerializer.SerializeXmlValue(threadIdValue, true)}\"");
        }
    }

    private void WriteMessage(LogEvent logEvent, TextWriter output)
    {
        output.Write("<log4j:message>");
        xmlSerializer.SerializeXmlValue(output, logEvent.RenderMessage(), false);
        output.Write("</log4j:message>");
    }

    private void WriteException(LogEvent logEvent, TextWriter output)
    {
        if (logEvent.Exception == null)
        {
            return;
        }

        output.Write("<log4j:throwable>");
        xmlSerializer.SerializeXmlValue(output, logEvent.Exception.ToString(), false);
        output.Write("</log4j:throwable>");
    }
}