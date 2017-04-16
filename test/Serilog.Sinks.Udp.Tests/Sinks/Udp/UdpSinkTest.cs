using System;
using System.Net;
using Moq;
using Serilog.Formatting.Display;
using Serilog.Sinks.Udp.Tests.Support;
using Xunit;

namespace Serilog.Sinks.Udp.Tests.Sinks.Udp
{
    public class UdpSinkTest
    {
        private readonly Mock<IUdpClient> client;
        private readonly IPAddress remoteAddress;
        private readonly int remotePort;
        private readonly UdpSink sink;

        public UdpSinkTest()
        {
            client = new Mock<IUdpClient>();
            remoteAddress = IPAddress.Loopback;
            remotePort = 7071;
            sink = new UdpSink(
                client.Object,
                remoteAddress,
                remotePort,
                new MessageTemplateTextFormatter(string.Empty, null));
        }

        [Fact]
        public void RemoteEndPoint()
        {
            // Act
            sink.Emit(Some.DebugEvent());

            // Assert
            client.Verify(
                mock => mock.SendAsync(
                    It.IsAny<byte[]>(),
                    It.IsAny<int>(),
                    It.Is<IPEndPoint>(remoteEndpoint => VerifyRemoteEndpoint(remoteEndpoint))),
                Times.Once);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        public void Emit(int numberOfEvents)
        {
            // Arrange
            var counter = new Counter(numberOfEvents);

            client
                .Setup(
                    mock => mock.SendAsync(
                        It.IsAny<byte[]>(),
                        It.IsAny<int>(),
                        It.IsAny<IPEndPoint>()))
                .Callback(() => counter.Increment());

            // Act
            for (int i = 0; i < numberOfEvents; i++)
            {
                sink.Emit(Some.DebugEvent());
            }

            // Assert
            counter.Wait();
        }

        private bool VerifyRemoteEndpoint(IPEndPoint endPoint)
        {
            return endPoint != null &&
                endPoint.AddressFamily == remoteAddress.AddressFamily &&
                endPoint.Address.Equals(remoteAddress) &&
                endPoint.Port == remotePort;
        }
    }
}
