// Copyright 2015-2024 Serilog Contributors
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

namespace Serilog.Sinks.Udp.Private;

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

    /// <summary>
    /// It's a very small performance optimization to parse the IP address and use it instead
    /// of having the HTTP client trying to resolve the address and figure out that it isn't a
    /// hostname at all but instead an ordinary IP address.
    ///
    /// A small optimization indeed, but one that was requested by one of the consumers of the
    /// package.
    /// </summary>
    public IPEndPoint? IPEndPoint { get; }
}