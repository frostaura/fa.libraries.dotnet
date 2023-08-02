using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using NSubstitute;
using Semantic.Core.Functions.Semantic;
using Semantic.Core.Models.Configuration;
using Xunit;
using Semantic.Core.Extensions.Configuration;
using Semantic.Core.Constants.Functions;
using Semantic.Core.Functions.Procedural;
using Microsoft.Extensions.DependencyInjection;

namespace Semantic.Core.Tests.Functions.Semantic
{
	public class HttpGetTests
    {
        [Fact]
		public void Constructor_WithInvalidLogger_ShouldThrow()
		{
            ILogger<HttpGet> logger = null;

            var actual = Assert.Throws<ArgumentNullException>(() => new HttpGet(GetHttpClientFactory(), logger));

            Assert.Equal(nameof(logger), actual.ParamName);
		}

        [Fact]
        public void Constructor_WithInvalidHttpFactory_ShouldThrow()
        {
            ILogger<HttpGet> logger = null;
            IHttpClientFactory httpClientFactory = null;

            var actual = Assert.Throws<ArgumentNullException>(() => new HttpGet(httpClientFactory, logger));

            Assert.Equal(nameof(httpClientFactory), actual.ParamName);
        }

        [Fact]
        public void Constructor_WithValidParams_ShouldConstruct()
        {
            var logger = Substitute.For<ILogger<HttpGet>>();

            var actual = new HttpGet(GetHttpClientFactory(), logger);

            Assert.NotNull(actual);
        }

        [Fact]
        public async Task ExecuteAsync_WithInvalidInput_ShouldThrow()
        {
            var logger = Substitute.For<ILogger<HttpGet>>();
            var instance = new HttpGet(GetHttpClientFactory(), logger);
            var args = new Dictionary<string, string>();

            var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.ExecuteAsync(args));

            Assert.NotNull(actual);
            Assert.Equal(ArgumentNames.OUTPUT, actual.ParamName);
        }

        [Fact]
        public async Task ExecuteAsync_WithValidInput_ShouldSetContent()
        {
            var logger = Substitute.For<ILogger<HttpGet>>();
            var instance = new HttpGet(GetHttpClientFactory(), logger);
            var expected = "https://www.google.com.au/search?q=Latest%20trending%20book%20topics";
            var args = new Dictionary<string, string>
            {
                { ArgumentNames.INPUT, expected }
            };

            var actual = await instance.ExecuteAsync(args);

            Assert.NotEmpty(actual);
        }

        private IHttpClientFactory GetHttpClientFactory()
        {
            // Create an instance of IHttpClientFactory
            var services = new ServiceCollection();
            services.AddHttpClient();

            var serviceProvider = services.BuildServiceProvider();
            var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();

            return httpClientFactory;
        }
    }
}
