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

namespace Semantic.Core.Tests.Functions.Semantic.Memory
{
	public class CommitToMemoryTests
    {
		[Fact]
		public void Constructor_WithInvalidKernel_ShouldThrow()
		{
            IKernel kernel = null;
            ILogger<CommitToMemory> logger = Substitute.For<ILogger<CommitToMemory>>();

            var actual = Assert.Throws<ArgumentNullException>(() => new CommitToMemory(kernel, logger));

            Assert.Equal(nameof(kernel), actual.ParamName);
		}

        [Fact]
        public void Constructor_WithInvalidLogger_ShouldThrow()
        {
            IKernel kernel = Substitute.For<IKernel>();
            ILogger<CommitToMemory> logger = null;

            var actual = Assert.Throws<ArgumentNullException>(() => new CommitToMemory(kernel, logger));

            Assert.Equal(nameof(logger), actual.ParamName);
        }

        [Fact]
        public void Constructor_WithValidParams_ShouldConstruct()
        {
            var kernel = Substitute.For<IKernel>();
            var logger = Substitute.For<ILogger<CommitToMemory>>();

            var actual = new CommitToMemory(kernel, logger);

            Assert.NotNull(actual);
        }

        [Fact]
        public async Task ExecuteAsync_WithInvalidInput_ShouldThrow()
        {
            var kernel = Config.SEMANTIC_CONFIG.GetComprehensiveKernel();
            var logger = Substitute.For<ILogger<CommitToMemory>>();
            var instance = new CommitToMemory(kernel, logger);
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
            var logger = Substitute.For<ILogger<CommitToMemory>>();
            var instance = new CommitToMemory(kernel, logger);
            string input = new string(Enumerable.Range(0, 2200)
                .Select(_ => (char)(new Random().Next(32, 127)))
                .ToArray());
            var args = new Dictionary<string, string>
            {
                { ArgumentNames.INPUT, input },
                { ArgumentNames.SOURCE, "Test" }
            };

            var actual = await instance.ExecuteAsync(args);

            kernel
                .Memory
                .ReceivedWithAnyArgs(3)
                .SaveInformationAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact]
        public async Task ExecuteAsync_WithValidInputAndStore_ShouldRespond()
        {
            var kernel = Config.SEMANTIC_CONFIG.GetComprehensiveKernel();
            var logger = Substitute.For<ILogger<CommitToMemory>>();
            var instance = new CommitToMemory(kernel, logger);
            string input = new string(Enumerable.Range(0, 2200)
                .Select(_ => (char)(new Random().Next(32, 127)))
                .ToArray());
            var args = new Dictionary<string, string>
            {
                { ArgumentNames.INPUT, input },
                { ArgumentNames.SOURCE, "Test" }
            };

            var actual = await instance.ExecuteAsync(args);
        }

        [Fact]
        public async Task ExecuteAsync_WithValidInputAndStoreAndValue_ShouldRespond()
        {
            var kernel = Config.SEMANTIC_CONFIG.GetComprehensiveKernel();
            var logger = Substitute.For<ILogger<CommitToMemory>>();
            var instance = new CommitToMemory(kernel, logger);
            string input = "My full name is Deleo James Jonas";
            var args = new Dictionary<string, string>
            {
                { ArgumentNames.INPUT, input },
                { ArgumentNames.SOURCE, "Test" }
            };

            var actual = await instance.ExecuteAsync(args);
        }
    }
}
