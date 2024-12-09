using FrostAura.Libraries.Semantic.Core.Thoughts.Chains.Cognitive;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using FrostAura.Libraries.Semantic.Core.Extensions.Configuration;
using FrostAura.Libraries.Semantic.Core.Interfaces.Data;

namespace Semantic.Core.Tests.Thoughts.Chains.Cognitive;

public class AudioTranscriptionChainTests
{
    [Fact]
    public void Constructor_WithInvalidServiceProvider_ShouldThrow()
    {
        IServiceProvider serviceProvider = null;
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        ILogger<AudioTranscriptionChain> logger = Substitute.For<ILogger<AudioTranscriptionChain>>();

        var actual = Assert.Throws<ArgumentNullException>(() => new AudioTranscriptionChain(serviceProvider, semanticKernelLanguageModels, logger));

        Assert.Equal(nameof(serviceProvider), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithInvalidISemanticKernelLanguageModelsDataAccess_ShouldThrow()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = null;
        ILogger<AudioTranscriptionChain> logger = Substitute.For<ILogger<AudioTranscriptionChain>>();

        var actual = Assert.Throws<ArgumentNullException>(() => new AudioTranscriptionChain(serviceProvider, semanticKernelLanguageModels, logger));

        Assert.Equal(nameof(semanticKernelLanguageModels), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithInvalidLogger_ShouldThrow()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        ILogger<AudioTranscriptionChain> logger = null;

        var actual = Assert.Throws<ArgumentNullException>(() => new AudioTranscriptionChain(serviceProvider, semanticKernelLanguageModels, logger));

        Assert.Equal(nameof(logger), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithValidParams_ShouldConstruct()
    {
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(out var configuration);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var logger = Substitute.For<ILogger<AudioTranscriptionChain>>();

        var actual = new AudioTranscriptionChain(serviceProvider, semanticKernelLanguageModels, logger);

        Assert.NotNull(actual);
        Assert.NotEmpty(actual.QueryExample);
        Assert.NotEmpty(actual.QueryInputExample);
        Assert.NotEmpty(actual.Reasoning);
        Assert.NotEmpty(actual.ChainOfThoughts);
    }

    [Fact]
    public async Task TranscribeAudioFileAsync_WithInvalidInput_ShouldThrow()
    {
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(out var configuration);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var logger = Substitute.For<ILogger<AudioTranscriptionChain>>();
        var instance = new AudioTranscriptionChain(serviceProvider, semanticKernelLanguageModels, logger);
        string audioFilePath = default;

        var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.TranscribeAudioFileAsync(audioFilePath));

        Assert.Equal(nameof(audioFilePath), actual.ParamName);
    }

    [Fact]
    public async Task TranscribeAudioFileAsync_WithValidInput_ShouldCallInvokeAsync()
    {
        var userProxy = Substitute.For<IUserProxyDataAccess>();
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(out var configuration)
            .AddSingleton(userProxy);

        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var logger = Substitute.For<ILogger<AudioTranscriptionChain>>();
        var instance = new AudioTranscriptionChain(serviceProvider, semanticKernelLanguageModels, logger);
        var input = "./harvard.wav";
        var expected = "The stale smell of old beer lingers. It takes heat to bring out the odor. A cold dip restores health and zest. A salt pickle tastes fine with ham. Tacos al pastor are my favorite. A zestful food is the hot cross bun.";

        var actual = await instance.TranscribeAudioFileAsync(input);

        Assert.Contains(expected, actual);
    }

    [Fact]
    public async Task TranscribeAudioFilesAsync_WithValidInput_ShouldCallInvokeAsync()
    {
        var userProxy = Substitute.For<IUserProxyDataAccess>();
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(out var configuration)
            .AddSingleton(userProxy);

        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var logger = Substitute.For<ILogger<AudioTranscriptionChain>>();
        var instance = new AudioTranscriptionChain(serviceProvider, semanticKernelLanguageModels, logger);
        var input = "[\"./harvard.wav\", \"./harvard2.wav\"]";
        var expected = "[\"The stale smell of old beer lingers. It takes heat to bring out the odor. A cold dip restores health and zest. A salt pickle tastes fine with ham. Tacos al pastor are my favorite. A zestful food is the hot cross bun.\", \"The quick brown fox jumps over the lazy dog.\"]";

        var actual = await instance.TranscribeAudioFilesAsync(input);

        Assert.Contains(expected, actual);
    }
}
