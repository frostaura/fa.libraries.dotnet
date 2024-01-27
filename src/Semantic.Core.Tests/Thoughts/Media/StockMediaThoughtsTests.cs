using FrostAura.Libraries.Semantic.Core.Extensions.Configuration;
using FrostAura.Libraries.Semantic.Core.Interfaces.Data;
using FrostAura.Libraries.Semantic.Core.Models.Configuration;
using FrostAura.Libraries.Semantic.Core.Thoughts.Media;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Semantic.Core.Tests.Thoughts.Media;

public class StockMediaThoughtsTests
{
    [Fact]
    public void Constructor_WithInvalidServiceProvider_ShouldThrow()
    {
        IServiceProvider serviceProvider = null;
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        IHttpClientFactory httpClientFactory = Substitute.For<IHttpClientFactory>();
        IOptions<PexelsConfig> pexelsConfig = Substitute.For<IOptions<PexelsConfig>>();
        ILogger<StockMediaThoughts> logger = Substitute.For<ILogger<StockMediaThoughts>>();

        var actual = Assert.Throws<ArgumentNullException>(() => new StockMediaThoughts(serviceProvider, semanticKernelLanguageModels, httpClientFactory, pexelsConfig, logger));

        Assert.Equal(nameof(serviceProvider), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithInvalidISemanticKernelLanguageModelsDataAccess_ShouldThrow()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = null;
        IHttpClientFactory httpClientFactory = Substitute.For<IHttpClientFactory>();
        IOptions<PexelsConfig> pexelsConfig = Substitute.For<IOptions<PexelsConfig>>();
        ILogger<StockMediaThoughts> logger = Substitute.For<ILogger<StockMediaThoughts>>();

        var actual = Assert.Throws<ArgumentNullException>(() => new StockMediaThoughts(serviceProvider, semanticKernelLanguageModels, httpClientFactory, pexelsConfig, logger));

        Assert.Equal(nameof(semanticKernelLanguageModels), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithInvalidHttpClientFactory_ShouldThrow()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        IHttpClientFactory httpClientFactory = null;
        IOptions<PexelsConfig> pexelsConfig = Substitute.For<IOptions<PexelsConfig>>();
        pexelsConfig.Value.Returns(Config.SEMANTIC_CONFIG.PexelsConfig);
        ILogger<StockMediaThoughts> logger = Substitute.For<ILogger<StockMediaThoughts>>();

        var actual = Assert.Throws<ArgumentNullException>(() => new StockMediaThoughts(serviceProvider, semanticKernelLanguageModels, httpClientFactory, pexelsConfig, logger));

        Assert.Equal(nameof(httpClientFactory), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithInvalidPexelsConfig_ShouldThrow()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        IHttpClientFactory httpClientFactory = Substitute.For<IHttpClientFactory>();
        IOptions<PexelsConfig> pexelsConfig = null;
        ILogger<StockMediaThoughts> logger = Substitute.For<ILogger<StockMediaThoughts>>();

        var actual = Assert.Throws<ArgumentNullException>(() => new StockMediaThoughts(serviceProvider, semanticKernelLanguageModels, httpClientFactory, pexelsConfig, logger));

        Assert.Equal(nameof(pexelsConfig), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithInvalidLogger_ShouldThrow()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        IHttpClientFactory httpClientFactory = Substitute.For<IHttpClientFactory>();
        IOptions<PexelsConfig> pexelsConfig = Substitute.For<IOptions<PexelsConfig>>();
        ILogger<StockMediaThoughts> logger = null;

        var actual = Assert.Throws<ArgumentNullException>(() => new StockMediaThoughts(serviceProvider, semanticKernelLanguageModels, httpClientFactory, pexelsConfig, logger));

        Assert.Equal(nameof(logger), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithValidParams_ShouldConstruct()
    {
        var serviceProvider = Substitute.For<IServiceProvider>();
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        var pexelsConfig = new PexelsConfig { ApiKey = "test" };
        var pexelsConfigOptions = Substitute.For<IOptions<PexelsConfig>>();
        pexelsConfigOptions.Value.Returns(pexelsConfig);
        var logger = Substitute.For<ILogger<StockMediaThoughts>>();

        var actual = new StockMediaThoughts(serviceProvider, semanticKernelLanguageModels, httpClientFactory, pexelsConfigOptions, logger);

        Assert.NotNull(actual);
    }

    [Fact]
    public async Task DownloadAndGetStockVideoAsync_WithInvalidSearchQuery_ShouldThrow()
    {
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(out var configuration);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        IHttpClientFactory httpClientFactory = Substitute.For<IHttpClientFactory>();
        var pexelsConfig = Substitute.For<IOptions<PexelsConfig>>();
        pexelsConfig.Value.Returns(Config.SEMANTIC_CONFIG.PexelsConfig);
        var logger = Substitute.For<ILogger<StockMediaThoughts>>();
        var instance = new StockMediaThoughts(serviceProvider, semanticKernelLanguageModels, httpClientFactory, pexelsConfig, logger);
        string searchQuery = default;

        var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.DownloadAndGetStockVideoAsync(searchQuery));

        Assert.Equal(nameof(searchQuery), actual.ParamName);
    }

    [Fact(Skip = "Integration Test")]
    public async Task DownloadAndGetStockVideoAsync_WithValidInputs_ShouldRespond()
    {
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(out var configuration);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        httpClientFactory.CreateClient().Returns(new HttpClient());
        var pexelsConfig = Substitute.For<IOptions<PexelsConfig>>();
        pexelsConfig.Value.Returns(Config.SEMANTIC_CONFIG.PexelsConfig);
        var logger = Substitute.For<ILogger<StockMediaThoughts>>();
        var instance = new StockMediaThoughts(serviceProvider, semanticKernelLanguageModels, httpClientFactory, pexelsConfig, logger);
        string searchQuery = "cats";

        var actual = await instance.DownloadAndGetStockVideoAsync(searchQuery, "portrait");

        Assert.NotEmpty(actual);
    }
}
