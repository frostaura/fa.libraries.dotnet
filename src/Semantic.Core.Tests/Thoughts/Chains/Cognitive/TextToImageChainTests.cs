using FrostAura.Libraries.Semantic.Core.Thoughts.Chains.Cognitive;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using FrostAura.Libraries.Semantic.Core.Extensions.Configuration;
using FrostAura.Libraries.Semantic.Core.Interfaces.Data;

namespace Semantic.Core.Tests.Thoughts.Chains.Cognitive;

public class TextToImageChainTests
{
    [Fact]
    public void Constructor_WithInvalidServiceProvider_ShouldThrow()
    {
        IServiceProvider serviceProvider = null;
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        ILogger<TextToImageChain> logger = Substitute.For<ILogger<TextToImageChain>>();

        var actual = Assert.Throws<ArgumentNullException>(() => new TextToImageChain(serviceProvider, semanticKernelLanguageModels, logger));

        Assert.Equal(nameof(serviceProvider), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithInvalidISemanticKernelLanguageModelsDataAccess_ShouldThrow()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = null;
        ILogger<TextToImageChain> logger = Substitute.For<ILogger<TextToImageChain>>();

        var actual = Assert.Throws<ArgumentNullException>(() => new TextToImageChain(serviceProvider, semanticKernelLanguageModels, logger));

        Assert.Equal(nameof(semanticKernelLanguageModels), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithInvalidLogger_ShouldThrow()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        ILogger<TextToImageChain> logger = null;

        var actual = Assert.Throws<ArgumentNullException>(() => new TextToImageChain(serviceProvider, semanticKernelLanguageModels, logger));

        Assert.Equal(nameof(logger), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithValidParams_ShouldConstruct()
    {
        var serviceProvider = Substitute.For<IServiceProvider>();
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var logger = Substitute.For<ILogger<TextToImageChain>>();

        var actual = new TextToImageChain(serviceProvider, semanticKernelLanguageModels, logger);

        Assert.NotNull(actual);
        Assert.NotEmpty(actual.QueryExample);
        Assert.NotEmpty(actual.QueryInputExample);
        Assert.NotEmpty(actual.Reasoning);
        Assert.NotEmpty(actual.ChainOfThoughts);
    }

    [Fact]
    public async Task GenerateImageAndGetFilePathAsync_WithInvalidInput_ShouldThrow()
    {
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(Config.SEMANTIC_CONFIG);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var logger = Substitute.For<ILogger<TextToImageChain>>();
        var instance = new TextToImageChain(serviceProvider, semanticKernelLanguageModels, logger);
        string prompt = default;

        var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.GenerateImageAndGetFilePathAsync(prompt));

        Assert.Equal(nameof(prompt), actual.ParamName);
    }

    [Fact]
    public async Task GenerateImageAndGetFilePathAsync_WithValidInput_ShouldCallInvokeAsync()
    {
        var userProxy = Substitute.For<IUserProxyDataAccess>();
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(Config.SEMANTIC_CONFIG)
            .AddSingleton(userProxy);

        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var logger = Substitute.For<ILogger<TextToImageChain>>();
        var instance = new TextToImageChain(serviceProvider, semanticKernelLanguageModels, logger);
        var prompt = "A surfer in a hurricane, fighting off sharks that are on fire, photo-realistic, motion blur.";

        var actual = await instance.GenerateImageAndGetFilePathAsync(prompt);

        Assert.NotNull(actual);
    }
}
