using FrostAura.Libraries.Semantic.Core.Models.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using FrostAura.Libraries.Semantic.Core.Extensions.Configuration;
using NSubstitute;
using Xunit;
using FrostAura.Libraries.Semantic.Core.Interfaces.Data;
using FrostAura.Libraries.Semantic.Core.Thoughts.Finance;

namespace Semantic.Core.Tests.Thoughts.Finance;

public class FNBThoughtsTests
{
    [Fact]
    public void Constructor_WithInvalidServiceProvider_ShouldThrow()
    {
        IServiceProvider serviceProvider = null;
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        IOptions<FNBConfig> fnbOptions = Substitute.For<IOptions<FNBConfig>>();
        ILogger<FNBThoughts> logger = Substitute.For<ILogger<FNBThoughts>>();

        var actual = Assert.Throws<ArgumentNullException>(() => new FNBThoughts(serviceProvider, semanticKernelLanguageModels, fnbOptions, logger));

        Assert.Equal(nameof(serviceProvider), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithInvalidISemanticKernelLanguageModelsDataAccess_ShouldThrow()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = null;
        IOptions<FNBConfig> fnbOptions = Substitute.For<IOptions<FNBConfig>>();
        ILogger<FNBThoughts> logger = Substitute.For<ILogger<FNBThoughts>>();

        var actual = Assert.Throws<ArgumentNullException>(() => new FNBThoughts(serviceProvider, semanticKernelLanguageModels, fnbOptions, logger));

        Assert.Equal(nameof(semanticKernelLanguageModels), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithInvalidLogger_ShouldThrow()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        IOptions<FNBConfig> fnbOptions = Substitute.For<IOptions<FNBConfig>>();
        ILogger<FNBThoughts> logger = null;

        var actual = Assert.Throws<ArgumentNullException>(() => new FNBThoughts(serviceProvider, semanticKernelLanguageModels, fnbOptions, logger));

        Assert.Equal(nameof(logger), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithInvalidFNBConfig_ShouldThrow()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        IOptions<FNBConfig> fnbOptions = null;
        ILogger<FNBThoughts> logger = Substitute.For<ILogger<FNBThoughts>>();

        var actual = Assert.Throws<ArgumentNullException>(() => new FNBThoughts(serviceProvider, semanticKernelLanguageModels, fnbOptions, logger));

        Assert.Equal(nameof(fnbOptions), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithValidParams_ShouldConstruct()
    {
        var serviceProvider = Substitute.For<IServiceProvider>();
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var fnbOptions = Substitute.For<IOptions<FNBConfig>>();
        fnbOptions.Value.Returns(Substitute.For<FNBConfig>());
        var logger = Substitute.For<ILogger<FNBThoughts>>();

        var actual = new FNBThoughts(serviceProvider, semanticKernelLanguageModels, fnbOptions, logger);

        Assert.NotNull(actual);
    }

    [Fact(Skip = "Integration Test")]
    public async Task LoadTextAsync_WithValidInput_ShouldRespond()
    {
        var fnbOptions = Substitute.For<IOptions<FNBConfig>>();
        fnbOptions.Value.Returns(Config.SEMANTIC_CONFIG.FNBConfig);
        var logger = Substitute.For<ILogger<FNBThoughts>>();
        var userProxy = Substitute.For<IUserProxyDataAccess>();
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(out var configuration)
            .AddSingleton(userProxy);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var instance = new FNBThoughts(serviceProvider, semanticKernelLanguageModels, fnbOptions, logger);

        var actual = await instance.GetFNBAccountBalancesRawAsync();

        Assert.NotEmpty(actual);
    }
}
