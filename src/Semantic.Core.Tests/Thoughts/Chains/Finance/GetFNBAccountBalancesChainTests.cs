using FrostAura.Libraries.Semantic.Core.Thoughts.Chains.Cognitive;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using FrostAura.Libraries.Semantic.Core.Extensions.Configuration;

namespace Semantic.Core.Tests.Thoughts.Chains.Finance
{
    public class GetFNBAccountBalancesChainTests
    {
        [Fact]
        public void Constructor_WithInvalidLogger_ShouldThrow()
        {
            ILogger<GetFNBAccountBalancesChain> logger = null;
            IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();

            var actual = Assert.Throws<ArgumentNullException>(() => new GetFNBAccountBalancesChain(serviceProvider, logger));

            Assert.Equal(nameof(logger), actual.ParamName);
        }

        [Fact]
        public void Constructor_WithInvalidServiceProvider_ShouldThrow()
        {
            ILogger<GetFNBAccountBalancesChain> logger = Substitute.For<ILogger<GetFNBAccountBalancesChain>>();
            IServiceProvider serviceProvider = null;

            var actual = Assert.Throws<ArgumentNullException>(() => new GetFNBAccountBalancesChain(serviceProvider, logger));

            Assert.Equal(nameof(serviceProvider), actual.ParamName);
        }

        [Fact]
        public void Constructor_WithValidParams_ShouldConstruct()
        {
            var serviceCollection = new ServiceCollection()
                .AddSemanticCore(Config.SEMANTIC_CONFIG);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var logger = Substitute.For<ILogger<GetFNBAccountBalancesChain>>();

            var actual = new GetFNBAccountBalancesChain(serviceProvider, logger);

            Assert.NotNull(actual);
            Assert.NotEmpty(actual.QueryExample);
            Assert.NotEmpty(actual.Reasoning);
            Assert.NotEmpty(actual.ChainOfThoughts);
        }

        [Fact(Skip = "Integration Test")]
        public async Task ExecuteChainAsync_WithValidInput_ShouldCallInvokeAsyncAsync()
        {
            var serviceCollection = new ServiceCollection()
                .AddSemanticCore(Config.SEMANTIC_CONFIG);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var logger = Substitute.For<ILogger<GetFNBAccountBalancesChain>>();
            var instance = new GetFNBAccountBalancesChain(serviceProvider, logger);

            var actual = await instance.ExecuteChainAsync();

            Assert.NotEmpty(actual);
        }
    }
}
