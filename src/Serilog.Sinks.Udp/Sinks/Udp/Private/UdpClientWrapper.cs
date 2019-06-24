// Copyright 2015-2019 Serilog Contributors
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

namespace Serilog.Sinks.Udp.Private
{
    internal class UdpClientWrapper : IUdpClient
    {
        private readonly UdpClient client;

        public UdpClientWrapper(int localPort, InternetProtocol internetProtocol)
        {
            if (localPort < IPEndPoint.MinPort || localPort > IPEndPoint.MaxPort) throw new ArgumentOutOfRangeException(nameof(localPort));


            var addressFamily = ToAddressFamily(internetProtocol);

            client = localPort == 0
                ? new UdpClient(addressFamily)
                : new UdpClient(localPort, addressFamily);

            // Allow for IPv4 mapped addresses over IPv6
            if (internetProtocol == InternetProtocol.Version6)
            {
                client.Client.DualMode = true;
            }
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

        private static AddressFamily ToAddressFamily(InternetProtocol internetProtocol)
        {
            switch (internetProtocol)
            {
                case InternetProtocol.Version4:
                    return AddressFamily.InterNetwork;

                case InternetProtocol.Version6:
                    return AddressFamily.InterNetworkV6;

                default:
                    throw new ArgumentException($"Internet protocol {internetProtocol} is not supported.");
            }
        }
    }
}
