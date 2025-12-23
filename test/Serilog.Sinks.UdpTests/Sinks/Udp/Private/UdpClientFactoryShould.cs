using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Serilog.Sinks.Udp.Private;

public class UdpClientFactoryShould : IDisposable
{
    private static readonly string Payload = "test";
    private static readonly byte[] PayloadAsBytes = Encoding.UTF8.GetBytes(Payload);

    private IUdpClient client;


    [Fact]
    public void UseDualModeOnInterNetworkV6()
    {
        // Act
        client = UdpClientFactory.Create(0, AddressFamily.InterNetworkV6, false);

        // Assert
        client.Client.DualMode.ShouldBeTrue();
    }

    [Theory]
    [InlineData("127.0.0.1", AddressFamily.InterNetwork)]
    [InlineData("::1", AddressFamily.InterNetworkV6)]
    public async Task SendPayload(string address, AddressFamily family)
    {
        // Arrange
        client = UdpClientFactory.Create(0, family, false);

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
