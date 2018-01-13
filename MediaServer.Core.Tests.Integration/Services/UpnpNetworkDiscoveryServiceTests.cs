using System;
using System.Threading;
using System.Threading.Tasks;
using MediaServer.Core.Interfaces;
using MediaServer.Core.Services;
using Xunit;

namespace MediaServer.Core.Tests.Integration.Services
{
    public class UpnpNetworkDiscoveryServiceTests
    {
        [Fact]
        public async Task GetDevicesAsync_WithNoParams_ShouldDiscoverDevices()
        {
            // Setup
            var cancellationTokenSource = new CancellationTokenSource();
            INetworkDiscoveryService instance = new UpnpNetworkDiscoveryService();

            instance.OnDiscovery += (discoveredDevice) => { Console.WriteLine($"IP Discovered: {discoveredDevice}"); };

            // Perform
            instance.BackgroundQueueDiscovery(cancellationTokenSource.Token);
            cancellationTokenSource.CancelAfter(10000);
            await Task.Delay(10000);

            // Assert
            Assert.NotEmpty(instance.DiscoveredAddresses);
        }
    }
}