using FrostAura.Libraries.Semantic.Core.Thoughts.Chains.Finance;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using FrostAura.Libraries.Semantic.Core.Extensions.Configuration;
using FrostAura.Libraries.Semantic.Core.Interfaces.Data;

namespace Semantic.Core.Tests.Thoughts.Chains.Finance;

public class ForecastSearchChainTests
{
    [Fact]
    public void Constructor_WithInvalidServiceProvider_ShouldThrow()
    {
        IServiceProvider serviceProvider = null;
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        ILogger<ForecastSearchChain> logger = Substitute.For<ILogger<ForecastSearchChain>>();

        var actual = Assert.Throws<ArgumentNullException>(() => new ForecastSearchChain(serviceProvider, semanticKernelLanguageModels, logger));

        Assert.Equal(nameof(serviceProvider), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithInvalidISemanticKernelLanguageModelsDataAccess_ShouldThrow()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = null;
        ILogger<ForecastSearchChain> logger = Substitute.For<ILogger<ForecastSearchChain>>();

        var actual = Assert.Throws<ArgumentNullException>(() => new ForecastSearchChain(serviceProvider, semanticKernelLanguageModels, logger));

        Assert.Equal(nameof(semanticKernelLanguageModels), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithInvalidLogger_ShouldThrow()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        ILogger<ForecastSearchChain> logger = null;

        var actual = Assert.Throws<ArgumentNullException>(() => new ForecastSearchChain(serviceProvider, semanticKernelLanguageModels, logger));

        Assert.Equal(nameof(logger), actual.ParamName);
    }

    [Fact(Skip = "Integration Test")]
    public async Task GetForecastAsync_WithValidInput_ShouldReturnForecastAsync()
    {
        var userProxy = Substitute.For<IUserProxyDataAccess>();
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(out var configuration)
            .AddSingleton(userProxy);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var logger = Substitute.For<ILogger<ForecastSearchChain>>();
        var instance = new ForecastSearchChain(serviceProvider, semanticKernelLanguageModels, logger);
        var assetName = "Bitcoin";
        var timeframe = "for today";

        var actual = await instance.GetForecastAsync(assetName, timeframe);

        Assert.NotEmpty(actual);
    }
}
