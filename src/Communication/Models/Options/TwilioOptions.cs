namespace FrostAura.Libraries.Communication.Models.Options
{
    /// <summary>
    /// Model for storing Twilio platform options.
    /// </summary>
    public class TwilioOptions
    {
        /// <summary>
        /// Unique account id from the platform.
        /// </summary>
        public string AccountSID { get; set; }
        /// <summary>
        /// Auth token to use for accessing the platform.
        /// </summary>
        public string AuthToken { get; set; }
        /// <summary>
        /// Destination region.
        /// </summary>
        public string Region { get; set; }
    }
}
