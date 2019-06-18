using System;
using Serilog.Events;
using Xunit;
using Serilog.Support;
using Shouldly;
using System.IO;
using System.Xml.Linq;

namespace Serilog.Sinks.Udp.TextFormatters
{
    public class Log4jTextFormatterShould
    {
        private static readonly XNamespace Namespace = "http://jakarta.apache.org/log4j/";

        private readonly Log4jTextFormatter formatter;
        private readonly TextWriter output;

        public Log4jTextFormatterShould()
        {
            formatter = new Log4jTextFormatter();
            output = new StringWriter();
        }

        [Fact]
        public void WriteLoggerAttribute()
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
        public void WriteTimestampAttribute()
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
        public void WriteLevelAttribute(LogEventLevel actual, string expected)
        {
            // Act
            formatter.Format(Some.LogEvent(level: actual), output);

            // Assert
            Deserialize().Root.Attribute("level").Value.ShouldBe(expected);
        }

        [Fact]
        public void WriteTheadAttribute()
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
        public void WriteMessageElement()
        {
            // Arrange
            var logEvent = Some.LogEvent(message: "Some message");

            // Act
            formatter.Format(logEvent, output);

            // Assert
            Deserialize().Root.Element(Namespace + "message").Value.ShouldBe("Some message");
        }

        [Fact]
        public void WriteExceptionElement()
        {
            // Arrange
            var logEvent = Some.LogEvent(exception: new DivideByZeroException());

            // Act
            formatter.Format(logEvent, output);

            // Assert
            Deserialize().Root.Element(Namespace + "throwable").Value.ShouldNotBeNull();
        }

        [Theory]
        [InlineData("<", "&lt;")]
        [InlineData(">", "&gt;")]
        [InlineData("&", "&amp;")]
        public void WriteEscapedExceptionElement(string message, string expected)
        {
            // Arrange
            var logEvent = Some.LogEvent(exception: new DivideByZeroException(message));

            // Act
            formatter.Format(logEvent, output);

            // Assert
            output.ToString().ShouldContain($"<log4j:throwable>System.DivideByZeroException: {expected}</log4j:throwable>");
        }

        private XDocument Deserialize()
        {
            return XDocument.Parse(output.ToString());
        }
    }
}
