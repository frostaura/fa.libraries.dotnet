using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediaServer.Core.Interfaces;
using MediaServer.Core.Services;
using Xunit;

namespace MediaServer.Core.Tests.Services
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