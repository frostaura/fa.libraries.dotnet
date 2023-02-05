namespace FrostAura.Libraries.Security.OAuth.Models.Google
{
    /// <summary>
    /// Entity that represents the user's organizational data we receive from the Google API.
    /// </summary>
    public class GoogleOrganizationModel
    {
        /// <summary>
        /// Organication name.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Organization type.
        /// </summary>
        public string Type { get; set; }
        
        /// <summary>
        /// Whether or not this is the primary organization.
        /// </summary>
        public bool Primary { get; set; }
    }
}