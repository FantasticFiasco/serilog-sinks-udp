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

        [Theory]
        [InlineData("Some < source context", "Some &lt; source context")]
        [InlineData("Some > source context", "Some &gt; source context")]
        [InlineData("Some & source context", "Some &amp; source context")]
        // The following characters should be escaped in a XML attribute
        [InlineData("Some \" source context", "Some &quot; source context")]
        [InlineData("Some ' source context", "Some &apos; source context")]
        [InlineData("Some \n source context", "Some &#xA; source context")]
        [InlineData("Some \r source context", "Some &#xD; source context")]
        [InlineData("Some \t source context", "Some &#x9; source context")]
        public void WriteEscapedLoggerAttribute(string sourceContext, string expected)
        {
            // Arrange
            var logEvent = Some.LogEvent();
            logEvent.AddOrUpdateProperty(new LogEventProperty("SourceContext", new ScalarValue(sourceContext)));

            // Act
            formatter.Format(logEvent, output);

            // Assert
            output.ToString().ShouldContain($" logger=\"{expected}\"");

            // Lets make sure that the escaped XML can be deserialized back into its original form
            Deserialize().Root.Attribute("logger").Value.ShouldBe(sourceContext);
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

        [Theory]
        [InlineData("Some < thread", "Some &lt; thread")]
        [InlineData("Some > thread", "Some &gt; thread")]
        [InlineData("Some & thread", "Some &amp; thread")]
        // The following characters should be escaped in a XML attribute
        [InlineData("Some \" thread", "Some &quot; thread")]
        [InlineData("Some ' thread", "Some &apos; thread")]
        [InlineData("Some \n thread", "Some &#xA; thread")]
        [InlineData("Some \r thread", "Some &#xD; thread")]
        [InlineData("Some \t thread", "Some &#x9; thread")]
        public void WriteEscapedTheadAttribute(string thread, string expected)
        {
            // Arrange
            var logEvent = Some.LogEvent();
            logEvent.AddOrUpdateProperty(new LogEventProperty("ThreadId", new ScalarValue(thread)));

            // Act
            formatter.Format(logEvent, output);

            // Assert
            output.ToString().ShouldContain($" thread=\"{expected}\"");

            // Lets make sure that the escaped XML can be deserialized back into its original form
            Deserialize().Root.Attribute("thread").Value.ShouldBe(thread);
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

        [Theory]
        [InlineData("Some < message", "Some &lt; message")]
        [InlineData("Some > message", "Some &gt; message")]
        [InlineData("Some & message", "Some &amp; message")]
        // The following characters should not be escaped in a XML element
        [InlineData("Some \" message", "Some \" message")]
        [InlineData("Some ' message", "Some ' message")]
        [InlineData("Some \n message", "Some \n message")]
        [InlineData("Some \r message", "Some \r message")]
        [InlineData("Some \t message", "Some \t message")]
        public void WriteEscapedMessageElement(string message, string expected)
        {
            // Arrange
            var logEvent = Some.LogEvent(message: message);

            // Act
            formatter.Format(logEvent, output);

            // Assert
            output.ToString().ShouldContain($"<log4j:message>{expected}</log4j:message>");

            // Lets make sure that the escaped XML can be deserialized back into its original form.
            //
            // "\r" are deserialized into "\n" by the .NET XML serializer, thus we need to
            // compensate for that.
            message = message.Replace("\r", "\n");

            Deserialize().Root.Element(Namespace + "message").Value.ShouldBe(message);
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
        [InlineData("Some < message", "Some &lt; message")]
        [InlineData("Some > message", "Some &gt; message")]
        [InlineData("Some & message", "Some &amp; message")]
        // The following characters should not be escaped in a XML element
        [InlineData("Some \" message", "Some \" message")]
        [InlineData("Some ' message", "Some ' message")]
        [InlineData("Some \n message", "Some \n message")]
        [InlineData("Some \r message", "Some \r message")]
        [InlineData("Some \t message", "Some \t message")]
        public void WriteEscapedExceptionElement(string message, string expected)
        {
            // Arrange
            var logEvent = Some.LogEvent(exception: new DivideByZeroException(message));

            // Act
            formatter.Format(logEvent, output);

            // Assert
            output.ToString().ShouldContain($"<log4j:throwable>System.DivideByZeroException: {expected}</log4j:throwable>");

            // Lets make sure that the escaped XML can be deserialized back into its original form.
            //
            // "\r" are deserialized into "\n" by the .NET XML serializer, thus we need to
            // compensate for that.
            message = message.Replace("\r", "\n");

            Deserialize().Root.Element(Namespace + "throwable").Value.ShouldBe($"System.DivideByZeroException: {message}");
        }

        private XDocument Deserialize()
        {
            return XDocument.Parse(output.ToString());
        }
    }
}
