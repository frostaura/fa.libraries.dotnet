using FrostAura.Libraries.Semantic.Core.Thoughts.Chains.Cognitive;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using FrostAura.Libraries.Semantic.Core.Extensions.Configuration;
using FrostAura.Libraries.Semantic.Core.Interfaces.Data;

namespace Semantic.Core.Tests.Thoughts.Chains.Cognitive;

public class TextToSpeechChainTests
{
    [Fact]
    public void Constructor_WithInvalidServiceProvider_ShouldThrow()
    {
        IServiceProvider serviceProvider = null;
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        ILogger<TextToSpeechChain> logger = Substitute.For<ILogger<TextToSpeechChain>>();

        var actual = Assert.Throws<ArgumentNullException>(() => new TextToSpeechChain(serviceProvider, semanticKernelLanguageModels, logger));

        Assert.Equal(nameof(serviceProvider), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithInvalidISemanticKernelLanguageModelsDataAccess_ShouldThrow()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = null;
        ILogger<TextToSpeechChain> logger = Substitute.For<ILogger<TextToSpeechChain>>();

        var actual = Assert.Throws<ArgumentNullException>(() => new TextToSpeechChain(serviceProvider, semanticKernelLanguageModels, logger));

        Assert.Equal(nameof(semanticKernelLanguageModels), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithInvalidLogger_ShouldThrow()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        ILogger<TextToSpeechChain> logger = null;

        var actual = Assert.Throws<ArgumentNullException>(() => new TextToSpeechChain(serviceProvider, semanticKernelLanguageModels, logger));

        Assert.Equal(nameof(logger), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithValidParams_ShouldConstruct()
    {
        var serviceProvider = Substitute.For<IServiceProvider>();
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var logger = Substitute.For<ILogger<TextToSpeechChain>>();

        var actual = new TextToSpeechChain(serviceProvider, semanticKernelLanguageModels, logger);

        Assert.NotNull(actual);
        Assert.NotEmpty(actual.QueryExample);
        Assert.NotEmpty(actual.QueryInputExample);
        Assert.NotEmpty(actual.Reasoning);
        Assert.NotEmpty(actual.ChainOfThoughts);
    }

    [Fact]
    public async Task SpeakTextAndGetFilePathAsync_WithInvalidInput_ShouldThrow()
    {
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(out var configuration);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var logger = Substitute.For<ILogger<TextToSpeechChain>>();
        var instance = new TextToSpeechChain(serviceProvider, semanticKernelLanguageModels, logger);
        string text = default;

        var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.SpeakTextAndGetFilePathAsync(text));

        Assert.Equal(nameof(text), actual.ParamName);
    }

    [Fact]
    public async Task SpeakTextAndGetFilePathAsync_WithValidInput_ShouldCallInvokeAsync()
    {
        var userProxy = Substitute.For<IUserProxyDataAccess>();
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(out var configuration)
            .AddSingleton(userProxy);

        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var logger = Substitute.For<ILogger<TextToSpeechChain>>();
        var instance = new TextToSpeechChain(serviceProvider, semanticKernelLanguageModels, logger);
        var input = "Hey there! I am an AI here to do your bidding. Do you have any coding questions for me?";

        var actual = await instance.SpeakTextAndGetFilePathAsync(input);

        Assert.NotNull(actual);
    }
}
