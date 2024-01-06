using FrostAura.Libraries.Semantic.Core.Extensions.Configuration;
using FrostAura.Libraries.Semantic.Core.Thoughts.Cognitive;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using NSubstitute;
using Xunit;

namespace Semantic.Core.Tests.Thoughts.Cognitive
{
	public class LanguageModelThoughtsTests
    {
		[Fact]
		public void Constructor_WithInvalidKernel_ShouldThrow()
		{
            Kernel kernel = null;
            ILogger<LanguageModelThoughts> logger = Substitute.For<ILogger<LanguageModelThoughts>>();

            var actual = Assert.Throws<ArgumentNullException>(() => new LanguageModelThoughts(kernel, logger));

            Assert.Equal(nameof(kernel), actual.ParamName);
		}

        [Fact]
        public void Constructor_WithInvalidLogger_ShouldThrow()
        {
            Kernel kernel = Config.SEMANTIC_CONFIG.GetComprehensiveKernel();
            ILogger<LanguageModelThoughts> logger = null;

            var actual = Assert.Throws<ArgumentNullException>(() => new LanguageModelThoughts(kernel, logger));

            Assert.Equal(nameof(logger), actual.ParamName);
        }

        [Fact]
        public void Constructor_WithValidParams_ShouldConstruct()
        {
            var kernel = Config.SEMANTIC_CONFIG.GetComprehensiveKernel();
            var logger = Substitute.For<ILogger<LanguageModelThoughts>>();

            var actual = new LanguageModelThoughts(kernel, logger);

            Assert.NotNull(actual);
        }

        [Fact]
        public async Task PromptLLMAsync_WithInvalidInput_ShouldThrow()
        {
            var kernel = Config.SEMANTIC_CONFIG.GetComprehensiveKernel();
            var logger = Substitute.For<ILogger<LanguageModelThoughts>>();
            var instance = new LanguageModelThoughts(kernel, logger);
            string prompt = default;

            var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.PromptLLMAsync(prompt));

            Assert.Equal(nameof(prompt), actual.ParamName);
        }

        [Fact]
        public async Task PromptLLMAsync_WithValidInput_ShouldRespond()
        {
            var kernel = Config.SEMANTIC_CONFIG.GetComprehensiveKernel();
            var logger = Substitute.For<ILogger<LanguageModelThoughts>>();
            var instance = new LanguageModelThoughts(kernel, logger);
            var prompt = "Answer the following as directly as possible: 2+2=";

            var actual = await instance.PromptLLMAsync(prompt);

            Assert.Equal(4.ToString(), actual);
        }

        [Fact]
        public async Task PromptSmallLLMAsync_WithInvalidInput_ShouldThrow()
        {
            var kernel = Config.SEMANTIC_CONFIG.GetComprehensiveKernel();
            var logger = Substitute.For<ILogger<LanguageModelThoughts>>();
            var instance = new LanguageModelThoughts(kernel, logger);
            string prompt = default;

            var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.PromptSmallLLMAsync(prompt));

            Assert.Equal(nameof(prompt), actual.ParamName);
        }

        [Fact]
        public async Task PromptSmallLLMAsync_WithValidInput_ShouldRespond()
        {
            var kernel = Config.SEMANTIC_CONFIG.GetComprehensiveKernel();
            var logger = Substitute.For<ILogger<LanguageModelThoughts>>();
            var instance = new LanguageModelThoughts(kernel, logger);
            var prompt = "Answer the following as directly as possible: 2+2=";

            var actual = await instance.PromptSmallLLMAsync(prompt);

            Assert.Equal(4.ToString(), actual);
        }
    }
}
