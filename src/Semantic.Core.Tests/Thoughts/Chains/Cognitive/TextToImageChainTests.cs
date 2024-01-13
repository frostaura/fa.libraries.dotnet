using FrostAura.Libraries.Semantic.Core.Thoughts.Chains.Cognitive;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using FrostAura.Libraries.Semantic.Core.Extensions.Configuration;

namespace Semantic.Core.Tests.Thoughts.Chains.Cognitive
{
    public class TextToImageChainTests
    {
        [Fact]
        public void Constructor_WithInvalidLogger_ShouldThrow()
        {
            ILogger<TextToImageChain> logger = null;
            IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();

            var actual = Assert.Throws<ArgumentNullException>(() => new TextToImageChain(serviceProvider, logger));

            Assert.Equal(nameof(logger), actual.ParamName);
        }

        [Fact]
        public void Constructor_WithInvalidServiceProvider_ShouldThrow()
        {
            ILogger<TextToImageChain> logger = Substitute.For<ILogger<TextToImageChain>>();
            IServiceProvider serviceProvider = null;

            var actual = Assert.Throws<ArgumentNullException>(() => new TextToImageChain(serviceProvider, logger));

            Assert.Equal(nameof(serviceProvider), actual.ParamName);
        }

        [Fact]
        public void Constructor_WithValidParams_ShouldConstruct()
        {
            var serviceProvider = Substitute.For<IServiceProvider>();
            var logger = Substitute.For<ILogger<TextToImageChain>>();

            var actual = new TextToImageChain(serviceProvider, logger);

            Assert.NotNull(actual);
            Assert.NotEmpty(actual.QueryExample);
            Assert.NotEmpty(actual.QueryInputExample);
            Assert.NotEmpty(actual.Reasoning);
            Assert.NotEmpty(actual.ChainOfThoughts);
        }

        [Fact]
        public async Task GenerateImageAndGetFilePathAsync_WithInvalidInput_ShouldThrow()
        {
            var serviceCollection = new ServiceCollection()
                .AddSemanticCore(Config.SEMANTIC_CONFIG);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var logger = Substitute.For<ILogger<TextToImageChain>>();
            var instance = new TextToImageChain(serviceProvider, logger);
            string prompt = default;

            var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.GenerateImageAndGetFilePathAsync(prompt));

            Assert.Equal(nameof(prompt), actual.ParamName);
        }

        [Fact]
        public async Task GenerateImageAndGetFilePathAsync_WithValidInput_ShouldCallInvokeAsync()
        {
            var serviceCollection = new ServiceCollection()
                .AddSemanticCore(Config.SEMANTIC_CONFIG);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var logger = Substitute.For<ILogger<TextToImageChain>>();
            var instance = new TextToImageChain(serviceProvider, logger);
            var prompt = "A surfer in a hurricane, fighting off sharks that are on fire, photo-realistic, motion blur.";

            var actual = await instance.GenerateImageAndGetFilePathAsync(prompt);

            Assert.NotNull(actual);
        }
    }
}
