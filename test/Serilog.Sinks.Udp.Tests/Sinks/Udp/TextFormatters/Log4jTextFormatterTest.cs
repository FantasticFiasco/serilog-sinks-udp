using System;
using System.Collections.Generic;
using Serilog.Events;
using Xunit;
using Serilog.Support;
using Shouldly;
using System.IO;
using System.Xml.Linq;
using Serilog.Parsing;

namespace Serilog.Sinks.Udp.TextFormatters
{
    public class Log4jTextFormatterTest
    {
        private readonly Log4jTextFormatter formatter;
        private readonly TextWriter output;

        public Log4jTextFormatterTest()
        {
            formatter = new Log4jTextFormatter();
            output = new StringWriter();
        }

        [Fact]
        public void Logger()
        {
            // Arrange
            var logEvent = Some.LogEvent();
            logEvent.AddOrUpdateProperty(new LogEventProperty("SourceContext", new ScalarValue("source context")));

            // Act
            formatter.Format(logEvent, output);

            // Assert
            Deserialize().Root.Attribute("logger").Value.ShouldBe("source context");
        }

        [Fact]
        public void Timestamp()
        {
            // Act
            formatter.Format(Some.LogEvent(), output);

            // Assert
            var timestamp = Deserialize().Root.Attribute("timestamp").Value;
            long.TryParse(timestamp, out long _).ShouldBeTrue();
        }

        [Theory]
        [InlineData(LogEventLevel.Verbose, "TRACE")]
        [InlineData(LogEventLevel.Debug, "DEBUG")]
        [InlineData(LogEventLevel.Information, "INFO")]
        [InlineData(LogEventLevel.Warning, "WARN")]
        [InlineData(LogEventLevel.Error, "ERROR")]
        [InlineData(LogEventLevel.Fatal, "FATAL")]
        public void Level(LogEventLevel actual, string expected)
        {
            // Act
            formatter.Format(Some.LogEvent(level: actual), output);

            // Assert
            Deserialize().Root.Attribute("level").Value.ShouldBe(expected);
        }

        [Fact]
        public void Thead()
        {
            // Arrange
            var logEvent = Some.LogEvent();
            logEvent.AddOrUpdateProperty(new LogEventProperty("ThreadId", new ScalarValue("1")));

            // Act
            formatter.Format(logEvent, output);

            // Assert
            Deserialize().Root.Attribute("thread").Value.ShouldBe("1");
        }

        [Fact]
        public void Message()
        {
            // Arrange
            var logEvent = Some.LogEvent(message: "Some message");

            // Act
            formatter.Format(logEvent, output);

            // Assert
            Deserialize().Root.Element("message").Value.ShouldBe("Some message");
        }

        [Fact]
        public void Exception()
        {
            // Arrange
            var logEvent = Some.LogEvent(exception: new DivideByZeroException());

            // Act
            formatter.Format(logEvent, output);

            // Assert
            Deserialize().Root.Element("throwable").Value.ShouldNotBeNull();
        }

        private XDocument Deserialize()
        {
            var xmlWithoutNamespaces = output.ToString().Replace("log4j:", string.Empty);
            return XDocument.Parse(xmlWithoutNamespaces);
        }
    }
}
