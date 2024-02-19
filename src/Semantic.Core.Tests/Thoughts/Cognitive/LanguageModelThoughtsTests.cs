using System;
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

public class LanguageModelThoughtsTests
{
    [Fact]
    public void Constructor_WithInvalidServiceProvider_ShouldThrow()
    {
        IServiceProvider serviceProvider = null;
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        IHttpClientFactory httpClientFactory = Substitute.For<IHttpClientFactory>();
        IOptions<OpenAIConfig> openAIConfigOptions = Substitute.For<IOptions<OpenAIConfig>>();
        ILogger<LanguageModelThoughts> logger = Substitute.For<ILogger<LanguageModelThoughts>>();

        var actual = Assert.Throws<ArgumentNullException>(() => new LanguageModelThoughts(serviceProvider, semanticKernelLanguageModels, httpClientFactory, openAIConfigOptions, logger));

        Assert.Equal(nameof(serviceProvider), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithInvalidISemanticKernelLanguageModelsDataAccess_ShouldThrow()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = null;
        IHttpClientFactory httpClientFactory = Substitute.For<IHttpClientFactory>();
        IOptions<OpenAIConfig> openAIConfigOptions = Substitute.For<IOptions<OpenAIConfig>>();
        ILogger<LanguageModelThoughts> logger = Substitute.For<ILogger<LanguageModelThoughts>>();

        var actual = Assert.Throws<ArgumentNullException>(() => new LanguageModelThoughts(serviceProvider, semanticKernelLanguageModels, httpClientFactory, openAIConfigOptions, logger));

        Assert.Equal(nameof(semanticKernelLanguageModels), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithInvalidHttpClientFactory_ShouldThrow()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        IHttpClientFactory httpClientFactory = Substitute.For<IHttpClientFactory>(); ;
        IOptions<OpenAIConfig> openAIConfigOptions = default;
        ILogger<LanguageModelThoughts> logger = Substitute.For<ILogger<LanguageModelThoughts>>();

        var actual = Assert.Throws<ArgumentNullException>(() => new LanguageModelThoughts(serviceProvider, semanticKernelLanguageModels, httpClientFactory, openAIConfigOptions, logger));

        Assert.Equal(nameof(openAIConfigOptions), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithInvalidOpenAIConfigOptions_ShouldThrow()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        IHttpClientFactory httpClientFactory = null;
        IOptions<OpenAIConfig> openAIConfigOptions = Substitute.For<IOptions<OpenAIConfig>>();
        openAIConfigOptions.Value.Returns(new OpenAIConfig());
        ILogger<LanguageModelThoughts> logger = Substitute.For<ILogger<LanguageModelThoughts>>();

        var actual = Assert.Throws<ArgumentNullException>(() => new LanguageModelThoughts(serviceProvider, semanticKernelLanguageModels, httpClientFactory, openAIConfigOptions, logger));

        Assert.Equal(nameof(httpClientFactory), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithInvalidLogger_ShouldThrow()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        IHttpClientFactory httpClientFactory = Substitute.For<IHttpClientFactory>();
        IOptions<OpenAIConfig> openAIConfigOptions = Substitute.For<IOptions<OpenAIConfig>>();
        openAIConfigOptions.Value.Returns(new OpenAIConfig());
        ILogger<LanguageModelThoughts> logger = null;

        var actual = Assert.Throws<ArgumentNullException>(() => new LanguageModelThoughts(serviceProvider, semanticKernelLanguageModels, httpClientFactory, openAIConfigOptions, logger));

        Assert.Equal(nameof(logger), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithValidParams_ShouldConstruct()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        IHttpClientFactory httpClientFactory = Substitute.For<IHttpClientFactory>();
        IOptions<OpenAIConfig> openAIConfigOptions = Substitute.For<IOptions<OpenAIConfig>>();
        openAIConfigOptions.Value.Returns(new OpenAIConfig());
        var logger = Substitute.For<ILogger<LanguageModelThoughts>>();

        var actual = new LanguageModelThoughts(serviceProvider, semanticKernelLanguageModels, httpClientFactory, openAIConfigOptions, logger);

        Assert.NotNull(actual);
    }

    [Fact]
    public async Task PromptLLMAsync_WithInvalidInput_ShouldThrow()
    {
        var serviceCollection = new ServiceCollection().AddSemanticCore(out var configuration);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        var openAIConfigOptions = serviceProvider.GetRequiredService<IOptions<OpenAIConfig>>();
        var logger = Substitute.For<ILogger<LanguageModelThoughts>>();
        var instance = new LanguageModelThoughts(serviceProvider, semanticKernelLanguageModels, httpClientFactory, openAIConfigOptions, logger);
        string prompt = default;

        var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.PromptLLMAsync(prompt));

        Assert.Equal(nameof(prompt), actual.ParamName);
    }

    [Fact]
    public async Task PromptLLMAsync_WithValidInput_ShouldRespond()
    {
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(out var configuration)
            .AddSingleton(Substitute.For<IUserProxyDataAccess>());
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = serviceProvider.GetRequiredService<ISemanticKernelLanguageModelsDataAccess>();
        var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
        var openAIConfigOptions = serviceProvider.GetRequiredService<IOptions<OpenAIConfig>>();
        var logger = Substitute.For<ILogger<LanguageModelThoughts>>();
        var instance = new LanguageModelThoughts(serviceProvider, semanticKernelLanguageModels, httpClientFactory, openAIConfigOptions, logger);
        var prompt = "Answer the following as directly as possible: 2+2=";

        var actual = await instance.PromptLLMAsync(prompt);

        Assert.Contains(4.ToString(), actual);
    }

    [Fact]
    public async Task PromptSmallLLMAsync_WithInvalidInput_ShouldThrow()
    {
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(out var configuration)
            .AddSingleton(Substitute.For<IUserProxyDataAccess>());
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = serviceProvider.GetRequiredService<ISemanticKernelLanguageModelsDataAccess>();
        var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
        var openAIConfigOptions = serviceProvider.GetRequiredService<IOptions<OpenAIConfig>>();
        var logger = Substitute.For<ILogger<LanguageModelThoughts>>();
        var instance = new LanguageModelThoughts(serviceCollection.BuildServiceProvider(), semanticKernelLanguageModels, httpClientFactory, openAIConfigOptions, logger);
        string prompt = default;

        var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.PromptSmallLLMAsync(prompt));

        Assert.Equal(nameof(prompt), actual.ParamName);
    }

    [Fact]
    public async Task PromptSmallLLMAsync_WithValidInput_ShouldRespond()
    {
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(out var configuration)
            .AddSingleton(Substitute.For<IUserProxyDataAccess>());
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = serviceProvider.GetRequiredService<ISemanticKernelLanguageModelsDataAccess>();
        var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
        var openAIConfigOptions = serviceProvider.GetRequiredService<IOptions<OpenAIConfig>>();
        var logger = Substitute.For<ILogger<LanguageModelThoughts>>();
        var instance = new LanguageModelThoughts(serviceCollection.BuildServiceProvider(), semanticKernelLanguageModels, httpClientFactory, openAIConfigOptions, logger);
        var prompt = "Answer the following as directly as possible: 2+2=";

        var actual = await instance.PromptSmallLLMAsync(prompt);

        Assert.Contains(4.ToString(), actual);
    }

    [Fact]
    public async Task GetStringEmbeddingsAsync_WithInvalidInput_ShouldThrow()
    {
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(out var configuration)
            .AddSingleton(Substitute.For<IUserProxyDataAccess>());
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
        var openAIConfigOptions = serviceProvider.GetRequiredService<IOptions<OpenAIConfig>>();
        var logger = Substitute.For<ILogger<LanguageModelThoughts>>();
        var instance = new LanguageModelThoughts(serviceProvider, semanticKernelLanguageModels, httpClientFactory, openAIConfigOptions, logger);
        string input = default;

        var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.GetStringEmbeddingsAsync(input));

        Assert.Equal(nameof(input), actual.ParamName);
    }

    [Fact]
    public async Task GetStringEmbeddingsAsync_WithValidInput_ShouldRespond()
    {
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(out var configuration)
            .AddSingleton(Substitute.For<IUserProxyDataAccess>());
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
        var openAIConfigOptions = serviceProvider.GetRequiredService<IOptions<OpenAIConfig>>();
        var logger = Substitute.For<ILogger<LanguageModelThoughts>>();
        var instance = new LanguageModelThoughts(serviceProvider, semanticKernelLanguageModels, httpClientFactory, openAIConfigOptions, logger);
        var prompt = "Answer the following as directly as possible: 2+2=";

        var actual = await instance.GetStringEmbeddingsAsync(prompt);

        Assert.NotEmpty(actual);
    }

    [Fact]
    public async Task PromptLLMAboutImageFromUrlAsync_WithInvalidPrompt_ShouldThrow()
    {
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(out var configuration)
            .AddSingleton(Substitute.For<IUserProxyDataAccess>());
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = serviceProvider.GetRequiredService<ISemanticKernelLanguageModelsDataAccess>();
        var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
        var openAIConfigOptions = serviceProvider.GetRequiredService<IOptions<OpenAIConfig>>();
        var logger = Substitute.For<ILogger<LanguageModelThoughts>>();
        var instance = new LanguageModelThoughts(serviceCollection.BuildServiceProvider(), semanticKernelLanguageModels, httpClientFactory, openAIConfigOptions, logger);
        string prompt = default;
        string imageUrl = "http://via.placeholder.com/200x200";

        var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.PromptLLMAboutImageFromUrlAsync(prompt, imageUrl, CancellationToken.None));

        Assert.Equal(nameof(prompt), actual.ParamName);
    }

    [Fact]
    public async Task PromptLLMAboutImageFromUrlAsync_WithInvalidImageUrl_ShouldThrow()
    {
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(out var configuration)
            .AddSingleton(Substitute.For<IUserProxyDataAccess>());
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = serviceProvider.GetRequiredService<ISemanticKernelLanguageModelsDataAccess>();
        var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
        var openAIConfigOptions = serviceProvider.GetRequiredService<IOptions<OpenAIConfig>>();
        var logger = Substitute.For<ILogger<LanguageModelThoughts>>();
        var instance = new LanguageModelThoughts(serviceCollection.BuildServiceProvider(), semanticKernelLanguageModels, httpClientFactory, openAIConfigOptions, logger);
        string prompt = "Describe the image.";
        string imageUrl = default;

        var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.PromptLLMAboutImageFromUrlAsync(prompt, imageUrl, CancellationToken.None));

        Assert.Equal(nameof(imageUrl), actual.ParamName);
    }

    [Fact]
    public async Task PromptLLMAboutImageFromUrlAsync_WithValidInput_ShouldRespond()
    {
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(out var configuration)
            .AddSingleton(Substitute.For<IUserProxyDataAccess>());
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = serviceProvider.GetRequiredService<ISemanticKernelLanguageModelsDataAccess>();
        var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
        var openAIConfigOptions = serviceProvider.GetRequiredService<IOptions<OpenAIConfig>>();
        var logger = Substitute.For<ILogger<LanguageModelThoughts>>();
        var instance = new LanguageModelThoughts(serviceCollection.BuildServiceProvider(), semanticKernelLanguageModels, httpClientFactory, openAIConfigOptions, logger);
        string prompt = "What's in this image?";
        string imageUrl = "https://upload.wikimedia.org/wikipedia/commons/d/d5/Half-timbered_mansion%2C_Zirkel%2C_East_view.jpg";

        var actual = await instance.PromptLLMAboutImageFromUrlAsync(prompt, imageUrl, CancellationToken.None);

        Assert.NotEmpty(actual);
    }
}
