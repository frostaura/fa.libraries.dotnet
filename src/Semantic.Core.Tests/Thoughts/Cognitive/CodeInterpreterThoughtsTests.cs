using FrostAura.Libraries.Semantic.Core.Extensions.Configuration;
using FrostAura.Libraries.Semantic.Core.Interfaces.Data;
using FrostAura.Libraries.Semantic.Core.Thoughts.Cognitive;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Semantic.Core.Tests.Thoughts.Cognitive;

public class CodeInterpreterThoughtsTests
{
    [Fact]
    public void Constructor_WithInvalidServiceProvider_ShouldThrow()
    {
        IServiceProvider serviceProvider = null;
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        ILogger<CodeInterpreterThoughts> logger = Substitute.For<ILogger<CodeInterpreterThoughts>>();

        var actual = Assert.Throws<ArgumentNullException>(() => new CodeInterpreterThoughts(serviceProvider, semanticKernelLanguageModels, logger));

        Assert.Equal(nameof(serviceProvider), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithInvalidISemanticKernelLanguageModelsDataAccess_ShouldThrow()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = null;
        ILogger<CodeInterpreterThoughts> logger = Substitute.For<ILogger<CodeInterpreterThoughts>>();

        var actual = Assert.Throws<ArgumentNullException>(() => new CodeInterpreterThoughts(serviceProvider, semanticKernelLanguageModels, logger));

        Assert.Equal(nameof(semanticKernelLanguageModels), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithInvalidLogger_ShouldThrow()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        ILogger<CodeInterpreterThoughts> logger = null;

        var actual = Assert.Throws<ArgumentNullException>(() => new CodeInterpreterThoughts(serviceProvider, semanticKernelLanguageModels, logger));

        Assert.Equal(nameof(logger), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithValidParams_ShouldConstruct()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var logger = Substitute.For<ILogger<CodeInterpreterThoughts>>();

        var actual = new CodeInterpreterThoughts(serviceProvider, semanticKernelLanguageModels, logger);

        Assert.NotNull(actual);
    }

    [Fact]
    public async Task InvokeAsync_WithInvalidPythonVersion_ShouldThrow()
    {
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(out var configuration);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var logger = Substitute.For<ILogger<CodeInterpreterThoughts>>();
        var instance = new CodeInterpreterThoughts(serviceProvider, semanticKernelLanguageModels, logger);
        string pythonVersion = default;
        string pipDependencies = "";
        string condaDependencies = "";
        string code = "def main()";

        var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.InvokeAsync(pythonVersion, pipDependencies, condaDependencies, code));

        Assert.Equal(nameof(pythonVersion), actual.ParamName);
    }

    [Fact]
    public async Task InvokeAsync_WithInvalidCode_ShouldThrow()
    {
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(out var configuration);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var logger = Substitute.For<ILogger<CodeInterpreterThoughts>>();
        var instance = new CodeInterpreterThoughts(serviceProvider, semanticKernelLanguageModels, logger);
        string pythonVersion = "3.8";
        string pipDependencies = "";
        string condaDependencies = "";
        string code = default;

        var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.InvokeAsync(pythonVersion, pipDependencies, condaDependencies, code));

        Assert.Equal(nameof(code), actual.ParamName);
    }

    [Theory]
    [InlineData("""
            def main() -> str:
                return 'Test 1'
            """, "Test 1")]
    [InlineData("""
            def main() -> str:
                return f'{1 + 1}'
            """, "2")]
    [InlineData("""
            def main() -> str:
                return f'{True}'
            """, "True")]
    public async Task InvokeAsync_WithValidParams_ShouldExecuteAndRespondCorrectly(string code, string expected)
    {
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(out var configuration);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var logger = Substitute.For<ILogger<CodeInterpreterThoughts>>();
        var instance = new CodeInterpreterThoughts(serviceProvider, semanticKernelLanguageModels, logger);
        string pythonVersion = "3.9";
        string pipDependencies = "pip";
        string condaDependencies = "ffmpeg";

        var actual = await instance.InvokeAsync(pythonVersion, pipDependencies, condaDependencies, code);

        Assert.Equal(expected, actual);
    }
}
