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
using MediaServer.Plex.Interfaces;
using MediaServer.Plex.Models.Config;
using MediaServer.Plex.Models.Requests;
using MediaServer.Plex.Models.Responses;

namespace MediaServer.Plex.Services
{
    /// <summary>
    /// Plex.tv authenticator service.
    /// </summary>
    public class PlexTvAuthenticator : IPlexAuthenticator
    {
        /// <summary>
        /// Media server specific configuration. Should be set in a constructor.
        /// </summary>
        public PlexMediaServerConfig _configuration { get; }
        
        /// <summary>
        /// Instance of static http service to use in making web requests.
        /// </summary>
        private IHttpService _httpService { get; }
        
        /// <summary>
        /// Constructor for Plex headers.
        /// </summary>
        private IDictionary<string, string> _plexBasicHeaders { get; }
        
        /// <summary>
        /// Constructor for basic authentication.
        /// </summary>
        private IHeaderConstructor<BasicAuthRequest> _basicAuthConstructor { get; }
        
        /// <summary>
        /// Overloaded constructor to pass configuration.
        /// </summary>
        /// <param name="httpService">Instance of static http service to use in making web requests.</param>
        /// <param name="plexBasicHeadersConstructor">Constructor for Plex headers.</param>
        /// <param name="basicAuthHeadersConstructor">Constructor for Plex headers.</param>
        /// <param name="configuration">Media server specific configuration.</param>
        public PlexTvAuthenticator(IHttpService httpService,
            IHeaderConstructor<PlexBasicRequestHeaders> plexBasicHeadersConstructor,
            IHeaderConstructor<BasicAuthRequest> basicAuthHeadersConstructor,
            PlexMediaServerConfig configuration)
        {
            _configuration = configuration
                .ThrowIfNull(nameof(configuration))
                .ThrowIfInvalid(nameof(configuration));
            _httpService = httpService
                .ThrowIfNull(nameof(httpService));
            _plexBasicHeaders = plexBasicHeadersConstructor
                .ThrowIfNull(nameof(plexBasicHeadersConstructor))
                .ConstructRequestHeaders(configuration.BasicPlexHeaders);
            _basicAuthConstructor = basicAuthHeadersConstructor
                .ThrowIfNull(nameof(basicAuthHeadersConstructor));
        }
        
        /// <summary>
        /// Authenticate a user using username and password.
        /// </summary>
        /// <param name="token">Cancellation token instance.</param>
        /// <returns>Response for authentication whether success or failure.</returns>
        public async Task<UserAuthenticationResponse> AuthenticateAsync(CancellationToken token)
        {
            string requestUrl = Endpoint.SignIn.Description();
            var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
            IDictionary<string, string> authHeaders = _basicAuthConstructor.ConstructRequestHeaders(new BasicAuthRequest
            {
                Username = _configuration.PlexAuthenticationRequestUser.Username,
                Password = _configuration.PlexAuthenticationRequestUser.Password
            });
            HttpRequest httpRequest = request
                .AddRequestHeaders(_plexBasicHeaders)
                .AddRequestHeaders(authHeaders)
                .ToHttpRequest();
            HttpResponse<UserAuthenticationResponse> response = await _httpService
                .RequestAsync<UserAuthenticationResponse>(httpRequest, token);

            return response.Response;
        }
    }
}