using System.Net;
using Moq;
using Serilog.Core;
using Serilog.Sinks.Udp.Private;
using Serilog.Support;
using Xunit;

namespace Serilog
{
    public abstract class SinkFixture
    {
        private readonly UdpClientMock client;

        protected SinkFixture()
        {
            client = new UdpClientMock(IPAddress.Loopback, 7071);
            UdpClientFactory.Create = (_, __) => client.Object;
        }

        protected Logger Logger { get; set; }

        [Fact]
        public void SentToCorrectEndPoint()
        {
            // Act
            Logger.Write(Some.DebugEvent());

            // Assert
            client.VerifySendAsync(Times.Once());
        }

        [Theory]
        [InlineData(1)]         // 1 batch
        [InlineData(10)]        // 1 batch
        [InlineData(100)]       // 1 batch
        [InlineData(1000)]      // ~1 batch
        [InlineData(10000)]     // ~10 batches
        public void Emit(int numberOfEvents)
        {
            // Arrange
            var counter = new Counter(numberOfEvents);

            client
                .SetupSendAsync()
                .Callback(() => counter.Increment());

            // Act
            for (int i = 0; i < numberOfEvents; i++)
            {
                Logger.Write(Some.DebugEvent());
            }

            // Assert
            counter.Wait();
        }
    }
}
