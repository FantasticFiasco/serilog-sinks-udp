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
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Sinks.PeriodicBatching;

namespace Serilog.Sinks.Udp
{
    /// <summary>
    /// Send log events as UDP packages over the network.
    /// </summary>
    public sealed class UdpSink : PeriodicBatchingSink
    {
        private readonly IPEndPoint remoteEndPoint;
        private readonly ITextFormatter textFormatter;
        private readonly Encoding encoding;

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
        /// <see cref="Encoding.GetEncoding(int)"/>.
        /// </param>
        public UdpSink(
            int localPort,
            IPAddress remoteAddress,
            int remotePort,
            ITextFormatter textFormatter,
            Encoding encoding = null)
            : base(1000, TimeSpan.FromSeconds(0.5))
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
            this.encoding = encoding ?? Encoding.GetEncoding(0);

            client = localPort == 0
                ? new UdpClient(remoteAddress.AddressFamily)
                : new UdpClient(localPort, remoteAddress.AddressFamily);
        }

        #region PeriodicBatchingSink Members

        /// <summary>
        /// Emit a batch of log events, running asynchronously.
        /// </summary>
        /// <param name="events">The events to emit.</param>
        protected override async Task EmitBatchAsync(IEnumerable<LogEvent> events)
        {
            foreach (LogEvent logEvent in events)
            {
                try
                {
                    using (var stringWriter = new StringWriter())
                    {
                        textFormatter.Format(logEvent, stringWriter);

                        byte[] buffer = encoding.GetBytes(
                            stringWriter
                                .ToString()
                                .Trim()
                                .ToCharArray());

                        await client.SendAsync(buffer, buffer.Length, remoteEndPoint);
                    }
                }
                catch (Exception e)
                {
                    SelfLog.WriteLine("Failed to send UDP package. {0}", e);
                }
            }
        }

        /// <summary>
        /// Free resources held by the sink.
        /// </summary>
        /// <param name="disposing">
        /// If true, called because the object is being disposed; if false, the object is being
        /// disposed from the finalizer.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing && client != null)
            {
#if NET4
                // UdpClient does not implement IDisposable, but calling Close disables the
                // underlying socket and releases all managed and unmanaged resources associated
                // with the instance.
                client.Close();
#else
                client.Dispose();

#endif
                client = null;

            }
        }

        #endregion
    }
}