using System;
using System.Linq;
using System.Net.Http;
using FrostAura.Libraries.Core.Exceptions.Validation;
using FrostAura.Libraries.Core.Models.Auth;
using MediaServer.Plex.Extensions;
using MediaServer.Plex.Models.Config;
using Xunit;

namespace MediaServer.Plex.Tests.Extensions
{
    public class HttpRequestMessageExtensionsTests
    {
        [Fact]
        public void WithAuthToken_WithInvalidConfig_ShouldThrowArgumentNullException()
        {
            // Setup
            var request = new HttpRequestMessage();
            PlexMediaServerConfig config = null;

            // Perform
            var exception = Assert.Throws<ArgumentNullException>(() =>
            {
                request
                    .WithAuthToken(config);
            });
        }
        
        [Fact]
        public void WithAuthToken_WithInvalidToken_ShouldThrowValidationException()
        {
            // Setup
            var request = new HttpRequestMessage();
            var config = new PlexMediaServerConfig();
            
            // Perform action 'WithAuthToken'
            Assert.Throws<ValidationException>(() =>
            {
                request
                    .WithAuthToken(config);
            });
            
            // Assert that 'ShouldThrowValidationException' = 'WithInvalidToken'
        }
        
        [Fact]
        public void WithAuthToken_WithValidConfig_ShouldAppendPlexTokenHeader()
        {
            // Setup
            var request = new HttpRequestMessage();
            var config = new PlexMediaServerConfig
            {
                PlexAuthenticatedUser = new User
                {
                    AuthToken = "Test Token"
                },
                PlexAuthenticationRequestUser = new BasicAuth
                {
                    Username = "test user",
                    Password = "Test password"
                },
                ServerAddress = "test"
            };
            
            // Perform
            request.WithAuthToken(config);

            var header = request
                .Headers
                .FirstOrDefault(h => h.Key == "X-Plex-Token");

            // Assert
            Assert.NotNull(header);
            Assert.Equal(config.PlexAuthenticatedUser.AuthToken, header.Value.FirstOrDefault());
        }
    }
}