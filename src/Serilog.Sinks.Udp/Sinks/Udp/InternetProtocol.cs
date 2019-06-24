namespace Serilog.Sinks.Udp
{
    /// <summary>
    /// Enum describing internet protocols supported by the sink.
    /// </summary>
    public enum InternetProtocol
    {
        /// <summary>
        /// Sink is using the IPv4 (Internet Protocol version 4) addressing scheme of the socket.
        /// </summary>
        Version4,

        /// <summary>
        /// Sink is using the IPv6 (Internet Protocol version 6) addressing scheme of the socket.
        /// </summary>
        Version6
    }
}
