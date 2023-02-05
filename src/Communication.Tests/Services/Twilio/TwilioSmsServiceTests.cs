using FrostAura.Libraries.Communication.Models.Options;
using FrostAura.Libraries.Communication.Services.Twilio;
using Microsoft.Extensions.Options;
using NSubstitute;
using System;
using System.Net.Http;
using Xunit;

namespace Communication.Tests.Services.Twilio
{
    public class TwilioSmsServiceTests
    {
        [Fact]
        public void Constructor_WithInvalidOptions_ShouldThrow() 
        {
            var client = Substitute.For<HttpClient>();
            var actual = Assert.Throws<ArgumentNullException>(() => new TwilioSmsService(null, client));

            Assert.NotNull(actual);
            Assert.Equal("config", actual.ParamName);
        }

        [Fact]
        public void Constructor_WithInvalidOptionsValue_ShouldThrow()
        {
            var client = Substitute.For<HttpClient>();
            var options = Substitute.For<IOptions<TwilioOptions>>();

            options.Value.Returns(default(TwilioOptions));

            var actual = Assert.Throws<ArgumentNullException>(() => new TwilioSmsService(options, client));

            Assert.NotNull(actual);
            Assert.Equal("Value", actual.ParamName);
        }

        [Fact]
        public void Constructor_WithInvalidClient_ShouldThrow()
        {
            var options = Substitute.For<IOptions<TwilioOptions>>();
            var config = new TwilioOptions();

            options.Value.Returns(config);

            var actual = Assert.Throws<ArgumentNullException>(() => new TwilioSmsService(options, null));

            Assert.NotNull(actual);
            Assert.Equal("httpClient", actual.ParamName);
        }

        [Fact]
        public void Constructor_WithValidArguments_ShouldConstruct()
        {
            var client = Substitute.For<HttpClient>();
            var options = Substitute.For<IOptions<TwilioOptions>>();
            var config = new TwilioOptions();

            options.Value.Returns(config);

            var actual = new TwilioSmsService(options, client);

            Assert.NotNull(actual);
        }
    }
}
