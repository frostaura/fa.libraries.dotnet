using FrostAura.Libraries.Semantic.Core.Thoughts.Chains.Cognitive;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using FrostAura.Libraries.Semantic.Core.Extensions.Configuration;

namespace Semantic.Core.Tests.Thoughts.Chains.Cognitive
{
    public class MemoryCommitChainTests
    {
        [Fact]
        public void Constructor_WithInvalidLogger_ShouldThrow()
        {
            ILogger<MemoryCommitChain> logger = null;
            IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();

            var actual = Assert.Throws<ArgumentNullException>(() => new MemoryCommitChain(serviceProvider, logger));

            Assert.Equal(nameof(logger), actual.ParamName);
        }

        [Fact]
        public void Constructor_WithInvalidServiceProvider_ShouldThrow()
        {
            ILogger<MemoryCommitChain> logger = Substitute.For<ILogger<MemoryCommitChain>>();
            IServiceProvider serviceProvider = null;

            var actual = Assert.Throws<ArgumentNullException>(() => new MemoryCommitChain(serviceProvider, logger));

            Assert.Equal(nameof(serviceProvider), actual.ParamName);
        }

        [Fact]
        public void Constructor_WithValidParams_ShouldConstruct()
        {
            var serviceProvider = Substitute.For<IServiceProvider>();
            var logger = Substitute.For<ILogger<MemoryCommitChain>>();

            var actual = new MemoryCommitChain(serviceProvider, logger);

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
            var logger = Substitute.For<ILogger<MemoryCommitChain>>();
            var instance = new MemoryCommitChain(serviceProvider, logger);
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
            var logger = Substitute.For<ILogger<MemoryCommitChain>>();
            var instance = new MemoryCommitChain(serviceProvider, logger);
            var input = "I love cats!";
            var expected = "Fantastic! I will remember that for future reference.";

            var actual = await instance.ExecuteChainAsync(input);

            Assert.Equal(expected, actual);
        }
    }
}
