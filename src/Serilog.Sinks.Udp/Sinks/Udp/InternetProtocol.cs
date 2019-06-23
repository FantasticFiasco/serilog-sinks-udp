namespace Serilog.Sinks.Udp
{
    /// <summary>
    /// Enum describing internet protocols supported by the sink.
    /// </summary>
    public enum InternetProtocol
    {
        /// <summary>
        /// Sink sending network packages to address defined in IP version 4.
        /// </summary>
        Version4,

        /// <summary>
        /// Sink sending network packages to address defined in IP version 6.
        /// </summary>
        Version5
    }
}
