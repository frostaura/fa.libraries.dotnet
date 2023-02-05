using System;
using System.Collections.Generic;
using FrostAura.Libraries.Http.Interfaces;
using MediaServer.Plex.Models.Requests;
using MediaServer.Plex.Services;
using Xunit;

namespace MediaServer.Plex.Tests.Services
{
    public class PlexBasicHeaderConstructorServiceTests
    {
        [Fact]
        public void ConstructRequestHeaders_WithInvalidRequest_ShouldThrowArgumentNullException()
        {
            // Setup
            PlexBasicRequestHeaders request = null;
            IHeaderConstructor<PlexBasicRequestHeaders> instance = new PlexBasicHeaderConstructorService();

            // Perform
            var exception = Assert.Throws<ArgumentNullException>(() => instance.ConstructRequestHeaders(request));

            // Assert
            Assert.Equal("data", exception.ParamName);
        }
        
        [Fact]
        public void ConstructRequestHeaders_WithInvalidPlatform_ShouldThrowArgumentNullException()
        {
            // Setup
            var request = new PlexBasicRequestHeaders
            {
                Platform = null
            };
            IHeaderConstructor<PlexBasicRequestHeaders> instance = new PlexBasicHeaderConstructorService();

            // Perform
            var exception = Assert.Throws<ArgumentNullException>(() => instance.ConstructRequestHeaders(request));

            // Assert
            Assert.Equal("Platform", exception.ParamName);
        }
        
        [Fact]
        public void ConstructRequestHeaders_WithInvalidPlatformVersion_ShouldThrowArgumentNullException()
        {
            // Setup
            var request = new PlexBasicRequestHeaders
            {
                PlatformVersion = null
            };
            IHeaderConstructor<PlexBasicRequestHeaders> instance = new PlexBasicHeaderConstructorService();

            // Perform
            var exception = Assert.Throws<ArgumentNullException>(() => instance.ConstructRequestHeaders(request));

            // Assert
            Assert.Equal("PlatformVersion", exception.ParamName);
        }
        
        [Fact]
        public void ConstructRequestHeaders_WithInvalidProvides_ShouldThrowArgumentNullException()
        {
            // Setup
            var request = new PlexBasicRequestHeaders
            {
                Provides = null
            };
            IHeaderConstructor<PlexBasicRequestHeaders> instance = new PlexBasicHeaderConstructorService();

            // Perform
            var exception = Assert.Throws<ArgumentNullException>(() => instance.ConstructRequestHeaders(request));

            // Assert
            Assert.Equal("Provides", exception.ParamName);
        }
        
        [Fact]
        public void ConstructRequestHeaders_WithInvalidClientIdentifier_ShouldThrowArgumentNullException()
        {
            // Setup
            var request = new PlexBasicRequestHeaders
            {
                ClientIdentifier = null
            };
            IHeaderConstructor<PlexBasicRequestHeaders> instance = new PlexBasicHeaderConstructorService();

            // Perform
            var exception = Assert.Throws<ArgumentNullException>(() => instance.ConstructRequestHeaders(request));

            // Assert
            Assert.Equal("ClientIdentifier", exception.ParamName);
        }
        
        [Fact]
        public void ConstructRequestHeaders_WithInvalidProduct_ShouldThrowArgumentNullException()
        {
            // Setup
            var request = new PlexBasicRequestHeaders
            {
                Product = null
            };
            IHeaderConstructor<PlexBasicRequestHeaders> instance = new PlexBasicHeaderConstructorService();

            // Perform
            var exception = Assert.Throws<ArgumentNullException>(() => instance.ConstructRequestHeaders(request));

            // Assert
            Assert.Equal("Product", exception.ParamName);
        }
        
        [Fact]
        public void ConstructRequestHeaders_WithInvalidVersion_ShouldThrowArgumentNullException()
        {
            // Setup
            var request = new PlexBasicRequestHeaders
            {
                Version = null
            };
            IHeaderConstructor<PlexBasicRequestHeaders> instance = new PlexBasicHeaderConstructorService();

            // Perform
            var exception = Assert.Throws<ArgumentNullException>(() => instance.ConstructRequestHeaders(request));

            // Assert
            Assert.Equal("Version", exception.ParamName);
        }
        
        [Fact]
        public void ConstructRequestHeaders_WithValidRequest_ShouldReturnValidHeaders()
        {
            // Setup
            var request = new PlexBasicRequestHeaders();
            IHeaderConstructor<PlexBasicRequestHeaders> instance = new PlexBasicHeaderConstructorService();

            // Perform
            IDictionary<string, string> actual = instance.ConstructRequestHeaders(request);

            // Assert
            Assert.True(actual.ContainsKey("X-Plex-Platform"));
            Assert.True(actual.ContainsKey("X-Plex-Platform-Version"));
            Assert.True(actual.ContainsKey("X-Plex-Provides"));
            Assert.True(actual.ContainsKey("X-Plex-Client-Identifier"));
            Assert.True(actual.ContainsKey("X-Plex-Product"));
            Assert.True(actual.ContainsKey("X-Plex-Version"));
            
            Assert.Equal(request.Platform, actual["X-Plex-Platform"]);
            Assert.Equal(request.PlatformVersion, actual["X-Plex-Platform-Version"]);
            Assert.Equal(request.Provides, actual["X-Plex-Provides"]);
            Assert.Equal(request.ClientIdentifier, actual["X-Plex-Client-Identifier"]);
            Assert.Equal(request.Product, actual["X-Plex-Product"]);
            Assert.Equal(request.Version, actual["X-Plex-Version"]);
        }
    }
}