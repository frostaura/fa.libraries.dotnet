using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using NSubstitute;
using Semantic.Core.Functions.Semantic;
using Semantic.Core.Models.Configuration;
using Xunit;
using Semantic.Core.Extensions.Configuration;
using Semantic.Core.Constants.Functions;
using Microsoft.SemanticKernel.Memory;

namespace Semantic.Core.Tests.Functions.Semantic
{
	public class RecallFromMemoryTests
    {
		[Fact]
		public void Constructor_WithInvalidKernel_ShouldThrow()
		{
            IKernel kernel = null;
            ILogger<RecallFromMemory> logger = Substitute.For<ILogger<RecallFromMemory>>();

            var actual = Assert.Throws<ArgumentNullException>(() => new RecallFromMemory(kernel, logger));

            Assert.Equal(nameof(kernel), actual.ParamName);
		}

        [Fact]
        public void Constructor_WithInvalidLogger_ShouldThrow()
        {
            IKernel kernel = Substitute.For<IKernel>();
            ILogger<RecallFromMemory> logger = null;

            var actual = Assert.Throws<ArgumentNullException>(() => new RecallFromMemory(kernel, logger));

            Assert.Equal(nameof(logger), actual.ParamName);
        }

        [Fact]
        public void Constructor_WithValidParams_ShouldConstruct()
        {
            var kernel = Substitute.For<IKernel>();
            var logger = Substitute.For<ILogger<RecallFromMemory>>();

            var actual = new RecallFromMemory(kernel, logger);

            Assert.NotNull(actual);
        }

        [Fact]
        public async Task ExecuteAsync_WithInvalidInput_ShouldThrow()
        {
            var kernel = Config.SEMANTIC_CONFIG.GetComprehensiveKernel();
            var logger = Substitute.For<ILogger<RecallFromMemory>>();
            var instance = new RecallFromMemory(kernel, logger);
            var args = new Dictionary<string, string>();

            var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.ExecuteAsync(args));

            Assert.Equal(ArgumentNames.INPUT, actual.ParamName);
        }

        [Fact]
        public async Task ExecuteAsync_WithValidInput_ShouldRespond()
        {
            var kernel = Substitute.For<IKernel>();
            var memory = Substitute.For<ISemanticTextMemory>();
            kernel.Memory.ReturnsForAnyArgs(memory);
            var logger = Substitute.For<ILogger<RecallFromMemory>>();
            var instance = new RecallFromMemory(kernel, logger);
            string input = "What is my name?";
            var args = new Dictionary<string, string>
            {
                { ArgumentNames.INPUT, input }
            };

            var actual = await instance.ExecuteAsync(args);

            kernel
                .Memory
                .ReceivedWithAnyArgs(1)
                .SearchAsync(Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact]
        public async Task ExecuteAsync_WithValidInputAndStoreAndValue_ShouldRespond()
        {
            var kernel = Config.SEMANTIC_CONFIG.GetComprehensiveKernel();
            var logger = Substitute.For<ILogger<RecallFromMemory>>();
            var instance = new RecallFromMemory(kernel, logger);
            string input = "What is my name?";
            var args = new Dictionary<string, string>
            {
                { ArgumentNames.INPUT, input }
            };

            var actual = await instance.ExecuteAsync(args);

            Assert.NotEmpty(actual);
        }
    }
}
