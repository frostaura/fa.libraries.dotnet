namespace FrostAura.Libraries.Security.OAuth.Models.Google
{
    /// <summary>
    /// Response model for auth tokens.
    /// </summary>
    public class AuthTokenResponse
    {
        /// <summary>
        /// API access token for user.
        /// </summary>
        public string Access_token { get; set; }
    }
}