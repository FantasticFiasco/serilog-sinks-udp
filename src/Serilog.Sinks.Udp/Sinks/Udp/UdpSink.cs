// Copyright 2015-2017 Serilog Contributors
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
        private readonly ITextFormatter formatter;

        private IUdpClient client;

        /// <summary>
        /// Construct a <see cref="UdpSink"/>.
        /// </summary>
        /// <param name="client">
        /// The UDP client responsible for sending multicast messages.
        /// </param>
        /// <param name="remoteAddress">
        /// The <see cref="IPAddress"/> of the remote host or multicast group to which the UDP
        /// client should sent the logging event.
        /// </param>
        /// <param name="remotePort">
        /// The TCP port of the remote host or multicast group to which the UDP client should sent
        /// the logging event.
        /// </param>
        /// <param name="formatter">Formatter used to convert log events to text.</param>
        public UdpSink(
            IUdpClient client,
            IPAddress remoteAddress,
            int remotePort,
            ITextFormatter formatter)
            : base(1000, TimeSpan.FromSeconds(0.5))
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));
            if (remoteAddress == null)
                throw new ArgumentNullException(nameof(remoteAddress));
            if (remotePort < IPEndPoint.MinPort || remotePort > IPEndPoint.MaxPort)
                throw new ArgumentOutOfRangeException(nameof(remotePort));
            if (formatter == null)
                throw new ArgumentNullException(nameof(formatter));

            remoteEndPoint = new IPEndPoint(remoteAddress, remotePort);
            this.formatter = formatter;
            this.client = client;
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
                        formatter.Format(logEvent, stringWriter);

                        byte[] buffer = Encoding.UTF8.GetBytes(
                            stringWriter
                                .ToString()
                                .Trim()
                                .ToCharArray());

                        await client
                            .SendAsync(buffer, buffer.Length, remoteEndPoint)
                            .ConfigureAwait(false);
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