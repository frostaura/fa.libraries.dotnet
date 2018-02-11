namespace FrostAura.Libraries.Security.OAuth.Models.Google
{
    /// <summary>
    /// Entity that represents the email data we receive from the Google API.
    /// </summary>
    public class GoogleEmail
    {
        public string Value { get; set; }
        public string Type { get; set; }
    }
}