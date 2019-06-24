using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Shouldly;
using Xunit;

namespace Serilog.Sinks.Udp.Private
{
    public class UdpClientFactoryShould : IDisposable
    {
        private static readonly string Payload = "test";
        private static readonly byte[] PayloadAsBytes = Encoding.UTF8.GetBytes(Payload);

        private IUdpClient client;

        [Theory]
        [InlineData(InternetProtocol.Version4, AddressFamily.InterNetwork)]
        [InlineData(InternetProtocol.Version6, AddressFamily.InterNetworkV6)]
        public void SupportInternetProtocols(InternetProtocol internetProtocol, AddressFamily expectedAddressFamily)
        {
            // Act
            client = UdpClientFactory.Create(0, internetProtocol);

            // Assert
            client.Client.AddressFamily.ShouldBe(expectedAddressFamily);
        }

        [Fact]
        public void UseDualModeOnInternetProtocolVersion6()
        {
            // Act
            client = UdpClientFactory.Create(0, InternetProtocol.Version6);

            // Assert
            client.Client.DualMode.ShouldBeTrue();
        }

        [Theory]
        [InlineData(InternetProtocol.Version4, "127.0.0.1")]
        [InlineData(InternetProtocol.Version6, "::1")]
        public async void SendPayload(InternetProtocol internetProtocol, string address)
        {
            // Arrange
            client = UdpClientFactory.Create(0, internetProtocol);

            var ipAddress = IPAddress.Parse(address);

            using (var server = new UdpClient(0, ipAddress.AddressFamily))
            {
                var receiveTask = server.ReceiveAsync();

                // Act
                await client.SendAsync(
                    PayloadAsBytes,
                    PayloadAsBytes.Length,
                    new IPEndPoint(ipAddress, ((IPEndPoint)server.Client.LocalEndPoint).Port));

                // Assert
                var receivedData = (await receiveTask).Buffer;
                Encoding.UTF8.GetString(receivedData).ShouldBe(Payload);
            }
        }

        public void Dispose()
        {
            client.Dispose();
        }
    }
}
