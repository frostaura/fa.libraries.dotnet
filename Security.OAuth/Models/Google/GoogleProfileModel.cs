using System.Collections.Generic;

namespace FrostAura.Libraries.Security.OAuth.Models.Google
{
    /// <summary>
    /// Entity that represents the data we can retrieve from the Google peoples API.
    /// </summary>
    public class GoogleProfileModel
    {
        /// <summary>
        /// User gender.
        /// </summary>
        public string Gender { get; set; }
        
        /// <summary>
        /// Collection of all user emails.
        /// </summary>
        public List<GoogleEmailModel> Emails { get; set; } = new List<GoogleEmailModel>();
        
        /// <summary>
        /// User names.
        /// </summary>
        public GoogleNameModel NameModel { get; set; }
        
        /// <summary>
        /// User images.
        /// </summary>
        public GoogleImageModel ImageModel { get; set; }
        
        /// <summary>
        /// User organizations collections.
        /// </summary>
        public List<GoogleOrganizationModel> Organizations { get; set; } = new List<GoogleOrganizationModel>();
    }
}