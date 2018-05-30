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
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Serilog.Debugging;

namespace Serilog.Sinks.Udp.Private
{
    /// <summary>
    /// Class containing extensions methods to <see cref="string"/>.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Converts specified address or hostname into an <see cref="IPAddress"/>.
        /// </summary>
        /// <param name="address">The address to convert.</param>
        /// <returns>
        /// An <see cref="IPAddress"/> if address is found to be a valid IP address or DNS name;
        /// otherwise null.
        /// </returns>
        public static IPAddress ToIPAddress(this string address)
        {
            if (IPAddress.TryParse(address, out var ipAddress))
            {
                return ipAddress;
            }

            try
            {
                // TODO: Use Dns.GetHostEntry when moving to .NET Standard 2.0
                var hostEntry = Dns.GetHostEntryAsync(address).Result;

                return hostEntry
                    .AddressList
                    .FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);
            }
            catch (Exception e)
            {
                SelfLog.WriteLine("Unable to lookup hostname {0}: {1}", address, e);
            }

            return null;
        }
    }
}
