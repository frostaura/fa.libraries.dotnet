using FrostAura.Libraries.Semantic.Core.Extensions.Configuration;
using FrostAura.Libraries.Semantic.Core.Interfaces.Data;
using FrostAura.Libraries.Semantic.Core.Thoughts.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Semantic.Core.Tests.Thoughts.IO;

public class SystemThoughtsTests
{
    [Fact]
    public void Constructor_WithInvalidServiceProvider_ShouldThrow()
    {
        IServiceProvider serviceProvider = null;
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        IUserProxyDataAccess userProxy = Substitute.For<IUserProxyDataAccess>();
        ILogger<SystemThoughts> logger = Substitute.For<ILogger<SystemThoughts>>();

        var actual = Assert.Throws<ArgumentNullException>(() => new SystemThoughts(serviceProvider, semanticKernelLanguageModels, userProxy, logger));

        Assert.Equal(nameof(serviceProvider), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithInvalidISemanticKernelLanguageModelsDataAccess_ShouldThrow()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = null;
        IUserProxyDataAccess userProxy = Substitute.For<IUserProxyDataAccess>();
        ILogger<SystemThoughts> logger = Substitute.For<ILogger<SystemThoughts>>();

        var actual = Assert.Throws<ArgumentNullException>(() => new SystemThoughts(serviceProvider, semanticKernelLanguageModels, userProxy, logger));

        Assert.Equal(nameof(semanticKernelLanguageModels), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithInvalidUserProxy_ShouldThrow()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        IUserProxyDataAccess userProxy = null;
        ILogger<SystemThoughts> logger = Substitute.For<ILogger<SystemThoughts>>();

        var actual = Assert.Throws<ArgumentNullException>(() => new SystemThoughts(serviceProvider, semanticKernelLanguageModels, userProxy, logger));

        Assert.Equal(nameof(userProxy), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithInvalidLogger_ShouldThrow()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        IUserProxyDataAccess userProxy = Substitute.For<IUserProxyDataAccess>();
        ILogger<SystemThoughts> logger = null;

        var actual = Assert.Throws<ArgumentNullException>(() => new SystemThoughts(serviceProvider, semanticKernelLanguageModels, userProxy, logger));

        Assert.Equal(nameof(logger), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithValidParams_ShouldConstruct()
    {
        var serviceProvider = Substitute.For<IServiceProvider>();
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var userProxy = Substitute.For<IUserProxyDataAccess>();
        var logger = Substitute.For<ILogger<SystemThoughts>>();

        var actual = new SystemThoughts(serviceProvider, semanticKernelLanguageModels, userProxy, logger);

        Assert.NotNull(actual);
    }

    [Fact]
    public async Task AskForInputAsync_WithInvalidInput_ShouldThrow()
    {
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(out var configuration);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var userProxy = Substitute.For<IUserProxyDataAccess>();
        var logger = Substitute.For<ILogger<SystemThoughts>>();
        var instance = new SystemThoughts(serviceProvider, semanticKernelLanguageModels, userProxy, logger);
        string question = default;

        var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.AskForInputAsync(question));

        Assert.Equal(nameof(question), actual.ParamName);
    }

    [Fact]
    public async Task AskForInputAsync_WithInput_ShouldCallAskUserAsync()
    {
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(out var configuration);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var userProxy = Substitute.For<IUserProxyDataAccess>();
        var logger = Substitute.For<ILogger<SystemThoughts>>();
        var instance = new SystemThoughts(serviceProvider, semanticKernelLanguageModels, userProxy, logger);
        string output = "expected output";

        var actual = await instance.AskForInputAsync(output);

        userProxy
            .Received()
            .AskUserAsync(output, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task OutputTextAsync_WithInvalidInput_ShouldThrow()
    {
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(out var configuration);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var userProxy = Substitute.For<IUserProxyDataAccess>();
        var logger = Substitute.For<ILogger<SystemThoughts>>();
        var instance = new SystemThoughts(serviceProvider, semanticKernelLanguageModels, userProxy, logger);
        string output = default;

        var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.OutputTextAsync(output));

        Assert.Equal(nameof(output), actual.ParamName);
    }

    [Fact]
    public async Task OutputTextAsync_WithInput_ShouldReturnCorrectValue()
    {
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(out var configuration);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var userProxy = Substitute.For<IUserProxyDataAccess>();
        var logger = Substitute.For<ILogger<SystemThoughts>>();
        var instance = new SystemThoughts(serviceProvider, semanticKernelLanguageModels, userProxy, logger);
        string output = "expected output";

        var actual = await instance.OutputTextAsync(output);

        Assert.Equal(output, actual);
    }
}
