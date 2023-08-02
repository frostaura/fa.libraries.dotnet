using FrostAura.Libraries.Semantic.Core.Thoughts.Chains.Cognitive;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using FrostAura.Libraries.Semantic.Core.Extensions.Configuration;

namespace Semantic.Core.Tests.Thoughts.Chains.Cognitive
{
    public class MemoryRecallChainTests
    {
        [Fact]
        public void Constructor_WithInvalidLogger_ShouldThrow()
        {
            ILogger<MemoryRecallChain> logger = null;
            IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();

            var actual = Assert.Throws<ArgumentNullException>(() => new MemoryRecallChain(serviceProvider, logger));

            Assert.Equal(nameof(logger), actual.ParamName);
        }

        [Fact]
        public void Constructor_WithInvalidServiceProvider_ShouldThrow()
        {
            ILogger<MemoryRecallChain> logger = Substitute.For<ILogger<MemoryRecallChain>>();
            IServiceProvider serviceProvider = null;

            var actual = Assert.Throws<ArgumentNullException>(() => new MemoryRecallChain(serviceProvider, logger));

            Assert.Equal(nameof(serviceProvider), actual.ParamName);
        }

        [Fact]
        public void Constructor_WithValidParams_ShouldConstruct()
        {
            var serviceProvider = Substitute.For<IServiceProvider>();
            var logger = Substitute.For<ILogger<MemoryRecallChain>>();

            var actual = new MemoryRecallChain(serviceProvider, logger);

            Assert.NotNull(actual);
            Assert.NotEmpty(actual.ExampleChallange);
            Assert.NotEmpty(actual.Reasoning);
            Assert.NotEmpty(actual.ChainOfThoughts);
        }

        [Fact]
        public async Task ExecuteChainAsync_WithInvalidInput_ShouldThrow()
        {
            var serviceProvider = Substitute.For<IServiceProvider>();
            var logger = Substitute.For<ILogger<MemoryRecallChain>>();
            var instance = new MemoryRecallChain(serviceProvider, logger);
            string input = default;

            var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.ExecuteChainAsync(input));

            Assert.Equal(nameof(input), actual.ParamName);
        }

        [Fact]
        public async Task ExecuteChainAsync_WithValidInput_ShouldCallCommitToMemoryAsync()
        {
            var serviceCollection = new ServiceCollection()
                .AddSemanticCore(Config.SEMANTIC_CONFIG);

            // Inject depdencies.

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var logger = Substitute.For<ILogger<MemoryRecallChain>>();
            var instance = new MemoryRecallChain(serviceProvider, logger);
            var input = "How do I feel about cats?";

            var actual = await instance.ExecuteChainAsync(input);

            Assert.NotEmpty(actual);
        }
    }
}
