namespace FrostAura.Libraries.Security.OAuth.Models.LinkedIn
{
    /// <summary>
    /// Entity that represents the data we can retrieve from the LinkedIn peoples API
    /// </summary>
    public class LinkedInProfileModel
    {
        /// <summary>
        /// Unique user identifier.
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// User email address.
        /// </summary>
        public string EmailAddress { get; set; }
        
        /// <summary>
        /// User first name.
        /// </summary>
        public string FirstName { get; set; }
        
        /// <summary>
        /// User last name
        /// </summary>
        public string LastName { get; set; }
        
        /// <summary>
        /// User profile image URL.
        /// </summary>
        public string PictureUrl { get; set; }
        
        /// <summary>
        /// User biography.
        /// </summary>
        public string Summary { get; set; }
    }
}