using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FrostAura.Libraries.Core.Models.Auth;
using FrostAura.Libraries.Http.Interfaces;
using MediaServer.Core.Enums;
using MediaServer.Core.Models.Content;
using MediaServer.Plex.Interfaces;
using MediaServer.Plex.Models.Config;
using MediaServer.Plex.Models.Content;
using MediaServer.Plex.Models.Responses;
using MediaServer.Plex.Services;
using NSubstitute;
using Xunit;

namespace MediaServer.Plex.Tests.Services
{
    public class PlexMediaServiceTests
    {
        [Fact]
        public void Constructor_WithInvalidConfig_ShouldThrowArgumentNullException()
        {
            // Setup
            PlexMediaServerConfig config = null;
            var httpService = Substitute.For<IHttpService>();
            var authenticator = Substitute.For<IPlexAuthenticator>();
            var settingsProvider = Substitute.For<IPlexServerSettingsProvider>();
            var mediaProvider = Substitute.For<IPlexMediaProvider>();
            
            // Perform
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new PlexMediaService(config, httpService, authenticator, settingsProvider, mediaProvider));

            // Assert
            Assert.Equal("configuration", exception.ParamName);
        }
        
        [Fact]
        public void Constructor_WithInvalidHttpService_ShouldThrowArgumentNullException()
        {
            // Setup
            PlexMediaServerConfig config = new PlexMediaServerConfig
            {
                ServerAddress = "http://192.168.0.5:32400",
                PlexAuthenticationRequestUser = new BasicAuth
                {
                    Username = "test username",
                    Password = "test password"
                }
            };
            IHttpService httpService = null;
            var authenticator = Substitute.For<IPlexAuthenticator>();
            var settingsProvider = Substitute.For<IPlexServerSettingsProvider>();
            var mediaProvider = Substitute.For<IPlexMediaProvider>();
            
            // Perform
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new PlexMediaService(config, httpService, authenticator, settingsProvider, mediaProvider));

