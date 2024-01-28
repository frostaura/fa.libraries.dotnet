using FrostAura.Libraries.Semantic.Core.Extensions.Configuration;
using FrostAura.Libraries.Semantic.Core.Interfaces.Data;
using FrostAura.Libraries.Semantic.Core.Models.Configuration;
using FrostAura.Libraries.Semantic.Core.Thoughts.Media;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Semantic.Core.Tests.Data;
using Xunit;

namespace Semantic.Core.Tests.Thoughts.Media;

public class MediumThoughtsTests
{
    [Fact]
    public void Constructor_WithInvalidServiceProvider_ShouldThrow()
    {
        IServiceProvider serviceProvider = null;
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        IHttpClientFactory httpClientFactory = Substitute.For<IHttpClientFactory>();
        IOptions<MediumConfig> mediumConfig = Substitute.For<IOptions<MediumConfig>>();
        ILogger<MediumThoughts> logger = Substitute.For<ILogger<MediumThoughts>>();

        var actual = Assert.Throws<ArgumentNullException>(() => new MediumThoughts(serviceProvider, semanticKernelLanguageModels, httpClientFactory, mediumConfig, logger));

        Assert.Equal(nameof(serviceProvider), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithInvalidISemanticKernelLanguageModelsDataAccess_ShouldThrow()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = null;
        IHttpClientFactory httpClientFactory = Substitute.For<IHttpClientFactory>();
        IOptions<MediumConfig> mediumConfig = Substitute.For<IOptions<MediumConfig>>();
        ILogger<MediumThoughts> logger = Substitute.For<ILogger<MediumThoughts>>();

        var actual = Assert.Throws<ArgumentNullException>(() => new MediumThoughts(serviceProvider, semanticKernelLanguageModels, httpClientFactory, mediumConfig, logger));

        Assert.Equal(nameof(semanticKernelLanguageModels), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithInvalidHttpClientFactory_ShouldThrow()
    {
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(out var configuration);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        IHttpClientFactory httpClientFactory = null;
        var mediumConfig = serviceProvider.GetRequiredService<IOptions<MediumConfig>>();
        ILogger<MediumThoughts> logger = Substitute.For<ILogger<MediumThoughts>>();

        var actual = Assert.Throws<ArgumentNullException>(() => new MediumThoughts(serviceProvider, semanticKernelLanguageModels, httpClientFactory, mediumConfig, logger));

        Assert.Equal(nameof(httpClientFactory), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithInvalidMediumConfig_ShouldThrow()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        IHttpClientFactory httpClientFactory = Substitute.For<IHttpClientFactory>();
        IOptions<MediumConfig> mediumConfig = null;
        ILogger<MediumThoughts> logger = Substitute.For<ILogger<MediumThoughts>>();

        var actual = Assert.Throws<ArgumentNullException>(() => new MediumThoughts(serviceProvider, semanticKernelLanguageModels, httpClientFactory, mediumConfig, logger));

        Assert.Equal(nameof(mediumConfig), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithInvalidLogger_ShouldThrow()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        IHttpClientFactory httpClientFactory = Substitute.For<IHttpClientFactory>();
        IOptions<MediumConfig> mediumConfig = Substitute.For<IOptions<MediumConfig>>();
        ILogger<MediumThoughts> logger = null;

        var actual = Assert.Throws<ArgumentNullException>(() => new MediumThoughts(serviceProvider, semanticKernelLanguageModels, httpClientFactory, mediumConfig, logger));

        Assert.Equal(nameof(logger), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithValidParams_ShouldConstruct()
    {
        var serviceProvider = Substitute.For<IServiceProvider>();
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        var mediumConfig = new MediumConfig { Token = "test" };
        var mediumConfigOptions = Substitute.For<IOptions<MediumConfig>>();
        mediumConfigOptions.Value.Returns(mediumConfig);
        var logger = Substitute.For<ILogger<MediumThoughts>>();

        var actual = new MediumThoughts(serviceProvider, semanticKernelLanguageModels, httpClientFactory, mediumConfigOptions, logger);

        Assert.NotNull(actual);
    }

    [Fact]
    public async Task PostMediumBlogHTMLAsync_WithInvalidTitle_ShouldThrow()
    {
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(out var configuration)
            .AddSingleton<IUserProxyDataAccess, ConsoleUserAgentProxy>(); ;
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        IHttpClientFactory httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
        var mediumConfig = serviceProvider.GetRequiredService<IOptions<MediumConfig>>();
        var logger = Substitute.For<ILogger<MediumThoughts>>();
        var instance = new MediumThoughts(serviceProvider, semanticKernelLanguageModels, httpClientFactory, mediumConfig, logger);
        string title = default;
        var content = "<hr>";
        var tags = "['hello', 'world']";

        var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.PostMediumBlogHTMLAsync(title, content, tags));

        Assert.Equal(nameof(title), actual.ParamName);
    }

    [Fact]
    public async Task PostMediumBlogHTMLAsync_WithInvalidContent_ShouldThrow()
    {
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(out var configuration)
            .AddSingleton<IUserProxyDataAccess, ConsoleUserAgentProxy>();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        IHttpClientFactory httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
        var mediumConfig = serviceProvider.GetRequiredService<IOptions<MediumConfig>>();
        var logger = Substitute.For<ILogger<MediumThoughts>>();
        var instance = new MediumThoughts(serviceProvider, semanticKernelLanguageModels, httpClientFactory, mediumConfig, logger);
        var title = "Test Article";
        string content = default;
        var tags = "['hello', 'world']";

        var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.PostMediumBlogHTMLAsync(title, content, tags));

        Assert.Equal(nameof(content), actual.ParamName);
    }

    [Fact]
    public async Task PostMediumBlogHTMLAsync_WithInvalidTags_ShouldThrow()
    {
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(out var configuration)
            .AddSingleton<IUserProxyDataAccess, ConsoleUserAgentProxy>(); ;
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        IHttpClientFactory httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
        var mediumConfig = serviceProvider.GetRequiredService<IOptions<MediumConfig>>();
        var logger = Substitute.For<ILogger<MediumThoughts>>();
        var instance = new MediumThoughts(serviceProvider, semanticKernelLanguageModels, httpClientFactory, mediumConfig, logger);
        var title = "Test Article";
        var content = "<hr>";
        string tags = default;

        var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.PostMediumBlogHTMLAsync(title, content, tags));

        Assert.Equal(nameof(tags), actual.ParamName);
    }

    [Fact(Skip = "Integration Test")]
    public async Task PostMediumBlogHTMLAsync_WithValidInputs_ShouldRespond()
    {
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(out var configuration)
            .AddSingleton<IUserProxyDataAccess, ConsoleUserAgentProxy>(); ;
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        httpClientFactory.CreateClient().Returns(new HttpClient());
        var mediumConfig = serviceProvider.GetRequiredService<IOptions<MediumConfig>>();
        var logger = Substitute.For<ILogger<MediumThoughts>>();
        var instance = new MediumThoughts(serviceProvider, semanticKernelLanguageModels, httpClientFactory, mediumConfig, logger);
        var title = "Test Article";
        var content = "<hr>";
        var tags = "['hello', 'world']";

        var actual = await instance.PostMediumBlogHTMLAsync(title, content, tags);

        Assert.NotEmpty(actual);
    }
}
