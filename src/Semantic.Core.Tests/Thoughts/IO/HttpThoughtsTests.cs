using FrostAura.Libraries.Semantic.Core.Thoughts.IO;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Semantic.Core.Tests.Thoughts.IO
{
    public class HttpThoughtsTests
    {
		[Fact]
		public void Constructor_WithInvalidHttpClientFactory_ShouldThrow()
		{
            IHttpClientFactory httpClientFactory = null;
            ILogger<HttpThoughts> logger = Substitute.For<ILogger<HttpThoughts>>();

            var actual = Assert.Throws<ArgumentNullException>(() => new HttpThoughts(httpClientFactory, logger));

            Assert.Equal(nameof(httpClientFactory), actual.ParamName);
		}

        [Fact]
        public void Constructor_WithInvalidLogger_ShouldThrow()
        {
            IHttpClientFactory httpClientFactory = Substitute.For<IHttpClientFactory>();
            ILogger<HttpThoughts> logger = null;

            var actual = Assert.Throws<ArgumentNullException>(() => new HttpThoughts(httpClientFactory, logger));

            Assert.Equal(nameof(logger), actual.ParamName);
        }

        [Fact]
        public void Constructor_WithValidParams_ShouldConstruct()
        {
            var httpClientFactory = Substitute.For<IHttpClientFactory>();
            var logger = Substitute.For<ILogger<HttpThoughts>>();

            var actual = new HttpThoughts(httpClientFactory, logger);

            Assert.NotNull(actual);
        }

        [Fact]
        public async Task GetAsync_WithInvalidInput_ShouldThrow()
        {
            var httpClientFactory = Substitute.For<IHttpClientFactory>();
            var logger = Substitute.For<ILogger<HttpThoughts>>();
            var instance = new HttpThoughts(httpClientFactory, logger);
            string uri = default;

            var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.GetAsync(uri));

            Assert.Equal(nameof(uri), actual.ParamName);
        }

        [Fact]
        public async Task GetAsync_WithValidInput_ShouldRespond()
        {
            var httpClientFactory = Substitute.For<IHttpClientFactory>();
            httpClientFactory.CreateClient().Returns(new HttpClient());
            var logger = Substitute.For<ILogger<HttpThoughts>>();
            var instance = new HttpThoughts(httpClientFactory, logger);
            var uri = "https://www.google.com.au/search?q=Latest%20trending%20book%20topics";

            var actual = await instance.GetAsync(uri);

            Assert.NotEmpty(actual);
        }
    }
}
