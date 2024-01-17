using FrostAura.Libraries.Semantic.Core.Thoughts.Chains.Cognitive;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using FrostAura.Libraries.Semantic.Core.Extensions.Configuration;
using FrostAura.Libraries.Semantic.Core.Interfaces.Data;

namespace Semantic.Core.Tests.Thoughts.Chains.Finance;

public class GetFNBAccountBalancesChainTests
{
    [Fact]
    public void Constructor_WithInvalidServiceProvider_ShouldThrow()
    {
        IServiceProvider serviceProvider = null;
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        ILogger<GetFNBAccountBalancesChain> logger = Substitute.For<ILogger<GetFNBAccountBalancesChain>>();

        var actual = Assert.Throws<ArgumentNullException>(() => new GetFNBAccountBalancesChain(serviceProvider, semanticKernelLanguageModels, logger));

        Assert.Equal(nameof(serviceProvider), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithInvalidISemanticKernelLanguageModelsDataAccess_ShouldThrow()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = null;
        ILogger<GetFNBAccountBalancesChain> logger = Substitute.For<ILogger<GetFNBAccountBalancesChain>>();

        var actual = Assert.Throws<ArgumentNullException>(() => new GetFNBAccountBalancesChain(serviceProvider, semanticKernelLanguageModels, logger));

        Assert.Equal(nameof(semanticKernelLanguageModels), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithInvalidLogger_ShouldThrow()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        ILogger<GetFNBAccountBalancesChain> logger = null;

        var actual = Assert.Throws<ArgumentNullException>(() => new GetFNBAccountBalancesChain(serviceProvider, semanticKernelLanguageModels, logger));

        Assert.Equal(nameof(logger), actual.ParamName);
    }

    //[Fact(Skip = "Integration Test")]
    [Fact]
    public async Task GetFNBAccountBalancesTableAsync_WithValidInput_ShouldReturnBalancesAsync()
    {
        var userProxy = Substitute.For<IUserProxyDataAccess>();
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(Config.SEMANTIC_CONFIG)
            .AddSingleton(userProxy);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var logger = Substitute.For<ILogger<GetFNBAccountBalancesChain>>();
        var instance = new GetFNBAccountBalancesChain(serviceProvider, semanticKernelLanguageModels, logger);

        var actual = await instance.GetFNBAccountBalancesTableAsync();

        Assert.NotEmpty(actual);
    }
}
