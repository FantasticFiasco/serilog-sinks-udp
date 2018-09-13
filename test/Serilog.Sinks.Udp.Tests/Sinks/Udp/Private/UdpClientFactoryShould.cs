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

        private readonly IUdpClient client;

        public UdpClientFactoryShould()
        {
            client = UdpClientFactory.Create(0);
        }

        [Theory]
        [InlineData("127.0.0.1")]
        [InlineData("::1")]
        public async void CreateUdpClientAndSendOnAddress(string address)
        {
            // Arrange
            var ipAddress = IPAddress.Parse(address);
            
            using (var server = new UdpClient(0, ipAddress.AddressFamily))
            {
                server.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 5000);

                var receiveTask = server.ReceiveAsync();

                // Act
                await client.SendAsync(
                    PayloadAsBytes,
                    PayloadAsBytes.Length,
                    new IPEndPoint(ipAddress, ((IPEndPoint)server.Client.LocalEndPoint).Port));

                // Assert
                Encoding.UTF8.GetString((await receiveTask).Buffer).ShouldBe(Payload);
            }
        }

        public void Dispose()
        {
            client.Dispose();
        }
    }
}
