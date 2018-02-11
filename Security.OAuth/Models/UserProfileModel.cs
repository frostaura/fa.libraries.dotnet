namespace FrostAura.Libraries.Security.OAuth.Models
{
    /// <summary>
    /// Generic user profile model.
    /// </summary>
    public class UserProfileModel
    {
        /// <summary>
        /// User first name.
        /// </summary>
        public string FirstName { get; set; }
        
        /// <summary>
        /// User last name.
        /// </summary>
        public string Lastname { get; set; }
        
        /// <summary>
        /// Fully qualified user profile image URL.
        /// </summary>
        public string ProfileImageUrl { get; set; }
        
        /// <summary>
        /// User email address.
        /// </summary>
        public string Email { get; set; }
    }
}