            // Assert
            Assert.Equal("httpService", exception.ParamName);
        }
        
        [Fact]
        public void Constructor_WithInvalidAuthenticator_ShouldThrowArgumentNullException()
        {
            // Setup
            PlexMediaServerConfig config = new PlexMediaServerConfig
            {
                ServerAddress = "http://192.168.0.5:32400",
                PlexAuthenticationRequestUser = new BasicAuth
                {
                    Username = "test username",
                    Password = "test password"
                }
            };
            var httpService = Substitute.For<IHttpService>();
            IPlexAuthenticator authenticator = null;
            var settingsProvider = Substitute.For<IPlexServerSettingsProvider>();
            var mediaProvider = Substitute.For<IPlexMediaProvider>();
            
            // Perform
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new PlexMediaService(config, httpService, authenticator, settingsProvider, mediaProvider));

            // Assert
            Assert.Equal("authenticator", exception.ParamName);
        }
        
        [Fact]
        public void Constructor_WithInvalidSettingsService_ShouldThrowArgumentNullException()
        {
            // Setup
            PlexMediaServerConfig config = new PlexMediaServerConfig
            {
                ServerAddress = "http://192.168.0.5:32400",
                PlexAuthenticationRequestUser = new BasicAuth
                {
                    Username = "test username",
                    Password = "test password"
                }
            };
            var httpService = Substitute.For<IHttpService>();
            var authenticator = Substitute.For<IPlexAuthenticator>();;
            IPlexServerSettingsProvider settingsProvider = null;
            var mediaProvider = Substitute.For<IPlexMediaProvider>();
            
            // Perform
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new PlexMediaService(config, httpService, authenticator, settingsProvider, mediaProvider));

            // Assert
            Assert.Equal("serverSettingsProvider", exception.ParamName);
        }
        
        [Fact]
        public void Constructor_WithInvalidMediaProviderService_ShouldThrowArgumentNullException()
        {
            // Setup
            PlexMediaServerConfig config = new PlexMediaServerConfig
            {
                ServerAddress = "http://192.168.0.5:32400",
                PlexAuthenticationRequestUser = new BasicAuth
                {
                    Username = "test username",
                    Password = "test password"
                }
            };
            var httpService = Substitute.For<IHttpService>();
            var authenticator = Substitute.For<IPlexAuthenticator>();;
            var settingsProvider = Substitute.For<IPlexServerSettingsProvider>();
            IPlexMediaProvider mediaProvider = null;
            
            // Perform
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new PlexMediaService(config, httpService, authenticator, settingsProvider, mediaProvider));

            // Assert
            Assert.Equal("mediaProvider", exception.ParamName);
        }
        
        [Fact]
        public void Constructor_WithValidParams_ShouldConstruct()
        {
            // Setup
            PlexMediaServerConfig config = new PlexMediaServerConfig
            {
                ServerAddress = "http://192.168.0.5:32400",
                PlexAuthenticationRequestUser = new BasicAuth
                {
                    Username = "test username",
                    Password = "test password"
                }
            };
            var httpService = Substitute.For<IHttpService>();
            var authenticator = Substitute.For<IPlexAuthenticator>();;
            var settingsProvider = Substitute.For<IPlexServerSettingsProvider>();
            var mediaProvider = Substitute.For<IPlexMediaProvider>();
            
            // Perform
            var actual = new PlexMediaService(config, httpService, authenticator, settingsProvider, mediaProvider);

            // Assert
            Assert.NotNull(actual);
        }
        
        [Fact]
        public async Task InitializeAsync_WithInvalidAuthResponse_ShouldReturnUnauthorised()
        {
            // Setup
            PlexMediaServerConfig config = new PlexMediaServerConfig
            {
                ServerAddress = "http://192.168.0.5:32400",
                PlexAuthenticationRequestUser = new BasicAuth
                {
                    Username = "test username",
                    Password = "test password"
                }
            };
            var httpService = Substitute.For<IHttpService>();
            var authenticator = Substitute.For<IPlexAuthenticator>();
            var settingsProvider = Substitute.For<IPlexServerSettingsProvider>();
            var mediaProvider = Substitute.For<IPlexMediaProvider>();
            var instance = new PlexMediaService(config, httpService, authenticator, settingsProvider, mediaProvider);

            authenticator
                .AuthenticateAsync(Arg.Any<CancellationToken>())
                .Returns(info => new UserAuthenticationResponse());
            
            // Perform
            InitializationStatus actual = await instance.InitializeAsync((servers) => servers.First().PublicAddress, CancellationToken.None);

            // Assert
            Assert.Equal(InitializationStatus.Unauthorised, actual);
        }
        
        [Fact]
        public async Task InitializeAsync_WithInvalidServersResponse_ShouldReturnNoServersDiscovered()
        {
            // Setup
            PlexMediaServerConfig config = new PlexMediaServerConfig
            {
                ServerAddress = "http://192.168.0.5:32400",
                PlexAuthenticationRequestUser = new BasicAuth
                {
                    Username = "test username",
                    Password = "test password"
                }
            };
            var httpService = Substitute.For<IHttpService>();
            var authenticator = Substitute.For<IPlexAuthenticator>();
            var settingsProvider = Substitute.For<IPlexServerSettingsProvider>();
            var mediaProvider = Substitute.For<IPlexMediaProvider>();
            var instance = new PlexMediaService(config, httpService, authenticator, settingsProvider, mediaProvider);
            var expectedUser = new User
            {
                Email = "test@test.com"
            };

            authenticator
                .GetAllServers(Arg.Any<CancellationToken>())
                .Returns(info => new List<Device>());
            authenticator
                .AuthenticateAsync(Arg.Any<CancellationToken>())
                .Returns(info => new UserAuthenticationResponse
                {
                    User = expectedUser
                });
            
            // Perform
            InitializationStatus actual = await instance.InitializeAsync((servers) => servers.First().PublicAddress, CancellationToken.None);

            // Assert
            Assert.Equal(InitializationStatus.NoServersDiscovered, actual);
        }
        
        [Fact]
        public async Task InitializeAsync_WithAuthenticatedUser_ShouldSetUserContext()
        {
            // Setup
            PlexMediaServerConfig config = new PlexMediaServerConfig
            {
                ServerAddress = "http://192.168.0.5:32400",
                PlexAuthenticationRequestUser = new BasicAuth
                {
                    Username = "test username",
                    Password = "test password"
                }
            };
            var httpService = Substitute.For<IHttpService>();
            var authenticator = Substitute.For<IPlexAuthenticator>();
            var settingsProvider = Substitute.For<IPlexServerSettingsProvider>();
            var mediaProvider = Substitute.For<IPlexMediaProvider>();
            var instance = new PlexMediaService(config, httpService, authenticator, settingsProvider, mediaProvider);
            var expectedUser = new User
            {
                Email = "test@test.com"
            };

            authenticator
                .GetAllServers(Arg.Any<CancellationToken>())
                .Returns(info => new List<Device>()
                {
                    new Device()
                });
            authenticator
                .AuthenticateAsync(Arg.Any<CancellationToken>())
                .Returns(info => new UserAuthenticationResponse
                {
                    User = expectedUser
                });
            
            // Perform
            InitializationStatus actual = await instance.InitializeAsync((servers) => servers.First().PublicAddress, CancellationToken.None);

            // Assert
            Assert.NotNull(instance.Configuration?.PlexAuthenticatedUser);
            Assert.Equal(expectedUser.Email, instance.Configuration.PlexAuthenticatedUser.Email);
        }
        
        [Fact]
        public async Task InitializeAsync_WithValidServers_ShouldSetDiscoveredServersContext()
        {
            // Setup
            PlexMediaServerConfig config = new PlexMediaServerConfig
            {
                ServerAddress = "http://192.168.0.5:32400",
                PlexAuthenticationRequestUser = new BasicAuth
                {
                    Username = "test username",
                    Password = "test password"
                }
            };
            var httpService = Substitute.For<IHttpService>();
            var authenticator = Substitute.For<IPlexAuthenticator>();
            var settingsProvider = Substitute.For<IPlexServerSettingsProvider>();
            var mediaProvider = Substitute.For<IPlexMediaProvider>();
            var instance = new PlexMediaService(config, httpService, authenticator, settingsProvider, mediaProvider);
            var expectedUser = new User
            {
                Email = "test@test.com"
            };
            var expectedDevice = new Device
            {
                Id = "some_server"
            };

            authenticator
                .GetAllServers(Arg.Any<CancellationToken>())
                .Returns(info =>
                    new List<Device>()
                    {
                        expectedDevice
                    });
            authenticator
                .AuthenticateAsync(Arg.Any<CancellationToken>())
                .Returns(info => new UserAuthenticationResponse
                {
                    User = expectedUser
                });
            
            // Perform
            InitializationStatus actual = await instance.InitializeAsync((servers) => servers.First().PublicAddress, CancellationToken.None);

            // Assert
            Assert.NotNull(instance.Configuration?.DiscoveredServers);
            Assert.Equal(expectedDevice.Id, instance.Configuration.DiscoveredServers.First().Id);
        }
        
        [Fact]
        public async Task InitializeAsync_WithNoSettings_ShouldReturnError()
        {
            // Setup
            PlexMediaServerConfig config = new PlexMediaServerConfig
            {
                ServerAddress = "http://192.168.0.5:32400",
                PlexAuthenticationRequestUser = new BasicAuth
                {
                    Username = "test username",
                    Password = "test password"
                }
            };
            var httpService = Substitute.For<IHttpService>();
            var authenticator = Substitute.For<IPlexAuthenticator>();
            var settingsProvider = Substitute.For<IPlexServerSettingsProvider>();
            var mediaProvider = Substitute.For<IPlexMediaProvider>();
            var instance = new PlexMediaService(config, httpService, authenticator, settingsProvider, mediaProvider);
            var expectedUser = new User
            {
                Email = "test@test.com"
            };

            authenticator
                .GetAllServers(Arg.Any<CancellationToken>())
                .Returns(info => new List<Device>()
                {
                    new Device()
                });
            authenticator
                .AuthenticateAsync(Arg.Any<CancellationToken>())
                .Returns(info => new UserAuthenticationResponse
                {
                    User = expectedUser
                });
            settingsProvider
                .GetServerSettingsAsync(Arg.Any<CancellationToken>())
                .Returns(info => new ServerPreferences());
            
            // Perform
            InitializationStatus actual = await instance.InitializeAsync((servers) => servers.First().PublicAddress, CancellationToken.None);

            // Assert
            Assert.Equal(InitializationStatus.Error, actual);
        }
        
        [Fact]
        public async Task InitializeAsync_WithSettings_ShouldSetSettings()
        {
            // Setup
            PlexMediaServerConfig config = new PlexMediaServerConfig
            {
                ServerAddress = "http://192.168.0.5:32400",
                PlexAuthenticationRequestUser = new BasicAuth
                {
                    Username = "test username",
                    Password = "test password"
                }
            };
            var httpService = Substitute.For<IHttpService>();
            var authenticator = Substitute.For<IPlexAuthenticator>();
            var settingsProvider = Substitute.For<IPlexServerSettingsProvider>();
            var mediaProvider = Substitute.For<IPlexMediaProvider>();
            var instance = new PlexMediaService(config, httpService, authenticator, settingsProvider, mediaProvider);
            var expectedUser = new User
            {
                Email = "test@test.com"
            };

            authenticator
                .GetAllServers(Arg.Any<CancellationToken>())
                .Returns(info => new List<Device>()
                {
                    new Device()
                });
            authenticator
                .AuthenticateAsync(Arg.Any<CancellationToken>())
                .Returns(info => new UserAuthenticationResponse
                {
                    User = expectedUser
                });
            settingsProvider
                .GetServerSettingsAsync(Arg.Any<CancellationToken>())
                .Returns(info => new ServerPreferences
                {
                    Setting = new List<Setting>()
                    {
                        new Setting()
                    }
                });
            
            // Perform
            InitializationStatus actual = await instance.InitializeAsync((servers) => servers.First().PublicAddress, CancellationToken.None);

            // Assert
            Assert.True(instance.Configuration.ServerPreferences.Setting.Any());
        }
        
        [Fact]
        public async Task InitializeAsync_WithSettings_ShouldReturnOk()
        {
            // Setup
            PlexMediaServerConfig config = new PlexMediaServerConfig
            {
                ServerAddress = "http://192.168.0.5:32400",
                PlexAuthenticationRequestUser = new BasicAuth
                {
                    Username = "test username",
                    Password = "test password"
                }
            };
            var httpService = Substitute.For<IHttpService>();
            var authenticator = Substitute.For<IPlexAuthenticator>();
            var settingsProvider = Substitute.For<IPlexServerSettingsProvider>();
            var mediaProvider = Substitute.For<IPlexMediaProvider>();
            var instance = new PlexMediaService(config, httpService, authenticator, settingsProvider, mediaProvider);
            var expectedUser = new User
            {
                Email = "test@test.com"
            };

            authenticator
                .GetAllServers(Arg.Any<CancellationToken>())
                .Returns(info => new List<Device>()
                {
                    new Device()
                });
            authenticator
                .AuthenticateAsync(Arg.Any<CancellationToken>())
                .Returns(info => new UserAuthenticationResponse
                {
                    User = expectedUser
                });
            settingsProvider
                .GetServerSettingsAsync(Arg.Any<CancellationToken>())
                .Returns(info => new ServerPreferences
                {
                    Setting = new List<Setting>()
                });
            
            // Perform
            InitializationStatus actual = await instance.InitializeAsync((servers) => servers.First().PublicAddress, CancellationToken.None);

            // Assert
            Assert.Equal(InitializationStatus.Ok, actual);
        }
        
        [Fact]
        public async Task GetAllLibrariesAsync_WithValidParams_ShouldCallAndReturnMediaProviderLibraries()
        {
            // Setup
            PlexMediaServerConfig config = new PlexMediaServerConfig
            {
                ServerAddress = "http://192.168.0.5:32400",
                PlexAuthenticationRequestUser = new BasicAuth
                {
                    Username = "test username",
                    Password = "test password"
                }
            };
            var httpService = Substitute.For<IHttpService>();
            var authenticator = Substitute.For<IPlexAuthenticator>();
            var settingsProvider = Substitute.For<IPlexServerSettingsProvider>();
            var mediaProvider = Substitute.For<IPlexMediaProvider>();
            var instance = new PlexMediaService(config, httpService, authenticator, settingsProvider, mediaProvider);

            mediaProvider
                .GetAllLibrariesAsync(Arg.Any<CancellationToken>())
                .Returns(info => new List<Library>
                {
                    new Library()
                });
            
            // Perform
            var actual = await instance.GetAllLibrariesAsync(CancellationToken.None);

            // Assert
            Assert.True(actual.Any());
        }
    }
}