using FrostAura.Libraries.Semantic.Core.Thoughts.Chains.Cognitive;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;
using FrostAura.Libraries.Semantic.Core.Extensions.Configuration;
using FrostAura.Libraries.Semantic.Core.Interfaces.Data;

namespace Semantic.Core.Tests.Thoughts.Chains.Cognitive;

public class YouTubeShortFactualVideoGenerationChainTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public YouTubeShortFactualVideoGenerationChainTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void Constructor_WithInvalidServiceProvider_ShouldThrow()
    {
        IServiceProvider serviceProvider = null;
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        ILogger<YouTubeShortFactualVideoGenerationChain> logger = Substitute.For<ILogger<YouTubeShortFactualVideoGenerationChain>>();

        var actual = Assert.Throws<ArgumentNullException>(() => new YouTubeShortFactualVideoGenerationChain(serviceProvider, semanticKernelLanguageModels, logger));

        Assert.Equal(nameof(serviceProvider), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithInvalidISemanticKernelLanguageModelsDataAccess_ShouldThrow()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = null;
        ILogger<YouTubeShortFactualVideoGenerationChain> logger = Substitute.For<ILogger<YouTubeShortFactualVideoGenerationChain>>();

        var actual = Assert.Throws<ArgumentNullException>(() => new YouTubeShortFactualVideoGenerationChain(serviceProvider, semanticKernelLanguageModels, logger));

        Assert.Equal(nameof(semanticKernelLanguageModels), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithInvalidLogger_ShouldThrow()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        ILogger<YouTubeShortFactualVideoGenerationChain> logger = null;

        var actual = Assert.Throws<ArgumentNullException>(() => new YouTubeShortFactualVideoGenerationChain(serviceProvider, semanticKernelLanguageModels, logger));

        Assert.Equal(nameof(logger), actual.ParamName);
    }

    [Fact]
    public async Task GenerateDocuemntaryVideoAsync_WithInvalidInput_ShouldThrow()
    {
        var userProxy = Substitute.For<IUserProxyDataAccess>();
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(Config.SEMANTIC_CONFIG)
            .AddSingleton(userProxy);
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var logger = Substitute.For<ILogger<YouTubeShortFactualVideoGenerationChain>>();
        var instance = new YouTubeShortFactualVideoGenerationChain(serviceCollection.BuildServiceProvider(), semanticKernelLanguageModels, logger);
        string topic = default;

        var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.GenerateDocumentaryVideoAsync(topic));

        Assert.Equal(nameof(topic), actual.ParamName);
    }

    [Fact(Skip = "Integration Test")]
    public async Task GenerateDocumentaryVideoWithStateAsync_WithValidInput_ShouldCallInvokeAsyncAsync()
    {
        var userProxy = Substitute.For<IUserProxyDataAccess>();
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(Config.SEMANTIC_CONFIG)
            .AddSingleton(userProxy);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var logger = Config.GetLogger<YouTubeShortFactualVideoGenerationChain>(_testOutputHelper);

        var instance = new YouTubeShortFactualVideoGenerationChain(serviceProvider, semanticKernelLanguageModels, logger);
        var input = "planets";
        var chainState = new Dictionary<string, string>
        {
            { "$TITLE", "Journey to the Red Planet: Unveiling the Mysteries of Mars" },
            { "$SCRIPT", "As we embark on this cosmic journey, Mars, the Red Planet, unfolds its mysteries. Mars, named after the Roman god of war, is often visible from Earth with the naked eye. Its reddish hue, caused by iron oxide or rust, gives it a distinct appearance. Mars is home to the largest volcano and the deepest canyon in the solar system, Olympus Mons and Valles Marineris respectively. The planet's thin atmosphere and harsh climate, with temperatures dropping to -80 degrees Celsius, make it a hostile environment. Yet, evidence of ancient rivers and lakes suggest that Mars may have once harbored life. The exploration of Mars is not just a journey into space, but a voyage back in time, offering clues about the early solar system and the potential for life on other planets." },
            { "$DESCRIPTION", "Embark on a thrilling cosmic journey as we unravel the mysteries of Mars, the Red Planet. Discover its unique features, from its distinct reddish hue to the largest volcano and deepest canyon in the solar system. Despite its harsh climate, evidence suggests that Mars may have once harbored life. This exploration is not just a space journey, but a voyage back in time, offering insights into the early solar system and the potential for life beyond Earth." },
            { "$TAGS", "mars-exploration space-journey red-planet roman-god-of-war iron-oxide olympus-mons valles-marineris mars-climate ancient-rivers-on-mars life-on-other-planets" },
            { "$VIDEO_QUERY_1", "Mars planet features and exploration" },
            { "$VIDEO_QUERY_2", "Ancient rivers and lakes on Mars" },
            { "$VIDEO_QUERY_3", "Mars atmosphere and harsh climate conditions" },
            { "$VIDEO_QUERY_4", "Roman god of war Mars and its distinct reddish hue" },
            { "$VIDEO_QUERY_5", "Olympus Mons and Valles Marineris on Mars" },
            { "$VIDEO_QUERY_6", "Mars visibility from Earth and potential for life" },
            { "$VIDEO_1_PATH", "videos/16fb116f04384063ac46e4a10570a3ac.mp4" },
            { "$VIDEO_2_PATH", "videos/24d3c301a8b84b31af0437f48028469e.mp4" },
            { "$VIDEO_3_PATH", "videos/05a4f6cc425043719cb822c755d1b2fb.mp4" },
            { "$VIDEO_4_PATH", "videos/429e4b77be4c4bd3a7d026b84795d490.mp4" },
            { "$VIDEO_5_PATH", "videos/a4fcbbdb4dc146e580eed7d66cf448fe.mp4" },
            { "$VIDEO_6_PATH", "videos/05a4f6cc425043719cb822c755d1b2fb.mp4" },
            { "$VOICEOVER_AUDIO_PATH", "/Users/deanmartin/Source/fa.libraries.dotnet/src/Semantic.Core.Tests/bin/Debug/net7.0/ElevenLabs/TextToSpeech/Rachel/8736d051-1521-7266-aed1-cc191606f685.mp3" },
            { "$VIDEO_PATH", "videos/6907538454af4d90b4832fa31fdaf070.mp4"}
        };

        var actual = await instance.GenerateDocumentaryVideoWithStateAsync(input, chainState);

        Assert.NotEmpty(actual);
    }
}
