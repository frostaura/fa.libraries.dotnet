using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FrostAura.Libraries.Core.Extensions.Decoration;
using FrostAura.Libraries.Core.Models.Auth;
using FrostAura.Libraries.Http.Extensions;
using FrostAura.Libraries.Http.Interfaces;
using FrostAura.Libraries.Http.Models.Requests;
using FrostAura.Libraries.Http.Models.Responses;
using MediaServer.Plex.Enums;
using MediaServer.Plex.Models.Config;
using MediaServer.Plex.Models.Requests;
using MediaServer.Plex.Models.Responses;
using MediaServer.Plex.Services;
using Newtonsoft.Json;
using NSubstitute;
using Xunit;

namespace MediaServer.Plex.Tests.Services
{
    public class PlexServerPreferencesProviderServiceTests
    {
        [Fact]
        public void Constructor_WithInvalidConfig_ShouldThrowArgumentNullException()
        {
            // Setup
            PlexMediaServerConfig config = null;
            var httpService = Substitute.For<IHttpService>();
            var plexBasicHeadersConstructor = Substitute.For<IHeaderConstructor<PlexBasicRequestHeaders>>();
            
            // Perform action 'Constructor'
            var exception = Assert.Throws<ArgumentNullException>(() =>
            {
                var instance = new PlexServerPreferencesProviderService(httpService, plexBasicHeadersConstructor, config);
            });

            // Assert that 'ShouldThrowArgumentNullException' = 'WithInvalidConfig'
            Assert.Equal("configuration", exception.ParamName);
        }
        
        [Fact]
        public void Constructor_WithInvalidHttpService_ShouldThrowArgumentNullException()
        {
            // Setup
            var config = new PlexMediaServerConfig
            {
                PlexAuthenticationRequestUser = new BasicAuth
                {
                    Username = "test user",
                    Password = "Test password"
                },
                ServerAddress = "http://192.168.0.5:32400"
            };
            IHttpService httpService = null;
            var plexBasicHeadersConstructor = Substitute.For<IHeaderConstructor<PlexBasicRequestHeaders>>();
            
            // Perform action 'Constructor'
            var exception = Assert.Throws<ArgumentNullException>(() =>
            {
                var instance = new PlexServerPreferencesProviderService(httpService, plexBasicHeadersConstructor, config);
            });

            // Assert that 'ShouldThrowArgumentNullException' = 'WithInvalidConfig'
            Assert.Equal("httpService", exception.ParamName);
        }
        
        [Fact]
        public void Constructor_WithInvalidBasicPlexHeaderConstructor_ShouldThrowArgumentNullException()
        {
            // Setup
            var config = new PlexMediaServerConfig
            {
                PlexAuthenticationRequestUser = new BasicAuth
                {
                    Username = "test user",
                    Password = "Test password"
                },
                ServerAddress = "http://192.168.0.5:32400"
            };
            var httpService = Substitute.For<IHttpService>();
            IHeaderConstructor<PlexBasicRequestHeaders> plexBasicHeadersConstructor = null;
            
            // Perform action 'Constructor'
            var exception = Assert.Throws<ArgumentNullException>(() =>
            {
                var instance = new PlexServerPreferencesProviderService(httpService, plexBasicHeadersConstructor, config);
            });

            // Assert that 'ShouldThrowArgumentNullException' = 'WithInvalidConfig'
            Assert.Equal("plexBasicHeadersConstructor", exception.ParamName);
        }
        
        [Fact]
        public void Constructor_WithValidParams_ShouldConstruct()
        {
            // Setup
            var config = new PlexMediaServerConfig
            {
                PlexAuthenticationRequestUser = new BasicAuth
                {
                    Username = "test user",
                    Password = "Test password"
                },
                ServerAddress = "http://192.168.0.5:32400"
            };
            var httpService = Substitute.For<IHttpService>();
            var plexBasicHeadersConstructor = Substitute.For<IHeaderConstructor<PlexBasicRequestHeaders>>();
            
            // Perform action 'Constructor'
            var instance = new PlexServerPreferencesProviderService(httpService, plexBasicHeadersConstructor, config);

            // Assert that 'ShouldThrowArgumentNullException' = 'WithInvalidConfig'
            Assert.NotNull(instance);
        }
        
