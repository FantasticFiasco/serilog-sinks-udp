// Copyright 2015-2026 Serilog Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Serilog.Sinks.Udp.Private;

internal class UdpClientWrapper : IUdpClient
{
    private readonly UdpClient client;

    public UdpClientWrapper(int localPort, AddressFamily family, bool enableBroadcast)
    {
        if (localPort < IPEndPoint.MinPort || localPort > IPEndPoint.MaxPort) throw new ArgumentOutOfRangeException(nameof(localPort));

        client = localPort == 0
            ? new UdpClient(family)
            : new UdpClient(localPort, family);

        // Allow for IPv4 mapped addresses over IPv6
        if (family == AddressFamily.InterNetworkV6)
        {
            client.Client.DualMode = true;
        }

        // Enable broadcasting
        client.EnableBroadcast = enableBroadcast;
    }

    public Socket Client => client.Client;

    public Task<int> SendAsync(byte[] datagram, int bytes, IPEndPoint endPoint)
    {
        return client.SendAsync(datagram, bytes, endPoint);
    }

    public Task<int> SendAsync(byte[] datagram, int bytes, string hostname, int port)
    {
        return client.SendAsync(datagram, bytes, hostname, port);
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
