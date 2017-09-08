using System;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Moq;
using Moq.Language.Flow;

namespace Serilog.Sinks.Udp.Private
{
    internal class UdpClientMock : Mock<IUdpClient>
    {
        internal ISetup<IUdpClient, Task<int>> SetupSendAsync(
            IPAddress remoteAddress,
            int remotePort)
        {
            return Setup(
                mock => mock.SendAsync(
                    It.IsAny<byte[]>(),
                    It.IsAny<int>(),
                    It.Is(RemoteEndPointCriteria(remoteAddress, remotePort))));
        }

        internal void VerifySendAsync(
            IPAddress remoteAddress,
            int remotePort,
            Times times)
        {
            Verify(
                mock => mock.SendAsync(
                    It.IsAny<byte[]>(),
                    It.IsAny<int>(),
                    It.Is(RemoteEndPointCriteria(remoteAddress, remotePort))),
                times);
        }

        private static Expression<Func<IPEndPoint, bool>> RemoteEndPointCriteria(
            IPAddress remoteAddress,
            int remotePort)
        {
            return remoteEndpoint =>
                remoteEndpoint.Address.Equals(remoteAddress) &&
                remoteEndpoint.Port == remotePort &&
                remoteEndpoint.AddressFamily == remoteAddress.AddressFamily;
        }
    }
}
