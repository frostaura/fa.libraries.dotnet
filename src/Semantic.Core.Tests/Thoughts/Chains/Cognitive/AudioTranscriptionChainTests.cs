using FrostAura.Libraries.Semantic.Core.Thoughts.Chains.Cognitive;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using FrostAura.Libraries.Semantic.Core.Extensions.Configuration;

namespace Semantic.Core.Tests.Thoughts.Chains.Cognitive
{
    public class AudioTranscriptionChainTests
    {
        [Fact]
        public void Constructor_WithInvalidLogger_ShouldThrow()
        {
            ILogger<AudioTranscriptionChain> logger = null;
            IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();

            var actual = Assert.Throws<ArgumentNullException>(() => new AudioTranscriptionChain(serviceProvider, logger));

            Assert.Equal(nameof(logger), actual.ParamName);
        }

        [Fact]
        public void Constructor_WithInvalidServiceProvider_ShouldThrow()
        {
            ILogger<AudioTranscriptionChain> logger = Substitute.For<ILogger<AudioTranscriptionChain>>();
            IServiceProvider serviceProvider = null;

            var actual = Assert.Throws<ArgumentNullException>(() => new AudioTranscriptionChain(serviceProvider, logger));

            Assert.Equal(nameof(serviceProvider), actual.ParamName);
        }

        [Fact]
        public void Constructor_WithValidParams_ShouldConstruct()
        {
            var serviceProvider = Substitute.For<IServiceProvider>();
            var logger = Substitute.For<ILogger<AudioTranscriptionChain>>();

            var actual = new AudioTranscriptionChain(serviceProvider, logger);

            Assert.NotNull(actual);
            Assert.NotEmpty(actual.QueryExample);
            Assert.NotEmpty(actual.QueryInputExample);
            Assert.NotEmpty(actual.Reasoning);
            Assert.NotEmpty(actual.ChainOfThoughts);
        }

        [Fact]
        public async Task ExecuteChainAsync_WithInvalidInput_ShouldThrow()
        {
            var serviceProvider = Substitute.For<IServiceProvider>();
            var logger = Substitute.For<ILogger<AudioTranscriptionChain>>();
            var instance = new AudioTranscriptionChain(serviceProvider, logger);
            string input = default;

            var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.ExecuteChainAsync(input));

            Assert.Equal(nameof(input), actual.ParamName);
        }

        [Fact]
        public async Task ExecuteChainAsync_WithValidInput_ShouldCallInvokeAsyncAsync()
        {
            var serviceCollection = new ServiceCollection()
                .AddSemanticCore(Config.SEMANTIC_CONFIG);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var logger = Substitute.For<ILogger<AudioTranscriptionChain>>();
            var instance = new AudioTranscriptionChain(serviceProvider, logger);
            var input = "./harvard.wav";
            var expected = "The stale smell of old beer lingers. It takes heat to bring out the odor. A cold dip restores health and zest. A salt pickle tastes fine with ham. Tacos al pastor are my favorite. A zestful food is the hot cross bun.";

            var actual = await instance.ExecuteChainAsync(input);

            Assert.Contains(expected, actual);
        }
    }
}
