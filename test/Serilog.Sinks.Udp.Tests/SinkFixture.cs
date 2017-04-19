using System;
using System.Net;
using Moq;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.Udp.Private;
using Serilog.Support;
using Xunit;

namespace Serilog
{
    public abstract class SinkFixture : IDisposable
    {
        private readonly UdpClientMock client;

        protected SinkFixture()
        {
            client = new UdpClientMock();
            UdpClientFactory.Create = (_, __) => client.Object;
        }

        protected IPAddress RemoteAddress { get; set; }

        protected int RemotePort { get; set; }

        protected Logger Logger { get; set; }

        [Theory]
        [InlineData(LogEventLevel.Verbose)]
        [InlineData(LogEventLevel.Debug)]
        [InlineData(LogEventLevel.Information)]
        [InlineData(LogEventLevel.Warning)]
        [InlineData(LogEventLevel.Error)]
        [InlineData(LogEventLevel.Fatal)]
        public void Level(LogEventLevel level)
        {
            // Arrange
            var counter = new Counter(1);

            client
                .SetupSendAsync(RemoteAddress, RemotePort)
                .Callback(() => counter.Increment());

            // Act
            Logger.Write(level, "Some message");

            // Assert
            counter.Wait();

            // Verify that event is sent to the correct endpoint
            client.VerifySendAsync(RemoteAddress, RemotePort, Times.Once());
        }

        [Theory]
        [InlineData(1)]         // 1 batch
        [InlineData(10)]        // 1 batch
        [InlineData(100)]       // 1 batch
        [InlineData(1000)]      // ~1 batch
        [InlineData(10000)]     // ~10 batches
        public void Batches(int numberOfEvents)
        {
            // Arrange
            var counter = new Counter(numberOfEvents);

            client
                .SetupSendAsync(RemoteAddress, RemotePort)
                .Callback(() => counter.Increment());

            // Act
            for (int i = 0; i < numberOfEvents; i++)
            {
                Logger.Write(Some.DebugEvent());
            }

            // Assert
            counter.Wait();
        }

        public void Dispose()
        {
            Logger?.Dispose();

            UdpClientFactory.Create = null;
        }
    }
}
