namespace FrostAura.Libraries.Security.OAuth.Models.Google
{
    /// <summary>
    /// Entity that represents the email data we receive from the Google API.
    /// </summary>
    public class GoogleEmailModel
    {
        /// <summary>
        /// The actual value of the email.
        /// </summary>
        public string Value { get; set; }
        
        /// <summary>
        /// The tyope of email address.
        /// </summary>
        public string Type { get; set; }
    }
}