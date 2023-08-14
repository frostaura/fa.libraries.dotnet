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
using FrostAura.Libraries.MediaServer.Core.Enums;
using FrostAura.Libraries.MediaServer.Core.Models.Content;
using MediaServer.Plex.Enums;
using MediaServer.Plex.Models.Collections;
using MediaServer.Plex.Models.Config;
using MediaServer.Plex.Models.Content;
using MediaServer.Plex.Models.Requests;
using MediaServer.Plex.Models.Responses;
using MediaServer.Plex.Services;
using Newtonsoft.Json;
using NSubstitute;
using Xunit;
using Directory = MediaServer.Plex.Models.Content.Directory;

namespace MediaServer.Plex.Tests.Services
{
    public class PlexMediaProviderServiceTests
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
                var instance = new PlexMediaProviderService(httpService, plexBasicHeadersConstructor, config);
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
                ServerAddress = "http://192.168.0.5:32400",
                PlexAuthenticationRequestUser = new BasicAuth
                {
                    Username = "test user",
                    Password = "Test password"
                },
            };
            IHttpService httpService = null;
            var plexBasicHeadersConstructor = Substitute.For<IHeaderConstructor<PlexBasicRequestHeaders>>();
            
            // Perform action 'Constructor'
            var exception = Assert.Throws<ArgumentNullException>(() =>
            {
                var instance = new PlexMediaProviderService(httpService, plexBasicHeadersConstructor, config);
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
                ServerAddress = "http://192.168.0.5:32400",
                PlexAuthenticationRequestUser = new BasicAuth
                {
                    Username = "test user",
                    Password = "Test password"
                }
            };
            var httpService = Substitute.For<IHttpService>();
            IHeaderConstructor<PlexBasicRequestHeaders> plexBasicHeadersConstructor = null;
            
            // Perform action 'Constructor'
            var exception = Assert.Throws<ArgumentNullException>(() =>
            {
                var instance = new PlexMediaProviderService(httpService, plexBasicHeadersConstructor, config);
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
                ServerAddress = "http://192.168.0.5:32400",
                PlexAuthenticationRequestUser = new BasicAuth
                {
                    Username = "test user",
                    Password = "Test password"
                },
            };
            var httpService = Substitute.For<IHttpService>();
            var plexBasicHeadersConstructor = Substitute.For<IHeaderConstructor<PlexBasicRequestHeaders>>();
            
            // Perform action 'Constructor'
            var instance = new PlexMediaProviderService(httpService, plexBasicHeadersConstructor, config);

            // Assert that 'ShouldThrowArgumentNullException' = 'WithInvalidConfig'
            Assert.NotNull(instance);
        }
        
        [Fact]
        public async Task GetAllLibrariesAsync_WithValidParamsAndResponse_ShouldCallCorrectHttpEndpoint()
        {
            // Setup
            var plexConfig = new PlexMediaServerConfig
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
                ServerAddress = "http://192.168.0.5:32400"
            };
            
            var httpService = Substitute.For<IHttpService>();
            var plexBasicHeadersConstructor = Substitute.For<IHeaderConstructor<PlexBasicRequestHeaders>>();
            var instance = new PlexMediaProviderService(httpService, plexBasicHeadersConstructor, plexConfig);
            string endpointUrl = Endpoint.Libraries.Description(plexConfig.ServerAddress);
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, endpointUrl);
            HttpRequest httpRequest = httpRequestMessage.ToHttpRequest();
            
            var expectedMediaContainer = new BasePlexResponse<Libraries>
            {
                MediaContainer = new Libraries
                {
                    Size = 5,
                    Directory = new List<Directory>()
                }
            };
            var expectedHttpResponse = new HttpResponse<BasePlexResponse<Libraries>>();
            var expectedHttpResponseMessage = new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedMediaContainer))
            };
            
            await expectedHttpResponse.SetResponseAsync(httpRequest.Identifier, expectedHttpResponseMessage, CancellationToken.None);

            httpService
                .RequestAsync<BasePlexResponse<Libraries>>(Arg.Any<HttpRequest>(), Arg.Any<CancellationToken>())
                .Returns((request) =>
                {
                    // Assert the details of the request to be correct
                    var requestContext = request.Args()[0] as HttpRequest;
                    
                    Assert.Equal(httpRequestMessage.RequestUri.AbsoluteUri, requestContext?.Request.RequestUri.AbsoluteUri);
                    Assert.Equal(httpRequestMessage.Method, requestContext?.Request.Method);
                    
                    return expectedHttpResponse;
                });
            
            // Perform action 'WithValidParamsAndResponse'
            await instance.GetAllLibrariesAsync(CancellationToken.None);
            
            // Assert that 'GetAllLibrariesAsync' = 'ShouldCallCorrectHttpEndpoint'
            httpService
                .Received()
                .RequestAsync<BasePlexResponse<Libraries>>(Arg.Any<HttpRequest>(), CancellationToken.None);
            Assert.Equal(httpRequest.Identifier, expectedHttpResponse.RequestIdentifier);
        }
        
        [Fact]
        public async Task GetAllLibrariesAsync_WithValidResponseSections_ShouldReturnCastedLibraries()
        {
            // Setup
            var plexConfig = new PlexMediaServerConfig
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
                ServerAddress = "http://192.168.0.5:32400",
                
            };
            
            var httpService = Substitute.For<IHttpService>();
            var plexBasicHeadersConstructor = Substitute.For<IHeaderConstructor<PlexBasicRequestHeaders>>();
            var instance = new PlexMediaProviderService(httpService, plexBasicHeadersConstructor, plexConfig);
            string endpointUrl = Endpoint.Libraries.Description(plexConfig.ServerAddress);
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, endpointUrl);
            HttpRequest httpRequest = httpRequestMessage.ToHttpRequest();
            
            var expectedMediaContainer = new BasePlexResponse<Libraries>
            {
                MediaContainer = new Libraries
                {
                    Size = 1,
                    Directory = new List<Directory>
                    {
                        new Directory
                        {
                            Title = "Test Movie Title",
                            Key = "123",
                            Art = "test-art",
                            Thumb = "test-thumb",
                            Type = "movie"
                        }
                    }
                }
            };
            var expectedHttpResponse = new HttpResponse<BasePlexResponse<Libraries>>();
            var expectedHttpResponseMessage = new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedMediaContainer))
            };
            
            await expectedHttpResponse.SetResponseAsync(httpRequest.Identifier, expectedHttpResponseMessage, CancellationToken.None);

            httpService
                .RequestAsync<BasePlexResponse<Libraries>>(Arg.Any<HttpRequest>(), Arg.Any<CancellationToken>())
                .Returns((request) =>
                {
                    // Assert the details of the request to be correct
                    var requestContext = request.Args()[0] as HttpRequest;
                    
                    Assert.Equal(httpRequestMessage.RequestUri.AbsoluteUri, requestContext?.Request.RequestUri.AbsoluteUri);
                    Assert.Equal(httpRequestMessage.Method, requestContext?.Request.Method);
                    
                    return expectedHttpResponse;
                });
            
            // Perform action
            IEnumerable<Library> result = await instance.GetAllLibrariesAsync(CancellationToken.None);
            
            // Assert
            httpService
                .Received()
                .RequestAsync<BasePlexResponse<Libraries>>(Arg.Any<HttpRequest>(), CancellationToken.None);
            Assert.Equal(httpRequest.Identifier, expectedHttpResponse.RequestIdentifier);
            Assert.True(result.Any());
            Assert.Equal(expectedMediaContainer.MediaContainer.Directory.First().Key, result.First().Id);
            Assert.Equal(expectedMediaContainer.MediaContainer.Directory.First().Title, result.First().Title);
            Assert.Equal($"{plexConfig.ServerAddress}{expectedMediaContainer.MediaContainer.Directory.First().Art}?X-Plex-Token={plexConfig.PlexAuthenticatedUser.AuthToken}", result.First().Poster);
            Assert.Equal($"{plexConfig.ServerAddress}{expectedMediaContainer.MediaContainer.Directory.First().Thumb}?X-Plex-Token={plexConfig.PlexAuthenticatedUser.AuthToken}", result.First().Thumbnail);
            Assert.Equal(LibraryType.Movie, result.First().Type);
        }
        
        [Fact]
        public async Task GetAllLibrariesAsync_WithResultWithInvalidType_ShouldThrowArgumentNullException()
        {
            // Setup
            var plexConfig = new PlexMediaServerConfig
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
                ServerAddress = "http://192.168.0.5:32400",
                
            };
            
            var httpService = Substitute.For<IHttpService>();
            var plexBasicHeadersConstructor = Substitute.For<IHeaderConstructor<PlexBasicRequestHeaders>>();
            var instance = new PlexMediaProviderService(httpService, plexBasicHeadersConstructor, plexConfig);
            string endpointUrl = Endpoint.Libraries.Description(plexConfig.ServerAddress);
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, endpointUrl);
            HttpRequest httpRequest = httpRequestMessage.ToHttpRequest();
            
            var expectedMediaContainer = new BasePlexResponse<Libraries>
            {
                MediaContainer = new Libraries
                {
                    Size = 1,
                    Directory = new List<Directory>
                    {
                        new Directory
                        {
                            Title = "Test Movie Title",
                            Key = "123",
                            Art = "test-art",
                            Thumb = "test-thumb",
                            Type = null
                        }
                    }
                }
            };
            var expectedHttpResponse = new HttpResponse<BasePlexResponse<Libraries>>();
            var expectedHttpResponseMessage = new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedMediaContainer))
            };
            
            await expectedHttpResponse.SetResponseAsync(httpRequest.Identifier, expectedHttpResponseMessage, CancellationToken.None);

            httpService
                .RequestAsync<BasePlexResponse<Libraries>>(Arg.Any<HttpRequest>(), Arg.Any<CancellationToken>())
                .Returns((request) =>
                {
                    // Assert the details of the request to be correct
                    var requestContext = request.Args()[0] as HttpRequest;
                    
                    Assert.Equal(httpRequestMessage.RequestUri.AbsoluteUri, requestContext?.Request.RequestUri.AbsoluteUri);
                    Assert.Equal(httpRequestMessage.Method, requestContext?.Request.Method);
                    
                    return expectedHttpResponse;
                });
            
            // Perform action
            IEnumerable<Library> result = await instance.GetAllLibrariesAsync(CancellationToken.None);
            
            // Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(async () => await result.First().GetMoviesAsync);
            
            Assert.Equal("libraryType", exception.ParamName);
        }
        
        [Fact]
        public async Task GetAllLibrariesAsync_WithResultWithInvalidId_ShouldThrowArgumentNullException()
        {
            // Setup
            var plexConfig = new PlexMediaServerConfig
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
                ServerAddress = "http://192.168.0.5:32400",
                
            };
            
            var httpService = Substitute.For<IHttpService>();
            var plexBasicHeadersConstructor = Substitute.For<IHeaderConstructor<PlexBasicRequestHeaders>>();
            var instance = new PlexMediaProviderService(httpService, plexBasicHeadersConstructor, plexConfig);
            string endpointUrl = Endpoint.Libraries.Description(plexConfig.ServerAddress);
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, endpointUrl);
            HttpRequest httpRequest = httpRequestMessage.ToHttpRequest();
            
            var expectedMediaContainer = new BasePlexResponse<Libraries>
            {
                MediaContainer = new Libraries
                {
                    Size = 1,
                    Directory = new List<Directory>
                    {
                        new Directory
                        {
                            Title = "Test Movie Title",
                            Key = null,
                            Art = "test-art",
                            Thumb = "test-thumb",
                            Type = "movie"
                        }
                    }
                }
            };
            var expectedHttpResponse = new HttpResponse<BasePlexResponse<Libraries>>();
            var expectedHttpResponseMessage = new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedMediaContainer))
            };
            
            await expectedHttpResponse.SetResponseAsync(httpRequest.Identifier, expectedHttpResponseMessage, CancellationToken.None);

            httpService
                .RequestAsync<BasePlexResponse<Libraries>>(Arg.Any<HttpRequest>(), Arg.Any<CancellationToken>())
                .Returns((request) =>
                {
                    // Assert the details of the request to be correct
                    var requestContext = request.Args()[0] as HttpRequest;
                    
                    Assert.Equal(httpRequestMessage.RequestUri.AbsoluteUri, requestContext?.Request.RequestUri.AbsoluteUri);
                    Assert.Equal(httpRequestMessage.Method, requestContext?.Request.Method);
                    
                    return expectedHttpResponse;
                });
            
            // Perform action
            IEnumerable<Library> result = await instance.GetAllLibrariesAsync(CancellationToken.None);
            
            // Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(async () => await result.First().GetMoviesAsync);
            
            Assert.Equal("libraryId", exception.ParamName);
        }
        
        [Fact]
        public async Task GetAllLibrariesAsync_WithResultWithNonMovieLibraryType_ShouldReturnEmpty()
        {
            // Setup
            var plexConfig = new PlexMediaServerConfig
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
                ServerAddress = "http://192.168.0.5:32400",
                
            };
            
            var httpService = Substitute.For<IHttpService>();
            var plexBasicHeadersConstructor = Substitute.For<IHeaderConstructor<PlexBasicRequestHeaders>>();
            var instance = new PlexMediaProviderService(httpService, plexBasicHeadersConstructor, plexConfig);
            string endpointUrl = Endpoint.Libraries.Description(plexConfig.ServerAddress);
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, endpointUrl);
            HttpRequest httpRequest = httpRequestMessage.ToHttpRequest();
            
            var expectedMediaContainer = new BasePlexResponse<Libraries>
            {
                MediaContainer = new Libraries
                {
                    Size = 1,
                    Directory = new List<Directory>
                    {
                        new Directory
                        {
                            Title = "Test Movie Title",
                            Key = "123",
                            Art = "test-art",
                            Thumb = "test-thumb",
                            Type = "tv"
                        }
                    }
                }
            };
            var expectedHttpResponse = new HttpResponse<BasePlexResponse<Libraries>>();
            var expectedHttpResponseMessage = new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedMediaContainer))
            };
            
            await expectedHttpResponse.SetResponseAsync(httpRequest.Identifier, expectedHttpResponseMessage, CancellationToken.None);

            httpService
                .RequestAsync<BasePlexResponse<Libraries>>(Arg.Any<HttpRequest>(), Arg.Any<CancellationToken>())
                .Returns((request) =>
                {
                    // Assert the details of the request to be correct
                    var requestContext = request.Args()[0] as HttpRequest;
                    
                    Assert.Equal(httpRequestMessage.RequestUri.AbsoluteUri, requestContext?.Request.RequestUri.AbsoluteUri);
                    Assert.Equal(httpRequestMessage.Method, requestContext?.Request.Method);
                    
                    return expectedHttpResponse;
                });
            
            // Perform action
            IEnumerable<Library> result = await instance.GetAllLibrariesAsync(CancellationToken.None);
            IEnumerable<Movie> movies = await result.First().GetMoviesAsync;
            
            // Assert
            Assert.Empty(movies);
        }
        
        [Fact]
        public async Task GetAllLibrariesAsync_WithResult_ShouldCallHttpRequestWithCorrectParamsAndReturnEmptyMoviesList()
        {
            // Setup
            var plexConfig = new PlexMediaServerConfig
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
                ServerAddress = "http://192.168.0.5:32400",
                
            };
            
            var httpService = Substitute.For<IHttpService>();
            var plexBasicHeadersConstructor = Substitute.For<IHeaderConstructor<PlexBasicRequestHeaders>>();
            var instance = new PlexMediaProviderService(httpService, plexBasicHeadersConstructor, plexConfig);
            string endpointUrl = Endpoint.Libraries.Description(plexConfig.ServerAddress);
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, endpointUrl);
            HttpRequest httpRequest = httpRequestMessage.ToHttpRequest();
            
            var expectedMediaContainer = new BasePlexResponse<Libraries>
            {
                MediaContainer = new Libraries
                {
                    Size = 1,
                    Directory = new List<Directory>
                    {
                        new Directory
                        {
                            Title = "Test Movie Title",
                            Key = "123",
                            Art = "test-art",
                            Thumb = "test-thumb",
                            Type = "movie"
                        }
                    }
                }
            };
            var expectedHttpResponse = new HttpResponse<BasePlexResponse<Libraries>>();
            var expectedHttpResponseMessage = new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedMediaContainer))
            };
            
            await expectedHttpResponse.SetResponseAsync(httpRequest.Identifier, expectedHttpResponseMessage, CancellationToken.None);

            httpService
                .RequestAsync<BasePlexResponse<Libraries>>(Arg.Any<HttpRequest>(), Arg.Any<CancellationToken>())
                .Returns((request) =>
                {
                    // Assert the details of the request to be correct
                    var requestContext = request.Args()[0] as HttpRequest;
                    
                    Assert.Equal(httpRequestMessage.RequestUri.AbsoluteUri, requestContext?.Request.RequestUri.AbsoluteUri);
                    Assert.Equal(httpRequestMessage.Method, requestContext?.Request.Method);
                    
                    return expectedHttpResponse;
                });
            
            // Get movies for library
            var expectedMoviesMediaContainer = new BasePlexResponse<MediaContainer>
            {
                MediaContainer = new MediaContainer()
            };
            var expectedMoviesHttpResponse = new HttpResponse<BasePlexResponse<MediaContainer>>();
            var expectedMoviesHttpResponseMessage = new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedMoviesMediaContainer))
            };
            
            await expectedMoviesHttpResponse.SetResponseAsync(Guid.NewGuid(), expectedMoviesHttpResponseMessage, CancellationToken.None);
            
            httpService
                .RequestAsync<BasePlexResponse<MediaContainer>>(Arg.Any<HttpRequest>(), Arg.Any<CancellationToken>())
                .Returns((request) =>
                {
                    // Assert the details of the request to be correct
                    var requestContext = request.Args()[0] as HttpRequest;
                    
                    Assert.Equal(Endpoint.LibraryMovies.Description(plexConfig.ServerAddress, expectedMediaContainer.MediaContainer.Directory.First().Key), requestContext?.Request.RequestUri.AbsoluteUri);
                    Assert.Equal(httpRequestMessage.Method, HttpMethod.Get);
                    
                    return expectedMoviesHttpResponse;
                });
            
            // Perform action
            IEnumerable<Library> result = await instance.GetAllLibrariesAsync(CancellationToken.None);
            IEnumerable<Movie> movies = await result.First().GetMoviesAsync;
            
            // Assert
            httpService
                .Received()
                .RequestAsync<BasePlexResponse<MediaContainer>>(Arg.Any<HttpRequest>(), CancellationToken.None);
            Assert.Empty(movies);
        }
        
        [Fact]
        public async Task GetAllLibrariesAsync_WithResult_ShouldCallHttpRequestWithCorrectParamsAndReturnCorrectlyCastedMovies()
        {
            // Setup
            var plexConfig = new PlexMediaServerConfig
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
                ServerAddress = "http://192.168.0.5:32400",
                
            };
            
            var httpService = Substitute.For<IHttpService>();
            var plexBasicHeadersConstructor = Substitute.For<IHeaderConstructor<PlexBasicRequestHeaders>>();
            var instance = new PlexMediaProviderService(httpService, plexBasicHeadersConstructor, plexConfig);
            string endpointUrl = Endpoint.Libraries.Description(plexConfig.ServerAddress);
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, endpointUrl);
            HttpRequest httpRequest = httpRequestMessage.ToHttpRequest();
            
            var expectedMediaContainer = new BasePlexResponse<Libraries>
            {
                MediaContainer = new Libraries
                {
                    Size = 1,
                    Directory = new List<Directory>
                    {
                        new Directory
                        {
                            Title = "Test Movie Title",
                            Key = "123",
                            Art = "test-art",
                            Thumb = "test-thumb",
                            Type = "movie"
                        }
                    }
                }
            };
            var expectedHttpResponse = new HttpResponse<BasePlexResponse<Libraries>>();
            var expectedHttpResponseMessage = new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedMediaContainer))
            };
            
            await expectedHttpResponse.SetResponseAsync(httpRequest.Identifier, expectedHttpResponseMessage, CancellationToken.None);

            httpService
                .RequestAsync<BasePlexResponse<Libraries>>(Arg.Any<HttpRequest>(), Arg.Any<CancellationToken>())
                .Returns((request) =>
                {
                    // Assert the details of the request to be correct
                    var requestContext = request.Args()[0] as HttpRequest;
                    
                    Assert.Equal(httpRequestMessage.RequestUri.AbsoluteUri, requestContext?.Request.RequestUri.AbsoluteUri);
                    Assert.Equal(httpRequestMessage.Method, requestContext?.Request.Method);
                    
                    return expectedHttpResponse;
                });
            
            // Get movies for library
            var expectedMetadata = new Metadata
            {
                Media = new List<Media>
                {
                    new Media
                    {
                        MemoryChannels = 6,
                        MemoryCodec = "aac",
                        Bitrate = 128,
                        Container = "mp4",
                        Height = 1080,
                        Width = 1920,
                        Part = new List<Part>
                        {
                            new Part
                            {
                                Key = "test_key"
                            }
                        },
                        VideoCodec = "test_vid_codec"
                    }
                },
                Summary = "Test summary",
                Duration = 999,
                Art = "test_art",
                Rating = 9.5,
                Studio = "Marvel",
                Title = "test_title",
                ViewCount = 10,
                Year = 2010
            };
            var expectedMoviesMediaContainer = new BasePlexResponse<MediaContainer>
            {
                MediaContainer = new MediaContainer
                {
                    Metadata = new List<Metadata>
                    {
                        expectedMetadata
                    }
                }
            };
            var expectedMoviesHttpResponse = new HttpResponse<BasePlexResponse<MediaContainer>>();
            var expectedMoviesHttpResponseMessage = new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedMoviesMediaContainer))
            };
            
            await expectedMoviesHttpResponse.SetResponseAsync(Guid.NewGuid(), expectedMoviesHttpResponseMessage, CancellationToken.None);
            
            httpService
                .RequestAsync<BasePlexResponse<MediaContainer>>(Arg.Any<HttpRequest>(), Arg.Any<CancellationToken>())
                .Returns((request) =>
                {
                    // Assert the details of the request to be correct
                    var requestContext = request.Args()[0] as HttpRequest;
                    
                    Assert.Equal(Endpoint.LibraryMovies.Description(plexConfig.ServerAddress, expectedMediaContainer.MediaContainer.Directory.First().Key), requestContext?.Request.RequestUri.AbsoluteUri);
                    Assert.Equal(httpRequestMessage.Method, HttpMethod.Get);
                    
                    return expectedMoviesHttpResponse;
                });
            
            // Perform action
            IEnumerable<Library> result = await instance.GetAllLibrariesAsync(CancellationToken.None);
            IEnumerable<Movie> movies = await result.First().GetMoviesAsync;
            var actual = movies.First();
            
            // Assert
            httpService
                .Received()
                .RequestAsync<BasePlexResponse<MediaContainer>>(Arg.Any<HttpRequest>(), CancellationToken.None);
            
            Assert.Equal(expectedMetadata.Media.First().MemoryChannels, actual.MemoryChannels);
            Assert.Equal(expectedMetadata.Media.First().MemoryCodec, actual.MemoryCodec);
            Assert.Equal(expectedMetadata.Media.First().Bitrate, actual.Bitrate);
            Assert.Equal(expectedMetadata.Media.First().Container, actual.Container);
            Assert.Equal(expectedMetadata.Summary, actual.Description);
            Assert.Equal(expectedMetadata.Duration, actual.Duration);
            Assert.Equal(expectedMetadata.Media.First().Height, actual.Height);
            Assert.Equal(expectedMetadata.Media.First().Width, actual.Width);
            Assert.Equal($"{plexConfig.ServerAddress}{expectedMetadata.Art}?{plexConfig.QueryStringPlexToken}", actual.Poster);
            Assert.Equal(expectedMetadata.Rating, actual.Rating);
            Assert.Equal($"{plexConfig.ServerAddress}{expectedMetadata.Media.First().Part.First().Key}?{plexConfig.QueryStringPlexToken}", actual.StreamingUrl);
            Assert.Equal(expectedMetadata.Studio, actual.Studio);
            Assert.Equal($"{plexConfig.ServerAddress}{expectedMetadata.Thumb}?{plexConfig.QueryStringPlexToken}", actual.Thumbnail);
            Assert.Equal(expectedMetadata.Title, actual.Title);
            Assert.Equal(expectedMetadata.Media.First().VideoCodec, actual.VideoCodec);
            Assert.Equal(expectedMetadata.ViewCount, actual.ViewCount);
            Assert.Equal(expectedMetadata.Year, actual.Year);
        }
    }
}