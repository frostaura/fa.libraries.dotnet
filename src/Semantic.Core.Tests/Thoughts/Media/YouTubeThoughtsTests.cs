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

public class YouTubeThoughtsTests
{
    [Fact]
    public void Constructor_WithInvalidServiceProvider_ShouldThrow()
    {
        IServiceProvider serviceProvider = null;
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        IOptions<GoogleConfig> googleConfig = Substitute.For<IOptions<GoogleConfig>>();
        ILogger<YouTubeThoughts> logger = Substitute.For<ILogger<YouTubeThoughts>>();

        var actual = Assert.Throws<ArgumentNullException>(() => new YouTubeThoughts(serviceProvider, semanticKernelLanguageModels, googleConfig, logger));

        Assert.Equal(nameof(serviceProvider), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithInvalidISemanticKernelLanguageModelsDataAccess_ShouldThrow()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = null;
        IOptions<GoogleConfig> googleConfig = Substitute.For<IOptions<GoogleConfig>>();
        ILogger<YouTubeThoughts> logger = Substitute.For<ILogger<YouTubeThoughts>>();

        var actual = Assert.Throws<ArgumentNullException>(() => new YouTubeThoughts(serviceProvider, semanticKernelLanguageModels, googleConfig, logger));

        Assert.Equal(nameof(semanticKernelLanguageModels), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithInvalidGoogleConfig_ShouldThrow()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        IOptions<GoogleConfig> googleConfig = null;
        ILogger<YouTubeThoughts> logger = Substitute.For<ILogger<YouTubeThoughts>>();

        var actual = Assert.Throws<ArgumentNullException>(() => new YouTubeThoughts(serviceProvider, semanticKernelLanguageModels, googleConfig, logger));

        Assert.Equal(nameof(googleConfig), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithInvalidLogger_ShouldThrow()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        IOptions<GoogleConfig> googleConfig = Substitute.For<IOptions<GoogleConfig>>();
        ILogger<YouTubeThoughts> logger = null;

        var actual = Assert.Throws<ArgumentNullException>(() => new YouTubeThoughts(serviceProvider, semanticKernelLanguageModels, googleConfig, logger));

        Assert.Equal(nameof(logger), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithValidParams_ShouldConstruct()
    {
        var serviceProvider = Substitute.For<IServiceProvider>();
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var googleConfig = Substitute.For<IOptions<GoogleConfig>>();
        googleConfig.Value.Returns(Substitute.For<GoogleConfig>());
        var logger = Substitute.For<ILogger<YouTubeThoughts>>();

        var actual = new YouTubeThoughts(serviceProvider, semanticKernelLanguageModels, googleConfig, logger);

        Assert.NotNull(actual);
    }

    [Fact]
    public async Task PublishLocalVideoToYouTubeAsync_WithInvalidFilePath_ShouldThrow()
    {
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(Config.SEMANTIC_CONFIG);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var googleConfig = Substitute.For<IOptions<GoogleConfig>>();
        googleConfig.Value.Returns(Config.SEMANTIC_CONFIG.GoogleConfig);
        var logger = Substitute.For<ILogger<YouTubeThoughts>>();
        var instance = new YouTubeThoughts(serviceProvider, semanticKernelLanguageModels, googleConfig, logger);
        string filePath = default;
        string title = "test title";
        string categoryId = "1";
        string description = "test description";
        string tags = "hello world";

        var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.PublishLocalVideoToYouTubeAsync(filePath, title, categoryId, description, tags));

        Assert.Equal(nameof(filePath), actual.ParamName);
    }

    [Fact]
    public async Task PublishLocalVideoToYouTubeAsync_WithInvalidTitle_ShouldThrow()
    {
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(Config.SEMANTIC_CONFIG);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var googleConfig = Substitute.For<IOptions<GoogleConfig>>();
        googleConfig.Value.Returns(Config.SEMANTIC_CONFIG.GoogleConfig);
        var logger = Substitute.For<ILogger<YouTubeThoughts>>();
        var instance = new YouTubeThoughts(serviceProvider, semanticKernelLanguageModels, googleConfig, logger);
        string filePath = "test filename";
        string title = null;
        string categoryId = "1";
        string description = "test description";
        string tags = "hello world";

        var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.PublishLocalVideoToYouTubeAsync(filePath, title, categoryId, description, tags));

        Assert.Equal(nameof(title), actual.ParamName);
    }

    [Fact]
    public async Task PublishLocalVideoToYouTubeAsync_WithInvalidCategoryId_ShouldThrow()
    {
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(Config.SEMANTIC_CONFIG);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var googleConfig = Substitute.For<IOptions<GoogleConfig>>();
        googleConfig.Value.Returns(Config.SEMANTIC_CONFIG.GoogleConfig);
        var logger = Substitute.For<ILogger<YouTubeThoughts>>();
        var instance = new YouTubeThoughts(serviceProvider, semanticKernelLanguageModels, googleConfig, logger);
        string filePath = "test filename";
        string title = "test title";
        string categoryId = null;
        string description = "test description";
        string tags = "hello world";

        var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.PublishLocalVideoToYouTubeAsync(filePath, title, categoryId, description, tags));

        Assert.Equal(nameof(categoryId), actual.ParamName);
    }

    [Fact]
    public async Task PublishLocalVideoToYouTubeAsync_WithInvalidDescription_ShouldThrow()
    {
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(Config.SEMANTIC_CONFIG);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var googleConfig = Substitute.For<IOptions<GoogleConfig>>();
        googleConfig.Value.Returns(Config.SEMANTIC_CONFIG.GoogleConfig);
        var logger = Substitute.For<ILogger<YouTubeThoughts>>();
        var instance = new YouTubeThoughts(serviceProvider, semanticKernelLanguageModels, googleConfig, logger);
        string filePath = "test filename";
        string title = "test title";
        string categoryId = "1";
        string description = default;
        string tags = "hello world";

        var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.PublishLocalVideoToYouTubeAsync(filePath, title, categoryId, description, tags));

        Assert.Equal(nameof(description), actual.ParamName);
    }

    [Fact]
    public async Task PublishLocalVideoToYouTubeAsync_WithInvalidTags_ShouldThrow()
    {
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(Config.SEMANTIC_CONFIG);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var googleConfig = Substitute.For<IOptions<GoogleConfig>>();
        googleConfig.Value.Returns(Config.SEMANTIC_CONFIG.GoogleConfig);
        var logger = Substitute.For<ILogger<YouTubeThoughts>>();
        var instance = new YouTubeThoughts(serviceProvider, semanticKernelLanguageModels, googleConfig, logger);
        string filePath = "dummy filename";
        string title = "test title";
        string categoryId = "1";
        string description = "test description";
        string tags = default;

        var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.PublishLocalVideoToYouTubeAsync(filePath, title, categoryId, description, tags));

        Assert.Equal(nameof(tags), actual.ParamName);
    }

