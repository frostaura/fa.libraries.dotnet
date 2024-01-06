using FrostAura.Libraries.Semantic.Core.Models.Configuration;
using FrostAura.Libraries.Semantic.Core.Thoughts.Media;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Semantic.Core.Tests.Thoughts.Media
{
    public class StockMediaThoughtsTests
    {
        [Fact]
        public void Constructor_WithInvalidHttpClientFactory_ShouldThrow()
        {
            IHttpClientFactory httpClientFactory = null;
            var pexelsConfig = Substitute.For<IOptions<PexelsConfig>>();
            pexelsConfig.Value.Returns(Config.SEMANTIC_CONFIG.PexelsConfig);
            ILogger<StockMediaThoughts> logger = Substitute.For<ILogger<StockMediaThoughts>>();

            var actual = Assert.Throws<ArgumentNullException>(() => new StockMediaThoughts(httpClientFactory, pexelsConfig, logger));

            Assert.Equal(nameof(httpClientFactory), actual.ParamName);
        }

        [Fact]
		public void Constructor_WithInvalidPexelsConfig_ShouldThrow()
		{
            IHttpClientFactory httpClientFactory = Substitute.For<IHttpClientFactory>();
            IOptions<PexelsConfig> pexelsConfig = null;
            ILogger<StockMediaThoughts> logger = Substitute.For<ILogger<StockMediaThoughts>>();

            var actual = Assert.Throws<ArgumentNullException>(() => new StockMediaThoughts(httpClientFactory, pexelsConfig, logger));

            Assert.Equal(nameof(pexelsConfig), actual.ParamName);
		}

        [Fact]
        public void Constructor_WithInvalidLogger_ShouldThrow()
        {
            var httpClientFactory = Substitute.For<IHttpClientFactory>();
            var pexelsConfig = Substitute.For<IOptions<PexelsConfig>>();
            pexelsConfig.Value.Returns(Config.SEMANTIC_CONFIG.PexelsConfig);
            ILogger<StockMediaThoughts> logger = null;

            var actual = Assert.Throws<ArgumentNullException>(() => new StockMediaThoughts(httpClientFactory, pexelsConfig, logger));

            Assert.Equal(nameof(logger), actual.ParamName);
        }

        [Fact]
        public void Constructor_WithValidParams_ShouldConstruct()
        {
            var httpClientFactory = Substitute.For<IHttpClientFactory>();
            var pexelsConfig = Substitute.For<IOptions<PexelsConfig>>();
            pexelsConfig.Value.Returns(Config.SEMANTIC_CONFIG.PexelsConfig);
            var logger = Substitute.For<ILogger<StockMediaThoughts>>();

            var actual = new StockMediaThoughts(httpClientFactory, pexelsConfig, logger);

            Assert.NotNull(actual);
        }

        [Fact]
        public async Task DownloadAndGetStockVideoAsync_WithInvalidSearchQuery_ShouldThrow()
        {
            IHttpClientFactory httpClientFactory = Substitute.For<IHttpClientFactory>();
            var pexelsConfig = Substitute.For<IOptions<PexelsConfig>>();
            pexelsConfig.Value.Returns(Config.SEMANTIC_CONFIG.PexelsConfig);
            var logger = Substitute.For<ILogger<StockMediaThoughts>>();
            var instance = new StockMediaThoughts(httpClientFactory, pexelsConfig, logger);
            string searchQuery = default;

            var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.DownloadAndGetStockVideoAsync(searchQuery));

            Assert.Equal(nameof(searchQuery), actual.ParamName);
        }

        [Fact(Skip = "Integration Test")]
        public async Task DownloadAndGetStockVideoAsync_WithValidInputs_ShouldRespond()
        {
            var httpClientFactory = Substitute.For<IHttpClientFactory>();
            httpClientFactory.CreateClient().Returns(new HttpClient());
            var pexelsConfig = Substitute.For<IOptions<PexelsConfig>>();
            pexelsConfig.Value.Returns(Config.SEMANTIC_CONFIG.PexelsConfig);
            var logger = Substitute.For<ILogger<StockMediaThoughts>>();
            var instance = new StockMediaThoughts(httpClientFactory, pexelsConfig, logger);
            string searchQuery = "cats";

            var actual = await instance.DownloadAndGetStockVideoAsync(searchQuery, "portrait");

            Assert.NotEmpty(actual);
        }
    }
}
