using FrostAura.Libraries.Semantic.Core.Thoughts.IO;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Semantic.Core.Tests.Thoughts.IO
{
    public class WebDriverThoughtsTests
    {
		[Fact]
        public void Constructor_WithInvalidLogger_ShouldThrow()
        {
            ILogger<WebDriverThoughts> logger = null;

            var actual = Assert.Throws<ArgumentNullException>(() => new WebDriverThoughts(logger));

            Assert.Equal(nameof(logger), actual.ParamName);
        }

        [Fact]
        public void Constructor_WithValidParams_ShouldConstruct()
        {
            var logger = Substitute.For<ILogger<WebDriverThoughts>>();

            var actual = new WebDriverThoughts(logger);

            Assert.NotNull(actual);
        }

        [Fact]
        public async Task LoadTextAsync_WithInvalidInput_ShouldThrow()
        {
            var logger = Substitute.For<ILogger<WebDriverThoughts>>();
            var instance = new WebDriverThoughts(logger);
            string uri = default;

            var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.LoadTextAsync(uri));

            Assert.Equal(nameof(uri), actual.ParamName);
        }

        [Fact]
        public async Task LoadTextAsync_WithValidInput_ShouldRespond()
        {
            var logger = Substitute.For<ILogger<WebDriverThoughts>>();
            var instance = new WebDriverThoughts(logger);
            var uri = "https://techcrunch.com";

            var actual = await instance.LoadTextAsync(uri);

            Assert.NotEmpty(actual);
        }
    }
}
