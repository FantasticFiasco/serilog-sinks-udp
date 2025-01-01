// Copyright 2015-2025 Serilog Contributors
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
using System.Net.Sockets;

namespace Serilog.Sinks.Udp.Private;

/// <summary>
/// Factory class acting as a singleton producing instances of <see cref="IUdpClient"/>.
/// </summary>
public static class UdpClientFactory
{
    /// <summary>
    /// Gets or sets the factory creating instances of <see cref="IUdpClient"/>.
    /// </summary>
    public static Func<int, AddressFamily, bool, IUdpClient> Create { get; set; }
        = (localPort, family, enableBroadcast) => new UdpClientWrapper(localPort, family, enableBroadcast);
}
