using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FrostAura.Libraries.Core.Extensions.Decoration;
using FrostAura.Libraries.Core.Extensions.Validation;
using FrostAura.Libraries.Core.Models.Auth;
using FrostAura.Libraries.Http.Extensions;
using FrostAura.Libraries.Http.Interfaces;
using FrostAura.Libraries.Http.Models.Requests;
using FrostAura.Libraries.Http.Models.Responses;
using FrostAura.Libraries.Http.Services;
using MediaServer.Core.Enums;
using MediaServer.Core.Interfaces;
using MediaServer.Core.Models.Content;
using MediaServer.Plex.Enums;
using MediaServer.Plex.Extensions;
using MediaServer.Plex.Interfaces;
using MediaServer.Plex.Models.Collections;
using MediaServer.Plex.Models.Config;
using MediaServer.Plex.Models.Content;
using MediaServer.Plex.Models.Requests;
using MediaServer.Plex.Models.Responses;

namespace MediaServer.Plex.Services
{
    /// <summary>
    /// Plex Media Server service.
    /// </summary>
    public class PlexMediaService : IMediaServer<PlexMediaServerConfig, Device>
    {
        /// <summary>
        /// Media server specific configuration. Should be set in a constructor.
        /// </summary>
        public PlexMediaServerConfig Configuration { get; }

        /// <summary>
        /// Instance of static http service to use in making web requests.
        /// </summary>
        private IHttpService _httpService { get; }
        
        /// <summary>
        /// Instance of authentication service to use for Plex.
        /// </summary>
        private IPlexAuthenticator _authenticator { get; }
        
        /// <summary>
        /// Instance of Plex server settings provider.
        /// </summary>
        private IPlexServerSettingsProvider _serverSettingsProvider { get; }
        
        /// <summary>
        /// Instance of Plex server media provider.
        /// </summary>
        private IPlexMediaProvider _mediaProvider { get; }

        /// <summary>
        /// Overloaded constructor to pass configuration.
        /// </summary>
        /// <param name="configuration">Media server specific configuration.</param>
        /// <param name="httpService">Instance of static http service to use in making web requests.</param>
        /// <param name="authenticator">Instance of static http service to use in making web requests.</param>
        /// <param name="serverSettingsProvider">Instance of Plex server settings provider.</param>
        /// <param name="mediaProvider">Instance of Plex server media provider.</param>
        public PlexMediaService(PlexMediaServerConfig configuration,
            IHttpService httpService,
            IPlexAuthenticator authenticator,
            IPlexServerSettingsProvider serverSettingsProvider,
            IPlexMediaProvider mediaProvider)
        {
            Configuration = configuration
                .ThrowIfNull(nameof(configuration))
                .ThrowIfInvalid(nameof(configuration));
            _httpService = httpService
                .ThrowIfNull(nameof(httpService));
            _authenticator = authenticator
                .ThrowIfNull(nameof(authenticator));
            _serverSettingsProvider = serverSettingsProvider
                .ThrowIfNull(nameof(serverSettingsProvider));
            _mediaProvider = mediaProvider
                .ThrowIfNull(nameof(mediaProvider));
        }

        /// <summary>
        /// Construct a default instance of the Plex service.
        /// </summary>
        /// <param name="username">Username used to get auth token.</param>
        /// <param name="password">Username used to get auth token.</param>
        /// <returns>Plex service instance.</returns>
        public static PlexMediaService GetDefaultInstance(string username, string password)
        {
            var plexConfig = new PlexMediaServerConfig
            {
                PlexAuthenticationRequestUser = new BasicAuth
                {
                    Username = username,
                    Password = password
                },
                BasicPlexHeaders = new PlexBasicRequestHeaders()
            };
            IHttpService httpService = new JsonHttpService(new HttpClient());
            IHeaderConstructor<PlexBasicRequestHeaders> plexBasicHeaderConstructorService = new PlexBasicHeaderConstructorService();
            IPlexAuthenticator authenticator = new PlexTvAuthenticator(
                httpService,
                plexBasicHeaderConstructorService,
                new BasicAuthHeaderConstructorService(), plexConfig);
            IPlexServerSettingsProvider settingsProvider = new PlexServerPreferencesProviderService(httpService, plexBasicHeaderConstructorService, plexConfig);
            IPlexMediaProvider mediaProvider = new PlexMediaProviderService(httpService, plexBasicHeaderConstructorService, plexConfig);
            
            return new PlexMediaService(plexConfig, httpService, authenticator, settingsProvider, mediaProvider);
        }
        
        /// <summary>
        /// Media server initialized to be called before consuming the service.
        /// The setting up of the configuration object should happen in this function.
        /// <param name="serverSelector">Delegate for selecting a server as the default upon discovery.</param>
        /// <param name="token">Cancellation token instance.</param>
        /// </summary>
        public async Task<InitializationStatus> InitializeAsync(Func<IEnumerable<Device>, string> serverSelector, CancellationToken token)
        {
            var authenticationResponseTask = _authenticator.AuthenticateAsync(token);
            var getServersResponseTask = _authenticator.GetAllServers(token);

            await Task.WhenAll(authenticationResponseTask, getServersResponseTask);
            
            if(authenticationResponseTask.Result?.User == null) return InitializationStatus.Unauthorised;
            if(getServersResponseTask.Result == null || !getServersResponseTask.Result.Any()) return InitializationStatus.NoServersDiscovered;
            
            Configuration.PlexAuthenticatedUser = authenticationResponseTask.Result.User;
            Configuration.DiscoveredServers = getServersResponseTask.Result;
            Configuration.ServerAddress = serverSelector(Configuration.DiscoveredServers);
            Configuration.ServerPreferences = await _serverSettingsProvider.GetServerSettingsAsync(token);

            if (Configuration.ServerPreferences?.Setting == null) return InitializationStatus.Error;

            return InitializationStatus.Ok;
        }

        /// <summary>
        /// Collection of libraries from the server
        /// <param name="token">Cancellation token instance.</param>
        /// </summary>
        public async Task<IEnumerable<Library>> GetAllLibrariesAsync(CancellationToken token)
        {
            var result = await _mediaProvider
                .GetAllLibrariesAsync(token);

            return result;
        }
    }
}