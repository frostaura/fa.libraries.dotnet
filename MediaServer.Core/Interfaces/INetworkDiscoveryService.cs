using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FrostAura.Libraries.MediaServer.Core.Interfaces
{
    /// <summary>
    /// Service interface for device discovery on a network.
    /// </summary>
    public interface INetworkDiscoveryService
    {
        /// <summary>
        /// Event client can subscribe to which will notify them of new device discoveries one at a time.
        /// NOTE: The event should only fire once per IP address.
        /// <param>string : Discovered IP address.</param>
        /// </summary>
        event Action<string> OnDiscovery;
        
        /// <summary>
        /// Collection of all discovered IP addresses.
        /// </summary>
        IList<string> DiscoveredAddresses { get; }
        
        /// <summary>
        /// Task to start discovery. This task should be run indefinitely or for a certain timeframe.
        /// A cancellation token can be used to kill the background process.
        /// </summary>
        /// <param name="token">Cancellation token instance.</param>
        Task BackgroundQueueDiscovery(CancellationToken token);
    }
}