using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FrostAura.Libraries.Core.Models.Auth;
using FrostAura.Libraries.Http.Interfaces;
using FrostAura.Libraries.Http.Services;
using FrostAura.Libraries.MediaServer.Core.Models.Content;
using MediaServer.Plex.Interfaces;
using MediaServer.Plex.Models.Config;
using MediaServer.Plex.Models.Content;
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
            var plexService = PlexMediaService
                .GetDefaultInstance("USERNAME", "PASSWORD");
            Func<IEnumerable<Device>, string> serverSelectorDelegate = (servers) => servers
                .FirstOrDefault()?
                .LocalConnection?
                .Uri;

            // Perform action 'Initialize'
            await plexService.InitializeAsync(serverSelectorDelegate, CancellationToken.None);
            
            // Assert that 'ShouldSetMediaContainer' = 'WithValidParamsAndResponse'
            Assert.NotNull(plexService.Configuration.ServerPreferences);
            Assert.True(plexService.Configuration.ServerPreferences.Size > 0);
            Assert.True(plexService.Configuration.ServerPreferences.Setting.Any());
            
            Assert.NotNull(plexService.Configuration.DiscoveredServers);
            Assert.NotEmpty(plexService.Configuration.DiscoveredServers);
        }
        
        [Fact]
        public async Task GetAllLibraries_WithValidParamsAndResponse_ShouldSetMediaContainer()
        {
            // Setup
            var plexService = PlexMediaService
                .GetDefaultInstance("USERNAME", "PASSWORD");
            Func<IEnumerable<Device>, string> serverSelectorDelegate = (servers) => servers
                .FirstOrDefault()?
                .LocalConnection?
                .Uri;

            // Perform
            await plexService.InitializeAsync(serverSelectorDelegate,CancellationToken.None);
            
            IEnumerable<Library> libraries = await plexService.GetAllLibrariesAsync(CancellationToken.None);
            IEnumerable<Movie> movies = await libraries.First().GetMoviesAsync;
            
            // Assert
            Assert.True(libraries.Any());
            Assert.True(movies.Any());
        }
    }
}
