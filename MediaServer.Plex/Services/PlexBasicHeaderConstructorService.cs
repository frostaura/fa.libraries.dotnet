using System.Collections.Generic;
using FrostAura.Libraries.Core.Extensions.Validation;
using FrostAura.Libraries.Http.Interfaces;
using MediaServer.Plex.Models.Requests;

namespace MediaServer.Plex.Services
{
    /// <summary>
    /// Constructor for basic Plex request headers.
    /// </summary>
    public class PlexBasicHeaderConstructorService : IHeaderConstructor<PlexBasicRequestHeaders>
    {
        /// <summary>
        /// Construct request headers for HTTP request.
        /// </summary>
        /// <param name="data">Data to be passed to construct headers.</param>
        /// <returns>Headers to be added to HTTP request.</returns>
        public IDictionary<string, string> ConstructRequestHeaders(PlexBasicRequestHeaders data)
        {
            data.ThrowIfNull(nameof(data));
            
            var headers = new Dictionary<string, string>
            {
                {"X-Plex-Platform", data.Platform.ThrowIfNullOrWhitespace(nameof(data.Platform))},
                {"X-Plex-Platform-Version", data.PlatformVersion.ThrowIfNullOrWhitespace(nameof(data.PlatformVersion))}, 
                {"X-Plex-Provides", data.Provides.ThrowIfNullOrWhitespace(nameof(data.Provides))},
                {"X-Plex-Client-Identifier", data.ClientIdentifier.ThrowIfNullOrWhitespace(nameof(data.ClientIdentifier))},
                {"X-Plex-Product", data.Product.ThrowIfNullOrWhitespace(nameof(data.Product))},
                {"X-Plex-Version", data.Version.ThrowIfNullOrWhitespace(nameof(data.Version))}
            };

            return headers;
        }
    }
}