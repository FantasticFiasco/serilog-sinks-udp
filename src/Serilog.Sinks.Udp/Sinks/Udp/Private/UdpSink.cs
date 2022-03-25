﻿// Copyright 2015-2022 Serilog Contributors
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
using System.Text;
using System.Threading.Tasks;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Sinks.PeriodicBatching;

namespace Serilog.Sinks.Udp.Private
{
    /// <summary>
    /// Send log events as UDP packages over the network.
    /// </summary>
    internal class UdpSink : IBatchedLogEventSink, IDisposable
    {
        private readonly IUdpClient client;
        private readonly RemoteEndPoint remoteEndPoint;
        private readonly ITextFormatter formatter;

        public UdpSink(
            IUdpClient client,
            string remoteAddress,
            int remotePort,
            ITextFormatter formatter)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
            remoteEndPoint = new RemoteEndPoint(remoteAddress, remotePort);
            this.formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
        }

        #region IBatchedLogEventSink Members

        public async Task EmitBatchAsync(IEnumerable<LogEvent> events)
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
                                .ToCharArray());

                        if (remoteEndPoint.IPEndPoint != null)
                        {
                            await client
                                .SendAsync(buffer, buffer.Length, remoteEndPoint.IPEndPoint)
                                .ConfigureAwait(false);
                        }
                        else
                        {
                            await client
                                .SendAsync(buffer, buffer.Length, remoteEndPoint.Address, remoteEndPoint.Port)
                                .ConfigureAwait(false);
                        }
                    }
                }
                catch (Exception e)
                {
                    SelfLog.WriteLine("Failed to send UDP package. {0}", e);
                }
            }
        }

        public Task OnEmptyBatchAsync()
        {
            return Task.CompletedTask;
        }

        #endregion

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
#if NET4
                // IUdpClient does not implement IDisposable, but calling Close disables the
                // underlying socket and releases all managed and unmanaged resources associated
                // with the instance.
                client?.Close();
#else
                client?.Dispose();
#endif
            }
        }
    }
}