    [Fact(Skip = "This test requires an OAuth token which is an interactive flow. This test is to demonstrate how to use the thought.")]
    public async Task PublishLocalVideoToYouTubeAsync_WithValidTags_ShouldThrow()
    {
        var userProxy = Substitute.For<IUserProxyDataAccess>();
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(Config.SEMANTIC_CONFIG)
            .AddSingleton(userProxy);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var googleConfig = Substitute.For<IOptions<GoogleConfig>>();
        googleConfig.Value.Returns(Config.SEMANTIC_CONFIG.GoogleConfig);
        var logger = Substitute.For<ILogger<YouTubeThoughts>>();
        var instance = new YouTubeThoughts(serviceProvider, semanticKernelLanguageModels, googleConfig, logger);
        var filePath = "videos/6907538454af4d90b4832fa31fdaf070.mp4";
        var title = "The Amazingly-wild Cosmos!";
        var categoryId = "28"; // Sience & Technology
        var description = "Embark on a thrilling cosmic journey as we unravel the mysteries of Mars, the Red Planet. Discover its unique features, from its distinct reddish hue to the largest volcano and deepest canyon in the solar system. Despite its harsh climate, evidence suggests that Mars may have once harbored life. This exploration is not just a space journey, but a voyage back in time, offering insights into the early solar system and the potential for life beyond Earth.";
        var tags = "mars-exploration space-journey red-planet roman-god-of-war iron-oxide olympus-mons valles-marineris mars-climate ancient-rivers-on-mars life-on-other-planets";

        var actual = await instance.PublishLocalVideoToYouTubeAsync(filePath, title, categoryId, description, tags);

        Assert.NotEmpty(actual);
    }
}
