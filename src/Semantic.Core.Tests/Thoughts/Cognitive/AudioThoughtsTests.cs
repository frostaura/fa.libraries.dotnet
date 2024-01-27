using FrostAura.Libraries.Semantic.Core.Extensions.Configuration;
using FrostAura.Libraries.Semantic.Core.Interfaces.Data;
using FrostAura.Libraries.Semantic.Core.Models.Configuration;
using FrostAura.Libraries.Semantic.Core.Thoughts.Cognitive;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Semantic.Core.Tests.Thoughts.Cognitive;

public class AudioThoughtsTests
{
    [Fact]
    public void Constructor_WithInvalidServiceProvider_ShouldThrow()
    {
        IServiceProvider serviceProvider = null;
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        IOptions<ElevenLabsConfig> elevenLabsConfig = Substitute.For<IOptions<ElevenLabsConfig>>();
        ILogger<AudioThoughts> logger = Substitute.For<ILogger<AudioThoughts>>();

        var actual = Assert.Throws<ArgumentNullException>(() => new AudioThoughts(serviceProvider, semanticKernelLanguageModels, elevenLabsConfig, logger));

        Assert.Equal(nameof(serviceProvider), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithInvalidISemanticKernelLanguageModelsDataAccess_ShouldThrow()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = null;
        IOptions<ElevenLabsConfig> elevenLabsConfig = Substitute.For<IOptions<ElevenLabsConfig>>();
        ILogger<AudioThoughts> logger = Substitute.For<ILogger<AudioThoughts>>();

        var actual = Assert.Throws<ArgumentNullException>(() => new AudioThoughts(serviceProvider, semanticKernelLanguageModels, elevenLabsConfig, logger));

        Assert.Equal(nameof(semanticKernelLanguageModels), actual.ParamName);
    }

    [Fact]
		public void Constructor_WithInvalidElevenLabsConfig_ShouldThrow()
		{
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        IOptions<ElevenLabsConfig> elevenLabsConfig = default;
        ILogger<AudioThoughts> logger = Substitute.For<ILogger<AudioThoughts>>();

        var actual = Assert.Throws<ArgumentNullException>(() => new AudioThoughts(serviceProvider, semanticKernelLanguageModels, elevenLabsConfig, logger));

        Assert.Equal(nameof(elevenLabsConfig), actual.ParamName);
		}

    [Fact]
    public void Constructor_WithInvalidLogger_ShouldThrow()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        IOptions<ElevenLabsConfig> elevenLabsConfig = Substitute.For<IOptions<ElevenLabsConfig>>();
        elevenLabsConfig.Value.Returns(Config.SEMANTIC_CONFIG.ElevenLabsConfig);
        ILogger<AudioThoughts> logger = null;

        var actual = Assert.Throws<ArgumentNullException>(() => new AudioThoughts(serviceProvider, semanticKernelLanguageModels, elevenLabsConfig, logger));

        Assert.Equal(nameof(logger), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithValidParams_ShouldConstruct()
    {
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(out var configuration);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var elevenLabsConfig = Substitute.For<IOptions<ElevenLabsConfig>>();
        elevenLabsConfig.Value.Returns(Config.SEMANTIC_CONFIG.ElevenLabsConfig);
        var logger = Substitute.For<ILogger<AudioThoughts>>();

        var actual = new AudioThoughts(serviceProvider, semanticKernelLanguageModels, elevenLabsConfig, logger);

        Assert.NotNull(actual);
    }

    [Fact]
    public async Task TextToSpeechAsync_WithInvalidText_ShouldThrow()
    {
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(out var configuration);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var elevenLabsConfig = Substitute.For<IOptions<ElevenLabsConfig>>();
        elevenLabsConfig.Value.Returns(Config.SEMANTIC_CONFIG.ElevenLabsConfig);
        var logger = Substitute.For<ILogger<AudioThoughts>>();
        var instance = new AudioThoughts(serviceProvider, semanticKernelLanguageModels, elevenLabsConfig, logger);
        string text = default;

        var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.ElevenLabsTextToSpeechAsync(text));

        Assert.Equal(nameof(text), actual.ParamName);
    }

    [Fact(Skip = "Integration Test")]
    public async Task TextToSpeechAsync_WithValidText_ShouldReturnFilePath()
    {
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(out var configuration);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var elevenLabsConfig = Substitute.For<IOptions<ElevenLabsConfig>>();
        elevenLabsConfig.Value.Returns(Config.SEMANTIC_CONFIG.ElevenLabsConfig);
        var logger = Substitute.For<ILogger<AudioThoughts>>();
        var instance = new AudioThoughts(serviceProvider, semanticKernelLanguageModels, elevenLabsConfig, logger);
        string text = "Hi, my name is Iluvatar. How are you?";

        var actual = await instance.ElevenLabsTextToSpeechAsync(text);

        Assert.NotEmpty(actual);
        Assert.Contains(".mp3", actual);
    }
}
