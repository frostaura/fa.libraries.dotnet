namespace FrostAura.Libraries.Security.OAuth.Models.Facebook
{
    /// <summary>
    /// Facebook profile image data.
    /// </summary>
    public class FacebookPictureDataModel
    {
        /// <summary>
        /// Image height.
        /// </summary>
        public int Height { get; set; }
        
        public bool Is_silhouette { get; set; }
        
        /// <summary>
        /// Image URL.
        /// </summary>
        public string Url { get; set; }
        
        /// <summary>
        /// Image width.
        /// </summary>
        public int Width { get; set; }
    }
}