using FrostAura.Libraries.Semantic.Core.Extensions.Configuration;
using FrostAura.Libraries.Semantic.Core.Interfaces.Data;
using FrostAura.Libraries.Semantic.Core.Thoughts.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Semantic.Core.Tests.Thoughts.IO;

public class HttpThoughtsTests
{
    [Fact]
    public void Constructor_WithInvalidServiceProvider_ShouldThrow()
    {
        IServiceProvider serviceProvider = null;
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        IHttpClientFactory httpClientFactory = Substitute.For<IHttpClientFactory>();
        ILogger<HttpThoughts> logger = Substitute.For<ILogger<HttpThoughts>>();

        var actual = Assert.Throws<ArgumentNullException>(() => new HttpThoughts(serviceProvider, semanticKernelLanguageModels, httpClientFactory, logger));

        Assert.Equal(nameof(serviceProvider), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithInvalidISemanticKernelLanguageModelsDataAccess_ShouldThrow()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = null;
        IHttpClientFactory httpClientFactory = Substitute.For<IHttpClientFactory>();
        ILogger<HttpThoughts> logger = Substitute.For<ILogger<HttpThoughts>>();

        var actual = Assert.Throws<ArgumentNullException>(() => new HttpThoughts(serviceProvider, semanticKernelLanguageModels, httpClientFactory, logger));

        Assert.Equal(nameof(semanticKernelLanguageModels), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithInvalidLogger_ShouldThrow()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        IHttpClientFactory httpClientFactory = Substitute.For<IHttpClientFactory>();
        ILogger<HttpThoughts> logger = null;

        var actual = Assert.Throws<ArgumentNullException>(() => new HttpThoughts(serviceProvider, semanticKernelLanguageModels, httpClientFactory, logger));

        Assert.Equal(nameof(logger), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithInvalidHttpClientFactory_ShouldThrow()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        IHttpClientFactory httpClientFactory = null;
        ILogger<HttpThoughts> logger = Substitute.For<ILogger<HttpThoughts>>();

        var actual = Assert.Throws<ArgumentNullException>(() => new HttpThoughts(serviceProvider, semanticKernelLanguageModels, httpClientFactory, logger));

        Assert.Equal(nameof(httpClientFactory), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithValidParams_ShouldConstruct()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        IHttpClientFactory httpClientFactory = Substitute.For<IHttpClientFactory>();
        var logger = Substitute.For<ILogger<HttpThoughts>>();

        var actual = new HttpThoughts(serviceProvider, semanticKernelLanguageModels, httpClientFactory, logger);

        Assert.NotNull(actual);
    }

    [Fact]
    public async Task GetAsync_WithInvalidInput_ShouldThrow()
    {
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(Config.SEMANTIC_CONFIG);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        var logger = Substitute.For<ILogger<HttpThoughts>>();
        var instance = new HttpThoughts(serviceProvider, semanticKernelLanguageModels, httpClientFactory, logger);
        string uri = default;

        var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.GetAsync(uri));

        Assert.Equal(nameof(uri), actual.ParamName);
    }

    [Fact]
    public async Task GetAsync_WithValidInput_ShouldRespond()
    {
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(Config.SEMANTIC_CONFIG);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        httpClientFactory.CreateClient().Returns(new HttpClient());
        var logger = Substitute.For<ILogger<HttpThoughts>>();
        var instance = new HttpThoughts(serviceProvider, semanticKernelLanguageModels, httpClientFactory, logger);
        var uri = "https://www.google.com.au/search?q=Latest%20trending%20book%20topics";

        var actual = await instance.GetAsync(uri);

        Assert.NotEmpty(actual);
    }
}
