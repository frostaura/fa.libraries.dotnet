using Newtonsoft.Json;
using Twilio.Rest.Api.V2010.Account;

namespace FrostAura.Libraries.Communication.Exceptions.Twilio
{
    /// <summary>
    /// Exception for Twilio SMS communicator errors.
    /// </summary>
    public class TwilioSmsException : BaseCommunicatorException
    {
        public TwilioSmsException(MessageResource messageResponse)
            :base(messageResponse)
        { }
    }
}
