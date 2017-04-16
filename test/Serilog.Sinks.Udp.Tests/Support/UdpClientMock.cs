using System.Net;
using System.Threading.Tasks;
using Moq;
using Moq.Language.Flow;
using Serilog.Sinks.Udp;

namespace Serilog.Support
{
    internal class UdpClientMock : Mock<IUdpClient>
    {
        internal ISetup<IUdpClient, Task<int>> SetupSendAsync()
        {
            return Setup(
                mock => mock.SendAsync(
                    It.IsAny<byte[]>(),
                    It.IsAny<int>(),
                    It.IsAny<IPEndPoint>()));
        }

        internal void VerifySendAsync(IPAddress remoteAddress, int remotePort, Times times)
        {
            Verify(
                mock => mock.SendAsync(
                    It.IsAny<byte[]>(),
                    It.IsAny<int>(),
                    It.Is<IPEndPoint>(remoteEndpoint =>
                        remoteEndpoint != null &&
                        remoteEndpoint.AddressFamily == remoteAddress.AddressFamily &&
                        remoteEndpoint.Address.Equals(remoteAddress) &&
                        remoteEndpoint.Port == remotePort)),
                times);
        }
    }
}
