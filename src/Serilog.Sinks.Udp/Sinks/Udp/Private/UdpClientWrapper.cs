using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Serilog.Sinks.Udp.Private
{
    internal class UdpClientWrapper : IUdpClient
    {
        private readonly UdpClient client;

        public UdpClientWrapper(
            int localPort,
            IPAddress remoteAddress)
        {
            if (localPort < IPEndPoint.MinPort || localPort > IPEndPoint.MaxPort)
                throw new ArgumentOutOfRangeException(nameof(localPort));
            if (remoteAddress == null)
                throw new ArgumentNullException(nameof(remoteAddress));

            client = localPort == 0
                ? new UdpClient(remoteAddress.AddressFamily)
                : new UdpClient(localPort, remoteAddress.AddressFamily);
        }

        public Task<int> SendAsync(byte[] datagram, int bytes, IPEndPoint endPoint)
        {
            return client.SendAsync(datagram, bytes, endPoint);
        }

#if NET4
        public void Close()
        {
            client.Close();
        }
#else
        public void Dispose()
        {
            client.Dispose();
        }
#endif
    }
}
