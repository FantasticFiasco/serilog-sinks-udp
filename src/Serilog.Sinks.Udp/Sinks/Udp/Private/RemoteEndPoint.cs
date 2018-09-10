using System;
using System.Net;

namespace Serilog.Sinks.Udp.Private
{
    internal class RemoteEndPoint
    {
        public RemoteEndPoint(string address, int port)
        {
            if (port < IPEndPoint.MinPort || port > IPEndPoint.MaxPort) throw new ArgumentOutOfRangeException(nameof(port));

            Address = address ?? throw new ArgumentNullException(nameof(address));
            Port = port;

            if (IPAddress.TryParse(address, out var ipAddress))
            {
                IPEndPoint = new IPEndPoint(ipAddress, port);
            }
        }

        public string Address { get; }

        public int Port { get; }

        public IPEndPoint IPEndPoint { get; }
    }
}
