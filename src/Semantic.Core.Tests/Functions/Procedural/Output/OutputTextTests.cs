using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using NSubstitute;
using Semantic.Core.Functions.Semantic;
using Semantic.Core.Models.Configuration;
using Xunit;
using Semantic.Core.Extensions.Configuration;
using Semantic.Core.Constants.Functions;
using Semantic.Core.Functions.Procedural;

namespace Semantic.Core.Tests.Functions.Semantic
{
	public class OutputTextTests
    {
		[Fact]
		public void Constructor_WithInvalidLogger_ShouldThrow()
		{
            ILogger<OutputText> logger = null;

            var actual = Assert.Throws<ArgumentNullException>(() => new OutputText(logger));

            Assert.Equal(nameof(logger), actual.ParamName);
		}

        [Fact]
        public void Constructor_WithValidParams_ShouldConstruct()
        {
            var logger = Substitute.For<ILogger<OutputText>>();

            var actual = new OutputText(logger);

            Assert.NotNull(actual);
        }

        [Fact]
        public async Task ExecuteAsync_WithInvalidInput_ShouldThrow()
        {
            var logger = Substitute.For<ILogger<OutputText>>();
            var instance = new OutputText(logger);
            var args = new Dictionary<string, string>();

            var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.ExecuteAsync(args));

            Assert.NotNull(actual);
            Assert.Equal(ArgumentNames.OUTPUT, actual.ParamName);
        }

        [Fact]
        public async Task ExecuteAsync_WithValidInput_ShouldSetContent()
        {
            var logger = Substitute.For<ILogger<OutputText>>();
            var instance = new OutputText(logger);
            var expected = "Test output value";
            var args = new Dictionary<string, string>
            {
                { ArgumentNames.OUTPUT, expected }
            };

            var actual = await instance.ExecuteAsync(args);

            Assert.NotNull(actual);
            Assert.Equal(expected, actual);
        }
    }
}
