using System;
using System.Linq;
using System.Net.Http;
using FrostAura.Libraries.Core.Exceptions.Validation;
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

            // Perform action 'WithAuthToken'
            Assert.Throws<ArgumentNullException>(() =>
            {
                request
                    .WithAuthToken(config);
            });

            // Assert that 'ShouldThrowArgumentNullException' = 'WithInvalidConfig'
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
                PlexToken = "Test Token"
            };
            
            // Perform action 'WithAuthToken'
            request.WithAuthToken(config);

            var header = request
                .Headers
                .FirstOrDefault(h => h.Key == "X-Plex-Token");

            // Assert that 'ShouldAppendPlexTokenHeader' = 'WithValidConfig'
            Assert.NotNull(header);
            Assert.Equal(config.PlexToken, header.Value.FirstOrDefault());
        }
        
        [Fact]
        public void AcceptJson_WithNoParams_ShouldAppendAcceptJsonHeader()
        {
            // Setup
            var request = new HttpRequestMessage();
            
            // Perform action 'AcceptJson'
            request.AcceptJson();

            var header = request.Headers.FirstOrDefault(h => h.Key == "Accept");
            
            // Assert that 'ShouldAppendAcceptJsonHeader' = 'WithNoParams'
            Assert.NotNull(header);
            Assert.Equal("application/json", header.Value.FirstOrDefault());
        }
    }
}