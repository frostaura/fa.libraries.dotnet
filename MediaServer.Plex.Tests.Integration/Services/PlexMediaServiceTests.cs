using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FrostAura.Libraries.Core.Models.Auth;
using FrostAura.Libraries.Http.Interfaces;
using FrostAura.Libraries.Http.Services;
using MediaServer.Core.Models.Content;
using MediaServer.Plex.Interfaces;
using MediaServer.Plex.Models.Config;
using MediaServer.Plex.Models.Requests;
using MediaServer.Plex.Services;
using Xunit;

namespace MediaServer.Plex.Tests.Integration.Services
{
    public class PlexMediaServiceTests
    {
        [Fact]
        public async Task Initialize_WithValidParamsAndResponse_ShouldSetMediaContainer()
        {
            // Setup
            var plexConfig = new PlexMediaServerConfig
            {
                PlexAuthenticationRequestUser = new BasicAuth
                {
                    Username = "deanmar@outlook.com",
                    Password = "Reid1868*"
                },
                ServerAddress = "http://192.168.0.5:32400",
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
            var plexService = new PlexMediaService(plexConfig, httpService, authenticator, settingsProvider, mediaProvider);

            // Perform action 'Initialize'
            await plexService.InitializeAsync(CancellationToken.None);
            
            // Assert that 'ShouldSetMediaContainer' = 'WithValidParamsAndResponse'
            Assert.NotNull(plexService.Configuration.ServerPreferences);
            Assert.True(plexService.Configuration.ServerPreferences.Size > 0);
            Assert.True(plexService.Configuration.ServerPreferences.Setting.Any());
            
            (httpService as IDisposable).Dispose();
        }
        
        [Fact]
        public async Task GetAllLibraries_WithValidParamsAndResponse_ShouldSetMediaContainer()
        {
            // Setup
            var plexConfig = new PlexMediaServerConfig
            {
                PlexAuthenticationRequestUser = new BasicAuth
                {
                    Username = "deanmar@outlook.com",
                    Password = "Reid1868*"
                },
                ServerAddress = "http://192.168.0.5:32400",
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
            var plexService = new PlexMediaService(plexConfig, httpService, authenticator, settingsProvider, mediaProvider);

            // Perform action 'Initialize'
            await plexService.InitializeAsync(CancellationToken.None);
            IEnumerable<Library> libraries = await plexService.GetAllLibrariesAsync(CancellationToken.None);
            IEnumerable<Movie> movies = await libraries.First().GetMoviesAsync;
            
            // Assert that 'ShouldSetMediaContainer' = 'WithValidParamsAndResponse'
            Assert.True(libraries.Any());
            Assert.True(movies.Any());
            
            (httpService as IDisposable).Dispose();
        }
    }
}