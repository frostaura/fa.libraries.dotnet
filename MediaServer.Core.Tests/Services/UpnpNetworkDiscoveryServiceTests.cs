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
        
        #region INTEGRATION TESTS

        private const bool PERFORM_INTEGRATION_TESTS = false;
        
        [Fact]
        public async Task GetDevicesAsync_WithNoParams_ShouldDiscoverDevices()
        {
            if (!PERFORM_INTEGRATION_TESTS) return;
            
            // Setup
            var cancellationTokenSource = new CancellationTokenSource();
            INetworkDiscoveryService instance = new UpnpNetworkDiscoveryService();

            instance.OnDiscovery += (discoveredIp) => { Console.WriteLine($"IP Discovered: {discoveredIp}"); };

            // Perform
            instance.BackgroundQueueDiscovery(cancellationTokenSource.Token);
            cancellationTokenSource.CancelAfter(10000);
            await Task.Delay(10000);

            // Assert
            Assert.NotEmpty(instance.DiscoveredAddresses);
        }

        #endregion
    }
}