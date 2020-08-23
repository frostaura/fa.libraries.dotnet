using FrostAura.Libraries.Communication.Models.Options;
using FrostAura.Libraries.Communication.Models.SMS;
using FrostAura.Libraries.Communication.Services.Twilio;
using Microsoft.Extensions.Options;
using NSubstitute;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FrostAura.Libraries.Communication.Tests.Integration.Services
{
    public class TwilioSmsServiceTests
    {
        [Fact]
        public async Task SendMessageAsync_WithValidParams_ShouldSendSms()
        {
            // Setup
            var options = Substitute.For<IOptions<TwilioOptions>>();
            var config = new TwilioOptions
            {
                AccountSID = "xxx",
                AuthToken = "yyy",
                Region = "Middle East & Africa"
            };

            options.Value.Returns(config);

            var communicator = new TwilioSmsService(options, new HttpClient());
            var request = new SmsRequest
            {
                From = "+12019847676",
                Destination = "+27718690000",
                Message = "Test message"
            };

            // Perform
            await communicator.SendMessageAsync(request, CancellationToken.None);
        }

        [Fact]
        public async Task SendAndWaitForResponseAsync_WithValidParams_ShouldSendAndReceiveSms()
        {
            // Setup
            var options = Substitute.For<IOptions<TwilioOptions>>();
            var config = new TwilioOptions
            {
                AccountSID = "xxx",
                AuthToken = "yyy",
                Region = "Middle East & Africa"
            };

            options.Value.Returns(config);

            var communicator = new TwilioSmsService(options, new HttpClient());
            var request = new SmsRequest
            {
                From = "+12019847676",
                Destination = "+27718690000",
                Message = "Test message"
            };

            // Perform
            await communicator.SendAndWaitForResponseAsync(request, CancellationToken.None);
        }
    }
}