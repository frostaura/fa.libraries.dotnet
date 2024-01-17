using FrostAura.Libraries.Semantic.Core.Extensions.Configuration;
using FrostAura.Libraries.Semantic.Core.Interfaces.Data;
using FrostAura.Libraries.Semantic.Core.Thoughts.Cognitive;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Semantic.Core.Tests.Thoughts.Cognitive;

public class LanguageModelThoughtsTests
{
    [Fact]
    public void Constructor_WithInvalidServiceProvider_ShouldThrow()
    {
        IServiceProvider serviceProvider = null;
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        ILogger<LanguageModelThoughts> logger = Substitute.For<ILogger<LanguageModelThoughts>>();

        var actual = Assert.Throws<ArgumentNullException>(() => new LanguageModelThoughts(serviceProvider, semanticKernelLanguageModels, logger));

        Assert.Equal(nameof(serviceProvider), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithInvalidISemanticKernelLanguageModelsDataAccess_ShouldThrow()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = null;
        ILogger<LanguageModelThoughts> logger = Substitute.For<ILogger<LanguageModelThoughts>>();

        var actual = Assert.Throws<ArgumentNullException>(() => new LanguageModelThoughts(serviceProvider, semanticKernelLanguageModels, logger));

        Assert.Equal(nameof(semanticKernelLanguageModels), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithInvalidLogger_ShouldThrow()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        ILogger<LanguageModelThoughts> logger = null;

        var actual = Assert.Throws<ArgumentNullException>(() => new LanguageModelThoughts(serviceProvider, semanticKernelLanguageModels, logger));

        Assert.Equal(nameof(logger), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithValidParams_ShouldConstruct()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var logger = Substitute.For<ILogger<LanguageModelThoughts>>();

        var actual = new LanguageModelThoughts(serviceProvider, semanticKernelLanguageModels, logger);

        Assert.NotNull(actual);
    }

    [Fact]
    public async Task PromptLLMAsync_WithInvalidInput_ShouldThrow()
    {
        var serviceCollection = new ServiceCollection().AddSemanticCore(Config.SEMANTIC_CONFIG);
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var logger = Substitute.For<ILogger<LanguageModelThoughts>>();
        var instance = new LanguageModelThoughts(serviceCollection.BuildServiceProvider(), semanticKernelLanguageModels, logger);
        string prompt = default;

        var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.PromptLLMAsync(prompt));

        Assert.Equal(nameof(prompt), actual.ParamName);
    }

    [Fact]
    public async Task PromptLLMAsync_WithValidInput_ShouldRespond()
    {
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(Config.SEMANTIC_CONFIG)
            .AddSingleton(Substitute.For<IUserProxyDataAccess>());
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = serviceProvider.GetRequiredService<ISemanticKernelLanguageModelsDataAccess>();
        var logger = Substitute.For<ILogger<LanguageModelThoughts>>();
        var instance = new LanguageModelThoughts(serviceProvider, semanticKernelLanguageModels, logger);
        var prompt = "Answer the following as directly as possible: 2+2=";

        var actual = await instance.PromptLLMAsync(prompt);

        Assert.Contains(4.ToString(), actual);
    }

    [Fact]
    public async Task PromptSmallLLMAsync_WithInvalidInput_ShouldThrow()
    {
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(Config.SEMANTIC_CONFIG)
            .AddSingleton(Substitute.For<IUserProxyDataAccess>());
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = serviceProvider.GetRequiredService<ISemanticKernelLanguageModelsDataAccess>();
        var logger = Substitute.For<ILogger<LanguageModelThoughts>>();
        var instance = new LanguageModelThoughts(serviceCollection.BuildServiceProvider(), semanticKernelLanguageModels, logger);
        string prompt = default;

        var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.PromptSmallLLMAsync(prompt));

        Assert.Equal(nameof(prompt), actual.ParamName);
    }

    [Fact]
    public async Task PromptSmallLLMAsync_WithValidInput_ShouldRespond()
    {
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(Config.SEMANTIC_CONFIG)
            .AddSingleton(Substitute.For<IUserProxyDataAccess>());
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = serviceProvider.GetRequiredService<ISemanticKernelLanguageModelsDataAccess>();
        var logger = Substitute.For<ILogger<LanguageModelThoughts>>();
        var instance = new LanguageModelThoughts(serviceCollection.BuildServiceProvider(), semanticKernelLanguageModels, logger);
        var prompt = "Answer the following as directly as possible: 2+2=";

        var actual = await instance.PromptSmallLLMAsync(prompt);

        Assert.Contains(4.ToString(), actual);
    }

    [Fact]
    public async Task GetStringEmbeddingsAsync_WithInvalidInput_ShouldThrow()
    {
        var serviceCollection = new ServiceCollection().AddSemanticCore(Config.SEMANTIC_CONFIG);
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var logger = Substitute.For<ILogger<LanguageModelThoughts>>();
        var instance = new LanguageModelThoughts(serviceCollection.BuildServiceProvider(), semanticKernelLanguageModels, logger);
        string input = default;

        var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.GetStringEmbeddingsAsync(input));

        Assert.Equal(nameof(input), actual.ParamName);
    }

    [Fact]
    public async Task GetStringEmbeddingsAsync_WithValidInput_ShouldRespond()
    {
        var serviceCollection = new ServiceCollection().AddSemanticCore(Config.SEMANTIC_CONFIG);
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var logger = Substitute.For<ILogger<LanguageModelThoughts>>();
        var instance = new LanguageModelThoughts(serviceCollection.BuildServiceProvider(), semanticKernelLanguageModels, logger);
        var prompt = "Answer the following as directly as possible: 2+2=";

        var actual = await instance.GetStringEmbeddingsAsync(prompt);

        Assert.NotEmpty(actual);
    }
}
