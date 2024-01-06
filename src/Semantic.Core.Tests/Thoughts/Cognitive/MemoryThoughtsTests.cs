using FrostAura.Libraries.Semantic.Core.Extensions.Configuration;
using FrostAura.Libraries.Semantic.Core.Thoughts.Cognitive;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Memory;
using NSubstitute;
using Xunit;

namespace Semantic.Core.Tests.Thoughts.Cognitive
{
	public class MemoryThoughtsTests
    {
		[Fact]
		public void Constructor_WithInvalidKernel_ShouldThrow()
		{
            Kernel kernel = null;
            ILogger<MemoryThoughts> logger = Substitute.For<ILogger<MemoryThoughts>>();

            var actual = Assert.Throws<ArgumentNullException>(() => new MemoryThoughts(kernel, logger));

            Assert.Equal(nameof(kernel), actual.ParamName);
		}

        [Fact]
        public void Constructor_WithInvalidLogger_ShouldThrow()
        {
            var kernel = Config.SEMANTIC_CONFIG.GetComprehensiveKernel();
            ILogger<MemoryThoughts> logger = null;

            var actual = Assert.Throws<ArgumentNullException>(() => new MemoryThoughts(kernel, logger));

            Assert.Equal(nameof(logger), actual.ParamName);
        }

        [Fact]
        public void Constructor_WithValidParams_ShouldConstruct()
        {
            var kernel = Config.SEMANTIC_CONFIG.GetComprehensiveKernel();
            var logger = Substitute.For<ILogger<MemoryThoughts>>();

            var actual = new MemoryThoughts(kernel, logger);

            Assert.NotNull(actual);
        }

        [Fact]
        public async Task CommitToMemoryAsync_WithInvalidMemory_ShouldThrow()
        {
            var kernel = Config.SEMANTIC_CONFIG.GetComprehensiveKernel();
            var logger = Substitute.For<ILogger<MemoryThoughts>>();
            var instance = new MemoryThoughts(kernel, logger);
            string memory = default;
            string source = "test source";

            var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.CommitToMemoryAsync(memory, source));

            Assert.Equal(nameof(memory), actual.ParamName);
        }

        [Fact]
        public async Task CommitToMemoryAsync_WithInvalidSource_ShouldThrow()
        {
            var kernel = Config.SEMANTIC_CONFIG.GetComprehensiveKernel();
            var logger = Substitute.For<ILogger<MemoryThoughts>>();
            var instance = new MemoryThoughts(kernel, logger);
            string memory = "test Memory";
            string source = default;

            var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.CommitToMemoryAsync(memory, source));

            Assert.Equal(nameof(source), actual.ParamName);
        }

        [Fact]
        public async Task CommitToMemoryAsync_WithValidInput_ShouldRespond()
        {
            var memory = Substitute.For<ISemanticTextMemory>();
            var kernel = Config.SEMANTIC_CONFIG.GetComprehensiveKernel(memory);

            var logger = Substitute.For<ILogger<MemoryThoughts>>();
            var instance = new MemoryThoughts(kernel, logger);
            string input = new string(Enumerable.Range(0, 2200)
                .Select(_ => (char)(new Random().Next(32, 127)))
                .ToArray());

            var actual = await instance.CommitToMemoryAsync(input, "Test");

            memory
                .ReceivedWithAnyArgs(3)
                .SaveInformationAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact]
        public async Task RecallFromMemoryAsync_WithInvalidMemory_ShouldThrow()
        {
            var kernel = Config.SEMANTIC_CONFIG.GetComprehensiveKernel();
            var logger = Substitute.For<ILogger<MemoryThoughts>>();
            var instance = new MemoryThoughts(kernel, logger);
            string query = default;

            var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.RecallFromMemoryAsync(query));

            Assert.Equal(nameof(query), actual.ParamName);
        }

        [Fact]
        public async Task RecallFromMemoryAsync_WithValidInput_ShouldRespond()
        {
            var memory = Substitute.For<ISemanticTextMemory>();
            var kernel = Config.SEMANTIC_CONFIG.GetComprehensiveKernel(memory);

            var logger = Substitute.For<ILogger<MemoryThoughts>>();
            var instance = new MemoryThoughts(kernel, logger);
            string input = "What is my name?";

            var actual = await instance.RecallFromMemoryAsync(input);

            memory
                .ReceivedWithAnyArgs(1)
                .SearchAsync(Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact]
        public async Task RecallFromMemoryAsync_WithValidInputAndStoreAndValue_ShouldRespond()
        {
            var kernel = Config.SEMANTIC_CONFIG.GetComprehensiveKernel();
            var logger = Substitute.For<ILogger<MemoryThoughts>>();
            var instance = new MemoryThoughts(kernel, logger);
            string input = "What is my name?";

            var actual = await instance.RecallFromMemoryAsync(input);

            Assert.NotEmpty(actual);
        }
    }
}
