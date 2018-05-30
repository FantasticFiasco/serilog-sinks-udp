using System.Net;
using Serilog.Debugging;
using Shouldly;
using Xunit;

namespace Serilog.Sinks.Udp.Private
{
    public class StringExtensionsTest
    {
        [Fact]
        public void ShouldConvertIPAddress()
        {
            // Arrange
            var expected = IPAddress.Loopback;

            // Act
            var actual = "127.0.0.1".ToIPAddress();
            
            // Assert
            actual.ShouldBe(expected);
        }
        
        [Fact]
        public void ShouldConvertHostname()
        {
            // Arrange
            var expected = IPAddress.Loopback;

            // Act
            var actual = "localhost".ToIPAddress();
            
            // Assert
            actual.ShouldBe(expected);
        }

        [Fact]
        public void ShouldNotConvertInvalidHostname()
        {
            // Arrange
            string actualErrorMessage = null;

            SelfLog.Enable(output => { actualErrorMessage = output; });

            // Act
            var actual = "invalid-host-name".ToIPAddress();
            
            // Assert
            actual.ShouldBeNull();
            actualErrorMessage.ShouldNotBeNull();
        }
    }
}
