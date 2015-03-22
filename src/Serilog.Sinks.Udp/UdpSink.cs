// Copyright 2015 Serilog Contributors
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
    public sealed class UdpSink : ILogEventSink, IDisposable
    {
        private readonly IPEndPoint remoteEndPoint;
        private readonly ITextFormatter textFormatter;
        private readonly Encoding encoding;
        private readonly object syncRoot = new object();
        
        private UdpClient client;
        
        public UdpSink(
            int localPort,
            IPAddress remoteAddress,
            int remotePort,
            ITextFormatter textFormatter,
            Encoding encoding = null)
        {
            if (localPort < IPEndPoint.MinPort || localPort > IPEndPoint.MaxPort)
                throw new ArgumentOutOfRangeException("localPort");
            if (remoteAddress == null)
                throw new ArgumentNullException("remoteAddress");
            if (remotePort < IPEndPoint.MinPort || remotePort > IPEndPoint.MaxPort)
                throw new ArgumentOutOfRangeException("remotePort");
            if (textFormatter == null)
                throw new ArgumentNullException("textFormatter");

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
            if (logEvent == null) throw new ArgumentNullException("logEvent");

            try
            {
                lock (syncRoot)
                {
                    using (var stringWriter = new StringWriter())
                    {
                        textFormatter.Format(logEvent, stringWriter);
                        
                        byte[] buffer = encoding.GetBytes(stringWriter.ToString().ToCharArray());
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

        public void Dispose()
        {
            if (client != null)
            {
                client.Close();
                client = null;
            }
        }

        #endregion
    }
}