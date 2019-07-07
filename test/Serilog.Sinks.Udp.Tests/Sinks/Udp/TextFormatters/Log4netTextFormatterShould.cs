using System;
using Serilog.Events;
using Xunit;
using Serilog.Support;
using Shouldly;
using System.IO;
using System.Xml.Linq;

namespace Serilog.Sinks.Udp.TextFormatters
{
    public class Log4netTextFormatterShould
    {
        private static readonly XNamespace Namespace = "http://logging.apache.org/log4net/schemas/log4net-events-1.2/";

        private readonly Log4netTextFormatter formatter;
        private readonly TextWriter output;

        public Log4netTextFormatterShould()
        {
            formatter = new Log4netTextFormatter();
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
            DateTime.TryParse(timestamp, out DateTime _).ShouldBeTrue();
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
        public void WriteUsernameAttribute()
        {
            // Arrange
            var logEvent = Some.LogEvent();
            logEvent.AddOrUpdateProperty(new LogEventProperty("EnvironmentUserName", new ScalarValue("some user")));

            // Act
            formatter.Format(logEvent, output);

            // Assert
            Deserialize().Root.Attribute("username").Value.ShouldBe("some user");
        }

        [Theory]
        [InlineData("Some < username", "Some &lt; username")]
        [InlineData("Some > username", "Some &gt; username")]
        [InlineData("Some & username", "Some &amp; username")]
        // The following characters should be escaped in a XML attribute
        [InlineData("Some \" username", "Some &quot; username")]
        [InlineData("Some ' username", "Some &apos; username")]
        [InlineData("Some \n username", "Some &#xA; username")]
        [InlineData("Some \r username", "Some &#xD; username")]
        [InlineData("Some \t username", "Some &#x9; username")]
        public void WriteEscapedUsernameAttribute(string username, string expected)
        {
            // Arrange
            var logEvent = Some.LogEvent();
            logEvent.AddOrUpdateProperty(new LogEventProperty("EnvironmentUserName", new ScalarValue(username)));

            // Act
            formatter.Format(logEvent, output);

            // Assert
            output.ToString().ShouldContain($" username=\"{expected}\"");

            // Lets make sure that the escaped XML can be deserialized back into its original form
            Deserialize().Root.Attribute("username").Value.ShouldBe(username);
        }

        [Fact]
        public void WriteDomainAttribute()
        {
            // Arrange
            var logEvent = Some.LogEvent();
            logEvent.AddOrUpdateProperty(new LogEventProperty("ProcessName", new ScalarValue("process name")));

            // Act
            formatter.Format(logEvent, output);

            // Assert
            Deserialize().Root.Attribute("domain").Value.ShouldBe("process name");
        }

        [Theory]
        [InlineData("Some < process name", "Some &lt; process name")]
        [InlineData("Some > process name", "Some &gt; process name")]
        [InlineData("Some & process name", "Some &amp; process name")]
        // The following characters should be escaped in a XML attribute
        [InlineData("Some \" process name", "Some &quot; process name")]
        [InlineData("Some ' process name", "Some &apos; process name")]
        [InlineData("Some \n process name", "Some &#xA; process name")]
        [InlineData("Some \r process name", "Some &#xD; process name")]
        [InlineData("Some \t process name", "Some &#x9; process name")]
        public void WriteEscapedDomainAttribute(string processName, string expected)
        {
            // Arrange
            var logEvent = Some.LogEvent();
            logEvent.AddOrUpdateProperty(new LogEventProperty("ProcessName", new ScalarValue(processName)));

            // Act
            formatter.Format(logEvent, output);

            // Assert
            output.ToString().ShouldContain($" domain=\"{expected}\"");

            // Lets make sure that the escaped XML can be deserialized back into its original form
            Deserialize().Root.Attribute("domain").Value.ShouldBe(processName);
        }

        [Fact]
        public void WriteClassAttribute()
        {
            // Arrange
            var logEvent = Some.LogEvent();
            logEvent.AddOrUpdateProperty(new LogEventProperty("SourceContext", new ScalarValue("source context")));

            // Act
            formatter.Format(logEvent, output);

            // Assert
            Deserialize().Root.Element(Namespace + "locationInfo").Attribute("class").Value.ShouldBe("source context");
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
        public void WriteEscapedClassAttribute(string sourceContext, string expected)
        {
            // Arrange
            var logEvent = Some.LogEvent();
            logEvent.AddOrUpdateProperty(new LogEventProperty("SourceContext", new ScalarValue(sourceContext)));

            // Act
            formatter.Format(logEvent, output);

            // Assert
            output.ToString().ShouldContain($" class=\"{expected}\"");

            // Lets make sure that the escaped XML can be deserialized back into its original form
            Deserialize().Root.Element(Namespace + "locationInfo").Attribute("class").Value.ShouldBe(sourceContext);
        }

        [Fact]
        public void WriteMethodAttribute()
        {
            // Arrange
            var logEvent = Some.LogEvent();
            logEvent.AddOrUpdateProperty(new LogEventProperty("Method", new ScalarValue("Void Method()")));

            // Act
            formatter.Format(logEvent, output);

            // Assert
            Deserialize().Root.Element(Namespace + "locationInfo").Attribute("method").Value.ShouldBe("Void Method()");
        }

        [Theory]
        [InlineData("Some < method", "Some &lt; method")]
        [InlineData("Some > method", "Some &gt; method")]
        [InlineData("Some & method", "Some &amp; method")]
        // The following characters should be escaped in a XML attribute
        [InlineData("Some \" method", "Some &quot; method")]
        [InlineData("Some ' method", "Some &apos; method")]
        [InlineData("Some \n method", "Some &#xA; method")]
        [InlineData("Some \r method", "Some &#xD; method")]
        [InlineData("Some \t method", "Some &#x9; method")]
        public void WriteEscapedMethodAttribute(string method, string expected)
        {
            // Arrange
            var logEvent = Some.LogEvent();
            logEvent.AddOrUpdateProperty(new LogEventProperty("Method", new ScalarValue(method)));

            // Act
            formatter.Format(logEvent, output);

            // Assert
            output.ToString().ShouldContain($" method=\"{expected}\"");

            // Lets make sure that the escaped XML can be deserialized back into its original form
            Deserialize().Root.Element(Namespace + "locationInfo").Attribute("method").Value.ShouldBe(method);
        }

        [Fact]
        public void WriteHostNameAttribute()
        {
            // Arrange
            var logEvent = Some.LogEvent();
            logEvent.AddOrUpdateProperty(new LogEventProperty("MachineName", new ScalarValue("MachineName")));

            // Act
            formatter.Format(logEvent, output);

            // Assert
            Deserialize().Root.Element(Namespace + "properties").Element(Namespace + "data").Attribute("value").Value.ShouldBe("MachineName");
        }

        [Theory]
        [InlineData("Some < hostname", "Some &lt; hostname")]
        [InlineData("Some > hostname", "Some &gt; hostname")]
        [InlineData("Some & hostname", "Some &amp; hostname")]
        // The following characters should be escaped in a XML attribute
        [InlineData("Some \" hostname", "Some &quot; hostname")]
        [InlineData("Some ' hostname", "Some &apos; hostname")]
        [InlineData("Some \n hostname", "Some &#xA; hostname")]
        [InlineData("Some \r hostname", "Some &#xD; hostname")]
        [InlineData("Some \t hostname", "Some &#x9; hostname")]
        public void WriteEscapedHostNameAttribute(string hostname, string expected)
        {
            // Arrange
            var logEvent = Some.LogEvent();
            logEvent.AddOrUpdateProperty(new LogEventProperty("MachineName", new ScalarValue(hostname)));

            // Act
            formatter.Format(logEvent, output);

            // Assert
            output.ToString().ShouldContain($" <log4net:data name=\"log4net:HostName\" value=\"{expected}\"></log4net:data>");

            // Lets make sure that the escaped XML can be deserialized back into its original form
            Deserialize().Root.Element(Namespace + "properties").Element(Namespace + "data").Attribute("value").Value.ShouldBe(hostname);
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
            output.ToString().ShouldContain($"<log4net:message>{expected}</log4net:message>");

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
            output.ToString().ShouldContain($"<log4net:throwable>System.DivideByZeroException: {expected}</log4net:throwable>");

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
