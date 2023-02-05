using System.ComponentModel.DataAnnotations;

namespace FrostAura.Libraries.Security.OAuth.Models
{
    /// <summary>
    /// Generic user profile model.
    /// </summary>
    public class UserProfileModel
    {
        /// <summary>
        /// Unique user identifier for the social platform.
        /// </summary>
        [Required]
        public string SocialId { get; set; }
        
        /// <summary>
        /// User first name.
        /// </summary>
        [Required]
        public string FirstName { get; set; }
        
        /// <summary>
        /// User last name.
        /// </summary>
        [Required]
        public string Lastname { get; set; }
        
        /// <summary>
        /// Fully qualified user profile image URL.
        /// </summary>
        public string ProfileImageUrl { get; set; }
        
        /// <summary>
        /// User email address.
        /// </summary>
        [Required]
        public string Email { get; set; }

        /// <summary>
        /// Original provider specific profile.
        /// </summary>
        [Required]
        public object ProviderSpecificProfile { get; set; }
    }
}