using FrostAura.Libraries.Communication.Exceptions.Twilio;
using FrostAura.Libraries.Communication.Interfaces;
using FrostAura.Libraries.Communication.Models.Options;
using FrostAura.Libraries.Communication.Models.SMS;
using FrostAura.Libraries.Core.Extensions.Validation;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Twilio;
using Twilio.Clients;
using Twilio.Http;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace FrostAura.Libraries.Communication.Services.Twilio
{
    /// <summary>
    /// Twilio communicator for the SMS platform.
    /// </summary>
    public class TwilioSmsService : ISmsCommunicator
    {
        /// <summary>
        /// Twilio platform configuration.
        /// </summary>
        private readonly TwilioOptions _options;
        /// <summary>
        /// Http client.
        /// </summary>
        private readonly ITwilioRestClient _client;

        /// <summary>
        /// Initialite the client.
        /// </summary>
        /// <param name="config">Twilio platform configuration.</param>
        /// <param name="httpClient">Http client.</param>
        public TwilioSmsService(IOptions<TwilioOptions> config, System.Net.Http.HttpClient httpClient)
        {
            _options = config
                .ThrowIfNull(nameof(config))
                .Value
                .ThrowIfNull(nameof(config.Value));
            _client = new TwilioRestClient(
                _options.AccountSID,
                _options.AuthToken,
                region: _options.Region,
                httpClient: new SystemNetHttpClient(httpClient.ThrowIfNull(nameof(httpClient)))
            );
        }

        /// <summary>
        /// Send a message async.
        /// 
        /// Docs: https://www.twilio.com/code-exchange/receive-sms-twilio-phone-number
        /// </summary>
        /// <param name="message">Requested message to send.</param>
        /// <param name="token">Cancellation token.</param>
        /// <exception cref="FrostAura.Libraries.Communication.Exceptions.Twilio.TwilioSmsException">When something goes wrong.</exception>
        public Task<SmsResponse> SendAndWaitForResponseAsync(SmsRequest message, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            throw new NotImplementedException();
        }

        /// <summary>
        /// Send a message async and wait for a response.
        /// 
        /// Docs: https://www.twilio.com/code-exchange/sms-notifications
        /// </summary>
        /// <param name="message">Requested message to send.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>The response message, if any.</returns>
        /// <exception cref="FrostAura.Libraries.Communication.Exceptions.Twilio.TwilioSmsException">When something goes wrong.</exception>
        public async Task SendMessageAsync(SmsRequest message, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var response = await MessageResource.CreateAsync(
                to: new PhoneNumber(message.Destination),
                from: new PhoneNumber(message.From),
                body: message.Message,
                client: _client
            );

            if (response.Status == MessageResource.StatusEnum.Failed) throw new TwilioSmsException(response);
        }
    }
}
