using System;
using FrostAura.Libraries.Semantic.Core.Extensions.Configuration;
using FrostAura.Libraries.Semantic.Core.Interfaces.Data;
using FrostAura.Libraries.Semantic.Core.Models.Configuration;
using FrostAura.Libraries.Semantic.Core.Thoughts.Cognitive;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel.Memory;
using NSubstitute;
using Xunit;

namespace Semantic.Core.Tests.Thoughts.Cognitive;

public class LongTermMemoryThoughtsTests
{
    [Fact]
    public void Constructor_WithInvalidServiceProvider_ShouldThrow()
    {
        IServiceProvider serviceProvider = null;
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        IOptions<SemanticMemoryConfig> semanticMemoryConfig = Substitute.For<IOptions<SemanticMemoryConfig>>();
        ILogger<LongTermMemoryThoughts> logger = Substitute.For<ILogger<LongTermMemoryThoughts>>();

        var actual = Assert.Throws<ArgumentNullException>(() => new LongTermMemoryThoughts(serviceProvider, semanticKernelLanguageModels, semanticMemoryConfig, logger));

        Assert.Equal(nameof(serviceProvider), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithInvalidISemanticKernelLanguageModelsDataAccess_ShouldThrow()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = null;
        IOptions<SemanticMemoryConfig> semanticMemoryConfig = Substitute.For<IOptions<SemanticMemoryConfig>>();
        ILogger<LongTermMemoryThoughts> logger = Substitute.For<ILogger<LongTermMemoryThoughts>>();

        var actual = Assert.Throws<ArgumentNullException>(() => new LongTermMemoryThoughts(serviceProvider, semanticKernelLanguageModels, semanticMemoryConfig, logger));

        Assert.Equal(nameof(semanticKernelLanguageModels), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithSemanticMemoryConfig_ShouldThrow()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        IOptions<SemanticMemoryConfig> semanticMemoryConfig = null;
        ILogger<LongTermMemoryThoughts> logger = Substitute.For<ILogger<LongTermMemoryThoughts>>();

        var actual = Assert.Throws<ArgumentNullException>(() => new LongTermMemoryThoughts(serviceProvider, semanticKernelLanguageModels, semanticMemoryConfig, logger));

        Assert.Equal(nameof(semanticMemoryConfig), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithInvalidLogger_ShouldThrow()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        IOptions<SemanticMemoryConfig> semanticMemoryConfig = Substitute.For<IOptions<SemanticMemoryConfig>>();
        ILogger<LongTermMemoryThoughts> logger = null;

        var actual = Assert.Throws<ArgumentNullException>(() => new LongTermMemoryThoughts(serviceProvider, semanticKernelLanguageModels, semanticMemoryConfig, logger));

        Assert.Equal(nameof(logger), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithValidParams_ShouldConstruct()
    {
        var serviceProvider = Substitute.For<IServiceProvider>();
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        semanticKernelLanguageModels
            .GetSemanticTextMemoryAsync(Arg.Any<CancellationToken>())
            .Returns(Substitute.For<ISemanticTextMemory>());
        var semanticMemoryConfig = Substitute.For<IOptions<SemanticMemoryConfig>>();
        semanticMemoryConfig.Value.Returns(Substitute.For<SemanticMemoryConfig>());
        var logger = Substitute.For<ILogger<LongTermMemoryThoughts>>();

        var actual = new LongTermMemoryThoughts(serviceProvider, semanticKernelLanguageModels, semanticMemoryConfig, logger);

        Assert.NotNull(actual);
    }

    [Fact]
    public async Task CommitToMemoryAsync_WithInvalidMemory_ShouldThrow()
    {
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(Config.SEMANTIC_CONFIG);
        var options = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var semanticMemoryConfig = Substitute.For<IOptions<SemanticMemoryConfig>>();
        semanticMemoryConfig.Value.Returns(Substitute.For<SemanticMemoryConfig>());
        var logger = Substitute.For<ILogger<LongTermMemoryThoughts>>();
        var instance = new LongTermMemoryThoughts(serviceCollection.BuildServiceProvider(), options, semanticMemoryConfig, logger);
        string memory = default;
        string source = "test source";

        var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.CommitToMemoryAsync(memory, source));

        Assert.Equal(nameof(memory), actual.ParamName);
    }

    [Fact]
    public async Task CommitToMemoryAsync_WithInvalidSource_ShouldThrow()
    {
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(Config.SEMANTIC_CONFIG);
        var options = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var logger = Substitute.For<ILogger<LongTermMemoryThoughts>>();
        var semanticMemoryConfig = Substitute.For<IOptions<SemanticMemoryConfig>>();
        semanticMemoryConfig.Value.Returns(Substitute.For<SemanticMemoryConfig>());
        var instance = new LongTermMemoryThoughts(serviceCollection.BuildServiceProvider(), options, semanticMemoryConfig, logger);
        string memory = "test Memory";
        string source = default;

        var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.CommitToMemoryAsync(memory, source));

        Assert.Equal(nameof(source), actual.ParamName);
    }

    [Fact]
    public async Task CommitToMemoryAsync_WithValidInput_ShouldRespond()
    {
        var memory = Substitute.For<ISemanticTextMemory>();
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(Config.SEMANTIC_CONFIG);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        semanticKernelLanguageModels
            .GetSemanticTextMemoryAsync(Arg.Any<CancellationToken>())
            .Returns(memory);
        var semanticMemoryConfig = Substitute.For<IOptions<SemanticMemoryConfig>>();
        semanticMemoryConfig.Value.Returns(Substitute.For<SemanticMemoryConfig>());
        var logger = Substitute.For<ILogger<LongTermMemoryThoughts>>();
        var instance = new LongTermMemoryThoughts(serviceProvider, semanticKernelLanguageModels, semanticMemoryConfig, logger);
        string input = new string(Enumerable.Range(0, 2200)
            .Select(_ => (char)(new Random().Next(32, 127)))
            .ToArray());

        var actual = await instance.CommitToMemoryAsync(input, "Test");

        memory
            .ReceivedWithAnyArgs()
            .SaveInformationAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task RecallFromMemoryAsync_WithInvalidMemory_ShouldThrow()
    {
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(Config.SEMANTIC_CONFIG);
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        semanticKernelLanguageModels
            .GetSemanticTextMemoryAsync(Arg.Any<CancellationToken>())
            .Returns(Substitute.For<ISemanticTextMemory>());
        var semanticMemoryConfig = Substitute.For<IOptions<SemanticMemoryConfig>>();
        semanticMemoryConfig.Value.Returns(Substitute.For<SemanticMemoryConfig>());
        var logger = Substitute.For<ILogger<LongTermMemoryThoughts>>();
        var instance = new LongTermMemoryThoughts(serviceCollection.BuildServiceProvider(), semanticKernelLanguageModels, semanticMemoryConfig, logger);
        string query = default;

        var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.RecallFromMemoryAsync(query));

        Assert.Equal(nameof(query), actual.ParamName);
    }

    [Fact]
    public async Task RecallFromMemoryAsync_WithValidInputAndStoreAndValue_ShouldRespond()
    {
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(Config.SEMANTIC_CONFIG);
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        semanticKernelLanguageModels
            .GetSemanticTextMemoryAsync(Arg.Any<CancellationToken>())
            .Returns(Substitute.For<ISemanticTextMemory>());
        var semanticMemoryConfig = Substitute.For<IOptions<SemanticMemoryConfig>>();
        semanticMemoryConfig.Value.Returns(Substitute.For<SemanticMemoryConfig>());
        var logger = Substitute.For<ILogger<LongTermMemoryThoughts>>();
        var instance = new LongTermMemoryThoughts(serviceCollection.BuildServiceProvider(), semanticKernelLanguageModels, semanticMemoryConfig, logger);
        string input = "What is my name?";

        var actual = await instance.RecallFromMemoryAsync(input);

        Assert.NotEmpty(actual);
    }
}
