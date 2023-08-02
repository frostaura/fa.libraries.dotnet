using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using NSubstitute;
using Semantic.Core.Functions.Semantic;
using Semantic.Core.Models.Configuration;
using Xunit;
using Semantic.Core.Extensions.Configuration;
using Semantic.Core.Constants.Functions;
using Microsoft.SemanticKernel.Memory;
using Semantic.Core.Functions.Semantic.Memory;
using Semantic.Core.Functions.Semantic.Agents;
using Microsoft.Extensions.DependencyInjection;

namespace Semantic.Core.Tests.Functions.Semantic.Memory
{
	public class ReActAgentTests
    {
		[Fact]
		public void Constructor_WithInvalidKernel_ShouldThrow()
		{
            IKernel kernel = null;
            ILogger<ReActAgent> logger = Substitute.For<ILogger<ReActAgent>>();
            IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();

            var actual = Assert.Throws<ArgumentNullException>(() => new ReActAgent(serviceProvider, kernel, logger));

            Assert.Equal(nameof(kernel), actual.ParamName);
		}

        [Fact]
        public void Constructor_WithInvalidLogger_ShouldThrow()
        {
            IKernel kernel = Substitute.For<IKernel>();
            ILogger<ReActAgent> logger = null;
            IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();

            var actual = Assert.Throws<ArgumentNullException>(() => new ReActAgent(serviceProvider, kernel, logger));

            Assert.Equal(nameof(logger), actual.ParamName);
        }

        [Fact]
        public void Constructor_WithInvalidServiceProvider_ShouldThrow()
        {
            IKernel kernel = Substitute.For<IKernel>();
            ILogger<ReActAgent> logger = Substitute.For<ILogger<ReActAgent>>();
            IServiceProvider serviceProvider = null;

            var actual = Assert.Throws<ArgumentNullException>(() => new ReActAgent(serviceProvider, kernel, logger));

            Assert.Equal(nameof(serviceProvider), actual.ParamName);
        }

        [Fact]
        public void Constructor_WithValidParams_ShouldConstruct()
        {
            var kernel = Substitute.For<IKernel>();
            var logger = Substitute.For<ILogger<ReActAgent>>();
            var serviceProvider = new ServiceCollection()
                .AddSemanticCore(Config.SEMANTIC_CONFIG)
                .BuildServiceProvider();

            var actual = new ReActAgent(serviceProvider, kernel, logger);

            Assert.NotNull(actual);
        }

        [Fact]
        public async Task RunAsync_WithInvalidInput_ShouldThrow()
        {
            var kernel = Config.SEMANTIC_CONFIG.GetComprehensiveKernel();
            var logger = Substitute.For<ILogger<ReActAgent>>();
            var serviceProvider = new ServiceCollection()
                .AddSemanticCore(Config.SEMANTIC_CONFIG)
                .BuildServiceProvider();
            var instance = new ReActAgent(serviceProvider, kernel, logger);
            var args = new Dictionary<string, string>();

            var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.RunAsync(args));

            Assert.Equal(ArgumentNames.INPUT, actual.ParamName);
        }

        [Theory]
        [InlineData("Whats the latest trending movie?")]
        public async Task RunAsync_WithValidInput_ShouldRespond(string input)
        {
            var kernel = Config.SEMANTIC_CONFIG.GetComprehensiveKernel();
            var logger = Substitute.For<ILogger<ReActAgent>>();
            var serviceProvider = new ServiceCollection()
                .AddSemanticCore(Config.SEMANTIC_CONFIG)
                .BuildServiceProvider();
            var instance = new ReActAgent(serviceProvider, kernel, logger);
            var args = new Dictionary<string, string>
            {
                { ArgumentNames.INPUT, input }
            };

            var actual = await instance.RunAsync(args);

            Assert.NotEmpty(actual);
        }
    }
}
