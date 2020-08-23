namespace FrostAura.Libraries.Communication.Models.SMS
{
    /// <summary>
    /// Base sms request model.
    /// </summary>
    public class SmsRequest
    {
        /// <summary>
        /// Where the message originated.
        /// </summary>
        public string From { get; set; }
        /// <summary>
        /// Where the message is intended to go.
        /// </summary>
        public string Destination { get; set; }
        /// <summary>
        /// The content or body of the message.
        /// </summary>
        public string Message { get; set; }
    }
}
