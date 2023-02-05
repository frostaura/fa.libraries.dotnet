using System;
using System.Collections.Generic;
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
using MediaServer.Plex.Models.Collections;
using MediaServer.Plex.Models.Config;
using MediaServer.Plex.Models.Requests;
using MediaServer.Plex.Models.Responses;
using MediaServer.Plex.Services;
using Newtonsoft.Json;
using NSubstitute;
using Xunit;

namespace MediaServer.Plex.Tests.Services
{
    public class PlexTvAuthenticatorTests
    {
        [Fact]
        public void Constructor_WithInvalidConfig_ShouldThrowArgumentNullException()
        {
            // Setup
            PlexMediaServerConfig config = null;
            var httpService = Substitute.For<IHttpService>();
            var plexBasicHeadersConstructor = Substitute.For<IHeaderConstructor<PlexBasicRequestHeaders>>();
            var basicAuthHeadersConstructor = Substitute.For<IHeaderConstructor<BasicAuthRequest>>();
            
            // Perform action 'Constructor'
            var exception = Assert.Throws<ArgumentNullException>(() =>
            {
                var instance = new PlexTvAuthenticator(httpService, plexBasicHeadersConstructor, basicAuthHeadersConstructor, config);
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
            var basicAuthHeadersConstructor = Substitute.For<IHeaderConstructor<BasicAuthRequest>>();
            
            // Perform action 'Constructor'
            var exception = Assert.Throws<ArgumentNullException>(() =>
            {
                var instance = new PlexTvAuthenticator(httpService, plexBasicHeadersConstructor, basicAuthHeadersConstructor, config);
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
            var basicAuthHeadersConstructor = Substitute.For<IHeaderConstructor<BasicAuthRequest>>();
            
            // Perform action 'Constructor'
            var exception = Assert.Throws<ArgumentNullException>(() =>
            {
                var instance = new PlexTvAuthenticator(httpService, plexBasicHeadersConstructor, basicAuthHeadersConstructor, config);
            });

            // Assert that 'ShouldThrowArgumentNullException' = 'WithInvalidConfig'
            Assert.Equal("plexBasicHeadersConstructor", exception.ParamName);
        }
        
        [Fact]
        public void Constructor_WithInvalidAuthHeaderConstructor_ShouldThrowArgumentNullException()
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
            IHeaderConstructor<BasicAuthRequest> basicAuthHeadersConstructor = null;
            
            // Perform action 'Constructor'
            var exception = Assert.Throws<ArgumentNullException>(() =>
            {
                var instance = new PlexTvAuthenticator(httpService, plexBasicHeadersConstructor, basicAuthHeadersConstructor, config);
            });

            // Assert that 'ShouldThrowArgumentNullException' = 'WithInvalidConfig'
            Assert.Equal("basicAuthHeadersConstructor", exception.ParamName);
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
            var basicAuthHeadersConstructor = Substitute.For<IHeaderConstructor<BasicAuthRequest>>();
            
            // Perform action 'Constructor'
            var instance = new PlexTvAuthenticator(httpService, plexBasicHeadersConstructor, basicAuthHeadersConstructor, config);

            // Assert that 'ShouldThrowArgumentNullException' = 'WithInvalidConfig'
            Assert.NotNull(instance);
        }
        
        [Fact]
        public async Task AuthenticateAsync_WithInvalidHttpResponse_ShouldReturnDefaultResponseWithNullUser()
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
            var basicAuthHeadersConstructor = Substitute.For<IHeaderConstructor<BasicAuthRequest>>();
            var instance = new PlexTvAuthenticator(httpService, plexBasicHeadersConstructor, basicAuthHeadersConstructor, config);
            var username = "test username";
            var password = "test password";
            string endpointUrl = Endpoint.SignIn.Description();
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, endpointUrl);
            HttpRequest httpRequest = httpRequestMessage.ToHttpRequest();
            var expectedHttpResponse = new HttpResponse<UserAuthenticationResponse>();
            var expectedHttpResponseMessage = new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(new object()))
            };
            
            // Mocks
            httpService
                .RequestAsync<UserAuthenticationResponse>(Arg.Any<HttpRequest>(), Arg.Any<CancellationToken>())
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
            UserAuthenticationResponse actual = await instance.AuthenticateAsync(CancellationToken.None);

            // Assert
            httpService
                .Received()
                .RequestAsync<UserAuthenticationResponse>(Arg.Any<HttpRequest>(), CancellationToken.None);
            
            Assert.Null(actual.User);
        }
        
        [Fact]
        public async Task AuthenticateAsync_WithValidParams_ShouldCallRequestAsyncWithValidRequest()
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
            var basicAuthHeadersConstructor = Substitute.For<IHeaderConstructor<BasicAuthRequest>>();
            plexBasicHeadersConstructor
                .ConstructRequestHeaders(Arg.Any<PlexBasicRequestHeaders>())
                .Returns((request) => new Dictionary<string, string>
                {
                    { "X-Plex-Client-Identifier", "test header" }
                });
            basicAuthHeadersConstructor
                .ConstructRequestHeaders(Arg.Any<BasicAuthRequest>())
                .Returns((request) => new Dictionary<string, string>
                {
                    { "Authorization", "test header" }
                });
            var instance = new PlexTvAuthenticator(httpService, plexBasicHeadersConstructor, basicAuthHeadersConstructor, config);
            var username = "test username";
            var password = "test password";
            string endpointUrl = Endpoint.SignIn.Description();
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, endpointUrl);
            HttpRequest httpRequest = httpRequestMessage.ToHttpRequest();
            var expectedUser = new User
            {
                AuthToken = "test token",
                Email = "test email"
            };
            var expectedResponseBody = new UserAuthenticationResponse
            {
                User = expectedUser
            };
            var expectedHttpResponse = new HttpResponse<UserAuthenticationResponse>();
            var expectedHttpResponseMessage = new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedResponseBody))
            };
            
            // Mocks
            httpService
                .RequestAsync<UserAuthenticationResponse>(Arg.Any<HttpRequest>(), Arg.Any<CancellationToken>())
                .Returns((request) =>
                {
                    // Assert the details of the request to be correct
                    var requestContext = request.Args()[0] as HttpRequest;
                    
                    Assert.Equal(httpRequestMessage.RequestUri.AbsoluteUri, requestContext?.Request.RequestUri.AbsoluteUri);
                    Assert.Equal(httpRequestMessage.Method, requestContext?.Request.Method);
                    Assert.True(requestContext?.Request.Headers.Contains("X-Plex-Client-Identifier"));
                    Assert.True(requestContext?.Request.Headers.Contains("Authorization"));
                    
                    return expectedHttpResponse;
                });
            
            await expectedHttpResponse.SetResponseAsync(httpRequest.Identifier, expectedHttpResponseMessage, CancellationToken.None);
            
            // Perform
            UserAuthenticationResponse actual = await instance.AuthenticateAsync(CancellationToken.None);

            // Assert
            httpService
                .Received()
                .RequestAsync<UserAuthenticationResponse>(Arg.Any<HttpRequest>(), CancellationToken.None);
            
            Assert.Equal(expectedUser.AuthToken, actual.User.AuthToken);
            Assert.Equal(expectedUser.Email, actual.User.Email);
        }
    }
}