        [Fact]
        public async Task GetServerSettingsAsync_WithInvalidHttpResponse_ShouldReturnNullResponse()
        {
            // Setup
            var config = new PlexMediaServerConfig
            {
                PlexAuthenticationRequestUser = new BasicAuth
                {
                    Username = "test user",
                    Password = "Test password"
                },
                ServerAddress = "http://192.168.0.5:32400",
                PlexAuthenticatedUser = new User
                {
                    AuthToken = "Test token"
                }
            };
            var httpService = Substitute.For<IHttpService>();
            var plexBasicHeadersConstructor = Substitute.For<IHeaderConstructor<PlexBasicRequestHeaders>>();
            var instance = new PlexServerPreferencesProviderService(httpService, plexBasicHeadersConstructor, config);
            string endpointUrl = Endpoint.ServerPreferences.Description(config.ServerAddress);
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, endpointUrl);
            HttpRequest httpRequest = httpRequestMessage.ToHttpRequest();
            var expectedHttpResponse = new HttpResponse<BasePlexResponse<ServerPreferences>>();
            var expectedHttpResponseMessage = new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(new object()))
            };
            
            // Mocks
            httpService
                .RequestAsync<BasePlexResponse<ServerPreferences>>(Arg.Any<HttpRequest>(), Arg.Any<CancellationToken>())
                .Returns((request) =>
                {
                    // Assert the details of the request to be correct
                    var requestContext = request.Args()[0] as HttpRequest;
                    
                    Assert.Equal(httpRequestMessage.RequestUri.AbsoluteUri, requestContext?.Request.RequestUri.AbsoluteUri);
                    Assert.Equal(httpRequestMessage.Method, requestContext?.Request.Method);
                    
                    return expectedHttpResponse;
                });
            
            await expectedHttpResponse.SetResponseAsync(httpRequest.Identifier, expectedHttpResponseMessage, CancellationToken.None);
            
            // Perform
            ServerPreferences actual = await instance.GetServerSettingsAsync(CancellationToken.None);

            // Assert
            httpService
                .Received()
                .RequestAsync<BasePlexResponse<ServerPreferences>>(Arg.Any<HttpRequest>(), CancellationToken.None);
            
            Assert.Null(actual);
        }
        
        [Fact]
        public async Task GetServerSettingsAsync_WithValidParams_ShouldCallRequestAsyncWithValidRequest()
        {
            // Setup
            var config = new PlexMediaServerConfig
            {
                PlexAuthenticationRequestUser = new BasicAuth
                {
                    Username = "test user",
                    Password = "Test password"
                },
                ServerAddress = "http://192.168.0.5:32400",
                PlexAuthenticatedUser = new User
                {
                    AuthToken = "Test token"
                }
            };
            var httpService = Substitute.For<IHttpService>();
            var plexBasicHeadersConstructor = Substitute.For<IHeaderConstructor<PlexBasicRequestHeaders>>();
            plexBasicHeadersConstructor
                .ConstructRequestHeaders(Arg.Any<PlexBasicRequestHeaders>())
                .Returns((request) => new Dictionary<string, string>
                {
                    { "X-Plex-Client-Identifier", "test header" }
                });
            var instance = new PlexServerPreferencesProviderService(httpService, plexBasicHeadersConstructor, config);
            var authToken = "test auth token";
            string endpointUrl = Endpoint.ServerPreferences.Description(config.ServerAddress);
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, endpointUrl);
            HttpRequest httpRequest = httpRequestMessage.ToHttpRequest();
            var expectedSettings = new BasePlexResponse<ServerPreferences>
            {
                MediaContainer = new ServerPreferences
                {
                    Size = 1,
                    Setting = new List<Setting>
                    {
                        new Setting()
                    }
                }
            };
            var expectedHttpResponse = new HttpResponse<BasePlexResponse<ServerPreferences>>();
            var expectedHttpResponseMessage = new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedSettings))
            };
            
            // Mocks
            httpService
                .RequestAsync<BasePlexResponse<ServerPreferences>>(Arg.Any<HttpRequest>(), Arg.Any<CancellationToken>())
                .Returns((request) =>
                {
                    // Assert the details of the request to be correct
                    var requestContext = request.Args()[0] as HttpRequest;
                    
                    Assert.Equal(httpRequestMessage.RequestUri.AbsoluteUri, requestContext?.Request.RequestUri.AbsoluteUri);
                    Assert.Equal(httpRequestMessage.Method, requestContext?.Request.Method);
                    Assert.True(requestContext?.Request.Headers.Contains("X-Plex-Client-Identifier"));
                    Assert.True(requestContext?.Request.Headers.Contains("X-Plex-Token"));
                    
                    return expectedHttpResponse;
                });
            
            await expectedHttpResponse.SetResponseAsync(httpRequest.Identifier, expectedHttpResponseMessage, CancellationToken.None);
            
            // Perform
            ServerPreferences actual = await instance.GetServerSettingsAsync(CancellationToken.None);

            // Assert
            httpService
                .Received()
                .RequestAsync<BasePlexResponse<ServerPreferences>>(Arg.Any<HttpRequest>(), CancellationToken.None);
            
            Assert.Equal(actual.Size, expectedSettings.MediaContainer.Size);
            Assert.Equal(actual.Setting.Count(), expectedSettings.MediaContainer.Setting.Count());
        }
    }
}