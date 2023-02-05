using System;
using FrostAura.Libraries.MediaServer.Core.Services;
using Xunit;

namespace FrostAura.Libraries.MediaServer.Core.Tests.Services
{
    public class UpnpNetworkDiscoveryServiceTests
    {
        [Fact]
        public void Constructor_WithInvalidIPAddress_ShouldThrowArgumentNullException()
        {
            // Setup
            string ip = null;
            int port = 999;
            
            // Perform
            var exception = Assert.Throws<ArgumentNullException>(() => new UpnpNetworkDiscoveryService(ip, port));

            // Assert
            Assert.Equal("ipAddress", exception.ParamName);
        }
    }
}