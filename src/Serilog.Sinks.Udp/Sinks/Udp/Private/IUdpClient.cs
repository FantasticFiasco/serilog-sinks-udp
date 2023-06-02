// Copyright 2015-2023 Serilog Contributors
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

using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Serilog.Sinks.Udp.Private;

/// <summary>
/// Interface responsible for sending UDP messages.
/// </summary>
public interface IUdpClient
{
    /// <summary>
    /// Gets the underlying network <see cref="Socket"/>.
    /// </summary>
    Socket Client { get; }

    /// <summary>
    /// Sends a UDP datagram asynchronously to a remote host.
    /// </summary>
    /// <param name="datagram">
    /// An array of type <see cref="byte"/> that specifies the UDP datagram that you intend to
    /// send represented as an array of bytes.
    /// </param>
    /// <param name="bytes">
    /// The number of bytes in the datagram.
    /// </param>
    /// <param name="endPoint">
    /// An <see cref="IPEndPoint"/> that represents the host and port to which to send the datagram.
    /// </param>
    /// <returns></returns>
    Task<int> SendAsync(byte[] datagram, int bytes, IPEndPoint endPoint);

    /// <summary>
    /// Sends a UDP datagram asynchronously to a remote host.
    /// </summary>
    /// <param name="datagram">
    /// An array of type <see cref="byte"/> that specifies the UDP datagram that you intend to
    /// send represented as an array of bytes.
    /// </param>
    /// <param name="bytes">
    /// The number of bytes in the datagram.
    /// </param>
    /// <param name="hostname">
    /// The name of the remote host to which you intend to send the datagram.
    /// </param>
    /// <param name="port">
    /// The remote port number with which you intend to communicate.
    /// </param>
    /// <returns>
    /// Returns <see cref="Task{TResult}"/>.
    /// </returns>
    Task<int> SendAsync(byte[] datagram, int bytes, string hostname, int port);

#if NET4
        /// <summary>
        /// Closes the UDP connection.
        /// </summary>
        void Close();
#else
    /// <summary>
    /// Releases the managed and unmanaged resources used by the <see cref="IUdpClient"/>.
    /// </summary>
    void Dispose();
#endif
}