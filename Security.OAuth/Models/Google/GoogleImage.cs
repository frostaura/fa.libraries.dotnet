namespace FrostAura.Libraries.Security.OAuth.Models.Google
{
    /// <summary>
    /// Entity that represents the profile image data we receive from the Google API.
    /// </summary>
    public class GoogleImage
    {
        public string Url { get; set; }
        public bool IsDefault { get; set; }
    }
}