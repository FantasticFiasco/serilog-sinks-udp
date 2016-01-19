// Copyright 2015-2016 Serilog Contributors
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
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Serilog.Core;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Formatting;

namespace Serilog.Sinks.Udp
{
    /// <summary>
    /// Send log events as UDP packages over the network.
    /// </summary>
    public sealed class UdpSink : ILogEventSink, IDisposable
    {
        private readonly IPEndPoint remoteEndPoint;
        private readonly ITextFormatter textFormatter;
        private readonly Encoding encoding;
        private readonly object syncRoot = new object();
        
        private UdpClient client;

        /// <summary>
        /// Construct a <see cref="UdpSink"/>.
        /// </summary>
        /// <param name="localPort">
        /// The TCP port from which the UDP client will communicate. Setting the value to 0 will
        /// cause the UDP client not to bind to a local port.
        /// </param>
        /// <param name="remoteAddress">
        /// The <see cref="IPAddress"/> of the remote host or multicast group to which the UDP
        /// client should sent the logging event.
        /// </param>
        /// <param name="remotePort">
        /// The TCP port of the remote host or multicast group to which the UDP client should sent
        /// the logging event.
        /// </param>
        /// <param name="textFormatter">Formatter used to convert log events to text.</param>
        /// <param name="encoding">
        /// Character encoding used to write the data on the UDP package. The default is
        /// <see cref="Encoding.Default"/>.
        /// </param>
        public UdpSink(
            int localPort,
            IPAddress remoteAddress,
            int remotePort,
            ITextFormatter textFormatter,
            Encoding encoding = null)
        {
            if (localPort < IPEndPoint.MinPort || localPort > IPEndPoint.MaxPort)
                throw new ArgumentOutOfRangeException(nameof(localPort));
            if (remoteAddress == null)
                throw new ArgumentNullException(nameof(remoteAddress));
            if (remotePort < IPEndPoint.MinPort || remotePort > IPEndPoint.MaxPort)
                throw new ArgumentOutOfRangeException(nameof(remotePort));
            if (textFormatter == null)
                throw new ArgumentNullException(nameof(textFormatter));

            remoteEndPoint = new IPEndPoint(remoteAddress, remotePort);
            this.textFormatter = textFormatter;
            this.encoding = encoding ?? Encoding.Default;

            client = localPort == 0 ?
                new UdpClient(remoteAddress.AddressFamily) :
                new UdpClient(localPort, remoteAddress.AddressFamily);
        }
        
        #region ILogEventSink Members

        /// <summary>
        /// Emit the provided log event to the sink.
        /// </summary>
        /// <param name="logEvent">The log event to write.</param>
        public void Emit(LogEvent logEvent)
        {
            if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));

            try
            {
                lock (syncRoot)
                {
                    using (var stringWriter = new StringWriter())
                    {
                        textFormatter.Format(logEvent, stringWriter);

                        byte[] buffer = encoding.GetBytes(stringWriter
                            .ToString()
                            .Trim()
                            .ToCharArray());

                        client.Send(buffer, buffer.Length, remoteEndPoint);
                    }
                }
            }
            catch (Exception e)
            {
                SelfLog.WriteLine("Failed to send UDP package. {0}", e);
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (client != null)
            {
                // UdpClient does not implement IDisposable, but calling Close disables the
                // underlying socket and releases all managed and unmanaged resources associated
                // with the instance.
                client.Close();
                client = null;
            }
        }

        #endregion
    }
}