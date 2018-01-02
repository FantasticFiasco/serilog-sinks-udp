// Copyright 2015-2018 Serilog Contributors
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
