using System;
using Xunit;

namespace Serilog.Sinks.Udp.Private
{
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using Shouldly;

    public class UdpClientShould : IDisposable
    {

        private const int  serverIpV4Port = 30457;
        private const int serverIpV6Port = 30458;

        private readonly UdpClient serverIpV4;
        private readonly UdpClient serverIpV6;

        public UdpClientShould()
        {
            Console.WriteLine("creating");
            serverIpV4 = new UdpClient(serverIpV4Port, AddressFamily.InterNetwork);
            serverIpV4.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 5000);

            serverIpV6 = new UdpClient(serverIpV6Port, AddressFamily.InterNetworkV6);
            serverIpV6.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 5000);
        }

        public void Dispose()
        {
            Console.WriteLine("done");
            serverIpV4.Close();
            serverIpV6.Close();
        }

        [Fact]
        public async void ConnectOnIPv4()
        {
            var expectedData = "test";
            var remoteEndpoint = new IPEndPoint(IPAddress.Loopback, serverIpV4Port);
            var client = UdpClientFactory.Create(0);
            var receive = serverIpV4.ReceiveAsync();

            var b = Encoding.UTF8.GetBytes(expectedData);
            await client.SendAsync(b, b.Length, remoteEndpoint);

            b = receive.Result.Buffer;
            var receivedData = Encoding.UTF8.GetString(b);

            receivedData.ShouldBe(expectedData);
        }

        [Fact]
        public async void ConnectOnIPv6()
        {
            var expectedData = "test";
            var remoteEndpoint = new IPEndPoint(IPAddress.IPv6Loopback, serverIpV6Port);
            var client = UdpClientFactory.Create(0);
            var receive = serverIpV6.ReceiveAsync();

            var b = Encoding.UTF8.GetBytes(expectedData);
            await client.SendAsync(b, b.Length, remoteEndpoint);

            b = receive.Result.Buffer;
            var receivedData = Encoding.UTF8.GetString(b);

            receivedData.ShouldBe(expectedData);
        }
    }
}
