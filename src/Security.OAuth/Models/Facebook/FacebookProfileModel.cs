namespace FrostAura.Libraries.Security.OAuth.Models.Facebook
{
    /// <summary>
    /// Entity that represents the data we can retrieve from the Facebook peoples API
    /// </summary>
    public class FacebookProfile
    {
        /// <summary>
        /// Unique user identifier.
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// User email address.
        /// </summary>
        public string Email { get; set; }
        
        /// <summary>
        /// User first name.
        /// </summary>
        public string First_name { get; set; }
        
        /// <summary>
        /// User gender.
        /// </summary>
        public string Gender { get; set; }
        
        /// <summary>
        /// User last name
        /// </summary>
        public string Last_name { get; set; }

        /// <summary>
        /// Profile image URL.
        /// </summary>
        public FacebookPictureModel Picture { get; set; }
    }
}