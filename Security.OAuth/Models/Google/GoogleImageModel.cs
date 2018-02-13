namespace FrostAura.Libraries.Security.OAuth.Models.Google
{
    /// <summary>
    /// Entity that represents the profile image data we receive from the Google API.
    /// </summary>
    public class GoogleImageModel
    {
        /// <summary>
        /// Image URL.
        /// </summary>
        public string Url { get; set; }
        
        /// <summary>
        /// Whether or not this image is the profile default.
        /// </summary>
        public bool IsDefault { get; set; }
    }
}