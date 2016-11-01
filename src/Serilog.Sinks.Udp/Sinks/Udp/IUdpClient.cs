using System.Net;
using System.Threading.Tasks;

namespace Serilog.Sinks.Udp
{
    /// <summary>
    /// Interface responsible for sending UDP messages.
    /// </summary>
    public interface IUdpClient
    {
        /// <summary>
        /// Sends a UDP datagram asynchronously to a remote host.
        /// </summary>
        /// <param name="datagram">The datagram.
        /// An array of type <see cref="byte"/> that specifies the UDP datagram that you intend to
        /// send represented as an array of bytes.
        /// </param>
        /// <param name="bytes">The number of bytes in the datagram.</param>
        /// <param name="endPoint">
        /// An <see cref="IPEndPoint"/> that represents the host and port to which to send the
        /// datagram.
        /// </param>
        /// <returns>Returns <see cref="Task{TResult}"/>.</returns>
        Task<int> SendAsync(byte[] datagram, int bytes, IPEndPoint endPoint);

#if NET4
        /// <summary>
        /// Closes the UDP connection.
        /// </summary>
        void Close();
#else
        /// <summary>
        /// Releases the managed and unmanaged resources used by the <see cref="IUdpClient"/>.
        /// </summary>
        void Dispose();
#endif
    }
}
