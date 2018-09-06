using System.Threading.Tasks;
using Moq;
using Moq.Language.Flow;
using Serilog.Sinks.Udp.Private;

namespace Serilog.Support
{
    internal class UdpClientMock : Mock<IUdpClient>
    {
        internal ISetup<IUdpClient, Task<int>> SetupSendAsync(
            string remoteHost,
            int remotePort)
        {
            return Setup(
                mock => mock.SendAsync(
                    It.IsAny<byte[]>(),
                    It.IsAny<int>(),
                    It.Is<string>(x => x == remoteHost),
                    It.Is<int>(x => x == remotePort)));
        }

        internal void VerifySendAsync(
            string remoteHost,
            int remotePort,
            Times times)
        {
            Verify(
                mock => mock.SendAsync(
                    It.IsAny<byte[]>(),
                    It.IsAny<int>(),
                    It.Is<string>(x => x == remoteHost),
                    It.Is<int>(x => x == remotePort)),
                times);
        }
    }
}
