using FrostAura.Libraries.Semantic.Core.Thoughts.Chains.Cognitive;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using FrostAura.Libraries.Semantic.Core.Extensions.Configuration;

namespace Semantic.Core.Tests.Thoughts.Chains.Cognitive
{
    public class TextToSpeechChainTests
    {
        [Fact]
        public void Constructor_WithInvalidLogger_ShouldThrow()
        {
            ILogger<TextToSpeechChain> logger = null;
            IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();

            var actual = Assert.Throws<ArgumentNullException>(() => new TextToSpeechChain(serviceProvider, logger));

            Assert.Equal(nameof(logger), actual.ParamName);
        }

        [Fact]
        public void Constructor_WithInvalidServiceProvider_ShouldThrow()
        {
            ILogger<TextToSpeechChain> logger = Substitute.For<ILogger<TextToSpeechChain>>();
            IServiceProvider serviceProvider = null;

            var actual = Assert.Throws<ArgumentNullException>(() => new TextToSpeechChain(serviceProvider, logger));

            Assert.Equal(nameof(serviceProvider), actual.ParamName);
        }

        [Fact]
        public void Constructor_WithValidParams_ShouldConstruct()
        {
            var serviceProvider = Substitute.For<IServiceProvider>();
            var logger = Substitute.For<ILogger<TextToSpeechChain>>();

            var actual = new TextToSpeechChain(serviceProvider, logger);

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
                .AddSemanticCore(Config.SEMANTIC_CONFIG);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var logger = Substitute.For<ILogger<TextToSpeechChain>>();
            var instance = new TextToSpeechChain(serviceProvider, logger);
            string audioFilePath = default;

            var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.SpeakTextAndGetFilePathAsync(audioFilePath));

            Assert.Equal(nameof(audioFilePath), actual.ParamName);
        }

        [Fact]
        public async Task SpeakTextAndGetFilePathAsync_WithValidInput_ShouldCallInvokeAsync()
        {
            var serviceCollection = new ServiceCollection()
                .AddSemanticCore(Config.SEMANTIC_CONFIG);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var logger = Substitute.For<ILogger<TextToSpeechChain>>();
            var instance = new TextToSpeechChain(serviceProvider, logger);
            var input = "Hey there! I am an AI here to do your bidding. Do you have any coding questions for me?";

            var actual = await instance.SpeakTextAndGetFilePathAsync(input);

            Assert.NotNull(actual);
        }
    }
}
