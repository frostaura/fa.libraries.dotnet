﻿using FrostAura.Libraries.Semantic.Core.Models.Configuration;
using FrostAura.Libraries.Semantic.Core.Thoughts.Media;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Semantic.Core.Tests.Thoughts.Media
{
    public class YouTubeThoughtsTests
    {
        [Fact]
        public void Constructor_WithInvalidHttpClientFactory_ShouldThrow()
        {
            IHttpClientFactory httpClientFactory = null;
            var googleConfig = Substitute.For<IOptions<GoogleConfig>>();
            googleConfig.Value.Returns(Config.SEMANTIC_CONFIG.GoogleConfig);
            ILogger<YouTubeThoughts> logger = Substitute.For<ILogger<YouTubeThoughts>>();

            var actual = Assert.Throws<ArgumentNullException>(() => new YouTubeThoughts(httpClientFactory, googleConfig, logger));

            Assert.Equal(nameof(httpClientFactory), actual.ParamName);
        }

        [Fact]
		public void Constructor_WithInvalidGoogleConfig_ShouldThrow()
		{
            IHttpClientFactory httpClientFactory = Substitute.For<IHttpClientFactory>();
            IOptions<GoogleConfig> googleConfig = null;
            ILogger<YouTubeThoughts> logger = Substitute.For<ILogger<YouTubeThoughts>>();

            var actual = Assert.Throws<ArgumentNullException>(() => new YouTubeThoughts(httpClientFactory, googleConfig, logger));

            Assert.Equal(nameof(googleConfig), actual.ParamName);
		}

        [Fact]
        public void Constructor_WithInvalidLogger_ShouldThrow()
        {
            var httpClientFactory = Substitute.For<IHttpClientFactory>();
            var googleConfig = Substitute.For<IOptions<GoogleConfig>>();
            googleConfig.Value.Returns(Config.SEMANTIC_CONFIG.GoogleConfig);
            ILogger<YouTubeThoughts> logger = null;

            var actual = Assert.Throws<ArgumentNullException>(() => new YouTubeThoughts(httpClientFactory, googleConfig, logger));

            Assert.Equal(nameof(logger), actual.ParamName);
        }

        [Fact]
        public void Constructor_WithValidParams_ShouldConstruct()
        {
            var httpClientFactory = Substitute.For<IHttpClientFactory>();
            var googleConfig = Substitute.For<IOptions<GoogleConfig>>();
            googleConfig.Value.Returns(Config.SEMANTIC_CONFIG.GoogleConfig);
            var logger = Substitute.For<ILogger<YouTubeThoughts>>();

            var actual = new YouTubeThoughts(httpClientFactory, googleConfig, logger);

            Assert.NotNull(actual);
        }

        [Fact]
        public async Task PublishLocalVideoToYouTubeAsync_WithInvalidFilePath_ShouldThrow()
        {
            IHttpClientFactory httpClientFactory = Substitute.For<IHttpClientFactory>();
            var googleConfig = Substitute.For<IOptions<GoogleConfig>>();
            googleConfig.Value.Returns(Config.SEMANTIC_CONFIG.GoogleConfig);
            var logger = Substitute.For<ILogger<YouTubeThoughts>>();
            var instance = new YouTubeThoughts(httpClientFactory, googleConfig, logger);
            string filePath = default;
            string description = "test description";
            string tags = "hello world";

            var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.PublishLocalVideoToYouTubeAsync(filePath, description, tags));

            Assert.Equal(nameof(filePath), actual.ParamName);
        }

        [Fact]
        public async Task PublishLocalVideoToYouTubeAsync_WithInvalidDescription_ShouldThrow()
        {
            IHttpClientFactory httpClientFactory = Substitute.For<IHttpClientFactory>();
            var googleConfig = Substitute.For<IOptions<GoogleConfig>>();
            googleConfig.Value.Returns(Config.SEMANTIC_CONFIG.GoogleConfig);
            var logger = Substitute.For<ILogger<YouTubeThoughts>>();
            var instance = new YouTubeThoughts(httpClientFactory, googleConfig, logger);
            string filePath = "dummy filename";
            string description = default;
            string tags = "hello world";

            var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.PublishLocalVideoToYouTubeAsync(filePath, description, tags));

            Assert.Equal(nameof(description), actual.ParamName);
        }

        [Fact]
        public async Task PublishLocalVideoToYouTubeAsync_WithInvalidTags_ShouldThrow()
        {
            IHttpClientFactory httpClientFactory = Substitute.For<IHttpClientFactory>();
            var googleConfig = Substitute.For<IOptions<GoogleConfig>>();
            googleConfig.Value.Returns(Config.SEMANTIC_CONFIG.GoogleConfig);
            var logger = Substitute.For<ILogger<YouTubeThoughts>>();
            var instance = new YouTubeThoughts(httpClientFactory, googleConfig, logger);
            string filePath = "dummy filename";
            string description = "test description";
            string tags = default;

            var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.PublishLocalVideoToYouTubeAsync(filePath, description, tags));

            Assert.Equal(nameof(description), actual.ParamName);
        }

        [Fact]
        public async Task PublishLocalVideoToYouTubeAsync_WithValidTags_ShouldThrow()
        {
            var httpClientFactory = Substitute.For<IHttpClientFactory>();
            var googleConfig = Substitute.For<IOptions<GoogleConfig>>();
            googleConfig.Value.Returns(Config.SEMANTIC_CONFIG.GoogleConfig);
            var logger = Substitute.For<ILogger<YouTubeThoughts>>();
            var instance = new YouTubeThoughts(httpClientFactory, googleConfig, logger);
            var filePath = "videos/6907538454af4d90b4832fa31fdaf070.mp4";
            var description = "Embark on a thrilling cosmic journey as we unravel the mysteries of Mars, the Red Planet. Discover its unique features, from its distinct reddish hue to the largest volcano and deepest canyon in the solar system. Despite its harsh climate, evidence suggests that Mars may have once harbored life. This exploration is not just a space journey, but a voyage back in time, offering insights into the early solar system and the potential for life beyond Earth.";
            var tags = "mars-exploration space-journey red-planet roman-god-of-war iron-oxide olympus-mons valles-marineris mars-climate ancient-rivers-on-mars life-on-other-planets";

            var actual = await instance.PublishLocalVideoToYouTubeAsync(filePath, description, tags);

            Assert.NotEmpty(actual);
        }
    }
}
