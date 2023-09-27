using FrostAura.Libraries.Semantic.Core.Models.Configuration;
using FrostAura.Libraries.Semantic.Core.Thoughts.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using FrostAura.Libraries.Semantic.Core.Extensions.Configuration;
using NSubstitute;
using Xunit;

namespace Semantic.Core.Tests.Thoughts.Finance
{
    public class FNBThoughtsTests
    {
        [Fact]
        public void Constructor_WithInvalidServiceProvider_ShouldThrow()
        {
            var fnbOptions = Substitute.For<IOptions<FNBConfig>>();
            fnbOptions.Value.Returns(Config.SEMANTIC_CONFIG.FNBConfig);
            ILogger<FNBThoughts> logger = Substitute.For<ILogger<FNBThoughts>>();
            IServiceProvider serviceProvider = null;

            var actual = Assert.Throws<ArgumentNullException>(() => new FNBThoughts(serviceProvider, fnbOptions, logger));

            Assert.Equal(nameof(serviceProvider), actual.ParamName);
        }

        [Fact]
        public void Constructor_WithInvalidFNBConfig_ShouldThrow()
        {
            IOptions<FNBConfig> fnbOptions = null;
            ILogger<FNBThoughts> logger = Substitute.For<ILogger<FNBThoughts>>();
            var serviceProvider = Substitute.For<IServiceProvider>();

            var actual = Assert.Throws<ArgumentNullException>(() => new FNBThoughts(serviceProvider, fnbOptions, logger));

            Assert.Equal(nameof(fnbOptions), actual.ParamName);
        }

        [Fact]
        public void Constructor_WithInvalidLogger_ShouldThrow()
        {
            var fnbOptions = Substitute.For<IOptions<FNBConfig>>();
            fnbOptions.Value.Returns(Config.SEMANTIC_CONFIG.FNBConfig);
            ILogger<FNBThoughts> logger = null;
            var serviceProvider = Substitute.For<IServiceProvider>();

            var actual = Assert.Throws<ArgumentNullException>(() => new FNBThoughts(serviceProvider, fnbOptions, logger));

            Assert.Equal(nameof(logger), actual.ParamName);
        }

        [Fact]
        public void Constructor_WithValidParams_ShouldConstruct()
        {
            var fnbOptions = Substitute.For<IOptions<FNBConfig>>();
            fnbOptions.Value.Returns(Config.SEMANTIC_CONFIG.FNBConfig);
            var logger = Substitute.For<ILogger<FNBThoughts>>();
            var serviceCollection = new ServiceCollection()
                .AddSemanticCore(Config.SEMANTIC_CONFIG);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var actual = new FNBThoughts(serviceProvider, fnbOptions, logger);

            Assert.NotNull(actual);
        }

        [Fact]
        public async Task LoadTextAsync_WithValidInput_ShouldRespond()
        {
            var fnbOptions = Substitute.For<IOptions<FNBConfig>>();
            fnbOptions.Value.Returns(Config.SEMANTIC_CONFIG.FNBConfig);
            var logger = Substitute.For<ILogger<FNBThoughts>>();
            var serviceCollection = new ServiceCollection()
                .AddSemanticCore(Config.SEMANTIC_CONFIG);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var instance = new FNBThoughts(serviceProvider, fnbOptions, logger);

            var actual = await instance.GetFNBAccountBalancesAsync();

            Assert.NotEmpty(actual);
        }
    }
}
