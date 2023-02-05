using System;
using System.Threading;
using System.Threading.Tasks;
using FrostAura.Libraries.MediaServer.Core.Interfaces;
using FrostAura.Libraries.MediaServer.Core.Services;
using Xunit;

namespace FrostAura.Libraries.MediaServer.Core.Tests.Integration.Services
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