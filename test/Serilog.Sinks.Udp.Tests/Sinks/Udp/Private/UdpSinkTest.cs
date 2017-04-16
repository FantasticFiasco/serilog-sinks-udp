using System.Net;
using Moq;
using Serilog.Formatting.Display;
using Serilog.Support;
using Xunit;

namespace Serilog.Sinks.Udp.Private
{
    public class UdpSinkTest
    {
        private readonly UdpClientMock client;
        private readonly IPAddress remoteAddress;
        private readonly int remotePort;
        private readonly UdpSink sink;

        public UdpSinkTest()
        {
            client = new UdpClientMock();
            remoteAddress = IPAddress.Loopback;
            remotePort = 7071;
            sink = new UdpSink(
                client.Object,
                remoteAddress,
                remotePort,
                new MessageTemplateTextFormatter(string.Empty, null));
        }

        [Fact]
        public void SentToCorrectEndPoint()
        {
            // Act
            sink.Emit(Some.DebugEvent());

            // Assert
            client.VerifySendAsync(remoteAddress, remotePort, Times.Once());
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
                sink.Emit(Some.DebugEvent());
            }

            // Assert
            counter.Wait();
        }
    }
}
