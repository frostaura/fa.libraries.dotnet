using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FrostAura.Libraries.Core.Extensions.Validation;
using FrostAura.Libraries.MediaServer.Core.Interfaces;

namespace FrostAura.Libraries.MediaServer.Core.Services
{
    /// <summary>
    /// Service to discover Upnp devices on a network.
    /// </summary>
    public class UpnpNetworkDiscoveryService : INetworkDiscoveryService
    {
        /// <summary>
        /// Event call for when a new device has been discovered on the network.
        /// </summary>
        public event Action<string> OnDiscovery; 
        
        /// <summary>
        /// Collection of discovered addresses in order to not fire 
        /// </summary>
        public IList<string> DiscoveredAddresses { get; } = new List<string>();
        
        /// <summary>
        /// Multicast IP address for the discovery.
        /// </summary>
        private string _ipAddress { get; }
        
        /// <summary>
        /// Multicast port address for the discovery.
        /// </summary>
        private int _port { get; }

        /// <summary>
        /// Generic timeout for operations.
        /// </summary>
        private int _timeout { get; }

        /// <summary>
        /// Initialize the discovery service with default parameters.
        /// </summary>
        /// <param name="ipAddress">Multicast IP address for the discovery.</param>
        /// <param name="port">Multicast port address for the discovery.</param>
        /// <param name="timeout">Generic timeout for operations.</param>
        public UpnpNetworkDiscoveryService(string ipAddress = "239.0.0.250", int port = 32414, int timeout = 10000)
        {
            _ipAddress = ipAddress.ThrowIfNullOrWhitespace(nameof(ipAddress));
            _port = port;
            _timeout = timeout;
        }

        /// <summary>
        /// Task to start network discovery. This task should be run indefinitely or for a certain timeframe.
        /// A cancellation token can be used to kill the background process.
        /// </summary>
        /// <param name="token">Cancellation token instance.</param>
        public async Task BackgroundQueueDiscovery(CancellationToken token)
        {
            SendDiscoveryMessage();
            
            UdpClient client = await CreateUdpClientAsync(token);

            // Run an infinite retry loop for discovery.
            while (true)
            {
                // With each try, check cancellation token and throw cancellation exception if cancelled.
                token.ThrowIfCancellationRequested();
                
                do
                {
                    try
                    {
                        UdpReceiveResult result = await client.ReceiveAsync();
                        string discoveredAddress = result.RemoteEndPoint.Address.ToString();

                        // Notify all subscribers that a new device has been found on the network.
                        if (!DiscoveredAddresses.Any(da => da.Equals(discoveredAddress)))
                        {
                            DiscoveredAddresses.Add(discoveredAddress);
                            OnDiscovery?.Invoke(discoveredAddress);
                        }
                    }
                    catch (Exception ex)
                    { }
                } while (client.Client != null);

                try
                {
                    client.Close();
                }
                catch (Exception ex)
                {
                }

                await CreateUdpClientAsync(token);
            }
        }
        
        /// <summary>
        /// Initialize UDP client creation.
        /// </summary>
        /// <param name="token">Cancellation token instance.</param>
        /// <returns>Created UDP client</returns>
        private async Task<UdpClient> CreateUdpClientAsync(CancellationToken token)
        {
            // Run an infinite retry loop for establishing a UDP connection.
            while (true)
            {
                // With each try, check cancellation token and throw cancellation exception if cancelled.
                token.ThrowIfCancellationRequested();

                try
                {
                    return new UdpClient(_port)
                    {
                        Client =
                        {
                            ReceiveTimeout = _timeout,
                            SendTimeout = _timeout
                        }
                    };
                }
                catch (SocketException ex)
                { }

                // If the UDP client creation fails, timeout and retry.
                await Task.Delay(10000, token);
            }
        }

        /// <summary>
        /// Broadcast a message for devices to send discovery messages.
        /// </summary>
        private void SendDiscoveryMessage()
        {
            try
            {
                using (UdpClient udpClient = new UdpClient())
                {
                    IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(_ipAddress), _port);
                    byte[] bytes = Encoding.ASCII.GetBytes("M-SEARCH * HTTP/1.0");
                    udpClient.Send(bytes, bytes.Length, endPoint);
                    udpClient.Close();
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}