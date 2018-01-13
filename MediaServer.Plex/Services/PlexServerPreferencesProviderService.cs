using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FrostAura.Libraries.Core.Extensions.Decoration;
using FrostAura.Libraries.Core.Extensions.Validation;
using FrostAura.Libraries.Http.Extensions;
using FrostAura.Libraries.Http.Interfaces;
using FrostAura.Libraries.Http.Models.Requests;
using FrostAura.Libraries.Http.Models.Responses;
using MediaServer.Plex.Enums;
using MediaServer.Plex.Extensions;
using MediaServer.Plex.Interfaces;
using MediaServer.Plex.Models.Config;
using MediaServer.Plex.Models.Requests;
using MediaServer.Plex.Models.Responses;

namespace MediaServer.Plex.Services
{
    /// <summary>
    /// Plex server settings and preferences provider service.
    /// </summary>
    public class PlexServerPreferencesProviderService : IPlexServerSettingsProvider
    {
        /// <summary>
        /// Media server specific configuration. Should be set in a constructor.
        /// </summary>
        private PlexMediaServerConfig _configuration { get; }
        
        /// <summary>
        /// Instance of static http service to use in making web requests.
        /// </summary>
        private IHttpService _httpService { get; }
        
        /// <summary>
        /// Constructor for Plex headers.
        /// </summary>
        private IDictionary<string, string> _plexBasicHeaders { get; }
        
        /// <summary>
        /// Overloaded constructor to pass configuration.
        /// </summary>
        /// <param name="httpService">Instance of static http service to use in making web requests.</param>
        /// <param name="plexBasicHeadersConstructor">Constructor for Plex headers.</param>
        /// <param name="configuration">Media server specific configuration.</param>
        public PlexServerPreferencesProviderService(IHttpService httpService, IHeaderConstructor<PlexBasicRequestHeaders> plexBasicHeadersConstructor, PlexMediaServerConfig configuration)
        {
            _configuration = configuration
                .ThrowIfNull(nameof(configuration))
                .ThrowIfInvalid(nameof(configuration));
            
            _httpService = httpService
                .ThrowIfNull(nameof(httpService));
            _plexBasicHeaders = plexBasicHeadersConstructor
                .ThrowIfNull(nameof(plexBasicHeadersConstructor))
                .ConstructRequestHeaders(configuration.BasicPlexHeaders);
        }
        
        /// <summary>
        /// Fetch server settings and preferences async.
        /// </summary>
        /// <param name="token">Cancellation token.</param>
        /// <returns>Server preferences and settings.</returns>
        public async Task<ServerPreferences> GetServerSettingsAsync(CancellationToken token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, Endpoint.ServerPreferences.Description(_configuration.ServerAddress));
            HttpRequest httpRequest = request
                .WithAuthToken(_configuration)
                .AddRequestHeaders(_plexBasicHeaders)
                .AcceptJson()
                .ToHttpRequest();
            HttpResponse<BasePlexResponse<ServerPreferences>> response = await _httpService
                .RequestAsync<BasePlexResponse<ServerPreferences>>(httpRequest, token);

            return response.Response?.MediaContainer;
        }
    }
}