using FrostAura.Libraries.Semantic.Core.Thoughts.IO;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Semantic.Core.Tests.Thoughts.System
{
    public class OutputThoughtsTests
    {
		[Fact]
        public void Constructor_WithInvalidLogger_ShouldThrow()
        {
            ILogger<HttpThoughts> logger = null;

            var actual = Assert.Throws<ArgumentNullException>(() => new OutputThoughts(logger));

            Assert.Equal(nameof(logger), actual.ParamName);
        }

        [Fact]
        public void Constructor_WithValidParams_ShouldConstruct()
        {
            var logger = Substitute.For<ILogger<HttpThoughts>>();

            var actual = new OutputThoughts(logger);

            Assert.NotNull(actual);
        }

        [Fact]
        public async Task GetAsync_WithInvalidInput_ShouldThrow()
        {
            var logger = Substitute.For<ILogger<HttpThoughts>>();
            var instance = new OutputThoughts(logger);
            string output = default;

            var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.OutputTextAsync(output));

            Assert.Equal(nameof(output), actual.ParamName);
        }

        [Fact]
        public async Task GetAsync_WithInput_ShouldReturnCorrectValue()
        {
            var logger = Substitute.For<ILogger<HttpThoughts>>();
            var instance = new OutputThoughts(logger);
            string output = "expected output";

            var actual = await instance.OutputTextAsync(output);

            Assert.Equal(output, actual);
        }
    }
}
