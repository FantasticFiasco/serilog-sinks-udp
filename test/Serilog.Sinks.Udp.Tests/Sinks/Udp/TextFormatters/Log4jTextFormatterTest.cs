using System;
using Serilog.Events;
using Xunit;

namespace Serilog.Sinks.Udp.TextFormatters
{
    public class Log4jTextFormatterTest
    {
        [Fact]
        public void Logger()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void Timestamp()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        [Fact]
        public void Thead()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void Message()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void Exception()
        {
            throw new NotImplementedException();
        }
    }
}
