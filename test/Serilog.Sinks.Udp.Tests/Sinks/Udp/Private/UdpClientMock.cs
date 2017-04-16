using System.Net;
using System.Threading.Tasks;
using Moq;
using Moq.Language.Flow;

namespace Serilog.Sinks.Udp.Private
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
