using System.Net.Http;
using FrostAura.Libraries.Core.Extensions.Validation;
using MediaServer.Plex.Models.Config;

namespace MediaServer.Plex.Extensions
{
    /// <summary>
    /// Plex specific extensions for HttpRequestMessage.
    /// </summary>
    public static class HttpRequestMessageExtensions
    {
        /// <summary>
        /// Add a Plex authentication token to the request message headers.
        /// </summary>
        /// <param name="request">Original request message.</param>
        /// <param name="config">Plex server configuration.</param>
        /// <returns>The ammended request message.</returns>
        public static HttpRequestMessage WithAuthToken(this HttpRequestMessage request, PlexMediaServerConfig config)
        {
            config.ThrowIfInvalid(nameof(config));
            
            request.Headers.Add("X-Plex-Token", config.PlexToken);

            return request;
        }
        
        /// <summary>
        /// Add an accept json header to the request message headers.
        /// </summary>
        /// <param name="request">Original request message.</param>
        /// <returns>The ammended request message.</returns>
        public static HttpRequestMessage AcceptJson(this HttpRequestMessage request)
        {
            request.Headers.Add("Accept", "application/json");

            return request;
        }
    }
}